using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Languages;

namespace CodemerxDecompile.Service.Interfaces
{
    public interface IDecompilationContext 
    {
        Dictionary<string, Dictionary<IMemberDefinition, CodeSpan>> MemberDeclarationToCodeSpan { get; set; }
        Dictionary<TypeDefinition, string> TypeToFilePathMap { get; set; }
        Dictionary<string, Dictionary<CodeSpan, MemberReference>> CodeSpanToMemberReference { get; set; }
    }
}
