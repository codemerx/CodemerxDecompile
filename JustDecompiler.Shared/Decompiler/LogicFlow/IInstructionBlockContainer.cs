using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
    interface IInstructionBlockContainer
    {
        InstructionBlock TheBlock { get; }
        List<Telerik.JustDecompiler.Ast.Expressions.Expression> LogicalConstructExpressions { get; }
    }
}
