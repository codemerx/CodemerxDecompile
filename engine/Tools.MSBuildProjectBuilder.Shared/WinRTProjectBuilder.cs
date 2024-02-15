using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.Languages;
using JustDecompile.EngineInfrastructure;
using System.IO;
using Telerik.JustDecompiler.Languages.CSharp;
using Telerik.JustDecompiler.Languages.VisualBasic;
using Telerik.JustDecompiler.External;
using Mono.Cecil.Extensions;
using Mono.Cecil.AssemblyResolver;
using System.Xml;
using JustDecompile.Tools.MSBuildProjectBuilder.Constants;
using JustDecompile.Tools.MSBuildProjectBuilder.Contracts.FileManagers;
using JustDecompile.Tools.MSBuildProjectBuilder.ProjectFileManagers;

namespace JustDecompile.Tools.MSBuildProjectBuilder
{
    // Consider changing the way of how the different projects are build when thos class needs to be extended further. At the moment, it is
    // done by only one class WinRTProjectBuilder, which chooses what to put in the resulting project file based on the type of the WinRT
    // assembly. Better and more maintainable solution would be class hierarchy, containing class for every project type, where that's needed. 
    public class WinRTProjectBuilder : MSBuildProjectBuilder
    {
		private WinRTProjectType projectType;
        private Dictionary<string, string> dependencies;
        private List<string> runtimes;
        private Version minInstalledUAPVersion;
        private Version maxInstalledUAPVersion;
        private bool? isUWPProject;
        private HashSet<string> uwpReferenceAssemblies;

        public WinRTProjectBuilder(string assemblyPath, AssemblyDefinition assembly,
            Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<TypeDefinition>> userDefinedTypes,
            Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<Resource>> resources,
            string targetPath, ILanguage language, IDecompilationPreferences preferences, IAssemblyInfoService assemblyInfoService, VisualStudioVersion visualStudioVersion, ProjectGenerationSettings projectGenerationSettings = null)
            : base(assemblyPath, assembly, userDefinedTypes, resources, targetPath, language, null, preferences, assemblyInfoService, visualStudioVersion, projectGenerationSettings)
        {
            Initialize();

			this.projectFileManager = new WinRTProjectFileManager(this.assembly, this.assemblyInfo, this.language, this.visualStudioVersion, this.namespaceHierarchyTree, this.modulesProjectsGuids, this.projectType, this.IsUWPProject, this.minInstalledUAPVersion, this.maxInstalledUAPVersion);
		}

		public WinRTProjectBuilder(string assemblyPath, string targetPath, ILanguage language,
            IDecompilationPreferences preferences, IFileGenerationNotifier notifier,
			IAssemblyInfoService assemblyInfoService, VisualStudioVersion visualStudioVersion = VisualStudioVersion.VS2010,
			ProjectGenerationSettings projectGenerationSettings = null)
            : base(assemblyPath, targetPath, language, null, preferences, notifier, assemblyInfoService, visualStudioVersion, projectGenerationSettings)
        {
            Initialize();

			this.projectFileManager = new WinRTProjectFileManager(this.assembly, this.assemblyInfo, this.language, this.visualStudioVersion, this.namespaceHierarchyTree, this.modulesProjectsGuids, this.projectType, this.IsUWPProject, this.minInstalledUAPVersion, this.maxInstalledUAPVersion);
		}

		private bool IsUWPProject
        {
            get
            {
                if (!this.isUWPProject.HasValue)
                {
                    this.isUWPProject = this.projectType == WinRTProjectType.UWPComponent ||
                                        this.projectType == WinRTProjectType.UWPLibrary ||
                                        this.projectType == WinRTProjectType.UWPApplication;
                }

                return this.isUWPProject.Value;
            }
        }

        private HashSet<string> UWPReferenceAssemblies
        {
            get
            {
                if (this.uwpReferenceAssemblies == null)
                {
                    string maxInstalledUAPDirectory = Path.Combine(SystemInformation.UAP_PLATFORM, this.maxInstalledUAPVersion.ToString());
                    string maxInstalledUAPPlatformXmlFilePath = Path.Combine(maxInstalledUAPDirectory, "Platform.xml");
                    if (Directory.Exists(maxInstalledUAPDirectory) && File.Exists(maxInstalledUAPPlatformXmlFilePath))
                    {
                        this.uwpReferenceAssemblies = new HashSet<string>();
                        XmlDocument doc = new XmlDocument();
                        doc.Load(maxInstalledUAPPlatformXmlFilePath);
                        foreach (XmlNode node in doc.SelectNodes("//ApiContract"))
                        {
                            this.uwpReferenceAssemblies.Add(node.Attributes["name"].Value);
                        }
                    }
                    else
                    {
                        this.uwpReferenceAssemblies = UAPConstants.DefaultUAPReferences;
                    }
                }

                return this.uwpReferenceAssemblies;
            }
        }

        protected override TypeCollisionWriterContextService GetWriterContextService()
        {
            return new WinRTWriterContextService(new ProjectGenerationDecompilationCacheService(), decompilationPreferences.RenameInvalidMembers);
        }

        protected override void CreateProjectReferences(ModuleDefinition module)
        {
            base.CreateProjectReferences(module);

            if (IsUWPProject)
            {
				this.ProjectFileManager.CreateProjectItemGroupNone(ProjectJsonWriter.ProjectJsonFileName);
            }
        }

        protected override ICollection<AssemblyNameReference> FilterDependingOnAssemblies(ICollection<AssemblyNameReference> dependingOnAssemblies)
        {
            ICollection<AssemblyNameReference> result = new List<AssemblyNameReference>();

            foreach (AssemblyNameReference reference in base.FilterDependingOnAssemblies(dependingOnAssemblies))
            {
                if (IsUWPProject)
                {
                    if (WinRTConstants.NetCoreFrameworkAssemblies.Contains(reference.Name))
                    {
                        this.dependencies.Add(reference.Name, reference.Version.ToString(3));
                        continue;
                    }
                    else if (this.UWPReferenceAssemblies.Contains(reference.Name))
                    {
                        continue;
                    }
                }
                else if (reference.Name == "System.Runtime")
                {
                    continue;
                }

                result.Add(reference);
            }

            return result;
        }

        protected override void WriteModuleAdditionalFiles(ModuleDefinition module)
        {
            if (module.IsMain && IsUWPProject)
            {
                ProjectJsonWriter writer = new ProjectJsonWriter(targetDir, this.dependencies, UAPConstants.UAPPlatformIdentifier + this.minInstalledUAPVersion.ToString(3), this.runtimes);
                writer.ExceptionThrown += OnExceptionThrown;
                bool isSuccessfull = writer.WriteProjectJsonFile();
                writer.ExceptionThrown -= OnExceptionThrown;

                this.OnProjectFileCreated(new FileGeneratedInfo(writer.ProjectJsonFilePath, !isSuccessfull));
            }
        }

        protected override bool WriteSolutionFile()
        {
            SolutionWriter solutionWriter =
                new SolutionWriter(this.assembly, this.targetDir, this.filePathsService.GetSolutionRelativePath(),
                                   this.modulesToProjectsFilePathsMap, this.modulesProjectsGuids, this.visualStudioVersion,
                                   this.language);
            solutionWriter.WriteSolutionFile();

			return true;
        }

        private void Initialize()
        {
            this.projectType = WinRTProjectTypeDetector.GetProjectType(this.assembly);
            this.dependencies = new Dictionary<string, string>();
            this.runtimes = new List<string>();

            if (this.IsUWPProject)
            {
                TargetArchitecture architecture = this.assembly.MainModule.GetModuleArchitecture();
                if (architecture == TargetArchitecture.I386Windows)
                {
                    this.runtimes.Add("win10-x86");
                    this.runtimes.Add("win10-x86-aot");
                }
                else if (architecture == TargetArchitecture.AMD64Windows)
                {
                    this.runtimes.Add("win10-x64");
                    this.runtimes.Add("win10-x64-aot");
                }
                else if (architecture == TargetArchitecture.ARMv7Windows)
                {
                    this.runtimes.Add("win10-arm");
                    this.runtimes.Add("win10-arm-aot");
                }
            }

            InitializeInstalledUAPVersions();
        }

        private void InitializeInstalledUAPVersions()
        {
            Version minPossibleVersion = new Version(0, 0, 0, 0);
            Version maxPossibleVersion = new Version(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);
            
            Version minVersion = maxPossibleVersion;
            Version maxVersion = minPossibleVersion;
            if (Directory.Exists(SystemInformation.UAP_PLATFORM))
            {
                foreach (string item in Directory.EnumerateDirectories(SystemInformation.UAP_PLATFORM))
                {
                    Version currentVersion;
                    if (Version.TryParse((new DirectoryInfo(item)).Name, out currentVersion))
                    {
                        if (currentVersion < minVersion)
                        {
                            minVersion = currentVersion;
                        }

                        if (currentVersion > maxVersion)
                        {
                            maxVersion = currentVersion;
                        }
                    }
                }
            }

            this.minInstalledUAPVersion = minVersion != maxPossibleVersion ? minVersion : UAPConstants.DefaultUAPVersion;
            this.maxInstalledUAPVersion = maxVersion != minPossibleVersion ? maxVersion : UAPConstants.DefaultUAPVersion;
        }
    }
}
