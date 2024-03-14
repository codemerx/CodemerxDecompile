using Mono.Cecil.Cil;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	internal class VariableMatcher : Matcher
	{
		private VariableReference variable;

		private VariableMatcher(VariableReference variable)
		{
			this.variable = variable;
		}

		public static bool FindVariableInExpression(VariableReference variable, Expression expression)
		{
			VariableMatcher variableMatcher = new VariableMatcher(variable);
			variableMatcher.Visit(expression);
			return variableMatcher.Match;
		}

		private void VisitAddressOfExpression(UnaryExpression node)
		{
			VariableReferenceExpression operand = node.Operand as VariableReferenceExpression;
			if (operand != null)
			{
				this.VisitVariableReferenceExpression(operand);
			}
		}

		public override void VisitUnaryExpression(UnaryExpression node)
		{
			if (node.Operator == UnaryOperator.AddressOf)
			{
				this.VisitAddressOfExpression(node);
				return;
			}
			base.VisitUnaryExpression(node);
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if ((object)node.Variable != (object)this.variable)
			{
				return;
			}
			base.Match = true;
			base.Continue = false;
		}
	}
}