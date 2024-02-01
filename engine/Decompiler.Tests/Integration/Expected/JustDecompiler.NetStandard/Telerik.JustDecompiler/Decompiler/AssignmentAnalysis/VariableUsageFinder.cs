using Mono.Cecil.Cil;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
	internal class VariableUsageFinder : BaseUsageFinder
	{
		private readonly VariableDefinition variable;

		public VariableUsageFinder(VariableDefinition variable)
		{
			this.variable = variable;
		}

		public override bool CheckExpression(Expression node)
		{
			if (node.CodeNodeType != CodeNodeType.VariableReferenceExpression)
			{
				return false;
			}
			return (object)(node as VariableReferenceExpression).Variable.Resolve() == (object)this.variable;
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if ((object)node.Variable.Resolve() == (object)this.variable)
			{
				this.searchResult = UsageFinderSearchResult.Used;
			}
		}
	}
}