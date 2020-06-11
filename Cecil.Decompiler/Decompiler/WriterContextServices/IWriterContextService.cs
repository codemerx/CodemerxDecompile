using System;
using System.Linq;
using Mono.Cecil;
using Telerik.JustDecompiler.Languages;
using System.Collections.Generic;
using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.Decompiler.WriterContextServices
{
	public interface IWriterContextService : IExceptionThrownNotifier
    {
		WriterContext GetWriterContext(IMemberDefinition member, ILanguage language);
		ModuleSpecificContext GetModuleContext(ModuleDefinition module, ILanguage language);
		AssemblySpecificContext GetAssemblyContext(AssemblyDefinition assembly, ILanguage language);

		ICollection<MethodDefinition> ExceptionsWhileDecompiling { get; }
	}
}
