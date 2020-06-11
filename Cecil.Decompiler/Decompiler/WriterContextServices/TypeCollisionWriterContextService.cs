using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Cecil.AssemblyResolver;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.MemberRenamingServices;

namespace Telerik.JustDecompiler.Decompiler.WriterContextServices
{
	public class TypeCollisionWriterContextService : BaseWriterContextService
	{
		private static readonly object assemblyContextsLocker = new object();
		private static readonly object moduleContextsLocker = new object();

		public TypeCollisionWriterContextService(IDecompilationCacheService cacheService, bool renameInvalidMembers) : base(cacheService, renameInvalidMembers) { }

		public override WriterContext GetWriterContext(IMemberDefinition member, ILanguage language)
		{
			TypeDefinition type = Utilities.GetDeclaringTypeOrSelf(member);
			return GetWriterContextForType(type, language);
		}

		public override AssemblySpecificContext GetAssemblyContext(AssemblyDefinition assembly, ILanguage language)
		{
			if (this.cacheService.IsAssemblyContextInCache(assembly, language, this.renameInvalidMembers))
			{
				return this.cacheService.GetAssemblyContextFromCache(assembly, language, this.renameInvalidMembers);
			}

			ICollection<string> assemblyNamespaceUsings = GetAssemblyNamespaceUsings(assembly);
			AssemblySpecificContext assemblyContext = new AssemblySpecificContext(assemblyNamespaceUsings);

			lock (assemblyContextsLocker)
			{
				if (!this.cacheService.IsAssemblyContextInCache(assembly, language, this.renameInvalidMembers))
				{
					this.cacheService.AddAssemblyContextToCache(assembly, language, this.renameInvalidMembers, assemblyContext);
				}
			}

			return assemblyContext;
		}

		public override ModuleSpecificContext GetModuleContext(ModuleDefinition module, ILanguage language)
		{
			lock (moduleContextsLocker)
			{
				if (this.cacheService.IsModuleContextInCache(module, language, this.renameInvalidMembers))
				{
					return this.cacheService.GetModuleContextFromCache(module, language, this.renameInvalidMembers);
				}

				ICollection<string> moduleNamespaceUsings = GetModuleNamespaceUsings(module);
				Dictionary<string, List<string>> collisionTypesData = GetModuleCollisionTypesData(module, language);
				Dictionary<string, HashSet<string>> namespaceHieararchy = GetModuleNamespaceHierarchy(module, language);
				Dictionary<string, string> renamedNamespacesMap = GetRenamedNamespacesMap(module, language);
				MemberRenamingData memberRenamingData = GetMemberRenamingData(module, language);

				ModuleSpecificContext moduleContext =
					new ModuleSpecificContext(module, moduleNamespaceUsings, collisionTypesData, namespaceHieararchy, renamedNamespacesMap, memberRenamingData.RenamedMembers, memberRenamingData.RenamedMembersMap);

				this.cacheService.AddModuleContextToCache(module, language, this.renameInvalidMembers, moduleContext);

							return moduleContext;
			}
		}
  
		private WriterContext GetWriterContextForType(TypeDefinition type, ILanguage language)
		{
			TypeDefinition outerMostDeclaringType = Utilities.GetOuterMostDeclaringType(type);

			Dictionary<string, DecompiledType> decompiledTypes;
			if (this.cacheService.AreNestedDecompiledTypesInCache(outerMostDeclaringType, language, this.renameInvalidMembers))
			{
				decompiledTypes = this.cacheService.GetNestedDecompiledTypesFromCache(outerMostDeclaringType, language, this.renameInvalidMembers);
			}
			else
			{
				decompiledTypes = GetNestedDecompiledTypes(outerMostDeclaringType, language);
				this.cacheService.AddNestedDecompiledTypesToCache(outerMostDeclaringType, language, this.renameInvalidMembers, decompiledTypes);
			}

			TypeSpecificContext typeContext = GetTypeContext(outerMostDeclaringType, language, decompiledTypes);

            AddTypeContextsToCache(decompiledTypes, outerMostDeclaringType, language);

			Dictionary<string, MethodSpecificContext> methodContexts = new Dictionary<string, MethodSpecificContext>();
			Dictionary<string, Statement> decompiledStatements = new Dictionary<string, Statement>();

			DecompiledType decompiledType;

			if (!decompiledTypes.TryGetValue(type.FullName, out decompiledType))
			{
				throw new Exception("Decompiled type missing from DecompiledTypes cache.");
			}
			else
            {
                // If members were taken from the cache, generated filter methods must be added to decompiled type.
                if (typeContext.GeneratedFilterMethods.Count > 0)
                {
                    AddGeneratedFilterMethodsToDecompiledType(decompiledType, typeContext, language);
                }

				foreach (DecompiledMember decompiledMember in decompiledType.DecompiledMembers.Values)
				{
					methodContexts.Add(decompiledMember.MemberFullName, decompiledMember.Context);
					decompiledStatements.Add(decompiledMember.MemberFullName, decompiledMember.Statement);
				}
			}

			AssemblySpecificContext assemblyContext = GetAssemblyContext(outerMostDeclaringType.Module.Assembly, language);
			ModuleSpecificContext moduleContext = GetModuleContext(outerMostDeclaringType.Module, language);

			WriterContext writerContext = new WriterContext(assemblyContext, moduleContext, typeContext, methodContexts, decompiledStatements);

			return writerContext;
		}

		private Dictionary<string, string> GetRenamedNamespacesMap(ModuleDefinition module, ILanguage langugage)
		{
			Dictionary<string, string> result = new Dictionary<string, string>();
			if (!renameInvalidMembers)
			{
				// If the renaming of members is stopped, then this map must be empty.
				return result;
			}
			HashSet<string> traversedNamespaces = new HashSet<string>();

			foreach (TypeDefinition type in module.Types)
			{
				string currentNamespace = type.GetNamespace();
				if (currentNamespace == string.Empty)
				{
					traversedNamespaces.Add(currentNamespace);
					continue;
				}
				if (traversedNamespaces.Contains(currentNamespace))
				{
					continue;
				}
				traversedNamespaces.Add(currentNamespace);
				string[] namespaceParts = currentNamespace.Split('.');
				foreach (string part in namespaceParts)
				{
					if (!langugage.IsValidIdentifier(part))
					{
						string escapedNamespace = EscapeNamespace(namespaceParts, langugage);
						result.Add(currentNamespace, escapedNamespace);
						break;
					}
				}
			}
			
			return result;
		}
  
		private string EscapeNamespace(string[] namespaceParts, ILanguage langugage)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(langugage.ReplaceInvalidCharactersInIdentifier(namespaceParts[0]));
			for (int i = 1; i < namespaceParts.Length; i++)
			{
				sb.AppendFormat(".{0}", langugage.ReplaceInvalidCharactersInIdentifier(namespaceParts[i]));
			}
			return sb.ToString();
		}

		private Dictionary<AssemblyNameReference, List<TypeReference>> GetModuleDependsOnAnalysis(ModuleDefinition module)
		{
			Mono.Collections.Generic.Collection<TypeDefinition> assemblyModuleTypes = module.Types;

			HashSet<TypeReference> firstLevelDependanceTypes = new HashSet<TypeReference>();
			foreach (TypeReference type in assemblyModuleTypes)
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

			ICollection<TypeReference> expadendTypeDependanceList = Utilities.GetExpandedTypeDependanceList(firstLevelDependanceTypes);
			Dictionary<AssemblyNameReference, List<TypeReference>> dependingOnAssembliesToUsedTypesMap = Utilities.GetAssembliesDependingOnToUsedTypesMap(module, expadendTypeDependanceList);

			foreach (AssemblyNameReference assemblyReference in module.AssemblyReferences)
			{
				if (!dependingOnAssembliesToUsedTypesMap.ContainsKey(assemblyReference))
				{
					dependingOnAssembliesToUsedTypesMap.Add(assemblyReference, new List<TypeReference>());
				}
			}

			return dependingOnAssembliesToUsedTypesMap;
		}

		private void UpdateNamespaceHiearchyDataWithTypes(Dictionary<string, HashSet<string>> namespaceHierarchy, IEnumerable<TypeReference> types)
		{
			foreach (TypeReference typeReference in types)
			{
                string @namespace = typeReference.Namespace;

                if (Utilities.HasNamespaceParentNamespace(@namespace))
                {
                    string parentNamespace = Utilities.GetNamesapceParentNamesapce(@namespace);
                    string childNamespace = Utilities.GetNamespaceChildNamesapce(@namespace);

                    HashSet<string> namespaceChildren;

                    if (namespaceHierarchy.TryGetValue(parentNamespace, out namespaceChildren))
                    {
                        if (!namespaceChildren.Contains(childNamespace))
                        {
                            namespaceChildren.Add(childNamespace);
                        }
                    }
                    else
                    {
                        namespaceChildren = new HashSet<string>();
                        namespaceChildren.Add(childNamespace);
                        namespaceHierarchy.Add(parentNamespace, namespaceChildren);
                    }
                }
            }
		}
        
        private Dictionary<string, HashSet<string>> GetModuleNamespaceHierarchy(ModuleDefinition module, ILanguage language)
		{
			Dictionary<string, HashSet<string>> namespaceHierarchy = new Dictionary<string, HashSet<string>>(language.IdentifierComparer);

			UpdateNamespaceHiearchyDataWithTypes(namespaceHierarchy, module.Types);

            Dictionary<AssemblyNameReference, List<TypeReference>> dependingOnAssembliesToUsedTypesMap = GetModuleDependsOnAnalysis(module);

            SpecialTypeAssembly special = module.IsReferenceAssembly() ? SpecialTypeAssembly.Reference : SpecialTypeAssembly.None;
            foreach (AssemblyNameReference assemblyNameReference in dependingOnAssembliesToUsedTypesMap.Keys)
			{
				AssemblyDefinition referencedAssembly = module.AssemblyResolver.Resolve(assemblyNameReference, "", module.GetModuleArchitecture(), special);

				if (referencedAssembly != null)
				{
					UpdateNamespaceHiearchyDataWithTypes(namespaceHierarchy, referencedAssembly.MainModule.Types);
				}
                else
                {
                    UpdateNamespaceHiearchyDataWithTypes(namespaceHierarchy, dependingOnAssembliesToUsedTypesMap[assemblyNameReference]);
                }
			}

			return namespaceHierarchy;
		}

		private void UpdateCollisionTypesDataWithTypes(Dictionary<string, List<string>> collisionTypesData, 
			Dictionary<string, string> typeNamesFirstOccurrence, IEnumerable<TypeReference> types)
		{
			foreach (TypeReference typeReference in types)
			{
				string typeName = typeReference.Name;
				if (typeNamesFirstOccurrence.ContainsKey(typeName))
				{
					if (!collisionTypesData.ContainsKey(typeName))
					{
						string typeFirstNamespaceOccurence;
						if (!typeNamesFirstOccurrence.TryGetValue(typeName, out typeFirstNamespaceOccurence))
						{
							throw new Exception("Namespace not found in type first time occurence cache.");
						}

						List<string> typeNamespaces = new List<string>();
						typeNamespaces.Add(typeFirstNamespaceOccurence);
						typeNamespaces.Add(typeReference.Namespace);
						collisionTypesData.Add(typeName, typeNamespaces);
					}
					else
					{
						List<string> collisionTypeNamespaces;
						if (!collisionTypesData.TryGetValue(typeName, out collisionTypeNamespaces))
						{
							throw new Exception("Collision type namespaces collection not found in collision types cache.");
						}

						collisionTypeNamespaces.Add(typeReference.Namespace);
					}
				}
				else
				{
					typeNamesFirstOccurrence.Add(typeName, typeReference.Namespace);
				}
			}
		}

		private Dictionary<string, List<string>> GetModuleCollisionTypesData(ModuleDefinition module, ILanguage language)
		{
			Dictionary<string, List<string>> collisionTypesData = new Dictionary<string, List<string>>(language.IdentifierComparer);
			Dictionary<string, string> typeNamesFirstOccurrence = new Dictionary<string, string>(language.IdentifierComparer);

			UpdateCollisionTypesDataWithTypes(collisionTypesData, typeNamesFirstOccurrence, module.Types);

            Dictionary<AssemblyNameReference, List<TypeReference>> dependingOnAssembliesToUsedTypesMap = GetModuleDependsOnAnalysis(module);

            SpecialTypeAssembly special = module.IsReferenceAssembly() ? SpecialTypeAssembly.Reference : SpecialTypeAssembly.None;
            foreach (AssemblyNameReference assemblyNameReference in dependingOnAssembliesToUsedTypesMap.Keys)
			{
				AssemblyDefinition referencedAssembly = module.AssemblyResolver.Resolve(assemblyNameReference, "", module.GetModuleArchitecture(), special);

				if (referencedAssembly != null)
				{
					foreach (ModuleDefinition referencedModule in referencedAssembly.Modules)
					{
						UpdateCollisionTypesDataWithTypes(collisionTypesData, typeNamesFirstOccurrence, referencedModule.Types);
					}
				}
                else
                {
                    UpdateCollisionTypesDataWithTypes(collisionTypesData, typeNamesFirstOccurrence, dependingOnAssembliesToUsedTypesMap[assemblyNameReference]);
                }
			}

			return collisionTypesData;
		}
	}
}