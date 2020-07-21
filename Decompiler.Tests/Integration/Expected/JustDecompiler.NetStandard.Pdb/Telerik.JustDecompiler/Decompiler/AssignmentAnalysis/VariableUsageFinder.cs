using Mono.Cecil.Cil;
using System;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
	internal class VariableUsageFinder : BaseUsageFinder
	{
		private readonly VariableDefinition variable;

		public VariableUsageFinder(VariableDefinition variable)
		{
			base();
			this.variable = variable;
			return;
		}

		public override bool CheckExpression(Expression node)
		{
			if (node.get_CodeNodeType() != 26)
			{
				return false;
			}
			return (object)(node as VariableReferenceExpression).get_Variable().Resolve() == (object)this.variable;
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if ((object)node.get_Variable().Resolve() == (object)this.variable)
			{
				this.searchResult = 2;
			}
			return;
		}
	}
}