using Mono.Cecil;
using Mono.Cecil.Extensions;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	public static class Negator
	{
		private static bool IsBitwiseOperator(BinaryOperator @operator)
		{
			if ((int)@operator - (int)BinaryOperator.BitwiseOr <= (int)BinaryOperator.AddAssign)
			{
				return true;
			}
			return false;
		}

		private static bool IsLogicalOperator(BinaryOperator @operator)
		{
			if ((int)@operator - (int)BinaryOperator.LogicalOr <= (int)BinaryOperator.Add)
			{
				return true;
			}
			return false;
		}

		private static bool IsMathOperator(BinaryOperator binaryOperator)
		{
			switch (binaryOperator)
			{
				case BinaryOperator.Add:
				case BinaryOperator.Subtract:
				case BinaryOperator.Multiply:
				case BinaryOperator.Divide:
				{
					return true;
				}
				case BinaryOperator.AddAssign:
				case BinaryOperator.SubtractAssign:
				case BinaryOperator.MultiplyAssign:
				{
					return false;
				}
				default:
				{
					switch (binaryOperator)
					{
						case BinaryOperator.LeftShift:
						case BinaryOperator.RightShift:
						case BinaryOperator.BitwiseOr:
						case BinaryOperator.BitwiseAnd:
						case BinaryOperator.BitwiseXor:
						case BinaryOperator.Modulo:
						{
							return true;
						}
						case BinaryOperator.LeftShiftAssign:
						case BinaryOperator.RightShiftAssign:
						{
							return false;
						}
						default:
						{
							return false;
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
			CodeNodeType codeNodeType = expression.CodeNodeType;
			if (codeNodeType == CodeNodeType.UnaryExpression)
			{
				return Negator.NegateUnaryExpression(expression, typeSystem);
			}
			if (codeNodeType == CodeNodeType.BinaryExpression)
			{
				BinaryExpression binaryExpression = (BinaryExpression)expression;
				if (!Negator.IsMathOperator(binaryExpression.Operator))
				{
					return Negator.NegateBinaryExpression(expression, typeSystem);
				}
				if (binaryExpression.ExpressionType.get_FullName() == "System.Boolean")
				{
					return new UnaryExpression(UnaryOperator.LogicalNot, expression, null);
				}
				return new BinaryExpression(BinaryOperator.ValueEquality, expression, new LiteralExpression((object)0, typeSystem, null), typeSystem, null, false);
			}
			if (codeNodeType == CodeNodeType.ConditionExpression)
			{
				return Negator.NegateConditionExpression(expression, typeSystem);
			}
			if (expression.CodeNodeType == CodeNodeType.LiteralExpression && expression.ExpressionType.get_FullName() == typeSystem.get_Boolean().get_FullName())
			{
				return new LiteralExpression((object)(!(Boolean)(expression as LiteralExpression).Value), typeSystem, expression.UnderlyingSameMethodInstructions);
			}
			LiteralExpression defaultValueExpression = expression.ExpressionType.GetDefaultValueExpression(typeSystem) as LiteralExpression;
			if (defaultValueExpression == null)
			{
				return new UnaryExpression(UnaryOperator.LogicalNot, expression, null);
			}
			if (defaultValueExpression.ExpressionType.get_FullName() == typeSystem.get_Boolean().get_FullName())
			{
				return new UnaryExpression(UnaryOperator.LogicalNot, expression, null);
			}
			return new BinaryExpression(BinaryOperator.ValueEquality, expression, defaultValueExpression, typeSystem, null, false);
		}

		private static Expression NegateBinaryExpression(Expression expression, TypeSystem typeSystem)
		{
			BinaryOperator binaryOperator;
			BinaryExpression binaryExpression = (BinaryExpression)expression;
			if (Negator.IsLogicalOperator(binaryExpression.Operator))
			{
				BinaryOperator binaryOperator1 = (binaryExpression.Operator == BinaryOperator.LogicalAnd ? BinaryOperator.LogicalOr : BinaryOperator.LogicalAnd);
				binaryExpression.Left = Negator.Negate(binaryExpression.Left, typeSystem);
				binaryExpression.Operator = binaryOperator1;
				binaryExpression.Right = Negator.Negate(binaryExpression.Right, typeSystem);
				return binaryExpression;
			}
			if (!Negator.IsBitwiseOperator(binaryExpression.Operator) || !(binaryExpression.ExpressionType.get_FullName() == typeSystem.get_Boolean().get_FullName()))
			{
				if (!Negator.TryGetInverseOperator(binaryExpression.Operator, out binaryOperator))
				{
					throw new ArgumentException("expression");
				}
				binaryExpression.Operator = binaryOperator;
				return binaryExpression;
			}
			if (binaryExpression.Operator != BinaryOperator.BitwiseXor)
			{
				binaryExpression.Left = Negator.Negate(binaryExpression.Left, typeSystem);
				binaryExpression.Operator = (binaryExpression.Operator == BinaryOperator.BitwiseAnd ? BinaryOperator.BitwiseOr : BinaryOperator.BitwiseAnd);
				binaryExpression.Right = Negator.Negate(binaryExpression.Right, typeSystem);
			}
			else
			{
				binaryExpression.Operator = BinaryOperator.ValueEquality;
			}
			return binaryExpression;
		}

		private static Expression NegateConditionExpression(Expression expression, TypeSystem typeSystem)
		{
			ConditionExpression conditionExpression = (ConditionExpression)expression;
			conditionExpression.Then = Negator.Negate(conditionExpression.Then, typeSystem);
			conditionExpression.Else = Negator.Negate(conditionExpression.Else, typeSystem);
			return conditionExpression;
		}

		private static Expression NegateUnaryExpression(Expression expression, TypeSystem typeSystem)
		{
			UnaryExpression operand = (UnaryExpression)expression;
			UnaryOperator @operator = operand.Operator;
			if (@operator == UnaryOperator.LogicalNot)
			{
				operand.Operator = UnaryOperator.None;
				return operand;
			}
			if (@operator != UnaryOperator.AddressDereference && @operator != UnaryOperator.None)
			{
				throw new ArgumentException("expression");
			}
			if (operand.Operand.CodeNodeType == CodeNodeType.BinaryExpression)
			{
				Expression expression1 = Negator.NegateBinaryExpression(operand.Operand as BinaryExpression, typeSystem);
				operand.Operand = expression1;
				return operand;
			}
			if (operand.Operand.CodeNodeType != CodeNodeType.UnaryExpression || (operand.Operand as UnaryExpression).Operator != UnaryOperator.LogicalNot)
			{
				operand.Operator = UnaryOperator.LogicalNot;
			}
			else
			{
				operand.Operand = (operand.Operand as UnaryExpression).Operand;
			}
			return operand;
		}

		private static bool TryGetInverseOperator(BinaryOperator @operator, out BinaryOperator inverse)
		{
			switch (@operator)
			{
				case BinaryOperator.ValueEquality:
				{
					inverse = BinaryOperator.ValueInequality;
					break;
				}
				case BinaryOperator.ValueInequality:
				{
					inverse = BinaryOperator.ValueEquality;
					break;
				}
				case BinaryOperator.LogicalOr:
				case BinaryOperator.LogicalAnd:
				{
					inverse = @operator;
					return false;
				}
				case BinaryOperator.LessThan:
				{
					inverse = BinaryOperator.GreaterThanOrEqual;
					break;
				}
				case BinaryOperator.LessThanOrEqual:
				{
					inverse = BinaryOperator.GreaterThan;
					break;
				}
				case BinaryOperator.GreaterThan:
				{
					inverse = BinaryOperator.LessThanOrEqual;
					break;
				}
				case BinaryOperator.GreaterThanOrEqual:
				{
					inverse = BinaryOperator.LessThan;
					break;
				}
				default:
				{
					inverse = @operator;
					return false;
				}
			}
			return true;
		}
	}
}