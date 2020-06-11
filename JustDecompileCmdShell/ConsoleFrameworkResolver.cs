using System;
using System.Linq;
using Telerik.JustDecompiler.External.Interfaces;
using Mono.Cecil.AssemblyResolver;

namespace JustDecompileCmd
{
    class ConsoleFrameworkResolver : IFrameworkResolver
    {
        private readonly FrameworkVersion frameworkVersion;

        public ConsoleFrameworkResolver(FrameworkVersion frameworkVersion)
        {
            this.frameworkVersion = frameworkVersion;
        }

        public FrameworkVersion GetDefaultFallbackFramework4Version()
        {
            return this.frameworkVersion;
        }
    }
}
