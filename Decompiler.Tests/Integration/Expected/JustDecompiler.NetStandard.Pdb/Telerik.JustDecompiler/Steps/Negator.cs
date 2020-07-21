using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	public static class Negator
	{
		private static bool IsBitwiseOperator(BinaryOperator operator)
		{
			if (operator - 21 <= 2)
			{
				return true;
			}
			return false;
		}

		private static bool IsLogicalOperator(BinaryOperator operator)
		{
			if (operator - 11 <= 1)
			{
				return true;
			}
			return false;
		}

		private static bool IsMathOperator(BinaryOperator binaryOperator)
		{
			switch (binaryOperator - 1)
			{
				case 0:
				case 2:
				case 4:
				case 6:
				{
				Label0:
					return true;
				}
				case 1:
				case 3:
				case 5:
				{
				Label1:
					return false;
				}
				default:
				{
					switch (binaryOperator - 17)
					{
						case 0:
						case 2:
						case 4:
						case 5:
						case 6:
						case 7:
						{
							goto Label0;
						}
						case 1:
						case 3:
						{
							goto Label1;
						}
						default:
						{
							goto Label1;
						}
					}
					break;
				}
			}
		}

		public static Expression Negate(Expression expression, TypeSystem typeSystem)
		{
			if (expression == null)
			{
				return null;
			}
			V_1 = expression.get_CodeNodeType();
			if (V_1 == 23)
			{
				return Negator.NegateUnaryExpression(expression, typeSystem);
			}
			if (V_1 == 24)
			{
				V_2 = (BinaryExpression)expression;
				if (!Negator.IsMathOperator(V_2.get_Operator()))
				{
					return Negator.NegateBinaryExpression(expression, typeSystem);
				}
				if (String.op_Equality(V_2.get_ExpressionType().get_FullName(), "System.Boolean"))
				{
					return new UnaryExpression(1, expression, null);
				}
				return new BinaryExpression(9, expression, new LiteralExpression((object)0, typeSystem, null), typeSystem, null, false);
			}
			if (V_1 == 36)
			{
				return Negator.NegateConditionExpression(expression, typeSystem);
			}
			if (expression.get_CodeNodeType() == 22 && String.op_Equality(expression.get_ExpressionType().get_FullName(), typeSystem.get_Boolean().get_FullName()))
			{
				return new LiteralExpression((object)(!(Boolean)(expression as LiteralExpression).get_Value()), typeSystem, expression.get_UnderlyingSameMethodInstructions());
			}
			V_0 = expression.get_ExpressionType().GetDefaultValueExpression(typeSystem) as LiteralExpression;
			if (V_0 == null)
			{
				return new UnaryExpression(1, expression, null);
			}
			if (String.op_Equality(V_0.get_ExpressionType().get_FullName(), typeSystem.get_Boolean().get_FullName()))
			{
				return new UnaryExpression(1, expression, null);
			}
			return new BinaryExpression(9, expression, V_0, typeSystem, null, false);
		}

		private static Expression NegateBinaryExpression(Expression expression, TypeSystem typeSystem)
		{
			V_0 = (BinaryExpression)expression;
			if (Negator.IsLogicalOperator(V_0.get_Operator()))
			{
				if (V_0.get_Operator() == 12)
				{
					stackVariable48 = 11;
				}
				else
				{
					stackVariable48 = 12;
				}
				V_2 = stackVariable48;
				V_0.set_Left(Negator.Negate(V_0.get_Left(), typeSystem));
				V_0.set_Operator(V_2);
				V_0.set_Right(Negator.Negate(V_0.get_Right(), typeSystem));
				return V_0;
			}
			if (!Negator.IsBitwiseOperator(V_0.get_Operator()) || !String.op_Equality(V_0.get_ExpressionType().get_FullName(), typeSystem.get_Boolean().get_FullName()))
			{
				if (!Negator.TryGetInverseOperator(V_0.get_Operator(), out V_1))
				{
					throw new ArgumentException("expression");
				}
				V_0.set_Operator(V_1);
				return V_0;
			}
			if (V_0.get_Operator() != 23)
			{
				V_0.set_Left(Negator.Negate(V_0.get_Left(), typeSystem));
				stackVariable32 = V_0;
				if (V_0.get_Operator() == 22)
				{
					stackVariable36 = 21;
				}
				else
				{
					stackVariable36 = 22;
				}
				stackVariable32.set_Operator(stackVariable36);
				V_0.set_Right(Negator.Negate(V_0.get_Right(), typeSystem));
			}
			else
			{
				V_0.set_Operator(9);
			}
			return V_0;
		}

		private static Expression NegateConditionExpression(Expression expression, TypeSystem typeSystem)
		{
			stackVariable1 = (ConditionExpression)expression;
			stackVariable1.set_Then(Negator.Negate(stackVariable1.get_Then(), typeSystem));
			stackVariable1.set_Else(Negator.Negate(stackVariable1.get_Else(), typeSystem));
			return stackVariable1;
		}

		private static Expression NegateUnaryExpression(Expression expression, TypeSystem typeSystem)
		{
			V_0 = (UnaryExpression)expression;
			V_1 = V_0.get_Operator();
			if (V_1 == 1)
			{
				V_0.set_Operator(11);
				return V_0;
			}
			if (V_1 != 8 && V_1 != 11)
			{
				throw new ArgumentException("expression");
			}
			if (V_0.get_Operand().get_CodeNodeType() == 24)
			{
				V_2 = Negator.NegateBinaryExpression(V_0.get_Operand() as BinaryExpression, typeSystem);
				V_0.set_Operand(V_2);
				return V_0;
			}
			if (V_0.get_Operand().get_CodeNodeType() != 23 || (V_0.get_Operand() as UnaryExpression).get_Operator() != 1)
			{
				V_0.set_Operator(1);
			}
			else
			{
				V_0.set_Operand((V_0.get_Operand() as UnaryExpression).get_Operand());
			}
			return V_0;
		}

		private static bool TryGetInverseOperator(BinaryOperator operator, out BinaryOperator inverse)
		{
			switch (operator - 9)
			{
				case 0:
				{
					inverse = 10;
					break;
				}
				case 1:
				{
					inverse = 9;
					break;
				}
				case 2:
				case 3:
				{
				Label0:
					inverse = operator;
					return false;
				}
				case 4:
				{
					inverse = 16;
					break;
				}
				case 5:
				{
					inverse = 15;
					break;
				}
				case 6:
				{
					inverse = 14;
					break;
				}
				case 7:
				{
					inverse = 13;
					break;
				}
				default:
				{
					goto Label0;
				}
			}
			return true;
		}
	}
}