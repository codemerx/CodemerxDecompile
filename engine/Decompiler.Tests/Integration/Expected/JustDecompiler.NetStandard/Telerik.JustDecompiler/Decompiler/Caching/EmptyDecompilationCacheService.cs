using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler.Caching
{
	public class EmptyDecompilationCacheService : IDecompilationCacheService
	{
		public EmptyDecompilationCacheService()
		{
			base();
			return;
		}

		public void AddAssemblyContextToCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers, AssemblySpecificContext assemblyContext)
		{
			return;
		}

		public void AddDecompiledMemberToCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers, CachedDecompiledMember decompiledMember)
		{
			return;
		}

		public void AddModuleContextToCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers, ModuleSpecificContext assemblyContext)
		{
			return;
		}

		public void AddNestedDecompiledTypesToCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers, Dictionary<string, DecompiledType> decompiledTypes)
		{
			return;
		}

		public void AddTypeContextToCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers, TypeSpecificContext typeContex)
		{
			return;
		}

		public bool AreNestedDecompiledTypesInCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			return false;
		}

		public AssemblySpecificContext GetAssemblyContextFromCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers)
		{
			throw new NotSupportedException("EmptyDecompilationCacheService doesn't support this method.");
		}

		public CachedDecompiledMember GetDecompiledMemberFromCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers)
		{
			throw new NotSupportedException("EmptyDecompilationCacheService doesn't support this method.");
		}

		public ModuleSpecificContext GetModuleContextFromCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers)
		{
			throw new NotSupportedException("EmptyDecompilationCacheService doesn't support this method.");
		}

		public Dictionary<string, DecompiledType> GetNestedDecompiledTypesFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			throw new NotSupportedException("EmptyDecompilationCacheService doesn't support this method.");
		}

		public TypeSpecificContext GetTypeContextFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			throw new NotSupportedException("EmptyDecompilationCacheService doesn't support this method.");
		}

		public bool IsAssemblyContextInCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers)
		{
			return false;
		}

		public bool IsDecompiledMemberInCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers)
		{
			return false;
		}

		public bool IsModuleContextInCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers)
		{
			return false;
		}

		public bool IsTypeContextInCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers)
		{
			return false;
		}

		public void ReplaceCachedTypeContext(TypeDefinition type, ILanguage language, bool renameInvalidMembers, TypeSpecificContext newContext)
		{
			return;
		}
	}
}