using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	public class SimpleDereferencer : BaseCodeTransformer
	{
		public SimpleDereferencer()
		{
			base();
			return;
		}

		public override ICodeNode VisitUnaryExpression(UnaryExpression node)
		{
			if (node.get_Operator() == 8)
			{
				V_0 = node.get_Operand() as UnaryExpression;
				if (V_0 != null && V_0.get_Operator() == 7 || V_0.get_Operator() == 9)
				{
					return this.Visit(V_0.get_Operand());
				}
			}
			return this.VisitUnaryExpression(node);
		}
	}
}