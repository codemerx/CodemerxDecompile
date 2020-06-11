using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil.AssemblyResolver;

namespace JustDecompile.EngineInfrastructure.AssemblyLocators
{
    public class WinRTMetadataAssembliesLocator : BaseFrameworkAssemblyLocator
    {
        private static WinRTMetadataAssembliesLocator instance = null;

        private WinRTMetadataAssembliesLocator()
        {
        	
        }

        public override string FrameworkFolder
        {
            get
            {
                return WinRTMetadataDirectory;
            }
        }

        public static IFrameworkAssemblyLocator Instance()
        {
            return instance != null ? instance : instance = new WinRTMetadataAssembliesLocator();
        }

        protected override IEnumerable<string> GetAssemblies()
        {
            if (WinRTMetadataDirectory == string.Empty)
            {
                return new string[0];
            }

            List<string> files = new List<string>(Directory.GetFiles(WinRTMetadataDirectory, "*.winmd"));

            string windowsWinMDPath = GetWindowsWinMDPath();
            if (windowsWinMDPath != null)
            {
                files.Add(windowsWinMDPath);
            }

            return files;
        }

        private static string winMetadataDirectory;
        private static string WinRTMetadataDirectory
        {
            get
            {
                if (winMetadataDirectory == null)
                {
                    winMetadataDirectory = SystemInformation.WINRT_METADATA;
                    if (!Directory.Exists(winMetadataDirectory))
                    {
                        winMetadataDirectory = string.Empty;
                    }
                }
                return winMetadataDirectory;
            }
        }

        private string GetWindowsWinMDPath()
        {
            string result = null;
            if (Directory.Exists(SystemInformation.WINDOWS_WINMD_LOCATION))
            {
                result = Directory.GetFiles(SystemInformation.WINDOWS_WINMD_LOCATION, "windows.winmd", SearchOption.AllDirectories).FirstOrDefault();
            }
            return result;
        }
    }
}
