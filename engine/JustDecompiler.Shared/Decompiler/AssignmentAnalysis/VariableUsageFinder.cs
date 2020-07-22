using System;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
    class VariableUsageFinder : BaseUsageFinder
    {
        private readonly VariableDefinition variable;

        public VariableUsageFinder(VariableDefinition variable)
        {
            this.variable = variable;
        }

        public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
        {
            if (node.Variable.Resolve() == variable)
            {
                this.searchResult = UsageFinderSearchResult.Used;
            }
        }

        public override bool CheckExpression(Expression node)
        {
            return node.CodeNodeType == Ast.CodeNodeType.VariableReferenceExpression &&
                (node as VariableReferenceExpression).Variable.Resolve() == variable;
        }
    }
}
