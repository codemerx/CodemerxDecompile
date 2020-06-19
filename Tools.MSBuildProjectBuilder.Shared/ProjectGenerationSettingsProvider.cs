using JustDecompile.EngineInfrastructure;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.VisualBasic;
using Mono.Cecil.AssemblyResolver;

namespace JustDecompile.Tools.MSBuildProjectBuilder
{
    public static class ProjectGenerationSettingsProvider
    {
        public static ProjectGenerationSettings GetProjectGenerationSettings(string assemblyFilePath, IAssemblyInfoService assemblyInfoService,
            IFrameworkResolver frameworkResolver, VisualStudioVersion visualStudioVersion, ILanguage language, ITargetPlatformResolver targetPlatformResolver)
        {
            AssemblyDefinition assembly = Telerik.JustDecompiler.Decompiler.Utilities.GetAssembly(assemblyFilePath);
            AssemblyInfo assemblyInfo = assemblyInfoService.GetAssemblyInfo(assembly, frameworkResolver);
            TargetPlatform targetPlatform = targetPlatformResolver.GetTargetPlatform(assembly.MainModule.FilePath, assembly.MainModule);

            foreach (KeyValuePair<ModuleDefinition, FrameworkVersion> pair in assemblyInfo.ModulesFrameworkVersions)
            {
                if (pair.Value == FrameworkVersion.Unknown)
                {
                    return new ProjectGenerationSettings(true, ResourceStrings.GenerateOnlySourceFilesDueToUnknownFrameworkVersion, false);
                }
                else if (pair.Value == FrameworkVersion.WindowsCE || 
					(targetPlatform == TargetPlatform.WindowsPhone && pair.Value == FrameworkVersion.WindowsPhone) ||
                    (targetPlatform == TargetPlatform.WinRT && WinRTProjectTypeDetector.GetProjectType(assembly) == WinRTProjectType.Unknown))
                {
                    return new ProjectGenerationSettings(true, ResourceStrings.GenerateOnlySourceFilesDueToNotSupportedProjectType, false);
                }
            }

            string resultErrorMessage;
            if (visualStudioVersion == VisualStudioVersion.VS2010)
            {
                if (!CanBe2010ProjectCreated(assemblyInfo, targetPlatform, out resultErrorMessage))
                {
                    return new ProjectGenerationSettings(false, resultErrorMessage);
                }
            }
            else if (visualStudioVersion == VisualStudioVersion.VS2012)
            {
                if (!CanBe2012ProjectCreated(assembly, out resultErrorMessage))
                {
                    return new ProjectGenerationSettings(false, resultErrorMessage);
                }
            }
            else
            {
                if (targetPlatform == TargetPlatform.WinRT && language is IVisualBasic &&
                    WinRTProjectTypeDetector.GetProjectType(assembly) == WinRTProjectType.ComponentForUniversal)
                {
                    resultErrorMessage = string.Format(ResourceStrings.CannotCreateProjectForComponentForUniversalInVB, visualStudioVersion.ToFriendlyString());
                    return new ProjectGenerationSettings(false, resultErrorMessage);
                }

                if (visualStudioVersion == VisualStudioVersion.VS2013)
                {
                    if (!CanBe2013ProjectCreated(assembly, language, out resultErrorMessage))
                    {
                        return new ProjectGenerationSettings(false, resultErrorMessage);
                    }
                }
            }
            
            return new ProjectGenerationSettings(true);
        }

        private static bool CanBe2010ProjectCreated(AssemblyInfo assemblyInfo, TargetPlatform targetPlatform, out string errorMessage)
        {
            if (targetPlatform == TargetPlatform.WinRT)
            {
                errorMessage = ResourceStrings.CannotCreate2010ProjectDueToWinRT;
                return false;
            }

            Version max = new Version(0, 0);
            foreach (KeyValuePair<ModuleDefinition, FrameworkVersion> pair in assemblyInfo.ModulesFrameworkVersions)
            {
                Version current;
                if (Version.TryParse(pair.Value.ToString(false), out current))
                {
                    if (current > max)
                    {
                        max = current;
                    }
                }
            }

            if (max > new Version(4, 0))
            {
                errorMessage = ResourceStrings.CannotCreate2010ProjectDueToFramework45;
                return false;
            }
            else
            {
                errorMessage = null;
                return true;
            }
        }

        private static bool CanBe2012ProjectCreated(AssemblyDefinition assembly, out string errorMessage)
        {
            if (WinRTProjectTypeDetector.IsWinRTAssemblyGeneratedWithVS2013(assembly))
            {
                errorMessage = ResourceStrings.CannotCreate2012Project;
                return false;
            }
            else if (WinRTProjectTypeDetector.IsUniversalWindowsPlatformAssembly(assembly))
            {
                errorMessage = string.Format(ResourceStrings.CannotCreateProjectDueToUWP, 2012);
                return false;
            }

            errorMessage = null;
            return true;
        }

        private static bool CanBe2013ProjectCreated(AssemblyDefinition assembly, ILanguage language, out string errorMessage)
        {
            if (WinRTProjectTypeDetector.IsUniversalWindowsPlatformAssembly(assembly))
            {
                errorMessage = string.Format(ResourceStrings.CannotCreateProjectDueToUWP, 2013);
                return false;
            }
            
            errorMessage = null;
            return true;
        }
    }
}
