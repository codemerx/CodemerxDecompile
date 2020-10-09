using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.Decompiler;
using Mono.Cecil.Cil;
/* AGPL */
using JustDecompiler.Shared;
/* End AGPL */

namespace Telerik.JustDecompiler.Languages
{
	public abstract class NamespaceImperativeLanguageWriter : BaseImperativeLanguageWriter, INamespaceLanguageWriter
	{
        protected const string CastToObjectResolvementError = "The cast to object might be unnecessary. Please, locate the assembly where \"{0}\" is defined.";
        
		protected string currentNamespace;
		protected bool writeNamespacesandUsings;

		public IEnumerable<string> AssemblyInfoNamespacesUsed
		{
			get
			{
				return AssemblyContext.AssemblyNamespaceUsings.Union(ModuleContext.ModuleNamespaceUsings);
			}
		}

		public NamespaceImperativeLanguageWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
			: base(language, formatter, exceptionFormatter, settings)
		{
			writeNamespacesandUsings = false;
		}

		public List<WritingInfo> WriteTypeAndNamespaces(TypeDefinition type, IWriterContextService writerContextService)
		{
			this.writerContextService = writerContextService;
			this.writerContext = writerContextService.GetWriterContext(type, Language);
			this.CurrentType = type;
			this.currentNamespace = GetCurrentNamespace(type);
			this.currentWritingInfo = new WritingInfo(type);
			UpdateWritingInfo(this.writerContext, this.currentWritingInfo);
			this.writingInfos = new List<WritingInfo>();
			this.writingInfos.Add(this.currentWritingInfo);
			WriteTypeAndNamespacesInternal(type);
            return writingInfos;
		}

		public override void WriteNamespaceIfTypeInCollision(TypeReference reference)
		{
			if (this.Settings.WriteFullyQualifiedNames)
			{
				return;
			}

			base.WriteNamespaceIfTypeInCollision(reference);
		}

		protected void WriteTypeAndNamespacesInternal(TypeDefinition type)
		{
			this.writeNamespacesandUsings = true;
			this.currentNamespace = GetCurrentNamespace(type);

			if (TypeContext.UsedNamespaces.Count() > 0)
			{
				WriteUsings(TypeContext.UsedNamespaces);
				WriteLine();
				WriteLine();
			}

			WriteTypeNamespaceStart(type);
			WriteInternal(type);
			WriteTypeNamespaceEnd(type);
		}

		private string GetCurrentNamespace(TypeDefinition type)
		{
			TypeDefinition outerMostDeclaringType = Utilities.GetOuterMostDeclaringType(type);
			return outerMostDeclaringType.Namespace;
		}

		public List<WritingInfo> WriteType(TypeDefinition type, IWriterContextService writerContextService)
		{
			this.writeNamespacesandUsings = false;
			this.currentNamespace = GetCurrentNamespace(type);
			return base.Write(type, writerContextService);
		}

		public List<WritingInfo> WritePartialTypeAndNamespaces(TypeDefinition type, IWriterContextService writerContextService, Dictionary<string, ICollection<string>> fieldsToSkip = null)
		{
			this.writerContextService = writerContextService;
			this.writerContext = writerContextService.GetWriterContext(type, Language);
			this.CurrentType = type;
			this.currentWritingInfo = new WritingInfo(type);
			UpdateWritingInfo(this.writerContext, this.currentWritingInfo);
			this.writingInfos = new List<WritingInfo>();
			this.writingInfos.Add(this.currentWritingInfo);
			WritePartialTypeAndNamespacesInternal(type, fieldsToSkip);
            return writingInfos;
		}

		public void WritePartialTypeAndNamespacesInternal(TypeDefinition type, Dictionary<string, ICollection<string>> fieldsToSkip = null)
		{ 			
			this.writeNamespacesandUsings = true;
			currentNamespace = type.Namespace;
			ICollection<string> typeFieldsToSkip = null;
			if (fieldsToSkip.ContainsKey(type.FullName))
			{
				typeFieldsToSkip = fieldsToSkip[type.FullName];
			}

			if (TypeContext.UsedNamespaces.Count() > 0)
			{
				WriteUsings(TypeContext.UsedNamespaces);
				WriteLine();
				WriteLine();
			}

			WriteTypeNamespaceStart(type);
			WritePartialType(type, fieldsToSkip: typeFieldsToSkip);
			WriteTypeNamespaceEnd(type);
		}

		private void WriteNamespaceDeclaration(string @namespace)
		{
			if (ModuleContext.RenamedNamespacesMap.ContainsKey(@namespace))
			{
				WriteComment(@namespace);
				WriteLine();
				@namespace = ModuleContext.RenamedNamespacesMap[@namespace];
			}

			WriteKeyword(KeyWordWriter.Namespace);
			WriteSpace();
			Write(Utilities.EscapeNamespaceIfNeeded(@namespace, this.Language));
			this.formatter.WriteNamespaceStartBlock();
			WriteBeginBlock();
			WriteLine();
		}

		public void WriteSecurityDeclarationNamespaceIfNeeded()
		{
			if (this.Settings.WriteFullyQualifiedNames)
			{
				Write("System.Security.Permissions");
				WriteToken(".");
			}
		}

		public void WriteUsings(ICollection<string> usedNamespaces)
		{
			string[] namespaces = usedNamespaces.ToArray();
			Array.Sort(namespaces);

			bool isFirstUsing = true;
			foreach (string @namespace in namespaces)
			{
				if (!isFirstUsing)
				{
					WriteLine();
				}

				WriteKeyword(KeyWordWriter.NamespaceUsing);
				WriteSpace();

				if (isFirstUsing)
				{
					this.formatter.WriteStartUsagesBlock();
					isFirstUsing = false;
				}

				string namespaceName = @namespace;
				if (ModuleContext.RenamedNamespacesMap.ContainsKey(@namespace))
				{
					namespaceName = ModuleContext.RenamedNamespacesMap[@namespace];
				}
				Write(Utilities.EscapeNamespaceIfNeeded(namespaceName, Language));
				WriteEndOfStatement();
			}

			this.formatter.WriteEndUsagesBlock();
		}

		public void WriteAssemblyUsings()
		{
			WriteUsings(AssemblyContext.AssemblyNamespaceUsings);
		}

		public void WriteModuleUsings()
		{
			WriteUsings(ModuleContext.ModuleNamespaceUsings);
		}

		public void WriteAssemblyAndModuleUsings()
		{
			WriteUsings(Utilities.GetAssemblyAndModuleNamespaceUsings(this.AssemblyContext, this.ModuleContext));
		}

		protected sealed override void WriteTypeNamespaceStart(TypeDefinition type)
		{
			currentNamespace = type.GetNamespace();
			if (writeNamespacesandUsings && type.Namespace != String.Empty)
			{
				WriteNamespaceDeclaration(currentNamespace);
				Indent();
			}			
		}

		protected sealed override void WriteTypeNamespaceEnd(TypeDefinition type)
		{
			if (writeNamespacesandUsings && type.Namespace != String.Empty)
			{
				WriteLine();
				Outdent();
				WriteEndBlock(KeyWordWriter.Namespace);
				this.formatter.WriteNamespaceEndBlock();
			}
		}

		protected override void WriteTypeInANewWriterIfNeeded(TypeDefinition type)
		{
			if (this.CurrentType != type)
			{
				ILanguageWriter writer = Language.GetWriter(this.formatter, this.exceptionFormatter, this.Settings);
                writer.ExceptionThrown += OnExceptionThrown;
				List<WritingInfo> nestedWritingInfos = (writer as NamespaceImperativeLanguageWriter).WriteType(type, writerContextService);
                writer.ExceptionThrown -= OnExceptionThrown;
                this.writingInfos.AddRange(nestedWritingInfos);
			}
			else
			{
				WriteType(type);
			}
		}

		/* AGPL */
		protected override sealed void WriteTypeAndName(TypeReference typeReference, string name, object reference, TypeReferenceType typeReferenceType)
		{
			DoWriteTypeAndName(typeReference, name, reference, typeReferenceType);
		}

		protected override sealed void WriteTypeAndName(TypeReference typeReference, string name, TypeReferenceType typeReferenceType)
		{
			DoWriteTypeAndName(typeReference, name, typeReferenceType);			
		}
		/* End AGPL */

		protected override sealed void WriteVariableTypeAndName(VariableDefinition variable)
        {
            DoWriteVariableTypeAndName(variable);
        }

        protected override sealed void WriteParameterTypeAndName(TypeReference type, string name, ParameterDefinition reference)
		{
			DoWriteParameterTypeAndName(type, name, reference);
		}

		protected override void WriteNamespace(object reference, bool forceWriteNamespace = false)
		{
			if (reference is TypeReference)
			{
				WriteNamespaceIfNeeded(reference as TypeReference, forceWriteNamespace);
				return;
			}
			base.WriteNamespace(reference, forceWriteNamespace);
		}

		internal void WriteEnumValueField(FieldDefinition field)
		{
			string fieldName = GetFieldName(field);
			this.WriteReference(fieldName, field);
		}

		internal override void WriteReference(string name, object reference)
		{
			if (reference is TypeReference)
			{
				WriteNamespaceIfNeeded(reference as TypeReference);
				WriteTypeReference(name, reference as TypeReference);
				return;
			}
			if (reference is MethodReference)
			{
				WriteMethodReference(name, reference as MethodReference);
				return;
			}
			base.WriteReference(name, reference);
		}

		protected virtual void WriteMethodReference(string name, MethodReference reference)
		{
			base.WriteReference(name, reference);
		}

		protected virtual void WriteTypeReference(string name, TypeReference reference)
		{
			base.WriteReference(name, reference);
		}

		public override void WriteBody(IMemberDefinition member, IWriterContextService writerContextService)
		{
			this.writerContextService = writerContextService;
			this.writerContext = writerContextService.GetWriterContext(member, Language);
			this.currentWritingInfo = new WritingInfo(member);
			UpdateWritingInfo(this.writerContext, this.currentWritingInfo);
			this.writingInfos = new List<WritingInfo>();
			this.writingInfos.Add(this.currentWritingInfo);
			WriteBodyInternal(member);
		}

		protected override void WriteBodyInternal(IMemberDefinition member)
		{
			membersStack.Push(member);
			this.currentNamespace = member.DeclaringType.Namespace;
			base.WriteBodyInternal(member);
			membersStack.Pop();
		}

		private void WriteNamespaceIfNeeded(TypeReference reference, bool forceWriteNamespace = false)
		{
			if ((!forceWriteNamespace) && (!this.Settings.WriteFullyQualifiedNames || CheckForSpecialName(reference)))
			{
				return;
			}

			string @namespace = string.Empty;
			if (forceWriteNamespace)
			{
				@namespace = reference.Namespace;
			}
			else
			{
				if (reference.Namespace != currentNamespace)
				{
					@namespace = reference.Namespace;
				}
			}

			if (ModuleContext.RenamedNamespacesMap.ContainsKey(@namespace))
			{
				@namespace = ModuleContext.RenamedNamespacesMap[@namespace];
			}

			if (@namespace != string.Empty)
			{
				@namespace += '.';
			}

			Write(Utilities.EscapeNamespaceIfNeeded(@namespace, this.Language));
		}

		private bool CheckForSpecialName(TypeReference reference)
		{
			if(reference.HasGenericParameters)
			{
				return false;
			}
			return reference.Name != ToTypeString(reference);
		}

		/* AGPL */
		protected abstract void DoWriteTypeAndName(TypeReference typeReference, string name, object reference, TypeReferenceType typeReferenceType);
		protected abstract void DoWriteTypeAndName(TypeReference typeReference, string name, TypeReferenceType typeReferenceType);
		/* End AGPL */
		protected abstract void DoWriteVariableTypeAndName(VariableDefinition variable);
        protected abstract void DoWriteParameterTypeAndName(TypeReference type, string name, ParameterDefinition reference);
	}
}
