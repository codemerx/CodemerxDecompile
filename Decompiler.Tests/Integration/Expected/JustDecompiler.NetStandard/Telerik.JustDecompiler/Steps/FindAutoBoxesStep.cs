using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class FindAutoBoxesStep : BaseCodeVisitor, IDecompilationStep
	{
		private MethodSpecificContext context;

		private readonly Stack<Expression> parentExpressions = new Stack<Expression>();

		public FindAutoBoxesStep()
		{
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context.MethodContext;
			foreach (KeyValuePair<int, IList<Expression>> blockExpression in context.MethodContext.Expressions.BlockExpressions)
			{
				this.parentExpressions.Clear();
				foreach (Expression value in blockExpression.Value)
				{
					this.Visit(value);
				}
			}
			return body;
		}

		public override void Visit(ICodeNode node)
		{
			if (node is Expression)
			{
				this.parentExpressions.Push(node as Expression);
			}
			base.Visit(node);
			if (node is Expression)
			{
				this.parentExpressions.Pop();
			}
		}

		public override void VisitBoxExpression(BoxExpression node)
		{
			Stack<Expression> expressions = new Stack<Expression>();
			expressions.Push(this.parentExpressions.Pop());
			if (this.parentExpressions.Count > 0)
			{
				Expression expression = this.parentExpressions.Peek();
				if (expression is MemberReferenceExpresion)
				{
					MemberReferenceExpresion memberReferenceExpresion = this.parentExpressions.Pop() as MemberReferenceExpresion;
					expressions.Push(memberReferenceExpresion);
					if (memberReferenceExpresion.Member is MethodReference)
					{
						node.IsAutoBox = true;
					}
				}
				else if (expression is MethodInvocationExpression || expression is FieldReferenceExpression || expression is PropertyReferenceExpression)
				{
					node.IsAutoBox = true;
				}
				else if (expression is BinaryExpression)
				{
					BinaryExpression binaryExpression = expression as BinaryExpression;
					if (binaryExpression.IsAssignmentExpression && binaryExpression.Right.Equals(node))
					{
						node.IsAutoBox = true;
					}
					if (binaryExpression.IsComparisonExpression && node.BoxedAs.IsGenericParameter)
					{
						if (binaryExpression.Left == node && binaryExpression.Right is LiteralExpression && (binaryExpression.Right as LiteralExpression).Value == null)
						{
							node.IsAutoBox = true;
						}
						if (binaryExpression.Right == node && binaryExpression.Left is LiteralExpression && (binaryExpression.Left as LiteralExpression).Value == null)
						{
							node.IsAutoBox = true;
						}
					}
				}
				else if (expression is ReturnExpression)
				{
					if (this.context.Method.ReturnType.FullName != "System.Object")
					{
						TypeDefinition typeDefinition = node.BoxedExpression.ExpressionType.Resolve();
						if (typeDefinition != null && typeDefinition.IsValueType)
						{
							node.IsAutoBox = true;
						}
					}
					else
					{
						node.IsAutoBox = true;
					}
				}
				else if (expression is YieldReturnExpression && (expression as YieldReturnExpression).Expression.Equals(node))
				{
					node.IsAutoBox = true;
				}
			}
			while (expressions.Count > 0)
			{
				this.parentExpressions.Push(expressions.Pop());
			}
			base.VisitBoxExpression(node);
		}
	}
}