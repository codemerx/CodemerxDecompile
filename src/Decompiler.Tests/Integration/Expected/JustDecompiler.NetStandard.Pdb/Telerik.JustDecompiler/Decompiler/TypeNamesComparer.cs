using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;

namespace Telerik.JustDecompiler.Decompiler
{
	internal static class TypeNamesComparer
	{
		public static bool AreEqual(TypeReference firstType, TypeReference secondType)
		{
			if (firstType.GetFriendlyFullName(null) == secondType.GetFriendlyFullName(null))
			{
				return true;
			}
			TypeReference elementType = firstType.GetElementType();
			TypeReference typeReference = secondType.GetElementType();
			if (elementType.get_HasGenericParameters() && typeReference.get_HasGenericParameters() && elementType.get_FullName() == typeReference.get_FullName() && elementType.get_GenericParameters().get_Count() == typeReference.get_GenericParameters().get_Count())
			{
				return true;
			}
			return false;
		}
	}
}