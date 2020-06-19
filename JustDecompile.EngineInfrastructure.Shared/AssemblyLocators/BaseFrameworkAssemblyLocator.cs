using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JustDecompile.EngineInfrastructure.AssemblyLocators
{
    public abstract class BaseFrameworkAssemblyLocator : IFrameworkAssemblyLocator
    {
        private IEnumerable<string> assemblies;

        private static readonly string[] searchPatterns = { "System*.dll", "mscorlib.dll" };

        public abstract string FrameworkFolder { get; }

        public IEnumerable<string> Assemblies
        {
            get
            {
                if (assemblies == null)
                {
                    assemblies = this.GetAssemblies();
                }
                return assemblies;
            }
        }

        protected abstract IEnumerable<string> GetAssemblies();

        protected IEnumerable<string> GetFilesExcludingNativeImages(string path)
        {
            return searchPatterns.SelectMany(x => Directory.GetFiles(path, x).Where(s => !s.EndsWith(".ni.dll", StringComparison.OrdinalIgnoreCase)));
        }
    }
}
