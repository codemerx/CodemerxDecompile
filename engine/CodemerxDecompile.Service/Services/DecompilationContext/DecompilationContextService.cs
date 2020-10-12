//    Copyright CodeMerx 2020
//    This file is part of CodemerxDecompile.

//    CodemerxDecompile is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    CodemerxDecompile is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.

//    You should have received a copy of the GNU Affero General Public License
//    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.

using System.Collections.Generic;

using Mono.Cecil;

using CodemerxDecompile.Service.Interfaces;
using CodemerxDecompile.Service.Services.DecompilationContext.Models;
using Telerik.JustDecompiler.Languages;
using System.Linq;

namespace CodemerxDecompile.Service.Services.DecompilationContext
{
    public class DecompilationContextService : IDecompilationContext
    {
        private Dictionary<string, string> openedAssemblyNamesToFilePathsMap;

        public DecompilationContextService()
        {
            this.openedAssemblyNamesToFilePathsMap = new Dictionary<string, string>();
            this.FilePathToType = new Dictionary<string, TypeDefinition>();
            this.AssemblyStrongNameToAssemblyMetadata = new Dictionary<string, DecompiledAssemblyMetadata>();
        }

        public Dictionary<string, TypeDefinition> FilePathToType { get; set; }
        public Dictionary<string, DecompiledAssemblyMetadata> AssemblyStrongNameToAssemblyMetadata { get; set; }

        public void SaveAssemblyToCache(AssemblyDefinition assembly, string assemblyFilePath)
        {
            this.openedAssemblyNamesToFilePathsMap[assembly.FullName] = assemblyFilePath;
        }

        public IEnumerable<string> GetOpenedAssemliesPaths() => this.openedAssemblyNamesToFilePathsMap.Values;

        public bool TryGetTypeFilePathFromCache(TypeReference type, out string filePath)
        {
            KeyValuePair<string, TypeDefinition> entry = this.FilePathToType.FirstOrDefault(kvp =>
            {
                TypeDefinition def = kvp.Value;

                return def.FullName == type.FullName &&
                    def.Module.Name == type.Module.Name &&
                    def.Module.Assembly.FullName == type.Module.Assembly.FullName;
            });

            filePath = entry.Key;
            return entry.Key != null;
        }

        public bool TryGetTypeMetadataFromCache(TypeReference type, out DecompiledTypeMetadata typeMetadata)
        {
            ModuleDefinition moduleDefinition = type.Module;
            AssemblyDefinition assemblyDefinition = moduleDefinition.Assembly;

            if (!this.AssemblyStrongNameToAssemblyMetadata.TryGetValue(assemblyDefinition.FullName, out DecompiledAssemblyMetadata assemblyMetadata) ||
                !assemblyMetadata.ModuleNameToModuleMetadata.TryGetValue(moduleDefinition.Name, out DecompiledAssemblyModuleMetadata moduleMetadata) ||
                !moduleMetadata.TypeNameToTypeMetadata.TryGetValue(type.FullName, out typeMetadata))
            {
                typeMetadata = null;
                return false;
            }

            return true;
        }

        public void AddTypeMetadataToCache(TypeDefinition type, Dictionary<IMemberDefinition, CodeSpan> memberDeclarationToCodeSpan, Dictionary<CodeSpan, MemberReference> codeSpanToMemberReference, CodeMappingInfo<CodeSpan> codeMappingInfo)
        {
            ModuleDefinition moduleDefinition = type.Module;
            AssemblyDefinition assemblyDefinition = moduleDefinition.Assembly;

            if (!this.AssemblyStrongNameToAssemblyMetadata.ContainsKey(assemblyDefinition.FullName))
            {
                this.AssemblyStrongNameToAssemblyMetadata.Add(assemblyDefinition.FullName, new DecompiledAssemblyMetadata());
            }

            DecompiledAssemblyMetadata assemblyMetadataCache = this.AssemblyStrongNameToAssemblyMetadata[assemblyDefinition.FullName];
            if (!assemblyMetadataCache.ModuleNameToModuleMetadata.ContainsKey(moduleDefinition.Name))
            {
                assemblyMetadataCache.ModuleNameToModuleMetadata.Add(moduleDefinition.Name, new DecompiledAssemblyModuleMetadata());
            }

            DecompiledAssemblyModuleMetadata moduleMetadataCache = assemblyMetadataCache.ModuleNameToModuleMetadata[moduleDefinition.Name];

            if (!moduleMetadataCache.TypeNameToTypeMetadata.ContainsKey(type.FullName))
            {
                moduleMetadataCache.TypeNameToTypeMetadata.Add(type.FullName, new DecompiledTypeMetadata());
            }

            DecompiledTypeMetadata typeMetadataCache = moduleMetadataCache.TypeNameToTypeMetadata[type.FullName];

            typeMetadataCache.MemberDeclarationToCodeSpan = memberDeclarationToCodeSpan;
            typeMetadataCache.CodeSpanToMemberReference = codeSpanToMemberReference;
            typeMetadataCache.CodeMappingInfo = codeMappingInfo;
        }
    }
}
