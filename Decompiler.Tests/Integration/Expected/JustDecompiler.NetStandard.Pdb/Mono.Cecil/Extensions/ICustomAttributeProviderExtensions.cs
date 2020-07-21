using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class ICustomAttributeProviderExtensions
	{
		public static bool HasCustomAttribute(this ICustomAttributeProvider attributeProvider, IEnumerable<string> attributeTypes)
		{
			if (attributeProvider == null || attributeTypes == null)
			{
				return false;
			}
			if (!attributeProvider.get_HasCustomAttributes())
			{
				return false;
			}
			V_0 = attributeProvider.get_CustomAttributes().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = attributeTypes.GetEnumerator();
					try
					{
						while (V_2.MoveNext())
						{
							V_3 = V_2.get_Current();
							if (!String.op_Equality(V_1.get_AttributeType().get_FullName(), V_3))
							{
								continue;
							}
							V_4 = true;
							goto Label1;
						}
					}
					finally
					{
						if (V_2 != null)
						{
							V_2.Dispose();
						}
					}
				}
				goto Label0;
			}
			finally
			{
				V_0.Dispose();
			}
		Label1:
			return V_4;
		Label0:
			return false;
		}
	}
}