using Mono.Cecil.AssemblyResolver;
using System.IO;
using System;

namespace JustDecompile.EngineInfrastructure.AssemblyLocators
{
    public class SilverlightX64AssembliesLocator : SilverlightAssembliesLocatorBase
    {
        private static SilverlightX64AssembliesLocator instance;

        private SilverlightX64AssembliesLocator()
        {
        }

        public static IFrameworkAssemblyLocator Instance()
        {
            return instance != null ? instance : instance = new SilverlightX64AssembliesLocator();
        }

        protected override string GetArchitectureSpecificInstallFolder()
        {
            return SystemInformation.ProgramW6432;
        }
    }
}