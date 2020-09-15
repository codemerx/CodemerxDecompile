using System.Collections.Generic;

using Mono.Cecil;
using Telerik.JustDecompiler.Languages;

using CodemerxDecompile.Service.Services.DecompilationContext.Models;

namespace CodemerxDecompile.Service.Interfaces
{
    public interface IDecompilationContext 
    {
        Dictionary<string, TypeDefinition> FilePathToType { get; set; }

        bool TryGetTypeFilePathFromCache(TypeReference type, out string filePath);

        bool TryGetTypeMetadataFromCache(TypeReference type, out DecompiledTypeMetadata typeMetadata);

        void AddTypeMetadataToCache(TypeDefinition type, Dictionary<IMemberDefinition, CodeSpan> memberDeclarationToCodeSpan, Dictionary<CodeSpan, MemberReference> codeSpanToMemberReference);
    }
}
