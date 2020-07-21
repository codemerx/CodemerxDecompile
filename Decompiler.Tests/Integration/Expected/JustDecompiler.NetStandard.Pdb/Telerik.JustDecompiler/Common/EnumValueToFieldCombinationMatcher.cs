using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Common
{
	internal class EnumValueToFieldCombinationMatcher
	{
		public EnumValueToFieldCombinationMatcher()
		{
			base();
			return;
		}

		public static List<FieldDefinition> GetEnumFieldDefinitionByValue(Collection<FieldDefinition> fieldDefinitions, object value, Collection<CustomAttribute> customAttributes)
		{
			V_0 = false;
			V_3 = customAttributes.GetEnumerator();
			try
			{
				while (V_3.MoveNext())
				{
					if (!String.op_Equality(V_3.get_Current().get_AttributeType().get_FullName(), "System.FlagsAttribute"))
					{
						continue;
					}
					V_0 = true;
				}
			}
			finally
			{
				V_3.Dispose();
			}
			if (value as String == null)
			{
				V_1 = Convert.ToInt64(value);
			}
			else
			{
				if (!Int64.TryParse((String)value, out V_1))
				{
					V_1 = (long)0;
				}
			}
			V_2 = new List<FieldDefinition>();
			V_4 = 1;
			while (V_4 < fieldDefinitions.get_Count())
			{
				if (fieldDefinitions.get_Item(V_4).get_Constant() != null && fieldDefinitions.get_Item(V_4).get_Constant().get_Value() != null)
				{
					if (fieldDefinitions.get_Item(V_4).get_Constant().get_Value().Equals(value))
					{
						V_2.Clear();
						V_2.Add(fieldDefinitions.get_Item(V_4));
						return V_2;
					}
					if (V_0)
					{
						V_5 = Convert.ToInt64(fieldDefinitions.get_Item(V_4).get_Constant().get_Value());
						if (V_5 == 0)
						{
							if (V_1 == 0)
							{
								V_2.Add(fieldDefinitions.get_Item(V_4));
							}
						}
						else
						{
							if (V_5 & V_1 == V_5)
							{
								V_2.Add(fieldDefinitions.get_Item(V_4));
							}
						}
					}
				}
				V_4 = V_4 + 1;
			}
			return V_2;
		}
	}
}