using System.Collections.Generic;
using Mono.Cecil;
using Telerik.JustDecompiler.Languages;

namespace CodemerxDecompile;

public class DecompiledTypeMetadata
{
    public Dictionary<IMemberDefinition, CodeSpan> MemberDeclarationToCodeSpan { get; } = new();
    public Dictionary<IMemberDefinition, OffsetSpan> MemberDeclarationToCodePostionMap { get; } = new();
    public Dictionary<OffsetSpan, MemberReference> CodeSpanToMemberReference { get; } = new();
    public CodeMappingInfo<CodeSpan> CodeMappingInfo { get; } = new();
}
