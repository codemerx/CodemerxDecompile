using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JustDecompile.EngineInfrastructure.AssemblyLocators
{
    public abstract class FrameworkAssemblyLocator4xBase : NumberedFrameworkAssemblyLocator
    {
        private string frameworkFolder = string.Empty;

        protected static readonly string[] Wpf4 = { "WindowsBase.dll", "PresentationCore.dll", "PresentationFramework.dll" };

        public override string FrameworkFolder
        {
            get { return GetFrameworkFolder(); }
        }

        protected override IEnumerable<string> GetAssemblies()
        {
            string frameworkDirectory = GetFrameworkFolder();

            if (frameworkDirectory == string.Empty)
            {
                return new string[0];
            }
            List<string> files = GetFilesExcludingNativeImages(frameworkDirectory).ToList();

            string wpfPath = Path.Combine(frameworkDirectory, "WPF");
            if (Directory.Exists(wpfPath))
            {
                files.AddRange(Wpf4.Select(f => Directory.GetFiles(wpfPath, f).FirstOrDefault()));
            }
            RemoveNotSupportedAssemblies(frameworkDirectory, files);

            return files;
        }

        protected string GetFrameworkFolder()
        {
            if (frameworkFolder == string.Empty)
            {
                frameworkFolder = GetSpecificArchitectureFrameworkDirectory(4);
            }
            return frameworkFolder;
        }
    }
}