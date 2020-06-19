using System;
using System.IO;
using Mono.Cecil;
using System.Collections.Generic;

namespace JustDecompile.Tools.MSBuildProjectBuilder.Contracts.FileManagers
{
	public interface IProjectManager : IProjectItemWriter
	{
		Dictionary<string, string> ResourceDesignerMap { get; set; }

		Dictionary<string, string> XamlFullNameToRelativePathMap { get; set; }

		void ConstructProject(ModuleDefinition module, Action<ModuleDefinition> createProjectReferences);

		void CreateRootProjectElement();

		void GetMainModuleBasicProjectProperties();

		void GetProjectItems(ModuleDefinition module, Action<ModuleDefinition> createProjectReferences);

		void CreateReferencesProjectItem(int dependingOnAssembliesCount);

		void AddReferenceProjectItem(int assemblyReferenceIndex, string include = null, string item = null);

		object GetProjectItemsItemGroup();

		void SerializeProject(FileStream projectFile);
	}
}
