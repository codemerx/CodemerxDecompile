using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Mono.Cecil.Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.XmlDocumentationReaders;

namespace Telerik.JustDecompiler.Languages
{
	public abstract class BaseLanguageWriter : BaseCodeVisitor, ILanguageWriter, IExceptionThrownNotifier
	{
		protected bool isStopped;

		protected IFormatter formatter;

		protected Stack<IMemberDefinition> membersStack = new Stack<IMemberDefinition>();

		protected bool isWritingComment;

		protected WriterContext writerContext;

		protected IWriterContextService writerContextService;

		protected IExceptionFormatter exceptionFormatter;

		protected WritingInfo currentWritingInfo;

		protected List<WritingInfo> writingInfos;

		protected virtual AssemblySpecificContext AssemblyContext
		{
			get
			{
				return this.writerContext.AssemblyContext;
			}
		}

		protected MethodDefinition CurrentMethod
		{
			get
			{
				if (this.membersStack.Count <= 0 || !(this.membersStack.Peek() is MethodDefinition))
				{
					return null;
				}
				return this.membersStack.Peek() as MethodDefinition;
			}
		}

		protected TypeDefinition CurrentType
		{
			get;
			set;
		}

		public ILanguage Language
		{
			get;
			private set;
		}

		protected MethodSpecificContext MethodContext
		{
			get
			{
				if (this.membersStack.Count <= 0)
				{
					return null;
				}
				return this.GetMethodContext(this.membersStack.Peek());
			}
		}

		protected bool MethodContextsMissing
		{
			get
			{
				return this.writerContext.MethodContexts.Count == 0;
			}
		}

		protected virtual ModuleSpecificContext ModuleContext
		{
			get
			{
				return this.writerContext.ModuleContext;
			}
		}

		protected IWriterSettings Settings
		{
			get;
			private set;
		}

		protected virtual TypeSpecificContext TypeContext
		{
			get
			{
				return this.writerContext.TypeContext;
			}
		}

		public BaseLanguageWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
		{
			this.Language = language;
			this.formatter = formatter;
			this.exceptionFormatter = exceptionFormatter;
			this.Settings = settings;
		}

		protected void AddMemberAttributesOffsetSpan(uint memberToken, int startIndex, int endIndex)
		{
			if (startIndex < endIndex)
			{
				this.currentWritingInfo.MemberTokenToAttributesMap.Add(memberToken, new OffsetSpan(startIndex, endIndex - 1));
			}
		}

		protected void AddMemberDocumentationOffsetSpan(uint memberToken, int startIndex, int endIndex)
		{
			if (startIndex < endIndex)
			{
				this.currentWritingInfo.MemberTokenToDocumentationMap.Add(memberToken, new OffsetSpan(startIndex, endIndex - 1));
			}
		}

		internal static string ConvertChar(char ch)
		{
			int num;
			switch (ch)
			{
				case '\0':
				{
					return "\\0";
				}
				case '\u0001':
				case '\u0002':
				case '\u0003':
				case '\u0004':
				case '\u0005':
				case '\u0006':
				{
					if (!Char.IsControl(ch) && !Char.IsSurrogate(ch) && (!Char.IsWhiteSpace(ch) || ch == ' '))
					{
						return ch.ToString();
					}
					num = ch;
					return String.Concat("\\u", num.ToString("x4"));
				}
				case '\a':
				{
					return "\\a";
				}
				case '\b':
				{
					return "\\b";
				}
				case '\t':
				{
					return "\\t";
				}
				case '\n':
				{
					return "\\n";
				}
				case '\v':
				{
					return "\\v";
				}
				case '\f':
				{
					return "\\f";
				}
				case '\r':
				{
					return "\\r";
				}
				default:
				{
					if (ch != '\\')
					{
						if (!Char.IsControl(ch) && !Char.IsSurrogate(ch) && (!Char.IsWhiteSpace(ch) || ch == ' '))
						{
							return ch.ToString();
						}
						num = ch;
						return String.Concat("\\u", num.ToString("x4"));
					}
					return "\\\\";
				}
			}
		}

		internal static string ConvertString(string str)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string str1 = str;
			for (int i = 0; i < str1.Length; i++)
			{
				char chr = str1[i];
				if (chr != '\"')
				{
					stringBuilder.Append(BaseLanguageWriter.ConvertChar(chr));
				}
				else
				{
					stringBuilder.Append("\\\"");
				}
			}
			return stringBuilder.ToString();
		}

		protected virtual void EndWritingComment()
		{
			this.isWritingComment = false;
			this.formatter.EndWritingComment();
		}

		protected string GetCurrentModuleTypeName(TypeReference type)
		{
			TypeDefinition typeDefinition = type.Resolve();
			return this.ModuleContext.RenamedMembersMap[typeDefinition.MetadataToken.ToUInt32()];
		}

		protected virtual string GetEventName(EventReference @event)
		{
			EventDefinition eventDefinition = @event.Resolve();
			if (eventDefinition == null || !(eventDefinition.Module.FilePath == this.ModuleContext.Module.FilePath))
			{
				return Utilities.EscapeNameIfNeeded(GenericHelper.GetNonGenericName(@event.Name), this.Language);
			}
			return this.ModuleContext.RenamedMembersMap[eventDefinition.MetadataToken.ToUInt32()];
		}

		protected virtual string GetFieldName(FieldReference field)
		{
			FieldDefinition fieldDefinition = field.Resolve();
			if (fieldDefinition == null || !(fieldDefinition.Module.FilePath == this.ModuleContext.Module.FilePath))
			{
				return Utilities.EscapeNameIfNeeded(GenericHelper.GetNonGenericName(field.Name), this.Language);
			}
			if (this.TypeContext != null && this.TypeContext.BackingFieldToNameMap.ContainsKey(fieldDefinition))
			{
				return this.TypeContext.BackingFieldToNameMap[fieldDefinition];
			}
			return this.ModuleContext.RenamedMembersMap[fieldDefinition.MetadataToken.ToUInt32()];
		}

		private string GetGenericParameterName(GenericParameter genericParameter)
		{
			int position;
			if (genericParameter.Owner is TypeReference)
			{
				position = genericParameter.Position;
				return String.Concat("!", position.ToString());
			}
			position = genericParameter.Position;
			return String.Concat("!!", position.ToString());
		}

		protected string GetMemberEscapedOnlyName(object memberDefinition)
		{
			if (memberDefinition == null)
			{
				return String.Empty;
			}
			if (memberDefinition is TypeReference)
			{
				TypeReference elementType = ((TypeReference)memberDefinition).GetElementType();
				if (elementType.IsGenericParameter)
				{
					return this.GetGenericParameterName((GenericParameter)elementType);
				}
				if (elementType.HasGenericParameters)
				{
					StringBuilder stringBuilder = new StringBuilder(this.Language.ReplaceInvalidCharactersInIdentifier(elementType.Name));
					this.NormalizeGenericParams(elementType.GenericParameters, stringBuilder);
					return stringBuilder.ToString();
				}
				if (elementType.IsArray)
				{
					ArrayType arrayType = elementType as ArrayType;
					return String.Concat(this.GetMemberEscapedOnlyName(arrayType.ElementType), "[]");
				}
				if (memberDefinition is TypeDefinition)
				{
					return ((TypeDefinition)memberDefinition).GetFriendlyMemberName(this.Language);
				}
				return elementType.GetFriendlyTypeName(this.Language, "<", ">");
			}
			if (!(memberDefinition is MethodReference))
			{
				if (memberDefinition is IMemberDefinition)
				{
					return (memberDefinition as IMemberDefinition).GetFriendlyMemberName(this.Language);
				}
				if (memberDefinition is ParameterReference)
				{
					return this.Language.ReplaceInvalidCharactersInIdentifier(((ParameterReference)memberDefinition).Name);
				}
				if (!(memberDefinition is MemberReference))
				{
					return memberDefinition.ToString();
				}
				return this.Language.ReplaceInvalidCharactersInIdentifier(((MemberReference)memberDefinition).Name);
			}
			MethodReference elementMethod = ((MethodReference)memberDefinition).GetElementMethod();
			if (elementMethod.HasGenericParameters)
			{
				StringBuilder stringBuilder1 = new StringBuilder(this.Language.ReplaceInvalidCharactersInIdentifier(elementMethod.Name));
				this.NormalizeGenericParams(elementMethod.GenericParameters, stringBuilder1);
				return stringBuilder1.ToString();
			}
			if (memberDefinition is MethodDefinition)
			{
				return (memberDefinition as MethodDefinition).GetFriendlyMemberName(this.Language);
			}
			return this.Language.ReplaceInvalidCharactersInIdentifier(((MethodReference)memberDefinition).Name);
		}

		protected string GetMemberName(MemberReference member)
		{
			if (member is MethodReference)
			{
				return this.GetMethodName(member as MethodReference);
			}
			if (member is FieldReference)
			{
				return this.GetFieldName(member as FieldReference);
			}
			if (member is PropertyReference)
			{
				return this.GetPropertyName(member as PropertyReference);
			}
			if (member is EventReference)
			{
				return this.GetEventName(member as EventReference);
			}
			if (!(member is TypeReference))
			{
				throw new NotSupportedException("Unexpected member type.");
			}
			return this.GetTypeName(member as TypeReference);
		}

		protected MethodSpecificContext GetMethodContext(IMemberDefinition member)
		{
			MethodSpecificContext methodSpecificContext;
			string memberUniqueName = Utilities.GetMemberUniqueName(member);
			if (!this.writerContext.MethodContexts.TryGetValue(memberUniqueName, out methodSpecificContext))
			{
				return null;
			}
			return methodSpecificContext;
		}

		protected virtual string GetMethodName(MethodReference method)
		{
			MethodDefinition methodDefinition = method.Resolve();
			if (methodDefinition == null || !(methodDefinition.Module.FilePath == this.ModuleContext.Module.FilePath))
			{
				return Utilities.EscapeNameIfNeeded(GenericHelper.GetNonGenericName(method.Name), this.Language);
			}
			if (this.TypeContext.MethodDefinitionToNameMap.ContainsKey(methodDefinition))
			{
				return this.TypeContext.MethodDefinitionToNameMap[methodDefinition];
			}
			if (this.TypeContext.GeneratedMethodDefinitionToNameMap.ContainsKey(methodDefinition))
			{
				return this.TypeContext.GeneratedMethodDefinitionToNameMap[methodDefinition];
			}
			return this.ModuleContext.RenamedMembersMap[methodDefinition.MetadataToken.ToUInt32()];
		}

		protected virtual string GetPropertyName(PropertyReference property)
		{
			string item;
			PropertyDefinition propertyDefinition = property.Resolve();
			if (propertyDefinition == null || !(propertyDefinition.Module.FilePath == this.ModuleContext.Module.FilePath))
			{
				return Utilities.EscapeNameIfNeeded(GenericHelper.GetNonGenericName(property.Name), this.Language);
			}
			try
			{
				item = this.ModuleContext.RenamedMembersMap[propertyDefinition.MetadataToken.ToUInt32()];
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				uint num = propertyDefinition.MetadataToken.ToUInt32();
				this.Write(num.ToString());
				this.WriteLine();
				this.Write(this.ModuleContext.RenamedMembersMap.Count.ToString());
				this.WriteLine();
				foreach (KeyValuePair<uint, string> renamedMembersMap in this.ModuleContext.RenamedMembersMap)
				{
					num = renamedMembersMap.Key;
					this.Write(String.Concat(num.ToString(), " ", renamedMembersMap.Value));
					this.WriteLine();
				}
				throw exception;
			}
			return item;
		}

		protected Statement GetStatement(MethodDefinition method)
		{
			Statement statement;
			if (this.writerContext.DecompiledStatements.TryGetValue(Utilities.GetMemberUniqueName(method), out statement))
			{
				return statement;
			}
			return null;
		}

		protected virtual string GetTypeName(TypeReference type)
		{
			if (this.IsTypeInCurrentModule(type))
			{
				return this.GetCurrentModuleTypeName(type);
			}
			return Utilities.EscapeTypeNameIfNeeded(GenericHelper.GetNonGenericName(type.Name), this.Language);
		}

		internal virtual void Indent()
		{
			this.formatter.Indent();
		}

		protected bool IsTypeInCurrentModule(TypeReference type)
		{
			TypeDefinition typeDefinition = type.Resolve();
			if (typeDefinition == null)
			{
				return false;
			}
			return typeDefinition.Module.FilePath == this.ModuleContext.Module.FilePath;
		}

		private void NormalizeGenericParams(Collection<GenericParameter> genericCollection, StringBuilder nameBuilder)
		{
			nameBuilder.Append("<");
			for (int i = 0; i < genericCollection.Count; i++)
			{
				GenericParameter item = genericCollection[i];
				nameBuilder.Append(this.GetGenericParameterName(item));
				if (i < genericCollection.Count - 1)
				{
					nameBuilder.Append(",");
				}
			}
			nameBuilder.Append(">");
		}

		protected virtual string OnConvertString(string str)
		{
			return BaseLanguageWriter.ConvertString(str);
		}

		protected void OnExceptionThrown(Exception ex)
		{
			this.OnExceptionThrown(this, ex);
		}

		protected void OnExceptionThrown(object sender, Exception ex)
		{
			EventHandler<Exception> eventHandler = this.ExceptionThrown;
			if (eventHandler != null)
			{
				eventHandler(sender, ex);
			}
		}

		internal virtual void Outdent()
		{
			this.formatter.Outdent();
		}

		private bool ShouldWriteAsDelegate(TypeDefinition type)
		{
			if (!this.Language.HasDelegateSpecificSyntax)
			{
				return false;
			}
			return type.IsDelegate();
		}

		private int SortEnumFields(IMemberDefinition first, IMemberDefinition second)
		{
			FieldDefinition fieldDefinition = first as FieldDefinition;
			FieldDefinition fieldDefinition1 = second as FieldDefinition;
			if (fieldDefinition == null || fieldDefinition.Constant == null || fieldDefinition.Constant.Value == null)
			{
				return 1;
			}
			if (fieldDefinition1 == null || fieldDefinition1.Constant == null || fieldDefinition1.Constant.Value == null)
			{
				return -1;
			}
			int num = 0;
			if (fieldDefinition.Constant.Value.GetType().Name == "Int32")
			{
				int value = (Int32)fieldDefinition.Constant.Value;
				num = value.CompareTo((Int32)fieldDefinition1.Constant.Value);
			}
			if (fieldDefinition.Constant.Value.GetType().Name == "UInt32")
			{
				uint value1 = (UInt32)fieldDefinition.Constant.Value;
				num = value1.CompareTo((UInt32)fieldDefinition1.Constant.Value);
			}
			else if (fieldDefinition.Constant.Value.GetType().Name == "Int16")
			{
				short num1 = (Int16)fieldDefinition.Constant.Value;
				num = num1.CompareTo((Int16)fieldDefinition1.Constant.Value);
			}
			else if (fieldDefinition.Constant.Value.GetType().Name == "UInt16")
			{
				ushort value2 = (UInt16)fieldDefinition.Constant.Value;
				num = value2.CompareTo((UInt16)fieldDefinition1.Constant.Value);
			}
			else if (fieldDefinition.Constant.Value.GetType().Name == "Byte")
			{
				byte num2 = (Byte)fieldDefinition.Constant.Value;
				num = num2.CompareTo((Byte)fieldDefinition1.Constant.Value);
			}
			else if (fieldDefinition.Constant.Value.GetType().Name == "SByte")
			{
				sbyte value3 = (SByte)fieldDefinition.Constant.Value;
				num = value3.CompareTo((SByte)fieldDefinition1.Constant.Value);
			}
			else if (fieldDefinition.Constant.Value.GetType().Name == "Int64")
			{
				long num3 = (Int64)fieldDefinition.Constant.Value;
				num = num3.CompareTo((Int64)fieldDefinition1.Constant.Value);
			}
			else if (fieldDefinition.Constant.Value.GetType().Name == "UInt64")
			{
				ulong value4 = (UInt64)fieldDefinition.Constant.Value;
				num = value4.CompareTo((UInt64)fieldDefinition1.Constant.Value);
			}
			if (num != 0)
			{
				return num;
			}
			return fieldDefinition.Name.CompareTo(fieldDefinition1.Name);
		}

		protected virtual void StartWritingComment()
		{
			this.isWritingComment = true;
			this.formatter.StartWritingComment();
		}

		public virtual void Stop()
		{
			this.isStopped = true;
		}

		protected virtual bool TryWriteEnumField(FieldDefinition fieldDefinition)
		{
			if (!fieldDefinition.DeclaringType.IsEnum)
			{
				return false;
			}
			int currentPosition = this.formatter.CurrentPosition;
			this.WriteReference(this.GetFieldName(fieldDefinition), fieldDefinition);
			int num = this.formatter.CurrentPosition - 1;
			this.currentWritingInfo.MemberDeclarationToCodePostionMap[fieldDefinition] = new OffsetSpan(currentPosition, num);
			if (!fieldDefinition.DeclaringType.IsDefaultEnumConstants)
			{
				this.WriteLiteral(" = ");
				this.WriteLiteral(String.Format(CultureInfo.InvariantCulture, "{0}", fieldDefinition.Constant.Value));
			}
			return true;
		}

		protected void UpdateWritingInfo(WriterContext writerContext, WritingInfo writingInfo)
		{
			foreach (MethodSpecificContext value in writerContext.MethodContexts.Values)
			{
				if (value == null)
				{
					continue;
				}
				writingInfo.ControlFlowGraphs.Add(value.Method, value.ControlFlowGraph);
				writingInfo.MethodsVariableDefinitionToNameMap.Add(value.Method, value.VariableDefinitionToNameMap);
			}
			foreach (IMemberDefinition exceptionWhileDecompiling in writerContext.TypeContext.ExceptionWhileDecompiling)
			{
				if (writingInfo.MembersWithExceptions.Contains(exceptionWhileDecompiling.MetadataToken.ToUInt32()))
				{
					continue;
				}
				writingInfo.MembersWithExceptions.Add(exceptionWhileDecompiling.MetadataToken.ToUInt32());
			}
			writingInfo.AutoImplementedProperties.UnionWith(writerContext.TypeContext.AutoImplementedProperties);
			writingInfo.AutoImplementedEvents.UnionWith(writerContext.TypeContext.AutoImplementedEvents);
			foreach (GeneratedMethod generatedFilterMethod in writerContext.TypeContext.GeneratedFilterMethods)
			{
				writingInfo.GeneratedFilterMethods.Add(generatedFilterMethod.Method);
			}
		}

		public override void VisitExceptionStatement(ExceptionStatement node)
		{
			this.WriteException(new Exception(String.Concat("Exception in: ", node.Member.GetFullMemberName(this.Language)), node.ExceptionObject), node.Member);
		}

		public override void VisitMemberHandleExpression(MemberHandleExpression node)
		{
			throw new NotSupportedException();
		}

		internal virtual void Write(string str)
		{
			this.formatter.Write(str);
		}

		public virtual List<WritingInfo> Write(IMemberDefinition member, IWriterContextService writerContextService)
		{
			this.writerContextService = writerContextService;
			this.writerContext = writerContextService.GetWriterContext(member, this.Language);
			if (member is TypeDefinition)
			{
				this.CurrentType = member as TypeDefinition;
			}
			this.currentWritingInfo = new WritingInfo(member);
			this.UpdateWritingInfo(this.writerContext, this.currentWritingInfo);
			this.writingInfos = new List<WritingInfo>()
			{
				this.currentWritingInfo
			};
			this.WriteInternal(member);
			return this.writingInfos;
		}

		protected virtual void Write(Statement statement)
		{
		}

		protected virtual void Write(Expression expression)
		{
		}

		protected abstract void Write(MethodDefinition member);

		protected abstract void Write(PropertyDefinition property);

		protected abstract void Write(EventDefinition @event);

		protected abstract void Write(FieldDefinition field);

		protected virtual void WriteAttributes(IMemberDefinition member, IEnumerable<string> ignoredAttributes = null)
		{
		}

		protected virtual void WriteBeginBlock(bool inline = false)
		{
		}

		public virtual void WriteBody(IMemberDefinition member, IWriterContextService writerContextService)
		{
			this.writerContextService = writerContextService;
			this.currentWritingInfo = new WritingInfo(member);
			this.writingInfos = new List<WritingInfo>()
			{
				this.currentWritingInfo
			};
			this.writerContext = writerContextService.GetWriterContext(member, this.Language);
			this.WriteBodyInternal(member);
		}

		protected virtual void WriteBodyInternal(IMemberDefinition member)
		{
		}

		public virtual void WriteComment(string comment)
		{
			this.formatter.WriteComment(String.Concat(this.Language.CommentLineSymbol, " ", comment));
		}

		internal virtual void WriteDefinition(string name, object definition)
		{
			this.formatter.WriteDefinition(name, definition);
		}

		protected virtual void WriteDelegate(TypeDefinition delegateDefinition)
		{
		}

		protected void WriteDocumentation(IMemberDefinition member)
		{
			string documentationForMember = DocumentationManager.GetDocumentationForMember(member);
			if (!String.IsNullOrEmpty(documentationForMember))
			{
				this.WriteDocumentation(documentationForMember);
				this.WriteLine();
			}
		}

		private void WriteDocumentation(string memberDocumentation)
		{
			this.formatter.WriteDocumentationStartBlock();
			bool flag = false;
			bool flag1 = false;
			using (StringReader stringReader = new StringReader(memberDocumentation))
			{
				for (string i = stringReader.ReadLine(); i != null; i = stringReader.ReadLine())
				{
					if (flag)
					{
						this.WriteLine();
					}
					flag = true;
					this.formatter.WriteDocumentationTag(String.Concat(this.Language.DocumentationLineStarter, " "));
					flag1 = this.WriteDocumentationLine(i, flag1);
				}
			}
			this.formatter.WriteEndBlock();
		}

		private bool WriteDocumentationLine(string currentLine, bool isDocumentTag)
		{
			int num = -1;
			int num1 = -1;
			if (isDocumentTag)
			{
				int num2 = 0;
				while (num2 < currentLine.Length)
				{
					if (currentLine[num2] != '>')
					{
						num2++;
					}
					else
					{
						num1 = num2;
						break;
					}
				}
				if (num1 == -1)
				{
					this.formatter.WriteDocumentationTag(currentLine);
					return true;
				}
				string str = currentLine.Substring(0, num1 + 1);
				string str1 = currentLine.Substring(num1 + 1);
				this.formatter.WriteDocumentationTag(str);
				return this.WriteDocumentationLine(str1, false);
			}
			int num3 = 0;
			while (num3 < currentLine.Length)
			{
				if (currentLine[num3] != '<')
				{
					num3++;
				}
				else
				{
					num = num3;
					break;
				}
			}
			if (num == -1)
			{
				this.formatter.WriteComment(currentLine);
				return false;
			}
			string str2 = currentLine.Substring(0, num);
			this.formatter.WriteComment(str2);
			int num4 = num + 1;
			while (num4 < currentLine.Length)
			{
				if (currentLine[num4] != '>')
				{
					num4++;
				}
				else
				{
					num1 = num4;
					break;
				}
			}
			if (num1 == -1)
			{
				this.formatter.WriteDocumentationTag(currentLine.Substring(num));
				return true;
			}
			string str3 = currentLine.Substring(num, num1 - num + 1);
			this.formatter.WriteDocumentationTag(str3);
			string str4 = currentLine.Substring(num1 + 1);
			return this.WriteDocumentationLine(str4, false);
		}

		protected virtual void WriteEndBlock(string statementName)
		{
		}

		protected virtual void WriteEndOfStatement()
		{
		}

		protected virtual void WriteEnumValueSeparator()
		{
		}

		public virtual void WriteEscapeLiteral(string literal)
		{
			this.formatter.WriteLiteral(this.OnConvertString(literal));
		}

		protected abstract void WriteEventDeclaration(EventDefinition @event);

		public virtual void WriteException(Exception ex, IMemberDefinition member)
		{
			string[] strArray = this.exceptionFormatter.Format(ex, member.FullName, member.DeclaringType.Module.FilePath);
			if (!this.Settings.WriteExceptionsAsComments)
			{
				this.formatter.WriteException(strArray);
				this.formatter.WriteExceptionMailToLink("mailto: JustDecompilePublicFeedback@telerik.com", strArray);
				return;
			}
			string[] array = (
				from exceptionLine in strArray
				select String.Concat(this.Language.CommentLineSymbol, " ", exceptionLine)).ToArray<string>();
			this.formatter.WriteException(array);
			string str = String.Concat(this.Language.CommentLineSymbol, " mailto: JustDecompilePublicFeedback@telerik.com");
			this.formatter.WriteExceptionMailToLink(str, strArray);
		}

		internal virtual void WriteIdentifier(string name, object identifier)
		{
			this.formatter.WriteIdentifier(name, identifier);
		}

		protected virtual void WriteInternal(IMemberDefinition member)
		{
			uint num = member.MetadataToken.ToUInt32();
			if (this.isStopped)
			{
				return;
			}
			this.membersStack.Push(member);
			bool flag = (!(member is TypeDefinition) ? false : member != this.CurrentType);
			if (!flag)
			{
				this.formatter.PreserveIndent(member);
			}
			try
			{
				if (this.Settings.WriteDocumentation && !flag)
				{
					int currentPosition = this.formatter.CurrentPosition;
					this.WriteDocumentation(member);
					this.AddMemberDocumentationOffsetSpan(num, currentPosition, this.formatter.CurrentPosition);
				}
				if (!(member is TypeDefinition))
				{
					int currentPosition1 = this.formatter.CurrentPosition;
					this.WriteAttributes(member, null);
					this.AddMemberAttributesOffsetSpan(num, currentPosition1, this.formatter.CurrentPosition);
					int num1 = this.formatter.CurrentPosition;
					if (member is MethodDefinition)
					{
						this.formatter.WriteMemberDeclaration(member);
						this.Write((MethodDefinition)member);
					}
					else if (member is PropertyDefinition)
					{
						this.formatter.WriteMemberDeclaration(member);
						this.Write((PropertyDefinition)member);
					}
					else if (member is EventDefinition)
					{
						this.formatter.WriteMemberDeclaration(member);
						this.Write((EventDefinition)member);
					}
					else if (member is FieldDefinition)
					{
						this.formatter.WriteMemberDeclaration(member);
						this.Write((FieldDefinition)member);
					}
					this.currentWritingInfo.MemberTokenToDecompiledCodeMap.Add(num, new OffsetSpan(num1, this.formatter.CurrentPosition - 1));
				}
				else
				{
					this.formatter.WriteMemberDeclaration(member);
					this.WriteTypeInANewWriterIfNeeded((TypeDefinition)member);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.Visit(new ExceptionStatement(exception, member));
				this.formatter.RestoreIndent(member);
				this.currentWritingInfo.ExceptionsWhileWriting.Add(member);
				this.currentWritingInfo.MembersWithExceptions.Add(num);
				if (member is PropertyDefinition)
				{
					PropertyDefinition propertyDefinition = member as PropertyDefinition;
					if (propertyDefinition.GetMethod != null)
					{
						this.currentWritingInfo.ExceptionsWhileWriting.Add(propertyDefinition.GetMethod);
					}
					if (propertyDefinition.SetMethod != null)
					{
						this.currentWritingInfo.ExceptionsWhileWriting.Add(propertyDefinition.SetMethod);
					}
				}
				if (member is EventDefinition)
				{
					EventDefinition eventDefinition = member as EventDefinition;
					if (eventDefinition.AddMethod != null)
					{
						this.currentWritingInfo.ExceptionsWhileWriting.Add(eventDefinition.AddMethod);
					}
					if (eventDefinition.RemoveMethod != null)
					{
						this.currentWritingInfo.ExceptionsWhileWriting.Add(eventDefinition.RemoveMethod);
					}
					if (eventDefinition.InvokeMethod != null)
					{
						this.currentWritingInfo.ExceptionsWhileWriting.Add(eventDefinition.InvokeMethod);
					}
				}
				this.OnExceptionThrown(exception);
			}
			if (!(member is TypeDefinition) || member == this.CurrentType)
			{
				this.formatter.RemovePreservedIndent(member);
			}
			this.membersStack.Pop();
		}

		internal virtual void WriteKeyword(string keyword)
		{
			this.formatter.WriteKeyword(keyword);
		}

		internal virtual void WriteLine()
		{
			this.formatter.WriteLine();
		}

		internal virtual void WriteLiteral(string literal)
		{
			this.formatter.WriteLiteral(literal);
		}

		protected virtual bool WriteMemberDeclaration(IMemberDefinition member)
		{
			if (member is MethodDefinition)
			{
				this.WriteAttributes(member, null);
				this.WriteMethodDeclaration((MethodDefinition)member, false);
				this.WriteEndOfStatement();
				return true;
			}
			if (member is PropertyDefinition)
			{
				this.WriteAttributes(member, null);
				this.WritePropertyDeclaration((PropertyDefinition)member);
				this.WriteEndOfStatement();
				return true;
			}
			if (member is EventDefinition)
			{
				this.WriteAttributes(member, null);
				this.WriteEventDeclaration((EventDefinition)member);
				this.WriteEndOfStatement();
				return true;
			}
			if (member is FieldDefinition)
			{
				FieldDefinition fieldDefinition = member as FieldDefinition;
				if (fieldDefinition.IsSpecialName)
				{
					return false;
				}
				if (this.TryWriteEnumField(fieldDefinition))
				{
					return true;
				}
				this.WriteAttributes(member, null);
				this.Write((FieldDefinition)member);
				return true;
			}
			if (!(member is TypeDefinition))
			{
				return true;
			}
			TypeDefinition typeDefinition = member as TypeDefinition;
			if (!this.ShouldWriteAsDelegate(typeDefinition))
			{
				this.WriteTypeDeclarationsOnlyInternal(typeDefinition);
				return true;
			}
			this.WriteAttributes(typeDefinition, null);
			this.WriteDelegate(typeDefinition);
			return true;
		}

		public void WriteMemberEscapedOnlyName(object memberDefinition)
		{
			string memberEscapedOnlyName = this.GetMemberEscapedOnlyName(memberDefinition);
			this.formatter.Write(memberEscapedOnlyName);
		}

		public abstract void WriteMemberNavigationName(object memberDefinition);

		public abstract void WriteMemberNavigationPathFullName(object member);

		protected abstract void WriteMethodDeclaration(MethodDefinition method, bool writeDocumentation = false);

		protected virtual void WriteNamespace(object reference, bool forceWriteNamespace = false)
		{
		}

		public virtual void WriteNamespaceNavigationName(string memberReferenceName)
		{
			string[] strArray = memberReferenceName.Split(new Char[] { '.' });
			StringBuilder stringBuilder = new StringBuilder((int)strArray.Length);
			for (int i = 0; i < (int)strArray.Length; i++)
			{
				string str = strArray[i];
				if (this.Language.IsValidIdentifier(str) || !this.Settings.RenameInvalidMembers)
				{
					stringBuilder.Append(str);
				}
				else
				{
					stringBuilder.Append(this.Language.ReplaceInvalidCharactersInIdentifier(str));
				}
				if (i < (int)strArray.Length - 1)
				{
					stringBuilder.Append(".");
				}
			}
			this.formatter.Write(stringBuilder.ToString());
		}

		protected virtual void WriteNestedTypeWriteLine()
		{
		}

		internal virtual void WriteNotResolvedReference(string name, MemberReference memberReference, string errorMessage)
		{
			this.formatter.WriteNotResolvedReference(name, memberReference, errorMessage);
		}

		protected void WritePartialType(TypeDefinition type, ICollection<string> fieldsToSkip = null)
		{
			if (this.isStopped)
			{
				return;
			}
			this.WriteAttributes(type, new String[] { "System.CodeDom.Compiler.GeneratedCodeAttribute" });
			this.WritePartialType(type, (IMemberDefinition x) => this.WriteInternal(x), false, fieldsToSkip);
		}

		protected virtual void WritePartialType(TypeDefinition type, Action<IMemberDefinition> writeMember, bool writeNewLine, ICollection<string> fieldsToSkip = null)
		{
			string empty = String.Empty;
			empty = this.WriteTypeDeclaration(type, true);
			this.WriteBeginBlock(false);
			this.WriteTypeOpeningBlock(type);
			this.WriteTypeMembers(type, writeMember, new String[] { "System.Diagnostics.DebuggerNonUserCodeAttribute" }, fieldsToSkip);
			this.WriteLine();
			this.Outdent();
			this.WriteEndBlock(empty);
		}

		protected abstract void WritePropertyDeclaration(PropertyDefinition property);

		internal virtual void WriteReference(string name, object reference)
		{
			this.formatter.WriteReference(name, reference);
		}

		internal virtual void WriteSpace()
		{
			this.formatter.WriteSpace();
		}

		internal virtual void WriteToken(string token)
		{
			this.formatter.WriteToken(token);
		}

		protected void WriteType(TypeDefinition type)
		{
			if (this.isStopped)
			{
				return;
			}
			this.membersStack.Push(type);
			int currentPosition = this.formatter.CurrentPosition;
			this.WriteAttributes(type, new String[] { "System.Reflection.DefaultMemberAttribute" });
			Dictionary<uint, OffsetSpan> memberTokenToAttributesMap = this.currentWritingInfo.MemberTokenToAttributesMap;
			Mono.Cecil.MetadataToken metadataToken = type.MetadataToken;
			memberTokenToAttributesMap.Add(metadataToken.ToUInt32(), new OffsetSpan(currentPosition, this.formatter.CurrentPosition - 1));
			int num = this.formatter.CurrentPosition;
			this.WriteTypeInternal(type, (IMemberDefinition x) => this.WriteInternal(x));
			Dictionary<uint, OffsetSpan> memberTokenToDecompiledCodeMap = this.currentWritingInfo.MemberTokenToDecompiledCodeMap;
			metadataToken = type.MetadataToken;
			memberTokenToDecompiledCodeMap.Add(metadataToken.ToUInt32(), new OffsetSpan(num, this.formatter.CurrentPosition - 1));
			this.membersStack.Pop();
		}

		public void WriteTypeDeclaration(TypeDefinition type, IWriterContextService writerContextService)
		{
			this.writerContextService = writerContextService;
			this.writerContext = writerContextService.GetWriterContext(type, this.Language);
			this.currentWritingInfo = new WritingInfo(type);
			this.WriteAttributes(type, new String[] { "System.Reflection.DefaultMemberAttribute" });
			this.membersStack.Push(type);
			if (this.ShouldWriteAsDelegate(type))
			{
				this.WriteDelegate(type);
				this.WriteLine();
				this.membersStack.Pop();
				return;
			}
			this.WriteTypeDeclaration(type, false);
			this.WriteLine();
			this.membersStack.Pop();
		}

		protected abstract string WriteTypeDeclaration(TypeDefinition type, bool isPartial = false);

		public virtual void WriteTypeDeclarationsOnly(TypeDefinition member, IWriterContextService writerContextService)
		{
			this.writerContextService = writerContextService;
			this.writerContext = writerContextService.GetWriterContext(member, this.Language);
			this.currentWritingInfo = new WritingInfo(member);
			this.WriteTypeDeclarationsOnlyInternal(member);
		}

		protected virtual void WriteTypeDeclarationsOnlyInternal(TypeDefinition member)
		{
			this.WriteTypeNamespaceStart(member);
			this.WriteAttributes(member, null);
			this.WriteTypeInternal(member, (IMemberDefinition x) => this.WriteMemberDeclaration(x));
			this.WriteTypeNamespaceEnd(member);
		}

		protected virtual void WriteTypeInANewWriterIfNeeded(TypeDefinition type)
		{
			if (this.CurrentType == type)
			{
				this.WriteType(type);
				return;
			}
			ILanguageWriter writer = this.Language.GetWriter(this.formatter, this.exceptionFormatter, this.Settings);
			writer.ExceptionThrown += new EventHandler<Exception>(this.OnExceptionThrown);
			List<WritingInfo> writingInfos = writer.Write(type, this.writerContextService);
			writer.ExceptionThrown -= new EventHandler<Exception>(this.OnExceptionThrown);
			this.writingInfos.AddRange(writingInfos);
		}

		protected virtual void WriteTypeInternal(TypeDefinition type, Action<IMemberDefinition> writeMember)
		{
			string empty = String.Empty;
			this.membersStack.Push(type);
			if (this.ShouldWriteAsDelegate(type))
			{
				this.WriteDelegate(type);
				return;
			}
			empty = this.WriteTypeDeclaration(type, false);
			int currentPosition = this.formatter.CurrentPosition;
			this.formatter.WriteStartBlock();
			this.WriteBeginBlock(false);
			this.WriteTypeOpeningBlock(type);
			this.WriteTypeMembers(type, writeMember, null, null);
			this.WriteLine();
			this.Outdent();
			this.WriteEndBlock(empty);
			this.membersStack.Pop();
			this.currentWritingInfo.MemberDefinitionToFoldingPositionMap[type] = new OffsetSpan(currentPosition, this.formatter.CurrentPosition - 1);
			this.formatter.WriteEndBlock();
		}

		private void WriteTypeMembers(TypeDefinition type, Action<IMemberDefinition> writeMember, IEnumerable<string> attributesToSkip = null, ICollection<string> fieldsToSkip = null)
		{
			List<IMemberDefinition> typeMembers = Utilities.GetTypeMembers(type, this.Language, this.Settings.ShowCompilerGeneratedMembers, attributesToSkip, fieldsToSkip, this.currentWritingInfo.GeneratedFilterMethods, this.TypeContext.GetFieldToPropertyMap(this.Language).Keys);
			if (type.IsEnum)
			{
				typeMembers.Sort((IMemberDefinition x, IMemberDefinition y) => this.SortEnumFields(x, y));
			}
			for (int i = 0; i < typeMembers.Count; i++)
			{
				IMemberDefinition item = null;
				if (this.isStopped)
				{
					return;
				}
				item = typeMembers[i];
				if (!(item is FieldDefinition) || !(item as FieldDefinition).IsSpecialName)
				{
					this.membersStack.Push(item);
					bool count = i == typeMembers.Count - 1;
					writeMember(item);
					if (type.IsEnum)
					{
						if (!count && typeMembers[i + 1].Name != "value__")
						{
							this.WriteEnumValueSeparator();
							this.WriteLine();
						}
					}
					else if (!count)
					{
						this.WriteLine();
						this.WriteLine();
					}
					this.membersStack.Pop();
				}
			}
		}

		protected virtual void WriteTypeNamespaceEnd(TypeDefinition member)
		{
		}

		protected virtual void WriteTypeNamespaceStart(TypeDefinition member)
		{
		}

		protected virtual void WriteTypeOpeningBlock(TypeDefinition type)
		{
		}

		public event EventHandler<Exception> ExceptionThrown;
	}
}