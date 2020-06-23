using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Common;
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
		}

		public DefaultDecompilationCacheService()
		{
		}

		public void AddAssemblyContextToCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers, AssemblySpecificContext assemblyContext)
		{
			DefaultDecompilationCacheService.assemblyContextsCache.Add(this.GetModuleKey(assembly.MainModule, language, renameInvalidMembers), assemblyContext);
		}

		public virtual void AddDecompiledMemberToCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers, CachedDecompiledMember decompiledMember)
		{
			DefaultDecompilationCacheService.decompiledMembersCache.Add(this.GetMemberKey(member, language, renameInvalidMembers), decompiledMember);
		}

		public virtual void AddModuleContextToCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers, ModuleSpecificContext moduleContext)
		{
			DefaultDecompilationCacheService.moduleContextsCache.Add(this.GetModuleKey(module, language, renameInvalidMembers), moduleContext);
		}

		public void AddNestedDecompiledTypesToCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers, Dictionary<string, DecompiledType> decompiledTypes)
		{
		}

		public virtual void AddTypeContextToCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers, TypeSpecificContext typeContext)
		{
			DefaultDecompilationCacheService.typeContextsCache.Add(this.GetTypeKey(type, language, renameInvalidMembers), typeContext);
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
		}

		public AssemblySpecificContext GetAssemblyContextFromCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers)
		{
			return DefaultDecompilationCacheService.assemblyContextsCache.Get(this.GetModuleKey(assembly.MainModule, language, renameInvalidMembers));
		}

		public virtual CachedDecompiledMember GetDecompiledMemberFromCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers)
		{
			return DefaultDecompilationCacheService.decompiledMembersCache.Get(this.GetMemberKey(member, language, renameInvalidMembers));
		}

		private string GetMemberKey(IMemberDefinition member, ILanguage language, bool renameInvalidMembers)
		{
			return String.Concat(new String[] { this.GetModuleKey(member.DeclaringType.Module, language, renameInvalidMembers), " : ", language.Name, " : ", Utilities.GetMemberUniqueName(member) });
		}

		public virtual ModuleSpecificContext GetModuleContextFromCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers)
		{
			return DefaultDecompilationCacheService.moduleContextsCache.Get(this.GetModuleKey(module, language, renameInvalidMembers));
		}

		private string GetModuleKey(ModuleDefinition module, ILanguage language, bool renameInvalidMembers)
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
			return String.Concat(filePath);
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
			return String.Concat(new String[] { this.GetModuleKey(type.Module, language, renameInvalidMembers), " : ", language.Name, " : ", type.FullName });
		}

		public bool IsAssemblyContextInCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers)
		{
			return DefaultDecompilationCacheService.assemblyContextsCache.ContainsKey(this.GetModuleKey(assembly.MainModule, language, renameInvalidMembers));
		}

		public virtual bool IsDecompiledMemberInCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers)
		{
			TypeReference typeReference;
			bool flag = DefaultDecompilationCacheService.decompiledMembersCache.ContainsKey(this.GetMemberKey(member, language, renameInvalidMembers));
			if (flag)
			{
				foreach (ExplicitCastExpression ambiguousCastsToObject in this.GetDecompiledMemberFromCache(member, language, renameInvalidMembers).Member.Context.AnalysisResults.AmbiguousCastsToObject)
				{
					if (!Telerik.JustDecompiler.Common.Extensions.ResolveToOverloadedEqualityOperator(ambiguousCastsToObject.Expression.ExpressionType, out typeReference).HasValue)
					{
						continue;
					}
					this.RemoveDecompiledMemberFromCache(member, language, renameInvalidMembers);
					flag = false;
					return flag;
				}
			}
			return flag;
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
		}

		private void RemoveTypeContextFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			DefaultDecompilationCacheService.typeContextsCache.Remove(this.GetTypeKey(type, language, renameInvalidMembers));
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