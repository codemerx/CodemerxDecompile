using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Telerik.JustDecompiler.Languages;
using System.IO;
using System.Globalization;
using System.Text;
using System.Resources;
using System.Xml.Linq;
#if !ENGINEONLYBUILD && !JUSTASSEMBLY
using Telerik.Baml;
#endif
using Telerik.JustDecompiler.Common.NamespaceHierarchy;
using Telerik.JustDecompiler;
using Telerik.JustDecompiler.External;
using JustDecompile.EngineInfrastructure;

namespace JustDecompile.Tools.MSBuildProjectBuilder.FilePathsServices
{
	class DefaultFilePathsService : ExceptionThrownNotifier, IFilePathsService
	{
		private readonly string assemblyPath;
		private readonly AssemblyDefinition assembly;
		private readonly string mainModuleProjectFileName;
		private readonly IAssemblyResolver assemblyResolver;
		private readonly Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<TypeDefinition>> userDefinedTypes;
		private readonly int maxRelativePathLength;
		private readonly List<KeyValuePair<string, List<TypeDefinition>>> typesData;
		private readonly Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<Resource>> resources;
		private readonly List<XamlResource> xamlResources;
		private readonly ILanguage language;
		private readonly string sourceExtension;
		private readonly NamespaceHierarchyTree namespaceHierarchyTree;
        private readonly bool decompileDangerousResources;

		public const string XamlExtension = ".xaml";
		public const string XamlResourcesShortNameStartSymbol = ")";
		public const string ResourcesShortNameStartSymbol = "(";
		public const string AssemblyInfoShortFileName = "1AI";
		public const string ResourceDesignerShortNameExtension = ".D";
		private const string ResourceDesignerNameExtension = ".Designer";

		public DefaultFilePathsService(AssemblyDefinition assembly, string assemblyPath, string mainModuleProjectFileName, Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<TypeDefinition>> userDefinedTypes,
			Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<Resource>> resources, NamespaceHierarchyTree namespaceHierarchyTree, ILanguage language, int maxRelativePathLength, bool decompileDangerousResources)
		{
			this.assembly = assembly;
			this.assemblyPath = assemblyPath;
			this.mainModuleProjectFileName = mainModuleProjectFileName;
			this.userDefinedTypes = userDefinedTypes;
			this.namespaceHierarchyTree = namespaceHierarchyTree;
			this.language = language;
			this.maxRelativePathLength = maxRelativePathLength;
            this.decompileDangerousResources = decompileDangerousResources;

			this.assemblyResolver = new WeakAssemblyResolver(GlobalAssemblyResolver.CurrentAssemblyPathCache);

			this.typesData = GetFilePathsServiceData();
			this.resources = resources;
            this.xamlResources = GetXamlResources(resources);
            this.sourceExtension = language.VSCodeFileExtension;
		}

		public string GetSolutionRelativePath()
		{
			return Path.ChangeExtension(this.mainModuleProjectFileName, ".sln");
		}

		public Dictionary<ModuleDefinition, string> GetModulesToProjectsFilePathsMap()
		{
			int longModuleFilePathsCount = 0;
			Dictionary<ModuleDefinition, string> result = new Dictionary<ModuleDefinition, string>();

			result.Add(this.assembly.MainModule, this.mainModuleProjectFileName);

			foreach (ModuleDefinition module in this.assembly.Modules)
			{
				if (module.Kind == ModuleKind.NetModule)
				{
					string moduleName = Utilities.GetNetmoduleName(module);
					string moduleProjectFileName = moduleName + this.language.VSProjectFileExtension;
					if (moduleProjectFileName.Length <= maxRelativePathLength && moduleProjectFileName != this.mainModuleProjectFileName)
					{
						result.Add(module, moduleProjectFileName);
					}
					else
					{
						longModuleFilePathsCount++;
						moduleProjectFileName = longModuleFilePathsCount.ToString() + this.language.VSProjectFileExtension;
						if (moduleProjectFileName == this.mainModuleProjectFileName)
						{
							longModuleFilePathsCount++;
							moduleProjectFileName = longModuleFilePathsCount.ToString() + this.language.VSProjectFileExtension;
						}

						result.Add(module, moduleProjectFileName);
					}
				}
			}

			return result;
		}

		private List<KeyValuePair<string, List<TypeDefinition>>> GetFilePathsServiceData()
		{
			List<KeyValuePair<string, List<TypeDefinition>>> result = new List<KeyValuePair<string, List<TypeDefinition>>>();

			Dictionary<string, List<TypeDefinition>> namespaceTypes = new Dictionary<string, List<TypeDefinition>>();

			foreach (Mono.Collections.Generic.Collection<TypeDefinition> moduleTypes in userDefinedTypes.Values)
			{
				foreach (TypeDefinition type in moduleTypes)
				{
					if (!namespaceTypes.ContainsKey(type.Namespace))
					{
						namespaceTypes.Add(type.Namespace, new List<TypeDefinition>());
					}

					namespaceTypes[type.Namespace].Add(type);
				}
			}

			foreach (KeyValuePair<string, List<TypeDefinition>> pair in namespaceTypes)
			{
				result.Add(pair);
			}

			result.Sort(new FilePathsServiceDataComparer());

			return result;
		}

		private class FilePathsServiceDataComparer : IComparer<KeyValuePair<string, List<TypeDefinition>>>
		{
			public int Compare(KeyValuePair<string, List<TypeDefinition>> x, KeyValuePair<string, List<TypeDefinition>> y)
			{
				return x.Key.CompareTo(y.Key);
			}
		}

		private List<XamlResource> GetXamlResources(Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<Resource>> resources)
		{
			List<XamlResource> result = new List<XamlResource>();

            if (this.decompileDangerousResources)
            {
                foreach (KeyValuePair<ModuleDefinition, Mono.Collections.Generic.Collection<Resource>> moduleResources in resources)
                {
                    foreach (Resource resource in moduleResources.Value)
                    {
                        if (resource.ResourceType != ResourceType.Embedded)
                        {
                            continue;
                        }

                        EmbeddedResource embeddedResource = (EmbeddedResource)resource;
                        if (resource.Name.EndsWith(".g.resources", StringComparison.OrdinalIgnoreCase))
                        {
                            using (ResourceReader resourceReader = new ResourceReader(embeddedResource.GetResourceStream()))
                            {
                                foreach (System.Collections.DictionaryEntry resourceEntry in resourceReader)
                                {
                                    result.Add(new XamlResource(resourceEntry, moduleResources.Key));
                                }
                            }
                        }
                    }
                }
            }
			
			return result;
		}

		public Dictionary<TypeDefinition, string> GetTypesToFilePathsMap()
		{
			Dictionary<TypeDefinition, string> result = new Dictionary<TypeDefinition, string>();
			
			HashSet<string> namespacesToFlatten = new HashSet<string>();
			List<TypeDefinition> flattenedTypes = new List<TypeDefinition>();

			// at first, we flatten all namespaces that contain at least type
			// with potential filepath longer than the length constraint

			foreach (KeyValuePair<string, List<TypeDefinition>> pair in typesData)
			{
				string @namespace = pair.Key;
				List<TypeDefinition> typesInNamespace = pair.Value;

				bool shouldFlattenNamespace = false;
				foreach (TypeDefinition type in typesInNamespace)
				{
					string typePath = GetTypeFilePath(type);
					result.Add(type, typePath);

					if (typePath.Length > maxRelativePathLength)
					{
						shouldFlattenNamespace = true;
					}
				}

				if (pair.Key != "")
				{
					if (shouldFlattenNamespace)
					{
						namespacesToFlatten.Add(@namespace);
						foreach (TypeDefinition type in typesInNamespace)
						{
							flattenedTypes.Add(type);
							string fileName = Path.GetFileName(result[type]);
							result[type] = fileName;
						}
					}
				}

				string namespaceDirectory = Path.GetDirectoryName(result[typesInNamespace[0]]);
				if (pair.Key == "" || (!shouldFlattenNamespace && namespaceDirectory == ""))
				{
					// every type in the "" namespace is flattened by default

					foreach (TypeDefinition type in typesInNamespace)
					{
						flattenedTypes.Add(type);
					}
				}
			}

			// try to fix collisions for every but the empty namespace,
			// since when renaming files in collisions, we might hit the limit,
			// and add new types to the empty namespace directory

			foreach (KeyValuePair<string, List<TypeDefinition>> pair in typesData)
			{
				string @namespace = pair.Key;
				List<TypeDefinition> typesInNamespace = pair.Value;

				if (@namespace != "" && !namespacesToFlatten.Contains(@namespace))
				{
					FixCollisionsInNamespace(@namespace, typesInNamespace, result, namespacesToFlatten, flattenedTypes);
				}
			}

			// now we have fixed collisions and checked lengths in all namespaces, 
			// except for the empty namespace, where we should do collision renaming
			// and then length renaming if necessary

			FixCollisionsInNamespace("", flattenedTypes, result, namespacesToFlatten, flattenedTypes);

			int longPathsCount = 0;

			foreach (TypeDefinition type in flattenedTypes)
			{
				if (result[type].Length > maxRelativePathLength)
				{
					longPathsCount++;

					result[type] = Convert.ToString(longPathsCount) + sourceExtension;
				}
			}

			return result;
		}

		private void FixCollisionsInNamespace(string @namespace, List<TypeDefinition> typesInNamespace, Dictionary<TypeDefinition, string> currentPaths,
			HashSet<string> namespacesToFlatten, List<TypeDefinition> flattenedTypes)
		{
			Dictionary<string, List<TypeDefinition>> typesFilePathCollisionOccurrances = new Dictionary<string, List<TypeDefinition>>();
			HashSet<string> singleFilePathOccurances = new HashSet<string>();

			foreach (TypeDefinition type in typesInNamespace)
			{
				string typeFilePathKey = currentPaths[type].ToLower();

				if (typesFilePathCollisionOccurrances.ContainsKey(typeFilePathKey))
				{
					singleFilePathOccurances.Remove(typeFilePathKey);

					typesFilePathCollisionOccurrances[typeFilePathKey].Add(type);
				}
				else
				{
					singleFilePathOccurances.Add(typeFilePathKey);

					List<TypeDefinition> filePathOcurrancesList = new List<TypeDefinition>();
					filePathOcurrancesList.Add(type);
					typesFilePathCollisionOccurrances.Add(typeFilePathKey, filePathOcurrancesList);
				}
			}

			foreach (KeyValuePair<string, List<TypeDefinition>> typeFilePathCollisionPair in typesFilePathCollisionOccurrances)
			{
				if (typeFilePathCollisionPair.Value.Count > 1)
				{
					int currentFilePathCollisionCount = 0;
					foreach (TypeDefinition type in typeFilePathCollisionPair.Value)
					{
						string currentPath = currentPaths[type];
						string friendlyName = Path.GetFileNameWithoutExtension(currentPath);
						string relativeDirPath = Path.GetDirectoryName(currentPath);

						string pathCandidate = Path.Combine(relativeDirPath, friendlyName + ++currentFilePathCollisionCount + sourceExtension);
						while (singleFilePathOccurances.Contains(pathCandidate))
						{
							currentFilePathCollisionCount++;
							pathCandidate = Path.Combine(relativeDirPath, friendlyName + currentFilePathCollisionCount + sourceExtension);
						}

						if (pathCandidate.Length > maxRelativePathLength && @namespace != "" && relativeDirPath != "")
						{
							namespacesToFlatten.Add(@namespace);
							foreach (TypeDefinition typeInNamespace in typesInNamespace)
							{
								flattenedTypes.Add(typeInNamespace);
							}
							break;
						}

						currentPaths[type] = pathCandidate;
					}
				}
			}
		}

		private string GetTypeFilePath(TypeDefinition type)
		{
			string relativeDirPath = GetRelativeDirPath(type);

			string friendlyName = Utilities.GetLegalFileName(GetMemberNamePerLanguage(language, type));

            //13 == "u003cCLRu003e".Length
            if (this.assembly.Name.IsWindowsRuntime && friendlyName.Length > 13 && friendlyName.StartsWith("u003cCLRu003e"))
            {
                friendlyName = friendlyName.Substring(13);
            }

			return Path.Combine(relativeDirPath, friendlyName + sourceExtension);
		}

		private string GetRelativeDirPath(TypeDefinition type)
		{
			string friendlyNamespace = Utilities.GetLegalFolderName(GetNamespacePerLanguage(language, type.Namespace));

			string[] pathTokens = namespaceHierarchyTree.GetSpecialPathTokens(friendlyNamespace);

#if !NET35
			string relativeDirPath = pathTokens != null ? Path.Combine(pathTokens) : friendlyNamespace;
#else
			string relativeDirPath = pathTokens != null ? NET4Extensions.Combine(pathTokens) : friendlyNamespace;
#endif

			return relativeDirPath;
		}

		private string GetMemberNamePerLanguage(ILanguage language, TypeDefinition type)
		{
			var formatter = new PlainTextFormatter(new StringWriter());

            IWriterSettings settings = new WriterSettings(writeExceptionsAsComments: true,
                                                          renameInvalidMembers: true);
			ILanguageWriter writer = language.GetWriter(formatter, SimpleExceptionFormatter.Instance, settings);
            writer.ExceptionThrown += OnExceptionThrown;
			writer.WriteMemberNavigationName(type);
            writer.ExceptionThrown -= OnExceptionThrown;

            return formatter.ToString();
		}

		private string GetNamespacePerLanguage(ILanguage language, string @namespace)
		{
			var formatter = new PlainTextFormatter(new StringWriter());

            IWriterSettings settings = new WriterSettings(writeExceptionsAsComments: true,
                                                          renameInvalidMembers: true);
            ILanguageWriter writer = language.GetWriter(formatter, SimpleExceptionFormatter.Instance, settings);
            writer.ExceptionThrown += OnExceptionThrown;
            writer.WriteNamespaceNavigationName(@namespace);
            writer.ExceptionThrown -= OnExceptionThrown;

            return formatter.ToString();
		}

		public Dictionary<Resource, string> GetResourcesToFilePathsMap()
		{
			Dictionary<Resource, string> result = new Dictionary<Resource, string>();

			List<Resource> flattenedResources = new List<Resource>();
			Dictionary<Resource, string> flattenedResourcesFileNames = new Dictionary<Resource, string>();

			foreach (KeyValuePair<ModuleDefinition, Mono.Collections.Generic.Collection<Resource>> moduleResources in this.resources)
			{
				foreach (Resource resource in moduleResources.Value)
				{
                    if (!this.decompileDangerousResources &&
                        DangerousResourceIdentifier.IsDangerousResource(resource))
                    {
                        continue;
                    }

					if (resource.ResourceType != ResourceType.Embedded)
					{
						continue;
					}

					EmbeddedResource embeddedResource = (EmbeddedResource)resource;
					if (resource.Name.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
					{
						if (!embeddedResource.Name.EndsWith(".g.resources", StringComparison.OrdinalIgnoreCase))
						{
							string resourceName = embeddedResource.Name.Substring(0, embeddedResource.Name.Length - 10); //".resources".Length == 10
							string relativeResourcePath = GetResourceFilePath(resourceName);
							result.Add(resource, relativeResourcePath);

							if (relativeResourcePath.Length > maxRelativePathLength || Path.GetDirectoryName(relativeResourcePath) == "")
							{
								flattenedResources.Add(resource);

								string resourceFileName = Path.GetFileName(relativeResourcePath);
								flattenedResourcesFileNames.Add(resource, resourceFileName);
								result[resource] = Path.GetFileName(resourceFileName);
							}
						}
					}
					else
					{
						string resourceLegalName = Utilities.GetLegalFileName(resource.Name);
						result.Add(resource, resourceLegalName);

						// such resources are always in the root directory of the project,
						// so we add them, like the "" namespace types in GetTypesToFilePathsMap algorithm
						flattenedResources.Add(resource);
						flattenedResourcesFileNames.Add(resource, resourceLegalName);
					}
				}
			}

			// now we've got some resources flattened, we must fix any collisions and
			// after that rename any resources with still long paths.
			// note that if we have any code file along a resource, its path will
			// surely be with shorter path, since ".resx".Length() > ".cs"/".vb".Length()

			FixResourcesCollisions(result, flattenedResources, flattenedResourcesFileNames);

			HashSet<string> usedNames = new HashSet<string>();
			foreach (Resource resource in flattenedResources)
			{
				usedNames.Add(result[resource]);
			}

			int longResourcesNamesCount = 0;
			foreach (Resource resource in flattenedResources)
			{
				string currentResourceName = result[resource];
				string resourceExtension = Path.GetExtension(currentResourceName);
				if (currentResourceName.Length + sourceExtension.Length > maxRelativePathLength)
				{
					longResourcesNamesCount++;
					string nameCandidate = ResourcesShortNameStartSymbol + longResourcesNamesCount + resourceExtension;
					while (usedNames.Contains(nameCandidate))
					{
						longResourcesNamesCount++;
						nameCandidate = ResourcesShortNameStartSymbol + longResourcesNamesCount + resourceExtension;
					}
					usedNames.Remove(currentResourceName);
					usedNames.Add(nameCandidate);
					result[resource] = nameCandidate;
				}
			}

			return result;
		}

		private void FixResourcesCollisions(Dictionary<Resource, string> currentPaths, List<Resource> flattenedResources, Dictionary<Resource, string> flattenedResourcesFileNames)
		{
			Dictionary<string, List<Resource>> resourcesFilePathCollisionOccurrances = new Dictionary<string, List<Resource>>();
			HashSet<string> singleFilePathOccurances = new HashSet<string>();

			foreach (Resource resource in flattenedResources)
			{
				string resourceFilePathKey = currentPaths[resource].ToLower();

				if (resourcesFilePathCollisionOccurrances.ContainsKey(resourceFilePathKey))
				{
					singleFilePathOccurances.Remove(resourceFilePathKey);

					resourcesFilePathCollisionOccurrances[resourceFilePathKey].Add(resource);
				}
				else
				{
					singleFilePathOccurances.Add(resourceFilePathKey);

					List<Resource> filePathOcurrancesList = new List<Resource>();
					filePathOcurrancesList.Add(resource);
					resourcesFilePathCollisionOccurrances.Add(resourceFilePathKey, filePathOcurrancesList);
				}
			}

			foreach (KeyValuePair<string, List<Resource>> resourceFilePathCollisionPair in resourcesFilePathCollisionOccurrances)
			{
				if (resourceFilePathCollisionPair.Value.Count > 1)
				{
					int currentFilePathCollisionCount = 0;
					foreach (Resource resource in resourceFilePathCollisionPair.Value)
					{
						string currentResourceFullName = flattenedResourcesFileNames[resource];
						string currentResourceName = Path.GetFileNameWithoutExtension(currentResourceFullName);
						string resourceExtension = Path.GetExtension(currentResourceFullName);

						string pathCandidate = currentResourceName + ++currentFilePathCollisionCount + resourceExtension;
						while (singleFilePathOccurances.Contains(pathCandidate))
						{
							currentFilePathCollisionCount++;
							pathCandidate = currentResourceName + currentFilePathCollisionCount + resourceExtension;
						}

						currentPaths[resource] = pathCandidate;
					}
				}
			}
		}

		private string GetResourceFilePath(string resourceName)
		{
			string[] resourceNameTokens = resourceName.Split('.');

			int length = resourceNameTokens.Length;
			if (length > 1 && IsCultureName(resourceNameTokens[length - 1]))
			{
				--length;
				resourceNameTokens[length - 1] = new StringBuilder(resourceNameTokens[length - 1])
													.Append('.')
													.Append(resourceNameTokens[length])
													.ToString();
			}

			StringBuilder pathBuilder = new StringBuilder();
			string @namespace = string.Join(".", resourceNameTokens, 0, length - 1);

			if (@namespace != "")
			{
				string[] specialPath = namespaceHierarchyTree.GetSpecialPathTokens(@namespace);

				if (specialPath != null)
				{
					if (specialPath.Length > 0)
					{
						pathBuilder.Append(Utilities.GetLegalFolderName(string.Join(Path.DirectorySeparatorChar.ToString(), specialPath)))
								   .Append(Path.DirectorySeparatorChar);

					}
				}
				else
				{
					pathBuilder.Append(Utilities.GetLegalFileName(@namespace))
							   .Append('.');
				}
			}

			return pathBuilder.Append(Utilities.GetLegalFileName(resourceNameTokens[length - 1]))
							  .Append(".resx")
							  .ToString();
		}

		private bool IsCultureName(string str)
		{
			try
			{
				CultureInfo.GetCultureInfo(str);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public Dictionary<string, string> GetXamlResourcesToFilePathsMap()
		{
			Dictionary<string, string> result = new Dictionary<string, string>();
			List<string> flattenedXamlResources = new List<string>();

			foreach (XamlResource xamlResource in xamlResources)
			{
				string xamlResourceKey = Utilities.GetXamlResourceKey(xamlResource.Resource, xamlResource.Module);
				string xamlResourceRelativePath = ((string)xamlResource.Resource.Key).Replace('/', Path.DirectorySeparatorChar);

				string typeFullName = GetTypeNameFromXaml(xamlResourceRelativePath, xamlResource.Resource.Value as UnmanagedMemoryStream);
				if (typeFullName != null)
				{
					xamlResourceRelativePath = GetBetterXamlPath(xamlResourceRelativePath, typeFullName);
				}
#if !ENGINEONLYBUILD && !JUSTASSEMBLY
				else
				{
					if (xamlResourceRelativePath.EndsWith(".xaml") || xamlResourceRelativePath.EndsWith(".baml"))
					{
						xamlResourceRelativePath = Path.ChangeExtension(xamlResourceRelativePath, XamlExtension);
					}
                }
#endif

                xamlResourceRelativePath = Uri.UnescapeDataString(xamlResourceRelativePath);

				result.Add(xamlResourceKey, xamlResourceRelativePath);

				if (xamlResourceRelativePath.Length + sourceExtension.Length > maxRelativePathLength || Path.GetDirectoryName(xamlResourceRelativePath) == "")
				{
					flattenedXamlResources.Add(xamlResourceKey);
					result[xamlResourceKey] = Path.GetFileName(xamlResourceRelativePath);
				}
			}

			FixXamlResourcesCollisions(result, flattenedXamlResources);

			HashSet<string> usedNames = new HashSet<string>();
			foreach (string xamlResourceKey in flattenedXamlResources)
			{
				usedNames.Add(result[xamlResourceKey]);
			}

			int longResourcesNamesCount = 0;
			foreach (string xamlResourceKey in flattenedXamlResources)
			{
				string currentXamlResourceName = result[xamlResourceKey];
				if (currentXamlResourceName.Length + sourceExtension.Length > maxRelativePathLength)
				{
					longResourcesNamesCount++;
					string nameCandidate = XamlResourcesShortNameStartSymbol + longResourcesNamesCount + XamlExtension;
					while (usedNames.Contains(nameCandidate))
					{
						longResourcesNamesCount++;
						nameCandidate = XamlResourcesShortNameStartSymbol + longResourcesNamesCount + XamlExtension;
					}
					usedNames.Remove(currentXamlResourceName);
					usedNames.Add(nameCandidate);
					result[xamlResourceKey] = nameCandidate;
				}
			}

			return result;
		}

		private void FixXamlResourcesCollisions(Dictionary<string, string> currentPaths, List<string> flattenedXamlResources)
		{
			Dictionary<string, List<string>> xamlResourcesFilePathCollisionOccurrances = new Dictionary<string, List<string>>();
			HashSet<string> singleFilePathOccurances = new HashSet<string>();

			foreach (string xamlResourceKey in flattenedXamlResources)
			{
				string xamlResourceFilePathKey = currentPaths[xamlResourceKey].ToLower();

				if (xamlResourcesFilePathCollisionOccurrances.ContainsKey(xamlResourceFilePathKey))
				{
					singleFilePathOccurances.Remove(xamlResourceFilePathKey);

					xamlResourcesFilePathCollisionOccurrances[xamlResourceFilePathKey].Add(xamlResourceKey);
				}
				else
				{
					singleFilePathOccurances.Add(xamlResourceKey);

					List<string> filePathOcurrancesList = new List<string>();
					filePathOcurrancesList.Add(xamlResourceKey);
					xamlResourcesFilePathCollisionOccurrances.Add(xamlResourceFilePathKey, filePathOcurrancesList);
				}
			}

			foreach (KeyValuePair<string, List<string>> xamlResourceFilePathCollisionPair in xamlResourcesFilePathCollisionOccurrances)
			{
				if (xamlResourceFilePathCollisionPair.Value.Count > 1)
				{
					int currentFilePathCollisionCount = 0;
					foreach (string xamlResourceKey in xamlResourceFilePathCollisionPair.Value)
					{
						string currentXamlResourceFullName = currentPaths[xamlResourceKey];
						string currentXamlResourceName = Path.GetFileNameWithoutExtension(currentXamlResourceFullName);

						string pathCandidate = currentXamlResourceName + ++currentFilePathCollisionCount + XamlExtension;
						while (singleFilePathOccurances.Contains(pathCandidate))
						{
							currentFilePathCollisionCount++;
							pathCandidate = currentXamlResourceName + currentFilePathCollisionCount + XamlExtension;
						}

						currentPaths[xamlResourceKey] = pathCandidate;
					}
				}
			}
		}

		private string GetBetterXamlPath(string path, string fullClassName)
		{
			string pathWithoutExt = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
			string[] pathTokens = pathWithoutExt.Split(Path.DirectorySeparatorChar);
			string[] classNameTokens = fullClassName.Split('.');

			int pathIndex = pathTokens.Length - 1;
			int nameIndex = classNameTokens.Length - 1;
			for (; pathIndex >= 0 && nameIndex >= 0; --nameIndex, --pathIndex)
			{
				if (string.Compare(pathTokens[pathIndex], classNameTokens[nameIndex], StringComparison.OrdinalIgnoreCase) != 0)
				{
					break;
				}

				pathTokens[pathIndex] = classNameTokens[nameIndex];
			}

			return string.Join(Path.DirectorySeparatorChar.ToString(), pathTokens) + XamlExtension;
		}

		private string GetTypeNameFromXaml(string resourcePath, UnmanagedMemoryStream unmanagedStream)
		{
			if (unmanagedStream == null)
			{
				return null;
            }

#if ENGINEONLYBUILD || JUSTASSEMBLY
            return null;
#else
            bool isBaml = resourcePath.EndsWith(".baml", StringComparison.OrdinalIgnoreCase);
			XDocument xamlDoc = null;

			if (isBaml)
			{
				try
				{
					unmanagedStream.Seek(0, SeekOrigin.Begin);
					xamlDoc = BamlToXamlConverter.DecompileToDocument(unmanagedStream, assemblyResolver, assemblyPath);}
				catch
				{
					isBaml = false;
					unmanagedStream.Seek(0, SeekOrigin.Begin);
				}
			}

			string fullClassName = null;
			if (isBaml)
			{
				XAttribute classAttribute = xamlDoc.Root.Attribute(XName.Get("Class", "http://schemas.microsoft.com/winfx/2006/xaml"));
				if (classAttribute != null && classAttribute.Name != null)
				{
					fullClassName = classAttribute.Value;
				}
			}

			return fullClassName;
#endif
        }

		public string GetAssemblyInfoRelativePath()
		{
			string assemblyInfoFilePath = Path.Combine("Properties", "AssemblyInfo" + sourceExtension);

			if (assemblyInfoFilePath.Length > maxRelativePathLength)
			{
				assemblyInfoFilePath = AssemblyInfoShortFileName + sourceExtension;
			}

			return assemblyInfoFilePath;
		}

		public string GetResourceDesignerRelativePath(string resourceRelativePath)
		{
			string resourceRelativeDirectory = Path.GetDirectoryName(resourceRelativePath);
			string resourceName = Path.GetFileNameWithoutExtension(resourceRelativePath);

			string pathCandidate = Path.Combine(resourceRelativeDirectory, resourceName + ResourceDesignerNameExtension + sourceExtension);

			if (pathCandidate.Length > maxRelativePathLength)
			{
				pathCandidate = Path.Combine(resourceRelativeDirectory, resourceName + ResourceDesignerShortNameExtension + sourceExtension);
			}

			return pathCandidate;
		}

		private class XamlResource
		{
			public System.Collections.DictionaryEntry Resource { get; private set; }
			public ModuleDefinition Module { get; private set; }

			public XamlResource(System.Collections.DictionaryEntry resource, ModuleDefinition module)
			{
				this.Resource = resource;
				this.Module = module;
			}
		}
	}
}
