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
			if (first == second)
			{
				return 0;
			}
			string str = firstDeclaration.Action.ToString();
			string str1 = secondDeclaration.Action.ToString();
			if (str != str1)
			{
				return str.CompareTo(str1);
			}
			int num = Math.Max(first.Properties.Count, second.Properties.Count);
			for (int i = 0; i < num; i++)
			{
				if (i >= first.Properties.Count)
				{
					return 1;
				}
				if (i >= second.Properties.Count)
				{
					return -1;
				}
				CustomAttributeNamedArgument item = first.Properties[i];
				CustomAttributeNamedArgument customAttributeNamedArgument = second.Properties[i];
				if (item.Name != customAttributeNamedArgument.Name)
				{
					return item.Name.CompareTo(customAttributeNamedArgument.Name);
				}
				argument = item.Argument;
				string str2 = argument.Value.ToString();
				argument = customAttributeNamedArgument.Argument;
				string str3 = argument.Value.ToString();
				if (str2 != str3)
				{
					return str2.CompareTo(str3);
				}
			}
			int num1 = Math.Max(first.Fields.Count, second.Fields.Count);
			for (int j = 0; j < num1; j++)
			{
				if (j >= first.Fields.Count)
				{
					return 1;
				}
				if (j >= second.Fields.Count)
				{
					return -1;
				}
				CustomAttributeNamedArgument item1 = first.Fields[j];
				CustomAttributeNamedArgument customAttributeNamedArgument1 = second.Fields[j];
				if (item1.Name != customAttributeNamedArgument1.Name)
				{
					return item1.Name.CompareTo(customAttributeNamedArgument1.Name);
				}
				argument = item1.Argument;
				string str4 = argument.Value.ToString();
				argument = customAttributeNamedArgument1.Argument;
				string str5 = argument.Value.ToString();
				if (str4 != str5)
				{
					return str4.CompareTo(str5);
				}
			}
			return 0;
		}
	}
}