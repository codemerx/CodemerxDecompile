/*
    Copyright CodeMerx 2024
    This file is part of CodemerxDecompile.

    CodemerxDecompile is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    CodemerxDecompile is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.
*/

using System.IO;
using System.Text;
using JustDecompile.EngineInfrastructure;
using JustDecompile.Tools.MSBuildProjectBuilder;
using JustDecompile.Tools.MSBuildProjectBuilder.NetCore;
using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;
using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.Languages;

namespace CodemerxDecompile.Services;

public class ProjectGenerationService : IProjectGenerationService
{
    public string GenerateProject(AssemblyDefinition assembly, VisualStudioVersion visualStudioVersion, ILanguage language, string outputPath)
    {
        var assemblyFilePath = assembly.MainModule.FilePath;
        var assemblyInfoService = NoCacheAssemblyInfoService.Instance;
        var frameworkResolver = EmptyResolver.Instance;
        var targetPlatformResolver = TargetPlatformResolver.Instance;
        var settings = ProjectGenerationSettingsProvider.GetProjectGenerationSettings(assemblyFilePath, assemblyInfoService, frameworkResolver, visualStudioVersion, language, targetPlatformResolver);
        var projFilePath = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(assemblyFilePath) + language.VSProjectFileExtension + (settings.JustDecompileSupportedProjectType ? string.Empty : BaseProjectBuilder.ErrorFileExtension));

        var preferences = new DecompilationPreferences
        {
            WriteFullNames = false,
            WriteDocumentation = true,
            RenameInvalidMembers = true,
            WriteLargeNumbersInHex = false,
            DecompileDangerousResources = false    // TODO: Ask the user whether we should decompile those
        };

        var projectBuilder = GetProjectBuilder(assembly, assemblyFilePath, visualStudioVersion, settings, language, projFilePath, preferences, frameworkResolver, targetPlatformResolver);
        return CreateProject(projectBuilder);
    }

    private BaseProjectBuilder GetProjectBuilder(AssemblyDefinition assembly, string targetPath, VisualStudioVersion visualStudioVersion, ProjectGenerationSettings settings, ILanguage language, string projFilePath, DecompilationPreferences preferences, IFrameworkResolver frameworkResolver, ITargetPlatformResolver targetPlatformResolver)
    {
        var targetPlatform = targetPlatformResolver.GetTargetPlatform(assembly.MainModule.FilePath, assembly.MainModule);
        return targetPlatform switch
        {
            TargetPlatform.NetCore => new NetCoreProjectBuilder(targetPath, projFilePath, language, preferences, null, NoCacheAssemblyInfoService.Instance, visualStudioVersion, settings),
            TargetPlatform.WinRT => new WinRTProjectBuilder(targetPath, projFilePath, language, preferences, null, NoCacheAssemblyInfoService.Instance, visualStudioVersion, settings),
            _ => new MSBuildProjectBuilder(targetPath, projFilePath, language, frameworkResolver, preferences, null, NoCacheAssemblyInfoService.Instance, visualStudioVersion, settings)
        };
    }

    private string CreateProject(BaseProjectBuilder projectBuilder)
    {
        var builder = new StringBuilder();

        BaseProjectBuilder.ProjectGenerationFailureEventHandler onProjectGenerationFailure = (_, e) =>
        {
            builder.AppendLine(e.Message);
        };

        projectBuilder.ProjectGenerationFailure += onProjectGenerationFailure;

        projectBuilder.BuildProject();

        projectBuilder.ProjectGenerationFailure -= onProjectGenerationFailure;

        return builder.ToString();
    }
}
