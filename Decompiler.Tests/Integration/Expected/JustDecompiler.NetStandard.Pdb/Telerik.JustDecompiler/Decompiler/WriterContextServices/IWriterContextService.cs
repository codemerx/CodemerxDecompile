using Mono.Cecil;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler.WriterContextServices
{
	public interface IWriterContextService : IExceptionThrownNotifier
	{
		ICollection<MethodDefinition> ExceptionsWhileDecompiling
		{
			get;
		}

		AssemblySpecificContext GetAssemblyContext(AssemblyDefinition assembly, ILanguage language);

		ModuleSpecificContext GetModuleContext(ModuleDefinition module, ILanguage language);

		WriterContext GetWriterContext(IMemberDefinition member, ILanguage language);
	}
}