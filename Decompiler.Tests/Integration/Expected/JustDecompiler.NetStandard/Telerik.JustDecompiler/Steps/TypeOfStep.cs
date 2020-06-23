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
		}

		public ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			TypeReference typeReference;
			if (!node.IsTypeOfExpression(out typeReference))
			{
				return null;
			}
			return new TypeOfExpression(typeReference, node.UnderlyingSameMethodInstructions);
		}
	}
}