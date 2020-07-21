using Mono.Cecil;
using Mono.Collections.Generic;
using System;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
	internal static class ExpressionTypeInferer
	{
		internal static TypeReference GetContainingType(TypeDefinition leftType, TypeDefinition rightType)
		{
			if ((object)leftType == (object)rightType)
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
			V_0 = type.Resolve();
			if (V_0 != null && V_0.get_IsEnum() && V_0 != null)
			{
				V_1 = null;
				V_2 = V_0.get_Fields().GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = V_2.get_Current();
						if (!String.op_Equality(V_3.get_Name(), "value__"))
						{
							continue;
						}
						V_1 = V_3;
						goto Label0;
					}
				}
				finally
				{
					V_2.Dispose();
				}
			Label0:
				type = V_1.get_FieldType();
			}
			V_4 = type.get_FullName();
			if (V_4 != null)
			{
				if (String.op_Equality(V_4, "System.Boolean"))
				{
					return 0;
				}
				if (String.op_Equality(V_4, "System.SByte") || String.op_Equality(V_4, "System.Byte"))
				{
					return 1;
				}
				if (String.op_Equality(V_4, "System.Char") || String.op_Equality(V_4, "System.Int16") || String.op_Equality(V_4, "System.UInt16"))
				{
					return 2;
				}
				if (String.op_Equality(V_4, "System.Int32") || String.op_Equality(V_4, "System.UInt32"))
				{
					return 3;
				}
				if (String.op_Equality(V_4, "System.IntPtr") || String.op_Equality(V_4, "System.Int64") || String.op_Equality(V_4, "System.UInt64"))
				{
					return 4;
				}
				if (String.op_Equality(V_4, "System.Single"))
				{
					return 5;
				}
				if (String.op_Equality(V_4, "System.Double"))
				{
					return 6;
				}
			}
			throw new NotSupportedException("Not supported type.");
		}
	}
}