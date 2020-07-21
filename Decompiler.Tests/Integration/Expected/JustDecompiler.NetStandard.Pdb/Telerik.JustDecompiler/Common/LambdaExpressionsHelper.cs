using Mono.Cecil;
using Mono.Collections.Generic;
using System;

namespace Telerik.JustDecompiler.Common
{
	internal static class LambdaExpressionsHelper
	{
		internal static bool HasAnonymousParameter(Collection<ParameterDefinition> parameters)
		{
			V_0 = parameters.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					if (!V_0.get_Current().get_ParameterType().ContainsAnonymousType())
					{
						continue;
					}
					V_1 = true;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				V_0.Dispose();
			}
		Label1:
			return V_1;
		Label0:
			return false;
		}
	}
}