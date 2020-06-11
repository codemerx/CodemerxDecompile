using Mono.Cecil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
    /*
     * This step transforms ternary expressions into binary expressions with logical operators.
     * In matches ternary expressions that contains literal expression (int or bool) in the then or in the else,
     * and boolean expression in the other one (if the expression is in cast expression to int, it is also transformed).
     * Transforms this: !Program.Bool() ? 0 : (int)b is int
     * Into this: Program.Bool() && b is int
    */
    class TransformCatchClausesFilterExpressionStep : BaseCodeTransformer, IDecompilationStep
    {
        private TypeSystem typeSystem;

        public BlockStatement Process(DecompilationContext context, BlockStatement body)
        {
            this.typeSystem = context.MethodContext.Method.Module.TypeSystem;
            return (BlockStatement)Visit(body);
        }

        public override ICodeNode VisitBinaryExpression(BinaryExpression node)
        {
            if (TryTransform(node))
            {
                return node;
            }
            
            return base.VisitBinaryExpression(node);
        }

        private bool TryTransform(BinaryExpression node)
        {
            if (!node.IsAssignmentExpression)
            {
                return false;
            }

            if (node.ExpressionType.FullName != Constants.Boolean)
            {
                return false;
            }

            VariableReferenceExpression variableReference = node.Left as VariableReferenceExpression;
            if (variableReference == null)
            {
                return false;
            }

            BinaryExpression binaryExpression = node.Right as BinaryExpression;
            if (binaryExpression == null)
            {
                return false;
            }

            if (binaryExpression.Operator != BinaryOperator.ValueInequality)
            {
                return false;
            }

            LiteralExpression zero = binaryExpression.Right as LiteralExpression;
            if (zero == null || !(zero.Value is int) || (int)zero.Value != 0)
            {
                return false;
            }

            Expression nestedBinaryAsExpression;
            bool isNestedBinaryInUnary = false;
            if (IsBinaryExpression(binaryExpression.Left, out nestedBinaryAsExpression, out isNestedBinaryInUnary))
            {
                BinaryExpression nestedBinary = GetBinaryExpression(isNestedBinaryInUnary, nestedBinaryAsExpression);
                if (TryTransformBinary(nestedBinary))
                {
                    node.Right = GetResultExpression(isNestedBinaryInUnary, nestedBinaryAsExpression, nestedBinary);

                    return true;
                }
            }

            ConditionExpression ternary = binaryExpression.Left as ConditionExpression;
            if (ternary != null)
            {
                Expression transformedTernary;
                if (TryTransformTernary(ternary, out transformedTernary))
                {
                    node.Right = transformedTernary;

                    return true;
                }
            }

            if (binaryExpression.Left.ExpressionType.FullName == Constants.Boolean)
            {
                node.Right = binaryExpression.Left;

                return true;
            }

            return false;
        }

        private bool TryTransformBinary(BinaryExpression binaryExpression)
        {
            bool isTransformed = false;

            if (binaryExpression.Left.ExpressionType.FullName != Constants.Boolean ||
                binaryExpression.Right.ExpressionType.FullName != Constants.Boolean)
            {
                return false;
            }

            bool isNestedBinaryInUnary = false;

            Expression leftBinaryAsExpression;
            if (IsBinaryExpression(binaryExpression.Left, out leftBinaryAsExpression, out isNestedBinaryInUnary))
            {
                BinaryExpression leftBinary = GetBinaryExpression(isNestedBinaryInUnary, leftBinaryAsExpression);
                if (TryTransformBinary(leftBinary))
                {
                    binaryExpression.Left = GetResultExpression(isNestedBinaryInUnary, leftBinaryAsExpression, leftBinary);
                    isTransformed = true;
                }
            }

            isNestedBinaryInUnary = false;

            Expression rightBinaryAsExpression;
            if (IsBinaryExpression(binaryExpression.Right, out rightBinaryAsExpression, out isNestedBinaryInUnary))
            {
                BinaryExpression rightBinary = GetBinaryExpression(isNestedBinaryInUnary, rightBinaryAsExpression);
                if (TryTransformBinary(rightBinary))
                {
                    binaryExpression.Right = GetResultExpression(isNestedBinaryInUnary, rightBinaryAsExpression, rightBinary);
                    isTransformed = true;
                }
            }

            ConditionExpression leftTernary = binaryExpression.Left as ConditionExpression;
            Expression transformedLeftTernary;
            if (leftTernary != null && TryTransformTernary(leftTernary, out transformedLeftTernary))
            {
                binaryExpression.Left = transformedLeftTernary;
                isTransformed = true;
            }

            ConditionExpression rightTernary = binaryExpression.Right as ConditionExpression;
            Expression transformedRightTernary;
            if (rightTernary != null && TryTransformTernary(rightTernary, out transformedRightTernary))
            {
                binaryExpression.Right = transformedRightTernary;
                isTransformed = true;
            }

            return isTransformed;
        }

        private bool TryTransformTernary(ConditionExpression ternary, out Expression transformed)
        {
            transformed = null;

            LiteralExpression literal;
            bool isLiteralInThen;
            LiteralExpression literalInThen = ternary.Then as LiteralExpression;
            LiteralExpression literalInElse = ternary.Else as LiteralExpression;
            // Transforms the case: Method() && someVar > 12 ? 0 : 1
            if (literalInThen != null && literalInElse != null)
            {
                bool areValidValues = false;
                bool shouldBeNegated = false;
                if (literalInThen.ExpressionType.FullName == Constants.Int32 &&
                    literalInElse.ExpressionType.FullName == Constants.Int32)
                {
                    int valueInThen = (int)literalInThen.Value;
                    int valueInElse = (int)literalInElse.Value;
                    if ((valueInThen == 0 && valueInElse == 1) ||
                        (valueInThen == 1 && valueInElse == 0))
                    {
                        areValidValues = true;
                        if (valueInThen == 0 && valueInElse == 1)
                        {
                            shouldBeNegated = true;
                        }
                    }
                }
                else if (literalInThen.ExpressionType.FullName == Constants.Boolean &&
                         literalInElse.ExpressionType.FullName == Constants.Boolean)
                {
                    bool valueInThen = (bool)literalInThen.Value;
                    bool valueInElse = (bool)literalInElse.Value;
                    if ((valueInThen == false && valueInElse == true) ||
                        (valueInThen == true && valueInElse == false))
                    {
                        areValidValues = true;
                        if (valueInThen == false && valueInElse == true)
                        {
                            shouldBeNegated = true;
                        }
                    }
                }

                if (areValidValues)
                {
                    if (shouldBeNegated)
                    {
                        transformed = Transform(Negator.Negate(ternary.Condition, typeSystem));
                    }
                    else
                    {
                        transformed = Transform(ternary.Condition);
                    }

                    return true;
                }
            }

            if (literalInThen != null)
            {
                literal = literalInThen;
                isLiteralInThen = true;
            }
            else if (literalInElse != null)
            {
                literal = literalInElse;
                isLiteralInThen = false;
            }
            else // Transforms the case: someCondition ? (int)boolVar1 : (int)boolVar2
            {
                ExplicitCastExpression thenCast = ternary.Then as ExplicitCastExpression;
                ExplicitCastExpression elseCast = ternary.Else as ExplicitCastExpression;
                if (thenCast != null && elseCast != null &&
                    thenCast.TargetType.FullName == Constants.Int32 && thenCast.Expression.ExpressionType.FullName == Constants.Boolean &&
                    elseCast.TargetType.FullName == Constants.Int32 && elseCast.Expression.ExpressionType.FullName == Constants.Boolean)
                {
                    ternary.Then = thenCast.Expression;
                    ternary.Else = elseCast.Expression;

                    transformed = ternary;

                    return true;
                }

                return false;
            }

            bool isTrueLikeValue;
            if (literal.ExpressionType.FullName == Constants.Int32)
            {
                isTrueLikeValue = (int)literal.Value != 0 ? true : false;
            }
            else if (literal.ExpressionType.FullName == Constants.Boolean)
            {
                isTrueLikeValue = (bool)literal.Value;
            }
            else
            {
                return false;
            }

            BinaryOperator newOperator;
            Expression newLeft = null;
            Expression newRight = null;

            newLeft = Transform(ternary.Condition);

            Expression expression;
            if (isLiteralInThen)
            {
                expression = ternary.Else;
            }
            else
            {
                expression = ternary.Then;
            }

            ConditionExpression nestedTernary = expression as ConditionExpression;
            ExplicitCastExpression castExpression = expression as ExplicitCastExpression;
            Expression nestedBinaryAsExpression = null;
            bool isNestedBinaryInUnary = false;
            if (nestedTernary == null &&
                (castExpression == null || castExpression.TargetType.FullName != Constants.Int32 || castExpression.Expression.ExpressionType.FullName != Constants.Boolean) &&
                !IsBinaryExpression(expression, out nestedBinaryAsExpression, out isNestedBinaryInUnary) &&
                expression.ExpressionType.FullName != Constants.Boolean)
            {
                return false;
            }

            bool isRightTransformed = false;
            if (castExpression != null)
            {
                newRight = castExpression.Expression;
                isRightTransformed = true;
            }
            else
            {
                if (nestedBinaryAsExpression != null)
                {
                    BinaryExpression nestedBinary = GetBinaryExpression(isNestedBinaryInUnary, nestedBinaryAsExpression);
                    if (TryTransformBinary(nestedBinary))
                    {
                        newRight = GetResultExpression(isNestedBinaryInUnary, nestedBinaryAsExpression, nestedBinary);
                        isRightTransformed = true;
                    }
                }
                else if (nestedTernary != null && TryTransformTernary(nestedTernary, out newRight))
                {
                    isRightTransformed = true;
                }
            }

            if (!isRightTransformed)
            {
                newRight = expression;
            }

            if (newLeft == null || newRight == null)
            {
                return false;
            }

            if (isTrueLikeValue)
            {
                newOperator = BinaryOperator.LogicalOr;
                newLeft = isLiteralInThen ? newLeft : Negator.Negate(newLeft, this.typeSystem);
            }
            else
            {
                newOperator = BinaryOperator.LogicalAnd;
                newLeft = isLiteralInThen ? Negator.Negate(newLeft, this.typeSystem) : newLeft;
            }

            transformed = new BinaryExpression(newOperator, newLeft, newRight, this.typeSystem, ternary.MappedInstructions);
            return true;
        }

        private Expression Transform(Expression expression)
        {
            Expression result = null;

            ConditionExpression nestedTernaryInCondition = expression as ConditionExpression;
            Expression nestedBinaryInConditionAsExpression = null;
            bool isNestedBinaryInUnary = false;

            bool isLeftTransformed = false;
            if (IsBinaryExpression(expression, out nestedBinaryInConditionAsExpression, out isNestedBinaryInUnary))
            {
                BinaryExpression nestedBinaryInCondition = GetBinaryExpression(isNestedBinaryInUnary, nestedBinaryInConditionAsExpression);
                if (TryTransformBinary(nestedBinaryInCondition))
                {
                    result = GetResultExpression(isNestedBinaryInUnary, nestedBinaryInConditionAsExpression, nestedBinaryInCondition);
                    isLeftTransformed = true;
                }
            }
            else
            {
                if (nestedTernaryInCondition != null && TryTransformTernary(nestedTernaryInCondition, out result))
                {
                    isLeftTransformed = true;
                }
            }

            if (!isLeftTransformed)
            {
                result = expression;
            }

            return result;
        }

        private static BinaryExpression GetBinaryExpression(bool isNestedBinaryInUnary, Expression binaryAsExpression)
        {
            BinaryExpression nestedBinary;
            if (isNestedBinaryInUnary)
            {
                nestedBinary = (binaryAsExpression as UnaryExpression).Operand as BinaryExpression;
            }
            else
            {
                nestedBinary = binaryAsExpression as BinaryExpression;
            }

            return nestedBinary;
        }

        private static Expression GetResultExpression(bool isNestedBinaryInUnary, Expression binaryAsExpression, BinaryExpression newBinaryExpression)
        {
            Expression result;
            if (isNestedBinaryInUnary)
            {
                UnaryExpression unary = binaryAsExpression as UnaryExpression;
                unary.Operand = newBinaryExpression;
                result = unary;
            }
            else
            {
                result = newBinaryExpression;
            }

            return result;
        }

        private bool IsBinaryExpression(Expression expression, out Expression binaryExpression, out bool isWrappedInUnary)
        {
            binaryExpression = null;
            isWrappedInUnary = false;

            BinaryExpression nestedBinary = expression as BinaryExpression;
            UnaryExpression nestedUnary = expression as UnaryExpression;
            if (nestedBinary != null)
            {
                binaryExpression = nestedBinary;

                return true;
            }
            else if (nestedUnary != null &&
                     (nestedUnary.Operator == UnaryOperator.None || nestedUnary.Operator == UnaryOperator.LogicalNot) &&
                     nestedUnary.Operand is BinaryExpression)
            {
                if (nestedUnary.Operator == UnaryOperator.LogicalNot)
                {
                    binaryExpression = Negator.Negate(nestedUnary.Operand, typeSystem);
                    if (!(binaryExpression is BinaryExpression))
                    {
                        // Currently the only boolean expression that cannot be negated (without wrapping it in unary with logical not)
                        // is binary expression with XOR operator.
                        isWrappedInUnary = true;
                    }
                }
                else
                {
                    binaryExpression = nestedUnary.Operand;
                }

                return true;
            }

            return false;
        }
    }
}
