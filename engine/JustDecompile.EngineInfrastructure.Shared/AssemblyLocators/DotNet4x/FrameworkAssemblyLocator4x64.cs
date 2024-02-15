using System;
using System.Linq;
using Mono.Cecil;

namespace JustDecompile.EngineInfrastructure.AssemblyLocators
{
    public class FrameworkAssemblyLocator4x64 : FrameworkAssemblyLocator4xBase
    {
        private static FrameworkAssemblyLocator4x64 instance;

        private FrameworkAssemblyLocator4x64() { }

        public static IFrameworkAssemblyLocator Instance()
        {
            return instance != null ? instance : instance = new FrameworkAssemblyLocator4x64();
        }

        protected override TargetArchitecture GetTargetArchitecture()
        {
            return TargetArchitecture.AMD64Windows;
        }
    }
}
