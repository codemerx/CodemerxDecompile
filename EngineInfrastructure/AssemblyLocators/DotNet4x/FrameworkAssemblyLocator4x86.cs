using Mono.Cecil;

namespace JustDecompile.EngineInfrastructure.AssemblyLocators
{
    public class FrameworkAssemblyLocator4x86 : FrameworkAssemblyLocator4xBase
    {
        private static FrameworkAssemblyLocator4x86 instance;

        private FrameworkAssemblyLocator4x86()
        {
        }

        public static IFrameworkAssemblyLocator Instance()
        {
            return instance != null ? instance : instance = new FrameworkAssemblyLocator4x86();
        }

        protected override TargetArchitecture GetTargetArchitecture()
        {
            return TargetArchitecture.I386;
        }
    }
}