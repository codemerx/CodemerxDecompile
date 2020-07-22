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
			V_0 = self.get_GenericArguments().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					if (!V_0.get_Current().ContainsAnonymousType())
					{
						continue;
					}
					V_1 = true;
					goto Label0;
				}
			}
			finally
			{
				V_0.Dispose();
			}
			V_2 = self.get_PostionToArgument().get_Values().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					if (!V_2.get_Current().ContainsAnonymousType())
					{
						continue;
					}
					V_1 = true;
					goto Label0;
				}
				goto Label1;
			}
			finally
			{
				((IDisposable)V_2).Dispose();
			}
		Label0:
			return V_1;
		Label1:
			return false;
		}
	}
}