using System.Collections.Generic;

namespace Mono.Cecil.Extensions
{
	public static class IGenericParameterProviderExtensions
	{
		public static IEnumerable<GenericParameter> GetGenericParameters(this IGenericParameterProvider self)
		{
			int i = 0;
			if (self is TypeReference)
			{
				TypeReference typeRef = self as TypeReference;
				TypeReference parentTypeRef = typeRef.DeclaringType;
				if (parentTypeRef != null)
				{
					i = parentTypeRef.GenericParameters.Count;
				}
			}

			for (; i < self.GenericParameters.Count; i++)
			{
				yield return self.GenericParameters[i];
			}
		}
	}
}
