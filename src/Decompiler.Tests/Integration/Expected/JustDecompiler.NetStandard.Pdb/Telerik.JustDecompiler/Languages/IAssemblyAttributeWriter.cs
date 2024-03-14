using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.Languages
{
	public interface IAssemblyAttributeWriter : IExceptionThrownNotifier
	{
		void WriteAssemblyAttributes(AssemblyDefinition assembly, IWriterContextService writerContextService, bool writeUsings = false, ICollection<string> attributesToIgnore = null);

		void WriteAssemblyInfo(AssemblyDefinition assembly, IWriterContextService writerContextService, bool writeUsings = false, ICollection<string> assemblyAttributesToIgnore = null, ICollection<string> moduleAttributesToIgnore = null);

		void WriteModuleAttributes(ModuleDefinition module, IWriterContextService writerContextService, bool writeUsings = false, ICollection<string> attributesToIgnore = null);
	}
}