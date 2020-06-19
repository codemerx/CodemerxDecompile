using System;
using Telerik.JustDecompiler.Ast;
using Mono.Cecil;
using Telerik.JustDecompiler.Common;

namespace Telerik.JustDecompiler.Common
{
    internal static class DynamicHelper
    {
        public static BinaryOperator GetBinaryOperator(ExpressionType @operator)
        {
            switch (@operator)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.AddAssign:
                case ExpressionType.AddAssignChecked:
                    return BinaryOperator.Add;
                case ExpressionType.And:
                case ExpressionType.AndAssign:
                    return BinaryOperator.BitwiseAnd;
                case ExpressionType.AndAlso:
                    return BinaryOperator.LogicalAnd;
                case ExpressionType.Assign:
                    return BinaryOperator.Assign;
                case ExpressionType.Coalesce:
                    return BinaryOperator.NullCoalesce;
                case ExpressionType.Divide:
                case ExpressionType.DivideAssign:
                    return BinaryOperator.Divide;
                case ExpressionType.Equal:
                    return BinaryOperator.ValueEquality;
                case ExpressionType.ExclusiveOr:
                case ExpressionType.ExclusiveOrAssign:
                    return BinaryOperator.BitwiseXor;
                case ExpressionType.GreaterThan:
                    return BinaryOperator.GreaterThan;
                case ExpressionType.GreaterThanOrEqual:
                    return BinaryOperator.GreaterThanOrEqual;
                case ExpressionType.LeftShift:
                case ExpressionType.LeftShiftAssign:
                    return BinaryOperator.LeftShift;
                case ExpressionType.LessThan:
                    return BinaryOperator.LessThan;
                case ExpressionType.LessThanOrEqual:
                    return BinaryOperator.LessThanOrEqual;
                case ExpressionType.Modulo:
                case ExpressionType.ModuloAssign:
                    return BinaryOperator.Modulo;
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.MultiplyAssign:
                case ExpressionType.MultiplyAssignChecked:
                    return BinaryOperator.Multiply;
                case ExpressionType.NotEqual:
                    return BinaryOperator.ValueInequality;
                case ExpressionType.Or:
                case ExpressionType.OrAssign:
                    return BinaryOperator.BitwiseOr;
                case ExpressionType.OrElse:
                    return BinaryOperator.LogicalOr;
                case ExpressionType.RightShift:
                case ExpressionType.RightShiftAssign:
                    return BinaryOperator.RightShift;
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.SubtractAssign:
                case ExpressionType.SubtractAssignChecked:
                    return BinaryOperator.Subtract;
                default:
                    throw new Exception("Operator is not supported.");
            }
        }

        public static UnaryOperator GetUnaryOperator(ExpressionType @operator)
        {
            switch (@operator)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    return UnaryOperator.Negate;
                case ExpressionType.Not:
                case ExpressionType.IsFalse:
                    return UnaryOperator.LogicalNot;
                case ExpressionType.OnesComplement:
                    return UnaryOperator.BitwiseNot;
                case ExpressionType.PostDecrementAssign:
                    return UnaryOperator.PostDecrement;
                case ExpressionType.PostIncrementAssign:
                    return UnaryOperator.PostIncrement;
                case ExpressionType.PreDecrementAssign:
                case ExpressionType.Decrement:
                    return UnaryOperator.PreDecrement;
                case ExpressionType.PreIncrementAssign:
                case ExpressionType.Increment:
                    return UnaryOperator.PreIncrement;
                case ExpressionType.UnaryPlus:
                    return UnaryOperator.UnaryPlus;
                default:
                    throw new Exception("Operator is not supported.");
            }
        }

        public static bool[] GetDynamicPositioningFlags(CustomAttribute dynamicAttribute)
        {
            dynamicAttribute.Resolve();

            if (!dynamicAttribute.IsResolved)
            {
                throw new Exception("Could not resolve DynamicAttribute");
            }

            if (dynamicAttribute.ConstructorArguments.Count == 0)
            {
                return new bool[] { true };
            }

            if (dynamicAttribute.ConstructorArguments[0].Type.FullName != "System.Boolean[]")
            {
                throw new Exception("Invalid argument type for DynamicAttribute");
            }

            CustomAttributeArgument[] booleanArray = (CustomAttributeArgument[])dynamicAttribute.ConstructorArguments[0].Value;
            bool[] positioningFlags = new bool[booleanArray.Length];

            for (int i = 0; i < booleanArray.Length; i++)
            {
                positioningFlags[i] = (bool)booleanArray[i].Value;
            }

            return positioningFlags;
        }
    }
}
