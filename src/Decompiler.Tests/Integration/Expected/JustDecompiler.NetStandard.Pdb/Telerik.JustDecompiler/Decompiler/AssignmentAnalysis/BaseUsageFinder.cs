using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
	internal abstract class BaseUsageFinder : BaseCodeVisitor
	{
		protected UsageFinderSearchResult searchResult;

		protected BaseUsageFinder()
		{
		}

		public abstract bool CheckExpression(Expression node);

		public UsageFinderSearchResult SearchForUsage(Expression expression)
		{
			this.searchResult = UsageFinderSearchResult.NotFound;
			this.Visit(expression);
			return this.searchResult;
		}

		public override void Visit(ICodeNode node)
		{
			if (this.searchResult == UsageFinderSearchResult.NotFound)
			{
				base.Visit(node);
			}
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			if (!node.IsAssignmentExpression)
			{
				base.VisitBinaryExpression(node);
				return;
			}
			this.Visit(node.Right);
			if (this.searchResult != UsageFinderSearchResult.NotFound)
			{
				return;
			}
			if (this.CheckExpression(node.Left))
			{
				this.searchResult = UsageFinderSearchResult.Assigned;
				return;
			}
			base.Visit(node.Left);
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			MethodDefinition methodDefinition = node.MethodExpression.MethodDefinition;
			if (methodDefinition == null)
			{
				base.VisitMethodInvocationExpression(node);
				return;
			}
			this.Visit(node.MethodExpression);
			for (int i = 0; i < node.Arguments.Count; i++)
			{
				UnaryExpression item = node.Arguments[i] as UnaryExpression;
				if (methodDefinition.get_Parameters().get_Item(i).IsOutParameter() && (item != null && item.Operator == UnaryOperator.AddressReference && this.CheckExpression(item.Operand) || this.CheckExpression(node.Arguments[i])))
				{
					this.searchResult = UsageFinderSearchResult.Assigned;
					return;
				}
				this.Visit(node.Arguments[i]);
				if (this.searchResult != UsageFinderSearchResult.NotFound)
				{
					return;
				}
			}
		}
	}
}