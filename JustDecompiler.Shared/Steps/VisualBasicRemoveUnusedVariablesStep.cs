using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
    class VisualBasicRemoveUnusedVariablesStep : RemoveUnusedVariablesStep
    {
        protected override bool CanExistInStatement(Expression expression)
        {
            if (!base.CanExistInStatement(expression))
            {
                return false;
            }

            if (expression.CodeNodeType != CodeNodeType.MethodInvocationExpression)
            {
                return true;
            }

            Expression methodTarget = (expression as MethodInvocationExpression).GetTarget();
            if (methodTarget == null)
            {
                return true;
            }

            if (methodTarget.IsArgumentReferenceToRefParameter())
            {
                return true;
            }

            return this.context.Language.IsValidLineStarter(methodTarget.CodeNodeType);
        }
    }
}
