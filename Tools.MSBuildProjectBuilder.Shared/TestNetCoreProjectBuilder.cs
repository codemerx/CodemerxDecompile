using System;
using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.Languages;
using JustDecompile.EngineInfrastructure;
using Mono.Cecil.AssemblyResolver;
using JustDecompile.Tools.MSBuildProjectBuilder.NetCore;
using JustDecompile.Tools.MSBuildProjectBuilder.ProjectFileManagers;

namespace JustDecompile.Tools.MSBuildProjectBuilder
{
	public class TestNetCoreProjectBuilder : NetCoreProjectBuilder
	{
        public TestNetCoreProjectBuilder(string assemblyPath, string targetPath, ILanguage language, ProjectGenerationSettings projectGenerationSettings = null, IProjectGenerationNotifier projectNotifier = null, FrameworkVersion defaultFrameworkVersion = FrameworkVersion.NetCoreV2_0)
            : base(assemblyPath, targetPath, language, new DecompilationPreferences() { RenameInvalidMembers = true, WriteDocumentation = true, WriteFullNames = false, DecompileDangerousResources = true }, null, NoCacheAssemblyInfoService.Instance, VisualStudioVersion.VS2017, projectGenerationSettings, projectNotifier)
		{
			this.exceptionFormater = TestCaseExceptionFormatter.Instance;
			this.currentAssemblyResolver.ClearCache();

			this.projectFileManager = new TestNetCoreProjectFileManager(this.assembly, this.modulesProjectsGuids, defaultFrameworkVersion);
		}
	}
}
