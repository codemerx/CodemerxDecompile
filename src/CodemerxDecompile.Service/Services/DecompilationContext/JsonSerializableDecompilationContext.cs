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
using CodemerxDecompile.Service.Interfaces;
using CodemerxDecompile.Service.Services.DecompilationContext.Models;
using Mono.Cecil;
using Newtonsoft.Json;

namespace CodemerxDecompile.Service.Services.DecompilationContext
{
    internal class JsonSerializableDecompilationContext : IDecompilationContext
    {
        public Dictionary<string, string> OpenedAssemblyNamesToFilePathsMap { get; } = new Dictionary<string, string>();

        [JsonIgnore]
        public Dictionary<string, TypeDefinition> FilePathToType { get; } = new Dictionary<string, TypeDefinition>();

        [JsonIgnore]
        public Dictionary<string, DecompiledAssemblyMetadata> AssemblyStrongNameToAssemblyMetadata { get; } = new Dictionary<string, DecompiledAssemblyMetadata>();
    }
}