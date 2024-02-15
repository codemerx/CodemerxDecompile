using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;

namespace JustDecompile.EngineInfrastructure.AssemblyLocators
{
    public abstract class NumberedFrameworkAssemblyLocator : BaseFrameworkAssemblyLocator
    {
        protected abstract TargetArchitecture GetTargetArchitecture();

        protected string GetSpecificArchitectureFrameworkDirectory(double version)
        {
            string searchPattern = string.Format("v{0}*", version.ToString());

            TargetArchitecture targetArchitecture = GetTargetArchitecture();

            if (targetArchitecture == TargetArchitecture.AMD64 || targetArchitecture == TargetArchitecture.IA64)
            {
                if (Directory.Exists(SystemInformation.CLR_Default_64))
                {
                    string dirName = Directory.GetDirectories(SystemInformation.CLR_Default_64, searchPattern).FirstOrDefault();

                    if (HasFolderAnyAssemblies(dirName))
                    {
                        return dirName;
                    }
                }
            }
            else if (Directory.Exists(SystemInformation.CLR_Default_32))
            {
                string dirName = Directory.GetDirectories(SystemInformation.CLR_Default_32, searchPattern).FirstOrDefault();

                if (HasFolderAnyAssemblies(dirName))
                {
                    return dirName;
                }
            }
            return string.Empty;
        }

        protected bool HasFolderAnyAssemblies(string dirName)
        {
            if (string.IsNullOrEmpty(dirName))
            {
                return false;
            }
            else if (!Directory.Exists(dirName))
            {
                return false;
            }
            return Directory.GetFiles(dirName, "*.dll").Any();
        }

        protected static string GetFrameworkDirectory(double version)
        {
            string dir = string.Format("v{0}*", version.ToString());

            return Directory.GetDirectories(SystemInformation.CLR_Default_32, dir).FirstOrDefault() ?? String.Empty;
        }

        protected void RemoveNotSupportedAssemblies(string frameworkDirectory, List<string> files)
        {
            files.Remove(string.Format("{0}\\{1}", frameworkDirectory, "System.EnterpriseServices.Thunk.dll"));
            files.Remove(string.Format("{0}\\{1}", frameworkDirectory, "System.EnterpriseServices.Wrapper.dll"));
        }
    }
}
