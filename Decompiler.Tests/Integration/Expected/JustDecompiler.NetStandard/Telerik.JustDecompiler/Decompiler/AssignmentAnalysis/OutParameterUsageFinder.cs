using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
	internal class OutParameterUsageFinder : BaseUsageFinder
	{
		private readonly ParameterDefinition parameter;

		public OutParameterUsageFinder(ParameterDefinition parameter)
		{
			this.parameter = parameter;
		}

		private bool CheckArgumentReference(ArgumentReferenceExpression node)
		{
			if (node == null)
			{
				return false;
			}
			return node.Parameter.Resolve() == this.parameter;
		}

		public override bool CheckExpression(Expression node)
		{
			if (node.CodeNodeType == CodeNodeType.ArgumentReferenceExpression && this.CheckArgumentReference(node as ArgumentReferenceExpression))
			{
				return true;
			}
			if (node.CodeNodeType != CodeNodeType.UnaryExpression || (node as UnaryExpression).Operator != UnaryOperator.AddressDereference)
			{
				return false;
			}
			return this.CheckArgumentReference((node as UnaryExpression).Operand as ArgumentReferenceExpression);
		}

		public override void VisitArgumentReferenceExpression(ArgumentReferenceExpression node)
		{
			if (node.Parameter.Resolve() == this.parameter)
			{
				this.searchResult = UsageFinderSearchResult.Used;
			}
		}

		public override void VisitReturnExpression(ReturnExpression node)
		{
			this.Visit(node.Value);
			if (this.searchResult == UsageFinderSearchResult.NotFound)
			{
				this.searchResult = UsageFinderSearchResult.Used;
			}
		}
	}
}