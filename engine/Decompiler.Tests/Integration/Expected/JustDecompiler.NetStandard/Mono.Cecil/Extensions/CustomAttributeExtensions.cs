using Mono.Cecil;
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
			V_0 = Math.Max(first.get_ConstructorArguments().get_Count(), second.get_ConstructorArguments().get_Count());
			V_1 = 0;
			while (V_1 < V_0)
			{
				if (first.get_ConstructorArguments().get_Count() <= V_1)
				{
					return 1;
				}
				if (second.get_ConstructorArguments().get_Count() <= V_1)
				{
					return -1;
				}
				stackVariable25 = first.get_ConstructorArguments().get_Item(V_1);
				V_2 = second.get_ConstructorArguments().get_Item(V_1);
				V_3 = CustomAttributeExtensions.CompareCustomAttributeArguments(stackVariable25, V_2);
				if (V_3 != 0)
				{
					return V_3;
				}
				V_1 = V_1 + 1;
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
			V_0 = Math.Max(first.get_Fields().get_Count(), second.get_Fields().get_Count());
			V_1 = 0;
			while (V_1 < V_0)
			{
				if (first.get_Fields().get_Count() <= V_1)
				{
					return 1;
				}
				if (second.get_Fields().get_Count() <= V_1)
				{
					return -1;
				}
				V_2 = first.get_Fields().get_Item(V_1);
				V_3 = second.get_Fields().get_Item(V_1);
				V_4 = V_2.get_Name().CompareTo(V_3.get_Name());
				if (V_4 != 0)
				{
					return V_4;
				}
				V_5 = CustomAttributeExtensions.CompareCustomAttributeArguments(V_2.get_Argument(), V_3.get_Argument());
				if (V_5 != 0)
				{
					return V_5;
				}
				V_1 = V_1 + 1;
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
			V_0 = Math.Max(first.get_Properties().get_Count(), second.get_Properties().get_Count());
			V_1 = 0;
			while (V_1 < V_0)
			{
				if (first.get_Properties().get_Count() <= V_1)
				{
					return 1;
				}
				if (second.get_Properties().get_Count() <= V_1)
				{
					return -1;
				}
				V_2 = first.get_Properties().get_Item(V_1);
				V_3 = second.get_Properties().get_Item(V_1);
				V_4 = V_2.get_Name().CompareTo(V_3.get_Name());
				if (V_4 != 0)
				{
					return V_4;
				}
				V_5 = CustomAttributeExtensions.CompareCustomAttributeArguments(V_2.get_Argument(), V_3.get_Argument());
				if (V_5 != 0)
				{
					return V_5;
				}
				V_1 = V_1 + 1;
			}
			return 0;
		}

		private static int CompareCustomAttributeArguments(CustomAttributeArgument firstArgument, CustomAttributeArgument secondArgument)
		{
			return firstArgument.get_Value().ToString().CompareTo(secondArgument.get_Value().ToString());
		}

		public static int CompareToCustomAttribute(this CustomAttribute first, CustomAttribute second, bool fullNameCheck = false)
		{
			if (String.op_Inequality(first.get_AttributeType().get_Name(), second.get_AttributeType().get_Name()))
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
			V_0 = CustomAttributeExtensions.CompareConstructorArguments(first, second);
			if (V_0 != 0)
			{
				return V_0;
			}
			V_0 = CustomAttributeExtensions.CompareConstructorFields(first, second);
			if (V_0 != 0)
			{
				return V_0;
			}
			V_0 = CustomAttributeExtensions.CompareConstructorProperties(first, second);
			if (V_0 != 0)
			{
				return V_0;
			}
			return 0;
		}
	}
}