using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	internal static class IGenericInstanceExtensions
	{
		public static bool HasAnonymousArgument(this IGenericInstance self)
		{
			bool flag;
			foreach (TypeReference genericArgument in self.get_GenericArguments())
			{
				if (!genericArgument.ContainsAnonymousType())
				{
					continue;
				}
				flag = true;
				return flag;
			}
			Dictionary<int, TypeReference>.ValueCollection.Enumerator enumerator = self.get_PostionToArgument().Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.ContainsAnonymousType())
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
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}
	}
}