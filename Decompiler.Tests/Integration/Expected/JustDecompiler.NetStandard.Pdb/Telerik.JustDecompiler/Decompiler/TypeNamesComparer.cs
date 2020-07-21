using Mono.Cecil;
using System;

namespace Telerik.JustDecompiler.Decompiler
{
	internal static class TypeNamesComparer
	{
		public static bool AreEqual(TypeReference firstType, TypeReference secondType)
		{
			if (String.op_Equality(firstType.GetFriendlyFullName(null), secondType.GetFriendlyFullName(null)))
			{
				return true;
			}
			V_0 = firstType.GetElementType();
			V_1 = secondType.GetElementType();
			if (V_0.get_HasGenericParameters() && V_1.get_HasGenericParameters() && String.op_Equality(V_0.get_FullName(), V_1.get_FullName()) && V_0.get_GenericParameters().get_Count() == V_1.get_GenericParameters().get_Count())
			{
				return true;
			}
			return false;
		}
	}
}