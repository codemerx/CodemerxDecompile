using System.Collections.Generic;

using Telerik.JustDecompiler.Languages;
using Mono.Cecil;

using CodemerxDecompile.Service.Interfaces;

namespace CodemerxDecompile.Service.Services
{
    public class DecompilationContextService : IDecompilationContext
    {
        public DecompilationContextService()
        {
            this.MemberDeclarationToCodeSpan = new Dictionary<string, Dictionary<IMemberDefinition, CodeSpan>>();
            this.TypeToFilePathMap = new Dictionary<TypeDefinition, string>();
            this.CodeSpanToMemberReference = new Dictionary<string, Dictionary<CodeSpan, MemberReference>>();
        }

        public Dictionary<string, Dictionary<IMemberDefinition, CodeSpan>> MemberDeclarationToCodeSpan { get; set; }
        public Dictionary<TypeDefinition, string> TypeToFilePathMap { get; set; }
        public Dictionary<string, Dictionary<CodeSpan, MemberReference>> CodeSpanToMemberReference { get; set; }
    }
}
