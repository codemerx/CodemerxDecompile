using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class TransformCatchClausesFilterExpressionStep : BaseCodeTransformer, IDecompilationStep
	{
		private TypeSystem typeSystem;

		public TransformCatchClausesFilterExpressionStep()
		{
			base();
			return;
		}

		private static BinaryExpression GetBinaryExpression(bool isNestedBinaryInUnary, Expression binaryAsExpression)
		{
			if (!isNestedBinaryInUnary)
			{
				V_0 = binaryAsExpression as BinaryExpression;
			}
			else
			{
				V_0 = (binaryAsExpression as UnaryExpression).get_Operand() as BinaryExpression;
			}
			return V_0;
		}

		private static Expression GetResultExpression(bool isNestedBinaryInUnary, Expression binaryAsExpression, BinaryExpression newBinaryExpression)
		{
			if (!isNestedBinaryInUnary)
			{
				V_0 = newBinaryExpression;
			}
			else
			{
				stackVariable4 = binaryAsExpression as UnaryExpression;
				stackVariable4.set_Operand(newBinaryExpression);
				V_0 = stackVariable4;
			}
			return V_0;
		}

		private bool IsBinaryExpression(Expression expression, out Expression binaryExpression, out bool isWrappedInUnary)
		{
			binaryExpression = null;
			isWrappedInUnary = false;
			V_0 = expression as BinaryExpression;
			V_1 = expression as UnaryExpression;
			if (V_0 != null)
			{
				binaryExpression = V_0;
				return true;
			}
			if (V_1 == null || V_1.get_Operator() != 11 && V_1.get_Operator() != 1 || V_1.get_Operand() as BinaryExpression == null)
			{
				return false;
			}
			if (V_1.get_Operator() != 1)
			{
				binaryExpression = V_1.get_Operand();
			}
			else
			{
				binaryExpression = Negator.Negate(V_1.get_Operand(), this.typeSystem);
				if (binaryExpression as BinaryExpression == null)
				{
					isWrappedInUnary = true;
				}
			}
			return true;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.typeSystem = context.get_MethodContext().get_Method().get_Module().get_TypeSystem();
			return (BlockStatement)this.Visit(body);
		}

		private Expression Transform(Expression expression)
		{
			V_0 = null;
			V_1 = expression as ConditionExpression;
			V_2 = null;
			V_3 = false;
			V_4 = false;
			if (!this.IsBinaryExpression(expression, out V_2, out V_3))
			{
				if (V_1 != null && this.TryTransformTernary(V_1, out V_0))
				{
					V_4 = true;
				}
			}
			else
			{
				V_5 = TransformCatchClausesFilterExpressionStep.GetBinaryExpression(V_3, V_2);
				if (this.TryTransformBinary(V_5))
				{
					V_0 = TransformCatchClausesFilterExpressionStep.GetResultExpression(V_3, V_2, V_5);
					V_4 = true;
				}
			}
			if (!V_4)
			{
				V_0 = expression;
			}
			return V_0;
		}

		private bool TryTransform(BinaryExpression node)
		{
			if (!node.get_IsAssignmentExpression())
			{
				return false;
			}
			if (String.op_Inequality(node.get_ExpressionType().get_FullName(), "System.Boolean"))
			{
				return false;
			}
			if (node.get_Left() as VariableReferenceExpression == null)
			{
				return false;
			}
			V_0 = node.get_Right() as BinaryExpression;
			if (V_0 == null)
			{
				return false;
			}
			if (V_0.get_Operator() != 10)
			{
				return false;
			}
			V_1 = V_0.get_Right() as LiteralExpression;
			if (V_1 == null || V_1.get_Value() as Int32 == 0 || (Int32)V_1.get_Value() != 0)
			{
				return false;
			}
			V_3 = false;
			if (this.IsBinaryExpression(V_0.get_Left(), out V_2, out V_3))
			{
				V_5 = TransformCatchClausesFilterExpressionStep.GetBinaryExpression(V_3, V_2);
				if (this.TryTransformBinary(V_5))
				{
					node.set_Right(TransformCatchClausesFilterExpressionStep.GetResultExpression(V_3, V_2, V_5));
					return true;
				}
			}
			V_4 = V_0.get_Left() as ConditionExpression;
			if (V_4 != null && this.TryTransformTernary(V_4, out V_6))
			{
				node.set_Right(V_6);
				return true;
			}
			if (!String.op_Equality(V_0.get_Left().get_ExpressionType().get_FullName(), "System.Boolean"))
			{
				return false;
			}
			node.set_Right(V_0.get_Left());
			return true;
		}

		private bool TryTransformBinary(BinaryExpression binaryExpression)
		{
			V_0 = false;
			if (String.op_Inequality(binaryExpression.get_Left().get_ExpressionType().get_FullName(), "System.Boolean") || String.op_Inequality(binaryExpression.get_Right().get_ExpressionType().get_FullName(), "System.Boolean"))
			{
				return false;
			}
			V_1 = false;
			if (this.IsBinaryExpression(binaryExpression.get_Left(), out V_2, out V_1))
			{
				V_8 = TransformCatchClausesFilterExpressionStep.GetBinaryExpression(V_1, V_2);
				if (this.TryTransformBinary(V_8))
				{
					binaryExpression.set_Left(TransformCatchClausesFilterExpressionStep.GetResultExpression(V_1, V_2, V_8));
					V_0 = true;
				}
			}
			V_1 = false;
			if (this.IsBinaryExpression(binaryExpression.get_Right(), out V_3, out V_1))
			{
				V_9 = TransformCatchClausesFilterExpressionStep.GetBinaryExpression(V_1, V_3);
				if (this.TryTransformBinary(V_9))
				{
					binaryExpression.set_Right(TransformCatchClausesFilterExpressionStep.GetResultExpression(V_1, V_3, V_9));
					V_0 = true;
				}
			}
			V_4 = binaryExpression.get_Left() as ConditionExpression;
			if (V_4 != null && this.TryTransformTernary(V_4, out V_5))
			{
				binaryExpression.set_Left(V_5);
				V_0 = true;
			}
			V_6 = binaryExpression.get_Right() as ConditionExpression;
			if (V_6 != null && this.TryTransformTernary(V_6, out V_7))
			{
				binaryExpression.set_Right(V_7);
				V_0 = true;
			}
			return V_0;
		}

		private bool TryTransformTernary(ConditionExpression ternary, out Expression transformed)
		{
			transformed = null;
			V_2 = ternary.get_Then() as LiteralExpression;
			V_3 = ternary.get_Else() as LiteralExpression;
			if (V_2 != null && V_3 != null)
			{
				V_14 = false;
				V_15 = false;
				if (!String.op_Equality(V_2.get_ExpressionType().get_FullName(), "System.Int32") || !String.op_Equality(V_3.get_ExpressionType().get_FullName(), "System.Int32"))
				{
					if (String.op_Equality(V_2.get_ExpressionType().get_FullName(), "System.Boolean") && String.op_Equality(V_3.get_ExpressionType().get_FullName(), "System.Boolean"))
					{
						V_18 = (Boolean)V_2.get_Value();
						V_19 = (Boolean)V_3.get_Value();
						if (!V_18 && V_19 || V_18 && !V_19)
						{
							V_14 = true;
							if (!V_18 && V_19)
							{
								V_15 = true;
							}
						}
					}
				}
				else
				{
					V_16 = (Int32)V_2.get_Value();
					V_17 = (Int32)V_3.get_Value();
					if (V_16 == 0 && V_17 == 1 || V_16 == 1 && V_17 == 0)
					{
						V_14 = true;
						if (V_16 == 0 && V_17 == 1)
						{
							V_15 = true;
						}
					}
				}
				if (V_14)
				{
					if (!V_15)
					{
						transformed = this.Transform(ternary.get_Condition());
					}
					else
					{
						transformed = this.Transform(Negator.Negate(ternary.get_Condition(), this.typeSystem));
					}
					return true;
				}
			}
			if (V_2 == null)
			{
				if (V_3 == null)
				{
					V_20 = ternary.get_Then() as ExplicitCastExpression;
					V_21 = ternary.get_Else() as ExplicitCastExpression;
					if (V_20 == null || V_21 == null || !String.op_Equality(V_20.get_TargetType().get_FullName(), "System.Int32") || !String.op_Equality(V_20.get_Expression().get_ExpressionType().get_FullName(), "System.Boolean") || !String.op_Equality(V_21.get_TargetType().get_FullName(), "System.Int32") || !String.op_Equality(V_21.get_Expression().get_ExpressionType().get_FullName(), "System.Boolean"))
					{
						return false;
					}
					ternary.set_Then(V_20.get_Expression());
					ternary.set_Else(V_21.get_Expression());
					transformed = ternary;
					return true;
				}
				V_0 = V_3;
				V_1 = false;
			}
			else
			{
				V_0 = V_2;
				V_1 = true;
			}
			if (!String.op_Equality(V_0.get_ExpressionType().get_FullName(), "System.Int32"))
			{
				if (!String.op_Equality(V_0.get_ExpressionType().get_FullName(), "System.Boolean"))
				{
					return false;
				}
				V_4 = (Boolean)V_0.get_Value();
			}
			else
			{
				if ((Int32)V_0.get_Value() != 0)
				{
					stackVariable163 = true;
				}
				else
				{
					stackVariable163 = false;
				}
				V_4 = stackVariable163;
			}
			V_6 = null;
			V_7 = null;
			V_6 = this.Transform(ternary.get_Condition());
			if (!V_1)
			{
				V_8 = ternary.get_Then();
			}
			else
			{
				V_8 = ternary.get_Else();
			}
			V_9 = V_8 as ConditionExpression;
			V_10 = V_8 as ExplicitCastExpression;
			V_11 = null;
			V_12 = false;
			if (V_9 == null && V_10 == null || String.op_Inequality(V_10.get_TargetType().get_FullName(), "System.Int32") || String.op_Inequality(V_10.get_Expression().get_ExpressionType().get_FullName(), "System.Boolean") && !this.IsBinaryExpression(V_8, out V_11, out V_12) && String.op_Inequality(V_8.get_ExpressionType().get_FullName(), "System.Boolean"))
			{
				return false;
			}
			V_13 = false;
			if (V_10 == null)
			{
				if (V_11 == null)
				{
					if (V_9 != null && this.TryTransformTernary(V_9, out V_7))
					{
						V_13 = true;
					}
				}
				else
				{
					V_22 = TransformCatchClausesFilterExpressionStep.GetBinaryExpression(V_12, V_11);
					if (this.TryTransformBinary(V_22))
					{
						V_7 = TransformCatchClausesFilterExpressionStep.GetResultExpression(V_12, V_11, V_22);
						V_13 = true;
					}
				}
			}
			else
			{
				V_7 = V_10.get_Expression();
				V_13 = true;
			}
			if (!V_13)
			{
				V_7 = V_8;
			}
			if (V_6 == null || V_7 == null)
			{
				return false;
			}
			if (!V_4)
			{
				V_5 = 12;
				if (V_1)
				{
					stackVariable97 = Negator.Negate(V_6, this.typeSystem);
				}
				else
				{
					stackVariable97 = V_6;
				}
				V_6 = stackVariable97;
			}
			else
			{
				V_5 = 11;
				if (V_1)
				{
					stackVariable111 = V_6;
				}
				else
				{
					stackVariable111 = Negator.Negate(V_6, this.typeSystem);
				}
				V_6 = stackVariable111;
			}
			transformed = new BinaryExpression(V_5, V_6, V_7, this.typeSystem, ternary.get_MappedInstructions(), false);
			return true;
		}

		public override ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			if (this.TryTransform(node))
			{
				return node;
			}
			return this.VisitBinaryExpression(node);
		}
	}
}