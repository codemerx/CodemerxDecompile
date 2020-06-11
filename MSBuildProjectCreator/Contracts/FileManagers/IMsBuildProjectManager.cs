using System;

namespace JustDecompile.Tools.MSBuildProjectBuilder.Contracts.FileManagers
{
	public interface IMsBuildProjectManager : IProjectManager, IProjectItemWriter, IWinFormsProjectItemWriter, IXamlPageProjectItemWriter, IAppDefinitionProjectItemWriter
	{
		void IncludeAssemblyInfo(string assemblyInfoRelativePath);

		void CreateProjectItemGroupNone(string include);

		void CreateAddModulesProjectItem(int dependingOnModulesCount);

		void AddModuleProjectItem(int moduleReferenceIndex, string include = null, string condition = null);

		void WriteAppConfigFileEntryProjectItem();
	}
}
