using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.Languages
{
	public abstract class NamespaceImperativeLanguageWriter : BaseImperativeLanguageWriter, INamespaceLanguageWriter, ILanguageWriter, IExceptionThrownNotifier
	{
		protected const string CastToObjectResolvementError = "The cast to object might be unnecessary. Please, locate the assembly where \"{0}\" is defined.";

		protected string currentNamespace;

		protected bool writeNamespacesandUsings;

		public IEnumerable<string> AssemblyInfoNamespacesUsed
		{
			get
			{
				return this.AssemblyContext.AssemblyNamespaceUsings.Union<string>(this.ModuleContext.ModuleNamespaceUsings);
			}
		}

		public NamespaceImperativeLanguageWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings) : base(language, formatter, exceptionFormatter, settings)
		{
			this.writeNamespacesandUsings = false;
		}

		private bool CheckForSpecialName(TypeReference reference)
		{
			if (reference.HasGenericParameters)
			{
				return false;
			}
			return reference.Name != this.ToTypeString(reference);
		}

		protected abstract void DoWriteParameterTypeAndName(TypeReference type, string name, ParameterDefinition reference);

		protected abstract void DoWriteTypeAndName(TypeReference typeReference, string name, object reference);

		protected abstract void DoWriteTypeAndName(TypeReference typeReference, string name);

		protected abstract void DoWriteVariableTypeAndName(VariableDefinition variable);

		private string GetCurrentNamespace(TypeDefinition type)
		{
			return Utilities.GetOuterMostDeclaringType(type).Namespace;
		}

		public void WriteAssemblyAndModuleUsings()
		{
			this.WriteUsings(Utilities.GetAssemblyAndModuleNamespaceUsings(this.AssemblyContext, this.ModuleContext));
		}

		public void WriteAssemblyUsings()
		{
			this.WriteUsings(this.AssemblyContext.AssemblyNamespaceUsings);
		}

		public override void WriteBody(IMemberDefinition member, IWriterContextService writerContextService)
		{
			this.writerContextService = writerContextService;
			this.writerContext = writerContextService.GetWriterContext(member, base.Language);
			this.currentWritingInfo = new WritingInfo(member);
			base.UpdateWritingInfo(this.writerContext, this.currentWritingInfo);
			this.writingInfos = new List<WritingInfo>()
			{
				this.currentWritingInfo
			};
			this.WriteBodyInternal(member);
		}

		protected override void WriteBodyInternal(IMemberDefinition member)
		{
			this.membersStack.Push(member);
			this.currentNamespace = member.DeclaringType.Namespace;
			base.WriteBodyInternal(member);
			this.membersStack.Pop();
		}

		internal void WriteEnumValueField(FieldDefinition field)
		{
			this.WriteReference(this.GetFieldName(field), field);
		}

		protected virtual void WriteMethodReference(string name, MethodReference reference)
		{
			base.WriteReference(name, reference);
		}

		public void WriteModuleUsings()
		{
			this.WriteUsings(this.ModuleContext.ModuleNamespaceUsings);
		}

		protected override void WriteNamespace(object reference, bool forceWriteNamespace = false)
		{
			if (!(reference is TypeReference))
			{
				base.WriteNamespace(reference, forceWriteNamespace);
				return;
			}
			this.WriteNamespaceIfNeeded(reference as TypeReference, forceWriteNamespace);
		}

		private void WriteNamespaceDeclaration(string @namespace)
		{
			if (this.ModuleContext.RenamedNamespacesMap.ContainsKey(@namespace))
			{
				this.WriteComment(@namespace);
				this.WriteLine();
				@namespace = this.ModuleContext.RenamedNamespacesMap[@namespace];
			}
			this.WriteKeyword(base.KeyWordWriter.Namespace);
			this.WriteSpace();
			this.Write(Utilities.EscapeNamespaceIfNeeded(@namespace, base.Language));
			this.formatter.WriteNamespaceStartBlock();
			this.WriteBeginBlock(false);
			this.WriteLine();
		}

		private void WriteNamespaceIfNeeded(TypeReference reference, bool forceWriteNamespace = false)
		{
			if (!forceWriteNamespace && (!base.Settings.WriteFullyQualifiedNames || this.CheckForSpecialName(reference)))
			{
				return;
			}
			string empty = String.Empty;
			if (forceWriteNamespace)
			{
				empty = reference.Namespace;
			}
			else if (reference.Namespace != this.currentNamespace)
			{
				empty = reference.Namespace;
			}
			if (this.ModuleContext.RenamedNamespacesMap.ContainsKey(empty))
			{
				empty = this.ModuleContext.RenamedNamespacesMap[empty];
			}
			if (empty != String.Empty)
			{
				empty = String.Concat(empty, ".");
			}
			this.Write(Utilities.EscapeNamespaceIfNeeded(empty, base.Language));
		}

		public override void WriteNamespaceIfTypeInCollision(TypeReference reference)
		{
			if (base.Settings.WriteFullyQualifiedNames)
			{
				return;
			}
			base.WriteNamespaceIfTypeInCollision(reference);
		}

		protected sealed override void WriteParameterTypeAndName(TypeReference type, string name, ParameterDefinition reference)
		{
			this.DoWriteParameterTypeAndName(type, name, reference);
		}

		public List<WritingInfo> WritePartialTypeAndNamespaces(TypeDefinition type, IWriterContextService writerContextService, Dictionary<string, ICollection<string>> fieldsToSkip = null)
		{
			this.writerContextService = writerContextService;
			this.writerContext = writerContextService.GetWriterContext(type, base.Language);
			base.CurrentType = type;
			this.currentWritingInfo = new WritingInfo(type);
			base.UpdateWritingInfo(this.writerContext, this.currentWritingInfo);
			this.writingInfos = new List<WritingInfo>()
			{
				this.currentWritingInfo
			};
			this.WritePartialTypeAndNamespacesInternal(type, fieldsToSkip);
			return this.writingInfos;
		}

		public void WritePartialTypeAndNamespacesInternal(TypeDefinition type, Dictionary<string, ICollection<string>> fieldsToSkip = null)
		{
			this.writeNamespacesandUsings = true;
			this.currentNamespace = type.Namespace;
			ICollection<string> item = null;
			if (fieldsToSkip.ContainsKey(type.FullName))
			{
				item = fieldsToSkip[type.FullName];
			}
			if (this.TypeContext.UsedNamespaces.Count<string>() > 0)
			{
				this.WriteUsings(this.TypeContext.UsedNamespaces);
				this.WriteLine();
				this.WriteLine();
			}
			this.WriteTypeNamespaceStart(type);
			base.WritePartialType(type, item);
			this.WriteTypeNamespaceEnd(type);
		}

		internal override void WriteReference(string name, object reference)
		{
			if (reference is TypeReference)
			{
				this.WriteNamespaceIfNeeded(reference as TypeReference, false);
				this.WriteTypeReference(name, reference as TypeReference);
				return;
			}
			if (!(reference is MethodReference))
			{
				base.WriteReference(name, reference);
				return;
			}
			this.WriteMethodReference(name, reference as MethodReference);
		}

		public void WriteSecurityDeclarationNamespaceIfNeeded()
		{
			if (base.Settings.WriteFullyQualifiedNames)
			{
				this.Write("System.Security.Permissions");
				this.WriteToken(".");
			}
		}

		public List<WritingInfo> WriteType(TypeDefinition type, IWriterContextService writerContextService)
		{
			this.writeNamespacesandUsings = false;
			this.currentNamespace = this.GetCurrentNamespace(type);
			return base.Write(type, writerContextService);
		}

		protected sealed override void WriteTypeAndName(TypeReference typeReference, string name, object reference)
		{
			this.DoWriteTypeAndName(typeReference, name, reference);
		}

		protected sealed override void WriteTypeAndName(TypeReference typeReference, string name)
		{
			this.DoWriteTypeAndName(typeReference, name);
		}

		public List<WritingInfo> WriteTypeAndNamespaces(TypeDefinition type, IWriterContextService writerContextService)
		{
			this.writerContextService = writerContextService;
			this.writerContext = writerContextService.GetWriterContext(type, base.Language);
			base.CurrentType = type;
			this.currentNamespace = this.GetCurrentNamespace(type);
			this.currentWritingInfo = new WritingInfo(type);
			base.UpdateWritingInfo(this.writerContext, this.currentWritingInfo);
			this.writingInfos = new List<WritingInfo>()
			{
				this.currentWritingInfo
			};
			this.WriteTypeAndNamespacesInternal(type);
			return this.writingInfos;
		}

		protected void WriteTypeAndNamespacesInternal(TypeDefinition type)
		{
			this.writeNamespacesandUsings = true;
			this.currentNamespace = this.GetCurrentNamespace(type);
			if (this.TypeContext.UsedNamespaces.Count<string>() > 0)
			{
				this.WriteUsings(this.TypeContext.UsedNamespaces);
				this.WriteLine();
				this.WriteLine();
			}
			this.WriteTypeNamespaceStart(type);
			this.WriteInternal(type);
			this.WriteTypeNamespaceEnd(type);
		}

		protected override void WriteTypeInANewWriterIfNeeded(TypeDefinition type)
		{
			if (base.CurrentType == type)
			{
				base.WriteType(type);
				return;
			}
			ILanguageWriter writer = base.Language.GetWriter(this.formatter, this.exceptionFormatter, base.Settings);
			writer.ExceptionThrown += new EventHandler<Exception>(this.OnExceptionThrown);
			List<WritingInfo> writingInfos = (writer as NamespaceImperativeLanguageWriter).WriteType(type, this.writerContextService);
			writer.ExceptionThrown -= new EventHandler<Exception>(this.OnExceptionThrown);
			this.writingInfos.AddRange(writingInfos);
		}

		protected sealed override void WriteTypeNamespaceEnd(TypeDefinition type)
		{
			if (this.writeNamespacesandUsings && type.Namespace != String.Empty)
			{
				this.WriteLine();
				this.Outdent();
				this.WriteEndBlock(base.KeyWordWriter.Namespace);
				this.formatter.WriteNamespaceEndBlock();
			}
		}

		protected sealed override void WriteTypeNamespaceStart(TypeDefinition type)
		{
			this.currentNamespace = type.GetNamespace();
			if (this.writeNamespacesandUsings && type.Namespace != String.Empty)
			{
				this.WriteNamespaceDeclaration(this.currentNamespace);
				this.Indent();
			}
		}

		protected virtual void WriteTypeReference(string name, TypeReference reference)
		{
			base.WriteReference(name, reference);
		}

		public void WriteUsings(ICollection<string> usedNamespaces)
		{
			string[] array = usedNamespaces.ToArray<string>();
			Array.Sort<string>(array);
			bool flag = true;
			string[] strArray = array;
			for (int i = 0; i < (int)strArray.Length; i++)
			{
				string str = strArray[i];
				if (!flag)
				{
					this.WriteLine();
				}
				this.WriteKeyword(base.KeyWordWriter.NamespaceUsing);
				this.WriteSpace();
				if (flag)
				{
					this.formatter.WriteStartUsagesBlock();
					flag = false;
				}
				string item = str;
				if (this.ModuleContext.RenamedNamespacesMap.ContainsKey(str))
				{
					item = this.ModuleContext.RenamedNamespacesMap[str];
				}
				this.Write(Utilities.EscapeNamespaceIfNeeded(item, base.Language));
				this.WriteEndOfStatement();
			}
			this.formatter.WriteEndUsagesBlock();
		}

		protected sealed override void WriteVariableTypeAndName(VariableDefinition variable)
		{
			this.DoWriteVariableTypeAndName(variable);
		}
	}
}