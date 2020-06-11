using Mono.Cecil.Cil;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
    public interface IVariablesToNotInlineFinder
    {
        HashSet<VariableDefinition> Find(Dictionary<int, IList<Expression>> blockExpressions);

        HashSet<VariableDefinition> Find(StatementCollection statements);
    }
}
