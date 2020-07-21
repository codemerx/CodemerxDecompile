using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
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
				return this.get_AssemblyContext().get_AssemblyNamespaceUsings().Union<string>(this.get_ModuleContext().get_ModuleNamespaceUsings());
			}
		}

		public NamespaceImperativeLanguageWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
		{
			base(language, formatter, exceptionFormatter, settings);
			this.writeNamespacesandUsings = false;
			return;
		}

		private bool CheckForSpecialName(TypeReference reference)
		{
			if (reference.get_HasGenericParameters())
			{
				return false;
			}
			return String.op_Inequality(reference.get_Name(), this.ToTypeString(reference));
		}

		protected abstract void DoWriteParameterTypeAndName(TypeReference type, string name, ParameterDefinition reference);

		protected abstract void DoWriteTypeAndName(TypeReference typeReference, string name, object reference);

		protected abstract void DoWriteTypeAndName(TypeReference typeReference, string name);

		protected abstract void DoWriteVariableTypeAndName(VariableDefinition variable);

		private string GetCurrentNamespace(TypeDefinition type)
		{
			return Utilities.GetOuterMostDeclaringType(type).get_Namespace();
		}

		public void WriteAssemblyAndModuleUsings()
		{
			this.WriteUsings(Utilities.GetAssemblyAndModuleNamespaceUsings(this.get_AssemblyContext(), this.get_ModuleContext()));
			return;
		}

		public void WriteAssemblyUsings()
		{
			this.WriteUsings(this.get_AssemblyContext().get_AssemblyNamespaceUsings());
			return;
		}

		public override void WriteBody(IMemberDefinition member, IWriterContextService writerContextService)
		{
			this.writerContextService = writerContextService;
			this.writerContext = writerContextService.GetWriterContext(member, this.get_Language());
			this.currentWritingInfo = new WritingInfo(member);
			this.UpdateWritingInfo(this.writerContext, this.currentWritingInfo);
			this.writingInfos = new List<WritingInfo>();
			this.writingInfos.Add(this.currentWritingInfo);
			this.WriteBodyInternal(member);
			return;
		}

		protected override void WriteBodyInternal(IMemberDefinition member)
		{
			this.membersStack.Push(member);
			this.currentNamespace = member.get_DeclaringType().get_Namespace();
			this.WriteBodyInternal(member);
			dummyVar0 = this.membersStack.Pop();
			return;
		}

		internal void WriteEnumValueField(FieldDefinition field)
		{
			this.WriteReference(this.GetFieldName(field), field);
			return;
		}

		protected virtual void WriteMethodReference(string name, MethodReference reference)
		{
			this.WriteReference(name, reference);
			return;
		}

		public void WriteModuleUsings()
		{
			this.WriteUsings(this.get_ModuleContext().get_ModuleNamespaceUsings());
			return;
		}

		protected override void WriteNamespace(object reference, bool forceWriteNamespace = false)
		{
			if (reference as TypeReference == null)
			{
				this.WriteNamespace(reference, forceWriteNamespace);
				return;
			}
			this.WriteNamespaceIfNeeded(reference as TypeReference, forceWriteNamespace);
			return;
		}

		private void WriteNamespaceDeclaration(string namespace)
		{
			if (this.get_ModuleContext().get_RenamedNamespacesMap().ContainsKey(namespace))
			{
				this.WriteComment(namespace);
				this.WriteLine();
				namespace = this.get_ModuleContext().get_RenamedNamespacesMap().get_Item(namespace);
			}
			this.WriteKeyword(this.get_KeyWordWriter().get_Namespace());
			this.WriteSpace();
			this.Write(Utilities.EscapeNamespaceIfNeeded(namespace, this.get_Language()));
			this.formatter.WriteNamespaceStartBlock();
			this.WriteBeginBlock(false);
			this.WriteLine();
			return;
		}

		private void WriteNamespaceIfNeeded(TypeReference reference, bool forceWriteNamespace = false)
		{
			if (!forceWriteNamespace && !this.get_Settings().get_WriteFullyQualifiedNames() || this.CheckForSpecialName(reference))
			{
				return;
			}
			V_0 = String.Empty;
			if (!forceWriteNamespace)
			{
				if (String.op_Inequality(reference.get_Namespace(), this.currentNamespace))
				{
					V_0 = reference.get_Namespace();
				}
			}
			else
			{
				V_0 = reference.get_Namespace();
			}
			if (this.get_ModuleContext().get_RenamedNamespacesMap().ContainsKey(V_0))
			{
				V_0 = this.get_ModuleContext().get_RenamedNamespacesMap().get_Item(V_0);
			}
			if (String.op_Inequality(V_0, String.Empty))
			{
				V_0 = String.Concat(V_0, ".");
			}
			this.Write(Utilities.EscapeNamespaceIfNeeded(V_0, this.get_Language()));
			return;
		}

		public override void WriteNamespaceIfTypeInCollision(TypeReference reference)
		{
			if (this.get_Settings().get_WriteFullyQualifiedNames())
			{
				return;
			}
			this.WriteNamespaceIfTypeInCollision(reference);
			return;
		}

		protected sealed override void WriteParameterTypeAndName(TypeReference type, string name, ParameterDefinition reference)
		{
			this.DoWriteParameterTypeAndName(type, name, reference);
			return;
		}

		public List<WritingInfo> WritePartialTypeAndNamespaces(TypeDefinition type, IWriterContextService writerContextService, Dictionary<string, ICollection<string>> fieldsToSkip = null)
		{
			this.writerContextService = writerContextService;
			this.writerContext = writerContextService.GetWriterContext(type, this.get_Language());
			this.set_CurrentType(type);
			this.currentWritingInfo = new WritingInfo(type);
			this.UpdateWritingInfo(this.writerContext, this.currentWritingInfo);
			this.writingInfos = new List<WritingInfo>();
			this.writingInfos.Add(this.currentWritingInfo);
			this.WritePartialTypeAndNamespacesInternal(type, fieldsToSkip);
			return this.writingInfos;
		}

		public void WritePartialTypeAndNamespacesInternal(TypeDefinition type, Dictionary<string, ICollection<string>> fieldsToSkip = null)
		{
			this.writeNamespacesandUsings = true;
			this.currentNamespace = type.get_Namespace();
			V_0 = null;
			if (fieldsToSkip.ContainsKey(type.get_FullName()))
			{
				V_0 = fieldsToSkip.get_Item(type.get_FullName());
			}
			if (this.get_TypeContext().get_UsedNamespaces().Count<string>() > 0)
			{
				this.WriteUsings(this.get_TypeContext().get_UsedNamespaces());
				this.WriteLine();
				this.WriteLine();
			}
			this.WriteTypeNamespaceStart(type);
			this.WritePartialType(type, V_0);
			this.WriteTypeNamespaceEnd(type);
			return;
		}

		internal override void WriteReference(string name, object reference)
		{
			if (reference as TypeReference != null)
			{
				this.WriteNamespaceIfNeeded(reference as TypeReference, false);
				this.WriteTypeReference(name, reference as TypeReference);
				return;
			}
			if (reference as MethodReference == null)
			{
				this.WriteReference(name, reference);
				return;
			}
			this.WriteMethodReference(name, reference as MethodReference);
			return;
		}

		public void WriteSecurityDeclarationNamespaceIfNeeded()
		{
			if (this.get_Settings().get_WriteFullyQualifiedNames())
			{
				this.Write("System.Security.Permissions");
				this.WriteToken(".");
			}
			return;
		}

		public List<WritingInfo> WriteType(TypeDefinition type, IWriterContextService writerContextService)
		{
			this.writeNamespacesandUsings = false;
			this.currentNamespace = this.GetCurrentNamespace(type);
			return this.Write(type, writerContextService);
		}

		protected sealed override void WriteTypeAndName(TypeReference typeReference, string name, object reference)
		{
			this.DoWriteTypeAndName(typeReference, name, reference);
			return;
		}

		protected sealed override void WriteTypeAndName(TypeReference typeReference, string name)
		{
			this.DoWriteTypeAndName(typeReference, name);
			return;
		}

		public List<WritingInfo> WriteTypeAndNamespaces(TypeDefinition type, IWriterContextService writerContextService)
		{
			this.writerContextService = writerContextService;
			this.writerContext = writerContextService.GetWriterContext(type, this.get_Language());
			this.set_CurrentType(type);
			this.currentNamespace = this.GetCurrentNamespace(type);
			this.currentWritingInfo = new WritingInfo(type);
			this.UpdateWritingInfo(this.writerContext, this.currentWritingInfo);
			this.writingInfos = new List<WritingInfo>();
			this.writingInfos.Add(this.currentWritingInfo);
			this.WriteTypeAndNamespacesInternal(type);
			return this.writingInfos;
		}

		protected void WriteTypeAndNamespacesInternal(TypeDefinition type)
		{
			this.writeNamespacesandUsings = true;
			this.currentNamespace = this.GetCurrentNamespace(type);
			if (this.get_TypeContext().get_UsedNamespaces().Count<string>() > 0)
			{
				this.WriteUsings(this.get_TypeContext().get_UsedNamespaces());
				this.WriteLine();
				this.WriteLine();
			}
			this.WriteTypeNamespaceStart(type);
			this.WriteInternal(type);
			this.WriteTypeNamespaceEnd(type);
			return;
		}

		protected override void WriteTypeInANewWriterIfNeeded(TypeDefinition type)
		{
			if ((object)this.get_CurrentType() == (object)type)
			{
				this.WriteType(type);
				return;
			}
			stackVariable13 = this.get_Language().GetWriter(this.formatter, this.exceptionFormatter, this.get_Settings());
			stackVariable13.add_ExceptionThrown(new EventHandler<Exception>(this.OnExceptionThrown));
			V_0 = (stackVariable13 as NamespaceImperativeLanguageWriter).WriteType(type, this.writerContextService);
			stackVariable13.remove_ExceptionThrown(new EventHandler<Exception>(this.OnExceptionThrown));
			this.writingInfos.AddRange(V_0);
			return;
		}

		protected sealed override void WriteTypeNamespaceEnd(TypeDefinition type)
		{
			if (this.writeNamespacesandUsings && String.op_Inequality(type.get_Namespace(), String.Empty))
			{
				this.WriteLine();
				this.Outdent();
				this.WriteEndBlock(this.get_KeyWordWriter().get_Namespace());
				this.formatter.WriteNamespaceEndBlock();
			}
			return;
		}

		protected sealed override void WriteTypeNamespaceStart(TypeDefinition type)
		{
			this.currentNamespace = type.GetNamespace();
			if (this.writeNamespacesandUsings && String.op_Inequality(type.get_Namespace(), String.Empty))
			{
				this.WriteNamespaceDeclaration(this.currentNamespace);
				this.Indent();
			}
			return;
		}

		protected virtual void WriteTypeReference(string name, TypeReference reference)
		{
			this.WriteReference(name, reference);
			return;
		}

		public void WriteUsings(ICollection<string> usedNamespaces)
		{
			stackVariable1 = usedNamespaces.ToArray<string>();
			Array.Sort<string>(stackVariable1);
			V_0 = true;
			V_1 = stackVariable1;
			V_2 = 0;
			while (V_2 < (int)V_1.Length)
			{
				V_3 = V_1[V_2];
				if (!V_0)
				{
					this.WriteLine();
				}
				this.WriteKeyword(this.get_KeyWordWriter().get_NamespaceUsing());
				this.WriteSpace();
				if (V_0)
				{
					this.formatter.WriteStartUsagesBlock();
					V_0 = false;
				}
				V_4 = V_3;
				if (this.get_ModuleContext().get_RenamedNamespacesMap().ContainsKey(V_3))
				{
					V_4 = this.get_ModuleContext().get_RenamedNamespacesMap().get_Item(V_3);
				}
				this.Write(Utilities.EscapeNamespaceIfNeeded(V_4, this.get_Language()));
				this.WriteEndOfStatement();
				V_2 = V_2 + 1;
			}
			this.formatter.WriteEndUsagesBlock();
			return;
		}

		protected sealed override void WriteVariableTypeAndName(VariableDefinition variable)
		{
			this.DoWriteVariableTypeAndName(variable);
			return;
		}
	}
}