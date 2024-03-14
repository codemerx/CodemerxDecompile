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
				TypeReference declaringType = (self as TypeReference).get_DeclaringType();
				if (declaringType != null)
				{
					count = declaringType.get_GenericParameters().get_Count();
				}
			}
			while (count < self.get_GenericParameters().get_Count())
			{
				yield return self.get_GenericParameters().get_Item(count);
				count++;
			}
		}
	}
}