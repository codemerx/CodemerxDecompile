using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	public class SimpleDereferencer : BaseCodeTransformer
	{
		public SimpleDereferencer()
		{
		}

		public override ICodeNode VisitUnaryExpression(UnaryExpression node)
		{
			if (node.Operator == UnaryOperator.AddressDereference)
			{
				UnaryExpression operand = node.Operand as UnaryExpression;
				if (operand != null && (operand.Operator == UnaryOperator.AddressReference || operand.Operator == UnaryOperator.AddressOf))
				{
					return this.Visit(operand.Operand);
				}
			}
			return base.VisitUnaryExpression(node);
		}
	}
}