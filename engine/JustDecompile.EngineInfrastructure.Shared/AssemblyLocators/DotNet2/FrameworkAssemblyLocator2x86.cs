using System;
using System.Linq;
using Mono.Cecil;

namespace JustDecompile.EngineInfrastructure.AssemblyLocators
{
    public class FrameworkAssemblyLocator2x86 : FrameworkAssemblyLocator2Base
    {
        private static FrameworkAssemblyLocator2x86 instance;

        private FrameworkAssemblyLocator2x86() { }

        public static IFrameworkAssemblyLocator Instance()
        {
            if (instance == null)
            {
                instance = new FrameworkAssemblyLocator2x86();
            }
            return instance;
        }

        protected override TargetArchitecture GetTargetArchitecture()
        {
            return TargetArchitecture.I386Windows;
        }
    }
}
