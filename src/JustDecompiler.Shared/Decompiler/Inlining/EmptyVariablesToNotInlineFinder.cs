using System.Collections.Generic;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
    class EmptyVariablesToNotInlineFinder : IVariablesToNotInlineFinder
    {
        public HashSet<VariableDefinition> Find(StatementCollection statements)
        {
            return new HashSet<VariableDefinition>();
        }

        public HashSet<VariableDefinition> Find(Dictionary<int, IList<Expression>> blockExpressions)
        {
            return new HashSet<VariableDefinition>();
        }
    }
}
