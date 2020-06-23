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
			if (elementType.HasGenericParameters && typeReference.HasGenericParameters && elementType.FullName == typeReference.FullName && elementType.GenericParameters.Count == typeReference.GenericParameters.Count)
			{
				return true;
			}
			return false;
		}
	}
}