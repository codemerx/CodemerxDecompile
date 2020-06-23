using Mono.Cecil;
using Mono.Cecil.Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Common
{
	internal class EnumValueToFieldCombinationMatcher
	{
		public EnumValueToFieldCombinationMatcher()
		{
		}

		public static List<FieldDefinition> GetEnumFieldDefinitionByValue(Collection<FieldDefinition> fieldDefinitions, object value, Collection<CustomAttribute> customAttributes)
		{
			long num;
			bool flag = false;
			foreach (CustomAttribute customAttribute in customAttributes)
			{
				if (customAttribute.AttributeType.FullName != "System.FlagsAttribute")
				{
					continue;
				}
				flag = true;
			}
			if (!(value is String))
			{
				num = Convert.ToInt64(value);
			}
			else if (!Int64.TryParse((String)value, out num))
			{
				num = (long)0;
			}
			List<FieldDefinition> fieldDefinitions1 = new List<FieldDefinition>();
			for (int i = 1; i < fieldDefinitions.Count; i++)
			{
				if (fieldDefinitions[i].Constant != null && fieldDefinitions[i].Constant.Value != null)
				{
					if (fieldDefinitions[i].Constant.Value.Equals(value))
					{
						fieldDefinitions1.Clear();
						fieldDefinitions1.Add(fieldDefinitions[i]);
						return fieldDefinitions1;
					}
					if (flag)
					{
						long num1 = Convert.ToInt64(fieldDefinitions[i].Constant.Value);
						if (num1 != 0)
						{
							if ((num1 & num) == num1)
							{
								fieldDefinitions1.Add(fieldDefinitions[i]);
							}
						}
						else if (num == 0)
						{
							fieldDefinitions1.Add(fieldDefinitions[i]);
						}
					}
				}
			}
			return fieldDefinitions1;
		}
	}
}