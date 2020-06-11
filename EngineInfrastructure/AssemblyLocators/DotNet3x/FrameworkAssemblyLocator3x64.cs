using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;

namespace JustDecompile.EngineInfrastructure.AssemblyLocators
{
    public class FrameworkAssemblyLocator3x64 : FrameworkAssemblyLocator3xBase
    {
        private static FrameworkAssemblyLocator3x64 instance;

        private FrameworkAssemblyLocator3x64() { }

        public static IFrameworkAssemblyLocator Instance()
        {
            if (instance == null)
            {
                instance = new FrameworkAssemblyLocator3x64();
            }
            return instance;
        }

        protected override TargetArchitecture GetTargetArchitecture()
        {
            return TargetArchitecture.AMD64;
        }

        protected override string GetProgramFilesFolder()
        {
            return SystemInformation.ProgramW6432;
        }
    }
}
