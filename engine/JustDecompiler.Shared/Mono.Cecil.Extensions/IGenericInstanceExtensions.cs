using System;

namespace Mono.Cecil.Extensions
{
    static class IGenericInstanceExtensions
    {
        public static bool HasAnonymousArgument(this IGenericInstance self)
        {
			//for (int i = 0; i < self.GenericArguments.Count; i++)
			//{
			//    TypeReference argument = self.GenericArguments[i];
			//    if (self.PostionToArgument.ContainsKey(i))
			//    {
			//        argument = self.PostionToArgument[i];
			//    }
			//    if (argument.ContainsAnonymousType())
			//    {
			//        return true;
			//    }
			//}
			foreach (TypeReference typeRef in self.GenericArguments)
			{
				if (typeRef.ContainsAnonymousType())
				{
					return true;
				}
			}

			foreach (TypeReference item in self.PostionToArgument.Values)
			{
				if (item.ContainsAnonymousType())
				{
					return true;
				}
			}

            return false;
        }
    }
}
