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
			this.Language = language;
			this.formatter = formatter;
			this.exceptionFormatter = exceptionFormatter;
			this.Settings = settings;
		}

		public void WriteAssemblyAttributes(AssemblyDefinition assembly, IWriterContextService writerContextService, bool writeUsings = false, ICollection<string> attributesToIgnore = null)
		{
			IntermediateLanguageAttributeWriter intermediateLanguageAttributeWriter = new IntermediateLanguageAttributeWriter(this.Language, this.formatter, this.exceptionFormatter, this.Settings);
			intermediateLanguageAttributeWriter.ExceptionThrown += new EventHandler<Exception>(this.OnExceptionThrown);
			intermediateLanguageAttributeWriter.WriteAssemblyAttributes(assembly, attributesToIgnore);
			intermediateLanguageAttributeWriter.ExceptionThrown -= new EventHandler<Exception>(this.OnExceptionThrown);
		}

		public void WriteAssemblyInfo(AssemblyDefinition assembly, IWriterContextService writerContextService, bool writeUsings = false, ICollection<string> assemblyAttributesToIgnore = null, ICollection<string> moduleAttributesToIgnore = null)
		{
			this.WriteAssemblyAttributes(assembly, writerContextService, writeUsings, assemblyAttributesToIgnore);
			this.WriteModuleAttributes(assembly.get_MainModule(), writerContextService, writeUsings, moduleAttributesToIgnore);
		}

		public void WriteModuleAttributes(ModuleDefinition module, IWriterContextService writerContextService, bool writeUsings = false, ICollection<string> attributesToIgnore = null)
		{
		}
	}
}