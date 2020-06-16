using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil.AssemblyResolver;

namespace JustDecompile.EngineInfrastructure.AssemblyLocators
{
    public abstract class SilverlightAssembliesLocatorBase : BaseFrameworkAssemblyLocator
    {
        private string silverlightDirectory;

        public override string FrameworkFolder
        {
            get
            {
                return GetSilverlightDirectory();
            }
        }

        protected override IEnumerable<string> GetAssemblies()
        {
            if (GetSilverlightDirectory() == string.Empty)
            {
                return new string[0];
            }
            IEnumerable<string> files = GetFilesExcludingNativeImages(GetSilverlightDirectory());

            return files.ToList();
        }

        private string GetSilverlightDirectory()
        {
            if (silverlightDirectory == null)
            {
                var silverlightFolder = Path.Combine(GetArchitectureSpecificInstallFolder(), "Microsoft Silverlight");

                if (!Directory.Exists(silverlightFolder))
                {
                    silverlightDirectory = string.Empty;

                    return silverlightDirectory;
                }
                string version = SystemInformation.SilverlightVersion;

                if (version != null)
                {
                    silverlightDirectory = Path.Combine(silverlightFolder, version);

                    if (!Directory.Exists(silverlightDirectory))
                    {
                        silverlightDirectory = string.Empty;
                    }
                }
                else
                {
                    silverlightDirectory = string.Empty;
                }
            }
            return silverlightDirectory;
        }

        protected abstract string GetArchitectureSpecificInstallFolder();
    }
}