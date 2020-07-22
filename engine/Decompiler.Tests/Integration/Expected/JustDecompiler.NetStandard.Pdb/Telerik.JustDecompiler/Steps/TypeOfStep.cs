using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	public class TypeOfStep
	{
		public TypeOfStep()
		{
			base();
			return;
		}

		public ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			if (!node.IsTypeOfExpression(out V_0))
			{
				return null;
			}
			return new TypeOfExpression(V_0, node.get_UnderlyingSameMethodInstructions());
		}
	}
}