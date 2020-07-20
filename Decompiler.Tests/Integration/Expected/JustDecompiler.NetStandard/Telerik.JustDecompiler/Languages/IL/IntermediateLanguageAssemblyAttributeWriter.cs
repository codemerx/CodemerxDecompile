using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Languages.IL
{
	public class IntermediateLanguageAssemblyAttributeWriter : ExceptionThrownNotifier, IAssemblyAttributeWriter, IExceptionThrownNotifier
	{
		protected IFormatter formatter;

		protected IExceptionFormatter exceptionFormatter;

		private readonly bool shouldGenerateBlocks;

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

		public IntermediateLanguageAssemblyAttributeWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
		{
			base();
			this.set_Language(language);
			this.formatter = formatter;
			this.exceptionFormatter = exceptionFormatter;
			this.set_Settings(settings);
			return;
		}

		public void WriteAssemblyAttributes(AssemblyDefinition assembly, IWriterContextService writerContextService, bool writeUsings = false, ICollection<string> attributesToIgnore = null)
		{
			stackVariable8 = new IntermediateLanguageAttributeWriter(this.get_Language(), this.formatter, this.exceptionFormatter, this.get_Settings());
			stackVariable8.add_ExceptionThrown(new EventHandler<Exception>(this.OnExceptionThrown));
			stackVariable8.WriteAssemblyAttributes(assembly, attributesToIgnore);
			stackVariable8.remove_ExceptionThrown(new EventHandler<Exception>(this.OnExceptionThrown));
			return;
		}

		public void WriteAssemblyInfo(AssemblyDefinition assembly, IWriterContextService writerContextService, bool writeUsings = false, ICollection<string> assemblyAttributesToIgnore = null, ICollection<string> moduleAttributesToIgnore = null)
		{
			this.WriteAssemblyAttributes(assembly, writerContextService, writeUsings, assemblyAttributesToIgnore);
			this.WriteModuleAttributes(assembly.get_MainModule(), writerContextService, writeUsings, moduleAttributesToIgnore);
			return;
		}

		public void WriteModuleAttributes(ModuleDefinition module, IWriterContextService writerContextService, bool writeUsings = false, ICollection<string> attributesToIgnore = null)
		{
			return;
		}
	}
}