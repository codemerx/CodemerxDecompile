using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;

namespace Telerik.JustDecompiler.Common
{
	internal static class LambdaExpressionsHelper
	{
		internal static bool HasAnonymousParameter(Collection<ParameterDefinition> parameters)
		{
			bool flag;
			Collection<ParameterDefinition>.Enumerator enumerator = parameters.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.get_Current().get_ParameterType().ContainsAnonymousType())
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				enumerator.Dispose();
			}
			return flag;
		}
	}
}