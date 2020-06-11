using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using Mono.Cecil;
using JustDecompile.EngineInfrastructure;
using Telerik.JustDecompiler.Common;
using JustDecompile.Tools.MSBuildProjectBuilder.Contracts.FileManagers;
using Mono.Cecil.AssemblyResolver;

namespace JustDecompile.Tools.MSBuildProjectBuilder.ProjectFileManagers
{
	public class NetCoreProjectFileManager : ProjectFileManager, INetCoreProjectManager
	{
		protected NetCore.Project project;
		protected NetCore.ProjectPropertyGroup mainModulebasicProjectProperties;
		protected NetCore.ProjectItemGroup projectReferences;
		protected List<NetCore.ProjectItemGroupPackageReference> projectPackageReferences;

		public NetCoreProjectFileManager(AssemblyDefinition assembly, AssemblyInfo assemblyInfo, Dictionary<ModuleDefinition, Guid> modulesProjectsGuids)
			: base(assembly, assemblyInfo, modulesProjectsGuids)
		{
			this.projectPackageReferences = new List<NetCore.ProjectItemGroupPackageReference>();
			this.NormalCodeEntries = new List<NetCore.ProjectItemGroupCompile>();
			this.ResXEntries = new List<NetCore.ProjectItemGroupEmbeddedResource>();
			this.OtherEmbeddedResources = new List<NetCore.ProjectItemGroupEmbeddedResource>();
		}

		public List<NetCore.ProjectItemGroupCompile> NormalCodeEntries { get; private set; }
		public List<NetCore.ProjectItemGroupEmbeddedResource> ResXEntries { get; private set; }
		public List<NetCore.ProjectItemGroupEmbeddedResource> OtherEmbeddedResources { get; private set; }

		public override void CreateRootProjectElement()
		{
			this.project = new NetCore.Project();

			project.Sdk = "Microsoft.NET.Sdk";
		}

		public override void GetMainModuleBasicProjectProperties()
		{
			this.mainModulebasicProjectProperties = this.GetBasicProjectPropertiesInternal(this.assembly.MainModule, this.assembly.Name.Name);
		}

		public override void CreateReferencesProjectItem(int dependingOnAssembliesCount)
		{
			if (this.projectReferences == null)
			{
				this.projectReferences = new NetCore.ProjectItemGroup();
			}

			this.projectReferences.Reference = new NetCore.ProjectItemGroupReference[dependingOnAssembliesCount];
		}

		public override void AddReferenceProjectItem(int assemblyReferenceIndex, string include = null, string hintPath = null)
		{
			if (this.projectReferences.Reference[assemblyReferenceIndex] == null)
			{
				this.projectReferences.Reference[assemblyReferenceIndex] = new NetCore.ProjectItemGroupReference();
			}

			if (include != null)
			{
				this.projectReferences.Reference[assemblyReferenceIndex].Include = include;
			}

			if (hintPath != null)
			{
				this.projectReferences.Reference[assemblyReferenceIndex].HintPath = hintPath;
			}
		}

		public override void AddResourceToOtherEmbeddedResources(string resourceLegalName)
		{
			NetCore.ProjectItemGroupEmbeddedResource embeddedResource = new NetCore.ProjectItemGroupEmbeddedResource()
			{
				Include = resourceLegalName
			};

			this.OtherEmbeddedResources.Add(embeddedResource);
		}

		public override void AddResourceToOtherXamlResources(string xamlResourceRelativePath)
		{
			NetCore.ProjectItemGroupEmbeddedResource xamlResource = new NetCore.ProjectItemGroupEmbeddedResource()
			{
				Include = xamlResourceRelativePath
			};

			this.OtherEmbeddedResources.Add(xamlResource);
		}

		public override void GetProjectItems(ModuleDefinition module, Action<ModuleDefinition> createProjectReferences)
		{
			this.projectItems = new List<object>();

			this.projectItems.Add(this.mainModulebasicProjectProperties);

			createProjectReferences(module);

			if (this.projectPackageReferences.Count > 0)
			{
				this.projectReferences.PackageReference = this.projectPackageReferences.ToArray();
			}

			this.projectItems.Add(this.projectReferences);

			this.projectItems.Add(this.GetProjectItemsItemGroup());

			this.project.Items = this.projectItems.ToArray();
		}

		public override object GetProjectItemsItemGroup()
		{
			NetCore.ProjectItemGroup projectItemGroup = new NetCore.ProjectItemGroup();

			List<NetCore.ProjectItemGroupEmbeddedResource> embeddedResources = new List<NetCore.ProjectItemGroupEmbeddedResource>();
			embeddedResources.AddRange(this.ResXEntries);
			embeddedResources.AddRange(this.OtherEmbeddedResources);

			List<NetCore.ProjectItemGroupCompile> compileFiles = new List<NetCore.ProjectItemGroupCompile>();

			compileFiles.AddRange(this.NormalCodeEntries);

			projectItemGroup.Compile = compileFiles.ToArray();

			int xamlFileEntriesCount = base.XamlFileEntries.Count;
			List<NetCore.ProjectItemGroupEmbeddedResource> xamlPageList = new List<NetCore.ProjectItemGroupEmbeddedResource>(xamlFileEntriesCount);

			for (int i = 0; i < xamlFileEntriesCount; i++)
			{
				if (XamlFileEntries[i] is NetCore.ProjectItemGroupEmbeddedResource)
				{
					xamlPageList.Add((NetCore.ProjectItemGroupEmbeddedResource)XamlFileEntries[i]);
				}
			}

			embeddedResources.AddRange(xamlPageList.ToArray());

			projectItemGroup.EmbeddedResource = embeddedResources.ToArray();

			return projectItemGroup;
		}

		public override void SerializeProject(FileStream projectFile)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(NetCore.Project));
			XmlSerializerNamespaces noNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
			XmlWriterSettings writerSettings = new XmlWriterSettings()
			{
				Indent = true,
				OmitXmlDeclaration = true
			};

			using (XmlWriter writer = XmlWriter.Create(projectFile, writerSettings))
			{
				serializer.Serialize(writer, project, noNamespaces);
			}
		}
		
		public override void WriteRegularProjectItem(string relativePath)
		{
			NetCore.ProjectItemGroupCompile projectItem = new NetCore.ProjectItemGroupCompile()
			{
				Include = relativePath
			};

			this.NormalCodeEntries.Add(projectItem);
		}

		public override void WriteResXDesignerResourceProjectItem(string relativeResourcePath, string relativeDesignerPath)
		{
			NetCore.ProjectItemGroupEmbeddedResource resourceEntry = new NetCore.ProjectItemGroupEmbeddedResource()
			{
				Include = relativeResourcePath,
				Generator = "ResXFileCodeGenerator",
				LastGenOutput = Path.GetFileName(relativeDesignerPath)
			};

			this.ResXEntries.Add(resourceEntry);
		}

		public override void WriteResXDesignerSourceEntryProjectItem(string relativeDesignerPath, string relativeResourcePath)
		{
			NetCore.ProjectItemGroupCompile sourceEntry = new NetCore.ProjectItemGroupCompile()
			{
				Include = relativeDesignerPath,
				DependentUpon = Path.GetFileName(relativeResourcePath),
				AutoGen = true,
				DesignTime = true
			};

			this.NormalCodeEntries.Add(sourceEntry);
		}

		public virtual void AddPackageReferenceProjectItem(string include, string version)
		{
			NetCore.ProjectItemGroupPackageReference packageReference = new NetCore.ProjectItemGroupPackageReference()
			{
				Include = include,
				Version = version
			};

			this.projectPackageReferences.Add(packageReference);
		}

		protected override string GetTargetFrameworkVersion(ModuleDefinition module)
		{
			FrameworkVersion frameworkVersion = this.assemblyInfo.ModulesFrameworkVersions[module];
			if (frameworkVersion == FrameworkVersion.Unknown || frameworkVersion == FrameworkVersion.Silverlight)
			{
				return null;
			}

			return frameworkVersion.ToString(includeVersionSign: false);
		}

		private NetCore.ProjectPropertyGroup GetBasicProjectPropertiesInternal(ModuleDefinition module, string assemblyName)
		{
			NetCore.ProjectPropertyGroup basicProjectProperties = new NetCore.ProjectPropertyGroup()
			{
				TargetFramework = this.GetTargetFrameworkVersion(module),
				AssemblyName = assemblyName,
				OutputType = this.GetOutputType(module),
				EnableDefaultItems = "false"
			};

			this.WriteProjectGuid(this.modulesProjectsGuids, module);

			return basicProjectProperties;
		}
	}
}
