using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
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
				return this.Writer.AssemblyInfoNamespacesUsed;
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
			this.Language = language;
			this.exceptionFormatter = exceptionFormatter;
			this.Settings = settings;
		}

		protected abstract Telerik.JustDecompiler.Languages.AttributeWriter CreateAttributeWriter();

		protected abstract NamespaceImperativeLanguageWriter GetLanguageWriter();

		protected abstract void SetAssemblyContext(AssemblySpecificContext assemblyContext);

		protected abstract void SetModuleContext(ModuleSpecificContext moduleContext);

		public virtual void WriteAssemblyAttributes(AssemblyDefinition assembly, IWriterContextService writerContextService, bool writeUsings = false, ICollection<string> attributesToIgnore = null)
		{
			AssemblySpecificContext assemblyContext = writerContextService.GetAssemblyContext(assembly, this.Language);
			this.SetAssemblyContext(assemblyContext);
			ModuleSpecificContext moduleContext = writerContextService.GetModuleContext(assembly.get_MainModule(), this.Language);
			this.SetModuleContext(moduleContext);
			this.WriteAssemblyAttributesInternal(assembly, assemblyContext.AssemblyNamespaceUsings, moduleContext.ModuleNamespaceUsings, writeUsings, attributesToIgnore);
		}

		public virtual void WriteAssemblyAttributesInternal(AssemblyDefinition assembly, ICollection<string> assemblyNamespaceUsings, ICollection<string> mainModuleNamespaceUsings, bool writeUsings = false, ICollection<string> attributesToIgnore = null)
		{
			if (writeUsings && (assemblyNamespaceUsings.Count<string>() > 0 || mainModuleNamespaceUsings.Count<string>() > 0))
			{
				this.Writer.WriteAssemblyUsings();
				this.Writer.WriteLine();
				this.Writer.WriteLine();
			}
			this.AttributeWriter.WriteAssemblyAttributes(assembly, attributesToIgnore);
		}

		public void WriteAssemblyInfo(AssemblyDefinition assembly, IWriterContextService writerContextService, bool writeUsings = false, ICollection<string> assemblyAttributesToIgnore = null, ICollection<string> moduleAttributesToIgnore = null)
		{
			AssemblySpecificContext assemblyContext = writerContextService.GetAssemblyContext(assembly, this.Language);
			this.SetAssemblyContext(assemblyContext);
			ModuleSpecificContext moduleContext = writerContextService.GetModuleContext(assembly.get_MainModule(), this.Language);
			this.SetModuleContext(moduleContext);
			this.WriteAssemblyInfoInternal(assembly, assemblyContext.AssemblyNamespaceUsings, moduleContext.ModuleNamespaceUsings, writeUsings, assemblyAttributesToIgnore, moduleAttributesToIgnore);
		}

		private void WriteAssemblyInfoInternal(AssemblyDefinition assembly, ICollection<string> assemblyNamespaceUsings, ICollection<string> mainModuleNamespaceUsings, bool writeUsings = false, ICollection<string> assemblyAttributesToIgnore = null, ICollection<string> moduleAttributesToIgnore = null)
		{
			if (writeUsings && (assemblyNamespaceUsings.Count<string>() > 0 || mainModuleNamespaceUsings.Count<string>() > 0))
			{
				this.Writer.WriteAssemblyAndModuleUsings();
				this.Writer.WriteLine();
				this.Writer.WriteLine();
			}
			this.WriteAssemblyAttributesInternal(assembly, assemblyNamespaceUsings, mainModuleNamespaceUsings, false, assemblyAttributesToIgnore);
			this.WriteModuleAttributesInternal(assembly.get_MainModule(), mainModuleNamespaceUsings, false, moduleAttributesToIgnore);
		}

		public virtual void WriteModuleAttributes(ModuleDefinition module, IWriterContextService writerContextService, bool writeUsings = false, ICollection<string> attributesToIgnore = null)
		{
			this.SetAssemblyContext(new AssemblySpecificContext());
			ModuleSpecificContext moduleContext = writerContextService.GetModuleContext(module, this.Language);
			this.SetModuleContext(moduleContext);
			this.WriteModuleAttributesInternal(module, moduleContext.ModuleNamespaceUsings, writeUsings, attributesToIgnore);
		}

		public virtual void WriteModuleAttributesInternal(ModuleDefinition module, ICollection<string> moduleNamespaceUsings, bool writeUsings = false, ICollection<string> attributesToIgnore = null)
		{
			if (writeUsings && moduleNamespaceUsings.Count<string>() > 0)
			{
				this.Writer.WriteModuleUsings();
				this.Writer.WriteLine();
				this.Writer.WriteLine();
			}
			this.AttributeWriter.WriteModuleAttributes(module, attributesToIgnore);
		}
	}
}