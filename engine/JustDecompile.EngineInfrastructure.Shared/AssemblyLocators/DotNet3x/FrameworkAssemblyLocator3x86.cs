using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;

namespace JustDecompile.EngineInfrastructure.AssemblyLocators
{
    public class FrameworkAssemblyLocator3x86 : FrameworkAssemblyLocator3xBase
    {
        private static FrameworkAssemblyLocator3x86 instance;

        private FrameworkAssemblyLocator3x86() { }

        public static IFrameworkAssemblyLocator Instance()
        {
            if (instance == null)
            {
                instance = new FrameworkAssemblyLocator3x86();
            }
            return instance;
        }

        protected override TargetArchitecture GetTargetArchitecture()
        {
            return TargetArchitecture.I386Windows;
        }

        protected override string GetProgramFilesFolder()
        {
            return SystemInformation.ProgramFilesX86;
        }
    }
}