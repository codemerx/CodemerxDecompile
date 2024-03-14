using JustDecompile.Tools.MSBuildProjectBuilder;
using Mono.Cecil;
using Telerik.JustDecompiler.Languages;

namespace CodemerxDecompile.Services;

public interface IProjectGenerationService
{
    string GenerateProject(AssemblyDefinition assembly, VisualStudioVersion visualStudioVersion, ILanguage language, string outputPath);
}
