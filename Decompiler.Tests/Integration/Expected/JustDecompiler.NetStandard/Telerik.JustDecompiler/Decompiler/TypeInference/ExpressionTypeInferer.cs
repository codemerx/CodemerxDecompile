using Mono.Cecil;
using Mono.Collections.Generic;
using System;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
	internal static class ExpressionTypeInferer
	{
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
			if (ExpressionTypeInferer.GetTypeIndex(leftType) > ExpressionTypeInferer.GetTypeIndex(rightType))
			{
				return leftType;
			}
			return rightType;
		}

		internal static int GetTypeIndex(TypeReference type)
		{
			TypeDefinition typeDefinition = type.Resolve();
			if (typeDefinition != null && typeDefinition.IsEnum && typeDefinition != null)
			{
				FieldDefinition fieldDefinition = null;
				foreach (FieldDefinition field in typeDefinition.Fields)
				{
					if (field.Name != "value__")
					{
						continue;
					}
					fieldDefinition = field;
					goto Label0;
				}
			Label0:
				type = fieldDefinition.FieldType;
			}
			string fullName = type.FullName;
			if (fullName != null)
			{
				if (fullName == "System.Boolean")
				{
					return 0;
				}
				if (fullName == "System.SByte" || fullName == "System.Byte")
				{
					return 1;
				}
				if (fullName == "System.Char" || fullName == "System.Int16" || fullName == "System.UInt16")
				{
					return 2;
				}
				if (fullName == "System.Int32" || fullName == "System.UInt32")
				{
					return 3;
				}
				if (fullName == "System.IntPtr" || fullName == "System.Int64" || fullName == "System.UInt64")
				{
					return 4;
				}
				if (fullName == "System.Single")
				{
					return 5;
				}
				if (fullName == "System.Double")
				{
					return 6;
				}
			}
			throw new NotSupportedException("Not supported type.");
		}
	}
}