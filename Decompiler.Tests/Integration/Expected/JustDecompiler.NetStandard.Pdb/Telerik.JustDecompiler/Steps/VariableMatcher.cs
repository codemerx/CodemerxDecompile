using Mono.Cecil.Cil;
using System;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	internal class VariableMatcher : Matcher
	{
		private VariableReference variable;

		private VariableMatcher(VariableReference variable)
		{
			base();
			this.variable = variable;
			return;
		}

		public static bool FindVariableInExpression(VariableReference variable, Expression expression)
		{
			stackVariable1 = new VariableMatcher(variable);
			stackVariable1.Visit(expression);
			return stackVariable1.get_Match();
		}

		private void VisitAddressOfExpression(UnaryExpression node)
		{
			V_0 = node.get_Operand() as VariableReferenceExpression;
			if (V_0 != null)
			{
				this.VisitVariableReferenceExpression(V_0);
			}
			return;
		}

		public override void VisitUnaryExpression(UnaryExpression node)
		{
			if (node.get_Operator() == 9)
			{
				this.VisitAddressOfExpression(node);
				return;
			}
			this.VisitUnaryExpression(node);
			return;
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if ((object)node.get_Variable() != (object)this.variable)
			{
				return;
			}
			this.set_Match(true);
			this.set_Continue(false);
			return;
		}
	}
}