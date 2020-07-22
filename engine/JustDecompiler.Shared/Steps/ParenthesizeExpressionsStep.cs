using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	/// <remarks>
	/// Depends on the following previous steps:
	/// * ResolveDynamicVariablesStep
	/// * VariableInliningPattern
	/// * Steps that use Negator (e.g. CreateIfElseIfStatementsStep)
	/// </remarks>
	class ParenthesizeExpressionsStep : BaseCodeVisitor, IDecompilationStep
	{
		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			BlockStatement result = (BlockStatement)new Parenthesizer().Visit(body);
			return result;
		}

		private class Parenthesizer : BaseCodeTransformer
		{
			public override ICodeNode VisitBinaryExpression(BinaryExpression node)
			{
				bool shouldAddLeftParentheses = false;
				if (node.Left.CodeNodeType == CodeNodeType.BinaryExpression)
				{
					BinaryExpression leftAsBinary = node.Left as BinaryExpression;
					int compared = leftAsBinary.CompareOperators(node);

					// (a + b) * c
					if (compared > 0)
					{
						shouldAddLeftParentheses = true;
					}
					else if (leftAsBinary.IsOverridenOperation)
					{
						shouldAddLeftParentheses = true;
					}
				}
				if (shouldAddLeftParentheses)
				{
					node.Left = new ParenthesesExpression(node.Left);
				}

				bool shouldAddRightParentheses = false;
				if (node.Right.CodeNodeType == CodeNodeType.BinaryExpression)
				{
					BinaryExpression rightAsBinary = node.Right as BinaryExpression;
					int compared = rightAsBinary.CompareOperators(node);

					// a * (b + c)
					if (compared > 0)
					{
						shouldAddRightParentheses = true;
					}
					else if (compared == 0)
					{
						// a * (b / c)
						if (node.Operator != rightAsBinary.Operator)
						{
							shouldAddRightParentheses = true;
						}
						// a - (b - c)
						else if (!IsCommutative(node.Operator))
						{
							shouldAddRightParentheses = true;
						}
						// a + (b + c)
						else if (rightAsBinary.ExpressionType == rightAsBinary.ExpressionType.Module.TypeSystem.Single ||
								 rightAsBinary.ExpressionType == rightAsBinary.ExpressionType.Module.TypeSystem.Double)
						{
							shouldAddRightParentheses = true;
						}
					}
					else if (!node.IsAssignmentExpression && rightAsBinary.IsOverridenOperation)
					{
						shouldAddRightParentheses = true;
					}
				}
				if (shouldAddRightParentheses)
				{
					node.Right = new ParenthesesExpression(node.Right);
				}

				return base.VisitBinaryExpression(node);
			}

			public override ICodeNode VisitUnaryExpression(UnaryExpression node)
			{
				if (node.Operator == UnaryOperator.AddressDereference)
				{
					if (!(node.Operand.CodeNodeType == CodeNodeType.VariableReferenceExpression ||
						  node.Operand.CodeNodeType == CodeNodeType.ArgumentReferenceExpression ||
						  node.Operand.CodeNodeType == CodeNodeType.ExplicitCastExpression))
					{
						node.Operand = new ParenthesesExpression(node.Operand);
					}
				}
				else if (node.Operator != UnaryOperator.None)
				{
					if (node.Operand.CodeNodeType == CodeNodeType.BinaryExpression)
					{
						node.Operand = new ParenthesesExpression(node.Operand);
					}
				}

				return base.VisitUnaryExpression(node);
			}

			private bool IsCommutative(BinaryOperator @operator)
			{
				return @operator == BinaryOperator.Add ||
					   @operator == BinaryOperator.BitwiseAnd ||
					   @operator == BinaryOperator.BitwiseOr ||
					   @operator == BinaryOperator.BitwiseXor ||
					   @operator == BinaryOperator.LogicalAnd ||
					   @operator == BinaryOperator.LogicalOr ||
					   @operator == BinaryOperator.Multiply ||
					   @operator == BinaryOperator.ValueEquality ||
					   @operator == BinaryOperator.ValueInequality ||
					   @operator == BinaryOperator.Assign;
			}
		}
	}
}
