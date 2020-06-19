using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Telerik.JustDecompiler.Languages;
using System.IO;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.Caching
{
	public class DefaultDecompilationCacheService : IDecompilationCacheService
	{
		private const int AssemblyContextsCacheSize = 10;
		private const int ModuleContextsCacheSize = 10;
		private const int TypeContextsCacheSize = 10;
		private const int DecompiledMembersCacheSize = 30;

		private static DecompilationCache<string, AssemblySpecificContext> assemblyContextsCache = new DecompilationCache<string, AssemblySpecificContext>(AssemblyContextsCacheSize);
		private static DecompilationCache<string, ModuleSpecificContext> moduleContextsCache = new DecompilationCache<string, ModuleSpecificContext>(ModuleContextsCacheSize);
		private static DecompilationCache<string, TypeSpecificContext> typeContextsCache = new DecompilationCache<string, TypeSpecificContext>(TypeContextsCacheSize);
		private static DecompilationCache<string, CachedDecompiledMember> decompiledMembersCache = new DecompilationCache<string, CachedDecompiledMember>(DecompiledMembersCacheSize);

		public bool IsAssemblyContextInCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers)
		{
			return assemblyContextsCache.ContainsKey(GetModuleKey(assembly.MainModule, language, renameInvalidMembers));
		}

		public AssemblySpecificContext GetAssemblyContextFromCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers)
		{
			return assemblyContextsCache.Get(GetModuleKey(assembly.MainModule, language, renameInvalidMembers));
		}

		public void AddAssemblyContextToCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers, AssemblySpecificContext assemblyContext)
		{
			assemblyContextsCache.Add(GetModuleKey(assembly.MainModule, language, renameInvalidMembers), assemblyContext);
		}

		public bool AreNestedDecompiledTypesInCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			return false;
		}

		public Dictionary<string, DecompiledType> GetNestedDecompiledTypesFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			throw new NotSupportedException("DefaultDecompilationCacheService does not cache nested decompile types.");
		}

		public void AddNestedDecompiledTypesToCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers, Dictionary<string, DecompiledType> decompiledTypes)
		{
			return;
		}

		private string GetModuleKey(ModuleDefinition module, ILanguage language, bool renameInvalidMembers)
		{
			FileInfo fileInfo = new FileInfo(module.FilePath);
			return module.FilePath + " : " + fileInfo.LastWriteTime + " : " + module.Name.ToString() + " " + language.Name + " " + renameInvalidMembers.ToString();
		}

		private string GetTypeKey(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			return GetModuleKey(type.Module, language, renameInvalidMembers) + " : " + language.Name + " : " + type.FullName;
		}

		private string GetMemberKey(IMemberDefinition member, ILanguage language, bool renameInvalidMembers)
		{
			return GetModuleKey(member.DeclaringType.Module, language, renameInvalidMembers) + " : " + language.Name + " : " + Utilities.GetMemberUniqueName(member);
		}

		public virtual bool IsModuleContextInCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers)
		{
			return moduleContextsCache.ContainsKey(GetModuleKey(module, language, renameInvalidMembers));
		}

		public virtual bool IsTypeContextInCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			return typeContextsCache.ContainsKey(GetTypeKey(type, language, renameInvalidMembers));
		}

		public virtual bool IsDecompiledMemberInCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers)
		{
            bool isDecompiledMemberInCache = decompiledMembersCache.ContainsKey(GetMemberKey(member, language, renameInvalidMembers));
            if (isDecompiledMemberInCache)
            {
                CachedDecompiledMember cachedDecompiledMember = this.GetDecompiledMemberFromCache(member, language, renameInvalidMembers);
                foreach (ExplicitCastExpression cast in cachedDecompiledMember.Member.Context.AnalysisResults.AmbiguousCastsToObject)
                {
                    TypeReference lastResolvedType;
                    bool? haveOverloadedOperator = Common.Extensions.ResolveToOverloadedEqualityOperator(cast.Expression.ExpressionType, out lastResolvedType);
                    bool isResolved = haveOverloadedOperator.HasValue;
                    if (isResolved)
                    {
                        this.RemoveDecompiledMemberFromCache(member, language, renameInvalidMembers);
                        isDecompiledMemberInCache = false;

                        break;
                    }
                }
            }
            
            return isDecompiledMemberInCache;
		}

		public virtual ModuleSpecificContext GetModuleContextFromCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers)
		{
			return moduleContextsCache.Get(GetModuleKey(module, language, renameInvalidMembers));
		}

		public virtual TypeSpecificContext GetTypeContextFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			return typeContextsCache.Get(GetTypeKey(type, language, renameInvalidMembers));
		}

		public virtual CachedDecompiledMember GetDecompiledMemberFromCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers)
		{
			return decompiledMembersCache.Get(GetMemberKey(member, language, renameInvalidMembers));
		}

		public virtual void AddModuleContextToCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers, ModuleSpecificContext moduleContext)
		{
			moduleContextsCache.Add(GetModuleKey(module, language, renameInvalidMembers), moduleContext);
		}

		public virtual void AddTypeContextToCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers, TypeSpecificContext typeContext)
		{
			typeContextsCache.Add(GetTypeKey(type, language, renameInvalidMembers), typeContext);
		}

		public virtual void AddDecompiledMemberToCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers, CachedDecompiledMember decompiledMember)
		{
			decompiledMembersCache.Add(GetMemberKey(member, language, renameInvalidMembers), decompiledMember);
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

        private void RemoveDecompiledMemberFromCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers)
        {
            decompiledMembersCache.Remove(GetMemberKey(member, language, renameInvalidMembers));
        }

        private void RemoveTypeContextFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
        {
            typeContextsCache.Remove(GetTypeKey(type, language, renameInvalidMembers));
        }

		public static void ClearCaches()
		{
			assemblyContextsCache.Clear();
			moduleContextsCache.Clear();
			typeContextsCache.Clear();
			decompiledMembersCache.Clear();
		}
    }
}
