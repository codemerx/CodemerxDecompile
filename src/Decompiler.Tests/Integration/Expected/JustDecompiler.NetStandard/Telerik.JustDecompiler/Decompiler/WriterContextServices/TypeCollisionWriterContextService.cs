using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.MemberRenamingServices;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler.WriterContextServices
{
	public class TypeCollisionWriterContextService : BaseWriterContextService
	{
		private readonly static object assemblyContextsLocker;

		private readonly static object moduleContextsLocker;

		static TypeCollisionWriterContextService()
		{
			TypeCollisionWriterContextService.assemblyContextsLocker = new Object();
			TypeCollisionWriterContextService.moduleContextsLocker = new Object();
		}

		public TypeCollisionWriterContextService(IDecompilationCacheService cacheService, bool renameInvalidMembers) : base(cacheService, renameInvalidMembers)
		{
		}

		private string EscapeNamespace(string[] namespaceParts, ILanguage langugage)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(langugage.ReplaceInvalidCharactersInIdentifier(namespaceParts[0]));
			for (int i = 1; i < (int)namespaceParts.Length; i++)
			{
				stringBuilder.AppendFormat(".{0}", langugage.ReplaceInvalidCharactersInIdentifier(namespaceParts[i]));
			}
			return stringBuilder.ToString();
		}

		public override AssemblySpecificContext GetAssemblyContext(AssemblyDefinition assembly, ILanguage language)
		{
			if (this.cacheService.IsAssemblyContextInCache(assembly, language, this.renameInvalidMembers))
			{
				return this.cacheService.GetAssemblyContextFromCache(assembly, language, this.renameInvalidMembers);
			}
			AssemblySpecificContext assemblySpecificContext = new AssemblySpecificContext(base.GetAssemblyNamespaceUsings(assembly));
			lock (TypeCollisionWriterContextService.assemblyContextsLocker)
			{
				if (!this.cacheService.IsAssemblyContextInCache(assembly, language, this.renameInvalidMembers))
				{
					this.cacheService.AddAssemblyContextToCache(assembly, language, this.renameInvalidMembers, assemblySpecificContext);
				}
			}
			return assemblySpecificContext;
		}

		private Dictionary<string, List<string>> GetModuleCollisionTypesData(ModuleDefinition module, ILanguage language)
		{
			Dictionary<string, List<string>> strs = new Dictionary<string, List<string>>(language.IdentifierComparer);
			Dictionary<string, string> strs1 = new Dictionary<string, string>(language.IdentifierComparer);
			this.UpdateCollisionTypesDataWithTypes(strs, strs1, module.get_Types());
			Dictionary<AssemblyNameReference, List<TypeReference>> moduleDependsOnAnalysis = this.GetModuleDependsOnAnalysis(module);
			SpecialTypeAssembly specialTypeAssembly = (Mono.Cecil.AssemblyResolver.Extensions.IsReferenceAssembly(module) ? 1 : 0);
			foreach (AssemblyNameReference key in moduleDependsOnAnalysis.Keys)
			{
				AssemblyDefinition assemblyDefinition = module.get_AssemblyResolver().Resolve(key, "", ModuleDefinitionExtensions.GetModuleArchitecture(module), specialTypeAssembly, true);
				if (assemblyDefinition == null)
				{
					this.UpdateCollisionTypesDataWithTypes(strs, strs1, moduleDependsOnAnalysis[key]);
				}
				else
				{
					foreach (ModuleDefinition moduleDefinition in assemblyDefinition.get_Modules())
					{
						this.UpdateCollisionTypesDataWithTypes(strs, strs1, moduleDefinition.get_Types());
					}
				}
			}
			return strs;
		}

		public override ModuleSpecificContext GetModuleContext(ModuleDefinition module, ILanguage language)
		{
			ModuleSpecificContext moduleContextFromCache;
			lock (TypeCollisionWriterContextService.moduleContextsLocker)
			{
				if (!this.cacheService.IsModuleContextInCache(module, language, this.renameInvalidMembers))
				{
					ICollection<string> moduleNamespaceUsings = base.GetModuleNamespaceUsings(module);
					Dictionary<string, List<string>> moduleCollisionTypesData = this.GetModuleCollisionTypesData(module, language);
					Dictionary<string, HashSet<string>> moduleNamespaceHierarchy = this.GetModuleNamespaceHierarchy(module, language);
					Dictionary<string, string> renamedNamespacesMap = this.GetRenamedNamespacesMap(module, language);
					MemberRenamingData memberRenamingData = this.GetMemberRenamingData(module, language);
					ModuleSpecificContext moduleSpecificContext = new ModuleSpecificContext(module, moduleNamespaceUsings, moduleCollisionTypesData, moduleNamespaceHierarchy, renamedNamespacesMap, memberRenamingData.RenamedMembers, memberRenamingData.RenamedMembersMap);
					this.cacheService.AddModuleContextToCache(module, language, this.renameInvalidMembers, moduleSpecificContext);
					moduleContextFromCache = moduleSpecificContext;
				}
				else
				{
					moduleContextFromCache = this.cacheService.GetModuleContextFromCache(module, language, this.renameInvalidMembers);
				}
			}
			return moduleContextFromCache;
		}

		private Dictionary<AssemblyNameReference, List<TypeReference>> GetModuleDependsOnAnalysis(ModuleDefinition module)
		{
			Collection<TypeDefinition> types = module.get_Types();
			HashSet<TypeReference> typeReferences = new HashSet<TypeReference>();
			foreach (TypeReference type in types)
			{
				if (typeReferences.Contains(type))
				{
					continue;
				}
				typeReferences.Add(type);
			}
			foreach (TypeReference typeReference in module.GetTypeReferences())
			{
				if (typeReferences.Contains(typeReference))
				{
					continue;
				}
				typeReferences.Add(typeReference);
			}
			Dictionary<AssemblyNameReference, List<TypeReference>> assembliesDependingOnToUsedTypesMap = Utilities.GetAssembliesDependingOnToUsedTypesMap(module, Utilities.GetExpandedTypeDependanceList(typeReferences));
			foreach (AssemblyNameReference assemblyReference in module.get_AssemblyReferences())
			{
				if (assembliesDependingOnToUsedTypesMap.ContainsKey(assemblyReference))
				{
					continue;
				}
				assembliesDependingOnToUsedTypesMap.Add(assemblyReference, new List<TypeReference>());
			}
			return assembliesDependingOnToUsedTypesMap;
		}

		private Dictionary<string, HashSet<string>> GetModuleNamespaceHierarchy(ModuleDefinition module, ILanguage language)
		{
			Dictionary<string, HashSet<string>> strs = new Dictionary<string, HashSet<string>>(language.IdentifierComparer);
			this.UpdateNamespaceHiearchyDataWithTypes(strs, module.get_Types());
			Dictionary<AssemblyNameReference, List<TypeReference>> moduleDependsOnAnalysis = this.GetModuleDependsOnAnalysis(module);
			SpecialTypeAssembly specialTypeAssembly = (Mono.Cecil.AssemblyResolver.Extensions.IsReferenceAssembly(module) ? 1 : 0);
			foreach (AssemblyNameReference key in moduleDependsOnAnalysis.Keys)
			{
				AssemblyDefinition assemblyDefinition = module.get_AssemblyResolver().Resolve(key, "", ModuleDefinitionExtensions.GetModuleArchitecture(module), specialTypeAssembly, true);
				if (assemblyDefinition == null)
				{
					this.UpdateNamespaceHiearchyDataWithTypes(strs, moduleDependsOnAnalysis[key]);
				}
				else
				{
					this.UpdateNamespaceHiearchyDataWithTypes(strs, assemblyDefinition.get_MainModule().get_Types());
				}
			}
			return strs;
		}

		private Dictionary<string, string> GetRenamedNamespacesMap(ModuleDefinition module, ILanguage langugage)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			if (!this.renameInvalidMembers)
			{
				return strs;
			}
			HashSet<string> strs1 = new HashSet<string>();
		Label0:
			foreach (TypeDefinition type in module.get_Types())
			{
				string @namespace = type.GetNamespace();
				if (@namespace != String.Empty)
				{
					if (strs1.Contains(@namespace))
					{
						continue;
					}
					strs1.Add(@namespace);
					string[] strArray = @namespace.Split(new Char[] { '.' });
					string[] strArray1 = strArray;
					int num = 0;
					while (num < (int)strArray1.Length)
					{
						if (langugage.IsValidIdentifier(strArray1[num]))
						{
							num++;
						}
						else
						{
							strs.Add(@namespace, this.EscapeNamespace(strArray, langugage));
							goto Label0;
						}
					}
				}
				else
				{
					strs1.Add(@namespace);
				}
			}
			return strs;
		}

		public override WriterContext GetWriterContext(IMemberDefinition member, ILanguage language)
		{
			return this.GetWriterContextForType(Utilities.GetDeclaringTypeOrSelf(member), language);
		}

		private WriterContext GetWriterContextForType(TypeDefinition type, ILanguage language)
		{
			Dictionary<string, DecompiledType> nestedDecompiledTypes;
			DecompiledType decompiledType;
			TypeDefinition outerMostDeclaringType = Utilities.GetOuterMostDeclaringType(type);
			if (!this.cacheService.AreNestedDecompiledTypesInCache(outerMostDeclaringType, language, this.renameInvalidMembers))
			{
				nestedDecompiledTypes = base.GetNestedDecompiledTypes(outerMostDeclaringType, language);
				this.cacheService.AddNestedDecompiledTypesToCache(outerMostDeclaringType, language, this.renameInvalidMembers, nestedDecompiledTypes);
			}
			else
			{
				nestedDecompiledTypes = this.cacheService.GetNestedDecompiledTypesFromCache(outerMostDeclaringType, language, this.renameInvalidMembers);
			}
			TypeSpecificContext typeContext = this.GetTypeContext(outerMostDeclaringType, language, nestedDecompiledTypes);
			base.AddTypeContextsToCache(nestedDecompiledTypes, outerMostDeclaringType, language);
			Dictionary<string, MethodSpecificContext> strs = new Dictionary<string, MethodSpecificContext>();
			Dictionary<string, Statement> strs1 = new Dictionary<string, Statement>();
			if (!nestedDecompiledTypes.TryGetValue(type.get_FullName(), out decompiledType))
			{
				throw new Exception("Decompiled type missing from DecompiledTypes cache.");
			}
			if (typeContext.GeneratedFilterMethods.Count > 0)
			{
				base.AddGeneratedFilterMethodsToDecompiledType(decompiledType, typeContext, language);
			}
			foreach (DecompiledMember value in decompiledType.DecompiledMembers.Values)
			{
				strs.Add(value.MemberFullName, value.Context);
				strs1.Add(value.MemberFullName, value.Statement);
			}
			AssemblySpecificContext assemblyContext = this.GetAssemblyContext(outerMostDeclaringType.get_Module().get_Assembly(), language);
			ModuleSpecificContext moduleContext = this.GetModuleContext(outerMostDeclaringType.get_Module(), language);
			return new WriterContext(assemblyContext, moduleContext, typeContext, strs, strs1);
		}

		private void UpdateCollisionTypesDataWithTypes(Dictionary<string, List<string>> collisionTypesData, Dictionary<string, string> typeNamesFirstOccurrence, IEnumerable<TypeReference> types)
		{
			string str;
			List<string> strs;
			foreach (TypeReference type in types)
			{
				string name = type.get_Name();
				if (!typeNamesFirstOccurrence.ContainsKey(name))
				{
					typeNamesFirstOccurrence.Add(name, type.get_Namespace());
				}
				else if (collisionTypesData.ContainsKey(name))
				{
					if (!collisionTypesData.TryGetValue(name, out strs))
					{
						throw new Exception("Collision type namespaces collection not found in collision types cache.");
					}
					strs.Add(type.get_Namespace());
				}
				else
				{
					if (!typeNamesFirstOccurrence.TryGetValue(name, out str))
					{
						throw new Exception("Namespace not found in type first time occurence cache.");
					}
					List<string> strs1 = new List<string>()
					{
						str,
						type.get_Namespace()
					};
					collisionTypesData.Add(name, strs1);
				}
			}
		}

		private void UpdateNamespaceHiearchyDataWithTypes(Dictionary<string, HashSet<string>> namespaceHierarchy, IEnumerable<TypeReference> types)
		{
			HashSet<string> strs;
			foreach (TypeReference type in types)
			{
				string @namespace = type.get_Namespace();
				if (!Utilities.HasNamespaceParentNamespace(@namespace))
				{
					continue;
				}
				string namesapceParentNamesapce = Utilities.GetNamesapceParentNamesapce(@namespace);
				string namespaceChildNamesapce = Utilities.GetNamespaceChildNamesapce(@namespace);
				if (!namespaceHierarchy.TryGetValue(namesapceParentNamesapce, out strs))
				{
					strs = new HashSet<string>();
					strs.Add(namespaceChildNamesapce);
					namespaceHierarchy.Add(namesapceParentNamesapce, strs);
				}
				else
				{
					if (strs.Contains(namespaceChildNamesapce))
					{
						continue;
					}
					strs.Add(namespaceChildNamesapce);
				}
			}
		}
	}
}