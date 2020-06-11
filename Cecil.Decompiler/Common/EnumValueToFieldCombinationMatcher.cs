using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace Telerik.JustDecompiler.Common
{
	internal class EnumValueToFieldCombinationMatcher
	{
		public static List<FieldDefinition> GetEnumFieldDefinitionByValue(Mono.Collections.Generic.Collection<FieldDefinition> fieldDefinitions, object value, Collection<Mono.Cecil.CustomAttribute> customAttributes)
		{
			bool flagsEnum = false;
			foreach (CustomAttribute attribute in customAttributes)
			{
				if (attribute.AttributeType.FullName == "System.FlagsAttribute")
				{
					flagsEnum = true;
				}
			}

			long valueAsInt64;
			if (value is string)
			{
				if (!long.TryParse((string) value, out valueAsInt64))
				{
					valueAsInt64 = 0;
				}
			}
			else
			{
				valueAsInt64 = Convert.ToInt64(value);
			}
			List<FieldDefinition> result = new List<Mono.Cecil.FieldDefinition>();

			for (int i = 1; i < fieldDefinitions.Count; i++)
			{
				if (fieldDefinitions[i].Constant == null || fieldDefinitions[i].Constant.Value == null)
					continue;

				//Checking whether value corresponds directly to an enum field
				if (fieldDefinitions[i].Constant.Value.Equals(value))
				{
					result.Clear();
					result.Add(fieldDefinitions[i]);
					return result;
				}

				if (flagsEnum)
				{
					////After this there might be duplicates, i.e. assuming that there are enum.A, enum.B and enum.AB where enum.AB = enum.A | enum.B all the three are gonna be present in the result. 
					////Which is correct although ugly. TODO: Fix duplication of flags in enums.
					//Assuming the largest underlying type of an enum is 64-bit signed/unsigned int
					long enumConstantasInt64 = Convert.ToInt64(fieldDefinitions[i].Constant.Value);
					if (enumConstantasInt64 != 0)
					{
						if ((enumConstantasInt64 & valueAsInt64) == enumConstantasInt64)
						{
							result.Add(fieldDefinitions[i]);
						}
					}
					else
					{
						if (valueAsInt64 == 0)
						{
							result.Add(fieldDefinitions[i]);
						}
					}
				}
			}

			return result;
		}
	}
}
