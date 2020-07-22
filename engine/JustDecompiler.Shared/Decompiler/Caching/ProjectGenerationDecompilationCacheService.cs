using System;
using System.Linq;
using Mono.Cecil;
using Telerik.JustDecompiler.Languages;
using System.Collections.Generic;
using System.IO;

namespace Telerik.JustDecompiler.Decompiler.Caching
{
	public class ProjectGenerationDecompilationCacheService : IDecompilationCacheService
	{
		private static Dictionary<string, AssemblySpecificContext> assemblyContextsCache = new Dictionary<string, AssemblySpecificContext>();
		private static Dictionary<string, ModuleSpecificContext> moduleContextsCache = new Dictionary<string, ModuleSpecificContext>();
		private static Dictionary<ModuleDefinition, string> moduleContextsKeysMap = new Dictionary<ModuleDefinition, string>();
		private Dictionary<string, TypeSpecificContext> typeContextsCache = new Dictionary<string, TypeSpecificContext>();
		private Dictionary<string, CachedDecompiledMember> decompiledMembersCache = new Dictionary<string, CachedDecompiledMember>();
		private Dictionary<string, Dictionary<string, DecompiledType>> nestedDecompiledTypesCache = new Dictionary<string, Dictionary<string, DecompiledType>>();

		private static readonly object locker = new object();

		public ProjectGenerationDecompilationCacheService() { }

		public bool IsAssemblyContextInCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers)
		{
			return assemblyContextsCache.ContainsKey(GetModuleKey(assembly.MainModule, language, renameInvalidMembers));
		}

		public AssemblySpecificContext GetAssemblyContextFromCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers)
		{
			AssemblySpecificContext assemblyContext;

			if (!assemblyContextsCache.TryGetValue(GetModuleKey(assembly.MainModule, language, renameInvalidMembers), out assemblyContext))
			{
				throw new Exception("Key not found in AssemblyContextsCache");
			}

			return assemblyContext;
		}

		public void AddAssemblyContextToCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers, AssemblySpecificContext assemblyContext)
		{
			string moduleKey = GetModuleKey(assembly.MainModule, language, renameInvalidMembers);

			if (assemblyContextsCache.ContainsKey(moduleKey))
			{
				throw new Exception("Key already in AssemblyContextsCache");
			}

			assemblyContextsCache.Add(moduleKey, assemblyContext);
		}

		private string GetModuleKey(ModuleDefinition module, ILanguage language, bool renameInvalidMembers)
		{
			if (!moduleContextsKeysMap.ContainsKey(module))
			{
				lock (locker)
				{
					if (!moduleContextsKeysMap.ContainsKey(module))
					{
						FileInfo fileInfo = new FileInfo(module.FilePath);
						string key = module.FilePath + " : " + fileInfo.LastWriteTime + " : " + module.Name.ToString() + " " + language.Name + " " + renameInvalidMembers.ToString();
						moduleContextsKeysMap.Add(module, key);
						return key;
					}
				}
			}

			return moduleContextsKeysMap[module];
		}

		public bool AreNestedDecompiledTypesInCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			return nestedDecompiledTypesCache.ContainsKey(GetTypeKey(type, language, renameInvalidMembers));
		}

		public Dictionary<string, DecompiledType> GetNestedDecompiledTypesFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			Dictionary<string, DecompiledType> nestedDecompiledTypes;

			if (!nestedDecompiledTypesCache.TryGetValue(GetTypeKey(type, language, renameInvalidMembers), out nestedDecompiledTypes))
			{
				throw new Exception("Key not found in NestedDecompiledTypesCache");
			}

			return nestedDecompiledTypes;
		}

		public void AddNestedDecompiledTypesToCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers, Dictionary<string, DecompiledType> decompiledTypes)
		{
			string typeKey = GetTypeKey(type, language, renameInvalidMembers);

			if (nestedDecompiledTypesCache.ContainsKey(typeKey))
			{
				throw new Exception("Key already in NestedDecompiledTypesCache");
			}

			nestedDecompiledTypesCache.Add(typeKey, decompiledTypes);
		}

		private string GetTypeKey(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			return GetModuleKey(type.Module, language, renameInvalidMembers) + " : " + language.Name + " : " + type.FullName;
		}

		public bool IsModuleContextInCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers)
		{
			return moduleContextsCache.ContainsKey(GetModuleKey(module, language, renameInvalidMembers));
		}

		public ModuleSpecificContext GetModuleContextFromCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers)
		{
			ModuleSpecificContext moduleContext;

			if (!moduleContextsCache.TryGetValue(GetModuleKey(module, language, renameInvalidMembers), out moduleContext))
			{
				throw new Exception("Key not found in ModuleContextsCache");
			}

			return moduleContext;
		}

		public void AddModuleContextToCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers, ModuleSpecificContext moduleContext)
		{
			string moduleKey = GetModuleKey(module, language, renameInvalidMembers);

			if (moduleContextsCache.ContainsKey(moduleKey))
			{
				throw new Exception("Key already in ModuleContextsCache");
			}

			moduleContextsCache.Add(moduleKey, moduleContext);
		}

		public bool IsTypeContextInCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			return typeContextsCache.ContainsKey(GetTypeKey(type, language, renameInvalidMembers));
		}

		public TypeSpecificContext GetTypeContextFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			TypeSpecificContext typeContext;

			if (!typeContextsCache.TryGetValue(GetTypeKey(type, language, renameInvalidMembers), out typeContext))
			{
				throw new Exception("Key not found in TypeContextsCache");
			}

			return typeContext;
		}

		public  void AddTypeContextToCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers, TypeSpecificContext typeContext)
		{
			string typeKey = GetTypeKey(type, language, renameInvalidMembers);

			if (typeContextsCache.ContainsKey(typeKey))
			{
				throw new Exception("Key already in TypeContextsCache");
			}

			typeContextsCache.Add(typeKey, typeContext);
		}

        public void ReplaceCachedTypeContext(TypeDefinition type, ILanguage language, bool renameInvalidMembers, TypeSpecificContext newContext)
        {
            if (newContext == null)
            {
                throw new ArgumentNullException("newContext");
            }

            RemoveTypeContextFromCache(type, language, renameInvalidMembers);
            AddTypeContextToCache(type, language, renameInvalidMembers, newContext);
        }

        private void RemoveTypeContextFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
        {
            typeContextsCache.Remove(GetTypeKey(type, language, renameInvalidMembers));
        }

		public bool IsDecompiledMemberInCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers)
		{
			return false;
		}

		public CachedDecompiledMember GetDecompiledMemberFromCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers)
		{
			throw new NotSupportedException("ProjectGenerationDecompilationCacheServices caches only full nested decompiled types.");
		}

		public void AddDecompiledMemberToCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers, CachedDecompiledMember decompiledMember)
		{
			return;
		}

		public static void ClearAssemblyContextsCache()
		{
			assemblyContextsCache.Clear();
			moduleContextsCache.Clear();
			moduleContextsKeysMap.Clear();
		}
    }
}
