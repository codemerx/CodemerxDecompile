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

using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;

using CodemerxDecompile.Service.Interfaces;
using CodemerxDecompile.Service.Services.DecompilationContext.Models;

namespace CodemerxDecompile.Service.Services.DecompilationContext
{
    public class DecompilationContextService : IDecompilationContextService
    {
        private IDecompilationContext decompilationContext;

        public DecompilationContextService(IDecompilationContext decompilationContext)
        {
            this.DecompilationContext = decompilationContext;
        }

        public IDecompilationContext DecompilationContext
        {
            get => this.decompilationContext;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                this.decompilationContext = value;
            }
        }

        public void SaveAssemblyToCache(AssemblyDefinition assembly, string assemblyFilePath)
        {
            this.decompilationContext.OpenedAssemblyNamesToFilePathsMap[assembly.FullName] = assemblyFilePath;
        }

        public IEnumerable<string> GetOpenedAssemliesPaths() => this.decompilationContext.OpenedAssemblyNamesToFilePathsMap.Values;

        public bool TryGetTypeFilePathFromCache(TypeReference type, out string filePath)
        {
            KeyValuePair<string, TypeDefinition> entry = this.decompilationContext.FilePathToType.FirstOrDefault(kvp =>
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

            if (!this.decompilationContext.AssemblyStrongNameToAssemblyMetadata.TryGetValue(assemblyDefinition.FullName, out DecompiledAssemblyMetadata assemblyMetadata) ||
                !assemblyMetadata.ModuleNameToModuleMetadata.TryGetValue(moduleDefinition.Name, out DecompiledAssemblyModuleMetadata moduleMetadata) ||
                !moduleMetadata.TypeNameToTypeMetadata.TryGetValue(type.FullName, out typeMetadata))
            {
                typeMetadata = null;
                return false;
            }

            return true;
        }

        public void AddTypeMetadataToCache(TypeDefinition type, DecompiledTypeMetadata decompiledTypeMetadata)
        {
            ModuleDefinition moduleDefinition = type.Module;
            AssemblyDefinition assemblyDefinition = moduleDefinition.Assembly;

            if (!this.decompilationContext.AssemblyStrongNameToAssemblyMetadata.ContainsKey(assemblyDefinition.FullName))
            {
                this.decompilationContext.AssemblyStrongNameToAssemblyMetadata.Add(assemblyDefinition.FullName, new DecompiledAssemblyMetadata());
            }

            DecompiledAssemblyMetadata assemblyMetadataCache = this.decompilationContext.AssemblyStrongNameToAssemblyMetadata[assemblyDefinition.FullName];
            if (!assemblyMetadataCache.ModuleNameToModuleMetadata.ContainsKey(moduleDefinition.Name))
            {
                assemblyMetadataCache.ModuleNameToModuleMetadata.Add(moduleDefinition.Name, new DecompiledAssemblyModuleMetadata());
            }

            DecompiledAssemblyModuleMetadata moduleMetadataCache = assemblyMetadataCache.ModuleNameToModuleMetadata[moduleDefinition.Name];

            moduleMetadataCache.TypeNameToTypeMetadata[type.FullName] = decompiledTypeMetadata;
        }
    }
}
