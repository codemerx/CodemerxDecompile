using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler.Caching
{
	public class DefaultDecompilationCacheService : IDecompilationCacheService
	{
		private const int AssemblyContextsCacheSize = 10;

		private const int ModuleContextsCacheSize = 10;

		private const int TypeContextsCacheSize = 10;

		private const int DecompiledMembersCacheSize = 30;

		private static DecompilationCache<string, AssemblySpecificContext> assemblyContextsCache;

		private static DecompilationCache<string, ModuleSpecificContext> moduleContextsCache;

		private static DecompilationCache<string, TypeSpecificContext> typeContextsCache;

		private static DecompilationCache<string, CachedDecompiledMember> decompiledMembersCache;

		static DefaultDecompilationCacheService()
		{
			DefaultDecompilationCacheService.assemblyContextsCache = new DecompilationCache<string, AssemblySpecificContext>(10);
			DefaultDecompilationCacheService.moduleContextsCache = new DecompilationCache<string, ModuleSpecificContext>(10);
			DefaultDecompilationCacheService.typeContextsCache = new DecompilationCache<string, TypeSpecificContext>(10);
			DefaultDecompilationCacheService.decompiledMembersCache = new DecompilationCache<string, CachedDecompiledMember>(30);
			return;
		}

		public DefaultDecompilationCacheService()
		{
			base();
			return;
		}

		public void AddAssemblyContextToCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers, AssemblySpecificContext assemblyContext)
		{
			DefaultDecompilationCacheService.assemblyContextsCache.Add(this.GetModuleKey(assembly.get_MainModule(), language, renameInvalidMembers), assemblyContext);
			return;
		}

		public virtual void AddDecompiledMemberToCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers, CachedDecompiledMember decompiledMember)
		{
			DefaultDecompilationCacheService.decompiledMembersCache.Add(this.GetMemberKey(member, language, renameInvalidMembers), decompiledMember);
			return;
		}

		public virtual void AddModuleContextToCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers, ModuleSpecificContext moduleContext)
		{
			DefaultDecompilationCacheService.moduleContextsCache.Add(this.GetModuleKey(module, language, renameInvalidMembers), moduleContext);
			return;
		}

		public void AddNestedDecompiledTypesToCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers, Dictionary<string, DecompiledType> decompiledTypes)
		{
			return;
		}

		public virtual void AddTypeContextToCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers, TypeSpecificContext typeContext)
		{
			DefaultDecompilationCacheService.typeContextsCache.Add(this.GetTypeKey(type, language, renameInvalidMembers), typeContext);
			return;
		}

		public bool AreNestedDecompiledTypesInCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			return false;
		}

		public static void ClearCaches()
		{
			DefaultDecompilationCacheService.assemblyContextsCache.Clear();
			DefaultDecompilationCacheService.moduleContextsCache.Clear();
			DefaultDecompilationCacheService.typeContextsCache.Clear();
			DefaultDecompilationCacheService.decompiledMembersCache.Clear();
			return;
		}

		public AssemblySpecificContext GetAssemblyContextFromCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers)
		{
			return DefaultDecompilationCacheService.assemblyContextsCache.Get(this.GetModuleKey(assembly.get_MainModule(), language, renameInvalidMembers));
		}

		public virtual CachedDecompiledMember GetDecompiledMemberFromCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers)
		{
			return DefaultDecompilationCacheService.decompiledMembersCache.Get(this.GetMemberKey(member, language, renameInvalidMembers));
		}

		private string GetMemberKey(IMemberDefinition member, ILanguage language, bool renameInvalidMembers)
		{
			stackVariable1 = new String[5];
			stackVariable1[0] = this.GetModuleKey(member.get_DeclaringType().get_Module(), language, renameInvalidMembers);
			stackVariable1[1] = " : ";
			stackVariable1[2] = language.get_Name();
			stackVariable1[3] = " : ";
			stackVariable1[4] = Utilities.GetMemberUniqueName(member);
			return String.Concat(stackVariable1);
		}

		public virtual ModuleSpecificContext GetModuleContextFromCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers)
		{
			return DefaultDecompilationCacheService.moduleContextsCache.Get(this.GetModuleKey(module, language, renameInvalidMembers));
		}

		private string GetModuleKey(ModuleDefinition module, ILanguage language, bool renameInvalidMembers)
		{
			V_0 = new FileInfo(module.get_FilePath());
			stackVariable4 = new String[9];
			stackVariable4[0] = module.get_FilePath();
			stackVariable4[1] = " : ";
			V_1 = V_0.get_LastWriteTime();
			stackVariable4[2] = V_1.ToString();
			stackVariable4[3] = " : ";
			stackVariable4[4] = module.get_Name().ToString();
			stackVariable4[5] = " ";
			stackVariable4[6] = language.get_Name();
			stackVariable4[7] = " ";
			stackVariable4[8] = renameInvalidMembers.ToString();
			return String.Concat(stackVariable4);
		}

		public Dictionary<string, DecompiledType> GetNestedDecompiledTypesFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			throw new NotSupportedException("DefaultDecompilationCacheService does not cache nested decompile types.");
		}

		public virtual TypeSpecificContext GetTypeContextFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			return DefaultDecompilationCacheService.typeContextsCache.Get(this.GetTypeKey(type, language, renameInvalidMembers));
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
			return DefaultDecompilationCacheService.assemblyContextsCache.ContainsKey(this.GetModuleKey(assembly.get_MainModule(), language, renameInvalidMembers));
		}

		public virtual bool IsDecompiledMemberInCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers)
		{
			V_0 = DefaultDecompilationCacheService.decompiledMembersCache.ContainsKey(this.GetMemberKey(member, language, renameInvalidMembers));
			if (V_0)
			{
				V_1 = this.GetDecompiledMemberFromCache(member, language, renameInvalidMembers).get_Member().get_Context().get_AnalysisResults().get_AmbiguousCastsToObject().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						if (!Extensions.ResolveToOverloadedEqualityOperator(V_1.get_Current().get_Expression().get_ExpressionType(), out V_2).get_HasValue())
						{
							continue;
						}
						this.RemoveDecompiledMemberFromCache(member, language, renameInvalidMembers);
						V_0 = false;
						goto Label0;
					}
				}
				finally
				{
					((IDisposable)V_1).Dispose();
				}
			}
		Label0:
			return V_0;
		}

		public virtual bool IsModuleContextInCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers)
		{
			return DefaultDecompilationCacheService.moduleContextsCache.ContainsKey(this.GetModuleKey(module, language, renameInvalidMembers));
		}

		public virtual bool IsTypeContextInCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			return DefaultDecompilationCacheService.typeContextsCache.ContainsKey(this.GetTypeKey(type, language, renameInvalidMembers));
		}

		private void RemoveDecompiledMemberFromCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers)
		{
			DefaultDecompilationCacheService.decompiledMembersCache.Remove(this.GetMemberKey(member, language, renameInvalidMembers));
			return;
		}

		private void RemoveTypeContextFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			DefaultDecompilationCacheService.typeContextsCache.Remove(this.GetTypeKey(type, language, renameInvalidMembers));
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