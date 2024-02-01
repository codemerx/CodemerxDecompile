using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Steps
{
	internal class VisualBasicRemoveUnusedVariablesStep : RemoveUnusedVariablesStep
	{
		public VisualBasicRemoveUnusedVariablesStep()
		{
		}

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
			Expression target = (expression as MethodInvocationExpression).GetTarget();
			if (target == null)
			{
				return true;
			}
			if (target.IsArgumentReferenceToRefParameter())
			{
				return true;
			}
			return this.context.Language.IsValidLineStarter(target.CodeNodeType);
		}
	}
}