using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler.Caching
{
	public class ProjectGenerationDecompilationCacheService : IDecompilationCacheService
	{
		private static Dictionary<string, AssemblySpecificContext> assemblyContextsCache;

		private static Dictionary<string, ModuleSpecificContext> moduleContextsCache;

		private static Dictionary<ModuleDefinition, string> moduleContextsKeysMap;

		private Dictionary<string, TypeSpecificContext> typeContextsCache = new Dictionary<string, TypeSpecificContext>();

		private Dictionary<string, CachedDecompiledMember> decompiledMembersCache = new Dictionary<string, CachedDecompiledMember>();

		private Dictionary<string, Dictionary<string, DecompiledType>> nestedDecompiledTypesCache = new Dictionary<string, Dictionary<string, DecompiledType>>();

		private readonly static object locker;

		static ProjectGenerationDecompilationCacheService()
		{
			ProjectGenerationDecompilationCacheService.assemblyContextsCache = new Dictionary<string, AssemblySpecificContext>();
			ProjectGenerationDecompilationCacheService.moduleContextsCache = new Dictionary<string, ModuleSpecificContext>();
			ProjectGenerationDecompilationCacheService.moduleContextsKeysMap = new Dictionary<ModuleDefinition, string>();
			ProjectGenerationDecompilationCacheService.locker = new Object();
		}

		public ProjectGenerationDecompilationCacheService()
		{
		}

		public void AddAssemblyContextToCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers, AssemblySpecificContext assemblyContext)
		{
			string moduleKey = this.GetModuleKey(assembly.MainModule, language, renameInvalidMembers);
			if (ProjectGenerationDecompilationCacheService.assemblyContextsCache.ContainsKey(moduleKey))
			{
				throw new Exception("Key already in AssemblyContextsCache");
			}
			ProjectGenerationDecompilationCacheService.assemblyContextsCache.Add(moduleKey, assemblyContext);
		}

		public void AddDecompiledMemberToCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers, CachedDecompiledMember decompiledMember)
		{
		}

		public void AddModuleContextToCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers, ModuleSpecificContext moduleContext)
		{
			string moduleKey = this.GetModuleKey(module, language, renameInvalidMembers);
			if (ProjectGenerationDecompilationCacheService.moduleContextsCache.ContainsKey(moduleKey))
			{
				throw new Exception("Key already in ModuleContextsCache");
			}
			ProjectGenerationDecompilationCacheService.moduleContextsCache.Add(moduleKey, moduleContext);
		}

		public void AddNestedDecompiledTypesToCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers, Dictionary<string, DecompiledType> decompiledTypes)
		{
			string typeKey = this.GetTypeKey(type, language, renameInvalidMembers);
			if (this.nestedDecompiledTypesCache.ContainsKey(typeKey))
			{
				throw new Exception("Key already in NestedDecompiledTypesCache");
			}
			this.nestedDecompiledTypesCache.Add(typeKey, decompiledTypes);
		}

		public void AddTypeContextToCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers, TypeSpecificContext typeContext)
		{
			string typeKey = this.GetTypeKey(type, language, renameInvalidMembers);
			if (this.typeContextsCache.ContainsKey(typeKey))
			{
				throw new Exception("Key already in TypeContextsCache");
			}
			this.typeContextsCache.Add(typeKey, typeContext);
		}

		public bool AreNestedDecompiledTypesInCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			return this.nestedDecompiledTypesCache.ContainsKey(this.GetTypeKey(type, language, renameInvalidMembers));
		}

		public static void ClearAssemblyContextsCache()
		{
			ProjectGenerationDecompilationCacheService.assemblyContextsCache.Clear();
			ProjectGenerationDecompilationCacheService.moduleContextsCache.Clear();
			ProjectGenerationDecompilationCacheService.moduleContextsKeysMap.Clear();
		}

		public AssemblySpecificContext GetAssemblyContextFromCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers)
		{
			AssemblySpecificContext assemblySpecificContext;
			if (!ProjectGenerationDecompilationCacheService.assemblyContextsCache.TryGetValue(this.GetModuleKey(assembly.MainModule, language, renameInvalidMembers), out assemblySpecificContext))
			{
				throw new Exception("Key not found in AssemblyContextsCache");
			}
			return assemblySpecificContext;
		}

		public CachedDecompiledMember GetDecompiledMemberFromCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers)
		{
			throw new NotSupportedException("ProjectGenerationDecompilationCacheServices caches only full nested decompiled types.");
		}

		public ModuleSpecificContext GetModuleContextFromCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers)
		{
			ModuleSpecificContext moduleSpecificContext;
			if (!ProjectGenerationDecompilationCacheService.moduleContextsCache.TryGetValue(this.GetModuleKey(module, language, renameInvalidMembers), out moduleSpecificContext))
			{
				throw new Exception("Key not found in ModuleContextsCache");
			}
			return moduleSpecificContext;
		}

		private string GetModuleKey(ModuleDefinition module, ILanguage language, bool renameInvalidMembers)
		{
			string str;
			if (!ProjectGenerationDecompilationCacheService.moduleContextsKeysMap.ContainsKey(module))
			{
				lock (ProjectGenerationDecompilationCacheService.locker)
				{
					if (ProjectGenerationDecompilationCacheService.moduleContextsKeysMap.ContainsKey(module))
					{
						return ProjectGenerationDecompilationCacheService.moduleContextsKeysMap[module];
					}
					else
					{
						FileInfo fileInfo = new FileInfo(module.FilePath);
						String[] filePath = new String[] { module.FilePath, " : ", null, null, null, null, null, null, null };
						filePath[2] = fileInfo.LastWriteTime.ToString();
						filePath[3] = " : ";
						filePath[4] = module.Name.ToString();
						filePath[5] = " ";
						filePath[6] = language.Name;
						filePath[7] = " ";
						filePath[8] = renameInvalidMembers.ToString();
						string str1 = String.Concat(filePath);
						ProjectGenerationDecompilationCacheService.moduleContextsKeysMap.Add(module, str1);
						str = str1;
					}
				}
				return str;
			}
			return ProjectGenerationDecompilationCacheService.moduleContextsKeysMap[module];
		}

		public Dictionary<string, DecompiledType> GetNestedDecompiledTypesFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			Dictionary<string, DecompiledType> strs;
			if (!this.nestedDecompiledTypesCache.TryGetValue(this.GetTypeKey(type, language, renameInvalidMembers), out strs))
			{
				throw new Exception("Key not found in NestedDecompiledTypesCache");
			}
			return strs;
		}

		public TypeSpecificContext GetTypeContextFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			TypeSpecificContext typeSpecificContext;
			if (!this.typeContextsCache.TryGetValue(this.GetTypeKey(type, language, renameInvalidMembers), out typeSpecificContext))
			{
				throw new Exception("Key not found in TypeContextsCache");
			}
			return typeSpecificContext;
		}

		private string GetTypeKey(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			return String.Concat(new String[] { this.GetModuleKey(type.Module, language, renameInvalidMembers), " : ", language.Name, " : ", type.FullName });
		}

		public bool IsAssemblyContextInCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers)
		{
			return ProjectGenerationDecompilationCacheService.assemblyContextsCache.ContainsKey(this.GetModuleKey(assembly.MainModule, language, renameInvalidMembers));
		}

		public bool IsDecompiledMemberInCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers)
		{
			return false;
		}

		public bool IsModuleContextInCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers)
		{
			return ProjectGenerationDecompilationCacheService.moduleContextsCache.ContainsKey(this.GetModuleKey(module, language, renameInvalidMembers));
		}

		public bool IsTypeContextInCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			return this.typeContextsCache.ContainsKey(this.GetTypeKey(type, language, renameInvalidMembers));
		}

		private void RemoveTypeContextFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			this.typeContextsCache.Remove(this.GetTypeKey(type, language, renameInvalidMembers));
		}

		public void ReplaceCachedTypeContext(TypeDefinition type, ILanguage language, bool renameInvalidMembers, TypeSpecificContext newContext)
		{
			if (newContext == null)
			{
				throw new ArgumentNullException("newContext");
			}
			this.RemoveTypeContextFromCache(type, language, renameInvalidMembers);
			this.AddTypeContextToCache(type, language, renameInvalidMembers, newContext);
		}
	}
}