using System;
using System.Linq;
using Mono.Cecil;
using Telerik.JustDecompiler.Languages;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Decompiler.Caching
{
	public interface IDecompilationCacheService
	{
		bool IsAssemblyContextInCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers);
		bool IsModuleContextInCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers);
		bool IsTypeContextInCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers);
		bool IsDecompiledMemberInCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers);
		bool AreNestedDecompiledTypesInCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers);

		AssemblySpecificContext GetAssemblyContextFromCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers);
		ModuleSpecificContext GetModuleContextFromCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers);
		TypeSpecificContext GetTypeContextFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers);
		CachedDecompiledMember GetDecompiledMemberFromCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers);
		Dictionary<string, DecompiledType> GetNestedDecompiledTypesFromCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers);

		void AddAssemblyContextToCache(AssemblyDefinition assembly, ILanguage language, bool renameInvalidMembers, AssemblySpecificContext assemblyContext);
		void AddModuleContextToCache(ModuleDefinition module, ILanguage language, bool renameInvalidMembers, ModuleSpecificContext moduleContex);
		void AddTypeContextToCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers, TypeSpecificContext typeContex);
		void AddDecompiledMemberToCache(IMemberDefinition member, ILanguage language, bool renameInvalidMembers, CachedDecompiledMember decompiledMember);
		void AddNestedDecompiledTypesToCache(TypeDefinition type, ILanguage language, bool renameInvalidMembers, Dictionary<string, DecompiledType> decompiledTypes);

        void ReplaceCachedTypeContext(TypeDefinition type, ILanguage language, bool renameInvalidMembers, TypeSpecificContext newContext);
	}
}
