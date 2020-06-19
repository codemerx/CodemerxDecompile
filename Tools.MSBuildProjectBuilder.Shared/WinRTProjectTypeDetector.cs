using System;
using System.IO;
using System.Linq;

using Mono.Cecil;

namespace JustDecompile.Tools.MSBuildProjectBuilder
{
    public class WinRTProjectTypeDetector
    {
        public static WinRTProjectType GetProjectType(AssemblyDefinition assembly)
        {
            string fileExtension = Path.GetExtension(assembly.MainModule.FilePath);
            switch (fileExtension)
            {
                case ".winmd":
                    return DetectComponentType(assembly);
                case ".dll":
                    return IsUniversalWindowsPlatformAssembly(assembly) ? WinRTProjectType.UWPLibrary : WinRTProjectType.Unknown;
                case ".exe":
                    return IsUniversalWindowsPlatformAssembly(assembly) ? WinRTProjectType.UWPApplication : WinRTProjectType.Unknown;
                default:
                    return WinRTProjectType.Unknown;
            }
        }

        public static bool IsWinRTAssemblyGeneratedWithVS2013(AssemblyDefinition assembly)
        {
            return DoesAssemblyContainReferenceToSystemRuntimeWithVersion(assembly, new Version(4, 0, 10, 0));
        }

        public static bool IsUniversalWindowsPlatformAssembly(AssemblyDefinition assembly)
        {
            return DoesAssemblyContainReferenceToSystemRuntimeWithVersion(assembly, new Version(4, 0, 20, 0));
        }

        private static WinRTProjectType DetectComponentType(AssemblyDefinition assembly)
        {
            if (IsUniversalWindowsPlatformAssembly(assembly))
            {
                return WinRTProjectType.UWPComponent;
            }
            else if (!IsWinRTAssemblyGeneratedWithVS2013(assembly))
            {
                return WinRTProjectType.Component;
            }
            else if (assembly.TargetFrameworkAttributeValue.StartsWith(".NETPortable"))
            {
                return WinRTProjectType.ComponentForUniversal;
            }
            else if (assembly.TargetFrameworkAttributeValue.StartsWith(".NETCore"))
            {
                return WinRTProjectType.ComponentForWindows;
            }
            else if (assembly.TargetFrameworkAttributeValue.StartsWith("WindowsPhoneApp"))
            {
                return WinRTProjectType.ComponentForWindowsPhone;
            }
            else
            {
                return WinRTProjectType.Unknown;
            }
        }

        private static bool DoesAssemblyContainReferenceToSystemRuntimeWithVersion(AssemblyDefinition assembly, Version version)
        {
            return assembly.MainModule.AssemblyReferences.Any(r => r.Name == "System.Runtime" && r.Version == version);
        }
    }
}
