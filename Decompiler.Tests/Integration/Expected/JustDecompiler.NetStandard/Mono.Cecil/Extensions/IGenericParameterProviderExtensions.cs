using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class IGenericParameterProviderExtensions
	{
		public static IEnumerable<GenericParameter> GetGenericParameters(IGenericParameterProvider self)
		{
			int count = 0;
			if (self is TypeReference)
			{
				TypeReference declaringType = (self as TypeReference).DeclaringType;
				if (declaringType != null)
				{
					count = declaringType.GenericParameters.Count;
				}
			}
			while (count < self.GenericParameters.Count)
			{
				yield return self.GenericParameters[count];
				count++;
			}
		}
	}
}