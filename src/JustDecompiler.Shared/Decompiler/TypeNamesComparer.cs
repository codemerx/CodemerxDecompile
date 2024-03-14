using Mono.Cecil;
using Mono.Cecil.Extensions;
using System;
using System.Linq;

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

            TypeReference firstTypeRef = firstType.GetElementType();
            TypeReference secondTypeRef = secondType.GetElementType();
            if (firstTypeRef.HasGenericParameters && secondTypeRef.HasGenericParameters)
            {
                if (firstTypeRef.FullName == secondTypeRef.FullName &&
                    firstTypeRef.GenericParameters.Count == secondTypeRef.GenericParameters.Count)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
