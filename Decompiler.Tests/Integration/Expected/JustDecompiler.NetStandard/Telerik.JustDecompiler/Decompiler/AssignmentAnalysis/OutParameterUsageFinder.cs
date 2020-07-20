using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
	internal class OutParameterUsageFinder : BaseUsageFinder
	{
		private readonly ParameterDefinition parameter;

		public OutParameterUsageFinder(ParameterDefinition parameter)
		{
			base();
			this.parameter = parameter;
			return;
		}

		private bool CheckArgumentReference(ArgumentReferenceExpression node)
		{
			if (node == null)
			{
				return false;
			}
			return (object)node.get_Parameter().Resolve() == (object)this.parameter;
		}

		public override bool CheckExpression(Expression node)
		{
			if (node.get_CodeNodeType() == 25 && this.CheckArgumentReference(node as ArgumentReferenceExpression))
			{
				return true;
			}
			if (node.get_CodeNodeType() != 23 || (node as UnaryExpression).get_Operator() != 8)
			{
				return false;
			}
			return this.CheckArgumentReference((node as UnaryExpression).get_Operand() as ArgumentReferenceExpression);
		}

		public override void VisitArgumentReferenceExpression(ArgumentReferenceExpression node)
		{
			if ((object)node.get_Parameter().Resolve() == (object)this.parameter)
			{
				this.searchResult = 2;
			}
			return;
		}

		public override void VisitReturnExpression(ReturnExpression node)
		{
			this.Visit(node.get_Value());
			if (this.searchResult == UsageFinderSearchResult.NotFound)
			{
				this.searchResult = 2;
			}
			return;
		}
	}
}