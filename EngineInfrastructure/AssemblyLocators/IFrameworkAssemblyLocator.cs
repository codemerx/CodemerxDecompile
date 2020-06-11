using System;
using System.Collections.Generic;
using System.Linq;

namespace JustDecompile.EngineInfrastructure.AssemblyLocators
{
    public interface IFrameworkAssemblyLocator
    {
        IEnumerable<string> Assemblies { get; }

        string FrameworkFolder { get; }
    }
}
