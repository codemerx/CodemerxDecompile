using Mono.Cecil;

namespace JustDecompile.EngineInfrastructure.AssemblyLocators
{
    public class FrameworkAssembly4xLocatorFactory
    {
        public static IFrameworkAssemblyLocator Instance(TargetArchitecture targetArchitecture)
        {
            if (targetArchitecture == TargetArchitecture.AMD64Windows || targetArchitecture == TargetArchitecture.IA64Windows)
            {
                return FrameworkAssemblyLocator4x64.Instance();
            }
            else
            {
                return FrameworkAssemblyLocator4x86.Instance();
            }
        }
    }
}