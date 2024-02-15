using Mono.Cecil;

namespace JustDecompile.EngineInfrastructure.AssemblyLocators
{
    public class FrameworkAssemblyLocator2x64 : FrameworkAssemblyLocator2Base
    {
        private static FrameworkAssemblyLocator2x64 instance;

        private FrameworkAssemblyLocator2x64() { }

        public static IFrameworkAssemblyLocator Instance()
        {
            if (instance == null)
            {
                instance = new FrameworkAssemblyLocator2x64();
            }
            return instance;
        }

        protected override TargetArchitecture GetTargetArchitecture()
        {
            return TargetArchitecture.AMD64Windows;
        }
    }
}