using System;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Common
{
    internal class VariableFinder : BaseCodeVisitor
    {
        private readonly VariableReference variable;
        private bool found;

        public VariableFinder(VariableReference variable)
        {
            this.variable = variable;
        }

        public bool FindVariable(ICodeNode node)
        {
            found = false;
            Visit(node);
            return found;
        }

        public override void Visit(ICodeNode node)
        {
            if (!found)
            {
                base.Visit(node);
            }
        }

        public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
        {
            if (node.Variable == variable)
            {
                found = true;
                return;
            }
            base.VisitVariableDeclarationExpression(node);
        }

        public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
        {
            if (node.Variable == variable)
            {
                found = true;
                return;
            }
            base.VisitVariableReferenceExpression(node);
        }
    }
}
