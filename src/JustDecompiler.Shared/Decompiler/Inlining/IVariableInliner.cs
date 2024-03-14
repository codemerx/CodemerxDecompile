using System;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
    interface IVariableInliner
    {
        bool TryInlineVariable(VariableDefinition variableDef, Expression value, ICodeNode target, bool aggressive, out ICodeNode result);
    }
}
