using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil.AssemblyResolver;

namespace JustDecompile.EngineInfrastructure.AssemblyLocators
{
    public abstract class FrameworkAssemblyLocator3xBase : FrameworkAssemblyLocator2Base
    {
        private string framework35Directory;

        public override string FrameworkFolder
        {
            get { return GetFramework35Directory(); }
        }

        protected override IEnumerable<string> GetAssemblies()
        {
            string framework2Directory = base.FrameworkFolder;

            if (string.IsNullOrEmpty(framework2Directory))
            {
                return new string[0];
            }
            string programFilesFolder = GetFramework35Directory();

            List<string> files = (SystemInformation.CoreAssemblies
                                                   .Select(f => Path.Combine(framework2Directory, f))
                                                   .Union(SystemInformation.SystemAssemblies30.Select(f => Path.Combine(programFilesFolder, f)))
                                                   .Union(SystemInformation.SystemAssemblies35.Select(f => Path.Combine(programFilesFolder, f))))
                                                   .Where(f => File.Exists(f))
                                                   .ToList();
            RemoveNotSupportedAssemblies(framework2Directory, files);

            return files;
        }

        private string GetFramework35Directory()
        {
            if (framework35Directory == null)
            {
                string path = Path.Combine(GetProgramFilesFolder(), @"Reference Assemblies\Microsoft\Framework\v3.5\");

                framework35Directory = HasFolderAnyAssemblies(path) ? GetProgramFilesFolder() : string.Empty;
            }
            return framework35Directory;
        }

        protected abstract string GetProgramFilesFolder();
    }
}