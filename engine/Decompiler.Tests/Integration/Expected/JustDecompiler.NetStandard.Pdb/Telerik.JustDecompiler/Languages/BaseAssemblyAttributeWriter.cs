using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.Languages
{
	public abstract class BaseAssemblyAttributeWriter : ExceptionThrownNotifier, IAssemblyAttributeWriter, IExceptionThrownNotifier
	{
		protected IExceptionFormatter exceptionFormatter;

		protected Telerik.JustDecompiler.Languages.AttributeWriter attributeWriter;

		public IEnumerable<string> AssemblyInfoNamespacesUsed
		{
			get
			{
				return this.get_Writer().get_AssemblyInfoNamespacesUsed();
			}
		}

		protected Telerik.JustDecompiler.Languages.AttributeWriter AttributeWriter
		{
			get
			{
				if (this.attributeWriter == null)
				{
					this.attributeWriter = this.CreateAttributeWriter();
				}
				return this.attributeWriter;
			}
		}

		public ILanguage Language
		{
			get;
			private set;
		}

		protected IWriterSettings Settings
		{
			get;
			private set;
		}

		protected NamespaceImperativeLanguageWriter Writer
		{
			get
			{
				return this.GetLanguageWriter();
			}
		}

		public BaseAssemblyAttributeWriter(ILanguage language, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
		{
			base();
			this.set_Language(language);
			this.exceptionFormatter = exceptionFormatter;
			this.set_Settings(settings);
			return;
		}

		protected abstract Telerik.JustDecompiler.Languages.AttributeWriter CreateAttributeWriter();

		protected abstract NamespaceImperativeLanguageWriter GetLanguageWriter();

		protected abstract void SetAssemblyContext(AssemblySpecificContext assemblyContext);

		protected abstract void SetModuleContext(ModuleSpecificContext moduleContext);

		public virtual void WriteAssemblyAttributes(AssemblyDefinition assembly, IWriterContextService writerContextService, bool writeUsings = false, ICollection<string> attributesToIgnore = null)
		{
			V_0 = writerContextService.GetAssemblyContext(assembly, this.get_Language());
			this.SetAssemblyContext(V_0);
			V_1 = writerContextService.GetModuleContext(assembly.get_MainModule(), this.get_Language());
			this.SetModuleContext(V_1);
			this.WriteAssemblyAttributesInternal(assembly, V_0.get_AssemblyNamespaceUsings(), V_1.get_ModuleNamespaceUsings(), writeUsings, attributesToIgnore);
			return;
		}

		public virtual void WriteAssemblyAttributesInternal(AssemblyDefinition assembly, ICollection<string> assemblyNamespaceUsings, ICollection<string> mainModuleNamespaceUsings, bool writeUsings = false, ICollection<string> attributesToIgnore = null)
		{
			if (writeUsings && assemblyNamespaceUsings.Count<string>() > 0 || mainModuleNamespaceUsings.Count<string>() > 0)
			{
				this.get_Writer().WriteAssemblyUsings();
				this.get_Writer().WriteLine();
				this.get_Writer().WriteLine();
			}
			this.get_AttributeWriter().WriteAssemblyAttributes(assembly, attributesToIgnore);
			return;
		}

		public void WriteAssemblyInfo(AssemblyDefinition assembly, IWriterContextService writerContextService, bool writeUsings = false, ICollection<string> assemblyAttributesToIgnore = null, ICollection<string> moduleAttributesToIgnore = null)
		{
			V_0 = writerContextService.GetAssemblyContext(assembly, this.get_Language());
			this.SetAssemblyContext(V_0);
			V_1 = writerContextService.GetModuleContext(assembly.get_MainModule(), this.get_Language());
			this.SetModuleContext(V_1);
			this.WriteAssemblyInfoInternal(assembly, V_0.get_AssemblyNamespaceUsings(), V_1.get_ModuleNamespaceUsings(), writeUsings, assemblyAttributesToIgnore, moduleAttributesToIgnore);
			return;
		}

		private void WriteAssemblyInfoInternal(AssemblyDefinition assembly, ICollection<string> assemblyNamespaceUsings, ICollection<string> mainModuleNamespaceUsings, bool writeUsings = false, ICollection<string> assemblyAttributesToIgnore = null, ICollection<string> moduleAttributesToIgnore = null)
		{
			if (writeUsings && assemblyNamespaceUsings.Count<string>() > 0 || mainModuleNamespaceUsings.Count<string>() > 0)
			{
				this.get_Writer().WriteAssemblyAndModuleUsings();
				this.get_Writer().WriteLine();
				this.get_Writer().WriteLine();
			}
			this.WriteAssemblyAttributesInternal(assembly, assemblyNamespaceUsings, mainModuleNamespaceUsings, false, assemblyAttributesToIgnore);
			this.WriteModuleAttributesInternal(assembly.get_MainModule(), mainModuleNamespaceUsings, false, moduleAttributesToIgnore);
			return;
		}

		public virtual void WriteModuleAttributes(ModuleDefinition module, IWriterContextService writerContextService, bool writeUsings = false, ICollection<string> attributesToIgnore = null)
		{
			this.SetAssemblyContext(new AssemblySpecificContext());
			V_1 = writerContextService.GetModuleContext(module, this.get_Language());
			this.SetModuleContext(V_1);
			this.WriteModuleAttributesInternal(module, V_1.get_ModuleNamespaceUsings(), writeUsings, attributesToIgnore);
			return;
		}

		public virtual void WriteModuleAttributesInternal(ModuleDefinition module, ICollection<string> moduleNamespaceUsings, bool writeUsings = false, ICollection<string> attributesToIgnore = null)
		{
			if (writeUsings && moduleNamespaceUsings.Count<string>() > 0)
			{
				this.get_Writer().WriteModuleUsings();
				this.get_Writer().WriteLine();
				this.get_Writer().WriteLine();
			}
			this.get_AttributeWriter().WriteModuleAttributes(module, attributesToIgnore);
			return;
		}
	}
}