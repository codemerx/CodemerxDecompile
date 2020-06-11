using System;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
    class OutParameterUsageFinder : BaseUsageFinder
    {
        private readonly ParameterDefinition parameter;

        public OutParameterUsageFinder(ParameterDefinition parameter)
        {
            this.parameter = parameter;
        }

        public override bool CheckExpression(Expression node)
        {
            return node.CodeNodeType == CodeNodeType.ArgumentReferenceExpression && CheckArgumentReference(node as ArgumentReferenceExpression) ||
                node.CodeNodeType == CodeNodeType.UnaryExpression && (node as UnaryExpression).Operator == UnaryOperator.AddressDereference &&
                CheckArgumentReference((node as UnaryExpression).Operand as ArgumentReferenceExpression);
        }

        private bool CheckArgumentReference(ArgumentReferenceExpression node)
        {
            return node != null && node.Parameter.Resolve() == parameter;
        }

        public override void VisitArgumentReferenceExpression(ArgumentReferenceExpression node)
        {
            if (node.Parameter.Resolve() == parameter)
            {
                this.searchResult = UsageFinderSearchResult.Used;
            }
        }

        public override void VisitReturnExpression(ReturnExpression node)
        {
            Visit(node.Value);
            if(this.searchResult == UsageFinderSearchResult.NotFound)
            {
                this.searchResult = UsageFinderSearchResult.Used;
            }
        }
    }
}
