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
			if (first.HasConstructorArguments && !second.HasConstructorArguments)
			{
				return 1;
			}
			if (!first.HasConstructorArguments && second.HasConstructorArguments)
			{
				return -1;
			}
			int num = Math.Max(first.ConstructorArguments.Count, second.ConstructorArguments.Count);
			for (int i = 0; i < num; i++)
			{
				if (first.ConstructorArguments.Count <= i)
				{
					return 1;
				}
				if (second.ConstructorArguments.Count <= i)
				{
					return -1;
				}
				CustomAttributeArgument item = first.ConstructorArguments[i];
				CustomAttributeArgument customAttributeArgument = second.ConstructorArguments[i];
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
			if (first.HasFields && !second.HasFields)
			{
				return 1;
			}
			if (!first.HasFields && second.HasFields)
			{
				return -1;
			}
			int num = Math.Max(first.Fields.Count, second.Fields.Count);
			for (int i = 0; i < num; i++)
			{
				if (first.Fields.Count <= i)
				{
					return 1;
				}
				if (second.Fields.Count <= i)
				{
					return -1;
				}
				CustomAttributeNamedArgument item = first.Fields[i];
				CustomAttributeNamedArgument customAttributeNamedArgument = second.Fields[i];
				int num1 = item.Name.CompareTo(customAttributeNamedArgument.Name);
				if (num1 != 0)
				{
					return num1;
				}
				int num2 = CustomAttributeExtensions.CompareCustomAttributeArguments(item.Argument, customAttributeNamedArgument.Argument);
				if (num2 != 0)
				{
					return num2;
				}
			}
			return 0;
		}

		private static int CompareConstructorProperties(CustomAttribute first, CustomAttribute second)
		{
			if (first.HasProperties && !second.HasProperties)
			{
				return 1;
			}
			if (!first.HasProperties && second.HasProperties)
			{
				return -1;
			}
			int num = Math.Max(first.Properties.Count, second.Properties.Count);
			for (int i = 0; i < num; i++)
			{
				if (first.Properties.Count <= i)
				{
					return 1;
				}
				if (second.Properties.Count <= i)
				{
					return -1;
				}
				CustomAttributeNamedArgument item = first.Properties[i];
				CustomAttributeNamedArgument customAttributeNamedArgument = second.Properties[i];
				int num1 = item.Name.CompareTo(customAttributeNamedArgument.Name);
				if (num1 != 0)
				{
					return num1;
				}
				int num2 = CustomAttributeExtensions.CompareCustomAttributeArguments(item.Argument, customAttributeNamedArgument.Argument);
				if (num2 != 0)
				{
					return num2;
				}
			}
			return 0;
		}

		private static int CompareCustomAttributeArguments(CustomAttributeArgument firstArgument, CustomAttributeArgument secondArgument)
		{
			return firstArgument.Value.ToString().CompareTo(secondArgument.Value.ToString());
		}

		public static int CompareToCustomAttribute(this CustomAttribute first, CustomAttribute second, bool fullNameCheck = false)
		{
			if (first.AttributeType.Name != second.AttributeType.Name)
			{
				if (fullNameCheck)
				{
					return first.AttributeType.FullName.CompareTo(second.AttributeType.FullName);
				}
				return first.AttributeType.Name.CompareTo(second.AttributeType.Name);
			}
			if (first == second)
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