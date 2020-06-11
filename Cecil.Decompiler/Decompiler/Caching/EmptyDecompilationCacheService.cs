using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.JustDecompiler.Languages;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Decompiler.Caching
{
	public class EmptyDecompilationCacheService : IDecompilationCacheService
	{
		public bool IsAssemblyContextInCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers)
		{
			return false;
		}

		public AssemblySpecificContext GetAssemblyContextFromCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers)
		{
			throw new NotSupportedException("EmptyDecompilationCacheService doesn't support this method.");
		}

		public void AddAssemblyContextToCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers, AssemblySpecificContext assemblyContext)
		{
		}

		public bool IsModuleContextInCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers)
		{
			return false;
		}

		public bool IsTypeContextInCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			return false;
		}

		public bool IsDecompiledMemberInCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers)
		{
			return false;
		}

		public bool AreNestedDecompiledTypesInCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			return false;
		}

		public ModuleSpecificContext GetModuleContextFromCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers)
		{
			throw new NotSupportedException("EmptyDecompilationCacheService doesn't support this method.");
		}

		public TypeSpecificContext GetTypeContextFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			throw new NotSupportedException("EmptyDecompilationCacheService doesn't support this method.");
		}

		public CachedDecompiledMember GetDecompiledMemberFromCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers)
		{
			throw new NotSupportedException("EmptyDecompilationCacheService doesn't support this method.");
		}

		public Dictionary<string, DecompiledType> GetNestedDecompiledTypesFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			throw new NotSupportedException("EmptyDecompilationCacheService doesn't support this method.");
		}

		public void AddModuleContextToCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers, ModuleSpecificContext assemblyContext)
		{
		}

		public void AddTypeContextToCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers, TypeSpecificContext typeContex)
		{
		}

		public void AddDecompiledMemberToCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers, CachedDecompiledMember decompiledMember)
		{
		}

		public void AddNestedDecompiledTypesToCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers, Dictionary<string, DecompiledType> decompiledTypes)
		{
		}

        public void ReplaceCachedTypeContext(TypeDefinition type, ILanguage language, bool renameInvalidMembers, TypeSpecificContext newContext)
        {
        }
    }
}
