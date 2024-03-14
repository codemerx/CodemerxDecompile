using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler.Caching
{
	public interface IDecompilationCacheService
	{
		void AddAssemblyContextToCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers, AssemblySpecificContext assemblyContext);

		void AddDecompiledMemberToCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers, CachedDecompiledMember decompiledMember);

		void AddModuleContextToCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers, ModuleSpecificContext moduleContex);

		void AddNestedDecompiledTypesToCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers, Dictionary<string, DecompiledType> decompiledTypes);

		void AddTypeContextToCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers, TypeSpecificContext typeContex);

		bool AreNestedDecompiledTypesInCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers);

		AssemblySpecificContext GetAssemblyContextFromCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers);

		CachedDecompiledMember GetDecompiledMemberFromCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers);

		ModuleSpecificContext GetModuleContextFromCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers);

		Dictionary<string, DecompiledType> GetNestedDecompiledTypesFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers);

		TypeSpecificContext GetTypeContextFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers);

		bool IsAssemblyContextInCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers);

		bool IsDecompiledMemberInCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers);

		bool IsModuleContextInCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers);

		bool IsTypeContextInCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers);

		void ReplaceCachedTypeContext(TypeDefinition type, ILanguage language, bool renameInvalidMembers, TypeSpecificContext newContext);
	}
}