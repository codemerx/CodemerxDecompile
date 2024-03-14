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
