using SystemInformationHelpers;

namespace JustDecompile.EngineInfrastructure
{
    public static class Framework4VersionResolver
    {
        public static Mono.Cecil.AssemblyResolver.FrameworkVersion GetInstalledFramework4Version()
        {
            FrameworkVersion framework4VersionToParse = SystemInformationHelpers.Framework4VersionResolver.GetInstalledFramework4Version();

            return MapFrameworkVersion(framework4VersionToParse);
        }

        public static Mono.Cecil.AssemblyResolver.FrameworkVersion GetFrameworkVersionByFileVersion(string assemblyFilePath)
        {
            FrameworkVersion framework4VersionToParse = SystemInformationHelpers.Framework4VersionResolver.GetFrameworkVersionByFileVersion(assemblyFilePath);

            return MapFrameworkVersion(framework4VersionToParse);
        }

        private static Mono.Cecil.AssemblyResolver.FrameworkVersion MapFrameworkVersion(FrameworkVersion frameworkVersionToParse)
        {
            Mono.Cecil.AssemblyResolver.FrameworkVersion resultFrameworkVersion = Mono.Cecil.AssemblyResolver.FrameworkVersion.Unknown;

            switch (frameworkVersionToParse)
            {
                case FrameworkVersion.v4_0:
                    resultFrameworkVersion = Mono.Cecil.AssemblyResolver.FrameworkVersion.v4_0;
                    break;
                case FrameworkVersion.v4_5:
                    resultFrameworkVersion = Mono.Cecil.AssemblyResolver.FrameworkVersion.v4_5;
                    break;
                case FrameworkVersion.v4_5_1:
                    resultFrameworkVersion = Mono.Cecil.AssemblyResolver.FrameworkVersion.v4_5_1;
                    break;
                case FrameworkVersion.v4_5_2:
                    resultFrameworkVersion = Mono.Cecil.AssemblyResolver.FrameworkVersion.v4_5_2;
                    break;
                case FrameworkVersion.v4_6:
                    resultFrameworkVersion = Mono.Cecil.AssemblyResolver.FrameworkVersion.v4_6;
                    break;
                case FrameworkVersion.v4_6_1:
                    resultFrameworkVersion = Mono.Cecil.AssemblyResolver.FrameworkVersion.v4_6_1;
                    break;
                case FrameworkVersion.v4_6_2:
                    resultFrameworkVersion = Mono.Cecil.AssemblyResolver.FrameworkVersion.v4_6_2;
                    break;
				case FrameworkVersion.v4_7:
					resultFrameworkVersion = Mono.Cecil.AssemblyResolver.FrameworkVersion.v4_7;
					break;
				case FrameworkVersion.v4_7_1:
					resultFrameworkVersion = Mono.Cecil.AssemblyResolver.FrameworkVersion.v4_7_1;
					break;
                /* AGPL */
                case FrameworkVersion.v4_7_2:
                    resultFrameworkVersion = Mono.Cecil.AssemblyResolver.FrameworkVersion.v4_7_2;
                    break;
                case FrameworkVersion.v4_8:
                    resultFrameworkVersion = Mono.Cecil.AssemblyResolver.FrameworkVersion.v4_8;
                    break;
                /* End AGPL */
				default:
                    break;
            }

            return resultFrameworkVersion;
        }
    }
}