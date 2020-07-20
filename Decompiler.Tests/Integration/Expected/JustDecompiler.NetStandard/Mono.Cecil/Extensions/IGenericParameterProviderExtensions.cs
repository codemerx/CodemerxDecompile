using Mono.Cecil;
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
			stackVariable1 = new IGenericParameterProviderExtensions.u003cGetGenericParametersu003ed__0(-2);
			stackVariable1.u003cu003e3__self = self;
			return stackVariable1;
		}
	}
}