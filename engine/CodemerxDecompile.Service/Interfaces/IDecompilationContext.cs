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
using Telerik.JustDecompiler.Languages;

using CodemerxDecompile.Service.Services.DecompilationContext.Models;

namespace CodemerxDecompile.Service.Interfaces
{
    public interface IDecompilationContext 
    {
        Dictionary<string, TypeDefinition> FilePathToType { get; set; }

        void SaveAssemblyToCache(AssemblyDefinition assembly, string assemblyFilePath);

        IEnumerable<string> GetOpenedAssemliesPaths();

        bool TryGetTypeFilePathFromCache(TypeReference type, out string filePath);

        bool TryGetTypeMetadataFromCache(TypeReference type, out DecompiledTypeMetadata typeMetadata);

        void AddTypeMetadataToCache(TypeDefinition type, Dictionary<IMemberDefinition, CodeSpan> memberDeclarationToCodeSpan, Dictionary<CodeSpan, MemberReference> codeSpanToMemberReference, CodeMappingInfo<CodeSpan> codeMappingInfo);
    }
}
