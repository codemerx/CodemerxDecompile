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
			base();
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			return (BlockStatement)(new ParenthesizeExpressionsStep.Parenthesizer()).Visit(body);
		}

		private class Parenthesizer : BaseCodeTransformer
		{
			public Parenthesizer()
			{
				base();
				return;
			}

			private bool IsCommutative(BinaryOperator operator)
			{
				if (operator == 1 || operator == 22 || operator == 21 || operator == 23 || operator == 12 || operator == 11 || operator == 5 || operator == 9 || operator == 10)
				{
					return true;
				}
				return operator == 26;
			}

			public override ICodeNode VisitBinaryExpression(BinaryExpression node)
			{
				V_0 = false;
				if (node.get_Left().get_CodeNodeType() == 24)
				{
					V_2 = node.get_Left() as BinaryExpression;
					if (V_2.CompareOperators(node) <= 0)
					{
						if (V_2.get_IsOverridenOperation())
						{
							V_0 = true;
						}
					}
					else
					{
						V_0 = true;
					}
				}
				if (V_0)
				{
					node.set_Left(new ParenthesesExpression(node.get_Left()));
				}
				V_1 = false;
				if (node.get_Right().get_CodeNodeType() == 24)
				{
					V_3 = node.get_Right() as BinaryExpression;
					V_4 = V_3.CompareOperators(node);
					if (V_4 <= 0)
					{
						if (V_4 != 0)
						{
							if (!node.get_IsAssignmentExpression() && V_3.get_IsOverridenOperation())
							{
								V_1 = true;
							}
						}
						else
						{
							if (node.get_Operator() == V_3.get_Operator())
							{
								if (this.IsCommutative(node.get_Operator()))
								{
									if ((object)V_3.get_ExpressionType() == (object)V_3.get_ExpressionType().get_Module().get_TypeSystem().get_Single() || (object)V_3.get_ExpressionType() == (object)V_3.get_ExpressionType().get_Module().get_TypeSystem().get_Double())
									{
										V_1 = true;
									}
								}
								else
								{
									V_1 = true;
								}
							}
							else
							{
								V_1 = true;
							}
						}
					}
					else
					{
						V_1 = true;
					}
				}
				if (V_1)
				{
					node.set_Right(new ParenthesesExpression(node.get_Right()));
				}
				return this.VisitBinaryExpression(node);
			}

			public override ICodeNode VisitUnaryExpression(UnaryExpression node)
			{
				if (node.get_Operator() != 8)
				{
					if (node.get_Operator() != 11 && node.get_Operand().get_CodeNodeType() == 24)
					{
						node.set_Operand(new ParenthesesExpression(node.get_Operand()));
					}
				}
				else
				{
					if (node.get_Operand().get_CodeNodeType() != 26 && node.get_Operand().get_CodeNodeType() != 25 && node.get_Operand().get_CodeNodeType() != 31)
					{
						node.set_Operand(new ParenthesesExpression(node.get_Operand()));
					}
				}
				return this.VisitUnaryExpression(node);
			}
		}
	}
}