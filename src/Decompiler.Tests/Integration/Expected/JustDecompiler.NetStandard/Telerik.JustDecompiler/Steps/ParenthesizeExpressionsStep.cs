using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class ParenthesizeExpressionsStep : BaseCodeVisitor, IDecompilationStep
	{
		public ParenthesizeExpressionsStep()
		{
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			return (BlockStatement)(new ParenthesizeExpressionsStep.Parenthesizer()).Visit(body);
		}

		private class Parenthesizer : BaseCodeTransformer
		{
			public Parenthesizer()
			{
			}

			private bool IsCommutative(BinaryOperator @operator)
			{
				if (@operator == BinaryOperator.Add || @operator == BinaryOperator.BitwiseAnd || @operator == BinaryOperator.BitwiseOr || @operator == BinaryOperator.BitwiseXor || @operator == BinaryOperator.LogicalAnd || @operator == BinaryOperator.LogicalOr || @operator == BinaryOperator.Multiply || @operator == BinaryOperator.ValueEquality || @operator == BinaryOperator.ValueInequality)
				{
					return true;
				}
				return @operator == BinaryOperator.Assign;
			}

			public override ICodeNode VisitBinaryExpression(BinaryExpression node)
			{
				bool flag = false;
				if (node.Left.CodeNodeType == CodeNodeType.BinaryExpression)
				{
					BinaryExpression left = node.Left as BinaryExpression;
					if (left.CompareOperators(node) > 0)
					{
						flag = true;
					}
					else if (left.IsOverridenOperation)
					{
						flag = true;
					}
				}
				if (flag)
				{
					node.Left = new ParenthesesExpression(node.Left);
				}
				bool flag1 = false;
				if (node.Right.CodeNodeType == CodeNodeType.BinaryExpression)
				{
					BinaryExpression right = node.Right as BinaryExpression;
					int num = right.CompareOperators(node);
					if (num > 0)
					{
						flag1 = true;
					}
					else if (num != 0)
					{
						if (!node.IsAssignmentExpression && right.IsOverridenOperation)
						{
							flag1 = true;
						}
					}
					else if (node.Operator != right.Operator)
					{
						flag1 = true;
					}
					else if (!this.IsCommutative(node.Operator))
					{
						flag1 = true;
					}
					else if ((object)right.ExpressionType == (object)right.ExpressionType.get_Module().get_TypeSystem().get_Single() || (object)right.ExpressionType == (object)right.ExpressionType.get_Module().get_TypeSystem().get_Double())
					{
						flag1 = true;
					}
				}
				if (flag1)
				{
					node.Right = new ParenthesesExpression(node.Right);
				}
				return base.VisitBinaryExpression(node);
			}

			public override ICodeNode VisitUnaryExpression(UnaryExpression node)
			{
				if (node.Operator == UnaryOperator.AddressDereference)
				{
					if (node.Operand.CodeNodeType != CodeNodeType.VariableReferenceExpression && node.Operand.CodeNodeType != CodeNodeType.ArgumentReferenceExpression && node.Operand.CodeNodeType != CodeNodeType.ExplicitCastExpression)
					{
						node.Operand = new ParenthesesExpression(node.Operand);
					}
				}
				else if (node.Operator != UnaryOperator.None && node.Operand.CodeNodeType == CodeNodeType.BinaryExpression)
				{
					node.Operand = new ParenthesesExpression(node.Operand);
				}
				return base.VisitUnaryExpression(node);
			}
		}
	}
}