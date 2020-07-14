using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class ICustomAttributeProviderExtensions
	{
		public static bool HasCustomAttribute(this ICustomAttributeProvider attributeProvider, IEnumerable<string> attributeTypes)
		{
			bool flag;
			if (attributeProvider == null || attributeTypes == null)
			{
				return false;
			}
			if (!attributeProvider.get_HasCustomAttributes())
			{
				return false;
			}
			Collection<CustomAttribute>.Enumerator enumerator = attributeProvider.get_CustomAttributes().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					CustomAttribute current = enumerator.get_Current();
					using (IEnumerator<string> enumerator1 = attributeTypes.GetEnumerator())
					{
						while (enumerator1.MoveNext())
						{
							string str = enumerator1.Current;
							if (current.get_AttributeType().get_FullName() != str)
							{
								continue;
							}
							flag = true;
							return flag;
						}
					}
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