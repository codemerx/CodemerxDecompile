using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;
using Telerik.JustDecompiler.External.Interfaces;

namespace JustDecompile.EngineInfrastructure
{
    public interface IAssemblyInfoService
    {
        AssemblyInfo GetAssemblyInfo(AssemblyDefinition assembly, IFrameworkResolver frameworkResolver);
    }
}