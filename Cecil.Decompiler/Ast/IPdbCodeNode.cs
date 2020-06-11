using System;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Ast
{
    public interface IPdbCodeNode : ICodeNode
    {
        MethodDefinition UnderlyingInstructionsMember { get; }
    }
}
