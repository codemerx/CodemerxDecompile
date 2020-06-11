using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.External;

namespace Telerik.JustDecompiler.Languages.IL
{
	public class IntermediateLanguageAssemblyAttributeWriter : ExceptionThrownNotifier, IAssemblyAttributeWriter
	{
		protected IFormatter formatter;
		protected IWriterSettings Settings { get; private set; }
		protected IExceptionFormatter exceptionFormatter;
		private readonly bool shouldGenerateBlocks;

		public ILanguage Language { get; private set; }

		public IntermediateLanguageAssemblyAttributeWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
        {
            this.Language = language;
            this.formatter = formatter;
			this.exceptionFormatter = exceptionFormatter;
			this.Settings = settings;
		}

        public void WriteAssemblyAttributes(AssemblyDefinition assembly, IWriterContextService writerContextService, bool writeUsings = false, ICollection<string> attributesToIgnore = null)
        {
			IntermediateLanguageAttributeWriter attributeWriter = new IntermediateLanguageAttributeWriter(this.Language, this.formatter, this.exceptionFormatter, this.Settings);
            attributeWriter.ExceptionThrown += OnExceptionThrown;
			attributeWriter.WriteAssemblyAttributes(assembly, attributesToIgnore);
            attributeWriter.ExceptionThrown -= OnExceptionThrown;
		}

		public void WriteModuleAttributes(ModuleDefinition module, IWriterContextService writerContextService, bool writeUsings = false, ICollection<string> attributesToIgnore = null)
		{
		}

		public void WriteAssemblyInfo(AssemblyDefinition assembly, IWriterContextService writerContextService, bool writeUsings = false, 
			ICollection<string> assemblyAttributesToIgnore = null, ICollection<string> moduleAttributesToIgnore = null)
		{
			WriteAssemblyAttributes(assembly, writerContextService, writeUsings, assemblyAttributesToIgnore);
			WriteModuleAttributes(assembly.MainModule, writerContextService, writeUsings, moduleAttributesToIgnore);
		}
	}
}
