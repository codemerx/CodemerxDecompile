using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.Languages
{
	public abstract class BaseLanguageWriter : BaseCodeVisitor, ILanguageWriter, IExceptionThrownNotifier
	{
		protected bool isStopped;

		protected IFormatter formatter;

		protected Stack<IMemberDefinition> membersStack;

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
				return this.writerContext.get_AssemblyContext();
			}
		}

		protected MethodDefinition CurrentMethod
		{
			get
			{
				if (this.membersStack.get_Count() <= 0 || this.membersStack.Peek() as MethodDefinition == null)
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
				if (this.membersStack.get_Count() <= 0)
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
				return this.writerContext.get_MethodContexts().get_Count() == 0;
			}
		}

		protected virtual ModuleSpecificContext ModuleContext
		{
			get
			{
				return this.writerContext.get_ModuleContext();
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
				return this.writerContext.get_TypeContext();
			}
		}

		public BaseLanguageWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
		{
			this.membersStack = new Stack<IMemberDefinition>();
			base();
			this.set_Language(language);
			this.formatter = formatter;
			this.exceptionFormatter = exceptionFormatter;
			this.set_Settings(settings);
			return;
		}

		protected void AddMemberAttributesOffsetSpan(uint memberToken, int startIndex, int endIndex)
		{
			if (startIndex < endIndex)
			{
				this.currentWritingInfo.get_MemberTokenToAttributesMap().Add(memberToken, new OffsetSpan(startIndex, endIndex - 1));
			}
			return;
		}

		protected void AddMemberDocumentationOffsetSpan(uint memberToken, int startIndex, int endIndex)
		{
			if (startIndex < endIndex)
			{
				this.currentWritingInfo.get_MemberTokenToDocumentationMap().Add(memberToken, new OffsetSpan(startIndex, endIndex - 1));
			}
			return;
		}

		internal static string ConvertChar(char ch)
		{
			switch (ch)
			{
				case 0:
				{
					return "\\0";
				}
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				{
				Label0:
					if (!Char.IsControl(ch) && !Char.IsSurrogate(ch) && !Char.IsWhiteSpace(ch) || ch == ' ')
					{
						return ch.ToString();
					}
					V_0 = ch;
					return String.Concat("\\u", V_0.ToString("x4"));
				}
				case 7:
				{
					return "\\a";
				}
				case 8:
				{
					return "\\b";
				}
				case 9:
				{
					return "\\t";
				}
				case 10:
				{
					return "\\n";
				}
				case 11:
				{
					return "\\v";
				}
				case 12:
				{
					return "\\f";
				}
				case 13:
				{
					return "\\r";
				}
				default:
				{
					if (ch != '\\')
					{
						goto Label0;
					}
					return "\\\\";
				}
			}
		}

		internal static string ConvertString(string str)
		{
			V_0 = new StringBuilder();
			V_1 = str;
			V_2 = 0;
			while (V_2 < V_1.get_Length())
			{
				V_3 = V_1.get_Chars(V_2);
				if (V_3 != '\"')
				{
					dummyVar1 = V_0.Append(BaseLanguageWriter.ConvertChar(V_3));
				}
				else
				{
					dummyVar0 = V_0.Append("\\\"");
				}
				V_2 = V_2 + 1;
			}
			return V_0.ToString();
		}

		protected virtual void EndWritingComment()
		{
			this.isWritingComment = false;
			this.formatter.EndWritingComment();
			return;
		}

		protected string GetCurrentModuleTypeName(TypeReference type)
		{
			V_0 = type.Resolve();
			stackVariable4 = this.get_ModuleContext().get_RenamedMembersMap();
			V_1 = V_0.get_MetadataToken();
			return stackVariable4.get_Item(V_1.ToUInt32());
		}

		protected virtual string GetEventName(EventReference event)
		{
			V_0 = event.Resolve();
			if (V_0 == null || !String.op_Equality(V_0.get_Module().get_FilePath(), this.get_ModuleContext().get_Module().get_FilePath()))
			{
				return Utilities.EscapeNameIfNeeded(GenericHelper.GetNonGenericName(event.get_Name()), this.get_Language());
			}
			stackVariable19 = this.get_ModuleContext().get_RenamedMembersMap();
			V_1 = V_0.get_MetadataToken();
			return stackVariable19.get_Item(V_1.ToUInt32());
		}

		protected virtual string GetFieldName(FieldReference field)
		{
			V_0 = field.Resolve();
			if (V_0 == null || !String.op_Equality(V_0.get_Module().get_FilePath(), this.get_ModuleContext().get_Module().get_FilePath()))
			{
				return Utilities.EscapeNameIfNeeded(GenericHelper.GetNonGenericName(field.get_Name()), this.get_Language());
			}
			if (this.get_TypeContext() != null && this.get_TypeContext().get_BackingFieldToNameMap().ContainsKey(V_0))
			{
				return this.get_TypeContext().get_BackingFieldToNameMap().get_Item(V_0);
			}
			stackVariable21 = this.get_ModuleContext().get_RenamedMembersMap();
			V_1 = V_0.get_MetadataToken();
			return stackVariable21.get_Item(V_1.ToUInt32());
		}

		private string GetGenericParameterName(GenericParameter genericParameter)
		{
			if (genericParameter.get_Owner() as TypeReference != null)
			{
				V_0 = genericParameter.get_Position();
				return String.Concat("!", V_0.ToString());
			}
			V_0 = genericParameter.get_Position();
			return String.Concat("!!", V_0.ToString());
		}

		protected string GetMemberEscapedOnlyName(object memberDefinition)
		{
			if (memberDefinition == null)
			{
				return String.Empty;
			}
			if (memberDefinition as TypeReference != null)
			{
				V_0 = ((TypeReference)memberDefinition).GetElementType();
				if (V_0.get_IsGenericParameter())
				{
					return this.GetGenericParameterName((GenericParameter)V_0);
				}
				if (V_0.get_HasGenericParameters())
				{
					V_1 = new StringBuilder(this.get_Language().ReplaceInvalidCharactersInIdentifier(V_0.get_Name()));
					this.NormalizeGenericParams(V_0.get_GenericParameters(), V_1);
					return V_1.ToString();
				}
				if (V_0.get_IsArray())
				{
					V_2 = V_0 as ArrayType;
					return String.Concat(this.GetMemberEscapedOnlyName(V_2.get_ElementType()), "[]");
				}
				if (memberDefinition as TypeDefinition != null)
				{
					return ((TypeDefinition)memberDefinition).GetFriendlyMemberName(this.get_Language());
				}
				return V_0.GetFriendlyTypeName(this.get_Language(), "<", ">");
			}
			if (memberDefinition as MethodReference == null)
			{
				if (memberDefinition as IMemberDefinition != null)
				{
					return (memberDefinition as IMemberDefinition).GetFriendlyMemberName(this.get_Language());
				}
				if (memberDefinition as ParameterReference != null)
				{
					return this.get_Language().ReplaceInvalidCharactersInIdentifier(((ParameterReference)memberDefinition).get_Name());
				}
				if (memberDefinition as MemberReference == null)
				{
					return memberDefinition.ToString();
				}
				return this.get_Language().ReplaceInvalidCharactersInIdentifier(((MemberReference)memberDefinition).get_Name());
			}
			V_3 = ((MethodReference)memberDefinition).GetElementMethod();
			if (V_3.get_HasGenericParameters())
			{
				V_4 = new StringBuilder(this.get_Language().ReplaceInvalidCharactersInIdentifier(V_3.get_Name()));
				this.NormalizeGenericParams(V_3.get_GenericParameters(), V_4);
				return V_4.ToString();
			}
			if (memberDefinition as MethodDefinition != null)
			{
				return (memberDefinition as MethodDefinition).GetFriendlyMemberName(this.get_Language());
			}
			return this.get_Language().ReplaceInvalidCharactersInIdentifier(((MethodReference)memberDefinition).get_Name());
		}

		protected string GetMemberName(MemberReference member)
		{
			if (member as MethodReference != null)
			{
				return this.GetMethodName(member as MethodReference);
			}
			if (member as FieldReference != null)
			{
				return this.GetFieldName(member as FieldReference);
			}
			if (member as PropertyReference != null)
			{
				return this.GetPropertyName(member as PropertyReference);
			}
			if (member as EventReference != null)
			{
				return this.GetEventName(member as EventReference);
			}
			if (member as TypeReference == null)
			{
				throw new NotSupportedException("Unexpected member type.");
			}
			return this.GetTypeName(member as TypeReference);
		}

		protected MethodSpecificContext GetMethodContext(IMemberDefinition member)
		{
			V_0 = Utilities.GetMemberUniqueName(member);
			if (!this.writerContext.get_MethodContexts().TryGetValue(V_0, out V_1))
			{
				return null;
			}
			return V_1;
		}

		protected virtual string GetMethodName(MethodReference method)
		{
			V_0 = method.Resolve();
			if (V_0 == null || !String.op_Equality(V_0.get_Module().get_FilePath(), this.get_ModuleContext().get_Module().get_FilePath()))
			{
				return Utilities.EscapeNameIfNeeded(GenericHelper.GetNonGenericName(method.get_Name()), this.get_Language());
			}
			if (this.get_TypeContext().get_MethodDefinitionToNameMap().ContainsKey(V_0))
			{
				return this.get_TypeContext().get_MethodDefinitionToNameMap().get_Item(V_0);
			}
			if (this.get_TypeContext().get_GeneratedMethodDefinitionToNameMap().ContainsKey(V_0))
			{
				return this.get_TypeContext().get_GeneratedMethodDefinitionToNameMap().get_Item(V_0);
			}
			stackVariable29 = this.get_ModuleContext().get_RenamedMembersMap();
			V_1 = V_0.get_MetadataToken();
			return stackVariable29.get_Item(V_1.ToUInt32());
		}

		protected virtual string GetPropertyName(PropertyReference property)
		{
			V_0 = property.Resolve();
			if (V_0 == null || !String.op_Equality(V_0.get_Module().get_FilePath(), this.get_ModuleContext().get_Module().get_FilePath()))
			{
				return Utilities.EscapeNameIfNeeded(GenericHelper.GetNonGenericName(property.get_Name()), this.get_Language());
			}
			try
			{
				stackVariable19 = this.get_ModuleContext().get_RenamedMembersMap();
				V_1 = V_0.get_MetadataToken();
				V_2 = stackVariable19.get_Item(V_1.ToUInt32());
			}
			catch (Exception exception_0)
			{
				V_3 = exception_0;
				V_4 = V_0.get_MetadataToken().ToUInt32();
				this.Write(V_4.ToString());
				this.WriteLine();
				V_5 = this.get_ModuleContext().get_RenamedMembersMap().get_Count();
				this.Write(V_5.ToString());
				this.WriteLine();
				V_6 = this.get_ModuleContext().get_RenamedMembersMap().GetEnumerator();
				try
				{
					while (V_6.MoveNext())
					{
						V_7 = V_6.get_Current();
						V_4 = V_7.get_Key();
						this.Write(String.Concat(V_4.ToString(), " ", V_7.get_Value()));
						this.WriteLine();
					}
				}
				finally
				{
					((IDisposable)V_6).Dispose();
				}
				throw V_3;
			}
			return V_2;
		}

		protected Statement GetStatement(MethodDefinition method)
		{
			if (this.writerContext.get_DecompiledStatements().TryGetValue(Utilities.GetMemberUniqueName(method), out V_0))
			{
				return V_0;
			}
			return null;
		}

		protected virtual string GetTypeName(TypeReference type)
		{
			if (this.IsTypeInCurrentModule(type))
			{
				return this.GetCurrentModuleTypeName(type);
			}
			return Utilities.EscapeTypeNameIfNeeded(GenericHelper.GetNonGenericName(type.get_Name()), this.get_Language());
		}

		internal virtual void Indent()
		{
			this.formatter.Indent();
			return;
		}

		protected bool IsTypeInCurrentModule(TypeReference type)
		{
			V_0 = type.Resolve();
			if (V_0 == null)
			{
				return false;
			}
			return String.op_Equality(V_0.get_Module().get_FilePath(), this.get_ModuleContext().get_Module().get_FilePath());
		}

		private void NormalizeGenericParams(Collection<GenericParameter> genericCollection, StringBuilder nameBuilder)
		{
			dummyVar0 = nameBuilder.Append("<");
			V_0 = 0;
			while (V_0 < genericCollection.get_Count())
			{
				V_1 = genericCollection.get_Item(V_0);
				dummyVar1 = nameBuilder.Append(this.GetGenericParameterName(V_1));
				if (V_0 < genericCollection.get_Count() - 1)
				{
					dummyVar2 = nameBuilder.Append(",");
				}
				V_0 = V_0 + 1;
			}
			dummyVar3 = nameBuilder.Append(">");
			return;
		}

		protected virtual string OnConvertString(string str)
		{
			return BaseLanguageWriter.ConvertString(str);
		}

		protected void OnExceptionThrown(Exception ex)
		{
			this.OnExceptionThrown(this, ex);
			return;
		}

		protected void OnExceptionThrown(object sender, Exception ex)
		{
			V_0 = this.ExceptionThrown;
			if (V_0 != null)
			{
				V_0.Invoke(sender, ex);
			}
			return;
		}

		internal virtual void Outdent()
		{
			this.formatter.Outdent();
			return;
		}

		private bool ShouldWriteAsDelegate(TypeDefinition type)
		{
			if (!this.get_Language().get_HasDelegateSpecificSyntax())
			{
				return false;
			}
			return type.IsDelegate();
		}

		private int SortEnumFields(IMemberDefinition first, IMemberDefinition second)
		{
			V_0 = first as FieldDefinition;
			V_1 = second as FieldDefinition;
			if (V_0 == null || V_0.get_Constant() == null || V_0.get_Constant().get_Value() == null)
			{
				return 1;
			}
			if (V_1 == null || V_1.get_Constant() == null || V_1.get_Constant().get_Value() == null)
			{
				return -1;
			}
			V_2 = 0;
			if (String.op_Equality(V_0.get_Constant().get_Value().GetType().get_Name(), "Int32"))
			{
				V_3 = (Int32)V_0.get_Constant().get_Value();
				V_4 = (Int32)V_1.get_Constant().get_Value();
				V_2 = V_3.CompareTo(V_4);
			}
			if (!String.op_Equality(V_0.get_Constant().get_Value().GetType().get_Name(), "UInt32"))
			{
				if (!String.op_Equality(V_0.get_Constant().get_Value().GetType().get_Name(), "Int16"))
				{
					if (!String.op_Equality(V_0.get_Constant().get_Value().GetType().get_Name(), "UInt16"))
					{
						if (!String.op_Equality(V_0.get_Constant().get_Value().GetType().get_Name(), "Byte"))
						{
							if (!String.op_Equality(V_0.get_Constant().get_Value().GetType().get_Name(), "SByte"))
							{
								if (!String.op_Equality(V_0.get_Constant().get_Value().GetType().get_Name(), "Int64"))
								{
									if (String.op_Equality(V_0.get_Constant().get_Value().GetType().get_Name(), "UInt64"))
									{
										V_17 = (UInt64)V_0.get_Constant().get_Value();
										V_18 = (UInt64)V_1.get_Constant().get_Value();
										V_2 = V_17.CompareTo(V_18);
									}
								}
								else
								{
									V_15 = (Int64)V_0.get_Constant().get_Value();
									V_16 = (Int64)V_1.get_Constant().get_Value();
									V_2 = V_15.CompareTo(V_16);
								}
							}
							else
							{
								V_13 = (SByte)V_0.get_Constant().get_Value();
								V_14 = (SByte)V_1.get_Constant().get_Value();
								V_2 = V_13.CompareTo(V_14);
							}
						}
						else
						{
							V_11 = (Byte)V_0.get_Constant().get_Value();
							V_12 = (Byte)V_1.get_Constant().get_Value();
							V_2 = V_11.CompareTo(V_12);
						}
					}
					else
					{
						V_9 = (UInt16)V_0.get_Constant().get_Value();
						V_10 = (UInt16)V_1.get_Constant().get_Value();
						V_2 = V_9.CompareTo(V_10);
					}
				}
				else
				{
					V_7 = (Int16)V_0.get_Constant().get_Value();
					V_8 = (Int16)V_1.get_Constant().get_Value();
					V_2 = V_7.CompareTo(V_8);
				}
			}
			else
			{
				V_5 = (UInt32)V_0.get_Constant().get_Value();
				V_6 = (UInt32)V_1.get_Constant().get_Value();
				V_2 = V_5.CompareTo(V_6);
			}
			if (V_2 != 0)
			{
				return V_2;
			}
			return V_0.get_Name().CompareTo(V_1.get_Name());
		}

		protected virtual void StartWritingComment()
		{
			this.isWritingComment = true;
			this.formatter.StartWritingComment();
			return;
		}

		public virtual void Stop()
		{
			this.isStopped = true;
			return;
		}

		protected virtual bool TryWriteEnumField(FieldDefinition fieldDefinition)
		{
			if (!fieldDefinition.get_DeclaringType().get_IsEnum())
			{
				return false;
			}
			V_0 = this.formatter.get_CurrentPosition();
			this.WriteReference(this.GetFieldName(fieldDefinition), fieldDefinition);
			V_2 = this.formatter.get_CurrentPosition() - 1;
			this.currentWritingInfo.get_MemberDeclarationToCodePostionMap().set_Item(fieldDefinition, new OffsetSpan(V_0, V_2));
			if (!fieldDefinition.get_DeclaringType().get_IsDefaultEnumConstants())
			{
				this.WriteLiteral(" = ");
				this.WriteLiteral(String.Format(CultureInfo.get_InvariantCulture(), "{0}", fieldDefinition.get_Constant().get_Value()));
			}
			return true;
		}

		protected void UpdateWritingInfo(WriterContext writerContext, WritingInfo writingInfo)
		{
			V_0 = writerContext.get_MethodContexts().get_Values().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1 == null)
					{
						continue;
					}
					writingInfo.get_ControlFlowGraphs().Add(V_1.get_Method(), V_1.get_ControlFlowGraph());
					writingInfo.get_MethodsVariableDefinitionToNameMap().Add(V_1.get_Method(), V_1.get_VariableDefinitionToNameMap());
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			V_2 = writerContext.get_TypeContext().get_ExceptionWhileDecompiling().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					if (writingInfo.get_MembersWithExceptions().Contains(V_3.get_MetadataToken().ToUInt32()))
					{
						continue;
					}
					stackVariable37 = writingInfo.get_MembersWithExceptions();
					V_4 = V_3.get_MetadataToken();
					dummyVar0 = stackVariable37.Add(V_4.ToUInt32());
				}
			}
			finally
			{
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
			writingInfo.get_AutoImplementedProperties().UnionWith(writerContext.get_TypeContext().get_AutoImplementedProperties());
			writingInfo.get_AutoImplementedEvents().UnionWith(writerContext.get_TypeContext().get_AutoImplementedEvents());
			V_5 = writerContext.get_TypeContext().get_GeneratedFilterMethods().GetEnumerator();
			try
			{
				while (V_5.MoveNext())
				{
					V_6 = V_5.get_Current();
					writingInfo.get_GeneratedFilterMethods().Add(V_6.get_Method());
				}
			}
			finally
			{
				if (V_5 != null)
				{
					V_5.Dispose();
				}
			}
			return;
		}

		public override void VisitExceptionStatement(ExceptionStatement node)
		{
			this.WriteException(new Exception(String.Concat("Exception in: ", node.get_Member().GetFullMemberName(this.get_Language())), node.get_ExceptionObject()), node.get_Member());
			return;
		}

		public override void VisitMemberHandleExpression(MemberHandleExpression node)
		{
			throw new NotSupportedException();
		}

		internal virtual void Write(string str)
		{
			this.formatter.Write(str);
			return;
		}

		public virtual List<WritingInfo> Write(IMemberDefinition member, IWriterContextService writerContextService)
		{
			this.writerContextService = writerContextService;
			this.writerContext = writerContextService.GetWriterContext(member, this.get_Language());
			if (member as TypeDefinition != null)
			{
				this.set_CurrentType(member as TypeDefinition);
			}
			this.currentWritingInfo = new WritingInfo(member);
			this.UpdateWritingInfo(this.writerContext, this.currentWritingInfo);
			this.writingInfos = new List<WritingInfo>();
			this.writingInfos.Add(this.currentWritingInfo);
			this.WriteInternal(member);
			return this.writingInfos;
		}

		protected virtual void Write(Statement statement)
		{
			return;
		}

		protected virtual void Write(Expression expression)
		{
			return;
		}

		protected abstract void Write(MethodDefinition member);

		protected abstract void Write(PropertyDefinition property);

		protected abstract void Write(EventDefinition event);

		protected abstract void Write(FieldDefinition field);

		protected virtual void WriteAttributes(IMemberDefinition member, IEnumerable<string> ignoredAttributes = null)
		{
			return;
		}

		protected virtual void WriteBeginBlock(bool inline = false)
		{
			return;
		}

		public virtual void WriteBody(IMemberDefinition member, IWriterContextService writerContextService)
		{
			this.writerContextService = writerContextService;
			this.currentWritingInfo = new WritingInfo(member);
			this.writingInfos = new List<WritingInfo>();
			this.writingInfos.Add(this.currentWritingInfo);
			this.writerContext = writerContextService.GetWriterContext(member, this.get_Language());
			this.WriteBodyInternal(member);
			return;
		}

		protected virtual void WriteBodyInternal(IMemberDefinition member)
		{
			return;
		}

		public virtual void WriteComment(string comment)
		{
			this.formatter.WriteComment(String.Concat(this.get_Language().get_CommentLineSymbol(), " ", comment));
			return;
		}

		internal virtual void WriteDefinition(string name, object definition)
		{
			this.formatter.WriteDefinition(name, definition);
			return;
		}

		protected virtual void WriteDelegate(TypeDefinition delegateDefinition)
		{
			return;
		}

		protected void WriteDocumentation(IMemberDefinition member)
		{
			V_0 = DocumentationManager.GetDocumentationForMember(member);
			if (!String.IsNullOrEmpty(V_0))
			{
				this.WriteDocumentation(V_0);
				this.WriteLine();
			}
			return;
		}

		private void WriteDocumentation(string memberDocumentation)
		{
			this.formatter.WriteDocumentationStartBlock();
			V_0 = false;
			V_1 = false;
			V_2 = new StringReader(memberDocumentation);
			try
			{
				V_3 = V_2.ReadLine();
				while (V_3 != null)
				{
					if (V_0)
					{
						this.WriteLine();
					}
					V_0 = true;
					this.formatter.WriteDocumentationTag(String.Concat(this.get_Language().get_DocumentationLineStarter(), " "));
					V_1 = this.WriteDocumentationLine(V_3, V_1);
					V_3 = V_2.ReadLine();
				}
			}
			finally
			{
				if (V_2 != null)
				{
					((IDisposable)V_2).Dispose();
				}
			}
			this.formatter.WriteEndBlock();
			return;
		}

		private bool WriteDocumentationLine(string currentLine, bool isDocumentTag)
		{
			V_0 = -1;
			V_1 = -1;
			if (isDocumentTag)
			{
				V_3 = 0;
				while (V_3 < currentLine.get_Length())
				{
					if (currentLine.get_Chars(V_3) != '>')
					{
						V_3 = V_3 + 1;
					}
					else
					{
						V_1 = V_3;
						break;
					}
				}
				if (V_1 == -1)
				{
					this.formatter.WriteDocumentationTag(currentLine);
					return true;
				}
				V_4 = currentLine.Substring(0, V_1 + 1);
				V_5 = currentLine.Substring(V_1 + 1);
				this.formatter.WriteDocumentationTag(V_4);
				return this.WriteDocumentationLine(V_5, false);
			}
			V_6 = 0;
			while (V_6 < currentLine.get_Length())
			{
				if (currentLine.get_Chars(V_6) != '<')
				{
					V_6 = V_6 + 1;
				}
				else
				{
					V_0 = V_6;
					break;
				}
			}
			if (V_0 == -1)
			{
				this.formatter.WriteComment(currentLine);
				return false;
			}
			V_2 = currentLine.Substring(0, V_0);
			this.formatter.WriteComment(V_2);
			V_7 = V_0 + 1;
			while (V_7 < currentLine.get_Length())
			{
				if (currentLine.get_Chars(V_7) != '>')
				{
					V_7 = V_7 + 1;
				}
				else
				{
					V_1 = V_7;
					break;
				}
			}
			if (V_1 == -1)
			{
				this.formatter.WriteDocumentationTag(currentLine.Substring(V_0));
				return true;
			}
			V_8 = currentLine.Substring(V_0, V_1 - V_0 + 1);
			this.formatter.WriteDocumentationTag(V_8);
			V_9 = currentLine.Substring(V_1 + 1);
			return this.WriteDocumentationLine(V_9, false);
		}

		protected virtual void WriteEndBlock(string statementName)
		{
			return;
		}

		protected virtual void WriteEndOfStatement()
		{
			return;
		}

		protected virtual void WriteEnumValueSeparator()
		{
			return;
		}

		public virtual void WriteEscapeLiteral(string literal)
		{
			this.formatter.WriteLiteral(this.OnConvertString(literal));
			return;
		}

		protected abstract void WriteEventDeclaration(EventDefinition event);

		public virtual void WriteException(Exception ex, IMemberDefinition member)
		{
			V_0 = this.exceptionFormatter.Format(ex, member.get_FullName(), member.get_DeclaringType().get_Module().get_FilePath());
			if (!this.get_Settings().get_WriteExceptionsAsComments())
			{
				this.formatter.WriteException(V_0);
				this.formatter.WriteExceptionMailToLink("mailto: JustDecompilePublicFeedback@telerik.com", V_0);
				return;
			}
			V_1 = V_0.Select<string, string>(new Func<string, string>(this.u003cWriteExceptionu003eb__69_0)).ToArray<string>();
			this.formatter.WriteException(V_1);
			V_2 = String.Concat(this.get_Language().get_CommentLineSymbol(), " mailto: JustDecompilePublicFeedback@telerik.com");
			this.formatter.WriteExceptionMailToLink(V_2, V_0);
			return;
		}

		internal virtual void WriteIdentifier(string name, object identifier)
		{
			this.formatter.WriteIdentifier(name, identifier);
			return;
		}

		protected virtual void WriteInternal(IMemberDefinition member)
		{
			V_0 = member.get_MetadataToken().ToUInt32();
			if (this.isStopped)
			{
				return;
			}
			this.membersStack.Push(member);
			if (member as TypeDefinition == null)
			{
				stackVariable11 = false;
			}
			else
			{
				stackVariable11 = member != this.get_CurrentType();
			}
			V_1 = stackVariable11;
			if (!V_1)
			{
				this.formatter.PreserveIndent(member);
			}
			try
			{
				if (this.get_Settings().get_WriteDocumentation() && !V_1)
				{
					V_3 = this.formatter.get_CurrentPosition();
					this.WriteDocumentation(member);
					this.AddMemberDocumentationOffsetSpan(V_0, V_3, this.formatter.get_CurrentPosition());
				}
				if (member as TypeDefinition == null)
				{
					V_4 = this.formatter.get_CurrentPosition();
					this.WriteAttributes(member, null);
					this.AddMemberAttributesOffsetSpan(V_0, V_4, this.formatter.get_CurrentPosition());
					V_5 = this.formatter.get_CurrentPosition();
					if (member as MethodDefinition == null)
					{
						if (member as PropertyDefinition == null)
						{
							if (member as EventDefinition == null)
							{
								if (member as FieldDefinition != null)
								{
									this.formatter.WriteMemberDeclaration(member);
									this.Write((FieldDefinition)member);
								}
							}
							else
							{
								this.formatter.WriteMemberDeclaration(member);
								this.Write((EventDefinition)member);
							}
						}
						else
						{
							this.formatter.WriteMemberDeclaration(member);
							this.Write((PropertyDefinition)member);
						}
					}
					else
					{
						this.formatter.WriteMemberDeclaration(member);
						this.Write((MethodDefinition)member);
					}
					this.currentWritingInfo.get_MemberTokenToDecompiledCodeMap().Add(V_0, new OffsetSpan(V_5, this.formatter.get_CurrentPosition() - 1));
				}
				else
				{
					this.formatter.WriteMemberDeclaration(member);
					this.WriteTypeInANewWriterIfNeeded((TypeDefinition)member);
				}
			}
			catch (Exception exception_0)
			{
				V_6 = exception_0;
				this.Visit(new ExceptionStatement(V_6, member));
				this.formatter.RestoreIndent(member);
				dummyVar0 = this.currentWritingInfo.ExceptionsWhileWriting.Add(member);
				dummyVar1 = this.currentWritingInfo.get_MembersWithExceptions().Add(V_0);
				if (member as PropertyDefinition != null)
				{
					V_8 = member as PropertyDefinition;
					if (V_8.get_GetMethod() != null)
					{
						dummyVar2 = this.currentWritingInfo.ExceptionsWhileWriting.Add(V_8.get_GetMethod());
					}
					if (V_8.get_SetMethod() != null)
					{
						dummyVar3 = this.currentWritingInfo.ExceptionsWhileWriting.Add(V_8.get_SetMethod());
					}
				}
				if (member as EventDefinition != null)
				{
					V_9 = member as EventDefinition;
					if (V_9.get_AddMethod() != null)
					{
						dummyVar4 = this.currentWritingInfo.ExceptionsWhileWriting.Add(V_9.get_AddMethod());
					}
					if (V_9.get_RemoveMethod() != null)
					{
						dummyVar5 = this.currentWritingInfo.ExceptionsWhileWriting.Add(V_9.get_RemoveMethod());
					}
					if (V_9.get_InvokeMethod() != null)
					{
						dummyVar6 = this.currentWritingInfo.ExceptionsWhileWriting.Add(V_9.get_InvokeMethod());
					}
				}
				this.OnExceptionThrown(V_6);
			}
			if (member as TypeDefinition == null || member == this.get_CurrentType())
			{
				this.formatter.RemovePreservedIndent(member);
			}
			dummyVar7 = this.membersStack.Pop();
			return;
		}

		internal virtual void WriteKeyword(string keyword)
		{
			this.formatter.WriteKeyword(keyword);
			return;
		}

		internal virtual void WriteLine()
		{
			this.formatter.WriteLine();
			return;
		}

		internal virtual void WriteLiteral(string literal)
		{
			this.formatter.WriteLiteral(literal);
			return;
		}

		protected virtual bool WriteMemberDeclaration(IMemberDefinition member)
		{
			if (member as MethodDefinition != null)
			{
				this.WriteAttributes(member, null);
				this.WriteMethodDeclaration((MethodDefinition)member, false);
				this.WriteEndOfStatement();
				return true;
			}
			if (member as PropertyDefinition != null)
			{
				this.WriteAttributes(member, null);
				this.WritePropertyDeclaration((PropertyDefinition)member);
				this.WriteEndOfStatement();
				return true;
			}
			if (member as EventDefinition != null)
			{
				this.WriteAttributes(member, null);
				this.WriteEventDeclaration((EventDefinition)member);
				this.WriteEndOfStatement();
				return true;
			}
			if (member as FieldDefinition != null)
			{
				V_0 = member as FieldDefinition;
				if (V_0.get_IsSpecialName())
				{
					return false;
				}
				if (this.TryWriteEnumField(V_0))
				{
					return true;
				}
				this.WriteAttributes(member, null);
				this.Write((FieldDefinition)member);
				return true;
			}
			if (member as TypeDefinition == null)
			{
				return true;
			}
			V_1 = member as TypeDefinition;
			if (!this.ShouldWriteAsDelegate(V_1))
			{
				this.WriteTypeDeclarationsOnlyInternal(V_1);
				return true;
			}
			this.WriteAttributes(V_1, null);
			this.WriteDelegate(V_1);
			return true;
		}

		public void WriteMemberEscapedOnlyName(object memberDefinition)
		{
			V_0 = this.GetMemberEscapedOnlyName(memberDefinition);
			this.formatter.Write(V_0);
			return;
		}

		public abstract void WriteMemberNavigationName(object memberDefinition);

		public abstract void WriteMemberNavigationPathFullName(object member);

		protected abstract void WriteMethodDeclaration(MethodDefinition method, bool writeDocumentation = false);

		protected virtual void WriteNamespace(object reference, bool forceWriteNamespace = false)
		{
			return;
		}

		public virtual void WriteNamespaceNavigationName(string memberReferenceName)
		{
			stackVariable2 = new Char[1];
			stackVariable2[0] = '.';
			V_0 = memberReferenceName.Split(stackVariable2);
			V_1 = new StringBuilder((int)V_0.Length);
			V_2 = 0;
			while (V_2 < (int)V_0.Length)
			{
				V_3 = V_0[V_2];
				if (this.get_Language().IsValidIdentifier(V_3) || !this.get_Settings().get_RenameInvalidMembers())
				{
					dummyVar1 = V_1.Append(V_3);
				}
				else
				{
					dummyVar0 = V_1.Append(this.get_Language().ReplaceInvalidCharactersInIdentifier(V_3));
				}
				if (V_2 < (int)V_0.Length - 1)
				{
					dummyVar2 = V_1.Append(".");
				}
				V_2 = V_2 + 1;
			}
			this.formatter.Write(V_1.ToString());
			return;
		}

		protected virtual void WriteNestedTypeWriteLine()
		{
			return;
		}

		internal virtual void WriteNotResolvedReference(string name, MemberReference memberReference, string errorMessage)
		{
			this.formatter.WriteNotResolvedReference(name, memberReference, errorMessage);
			return;
		}

		protected void WritePartialType(TypeDefinition type, ICollection<string> fieldsToSkip = null)
		{
			if (this.isStopped)
			{
				return;
			}
			stackVariable5 = new String[1];
			stackVariable5[0] = "System.CodeDom.Compiler.GeneratedCodeAttribute";
			this.WriteAttributes(type, stackVariable5);
			this.WritePartialType(type, new Action<IMemberDefinition>(this.u003cWritePartialTypeu003eb__111_0), false, fieldsToSkip);
			return;
		}

		protected virtual void WritePartialType(TypeDefinition type, Action<IMemberDefinition> writeMember, bool writeNewLine, ICollection<string> fieldsToSkip = null)
		{
			V_0 = String.Empty;
			V_0 = this.WriteTypeDeclaration(type, true);
			this.WriteBeginBlock(false);
			this.WriteTypeOpeningBlock(type);
			stackVariable13 = new String[1];
			stackVariable13[0] = "System.Diagnostics.DebuggerNonUserCodeAttribute";
			this.WriteTypeMembers(type, writeMember, stackVariable13, fieldsToSkip);
			this.WriteLine();
			this.Outdent();
			this.WriteEndBlock(V_0);
			return;
		}

		protected abstract void WritePropertyDeclaration(PropertyDefinition property);

		internal virtual void WriteReference(string name, object reference)
		{
			this.formatter.WriteReference(name, reference);
			return;
		}

		internal virtual void WriteSpace()
		{
			this.formatter.WriteSpace();
			return;
		}

		internal virtual void WriteToken(string token)
		{
			this.formatter.WriteToken(token);
			return;
		}

		protected void WriteType(TypeDefinition type)
		{
			if (this.isStopped)
			{
				return;
			}
			this.membersStack.Push(type);
			V_0 = this.formatter.get_CurrentPosition();
			stackVariable11 = new String[1];
			stackVariable11[0] = "System.Reflection.DefaultMemberAttribute";
			this.WriteAttributes(type, stackVariable11);
			stackVariable16 = this.currentWritingInfo.get_MemberTokenToAttributesMap();
			V_2 = type.get_MetadataToken();
			stackVariable16.Add(V_2.ToUInt32(), new OffsetSpan(V_0, this.formatter.get_CurrentPosition() - 1));
			V_1 = this.formatter.get_CurrentPosition();
			this.WriteTypeInternal(type, new Action<IMemberDefinition>(this.u003cWriteTypeu003eb__74_0));
			stackVariable38 = this.currentWritingInfo.get_MemberTokenToDecompiledCodeMap();
			V_2 = type.get_MetadataToken();
			stackVariable38.Add(V_2.ToUInt32(), new OffsetSpan(V_1, this.formatter.get_CurrentPosition() - 1));
			dummyVar0 = this.membersStack.Pop();
			return;
		}

		public void WriteTypeDeclaration(TypeDefinition type, IWriterContextService writerContextService)
		{
			this.writerContextService = writerContextService;
			this.writerContext = writerContextService.GetWriterContext(type, this.get_Language());
			this.currentWritingInfo = new WritingInfo(type);
			stackVariable14 = new String[1];
			stackVariable14[0] = "System.Reflection.DefaultMemberAttribute";
			this.WriteAttributes(type, stackVariable14);
			this.membersStack.Push(type);
			if (this.ShouldWriteAsDelegate(type))
			{
				this.WriteDelegate(type);
				this.WriteLine();
				dummyVar0 = this.membersStack.Pop();
				return;
			}
			dummyVar1 = this.WriteTypeDeclaration(type, false);
			this.WriteLine();
			dummyVar2 = this.membersStack.Pop();
			return;
		}

		protected abstract string WriteTypeDeclaration(TypeDefinition type, bool isPartial = false);

		public virtual void WriteTypeDeclarationsOnly(TypeDefinition member, IWriterContextService writerContextService)
		{
			this.writerContextService = writerContextService;
			this.writerContext = writerContextService.GetWriterContext(member, this.get_Language());
			this.currentWritingInfo = new WritingInfo(member);
			this.WriteTypeDeclarationsOnlyInternal(member);
			return;
		}

		protected virtual void WriteTypeDeclarationsOnlyInternal(TypeDefinition member)
		{
			this.WriteTypeNamespaceStart(member);
			this.WriteAttributes(member, null);
			this.WriteTypeInternal(member, new Action<IMemberDefinition>(this.u003cWriteTypeDeclarationsOnlyInternalu003eb__68_0));
			this.WriteTypeNamespaceEnd(member);
			return;
		}

		protected virtual void WriteTypeInANewWriterIfNeeded(TypeDefinition type)
		{
			if ((object)this.get_CurrentType() == (object)type)
			{
				this.WriteType(type);
				return;
			}
			stackVariable13 = this.get_Language().GetWriter(this.formatter, this.exceptionFormatter, this.get_Settings());
			stackVariable13.add_ExceptionThrown(new EventHandler<Exception>(this.OnExceptionThrown));
			V_0 = stackVariable13.Write(type, this.writerContextService);
			stackVariable13.remove_ExceptionThrown(new EventHandler<Exception>(this.OnExceptionThrown));
			this.writingInfos.AddRange(V_0);
			return;
		}

		protected virtual void WriteTypeInternal(TypeDefinition type, Action<IMemberDefinition> writeMember)
		{
			V_0 = String.Empty;
			this.membersStack.Push(type);
			if (this.ShouldWriteAsDelegate(type))
			{
				this.WriteDelegate(type);
				return;
			}
			V_0 = this.WriteTypeDeclaration(type, false);
			V_1 = this.formatter.get_CurrentPosition();
			this.formatter.WriteStartBlock();
			this.WriteBeginBlock(false);
			this.WriteTypeOpeningBlock(type);
			this.WriteTypeMembers(type, writeMember, null, null);
			this.WriteLine();
			this.Outdent();
			this.WriteEndBlock(V_0);
			dummyVar0 = this.membersStack.Pop();
			this.currentWritingInfo.get_MemberDefinitionToFoldingPositionMap().set_Item(type, new OffsetSpan(V_1, this.formatter.get_CurrentPosition() - 1));
			this.formatter.WriteEndBlock();
			return;
		}

		private void WriteTypeMembers(TypeDefinition type, Action<IMemberDefinition> writeMember, IEnumerable<string> attributesToSkip = null, ICollection<string> fieldsToSkip = null)
		{
			V_0 = Utilities.GetTypeMembers(type, this.get_Language(), this.get_Settings().get_ShowCompilerGeneratedMembers(), attributesToSkip, fieldsToSkip, this.currentWritingInfo.get_GeneratedFilterMethods(), this.get_TypeContext().GetFieldToPropertyMap(this.get_Language()).get_Keys());
			if (type.get_IsEnum())
			{
				V_0.Sort(new Comparison<IMemberDefinition>(this.u003cWriteTypeMembersu003eb__78_0));
			}
			V_1 = 0;
			while (V_1 < V_0.get_Count())
			{
				V_2 = null;
				if (this.isStopped)
				{
					return;
				}
				V_2 = V_0.get_Item(V_1);
				if (V_2 as FieldDefinition == null || !(V_2 as FieldDefinition).get_IsSpecialName())
				{
					this.membersStack.Push(V_2);
					V_3 = V_1 == V_0.get_Count() - 1;
					writeMember.Invoke(V_2);
					if (!type.get_IsEnum())
					{
						if (!V_3)
						{
							this.WriteLine();
							this.WriteLine();
						}
					}
					else
					{
						if (!V_3 && String.op_Inequality(V_0.get_Item(V_1 + 1).get_Name(), "value__"))
						{
							this.WriteEnumValueSeparator();
							this.WriteLine();
						}
					}
					dummyVar0 = this.membersStack.Pop();
				}
				V_1 = V_1 + 1;
			}
			return;
		}

		protected virtual void WriteTypeNamespaceEnd(TypeDefinition member)
		{
			return;
		}

		protected virtual void WriteTypeNamespaceStart(TypeDefinition member)
		{
			return;
		}

		protected virtual void WriteTypeOpeningBlock(TypeDefinition type)
		{
			return;
		}

		public event EventHandler<Exception> ExceptionThrown;
	}
}