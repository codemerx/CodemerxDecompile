using Telerik.JustDecompiler.Ast;
using Mono.Cecil.Extensions;
using System;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Steps
{
    public static class Negator
    {
        public static Expression Negate(Expression expression, TypeSystem typeSystem)
        {
			if (expression == null)
			{
				return null;
			}

            switch (expression.CodeNodeType)
            {
                case CodeNodeType.BinaryExpression:
                    {
						BinaryExpression binaryExpression = (BinaryExpression)expression;
						if (IsMathOperator(binaryExpression.Operator))
                        {
                            if (binaryExpression.ExpressionType.FullName == "System.Boolean")
                            {
                                return new UnaryExpression(UnaryOperator.LogicalNot, expression, null);
                            }
                            BinaryExpression result =
                                new BinaryExpression(BinaryOperator.ValueEquality, expression, new LiteralExpression(0, typeSystem, null), typeSystem, null);
                            return result;
                        }
                        return NegateBinaryExpression(expression, typeSystem);
                    }
                case CodeNodeType.UnaryExpression:
                    {
                        return NegateUnaryExpression(expression, typeSystem);
                    }
                case CodeNodeType.ConditionExpression:
                    {
                        return NegateConditionExpression(expression, typeSystem);
                    }
            }

            if (expression.CodeNodeType == CodeNodeType.LiteralExpression && expression.ExpressionType.FullName == typeSystem.Boolean.FullName)
            {
                return new LiteralExpression(!(bool)(expression as LiteralExpression).Value, typeSystem, expression.UnderlyingSameMethodInstructions);
            }

            LiteralExpression literalExpression = expression.ExpressionType.GetDefaultValueExpression(typeSystem) as LiteralExpression;
			if (literalExpression != null)
            {
				if (literalExpression.ExpressionType.FullName == typeSystem.Boolean.FullName)
				{
					UnaryExpression unaryResult = new UnaryExpression(UnaryOperator.LogicalNot, expression, null);
					return unaryResult;
				}
                BinaryExpression result = new BinaryExpression(BinaryOperator.ValueEquality, expression, literalExpression, typeSystem, null);
                return result;
            }
            else
            {
                return new UnaryExpression(UnaryOperator.LogicalNot, expression, null);
            }
        }

        private static Expression NegateConditionExpression(Expression expression, TypeSystem typeSystem)
        {
            ConditionExpression condition = (ConditionExpression)expression;
            condition.Then = Negate(condition.Then, typeSystem);
            condition.Else = Negate(condition.Else, typeSystem);
            return condition;
        }

        private static Expression NegateBinaryExpression(Expression expression, TypeSystem typeSystem)
        {
            BinaryExpression binary = (BinaryExpression)expression;

            if (IsLogicalOperator(binary.Operator))
            {
                BinaryOperator @operator = binary.Operator == BinaryOperator.LogicalAnd ?
                    BinaryOperator.LogicalOr
                    : BinaryOperator.LogicalAnd;

                binary.Left = Negate(binary.Left, typeSystem);

                binary.Operator = @operator;

                binary.Right = Negate(binary.Right, typeSystem);

                return binary;
            }

            //Roslyn uses bitwise logical operators for bool conditions
            if (IsBitwiseOperator(binary.Operator) && binary.ExpressionType.FullName == typeSystem.Boolean.FullName)
            {
                if (binary.Operator == BinaryOperator.BitwiseXor)
                {
                    binary.Operator = BinaryOperator.ValueEquality;
                }
                else
                {
                    binary.Left = Negate(binary.Left, typeSystem);

                    binary.Operator = binary.Operator == BinaryOperator.BitwiseAnd ? BinaryOperator.BitwiseOr : BinaryOperator.BitwiseAnd;

                    binary.Right = Negate(binary.Right, typeSystem);
                }
                return binary;
            }

            BinaryOperator op;
            if (TryGetInverseOperator(binary.Operator, out op))
            {
                binary.Operator = op;
                return binary;
            }

            //Generate error in unexpected case
            throw new ArgumentException("expression");
        }

        private static bool TryGetInverseOperator(BinaryOperator @operator, out BinaryOperator inverse)
        {
            switch (@operator)
            {
                case BinaryOperator.ValueEquality:
                    inverse = BinaryOperator.ValueInequality;
                    break;
                case BinaryOperator.ValueInequality:
                    inverse = BinaryOperator.ValueEquality;
                    break;
                case BinaryOperator.LessThan:
                    inverse = BinaryOperator.GreaterThanOrEqual;
                    break;
                case BinaryOperator.LessThanOrEqual:
                    inverse = BinaryOperator.GreaterThan;
                    break;
                case BinaryOperator.GreaterThan:
                    inverse = BinaryOperator.LessThanOrEqual;
                    break;
                case BinaryOperator.GreaterThanOrEqual:
                    inverse = BinaryOperator.LessThan;
                    break;
                default:
                    inverse = @operator;
                    return false;
            }
            return true;
        }

        private static bool IsBitwiseOperator(BinaryOperator @operator)
        {
            switch (@operator)
            {
                case BinaryOperator.BitwiseAnd:
                case BinaryOperator.BitwiseOr:
                case BinaryOperator.BitwiseXor:
                    return true;
            }
            return false;
        }

        private static bool IsLogicalOperator(BinaryOperator @operator)
        {
            switch (@operator)
            {
                case BinaryOperator.LogicalAnd:
                case BinaryOperator.LogicalOr:
                    return true;
            }
            return false;
        }

        private static Expression NegateUnaryExpression(Expression expression, TypeSystem typeSystem)
        {
			UnaryExpression unary = (UnaryExpression)expression;
			switch (unary.Operator)
            {
                case UnaryOperator.LogicalNot:
                    {
						unary.Operator = UnaryOperator.None;
						return unary;
                    }
				case UnaryOperator.AddressDereference:
				case UnaryOperator.None:
					{
						if (unary.Operand.CodeNodeType != CodeNodeType.BinaryExpression)
						{
                            if (unary.Operand.CodeNodeType == CodeNodeType.UnaryExpression && (unary.Operand as UnaryExpression).Operator == UnaryOperator.LogicalNot)
                            {
                                unary.Operand = (unary.Operand as UnaryExpression).Operand;
                            }
                            else
                            {
                                unary.Operator = UnaryOperator.LogicalNot;
                            }
							return unary;
						}
						else
						{
							Expression operand = NegateBinaryExpression(unary.Operand as BinaryExpression, typeSystem);
							unary.Operand = operand;
							return unary;
						}
					}
                default:
                    {
                        throw new ArgumentException("expression");
                    }
            }
        }

        private static bool IsMathOperator(BinaryOperator binaryOperator)
        {
            switch (binaryOperator)
            {
                case BinaryOperator.Add:
                case BinaryOperator.Divide:
                case BinaryOperator.LeftShift:
                case BinaryOperator.Modulo:
                case BinaryOperator.Multiply:
                case BinaryOperator.RightShift:
                case BinaryOperator.Subtract:
                case BinaryOperator.BitwiseAnd:
                case BinaryOperator.BitwiseOr:
                case BinaryOperator.BitwiseXor:
                    return true;
                default:
                    return false;
            }
        }
    }
}