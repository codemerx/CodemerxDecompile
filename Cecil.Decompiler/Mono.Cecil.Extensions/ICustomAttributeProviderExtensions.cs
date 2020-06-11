using System;
using System.Collections.Generic;

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
			if (!attributeProvider.HasCustomAttributes)
			{
				return false;
			}
			foreach (CustomAttribute attr in attributeProvider.CustomAttributes)
			{
				foreach (string attributeType in attributeTypes)
				{
					if (attr.AttributeType.FullName == attributeType)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}