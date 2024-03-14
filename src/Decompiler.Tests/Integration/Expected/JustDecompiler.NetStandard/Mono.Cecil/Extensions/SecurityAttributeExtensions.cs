using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class SecurityAttributeExtensions
	{
		public static int CompareToSecurityAttribute(this SecurityAttribute first, SecurityAttribute second, SecurityDeclaration firstDeclaration, SecurityDeclaration secondDeclaration)
		{
			CustomAttributeArgument argument;
			if ((object)first == (object)second)
			{
				return 0;
			}
			string str = firstDeclaration.get_Action().ToString();
			string str1 = secondDeclaration.get_Action().ToString();
			if (str != str1)
			{
				return str.CompareTo(str1);
			}
			int num = Math.Max(first.get_Properties().get_Count(), second.get_Properties().get_Count());
			for (int i = 0; i < num; i++)
			{
				if (i >= first.get_Properties().get_Count())
				{
					return 1;
				}
				if (i >= second.get_Properties().get_Count())
				{
					return -1;
				}
				CustomAttributeNamedArgument item = first.get_Properties().get_Item(i);
				CustomAttributeNamedArgument customAttributeNamedArgument = second.get_Properties().get_Item(i);
				if (item.get_Name() != customAttributeNamedArgument.get_Name())
				{
					return item.get_Name().CompareTo(customAttributeNamedArgument.get_Name());
				}
				argument = item.get_Argument();
				string str2 = argument.get_Value().ToString();
				argument = customAttributeNamedArgument.get_Argument();
				string str3 = argument.get_Value().ToString();
				if (str2 != str3)
				{
					return str2.CompareTo(str3);
				}
			}
			int num1 = Math.Max(first.get_Fields().get_Count(), second.get_Fields().get_Count());
			for (int j = 0; j < num1; j++)
			{
				if (j >= first.get_Fields().get_Count())
				{
					return 1;
				}
				if (j >= second.get_Fields().get_Count())
				{
					return -1;
				}
				CustomAttributeNamedArgument item1 = first.get_Fields().get_Item(j);
				CustomAttributeNamedArgument customAttributeNamedArgument1 = second.get_Fields().get_Item(j);
				if (item1.get_Name() != customAttributeNamedArgument1.get_Name())
				{
					return item1.get_Name().CompareTo(customAttributeNamedArgument1.get_Name());
				}
				argument = item1.get_Argument();
				string str4 = argument.get_Value().ToString();
				argument = customAttributeNamedArgument1.get_Argument();
				string str5 = argument.get_Value().ToString();
				if (str4 != str5)
				{
					return str4.CompareTo(str5);
				}
			}
			return 0;
		}
	}
}