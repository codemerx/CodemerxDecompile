using System;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using Telerik.JustDecompiler.External.Interfaces;
using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;
using JustDecompile.EngineInfrastructure.AssemblyLocators;

namespace JustDecompile.EngineInfrastructure
{
    public class NoCacheAssemblyInfoService : IAssemblyInfoService
    {
        private static NoCacheAssemblyInfoService instance = null;
        protected static IAssemblyFrameworkResolver assemblyFrameworkResolver = AssemblyFrameworkResolver.Instance;

        protected NoCacheAssemblyInfoService()
        {
        }

        public static NoCacheAssemblyInfoService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NoCacheAssemblyInfoService();
                }

                return instance;
            }
        }

        public AssemblyInfo GetAssemblyInfo(AssemblyDefinition assembly, IFrameworkResolver frameworkResolver)
        {
            AssemblyInfo assemblyInfo = new AssemblyInfo();

            AddModulesFrameworkVersions(assemblyInfo, assembly, frameworkResolver);
            AddAssemblyTypes(assemblyInfo, assembly);

            return assemblyInfo;
        }

        protected virtual void AddModulesFrameworkVersions(AssemblyInfo assemblyInfo, AssemblyDefinition assembly, IFrameworkResolver frameworkResolver)
        {
            foreach (ModuleDefinition module in assembly.Modules)
            {
                FrameworkVersion frameworkVersion = GetFrameworkVersionForModule(module, frameworkResolver);
                assemblyInfo.ModulesFrameworkVersions.Add(module, frameworkVersion);
            }
        }

        protected FrameworkVersion GetFrameworkVersionForModule(ModuleDefinition module, IFrameworkResolver frameworkResolver)
        {
            if (assemblyFrameworkResolver.IsCLR4Assembly(module))
            {
                return this.GetFramework4Version(module, frameworkResolver);
            }
            else
            {
                return assemblyFrameworkResolver.GetFrameworkVersionForModule(module);
            }
        }

        protected virtual FrameworkVersion GetFramework4Version(ModuleDefinition module, IFrameworkResolver frameworkResolver)
        {
            FrameworkVersion frameworkVersion;
            if (!TryDetectFramework4Upgrade(module, out frameworkVersion))
            {
                frameworkVersion = frameworkResolver.GetDefaultFallbackFramework4Version();
            }

            return frameworkVersion;
        }

        protected bool TryDetectFramework4Upgrade(ModuleDefinition module, out FrameworkVersion frameworkVersion)
        {
            frameworkVersion = FrameworkVersion.Unknown;
            if (module.IsMain)
            {
                FrameworkName frameworkName;
                if (TryGetTargetFrameworkName(module.Assembly.TargetFrameworkAttributeValue, out frameworkName) &&
                    TryParseFramework4Name(frameworkName.Version.ToString(), out frameworkVersion))
                {
                    return true;
                }
                else
                {
                    bool isInFrameworkDir;
                    if (IsFramework4Assembly(module.Assembly, out isInFrameworkDir))
                    {
                        if (isInFrameworkDir)
                        {
                            frameworkVersion = Framework4VersionResolver.GetInstalledFramework4Version();
                        }
                        else
                        {
                            frameworkVersion = Framework4VersionResolver.GetFrameworkVersionByFileVersion(module.Assembly.MainModule.FilePath);
                        }

                        if (frameworkVersion != FrameworkVersion.Unknown)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool TryParseFramework4Name(string frameworkVersionAsString, out FrameworkVersion frameworkVersion)
        {
            frameworkVersion = FrameworkVersion.Unknown;

            switch (frameworkVersionAsString)
            {
                case "4.0":
                    frameworkVersion = FrameworkVersion.v4_0;
                    break;
                case "4.5":
                    frameworkVersion = FrameworkVersion.v4_5;
                    break;
                case "4.5.1":
                    frameworkVersion = FrameworkVersion.v4_5_1;
                    break;
                case "4.5.2":
                    frameworkVersion = FrameworkVersion.v4_5_2;
                    break;
                case "4.6":
                    frameworkVersion = FrameworkVersion.v4_6;
                    break;
                case "4.6.1":
                    frameworkVersion = FrameworkVersion.v4_6_1;
                    break;
                case "4.6.2":
                    frameworkVersion = FrameworkVersion.v4_6_2;
                    break;
                case "4.7":
                    frameworkVersion = FrameworkVersion.v4_7;
                    break;
				case "4.7.1":
					frameworkVersion = FrameworkVersion.v4_7_1;
					break;
                /* AGPL */
                case "4.7.2":
                    frameworkVersion = FrameworkVersion.v4_7_2;
                    break;
                case "4.8":
                    frameworkVersion = FrameworkVersion.v4_8;
                    break;
                /* End AGPL */
                default:
                    return false;
            }

            return true;
        }

        private bool IsFramework4Assembly(AssemblyDefinition assembly, out bool isInFrameworkDir)
        {
            if (IsInFrameworkDir(assembly.MainModule.FilePath))
            {
                isInFrameworkDir = true;
                return true;
            }
            else
            {
                isInFrameworkDir = false;
            }

            IFrameworkAssemblyLocator assemblyLocator = FrameworkAssembly4xLocatorFactory.Instance(assembly.MainModule.Architecture);

            string assemblyFileName = Path.GetFileName(assembly.MainModule.FilePath);

            foreach (string filePath in assemblyLocator.Assemblies)
            {
                if (Path.GetFileName(filePath) == assemblyFileName)
                {
                    // check the public key token
                    ReaderParameters parameters = new ReaderParameters(assembly.MainModule.AssemblyResolver);

                    AssemblyDefinition listAssembly = AssemblyDefinition.ReadAssembly(filePath, parameters);

                    if (CompareByteArrays(listAssembly.Name.PublicKey, assembly.Name.PublicKey))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsInFrameworkDir(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath).ToLower();
            string framework32Directory = Path.Combine(SystemInformation.CLR_Default_32, "v4.0.30319").ToLower();
            string framework64Directory = Path.Combine(SystemInformation.CLR_Default_64, "v4.0.30319").ToLower();
            if (directory.StartsWith(framework32Directory) || directory.StartsWith(framework64Directory))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            if (array1 == null ^ array2 == null)
            {
                return false;
            }
            if (array1 == null && array2 == null)
            {
                return true;
            }
            if (array1.Length != array2.Length)
            {
                return false;
            }
            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }
            return true;
        }

        private bool TryGetTargetFrameworkName(string targetFrameworkAttributeValue, out FrameworkName frameworkName)
        {
            frameworkName = null;
            if (targetFrameworkAttributeValue != null)
            {
                try
                {
                    frameworkName = new FrameworkName(targetFrameworkAttributeValue);
                }
                catch (ArgumentException)
                {
                    /// The constructor throws exception if the version string is incorrectly formated.
                    return false;
                }

                return true;
            }

            return false;
        }

        protected virtual void AddAssemblyTypes(AssemblyInfo assemblyInfo, AssemblyDefinition assembly)
        {
            assemblyInfo.AssemblyTypes = GetAssemblyTypes(assembly);
        }

        protected AssemblyTypes GetAssemblyTypes(AssemblyDefinition assembly)
        {
            AssemblyTypes assemblyTypes = AssemblyTypes.Unknown;

            if (assembly.Name.PublicKeyTokenAsString == "b77a5c561934e089" ||
                assembly.Name.PublicKeyTokenAsString == "b03f5f7f11d50a3a" ||
                assembly.Name.PublicKeyTokenAsString == "31bf3856ad364e35")
            {
                assemblyTypes |= AssemblyTypes.FrameworkAssembly;
            }

            foreach (AssemblyNameReference reference in assembly.MainModule.AssemblyReferences)
            {
                if (reference.Name == "PresentationFramework")
                {
                    assemblyTypes |= AssemblyTypes.WPF;
                }
                else if (reference.Name == "System.Windows.Forms")
                {
                    assemblyTypes |= AssemblyTypes.WinForms;
                }
				else if (reference.Name.StartsWith("Microsoft.AspNetCore"))
				{
					assemblyTypes |= AssemblyTypes.AspNetCore;
				}
				else if (reference.Name == "System.Runtime" && reference.Version == new Version(4, 2, 0, 0))
				{
					assemblyTypes |= AssemblyTypes.NetCore;
				}
                else if (reference.Name == "System.Web.Mvc")
                {
                    assemblyTypes |= AssemblyTypes.MVC;
                }
                else if (reference.Name == "Windows")
                {
                    assemblyTypes |= AssemblyTypes.Windows8Application;
                }
                else if (reference.Name == "Windows.Foundation.UniversalApiContract")
                {
                    assemblyTypes |= AssemblyTypes.UniversalWindows;
                }
                else if (reference.Name == "Mono.Android")
                {
                    assemblyTypes |= AssemblyTypes.XamarinAndroid;
                }
                else if (reference.Name == "Xamarin.iOS" || reference.Name == "monotouch")
                {
                    assemblyTypes |= AssemblyTypes.XamarinIOS;
                }
            }

            return assemblyTypes;
        }
	}
}