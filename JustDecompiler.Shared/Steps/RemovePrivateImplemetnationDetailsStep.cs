using System;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Steps
{
    class RemovePrivateImplementationDetailsStep
    {
        private readonly TypeSystem typeSystem;

        public RemovePrivateImplementationDetailsStep(TypeSystem typeSystem)
        {
            this.typeSystem = typeSystem;
        }

        public ICodeNode VisitArrayCreationExpression(ArrayCreationExpression node)
        {
            if (node.Initializer == null || node.Initializer.Expressions == null || node.Initializer.Expressions.Count != 1)
            {
                return null;
            }

            MemberHandleExpression runtimeHandle = node.Initializer.Expressions[0] as MemberHandleExpression;

            if (runtimeHandle == null)
            {
                return null;
            }

            if (!TryFillInitializer(node, runtimeHandle))
            {
                return null;
            }

            return node;
        }

        private bool TryFillInitializer(ArrayCreationExpression expression, MemberHandleExpression values)
        {
            if (!(values.MemberReference is FieldDefinition))
            {
                return false;
            }
            ExpressionCollection convertedExpressions = ConvertInitialValues((values.MemberReference as FieldDefinition).InitialValue, expression.ElementType.Name);
            if(convertedExpressions == null || CheckElementsCount(convertedExpressions, expression.Dimensions) == false)
            {
                return false;
            }
            RebuildDimensions(ref convertedExpressions, expression.Dimensions);
            expression.Initializer.Expressions = convertedExpressions;
            return true;
        }

        private ExpressionCollection ConvertInitialValues(byte[] initialValues, string typeName)
        {
            ExpressionCollection literals = new ExpressionCollection();
            switch (typeName)
            {
                case "Boolean":
                    for (int j = 0; j < initialValues.Length; j++)
                    {
                        literals.Add(GetLiteralExpression(initialValues[j] != 0));
                    }
                    return literals;
                case "SByte":
                    for (int j = 0; j < initialValues.Length; j++)
                    {
                        literals.Add(GetLiteralExpression((sbyte)initialValues[j]));
                    }
                    return literals;
                case "Byte":
                    for (int j = 0; j < initialValues.Length; j++)
                    {
                        literals.Add(GetLiteralExpression(initialValues[j]));
                    }
                    return literals;
                case "Char":
                    for (int j = 0; j < initialValues.Length / 2; j++)
                    {
                        literals.Add(GetLiteralExpression(BitConverter.ToChar(initialValues, j * 2)));
                    }
                    return literals;
                case "Int16":
                    for (int j = 0; j < initialValues.Length / 2; j++)
                    {
                        literals.Add(GetLiteralExpression(BitConverter.ToInt16(initialValues, j * 2)));
                    }
                    return literals;
                case "UInt16":
                    for (int j = 0; j < initialValues.Length / 2; j++)
                    {
                        literals.Add(GetLiteralExpression(BitConverter.ToUInt16(initialValues, j * 2)));
                    }
                    return literals;
                case "Int32":
                    for (int j = 0; j < initialValues.Length / 4; j++)
                    {
                        literals.Add(GetLiteralExpression(BitConverter.ToInt32(initialValues, j * 4)));
                    }
                    return literals;
                case "UInt32":
                    for (int j = 0; j < initialValues.Length / 4; j++)
                    {
                        literals.Add(GetLiteralExpression(BitConverter.ToUInt32(initialValues, j * 4)));
                    }
                    return literals;
                case "Int64":
                    for (int j = 0; j < initialValues.Length / 8; j++)
                    {
                        literals.Add(GetLiteralExpression(BitConverter.ToInt64(initialValues, j * 8)));
                    }
                    return literals;
                case "UInt64":
                    for (int j = 0; j < initialValues.Length / 8; j++)
                    {
                        literals.Add(GetLiteralExpression(BitConverter.ToUInt64(initialValues, j * 8)));
                    }
                    return literals;
                case "Single":
                    for (int j = 0; j < initialValues.Length / 4; j++)
                    {
                        literals.Add(GetLiteralExpression(BitConverter.ToSingle(initialValues, j * 4)));
                    }
                    return literals;
                case "Double":
                    for (int j = 0; j < initialValues.Length / 8; j++)
                    {
                        literals.Add(GetLiteralExpression(BitConverter.ToDouble(initialValues, j * 8)));
                    }
                    return literals;
                default:
                    return null;
            }
        }

        private LiteralExpression GetLiteralExpression(object value)
        {
			return new LiteralExpression(value, typeSystem, null);
        }

        private bool CheckElementsCount(ExpressionCollection elements, ExpressionCollection dimensions)
        {
            int neededCount = 1;
            foreach (var literal in dimensions)
            {
                neededCount *= (int)(literal as LiteralExpression).Value;
            }
            return neededCount == elements.Count;
        }

        public void RebuildDimensions(ref ExpressionCollection elements, ExpressionCollection dimensions)
        {
            int count = elements.Count;
            for (int i = dimensions.Count - 1; i > 0; i--)
            {
                ExpressionCollection currentCollection = new ExpressionCollection();
                int currentDimension = (int)(dimensions[i] as LiteralExpression).Value;
                for (int j = 0; j < count; j += currentDimension)
                {
                    BlockExpression currentBlock = new BlockExpression(null);
                    currentBlock.Expressions = new ExpressionCollection();
                    for (int k = 0; k < currentDimension; k++)
                    {
                        currentBlock.Expressions.Add(elements[j + k]);
                    }
                    currentCollection.Add(currentBlock);
                }
                elements = currentCollection;
                count /= currentDimension;
            }
        }
    }
}
