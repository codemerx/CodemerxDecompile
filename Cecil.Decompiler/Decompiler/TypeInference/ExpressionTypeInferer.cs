using System;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
    /// <summary>
    /// Helper class, containing functions for resolving types of expressions, or part of expressions.
    /// </summary>
    static class ExpressionTypeInferer
    {
        /// <summary>
        /// Works only on primitive types.
        /// </summary>
        /// <param name="leftType"></param>
        /// <param name="rightType"></param>
        /// <returns>Reference to the bigger of the supplied types.</returns>
        internal static TypeReference GetContainingType(TypeDefinition leftType, TypeDefinition rightType)
        {
            if (leftType == rightType)
            {
                return rightType;
            }
            if (leftType == null)
            {
                return rightType;
            }
            if (rightType == null)
            {
                return leftType;
            }

            int leftSideIndex = GetTypeIndex(leftType);
            int rightSideIndex = GetTypeIndex(rightType);
            if (leftSideIndex > rightSideIndex)
            {
                return leftType;
            }
            else
            {
                return rightType;
            }
        }

        /// <summary>
        /// Gets and integer value, representing the size of the type. Works only on primitive types and enumerations.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Returns integer representation of the type. The bigger the integer, the bigger the type.</returns>
        internal static int GetTypeIndex(TypeReference type)
        {
            TypeDefinition typeDef = type.Resolve();
            if (typeDef != null && typeDef.IsEnum)
            {
                if (typeDef != null)
                {
                    FieldDefinition valueField = null;
                    foreach (FieldDefinition x in typeDef.Fields)
                    {
                        if (x.Name == "value__")
                        {
                            valueField = x;
                            break;
                        }
                    }
                    type = valueField.FieldType;
                }
            }

            switch (type.FullName)
            {
                case "System.Boolean":
                    return 0;
                case "System.SByte":
                case "System.Byte":
                    return 1;
                case "System.Char":
                case "System.Int16":
                case "System.UInt16":
                    return 2;
                case "System.Int32":
                case "System.UInt32":
                    return 3;
                case "System.IntPtr":
                case "System.Int64":
                case "System.UInt64":
                    return 4;
                case "System.Single":
                    return 5;
                case "System.Double":
                    return 6;
                default:
                    throw new NotSupportedException("Not supported type.");
            }
        }
    }
}
