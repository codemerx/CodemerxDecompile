using System;
using System.Linq;
using Mono.Cecil.AssemblyResolver;

namespace JustDecompile.EngineInfrastructure.AssemblyLocators
{
    public class SilverlightX86AssembliesLocator : SilverlightAssembliesLocatorBase
    {
        private static SilverlightX86AssembliesLocator instance = null;

        private SilverlightX86AssembliesLocator() { }

        public static IFrameworkAssemblyLocator Instance()
        {
            return instance != null ? instance : instance = new SilverlightX86AssembliesLocator();
        }

        protected override string GetArchitectureSpecificInstallFolder()
        {
            return SystemInformation.ProgramFilesX86;
        }
    }
}
