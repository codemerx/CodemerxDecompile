using System.Collections.Generic;
using Mono.Cecil;
using Mono.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	public static class EnumHelper
	{
		public static Expression GetEnumExpression(TypeDefinition enumDefinition, LiteralExpression targetedValue, TypeSystem typeSystem)
		{
			int enumDefinitionUnderlyingTypeSize = GetEnumBitSize(enumDefinition);

			/// This depends on the unchecked context, which is default for C#
			ulong value = 0;
            switch (targetedValue.Value.GetType().FullName)
            {
                case "System.Int32":
                    /// This is done, so that the same size extension is used when casting the literal's value to ulong 
                    /// and when casting anum's contstants to ulong.
                    /// Otherwise, casting int -1 and long -1 to ulong will yield different results.
                    if (enumDefinitionUnderlyingTypeSize == 32)
                    {
                        value = (uint)(int)targetedValue.Value;
                    }
                    else
                    {
                        value = (ulong)(int)targetedValue.Value;
                    }
                    break;
                case "System.Int64":
                    value = (ulong)(long)targetedValue.Value;
                    break;
                case "System.UInt32":
                    value = (uint)targetedValue.Value;
                    break;
                case "System.UInt64":
                    value = (ulong)targetedValue.Value;
                    break;
                // The types below should not be present at any point, but are added just to complete all the possible cases.
                case "System.Byte":
                    value = (byte)targetedValue.Value;
                    break;
                case "System.SByte":
                    value = (ulong)(sbyte)targetedValue.Value;
                    break;
                case "System.Int16":
                    value = (ulong)(short)targetedValue.Value;
                    break;
                case "System.UInt16":
                    value = (ushort)targetedValue.Value;
                    break;
            }

			Collection<FieldDefinition> fields = enumDefinition.Fields;
			List<FieldDefinition> enumFields = new List<FieldDefinition>();
			foreach (FieldDefinition currentField in fields)
			{
				if (currentField.Constant != null && currentField.Constant.Value != null)
				{
					ulong fieldValue = 0;

                    switch (currentField.Constant.Value.GetType().FullName)
                    {
                        case "System.Int32": // most common case
						    fieldValue = (uint)(int)(currentField.Constant.Value);
                            break;
                        case "System.UInt32":
						    fieldValue = (uint)(currentField.Constant.Value);
                            break;
                        case "System.Byte":
						    fieldValue = (byte)(currentField.Constant.Value);
                            break;
                        case "System.SByte":
						    fieldValue = (byte)(sbyte)(currentField.Constant.Value);
                            break;
                        case "System.Int16":
						    fieldValue = (ushort)(short)(currentField.Constant.Value);
                            break;
                        case "System.UInt16":
						    fieldValue = (ushort)(currentField.Constant.Value);
                            break;
                        case "System.Int64":
						    fieldValue = (ulong)(long)(currentField.Constant.Value);
                            break;
                        case "System.UInt64":
						    fieldValue = (ulong)(currentField.Constant.Value);
                            break;
                    }
					if (fieldValue == value)
					{
						return new EnumExpression(currentField, targetedValue.UnderlyingSameMethodInstructions);
					}
					if (fieldValue != 0 && (fieldValue | value) == value)
					//if (IsPartOfValue(fieldValue, value))
					{
						///Then this one of the flags, used to generate the value.
						enumFields.Add(currentField);
					}
				}
			}

			///Generate the expression of the flags.
			if (enumFields.Count < 2)
			{
				return targetedValue;
			}
			Expression result = new BinaryExpression(BinaryOperator.BitwiseOr, new EnumExpression(enumFields[0], null), new EnumExpression(enumFields[1], null), typeSystem, null);
			result.ExpressionType = enumDefinition;
			for (int i = 2; i < enumFields.Count; i++)
			{
				result = new BinaryExpression(BinaryOperator.BitwiseOr, result, new EnumExpression(enumFields[i], null), typeSystem, null);
				result.ExpressionType = enumDefinition;
			}

			return result.CloneAndAttachInstructions(targetedValue.UnderlyingSameMethodInstructions);
		}
  
		private static int GetEnumBitSize(TypeDefinition enumDefinition)
		{
			var enumImplementationType = enumDefinition.Fields[0].FieldType;
			if (enumImplementationType.FullName == "System.Int32" || enumImplementationType.FullName == "System.UInt32")
			{
				return 32;
			}
			if (enumImplementationType.FullName == "System.Int64" || enumImplementationType.FullName == "System.UInt64")
			{
				return 64;
			}
			if (enumImplementationType.FullName == "System.Int16" || enumImplementationType.FullName == "System.UInt16")
			{
				return 16;
			}
			if (enumImplementationType.FullName == "System.Byte" || enumImplementationType.FullName == "System.SByte")
			{
				return 8;
			}
			return -1;
		}
	}
}
