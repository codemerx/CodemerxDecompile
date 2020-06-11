using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Mono.Cecil;
using JustDecompile.EngineInfrastructure;
using Telerik.JustDecompiler.Common;
using JustDecompile.Tools.MSBuildProjectBuilder.Contracts.FileManagers;
using Mono.Cecil.AssemblyResolver;

namespace JustDecompile.Tools.MSBuildProjectBuilder.ProjectFileManagers
{
	public abstract class ProjectFileManager : IProjectManager
	{
		protected AssemblyDefinition assembly;
		protected AssemblyInfo assemblyInfo;
		protected List<object> projectItems;
		protected Dictionary<ModuleDefinition, Guid> modulesProjectsGuids;

		public ProjectFileManager(AssemblyDefinition assembly, AssemblyInfo assemblyInfo, Dictionary<ModuleDefinition, Guid> modulesProjectsGuids)
		{
			this.XamlFileEntries = new List<object>();
			this.ResourceDesignerMap = new Dictionary<string, string>();
			this.XamlFullNameToRelativePathMap = new Dictionary<string, string>();

			this.assembly = assembly;
			this.assemblyInfo = assemblyInfo;
			this.modulesProjectsGuids = modulesProjectsGuids;
		}

		public Dictionary<string, string> ResourceDesignerMap { get; set; }
		public Dictionary<string, string> XamlFullNameToRelativePathMap { get; set; }
		public List<object> XamlFileEntries { get; private set; }

		public virtual void ConstructProject(ModuleDefinition module, Action<ModuleDefinition> createProjectReferences)
		{
			this.CreateRootProjectElement();

			this.GetMainModuleBasicProjectProperties();

			this.GetProjectItems(module, createProjectReferences);
		}

		public abstract void CreateRootProjectElement();

		public abstract void GetMainModuleBasicProjectProperties();

		public abstract void GetProjectItems(ModuleDefinition module, Action<ModuleDefinition> createProjectReferences);

		public abstract void CreateReferencesProjectItem(int dependingOnAssembliesCount);

		public abstract void AddReferenceProjectItem(int assemblyReferenceIndex, string include = null, string item = null);

		public abstract void SerializeProject(FileStream projectFile);

		public abstract object GetProjectItemsItemGroup();

		public abstract void AddResourceToOtherEmbeddedResources(string resourceLegalName);

		public abstract void AddResourceToOtherXamlResources(string xamlResourceRelativePath);

		public abstract void WriteRegularProjectItem(string relativePath);

		public abstract void WriteResXDesignerResourceProjectItem(string relativeResourcePath, string relativeDesignerPath);

		public abstract void WriteResXDesignerSourceEntryProjectItem(string relativeDesignerPath, string relativeResourcePath);

		protected virtual string WriteProjectGuid(Dictionary<ModuleDefinition, Guid> modulesProjectsGuids, ModuleDefinition module)
		{
			Guid guid = Guid.NewGuid();
			modulesProjectsGuids.Add(module, guid);

			return guid.ToString();
		}
		
		protected virtual string GetTargetFrameworkVersion(ModuleDefinition module)
		{
			//TODO: handle Silverlight/WinPhone projects
			FrameworkVersion frameworkVersion = this.assemblyInfo.ModulesFrameworkVersions[module];
			if (frameworkVersion == FrameworkVersion.Unknown || frameworkVersion == FrameworkVersion.Silverlight)
			{
				return null;
			}

			return frameworkVersion.ToString(includeVersionSign: true);
		}

		protected virtual string GetOutputType(ModuleDefinition module)
		{
			switch (module.Kind)
			{
				case ModuleKind.Dll:
					return "Library";
				case ModuleKind.Console:
					return "Exe";
				case ModuleKind.Windows:
					return "WinExe";
				case ModuleKind.NetModule:
					return "module";
				default:
					throw new NotImplementedException();
			}
		}
	}
}
