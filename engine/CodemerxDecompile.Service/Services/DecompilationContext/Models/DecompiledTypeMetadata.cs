using System.Collections.Generic;

using Mono.Cecil;
using Telerik.JustDecompiler.Languages;

namespace CodemerxDecompile.Service.Services.DecompilationContext.Models
{
    public class DecompiledTypeMetadata
    {
        public Dictionary<IMemberDefinition, CodeSpan> MemberDeclarationToCodeSpan { get; set; }

        public Dictionary<CodeSpan, MemberReference> CodeSpanToMemberReference { get; set; }
    }
}
