using System;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	internal class VisualBasicRemoveUnusedVariablesStep : RemoveUnusedVariablesStep
	{
		public VisualBasicRemoveUnusedVariablesStep()
		{
			base();
			return;
		}

		protected override bool CanExistInStatement(Expression expression)
		{
			if (!this.CanExistInStatement(expression))
			{
				return false;
			}
			if (expression.get_CodeNodeType() != 19)
			{
				return true;
			}
			V_0 = (expression as MethodInvocationExpression).GetTarget();
			if (V_0 == null)
			{
				return true;
			}
			if (V_0.IsArgumentReferenceToRefParameter())
			{
				return true;
			}
			return this.context.get_Language().IsValidLineStarter(V_0.get_CodeNodeType());
		}
	}
}