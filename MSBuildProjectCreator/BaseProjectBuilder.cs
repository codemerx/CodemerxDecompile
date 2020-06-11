using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Resources;
using System.Collections;
using System.Xml.Linq;

#if !NET35
using System.Threading.Tasks;
#endif

using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;
using Mono.Cecil.Extensions;
using JustDecompile.EngineInfrastructure;
using JustDecompile.SmartAssembly.Attributes;
using Telerik.JustDecompiler;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Common.NamespaceHierarchy;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.Languages;
using JustDecompile.Tools.MSBuildProjectBuilder.FilePathsServices;
using JustDecompile.Tools.MSBuildProjectBuilder.ProjectItemFileWriters;
using JustDecompile.Tools.MSBuildProjectBuilder.Contracts.FileManagers;

#if !ENGINEONLYBUILD && !JUSTASSEMBLY
using Telerik.Baml;
#endif

namespace JustDecompile.Tools.MSBuildProjectBuilder
{
	[DoNotPrune]
	[DoNotObfuscateType]
	public abstract class BaseProjectBuilder : ExceptionThrownNotifier, IExceptionThrownNotifier
	{
		public const int MaxPathLength = 259; // 259 + NULL == 260
		public const string ErrorFileExtension = ".error";

		protected readonly string assemblyPath;
		protected AssemblyDefinition assembly;
		protected readonly string targetDir;
		protected readonly ILanguage language;
		protected IFileGenerationNotifier fileGeneratedNotifier;
		protected IProjectGenerationNotifier projectNotifier;
		protected Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<TypeDefinition>> userDefinedTypes;
		protected Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<Resource>> resources;
		protected readonly IFrameworkResolver frameworkResolver;
		protected ProjectGenerationSettings projectGenerationSettings;
		protected readonly IAssemblyResolver currentAssemblyResolver;
		protected IFilePathsService filePathsService;
		protected VisualStudioVersion visualStudioVersion;
		protected readonly IDecompilationPreferences decompilationPreferences;
		protected readonly NamespaceHierarchyTree namespaceHierarchyTree;
		protected Dictionary<ModuleDefinition, Guid> modulesProjectsGuids;
		protected readonly Dictionary<ModuleDefinition, string> modulesToProjectsFilePathsMap;
		protected readonly Dictionary<Resource, string> resourcesToPathsMap;
		protected readonly Dictionary<string, string> xamlResourcesToPathsMap;
		protected IExceptionFormatter exceptionFormater = SimpleExceptionFormatter.Instance;
		protected readonly Dictionary<string, ICollection<string>> xamlGeneratedFields = new Dictionary<string, ICollection<string>>();
		protected IProjectManager projectFileManager;
		protected IAssemblyInfoService assemblyInfoService;
		protected AssemblyInfo assemblyInfo;

		private static object writeTypesLock = new object();

		public event EventHandler ProjectGenerationFinished;
		public event EventHandler<ProjectFileCreated> ProjectFileCreated;
		public event ProjectGenerationFailureEventHandler ProjectGenerationFailure;
		public event TypeWritingFailureEventHandler TypeWritingFailure;
		public event ResourceWritingFailureEventHandler ResourceWritingFailure;

		public delegate void TypeWritingFailureEventHandler(object sender, string typeName, Exception ex);
		public delegate void ResourceWritingFailureEventHandler(object sender, string resourceName, Exception ex);
		public delegate void ProjectGenerationFailureEventHandler(object sender, Exception ex);

		public BaseProjectBuilder(string assemblyPath, AssemblyDefinition assembly,
			Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<TypeDefinition>> userDefinedTypes,
 			Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<Resource>> resources,
			string targetPath, ILanguage language, IFrameworkResolver frameworkResolver,
			IDecompilationPreferences preferences, IAssemblyInfoService assemblyInfoService,
			VisualStudioVersion visualStudioVersion = VisualStudioVersion.VS2010, ProjectGenerationSettings projectGenerationSettings = null,
			IProjectGenerationNotifier projectNotifier = null)
		{
			this.assemblyPath = assemblyPath;
			this.assembly = assembly;
			this.userDefinedTypes = userDefinedTypes;
			this.resources = resources;
			this.TargetPath = targetPath;
			this.targetDir = Path.GetDirectoryName(targetPath);
			this.language = language;
			this.frameworkResolver = frameworkResolver;
			this.assemblyInfoService = assemblyInfoService;
			this.currentAssemblyResolver = assembly.MainModule.AssemblyResolver;

			this.visualStudioVersion = visualStudioVersion;
			this.projectGenerationSettings = projectGenerationSettings;

			this.decompilationPreferences = preferences;

			this.namespaceHierarchyTree = assembly.BuildNamespaceHierarchyTree();

			filePathsService =
				new DefaultFilePathsService(
					this.assembly,
					this.assemblyPath,
					Path.GetFileName(this.TargetPath),
					this.UserDefinedTypes,
					this.Resources,
					this.namespaceHierarchyTree,
					this.language,
					Utilities.GetMaxRelativePathLength(targetPath),
                    this.decompilationPreferences.DecompileDangerousResources);
			filePathsService.ExceptionThrown += OnExceptionThrown;

			this.resourcesToPathsMap = this.filePathsService.GetResourcesToFilePathsMap();
			this.xamlResourcesToPathsMap = this.filePathsService.GetXamlResourcesToFilePathsMap();
			this.modulesToProjectsFilePathsMap = this.filePathsService.GetModulesToProjectsFilePathsMap();

			this.assemblyInfo = this.assemblyInfoService.GetAssemblyInfo(this.assembly, this.frameworkResolver);
			this.projectNotifier = projectNotifier;
			this.modulesProjectsGuids = new Dictionary<ModuleDefinition, Guid>();
		}

		public BaseProjectBuilder(string assemblyPath, string targetPath, ILanguage language,
			IFrameworkResolver frameworkResolver, IDecompilationPreferences preferences, IFileGenerationNotifier notifier,
			IAssemblyInfoService assemblyInfoService, VisualStudioVersion visualStudioVersion = VisualStudioVersion.VS2010,
			ProjectGenerationSettings projectGenerationSettings = null, IProjectGenerationNotifier projectNotifier = null)
		{
			this.assemblyPath = assemblyPath;
			this.TargetPath = targetPath;
			this.targetDir = Path.GetDirectoryName(targetPath);
			this.language = language;

			this.frameworkResolver = frameworkResolver;
			this.assemblyInfoService = assemblyInfoService;
			this.decompilationPreferences = preferences;
			this.visualStudioVersion = visualStudioVersion;
			this.projectGenerationSettings = projectGenerationSettings;

			this.currentAssemblyResolver = new WeakAssemblyResolver(GlobalAssemblyResolver.CurrentAssemblyPathCache);

			var readerParameters = new ReaderParameters(currentAssemblyResolver);
			assembly = currentAssemblyResolver.LoadAssemblyDefinition(assemblyPath, readerParameters, loadPdb: true);

			this.namespaceHierarchyTree = assembly.BuildNamespaceHierarchyTree();

			filePathsService =
				new DefaultFilePathsService(
					this.assembly,
					this.assemblyPath,
					Path.GetFileName(this.TargetPath),
					this.UserDefinedTypes,
					this.Resources,
					this.namespaceHierarchyTree,
					this.language,
					Utilities.GetMaxRelativePathLength(targetPath),
                    this.decompilationPreferences.DecompileDangerousResources);
			filePathsService.ExceptionThrown += OnExceptionThrown;

			this.modulesToProjectsFilePathsMap = this.filePathsService.GetModulesToProjectsFilePathsMap();
			this.fileGeneratedNotifier = notifier;

			this.resourcesToPathsMap = this.filePathsService.GetResourcesToFilePathsMap();
			this.xamlResourcesToPathsMap = this.filePathsService.GetXamlResourcesToFilePathsMap();

			this.assemblyInfo = this.assemblyInfoService.GetAssemblyInfo(this.assembly, this.frameworkResolver);
			this.projectNotifier = projectNotifier;
			this.modulesProjectsGuids = new Dictionary<ModuleDefinition, Guid>();
		}

		public string TargetPath { get; protected set; }

		public virtual uint NumberOfFilesToGenerate
		{
			get
			{
				if (this.projectGenerationSettings != null && !this.projectGenerationSettings.JustDecompileSupportedProjectType)
				{
					// 1 for AssemblyInfo.cs
					// + 1 proj file for the main module containing the error
					// + 1 for solution file
					return (uint)(this.NumberOfTypesInAssembly + this.ResourcesCount + 1 + 1 + 1);
				}
				else
				{
					// 1 for AssemblyInfo.cs
					// + proj file for every module
					// + 1 for solution file
					return (uint)(this.NumberOfTypesInAssembly + this.ResourcesCount + 1 + this.assembly.Modules.Count + 1);
				}
			}
		}

		public int NumberOfTypesInAssembly
		{
			get
			{
				int totalTypesCount = 0;

				foreach (Mono.Collections.Generic.Collection<TypeDefinition> moduleTypes in this.UserDefinedTypes.Values)
				{
					totalTypesCount += moduleTypes.Count;
				}

				return totalTypesCount;
			}
		}

		public int ResourcesCount
		{
			get
			{
				return Utilities.GetResourcesCount(Resources, this.decompilationPreferences.DecompileDangerousResources);
			}
		}

		public Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<TypeDefinition>> UserDefinedTypes
		{
			get
			{
				return userDefinedTypes ?? (userDefinedTypes = Utilities.GetUserDefinedTypes(assembly, this.decompilationPreferences.DecompileDangerousResources));
			}
		}

		public Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<Resource>> Resources
		{
			get
			{
				return resources ?? (resources = Utilities.GetResources(assembly));
			}
		}

#if !NET35
		public abstract void BuildProjectCancellable(CancellationToken cancellationToken);
#endif
		public abstract void BuildProject();

		public abstract void BuildProjectComCancellable(Func<bool> shouldCancel);

		protected virtual bool WriteMainModuleProjectFile(ModuleDefinition module)
		{
			//FileStream projectFile = new FileStream(Path.GetDirectoryName(this.TestName) + Path.DirectorySeparatorChar + assembly.Name.Name + language.VSProjectFileExtension, FileMode.OpenOrCreate);
			FileStream projectFile = null;
			if (this.TargetPath.EndsWith(language.VSProjectFileExtension))
			{
				projectFile = new FileStream(this.TargetPath, FileMode.OpenOrCreate);
			}
			else
			{
				projectFile = new FileStream(this.TargetPath + language.VSProjectFileExtension, FileMode.OpenOrCreate);
			}
			try
			{
				this.projectFileManager.ConstructProject(module, (m) => this.CreateProjectReferences(m));

				this.projectFileManager.SerializeProject(projectFile);
			}
			catch (Exception e)
			{
				StreamWriter fileWriter = new StreamWriter(projectFile);
				string exceptionWithStackTrance = string.Format("{0}{1}{2}", e.Message, Environment.NewLine, e.StackTrace);

				fileWriter.Write(exceptionWithStackTrance);
				throw;
			}
			finally
			{
				projectFile.Flush();
				projectFile.Close();
				projectFile.Dispose();
			}

			return true;
		}

		protected virtual void WriteUserDefinedTypes(ModuleDefinition module, Func<bool> shouldCancel)
		{
			Mono.Collections.Generic.Collection<TypeDefinition> moduleTypes;

			if (!UserDefinedTypes.TryGetValue(module, out moduleTypes))
			{
				throw new Exception("Module types not found.");
			}

			ProjectItemWriterFactory writerFactory =
				new ProjectItemWriterFactory(this.assembly, moduleTypes, this.projectFileManager, this.filePathsService, this.language.VSCodeFileExtension, this.targetDir);

#if !NET35
			CancellationTokenSource cts = new CancellationTokenSource();

			ParallelOptions parallelOptions = new ParallelOptions() { CancellationToken = cts.Token, MaxDegreeOfParallelism = Environment.ProcessorCount };

			try
			{
				Parallel.ForEach(
					moduleTypes,
					parallelOptions,
					(TypeDefinition type) =>
					{
						try
						{
							if (shouldCancel())
							{
								cts.Cancel();
								return;
							}

							IProjectItemFileWriter itemWriter = writerFactory.GetProjectItemWriter(type);

							bool shouldBeXamlPartial = Utilities.ShouldBePartial(type) || this.projectFileManager.XamlFullNameToRelativePathMap.ContainsKey(type.FullName);

							if (shouldBeXamlPartial)
							{
								string typeName = type.FullName;
								if (this.xamlGeneratedFields.ContainsKey(typeName))
								{
									this.xamlGeneratedFields[typeName].Add("_contentLoaded"); // Always present, not bound to any XAML element
								}
								else
								{
									this.xamlGeneratedFields.Add(typeName, new HashSet<string>(new string[1] { "_contentLoaded" })); // Always present, not bound to any XAML element
								}
							}

							List<WritingInfo> writingInfos;
							string theCodeString;
							bool exceptionsWhileDecompiling = WriteTypeToFile(type, itemWriter, this.xamlGeneratedFields, shouldBeXamlPartial,
																			language, out writingInfos, out theCodeString);
							bool exceptionsWhileWriting = HasExceptionsWhileWriting(writingInfos);

							lock (writeTypesLock)
							{
								itemWriter.GenerateProjectItems();

								Dictionary<MemberIdentifier, CodeSpan> mapping = null;
								if (this.fileGeneratedNotifier != null)
								{
									/// Create the mapping only when it's needed for the internal API.
									mapping = GenerateMemberMapping(this.assemblyPath, theCodeString, writingInfos);
								}

								IUniqueMemberIdentifier uniqueMemberIdentifier = new UniqueMemberIdentifier(module.FilePath, type.MetadataToken.ToInt32());
								IFileGeneratedInfo args = new TypeGeneratedInfo(itemWriter.FullSourceFilePath, exceptionsWhileDecompiling, exceptionsWhileWriting, uniqueMemberIdentifier, mapping);
								this.OnProjectFileCreated(args);
							}
						}
						catch (Exception ex)
						{
							this.OnTypeWritingFailure(type.FullName, ex);

							if (this.projectNotifier != null)
							{
								this.projectNotifier.OnTypeWritingFailure(type.FullName, ex);
							}
						}
					});
			}
			catch (OperationCanceledException e)
			{
			}
#else
			for (int typeIndex = 0; typeIndex < moduleTypes.Count; typeIndex++)
			{
				if (shouldCancel())
				{
					return;
				}
				TypeDefinition type = moduleTypes[typeIndex];
				try
				{
					IProjectItemFileWriter itemWriter = writerFactory.GetProjectItemWriter(type);
					bool shouldBeXamlPartial = Utilities.ShouldBePartial(type) || this.projectFileManager.XamlFullNameToRelativePathMap.ContainsKey(type.FullName);
					if (shouldBeXamlPartial)
					{
						string typeName = type.FullName;
						if (xamlGeneratedFields.ContainsKey(typeName))
						{
							xamlGeneratedFields[typeName].Add("_contentLoaded");//Always present, not bound to any XAML element
						}
						else
						{
							xamlGeneratedFields.Add(typeName, new HashSet<string>(new string[1] { "_contentLoaded" }));//Always present, not bound to any XAML element
						}
					}
					List<WritingInfo> writingInfos;
					string typeCode;
					bool exceptionsWhileDecompiling = WriteTypeToFile(type, itemWriter, xamlGeneratedFields, shouldBeXamlPartial, 
																		language, out writingInfos, out typeCode);
					bool exceptionsWhileWriting = HasExceptionsWhileWriting(writingInfos);
					itemWriter.GenerateProjectItems();
					Dictionary<MemberIdentifier, CodeSpan> mapping = GenerateMemberMapping(this.assemblyPath, typeCode, writingInfos);

					IUniqueMemberIdentifier uniqueMemberIdentifier = new UniqueMemberIdentifier(module.FilePath, type.MetadataToken.ToInt32());
					IFileGeneratedInfo args = new TypeGeneratedInfo(itemWriter.FullSourceFilePath, exceptionsWhileDecompiling, exceptionsWhileWriting, uniqueMemberIdentifier, mapping);
                    this.OnProjectFileCreated(args);
				}
				catch (Exception ex)
				{
					if (TypeWritingFailure != null)
					{
						TypeWritingFailure(this, type.FullName, ex);
					}
				}
			 }
#endif
		}

		protected virtual void CreateProjectReferences(ModuleDefinition module)
		{
			ICollection<AssemblyNameReference> dependingOnAssemblies = GetAssembliesDependingOn(module);
			this.projectFileManager.CreateReferencesProjectItem(dependingOnAssemblies.Count);

			string assemblyName = module.IsMain ? module.Assembly.Name.Name : Utilities.GetNetmoduleName(module);
			string copiedReferencesSubfolder = assemblyName + "References";
			string referencesPath = TargetPath.Remove(TargetPath.LastIndexOf(Path.DirectorySeparatorChar)) + Path.DirectorySeparatorChar + copiedReferencesSubfolder;

			ICollection<AssemblyNameReference> filteredDependingOnAssemblies = FilterDependingOnAssemblies(dependingOnAssemblies);
			int assemblyReferenceIndex = 0;
			SpecialTypeAssembly special = module.IsReferenceAssembly() ? SpecialTypeAssembly.Reference : SpecialTypeAssembly.None;

			foreach (AssemblyNameReference reference in filteredDependingOnAssemblies)
			{
				this.CreateProjectReferenceInternal(module, reference, ref assemblyReferenceIndex, special, referencesPath, copiedReferencesSubfolder);

				assemblyReferenceIndex++;
			}
		}

		protected virtual void CreateProjectReferenceInternal(ModuleDefinition module, AssemblyNameReference reference, ref int assemblyReferenceIndex,
			SpecialTypeAssembly special, string referencesPath, string copiedReferencesSubfolder)
		{
			AssemblyName assemblyName = new AssemblyName(reference.Name,
																reference.FullName,
																reference.Version,
																reference.PublicKeyToken);
			AssemblyStrongNameExtended assemblyKey = new AssemblyStrongNameExtended(assemblyName.FullName, module.Architecture, special);

			string currentReferenceInitialLocation = this.currentAssemblyResolver.FindAssemblyPath(assemblyName, null, assemblyKey);
			AssemblyDefinition referencedAssembly = this.currentAssemblyResolver.Resolve(reference, "", assembly.MainModule.GetModuleArchitecture(), special);
#if NET35
				if (!currentReferenceInitialLocation.IsNullOrWhiteSpace())
#else
			if (!string.IsNullOrWhiteSpace(currentReferenceInitialLocation))
#endif
			{
				if (this.IsInReferenceAssemblies(referencedAssembly))
				{
					//TODO: Consider doing additional check, to see if the assembly is resolved because it was pointed by the used/already in the tree
					//		In this case, it might be better to copy it.
					this.projectFileManager.AddReferenceProjectItem(assemblyReferenceIndex, reference.Name);
				}
				else // Copy the referenced assembly
				{
					if (!Directory.Exists(referencesPath))
					{
						Directory.CreateDirectory(referencesPath);
					}

					string currentReferenceFileName = Path.GetFileName(currentReferenceInitialLocation);
					string currentReferenceFinalLocation = Path.Combine(referencesPath, currentReferenceFileName);
					File.Copy(currentReferenceInitialLocation, currentReferenceFinalLocation, true);

					// set to normal for testing purposes- to allow the test project to delete the coppied file between test runs
					File.SetAttributes(currentReferenceFinalLocation, FileAttributes.Normal);

					string relativePath = Path.Combine(".", copiedReferencesSubfolder);
					relativePath = Path.Combine(relativePath, currentReferenceFileName);

					this.projectFileManager.AddReferenceProjectItem(
						assemblyReferenceIndex,
						Path.GetFileNameWithoutExtension(currentReferenceFinalLocation),
						relativePath);
				}
			}
			else
			{
				this.projectFileManager.AddReferenceProjectItem(assemblyReferenceIndex, reference.FullName);
			}
		}

		protected virtual ICollection<AssemblyNameReference> FilterDependingOnAssemblies(ICollection<AssemblyNameReference> dependingOnAssemblies)
		{
			ICollection<AssemblyNameReference> result = new List<AssemblyNameReference>();
			foreach (AssemblyNameReference reference in dependingOnAssemblies)
			{
				if (reference.Name == "mscorlib" ||
					reference.Name == "Windows" && reference.Version.Equals(new Version(255, 255, 255, 255)))
				{
					continue;
				}

				result.Add(reference);
			}

			return result;
		}

		protected void CreateResources(ModuleDefinition module)
		{
			string targetDir = Path.GetDirectoryName(this.TargetPath);
			foreach (Resource resource in module.Resources)
			{
				try
				{
                    if (!this.decompilationPreferences.DecompileDangerousResources &&
                        DangerousResourceIdentifier.IsDangerousResource(resource))
                    {
                        continue;
                    }

					if (resource.ResourceType != ResourceType.Embedded)
					{
						continue;
					}

					EmbeddedResource embeddedResource = (EmbeddedResource)resource;
					IFileGeneratedInfo args;
					if (resource.Name.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
					{
                        if (!embeddedResource.Name.EndsWith(".g.resources", StringComparison.OrdinalIgnoreCase))
                        {
                            string resourceName = embeddedResource.Name.Substring(0, embeddedResource.Name.Length - 10); //".resources".Length == 10
                            string relativeResourcePath = resourcesToPathsMap[resource];
                            string fullResourcePath = Path.Combine(targetDir, relativeResourcePath);

                            if (TryCreateResXFile(embeddedResource, fullResourcePath))
                            {
                                this.projectFileManager.ResourceDesignerMap.Add(resourceName, relativeResourcePath);
                                args = new FileGeneratedInfo(fullResourcePath, false);
                                OnProjectFileCreated(args);
                            }
                        }
                        else
                        {
                            ProcessXamlResources(embeddedResource, module);
                        }
					}
					else
					{
						string resourceLegalName = resourcesToPathsMap[resource];
						string resourceFullPath = Path.Combine(targetDir, resourceLegalName);
						using (FileStream fileStream = new FileStream(resourceFullPath, FileMode.Create, FileAccess.Write))
						{
							embeddedResource.GetResourceStream().CopyTo(fileStream);
						}

						this.projectFileManager.AddResourceToOtherEmbeddedResources(resourceLegalName);

						args = new FileGeneratedInfo(resourceFullPath, false);
						OnProjectFileCreated(args);
					}
				}
				catch (Exception ex)
				{
					if (ResourceWritingFailure != null)
					{
						ResourceWritingFailure(this, resource.Name, ex);
					}

					if (this.projectNotifier != null)
					{
						this.projectNotifier.OnResourceWritingFailure(resource.Name, ex);
					}
				}
			}
		}

		protected ICollection<AssemblyNameReference> GetAssembliesDependingOn(ModuleDefinition module)
		{
			ICollection<AssemblyNameReference> result;

			ICollection<TypeReference> expadendTypeDependanceList = GetExpandedTypeDependanceList(module);
			result = Telerik.JustDecompiler.Decompiler.Utilities.GetAssembliesDependingOn(module, expadendTypeDependanceList);

			foreach (AssemblyNameReference assemblyReference in module.AssemblyReferences)
			{
				if (!result.Contains(assemblyReference))
				{
					result.Add(assemblyReference);
				}
			}

			return result;
		}

		protected ICollection<TypeReference> GetExpandedTypeDependanceList(ModuleDefinition module)
		{
			Mono.Collections.Generic.Collection<TypeDefinition> moduleTypes;
			if (!UserDefinedTypes.TryGetValue(module, out moduleTypes))
			{
				throw new Exception("Module types not found.");
			}

			HashSet<TypeReference> firstLevelDependanceTypes = new HashSet<TypeReference>();
			foreach (TypeReference type in moduleTypes)
			{
				if (!firstLevelDependanceTypes.Contains(type))
				{
					firstLevelDependanceTypes.Add(type);
				}
			}

			foreach (TypeReference type in module.GetTypeReferences())
			{
				if (!firstLevelDependanceTypes.Contains(type))
				{
					firstLevelDependanceTypes.Add(type);
				}
			}

			return Telerik.JustDecompiler.Decompiler.Utilities.GetExpandedTypeDependanceList(firstLevelDependanceTypes);
		}

		private bool TryCreateResXFile(EmbeddedResource embeddedResource, string resourceFilePath)
		{
			List<System.Collections.DictionaryEntry> resourceEntries = new List<System.Collections.DictionaryEntry>();

			using (ResourceReader resourceReader = new ResourceReader(embeddedResource.GetResourceStream()))
			{
				IDictionaryEnumerator enumerator = resourceReader.GetEnumerator();

				while (enumerator.MoveNext())
				{
					try
					{
						resourceEntries.Add(enumerator.Entry);
					}
					catch (Exception ex)
					{
						this.OnResourceWritingFailure(embeddedResource.Name, ex);

						if (this.projectNotifier != null)
						{
							this.projectNotifier.OnResourceWritingFailure(embeddedResource.Name, ex);
						}
					}
				}
			}

			string dirPath = Path.GetDirectoryName(resourceFilePath);
			if (!Directory.Exists(dirPath))
			{
				Directory.CreateDirectory(dirPath);
			}

#if !NET35
			using (ResXResourceWriter resXWriter = new ResXResourceWriter(resourceFilePath, ResourceTypeNameConverter))
#else
            using (ResXResourceWriter resXWriter = new ResXResourceWriter(resourceFilePath))
#endif
			{
				foreach (System.Collections.DictionaryEntry resourceEntry in resourceEntries)
				{
					resXWriter.AddResource((string)resourceEntry.Key, resourceEntry.Value);
				}
			}

			return true;
		}

		private void ProcessXamlResources(EmbeddedResource resource, ModuleDefinition module)
		{
			try
			{
				string targetDir = Path.GetDirectoryName(TargetPath);
				using (ResourceReader resourceReader = new ResourceReader(resource.GetResourceStream()))
				{
					foreach (System.Collections.DictionaryEntry resourceEntry in resourceReader)
					{
						string xamlResourceKey = Utilities.GetXamlResourceKey(resourceEntry, module);

						bool isBamlResource = ((string)resourceEntry.Key).EndsWith(".baml", StringComparison.OrdinalIgnoreCase);

						string xamlResourceRelativePath = xamlResourcesToPathsMap[xamlResourceKey];
						string fullPath = Path.Combine(targetDir, xamlResourceRelativePath);

						string fullClassName = TryWriteBamlResource(fullPath, isBamlResource, resourceEntry.Value as UnmanagedMemoryStream);
						if (fullClassName != null)
						{
							this.projectFileManager.XamlFullNameToRelativePathMap.Add(fullClassName, xamlResourceRelativePath);
						}
						else
						{
							this.projectFileManager.AddResourceToOtherXamlResources(xamlResourceRelativePath);
						}

						IFileGeneratedInfo args = new FileGeneratedInfo(fullPath, false);
						OnProjectFileCreated(args);
					}
				}
			}
			catch (Exception ex)
			{
				if (ResourceWritingFailure != null)
				{
					ResourceWritingFailure(this, resource.Name, ex);
				}

				if (this.projectNotifier != null)
				{
					this.projectNotifier.OnResourceWritingFailure(resource.Name, ex);
				}
			}
		}

		private string TryWriteBamlResource(string resourcePath, bool isBamlResource, UnmanagedMemoryStream unmanagedStream)
		{
			if (unmanagedStream == null)
			{
				return null;
			}

			string resourceDir = Path.GetDirectoryName(resourcePath);
			if (!Directory.Exists(resourceDir))
			{
				Directory.CreateDirectory(resourceDir);
			}

			Stream sourceStream;
			string fullClassName = null;
#if ENGINEONLYBUILD || JUSTASSEMBLY
            sourceStream = unmanagedStream;
#else
			XDocument xamlDoc = null;

			bool exceptionOccurred = false;
			//TODO: refactor
			if (isBamlResource)
			{
				sourceStream = new MemoryStream();
				try
				{
					unmanagedStream.Seek(0, SeekOrigin.Begin);
					xamlDoc = BamlToXamlConverter.DecompileToDocument(unmanagedStream, currentAssemblyResolver, assemblyPath);
#if !NET35
					xamlDoc.Save(sourceStream);
#else
                    xamlDoc.Save(new StreamWriter(sourceStream));
#endif
				}
				catch (Exception ex)
				{
					exceptionOccurred = true;
					unmanagedStream.Seek(0, SeekOrigin.Begin);
					sourceStream = unmanagedStream;

					OnExceptionThrown(ex);
				}
			}
			else
			{
				sourceStream = unmanagedStream;
			}

			if (isBamlResource && !exceptionOccurred)
			{
				fullClassName = Utilities.GetXamlTypeFullName(xamlDoc);
				if (fullClassName != null)
				{
					xamlGeneratedFields.Add(fullClassName, Utilities.GetXamlGeneratedFields(xamlDoc));
				}
			}
#endif

			using (FileStream fileStream = new FileStream(resourcePath, FileMode.Create, FileAccess.Write))
			{
				using (sourceStream)
				{
					sourceStream.Seek(0, SeekOrigin.Begin);
					sourceStream.CopyTo(fileStream);
				}
			}

			return fullClassName;
		}

		private string ResourceTypeNameConverter(Type type)
		{
			return type.AssemblyQualifiedName;
		}

		protected bool IsInReferenceAssemblies(AssemblyDefinition referencedAssembly)
		{
			//Search C:\Program Files\Reference Assemblies 
			//and 
			//       C:\Program Files (x86)\Reference Assemblies
			//for the dll. If it is found there, then just a simple name will be enough for VS to locate it
			// Otherwise, the file should be copied it to local project folder and the reference should be pointing there.
			// TODO: Implement this method
			/// Try x86

#if !NET35
			string programFilesX64 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles, Environment.SpecialFolderOption.None);
			string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86, Environment.SpecialFolderOption.None);
#else
            string programFilesX64 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			string programFilesX86 =  programFilesX64 + " (x86)";
#endif

			//NOTE: programFilesX64 and programFilexX86 are expected to be equal on 32-bit machine

			string x86ReferenceAssemblies = Path.Combine(programFilesX86, "Reference Assemblies");
			string x64ReferenceAssemblies = Path.Combine(programFilesX64, "Reference Assemblies");

			return this.ContainsReference(x86ReferenceAssemblies, x64ReferenceAssemblies, referencedAssembly);
		}

		protected bool IsInDotNetAssemblies(AssemblyDefinition referencedAssembly)
		{
#if !NET35
			string programFilesX64 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles, Environment.SpecialFolderOption.None);
			string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86, Environment.SpecialFolderOption.None);
#else
            string programFilesX64 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			string programFilesX86 =  programFilesX64 + " (x86)";
#endif

			//NOTE: programFilesX64 and programFilexX86 are expected to be equal on 32-bit machine

			string x86DotNetAssemblies = Path.Combine(programFilesX86, "dotnet");
			string x64DotNetAssemblies = Path.Combine(programFilesX64, "dotnet");

			return this.ContainsReference(x86DotNetAssemblies, x64DotNetAssemblies, referencedAssembly);
		}

		private bool ContainsReference(string x86Assemblies, string x64Assemblies, AssemblyDefinition referencedAssembly)
		{
			if (!Directory.Exists(x86Assemblies))
			{
				if (x86Assemblies == x64Assemblies)
				{
					return false;
				}
			}
			else
			{
				bool returnValue = ContainsReferencedAssembly(x86Assemblies, referencedAssembly);
				if (returnValue)
				{
					return returnValue;
				}
			}

			if (x86Assemblies != x64Assemblies)
			{
				if (!Directory.Exists(x64Assemblies))
				{
					return false;
				}
				else
				{
					return ContainsReferencedAssembly(x64Assemblies, referencedAssembly);
				}
			}
			return false;
		}

		private bool ContainsReferencedAssembly(string directory, AssemblyDefinition referencedAssembly)
		{
			string assemblyName = Path.GetFileName(referencedAssembly.MainModule.FilePath);
#if !NET35
			IEnumerable<string> foundFiles = Directory.EnumerateFiles(directory, assemblyName, SearchOption.AllDirectories);
#else
            string[] foundFiles = Directory.GetFiles(directory, assemblyName, SearchOption.AllDirectories);
#endif
			foreach (string file in foundFiles)
			{
				///Chech if all other assemblyName attributes match
				//AssemblyDefinition a = AssemblyDefinition.ReadAssembly(file);
				AssemblyDefinition a = currentAssemblyResolver.GetAssemblyDefinition(file);
				if (a.FullName == referencedAssembly.FullName)
				{
					return true;
				}
			}
			return false;
		}

		public bool WriteTypeToFile(TypeDefinition type, IProjectItemFileWriter itemWriter, Dictionary<string, ICollection<string>> membersToSkip, bool shouldBePartial,
			ILanguage language, out List<WritingInfo> writingInfos, out string theCodeString)
		{
			theCodeString = string.Empty;
			writingInfos = null;
			StringWriter theWriter = new StringWriter();

			bool showCompilerGeneratedMembers = Utilities.IsVbInternalTypeWithoutRootNamespace(type) ||
												Utilities.IsVbInternalTypeWithRootNamespace(type);

			IFormatter formatter = GetFormatter(theWriter);
			IWriterSettings settings = new WriterSettings(writeExceptionsAsComments: true,
														  writeFullyQualifiedNames: decompilationPreferences.WriteFullNames,
														  writeDocumentation: decompilationPreferences.WriteDocumentation,
														  showCompilerGeneratedMembers: showCompilerGeneratedMembers,
														  writeLargeNumbersInHex: decompilationPreferences.WriteLargeNumbersInHex);
			ILanguageWriter writer = language.GetWriter(formatter, this.exceptionFormater, settings);

			IWriterContextService writerContextService = this.GetWriterContextService();

			writer.ExceptionThrown += OnExceptionThrown;
			writerContextService.ExceptionThrown += OnExceptionThrown;

			bool exceptionOccurred = false;

			try
			{
				if (!(writer is INamespaceLanguageWriter))
				{
					writingInfos = writer.Write(type, writerContextService);
				}
				else
				{

					if (shouldBePartial)
					{
						writingInfos = (writer as INamespaceLanguageWriter).WritePartialTypeAndNamespaces(type, writerContextService, membersToSkip);
					}
					else
					{
						writingInfos = (writer as INamespaceLanguageWriter).WriteTypeAndNamespaces(type, writerContextService);
					}
				}

				this.RecordGeneratedFileData(type, itemWriter.FullSourceFilePath, theWriter, formatter, writerContextService, writingInfos);

				MemoryStream sourceFileStream = new MemoryStream(Encoding.UTF8.GetBytes(theWriter.ToString()));
				itemWriter.CreateProjectSourceFile(sourceFileStream);
				sourceFileStream.Close();
				theWriter.Close();
			}
			catch (Exception e)
			{
				exceptionOccurred = true;

				string[] exceptionMessageLines = exceptionFormater.Format(e, type.FullName, itemWriter.FullSourceFilePath);
				string exceptionMessage = string.Join(Environment.NewLine, exceptionMessageLines);
				string commentedExceptionMessage = language.CommentLines(exceptionMessage);
				itemWriter.CreateProjectSourceFile(new MemoryStream(Encoding.UTF8.GetBytes(commentedExceptionMessage)));

				OnExceptionThrown(this, e);
			}

			theCodeString = theWriter.ToString();

			writer.ExceptionThrown -= OnExceptionThrown;
			writerContextService.ExceptionThrown -= OnExceptionThrown;

			return exceptionOccurred || writerContextService.ExceptionsWhileDecompiling.Any();
		}

		private Dictionary<MemberIdentifier, CodeSpan> GenerateMemberMapping(string assemblyFilePath, string theCodeString, List<WritingInfo> writingInfos)
		{
			StringBuilder sb = new StringBuilder();
			StringWriter strWriter = new StringWriter(sb);
			strWriter.Write(theCodeString);
			return ExternallyVisibleDecompilationUtilities.GenerateMemberMapping(assemblyFilePath, strWriter, writingInfos);
		}

		protected virtual IFormatter GetFormatter(StringWriter writer)
		{
			return new PlainTextFormatter(writer);
		}

		protected virtual TypeCollisionWriterContextService GetWriterContextService()
		{
			return new TypeCollisionWriterContextService(new ProjectGenerationDecompilationCacheService(), decompilationPreferences.RenameInvalidMembers);
		}

		private bool HasExceptionsWhileWriting(List<WritingInfo> writingInfos)
		{
			foreach (WritingInfo info in writingInfos)
			{
				if (info.ExceptionsWhileWriting.Count > 0)
				{
					return true;
				}
			}
			return false;
		}

		protected virtual void ClearCaches()
		{
			ProjectGenerationDecompilationCacheService.ClearAssemblyContextsCache();
			this.currentAssemblyResolver.ClearCache();
			this.currentAssemblyResolver.ClearAssemblyFailedResolverCache();
		}

		protected virtual bool WriteSolutionFile()
		{
			SolutionWriter solutionWriter =
				new SolutionWriter(this.assembly, this.targetDir, this.filePathsService.GetSolutionRelativePath(), this.modulesToProjectsFilePathsMap, this.modulesProjectsGuids, this.visualStudioVersion, this.language);
			solutionWriter.WriteSolutionFile();

			return true;
		}

		protected void OnProjectFileCreated(IFileGeneratedInfo projectFileGeneratedCallbackArgs)
		{
			if (ProjectFileCreated != null)
			{
				ProjectFileCreated(this, new ProjectFileCreated(projectFileGeneratedCallbackArgs.FullPath, projectFileGeneratedCallbackArgs.HasErrors));
			}
			if (fileGeneratedNotifier != null)
			{
				fileGeneratedNotifier.OnProjectFileGenerated(projectFileGeneratedCallbackArgs);
			}
		}

		protected void InformProjectFileCreated(ModuleDefinition module, string extension, bool hasErrors)
		{
			string projFileName;

			if (!modulesToProjectsFilePathsMap.TryGetValue(module, out projFileName))
			{
				throw new Exception("Module project file path not found in modules projects filepaths map.");
			}

			if (!projFileName.EndsWith(extension))
			{
				projFileName += extension;
			}

			string fullFilePath = Path.Combine(this.targetDir, projFileName);
			IFileGeneratedInfo csprojArgs = new FileGeneratedInfo(fullFilePath, hasErrors);
			this.OnProjectFileCreated(csprojArgs);
		}

		protected virtual void RecordGeneratedFileData(TypeDefinition type, string sourceFilePath, StringWriter theWriter, IFormatter formatter, IWriterContextService writerContextService, List<WritingInfo> writingInfos)
		{
		}

		protected void OnResourceWritingFailure(string embeddedResourceName, Exception ex)
		{
			if (ResourceWritingFailure != null)
			{
				ResourceWritingFailure(this, embeddedResourceName, ex);
			}

			if (this.projectNotifier != null)
			{
				this.projectNotifier.OnResourceWritingFailure(embeddedResourceName, ex);
			}
		}

		protected void OnProjectGenerationFailure(Exception ex)
		{
			if (ProjectGenerationFailure != null)
			{
				ProjectGenerationFailure(this, ex);
			}

			if (this.projectNotifier != null)
			{
				this.projectNotifier.OnProjectGenerationFailure(ex);
			}
		}

		protected void OnTypeWritingFailure(string typeDefinitionFullName, Exception ex)
		{
			if (TypeWritingFailure != null)
			{
				TypeWritingFailure(this, typeDefinitionFullName, ex);
			}

			if (this.projectNotifier != null)
			{
				this.projectNotifier.OnTypeWritingFailure(typeDefinitionFullName, ex);
			}
		}

		protected void OnProjectGenerationFinished()
		{
			EventHandler handler = ProjectGenerationFinished;
			if (handler != null)
			{
				handler(this, new EventArgs());
			}

			if (this.projectNotifier != null)
			{
				this.projectNotifier.OnProjectGenerationFinished();
			}
		}
	}
}
