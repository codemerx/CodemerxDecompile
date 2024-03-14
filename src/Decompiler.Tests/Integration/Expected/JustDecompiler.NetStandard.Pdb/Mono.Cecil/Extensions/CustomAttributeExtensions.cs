using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class CustomAttributeExtensions
	{
		private static int CompareConstructorArguments(CustomAttribute first, CustomAttribute second)
		{
			if (first.get_HasConstructorArguments() && !second.get_HasConstructorArguments())
			{
				return 1;
			}
			if (!first.get_HasConstructorArguments() && second.get_HasConstructorArguments())
			{
				return -1;
			}
			int num = Math.Max(first.get_ConstructorArguments().get_Count(), second.get_ConstructorArguments().get_Count());
			for (int i = 0; i < num; i++)
			{
				if (first.get_ConstructorArguments().get_Count() <= i)
				{
					return 1;
				}
				if (second.get_ConstructorArguments().get_Count() <= i)
				{
					return -1;
				}
				CustomAttributeArgument item = first.get_ConstructorArguments().get_Item(i);
				CustomAttributeArgument customAttributeArgument = second.get_ConstructorArguments().get_Item(i);
				int num1 = CustomAttributeExtensions.CompareCustomAttributeArguments(item, customAttributeArgument);
				if (num1 != 0)
				{
					return num1;
				}
			}
			return 0;
		}

		private static int CompareConstructorFields(CustomAttribute first, CustomAttribute second)
		{
			if (first.get_HasFields() && !second.get_HasFields())
			{
				return 1;
			}
			if (!first.get_HasFields() && second.get_HasFields())
			{
				return -1;
			}
			int num = Math.Max(first.get_Fields().get_Count(), second.get_Fields().get_Count());
			for (int i = 0; i < num; i++)
			{
				if (first.get_Fields().get_Count() <= i)
				{
					return 1;
				}
				if (second.get_Fields().get_Count() <= i)
				{
					return -1;
				}
				CustomAttributeNamedArgument item = first.get_Fields().get_Item(i);
				CustomAttributeNamedArgument customAttributeNamedArgument = second.get_Fields().get_Item(i);
				int num1 = item.get_Name().CompareTo(customAttributeNamedArgument.get_Name());
				if (num1 != 0)
				{
					return num1;
				}
				int num2 = CustomAttributeExtensions.CompareCustomAttributeArguments(item.get_Argument(), customAttributeNamedArgument.get_Argument());
				if (num2 != 0)
				{
					return num2;
				}
			}
			return 0;
		}

		private static int CompareConstructorProperties(CustomAttribute first, CustomAttribute second)
		{
			if (first.get_HasProperties() && !second.get_HasProperties())
			{
				return 1;
			}
			if (!first.get_HasProperties() && second.get_HasProperties())
			{
				return -1;
			}
			int num = Math.Max(first.get_Properties().get_Count(), second.get_Properties().get_Count());
			for (int i = 0; i < num; i++)
			{
				if (first.get_Properties().get_Count() <= i)
				{
					return 1;
				}
				if (second.get_Properties().get_Count() <= i)
				{
					return -1;
				}
				CustomAttributeNamedArgument item = first.get_Properties().get_Item(i);
				CustomAttributeNamedArgument customAttributeNamedArgument = second.get_Properties().get_Item(i);
				int num1 = item.get_Name().CompareTo(customAttributeNamedArgument.get_Name());
				if (num1 != 0)
				{
					return num1;
				}
				int num2 = CustomAttributeExtensions.CompareCustomAttributeArguments(item.get_Argument(), customAttributeNamedArgument.get_Argument());
				if (num2 != 0)
				{
					return num2;
				}
			}
			return 0;
		}

		private static int CompareCustomAttributeArguments(CustomAttributeArgument firstArgument, CustomAttributeArgument secondArgument)
		{
			return firstArgument.get_Value().ToString().CompareTo(secondArgument.get_Value().ToString());
		}

		public static int CompareToCustomAttribute(this CustomAttribute first, CustomAttribute second, bool fullNameCheck = false)
		{
			if (first.get_AttributeType().get_Name() != second.get_AttributeType().get_Name())
			{
				if (fullNameCheck)
				{
					return first.get_AttributeType().get_FullName().CompareTo(second.get_AttributeType().get_FullName());
				}
				return first.get_AttributeType().get_Name().CompareTo(second.get_AttributeType().get_Name());
			}
			if ((object)first == (object)second)
			{
				return 0;
			}
			int num = CustomAttributeExtensions.CompareConstructorArguments(first, second);
			if (num != 0)
			{
				return num;
			}
			num = CustomAttributeExtensions.CompareConstructorFields(first, second);
			if (num != 0)
			{
				return num;
			}
			num = CustomAttributeExtensions.CompareConstructorProperties(first, second);
			if (num != 0)
			{
				return num;
			}
			return 0;
		}
	}
}