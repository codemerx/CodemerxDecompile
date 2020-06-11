using System;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
    abstract class BaseUsageFinder : BaseCodeVisitor
    {
        protected UsageFinderSearchResult searchResult;

        public UsageFinderSearchResult SearchForUsage(Expression expression)
        {
            this.searchResult = UsageFinderSearchResult.NotFound;
            Visit(expression);
            return this.searchResult;
        }

        public override void Visit(ICodeNode node)
        {
            if (this.searchResult == UsageFinderSearchResult.NotFound)
            {
                base.Visit(node);
            }
        }

        public override void VisitMethodInvocationExpression(Ast.Expressions.MethodInvocationExpression node)
        {
            MethodDefinition methodDef = node.MethodExpression.MethodDefinition;
            if (methodDef == null)
            {
                base.VisitMethodInvocationExpression(node);
                return;
            }

            Visit(node.MethodExpression);

            for (int i = 0; i < node.Arguments.Count; i++)
            {
                UnaryExpression unaryArgument = node.Arguments[i] as UnaryExpression;
                if (methodDef.Parameters[i].IsOutParameter() && (unaryArgument != null && unaryArgument.Operator == UnaryOperator.AddressReference &&
                     CheckExpression(unaryArgument.Operand) || CheckExpression(node.Arguments[i])))
                {
                    this.searchResult = UsageFinderSearchResult.Assigned;
                    return;
                }
                else
                {
                    Visit(node.Arguments[i]);
                    if (this.searchResult != UsageFinderSearchResult.NotFound)
                    {
                        return;
                    }
                }
            }
        }

        public override void VisitBinaryExpression(BinaryExpression node)
        {
            if (!node.IsAssignmentExpression)
            {
                base.VisitBinaryExpression(node);
                return;
            }

            Visit(node.Right);
            if (this.searchResult != UsageFinderSearchResult.NotFound)
            {
                return;
            }

            if (CheckExpression(node.Left))
            {
                this.searchResult = UsageFinderSearchResult.Assigned;
            }
            else
            {
                base.Visit(node.Left);
            }
        }

        public abstract bool CheckExpression(Expression node);
    }
}
