using Mono.Cecil;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class SecurityAttributeExtensions
	{
		public static int CompareToSecurityAttribute(this SecurityAttribute first, SecurityAttribute second, SecurityDeclaration firstDeclaration, SecurityDeclaration secondDeclaration)
		{
			if ((object)first == (object)second)
			{
				return 0;
			}
			V_0 = firstDeclaration.get_Action().ToString();
			V_1 = secondDeclaration.get_Action().ToString();
			if (String.op_Inequality(V_0, V_1))
			{
				return V_0.CompareTo(V_1);
			}
			V_2 = Math.Max(first.get_Properties().get_Count(), second.get_Properties().get_Count());
			V_5 = 0;
			while (V_5 < V_2)
			{
				if (V_5 >= first.get_Properties().get_Count())
				{
					return 1;
				}
				if (V_5 >= second.get_Properties().get_Count())
				{
					return -1;
				}
				V_6 = first.get_Properties().get_Item(V_5);
				V_7 = second.get_Properties().get_Item(V_5);
				if (!String.op_Equality(V_6.get_Name(), V_7.get_Name()))
				{
					return V_6.get_Name().CompareTo(V_7.get_Name());
				}
				V_10 = V_6.get_Argument();
				V_8 = V_10.get_Value().ToString();
				V_10 = V_7.get_Argument();
				V_9 = V_10.get_Value().ToString();
				if (String.op_Inequality(V_8, V_9))
				{
					return V_8.CompareTo(V_9);
				}
				V_5 = V_5 + 1;
			}
			V_3 = Math.Max(first.get_Fields().get_Count(), second.get_Fields().get_Count());
			V_11 = 0;
			while (V_11 < V_3)
			{
				if (V_11 >= first.get_Fields().get_Count())
				{
					return 1;
				}
				if (V_11 >= second.get_Fields().get_Count())
				{
					return -1;
				}
				V_12 = first.get_Fields().get_Item(V_11);
				V_13 = second.get_Fields().get_Item(V_11);
				if (!String.op_Equality(V_12.get_Name(), V_13.get_Name()))
				{
					return V_12.get_Name().CompareTo(V_13.get_Name());
				}
				V_10 = V_12.get_Argument();
				V_14 = V_10.get_Value().ToString();
				V_10 = V_13.get_Argument();
				V_15 = V_10.get_Value().ToString();
				if (String.op_Inequality(V_14, V_15))
				{
					return V_14.CompareTo(V_15);
				}
				V_11 = V_11 + 1;
			}
			return 0;
		}
	}
}