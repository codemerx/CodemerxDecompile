using System.Collections.Generic;
using System.Linq;

namespace JustDecompile.EngineInfrastructure.AssemblyLocators
{
    public abstract class FrameworkAssemblyLocator2Base : NumberedFrameworkAssemblyLocator
    {
        private string framework20Directory;

        public override string FrameworkFolder
        {
            get { return GetFramework20Directory(); }
        }

        protected override IEnumerable<string> GetAssemblies()
        {
            string frameworkDirectory = GetFramework20Directory();

            if (frameworkDirectory == string.Empty)
            {
                return new List<string>();
            }
            List<string> files = GetFilesExcludingNativeImages(frameworkDirectory).ToList();

            RemoveNotSupportedAssemblies(frameworkDirectory, files);

            return files;
        }

        private string GetFramework20Directory()
        {
            if (framework20Directory == null)
            {
                framework20Directory = GetSpecificArchitectureFrameworkDirectory(2);
            }
            return framework20Directory;
        }
    }
}