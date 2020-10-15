using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JustDecompile.EngineInfrastructure;
using Mono.Cecil;
using Telerik.JustDecompiler.Common.NamespaceHierarchy;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.VisualBasic;
using Telerik.JustDecompiler.Languages.CSharp;
using JustDecompile.Tools.MSBuildProjectBuilder.Constants;
using JustDecompile.Tools.MSBuildProjectBuilder.Contracts.FileManagers;

namespace JustDecompile.Tools.MSBuildProjectBuilder.ProjectFileManagers
{
	public class WinRTProjectFileManager : MsBuildProjectFileManager
	{
		private readonly WinRTProjectType projectType;
		private Version minInstalledUAPVersion;
		private Version maxInstalledUAPVersion;
		private bool isUWPProject;

		public WinRTProjectFileManager(AssemblyDefinition assembly, AssemblyInfo assemblyInfo, ILanguage language, VisualStudioVersion visualStudioVersion,
			NamespaceHierarchyTree namespaceHierarchyTree, Dictionary<ModuleDefinition, Guid> modulesProjectsGuids, WinRTProjectType projectType, bool isUWPProject, Version minInstalledUAPVersion, Version maxInstalledUAPVersion)
			: base(assembly, assemblyInfo, visualStudioVersion, modulesProjectsGuids, language, namespaceHierarchyTree)
		{
			this.projectType = projectType;
			this.isUWPProject = isUWPProject;
			this.minInstalledUAPVersion = minInstalledUAPVersion;
			this.maxInstalledUAPVersion = maxInstalledUAPVersion;
		}

		public override void GetProjectItems(ModuleDefinition module, Action<ModuleDefinition> createProjectReferences)
		{
			bool shouldAddVisualBasicItems = this.language is IVisualBasic && this.projectType != WinRTProjectType.ComponentForUniversal;

			this.projectItems = new List<object>();

			this.projectItems.Add(this.GenerateCommonPropsProjectImportProperty());

			this.GetModuleSpecificProperties(module.IsMain);

			createProjectReferences(module);
			this.projectItems.Add(this.projectReferences);

			if (this.projectType == WinRTProjectType.ComponentForUniversal)
			{
				ProjectItemGroup targetPlatform = new ProjectItemGroup()
				{
					TargetPlatform = new ProjectItemGroupTargetPlatform[]
					{
						new ProjectItemGroupTargetPlatform() { Include = "WindowsPhoneApp, Version=8.1" },
						new ProjectItemGroupTargetPlatform() { Include = "Windows, Version=8.1" }
					}
				};

				this.projectItems.Add(targetPlatform);
			}

			this.projectItems.Add(this.GetProjectItemsItemGroup());

			if (this.projectType != WinRTProjectType.ComponentForUniversal)
			{
				this.projectItems.Add(this.GetVisualStudioVersionPropertyGroup());
			}

			if (this.projectType == WinRTProjectType.ComponentForWindowsPhone)
			{
				ProjectPropertyGroup targetPlatformIdentifier = new ProjectPropertyGroup()
				{
					Condition = " '$(TargetPlatformIdentifier)' == '' ",
					TargetPlatformIdentifier = "WindowsPhoneApp"
				};

				this.projectItems.Add(targetPlatformIdentifier);
			}

			if (shouldAddVisualBasicItems)
			{
				this.projectItems.Add(this.GetCompileOptions());
			}

			this.projectItems.Add(this.GenerateLanguageTargetsProjectImportProperty());

			this.project.Items = this.projectItems.ToArray();
		}

		protected override object GetConfiguration(string platform, bool debugConfiguration)
		{
			ProjectPropertyGroup configuration = (base.GetConfiguration(platform, debugConfiguration) as ProjectPropertyGroup);

			configuration.UseVSHostingProcess = false;
			configuration.UseVSHostingProcessSpecified = true;

			if (this.IsApplicationProject() && platform == "AnyCPU")
			{
				configuration.Prefer32Bit = true;
				configuration.Prefer32BitSpecified = true;
			}

			if (this.language is ICSharp)
			{
				configuration.NoWarn = ";2008";
				configuration.WarningLevelSpecified = false;
			}
			else if (this.language is IVisualBasic)
			{
				if (debugConfiguration)
				{
					configuration.NoConfig = true;
					configuration.NoConfigSpecified = true;
				}
				else
				{
					configuration.NoStdLib = true;
					configuration.NoStdLibSpecified = true;
					configuration.NoConfig = true;
					configuration.NoConfigSpecified = true;
				}
			}

			if (this.projectType == WinRTProjectType.UWPApplication)
			{
				configuration.UseDotNetNativeToolchain = !debugConfiguration;
				configuration.UseDotNetNativeToolchainSpecified = true;
			}

			return configuration;
		}

		public override void GetMainModuleBasicProjectProperties()
		{
			base.GetMainModuleBasicProjectProperties();

			this.AddAdditionalProjectProperties(this.mainModulebasicProjectProperties);
		}

		protected override void GetNetModuleBasicProjectProperties(ModuleDefinition netModule)
		{
			base.GetNetModuleBasicProjectProperties(netModule);

			this.AddAdditionalProjectProperties(this.netModulebasicProjectProperties);
		}

		private ProjectPropertyGroup GetVisualStudioVersionPropertyGroup()
		{
			string visualStudioProductVersion;

			if (this.visualStudioVersion == VisualStudioVersion.VS2012)
			{
				visualStudioProductVersion = "11.0";
			}
			else if (this.visualStudioVersion == VisualStudioVersion.VS2013)
			{
				visualStudioProductVersion = "12.0";
			}
			else if (this.visualStudioVersion == VisualStudioVersion.VS2015)
			{
				visualStudioProductVersion = "14.0";
			}
			else if (this.visualStudioVersion == VisualStudioVersion.VS2017)
			{
				visualStudioProductVersion = "15.0";
			}
			/* AGPL */
			else if (this.visualStudioVersion == VisualStudioVersion.VS2019)
            {
				visualStudioProductVersion = "16.0";
            }
			/* End AGPL */
			else
			{
				throw new NotSupportedException();
			}

			return new ProjectPropertyGroup()
			{
				Condition = " '$(VisualStudioVersion)' == '' or '$(VisualStudioVersion)' < '" + visualStudioProductVersion + "' ",
				VisualStudioVersion = visualStudioProductVersion
			};
		}

		private int GetNumberOfProjectItems(bool shouldAddVisualBasicItems)
		{
			int numberOfItems = 8;
			if (this.projectType == WinRTProjectType.ComponentForWindowsPhone)
			{
				numberOfItems++;
			}

			if (shouldAddVisualBasicItems)
			{
				numberOfItems++;
			}

			return numberOfItems;
		}

		private void AddAdditionalProjectProperties(ProjectPropertyGroup project)
		{
			project.ProjectTypeGuids = this.GetProjectTypeGuids(this.assembly.MainModule);
			project.MinimumVisualStudioVersion = this.GetMinimumVisualStudioVersion();
			project.TargetPlatformVersion = this.GetTargetPlatformVersion();
			project.TargetFrameworkProfile = this.GetTargetFrameworkProfile();

			if (this.isUWPProject)
			{
				project.TargetPlatformMinVersion = this.minInstalledUAPVersion.ToString();
				project.TargetPlatformIdentifier = UAPConstants.UAPPlatformIdentifier;
			}

			project.AllowCrossPlatformRetargeting = false;
			project.AllowCrossPlatformRetargetingSpecified = this.projectType == WinRTProjectType.UWPComponent;
		}

		protected override IList<string> GetWarningConfigurations()
		{
			IList<string> result = base.GetWarningConfigurations();
			result.Add("42314");

			return result;
		}

		protected override List<string> GetConfigurationConstants(bool debugConfiguration)
		{
			List<string> result = base.GetConfigurationConstants(debugConfiguration);

			if (this.projectType != WinRTProjectType.ComponentForUniversal)
			{
				result.Add("NETFX_CORE");
			}

			if (this.language is ICSharp)
			{
				if (this.projectType == WinRTProjectType.ComponentForWindows)
				{
					result.Add("WINDOWS_APP");
				}
				else if (this.projectType == WinRTProjectType.ComponentForWindowsPhone)
				{
					result.Add("WINDOWS_PHONE_APP");
				}
			}

			if (this.isUWPProject)
			{
				result.Add("WINDOWS_UWP");
			}

			return result;
		}

		protected override string GetOutputPath(string platform, bool debugConfiguration)
		{
			if (platform == "AnyCPU")
			{
				return base.GetOutputPath(platform, debugConfiguration);
			}

			if (debugConfiguration)
			{
				return @"bin\" + platform + @"\Debug\";
			}
			else
			{
				return @"bin\" + platform + @"\Release\";
			}
		}

		protected override ProjectImport GenerateLanguageTargetsProjectImportProperty()
		{
			if (this.projectType == WinRTProjectType.ComponentForUniversal)
			{
				if (this.language is ICSharp)
				{
					return new ProjectImport() { Project = @"$(MSBuildExtensionsPath32)\Microsoft\Portable\$(TargetFrameworkVersion)\Microsoft.Portable.CSharp.targets" };
				}
				else if (this.language is IVisualBasic)
				{
					return new ProjectImport() { Project = @"$(MSBuildExtensionsPath32)\Microsoft\Portable\$(TargetFrameworkVersion)\Microsoft.Portable.VisualBasic.targets" };
				}
			}
			else
			{
				if (this.language is ICSharp)
				{
					return new ProjectImport() { Project = @"$(MSBuildExtensionsPath)\Microsoft\WindowsXaml\v$(VisualStudioVersion)\Microsoft.Windows.UI.Xaml.CSharp.targets" };
				}
				else if (this.language is IVisualBasic)
				{
					return new ProjectImport() { Project = @"$(MSBuildExtensionsPath)\Microsoft\WindowsXaml\v$(VisualStudioVersion)\Microsoft.Windows.UI.Xaml.VisualBasic.targets" };
				}
			}

			throw new NotSupportedException("Project generation not supported in this language.");
		}

		protected override string GetOutputType(ModuleDefinition module)
		{
			if (module.IsMain)
			{
				if (this.projectType == WinRTProjectType.UWPLibrary)
				{
					return "Library";
				}
				else if (this.projectType == WinRTProjectType.UWPApplication)
				{
					return "AppContainerExe";
				}

				return "winmdobj";
			}
			else
			{
				// Not sure about this case, because can't generate multi-module assembly for WinRT.
				return "module";
			}
		}

		protected override string GetTargetFrameworkVersion(ModuleDefinition module)
		{
			if (this.projectType == WinRTProjectType.ComponentForUniversal)
			{
				string[] versionStringParts = this.assembly.TargetFrameworkAttributeValue.Split(',');
				string[] versionPart = versionStringParts[1].Split('=');

				return versionPart[1];
			}

			return null;
		}

		private string GetProjectTypeGuids(ModuleDefinition module)
		{
			string result = string.Empty;

			if (this.projectType == WinRTProjectType.Unknown)
			{
				return null;
			}

			if (this.projectType == WinRTProjectType.Component || this.projectType == WinRTProjectType.ComponentForWindows)
			{
				result += WinRTConstants.WindowsStoreAppGUID;
			}
			else if (this.projectType == WinRTProjectType.ComponentForUniversal)
			{
				result += WinRTConstants.PortableClassLibraryGUID;
			}
			else if (this.projectType == WinRTProjectType.ComponentForWindowsPhone)
			{
				result += WinRTConstants.WindowsPhoneAppGUID;
			}
			else if (this.isUWPProject)
			{
				result += WinRTConstants.UWPProjectGUID;
			}

			result += ";";

			if (this.language is ICSharp)
			{
				result += ("{" + LanguageConstants.CSharpGUID + "}");
			}
			else if (this.language is IVisualBasic)
			{
				result += ("{" + LanguageConstants.VisualBasicGUID + "}");
			}

			return result;
		}

		private string GetMinimumVisualStudioVersion()
		{
			if (this.projectType == WinRTProjectType.ComponentForUniversal ||
				this.projectType == WinRTProjectType.ComponentForWindows ||
				this.projectType == WinRTProjectType.ComponentForWindowsPhone)
			{
				return "12.0";
			}
			else if (this.isUWPProject)
			{
				return "14";
			}

			return null;
		}

		private string GetTargetPlatformVersion()
		{
			if (this.projectType == WinRTProjectType.ComponentForWindows || this.projectType == WinRTProjectType.ComponentForWindowsPhone)
			{
				return "8.1";
			}
			else if (this.isUWPProject)
			{
				return this.maxInstalledUAPVersion.ToString();
			}

			return null;
		}

		private string GetTargetFrameworkProfile()
		{
			if (this.projectType == WinRTProjectType.ComponentForUniversal)
			{
				string[] versionStringParts = this.assembly.TargetFrameworkAttributeValue.Split(',');
				string[] profilePart = versionStringParts[2].Split('=');

				return profilePart[1];
			}

			return null;
		}

		private bool IsApplicationProject()
		{
			return this.projectType == WinRTProjectType.UWPApplication;
		}
	}
}
