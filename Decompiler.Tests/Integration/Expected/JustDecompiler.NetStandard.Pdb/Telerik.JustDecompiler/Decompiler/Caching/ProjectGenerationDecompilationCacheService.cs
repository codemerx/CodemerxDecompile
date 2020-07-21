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

		private Dictionary<string, TypeSpecificContext> typeContextsCache;

		private Dictionary<string, CachedDecompiledMember> decompiledMembersCache;

		private Dictionary<string, Dictionary<string, DecompiledType>> nestedDecompiledTypesCache;

		private readonly static object locker;

		static ProjectGenerationDecompilationCacheService()
		{
			ProjectGenerationDecompilationCacheService.assemblyContextsCache = new Dictionary<string, AssemblySpecificContext>();
			ProjectGenerationDecompilationCacheService.moduleContextsCache = new Dictionary<string, ModuleSpecificContext>();
			ProjectGenerationDecompilationCacheService.moduleContextsKeysMap = new Dictionary<ModuleDefinition, string>();
			ProjectGenerationDecompilationCacheService.locker = new Object();
			return;
		}

		public ProjectGenerationDecompilationCacheService()
		{
			this.typeContextsCache = new Dictionary<string, TypeSpecificContext>();
			this.decompiledMembersCache = new Dictionary<string, CachedDecompiledMember>();
			this.nestedDecompiledTypesCache = new Dictionary<string, Dictionary<string, DecompiledType>>();
			base();
			return;
		}

		public void AddAssemblyContextToCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers, AssemblySpecificContext assemblyContext)
		{
			V_0 = this.GetModuleKey(assembly.get_MainModule(), language, renameInvalidMembers);
			if (ProjectGenerationDecompilationCacheService.assemblyContextsCache.ContainsKey(V_0))
			{
				throw new Exception("Key already in AssemblyContextsCache");
			}
			ProjectGenerationDecompilationCacheService.assemblyContextsCache.Add(V_0, assemblyContext);
			return;
		}

		public void AddDecompiledMemberToCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers, CachedDecompiledMember decompiledMember)
		{
			return;
		}

		public void AddModuleContextToCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers, ModuleSpecificContext moduleContext)
		{
			V_0 = this.GetModuleKey(module, language, renameInvalidMembers);
			if (ProjectGenerationDecompilationCacheService.moduleContextsCache.ContainsKey(V_0))
			{
				throw new Exception("Key already in ModuleContextsCache");
			}
			ProjectGenerationDecompilationCacheService.moduleContextsCache.Add(V_0, moduleContext);
			return;
		}

		public void AddNestedDecompiledTypesToCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers, Dictionary<string, DecompiledType> decompiledTypes)
		{
			V_0 = this.GetTypeKey(type, language, renameInvalidMembers);
			if (this.nestedDecompiledTypesCache.ContainsKey(V_0))
			{
				throw new Exception("Key already in NestedDecompiledTypesCache");
			}
			this.nestedDecompiledTypesCache.Add(V_0, decompiledTypes);
			return;
		}

		public void AddTypeContextToCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers, TypeSpecificContext typeContext)
		{
			V_0 = this.GetTypeKey(type, language, renameInvalidMembers);
			if (this.typeContextsCache.ContainsKey(V_0))
			{
				throw new Exception("Key already in TypeContextsCache");
			}
			this.typeContextsCache.Add(V_0, typeContext);
			return;
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
			return;
		}

		public AssemblySpecificContext GetAssemblyContextFromCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers)
		{
			if (!ProjectGenerationDecompilationCacheService.assemblyContextsCache.TryGetValue(this.GetModuleKey(assembly.get_MainModule(), language, renameInvalidMembers), out V_0))
			{
				throw new Exception("Key not found in AssemblyContextsCache");
			}
			return V_0;
		}

		public CachedDecompiledMember GetDecompiledMemberFromCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers)
		{
			throw new NotSupportedException("ProjectGenerationDecompilationCacheServices caches only full nested decompiled types.");
		}

		public ModuleSpecificContext GetModuleContextFromCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers)
		{
			if (!ProjectGenerationDecompilationCacheService.moduleContextsCache.TryGetValue(this.GetModuleKey(module, language, renameInvalidMembers), out V_0))
			{
				throw new Exception("Key not found in ModuleContextsCache");
			}
			return V_0;
		}

		private string GetModuleKey(ModuleDefinition module, ILanguage language, bool renameInvalidMembers)
		{
			if (!ProjectGenerationDecompilationCacheService.moduleContextsKeysMap.ContainsKey(module))
			{
				V_0 = ProjectGenerationDecompilationCacheService.locker;
				V_1 = false;
				try
				{
					Monitor.Enter(V_0, ref V_1);
					if (ProjectGenerationDecompilationCacheService.moduleContextsKeysMap.ContainsKey(module))
					{
						goto Label0;
					}
					else
					{
						V_2 = new FileInfo(module.get_FilePath());
						stackVariable17 = new String[9];
						stackVariable17[0] = module.get_FilePath();
						stackVariable17[1] = " : ";
						V_4 = V_2.get_LastWriteTime();
						stackVariable17[2] = V_4.ToString();
						stackVariable17[3] = " : ";
						stackVariable17[4] = module.get_Name().ToString();
						stackVariable17[5] = " ";
						stackVariable17[6] = language.get_Name();
						stackVariable17[7] = " ";
						stackVariable17[8] = renameInvalidMembers.ToString();
						V_3 = String.Concat(stackVariable17);
						ProjectGenerationDecompilationCacheService.moduleContextsKeysMap.Add(module, V_3);
						V_5 = V_3;
					}
				}
				finally
				{
					if (V_1)
					{
						Monitor.Exit(V_0);
					}
				}
				return V_5;
			}
		Label0:
			return ProjectGenerationDecompilationCacheService.moduleContextsKeysMap.get_Item(module);
		}

		public Dictionary<string, DecompiledType> GetNestedDecompiledTypesFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			if (!this.nestedDecompiledTypesCache.TryGetValue(this.GetTypeKey(type, language, renameInvalidMembers), out V_0))
			{
				throw new Exception("Key not found in NestedDecompiledTypesCache");
			}
			return V_0;
		}

		public TypeSpecificContext GetTypeContextFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			if (!this.typeContextsCache.TryGetValue(this.GetTypeKey(type, language, renameInvalidMembers), out V_0))
			{
				throw new Exception("Key not found in TypeContextsCache");
			}
			return V_0;
		}

		private string GetTypeKey(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			stackVariable1 = new String[5];
			stackVariable1[0] = this.GetModuleKey(type.get_Module(), language, renameInvalidMembers);
			stackVariable1[1] = " : ";
			stackVariable1[2] = language.get_Name();
			stackVariable1[3] = " : ";
			stackVariable1[4] = type.get_FullName();
			return String.Concat(stackVariable1);
		}

		public bool IsAssemblyContextInCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers)
		{
			return ProjectGenerationDecompilationCacheService.assemblyContextsCache.ContainsKey(this.GetModuleKey(assembly.get_MainModule(), language, renameInvalidMembers));
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
			dummyVar0 = this.typeContextsCache.Remove(this.GetTypeKey(type, language, renameInvalidMembers));
			return;
		}

		public void ReplaceCachedTypeContext(TypeDefinition type, ILanguage language, bool renameInvalidMembers, TypeSpecificContext newContext)
		{
			if (newContext == null)
			{
				throw new ArgumentNullException("newContext");
			}
			this.RemoveTypeContextFromCache(type, language, renameInvalidMembers);
			this.AddTypeContextToCache(type, language, renameInvalidMembers, newContext);
			return;
		}
	}
}