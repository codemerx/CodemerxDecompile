using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
	internal abstract class BaseUsageFinder : BaseCodeVisitor
	{
		protected UsageFinderSearchResult searchResult;

		protected BaseUsageFinder()
		{
			base();
			return;
		}

		public abstract bool CheckExpression(Expression node);

		public UsageFinderSearchResult SearchForUsage(Expression expression)
		{
			this.searchResult = 0;
			this.Visit(expression);
			return this.searchResult;
		}

		public override void Visit(ICodeNode node)
		{
			if (this.searchResult == UsageFinderSearchResult.NotFound)
			{
				this.Visit(node);
			}
			return;
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			if (!node.get_IsAssignmentExpression())
			{
				this.VisitBinaryExpression(node);
				return;
			}
			this.Visit(node.get_Right());
			if (this.searchResult != UsageFinderSearchResult.NotFound)
			{
				return;
			}
			if (this.CheckExpression(node.get_Left()))
			{
				this.searchResult = 1;
				return;
			}
			this.Visit(node.get_Left());
			return;
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			V_0 = node.get_MethodExpression().get_MethodDefinition();
			if (V_0 == null)
			{
				this.VisitMethodInvocationExpression(node);
				return;
			}
			this.Visit(node.get_MethodExpression());
			V_1 = 0;
			while (V_1 < node.get_Arguments().get_Count())
			{
				V_2 = node.get_Arguments().get_Item(V_1) as UnaryExpression;
				if (V_0.get_Parameters().get_Item(V_1).IsOutParameter() && V_2 != null && V_2.get_Operator() == 7 && this.CheckExpression(V_2.get_Operand()) || this.CheckExpression(node.get_Arguments().get_Item(V_1)))
				{
					this.searchResult = 1;
					return;
				}
				this.Visit(node.get_Arguments().get_Item(V_1));
				if (this.searchResult != UsageFinderSearchResult.NotFound)
				{
					return;
				}
				V_1 = V_1 + 1;
			}
			return;
		}
	}
}