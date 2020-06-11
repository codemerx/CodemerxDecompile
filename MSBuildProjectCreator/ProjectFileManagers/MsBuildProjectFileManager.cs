using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.JustDecompiler.Common;
using JustDecompile.EngineInfrastructure;
using Mono.Cecil;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.VisualBasic;
using Telerik.JustDecompiler.Languages.CSharp;
using System.IO;
using System.Xml.Serialization;
using Telerik.JustDecompiler.Common.NamespaceHierarchy;
using JustDecompile.Tools.MSBuildProjectBuilder.Contracts.FileManagers;

namespace JustDecompile.Tools.MSBuildProjectBuilder.ProjectFileManagers
{
	public class MsBuildProjectFileManager : ProjectFileManager, IMsBuildProjectManager
	{
		protected VisualStudioVersion visualStudioVersion;
		protected ILanguage language;

		protected Project project;
		protected ProjectPropertyGroup mainModulebasicProjectProperties;
		protected ProjectPropertyGroup netModulebasicProjectProperties;
		protected ProjectItemGroup projectReferences;

		public MsBuildProjectFileManager(AssemblyDefinition assembly, AssemblyInfo assemblyInfo, VisualStudioVersion visualStudioVersion,
			Dictionary<ModuleDefinition, Guid> modulesProjectsGuids, ILanguage language, NamespaceHierarchyTree namespaceHierarchyTree)
			: base(assembly, assemblyInfo, modulesProjectsGuids)
		{
			this.NormalCodeEntries = new List<ProjectItemGroupCompile>();
			this.WinFormCodeEntries = new List<ProjectItemGroupCompile>();
			this.WinFormResXEntries = new List<ProjectItemGroupEmbeddedResource>();
			this.ResXEntries = new List<ProjectItemGroupEmbeddedResource>();
			this.OtherEmbeddedResources = new List<ProjectItemGroupEmbeddedResource>();
			this.OtherXamlResources = new List<ProjectItemGroupResource>();

			this.NamespacesTree = namespaceHierarchyTree;
			this.visualStudioVersion = visualStudioVersion;
			this.language = language;
		}

		public NamespaceHierarchyTree NamespacesTree { get; private set; }
		public ProjectItemGroupCompile AssemblyInfoFileEntry { get; set; }
		public ProjectItemGroupNone AppConfigFileEntry { get; set; }
		public List<ProjectItemGroupCompile> NormalCodeEntries { get; private set; }
		public List<ProjectItemGroupCompile> WinFormCodeEntries { get; private set; }
		public List<ProjectItemGroupEmbeddedResource> WinFormResXEntries { get; private set; }
		public List<ProjectItemGroupEmbeddedResource> ResXEntries { get; private set; }
		public List<ProjectItemGroupEmbeddedResource> OtherEmbeddedResources { get; private set; }
		public List<ProjectItemGroupResource> OtherXamlResources { get; private set; }

		public override void ConstructProject(ModuleDefinition module, Action<ModuleDefinition> createProjectReferences)
		{
			this.CreateRootProjectElement();

			if (module.IsMain)
			{
				this.GetMainModuleBasicProjectProperties();
			}
			else
			{
				this.GetNetModuleBasicProjectProperties(module);
			}

			this.GetProjectItems(module, createProjectReferences);
		}

		public override void CreateRootProjectElement()
		{
			this.project = new Project();

			if (this.visualStudioVersion == VisualStudioVersion.VS2010 || this.visualStudioVersion == VisualStudioVersion.VS2012)
			{
				project.ToolsVersion = 4.0M;
			}
			else if (this.visualStudioVersion == VisualStudioVersion.VS2013)
			{
				project.ToolsVersion = 12.0M;
			}
			else if (this.visualStudioVersion == VisualStudioVersion.VS2015)
			{
				project.ToolsVersion = 14.0M;
			}
			else if (this.visualStudioVersion == VisualStudioVersion.VS2017)
			{
				project.ToolsVersion = 15.0M;
			}
			else
			{
				throw new NotImplementedException();
			}

			project.DefaultTargets = "Build";
		}

		protected virtual void GetNetModuleBasicProjectProperties(ModuleDefinition netModule)
		{
			if (netModule.Kind != ModuleKind.NetModule)
			{
				throw new Exception("Unexpected type of module.");
			}

			string projectGuid = this.WriteProjectGuid(this.modulesProjectsGuids, netModule);
			string moduleName = Utilities.GetNetmoduleName(netModule);

			this.netModulebasicProjectProperties = this.GetBasicProjectPropertiesInternal(netModule, moduleName, projectGuid);
		}

		public override void GetMainModuleBasicProjectProperties()
		{
			string projectGuid = this.WriteProjectGuid(this.modulesProjectsGuids, this.assembly.MainModule);

			this.mainModulebasicProjectProperties = this.GetBasicProjectPropertiesInternal(this.assembly.MainModule, this.assembly.Name.Name, projectGuid.ToUpper());
		}

		public override void GetProjectItems(ModuleDefinition module, Action<ModuleDefinition> createProjectReferences)
		{
			this.projectItems = new List<object>();

			if (this.visualStudioVersion == VisualStudioVersion.VS2012 ||
				this.visualStudioVersion == VisualStudioVersion.VS2013 ||
				this.visualStudioVersion == VisualStudioVersion.VS2015 ||
				this.visualStudioVersion == VisualStudioVersion.VS2017)
			{
				this.projectItems.Add(GenerateCommonPropsProjectImportProperty());
			}

			this.GetModuleSpecificProperties(module.IsMain);

			createProjectReferences(module);
			this.projectItems.Add(this.projectReferences);

			this.projectItems.Add(this.GetProjectItemsItemGroup());

			if (this.language is IVisualBasic)
			{
				this.projectItems.Add(GetCompileOptions());
			}

			this.projectItems.Add(this.GenerateLanguageTargetsProjectImportProperty());

			this.project.Items = this.projectItems.ToArray();
		}

		protected virtual object GetConfiguration(string platform, bool debugConfiguration)
		{
			ProjectPropertyGroup result = new ProjectPropertyGroup();

			if (debugConfiguration)
			{
				result.Condition = " '$(Configuration)|$(Platform)' == 'Debug|" + platform + "' ";
				result.DebugSymbols = true;
				result.DebugType = "full";
				result.Optimize = false;
				result.OutputPath = GetOutputPath(platform, debugConfiguration);
			}
			else
			{
				result.Condition = " '$(Configuration)|$(Platform)' == 'Release|" + platform + "' ";
				result.DebugSymbols = false;
				result.DebugType = "pdbonly";
				result.Optimize = true;
				result.OutputPath = GetOutputPath(platform, debugConfiguration);
			}

			string separator = this.language is IVisualBasic ? "," : ";";
			string defineConstants = string.Join(separator, this.GetConfigurationConstants(debugConfiguration));
			if (defineConstants != string.Empty)
			{
				result.DefineConstants = defineConstants;
			}

			if (this.language is ICSharp)
			{
				result.ErrorReport = "prompt";
				result.WarningLevel = 4;
				result.WarningLevelSpecified = true;
			}
			else if (this.language is IVisualBasic)
			{
				result.DefineDebug = debugConfiguration;
				result.DefineDebugSpecified = true;

				result.DefineTrace = true;
				result.DefineTraceSpecified = true;

				result.DocumentationFile = string.Format("{0}.xml", this.assembly.Name.Name);
				result.NoWarn = string.Join(",", this.GetWarningConfigurations());
			}
			else
			{
				throw new NotSupportedException();
			}

			result.PlatformTarget = platform;
			result.OptimizeSpecified = true;
			result.DebugSymbolsSpecified = true;

			return result;
		}

		protected virtual string GetOutputPath(string platform, bool debugConfiguration)
		{
			if (debugConfiguration)
			{
				return @"bin\Debug\";
			}
			else
			{
				return @"bin\Release\";
			}
		}

		protected virtual IList<string> GetWarningConfigurations()
		{
			return new List<string>() { "42016", "41999", "42017", "42018", "42019", "42032", "42036", "42020", "42021", "42022" };
		}

		protected virtual List<string> GetConfigurationConstants(bool debugConfiguration)
		{
			List<string> result = new List<string>();
			if (this.language is ICSharp)
			{
				if (debugConfiguration)
				{
					result.Add("DEBUG");
				}

				result.Add("TRACE");
			}

			return result;
		}

		protected virtual ProjectImport GenerateLanguageTargetsProjectImportProperty()
		{
			if (this.language is ICSharp)
			{
				return new ProjectImport() { Project = @"$(MSBuildToolsPath)\Microsoft.CSharp.targets" };
			}
			else if (this.language is IVisualBasic)
			{
				return new ProjectImport() { Project = @"$(MSBuildToolsPath)\Microsoft.VisualBasic.targets" };
			}

			throw new NotSupportedException("Project generation not supported in this language.");
		}

		public override void SerializeProject(FileStream projectFile)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(Project));
			serializer.Serialize(projectFile, project);
		}

		protected virtual object GetCompileOptions()
		{
			ProjectPropertyGroup compileOptions = new ProjectPropertyGroup()
			{
				OptionExplicit = "On",
				OptionCompare = "Binary",
				OptionStrict = "Off",
				OptionInfer = "On"
			};

			return compileOptions;
		}

		public ProjectImport GenerateCommonPropsProjectImportProperty()
		{
			return new ProjectImport()
			{
				Project = @"$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props",
				Condition = @"Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"
			};
		}

		protected virtual void GetModuleSpecificProperties(bool isMainModule)
		{
			ProjectPropertyGroup moduleBasicProperties = isMainModule ? this.mainModulebasicProjectProperties : this.netModulebasicProjectProperties;

			this.projectItems.Add(moduleBasicProperties);
			this.projectItems.Add(this.GetConfiguration(moduleBasicProperties.Platform.Value, true)); // Debug
			this.projectItems.Add(this.GetConfiguration(moduleBasicProperties.Platform.Value, false)); // Release
		}

		private bool IsAutoGenerateBindingRedirectsSupported(ModuleDefinition module)
		{
			if (module.Kind != ModuleKind.Console && module.Kind != ModuleKind.Windows)
			{
				return false;
			}

			Version targetFrameworkVersion;
			if (Version.TryParse(this.assemblyInfo.ModulesFrameworkVersions[module].ToString(includeVersionSign: false), out targetFrameworkVersion))
			{
				if (targetFrameworkVersion >= new Version(4, 5, 1))
				{
					return true;
				}
			}

			return false;
		}

		public override void CreateReferencesProjectItem(int dependingOnAssembliesCount)
		{
			if (this.projectReferences == null)
			{
				this.projectReferences = new ProjectItemGroup();
			}

			this.projectReferences.Reference = new ProjectItemGroupReference[dependingOnAssembliesCount];
		}

		public override void AddReferenceProjectItem(int assemblyReferenceIndex, string include = null, string item = null)
		{
			if (this.projectReferences.Reference[assemblyReferenceIndex] == null)
			{
				this.projectReferences.Reference[assemblyReferenceIndex] = new ProjectItemGroupReference();
			}

			if (include != null)
			{
				this.projectReferences.Reference[assemblyReferenceIndex].Include = include;
			}

			if (item != null)
			{
				this.projectReferences.Reference[assemblyReferenceIndex].Item = item;
			}
		}

		public virtual void CreateAddModulesProjectItem(int dependingOnModulesCount)
		{
			if (this.projectReferences == null)
			{
				this.projectReferences = new ProjectItemGroup();
			}

			this.projectReferences.AddModules = new ProjectItemGroupAddModules[dependingOnModulesCount];
		}

		public virtual void AddModuleProjectItem(int moduleReferenceIndex, string include = null, string condition = null)
		{
			if (this.projectReferences.AddModules[moduleReferenceIndex] == null)
			{
				this.projectReferences.AddModules[moduleReferenceIndex] = new ProjectItemGroupAddModules();
			}

			if (include != null)
			{
				this.projectReferences.AddModules[moduleReferenceIndex].Include = include;
			}

			if (condition != null)
			{
				this.projectReferences.AddModules[moduleReferenceIndex].Condition = condition;
			}
		}

		public virtual void CreateProjectItemGroupNone(string include)
		{
			if (this.projectReferences == null)
			{
				this.projectReferences = new ProjectItemGroup();
			}

			this.projectReferences.None = new ProjectItemGroupNone()
			{
				Include = include
			};
		}

		public override object GetProjectItemsItemGroup()
		{
			ProjectItemGroup projectItemGroup = new ProjectItemGroup();

			List<ProjectItemGroupEmbeddedResource> embeddedResources = new List<ProjectItemGroupEmbeddedResource>(this.WinFormResXEntries);
			embeddedResources.AddRange(this.ResXEntries);
			embeddedResources.AddRange(this.OtherEmbeddedResources);

			projectItemGroup.EmbeddedResource = embeddedResources.ToArray();

			List<ProjectItemGroupCompile> compileFiles = new List<ProjectItemGroupCompile>();

			if (this.AssemblyInfoFileEntry != null)
			{
				compileFiles.Add(AssemblyInfoFileEntry);
			}

			compileFiles.AddRange(this.NormalCodeEntries);
			compileFiles.AddRange(this.WinFormCodeEntries);

			projectItemGroup.Compile = compileFiles.ToArray();

			if (this.AppConfigFileEntry != null)
			{
				projectItemGroup.None = this.AppConfigFileEntry;
			}

			int xamlFileEntriesCount = base.XamlFileEntries.Count;
			List<ProjectItemGroupPage> xamlPageList = new List<ProjectItemGroupPage>(xamlFileEntriesCount);

			for (int i = 0; i < xamlFileEntriesCount; i++)
			{
				if (XamlFileEntries[i] is ProjectItemGroupPage)
				{
					xamlPageList.Add((ProjectItemGroupPage)XamlFileEntries[i]);
				}
				else
				{
					projectItemGroup.ApplicationDefinition = (ProjectItemGroupApplicationDefinition)XamlFileEntries[i];
				}
			}

			projectItemGroup.Page = xamlPageList.ToArray();

			projectItemGroup.Resource = OtherXamlResources.ToArray();

			return projectItemGroup;
		}

		public virtual void WriteWinFormsEntryProjectItem(string relativeWinFormPath)
		{
			ProjectItemGroupCompile winFormEntry = new ProjectItemGroupCompile()
			{
				Include = relativeWinFormPath,
				SubType = "Form"
			};
			
			this.WinFormCodeEntries.Add(winFormEntry);
		}

		public virtual void WriteWinFormsResourceProjectItem(string relativeWinFormResourcePath, string relativeWinFormPath)
		{
			ProjectItemGroupEmbeddedResource resourceEntry = new ProjectItemGroupEmbeddedResource()
			{
				Include = relativeWinFormResourcePath,
				DependentUpon = Path.GetFileName(relativeWinFormPath)
			};
			
			this.WinFormResXEntries.Add(resourceEntry);
		}

		public override void WriteResXDesignerSourceEntryProjectItem(string relativeDesignerPath, string relativeResourcePath)
		{
			ProjectItemGroupCompile sourceEntry = new ProjectItemGroupCompile()
			{
				Include = relativeDesignerPath,
				DependentUpon = Path.GetFileName(relativeResourcePath),
				DesignTimeSharedInput = true,
				AutoGen = true
			};

			this.NormalCodeEntries.Add(sourceEntry);
		}

		public override void WriteResXDesignerResourceProjectItem(string relativeResourcePath, string relativeDesignerPath)
		{
			ProjectItemGroupEmbeddedResource resourceEntry = new ProjectItemGroupEmbeddedResource()
			{
				Include = relativeResourcePath,
				Generator = "ResXFileCodeGenerator",
				LastGenOutput = Path.GetFileName(relativeDesignerPath)
			};

			this.ResXEntries.Add(resourceEntry);
		}

		public virtual void WriteXamlCodeEntryProjectItem(string relativeCodePath, string relativeXamlPath)
		{
			ProjectItemGroupCompile codeEntry = new ProjectItemGroupCompile()
			{
				Include = relativeCodePath,
				SubType = "Code",
				DependentUpon = Path.GetFileName(relativeXamlPath)
			};

			this.NormalCodeEntries.Add(codeEntry);
		}

		public virtual void WriteAppDefinitionXamlEntryProjectItem(string relativeXamlPath)
		{
			ProjectItemGroupApplicationDefinition appDefxamlEntry = new ProjectItemGroupApplicationDefinition()
			{
				Include = relativeXamlPath,
				Generator = "MSBuild:Compile",
				SubType = "Designer"
			};

			this.XamlFileEntries.Add(appDefxamlEntry);
		}

		public virtual void WriteXamlPageProjectItem(string relativeXamlPath)
		{
			ProjectItemGroupPage pageItem = new ProjectItemGroupPage()
			{
				Include = relativeXamlPath,
				Generator = "MSBuild:Compile",
				SubType = "Designer"
			};
			
			this.XamlFileEntries.Add(pageItem);
		}

		public override void WriteRegularProjectItem(string relativePath)
		{
			ProjectItemGroupCompile projectItem = new ProjectItemGroupCompile()
			{
				Include = relativePath
			};

			this.NormalCodeEntries.Add(projectItem);
		}

		public virtual void WriteAppConfigFileEntryProjectItem()
		{
			ProjectItemGroupNone appConfigFileEntry = new ProjectItemGroupNone()
			{
				Include = "App.config"
			};

			this.AppConfigFileEntry = appConfigFileEntry;
		}

		public override void AddResourceToOtherEmbeddedResources(string resourceLegalName)
		{
			ProjectItemGroupEmbeddedResource embeddedResource = new ProjectItemGroupEmbeddedResource()
			{
				Include = resourceLegalName
			};

			this.OtherEmbeddedResources.Add(embeddedResource);
		}

		public override void AddResourceToOtherXamlResources(string xamlResourceRelativePath)
		{
			ProjectItemGroupResource xamlResource = new ProjectItemGroupResource()
			{
				Include = xamlResourceRelativePath
			};

			this.OtherXamlResources.Add(xamlResource);
		}

		public void IncludeAssemblyInfo(string assemblyInfoRelativePath)
		{
			ProjectItemGroupCompile assemblyInfoFileEntry = new ProjectItemGroupCompile()
			{
				Include = assemblyInfoRelativePath
			};

			this.AssemblyInfoFileEntry = assemblyInfoFileEntry;
		}

		private ProjectPropertyGroup GetBasicProjectPropertiesInternal(ModuleDefinition module, string assemblyName, string projectGuid)
		{
			ProjectPropertyGroup basicProjectProperties = new ProjectPropertyGroup()
			{
				TargetFrameworkVersion = this.GetTargetFrameworkVersion(module),
				AssemblyName = assemblyName,
				OutputType = this.GetOutputType(module),

				Platform = new ProjectPropertyGroupPlatform()
				{
					Condition = " '$(Platform)' == '' ",
					Value = Utilities.GetModuleArchitecturePropertyValue(module)
				},
				Configuration = new ProjectPropertyGroupConfiguration() { Condition = " '$(Configuration)' == '' ", Value = "Debug" }
			};

			basicProjectProperties.ProjectGuid = "{" + projectGuid + "}";

			if (this.visualStudioVersion == VisualStudioVersion.VS2010)
			{
				basicProjectProperties.SchemaVersion = (decimal)2.0; //constant in VS 2010
				basicProjectProperties.SchemaVersionSpecified = true;

				//constant in VS 2010roperties.ProjectType
				//basicProjectProperties.SchemaVersionSpecified

				//basicProjectProperties.ProductVersion - the version of the project template VS used to create the project file
			}

			// VB compiler injects RootNamespace in all types
			// so we let it stay in the source code and remove it from the project settings
			if (!(this.language is IVisualBasic))
			{
				basicProjectProperties.RootNamespace = this.NamespacesTree.RootNamespace;
			}

			basicProjectProperties.AutoGenerateBindingRedirects = IsAutoGenerateBindingRedirectsSupported(module);
			basicProjectProperties.AutoGenerateBindingRedirectsSpecified = basicProjectProperties.AutoGenerateBindingRedirects;

			return basicProjectProperties;
		}
	}
}
