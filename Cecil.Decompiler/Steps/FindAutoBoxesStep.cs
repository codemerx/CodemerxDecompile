using System;
using System.Collections.Generic;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	class FindAutoBoxesStep : BaseCodeVisitor, IDecompilationStep
	{
		private MethodSpecificContext context;
		private readonly Stack<Expression> parentExpressions = new Stack<Expression>();

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context.MethodContext;
			foreach (KeyValuePair<int, IList<Expression>> blockPair in context.MethodContext.Expressions.BlockExpressions)
			{
				parentExpressions.Clear();
				foreach (Expression expr in blockPair.Value)
				{
					Visit(expr);
				}
			}
			return body;
		}

        public override void Visit(ICodeNode node)
		{
			if (node is Expression)
			{
				parentExpressions.Push(node as Expression);
			}
			base.Visit(node);
			if (node is Expression)
			{
				parentExpressions.Pop();
			}
		}

		public override void VisitBoxExpression(BoxExpression node)
		{
			Stack<Expression> removedExpressions = new Stack<Expression>();
			removedExpressions.Push(parentExpressions.Pop());// pop the box expression
			if (parentExpressions.Count > 0)
			{
				Expression containingExpression = parentExpressions.Peek();
				if (containingExpression is MemberReferenceExpresion)
				{
					MemberReferenceExpresion memberExpression = parentExpressions.Pop() as MemberReferenceExpresion;
					removedExpressions.Push(memberExpression);
					if (memberExpression.Member is MethodReference)
					{
						//The box expression is the method target.
						// This is automatically handled by C#/VB compilers.
						node.IsAutoBox = true;
					}
				}
				else if (containingExpression is MethodInvocationExpression ||
						containingExpression is FieldReferenceExpression ||
						containingExpression is PropertyReferenceExpression)
				{
					// The boxed expression is an argument to a method call.
					// This is automatically handled by C#/VB compilers.
					node.IsAutoBox = true;
				}
				else if (containingExpression is BinaryExpression)
				{
					BinaryExpression binary = containingExpression as BinaryExpression;

					// Check for expressions of the type:
					//		a = B;
					//	where a is of type object and B is the boxed expression.
					if (binary.IsAssignmentExpression && binary.Right.Equals(node))
					{
						node.IsAutoBox = true;
					}

					// check for expressions of the type:
					//		X == null 
					//		X != null 
					//		null == X 
					//		null != X
					//	where X is the boxed expression and X is of some generic parameter type T
					if (binary.IsComparisonExpression && node.BoxedAs.IsGenericParameter)
					{
						if (binary.Left == node)
						{
							if (binary.Right is LiteralExpression && (binary.Right as LiteralExpression).Value == null)
							{
								node.IsAutoBox = true;
							}
						}
						if (binary.Right == node)
						{
							if (binary.Left is LiteralExpression && (binary.Left as LiteralExpression).Value == null)
							{
								node.IsAutoBox = true;
							}
						}
					}
				}
				else if (containingExpression is ReturnExpression)
				{
					if (context.Method.ReturnType.FullName == "System.Object")
					{
						node.IsAutoBox = true;
					}
					else
					{
						TypeDefinition type = node.BoxedExpression.ExpressionType.Resolve();
						if (type != null && type.IsValueType)
						{
							node.IsAutoBox = true;
						}
					}
				}
				else if (containingExpression is YieldReturnExpression)
				{
					YieldReturnExpression yieldReturn = containingExpression as YieldReturnExpression;
					if (yieldReturn.Expression.Equals(node))
					{
						node.IsAutoBox = true;
					}
				}
			}

			// push back the popped expressions
			while (removedExpressions.Count > 0)
			{
				parentExpressions.Push(removedExpressions.Pop());
			}

			base.VisitBoxExpression(node);
		}
	}
}
