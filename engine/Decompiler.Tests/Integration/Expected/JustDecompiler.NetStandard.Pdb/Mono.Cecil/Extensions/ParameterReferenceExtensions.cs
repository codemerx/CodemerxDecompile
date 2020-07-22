using Mono.Cecil;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class ParameterReferenceExtensions
	{
		public static TypeReference ResolveParameterType(this ParameterReference param, MethodReference method)
		{
			V_0 = param.get_ParameterType();
			V_1 = V_0 as GenericParameter;
			V_2 = false;
			V_3 = false;
			if (V_0.get_IsByReference())
			{
				V_1 = V_0.GetElementType() as GenericParameter;
				V_2 = true;
			}
			if (V_0.get_IsArray())
			{
				V_1 = V_0.GetElementType() as GenericParameter;
				V_3 = true;
			}
			if (V_1 == null)
			{
				return V_0;
			}
			V_4 = V_1.get_Position();
			if (V_1.get_Owner() as MethodReference == null || !method.get_IsGenericInstance())
			{
				if (V_1.get_Owner() as TypeReference != null && method.get_DeclaringType().get_IsGenericInstance())
				{
					V_6 = method.get_DeclaringType() as GenericInstanceType;
					if (V_4 >= 0 && V_4 < V_6.get_GenericArguments().get_Count())
					{
						V_0 = V_6.get_GenericArguments().get_Item(V_4);
						if (V_6.get_PostionToArgument().ContainsKey(V_4))
						{
							V_0 = V_6.get_PostionToArgument().get_Item(V_4);
						}
					}
				}
			}
			else
			{
				V_5 = method as GenericInstanceMethod;
				if (V_4 >= 0 && V_4 < V_5.get_GenericArguments().get_Count())
				{
					V_0 = V_5.get_GenericArguments().get_Item(V_4);
					if (V_5.get_PostionToArgument().ContainsKey(V_4))
					{
						V_0 = V_5.get_PostionToArgument().get_Item(V_4);
					}
				}
			}
			if (V_2)
			{
				return new ByReferenceType(V_0);
			}
			if (!V_3)
			{
				return V_0;
			}
			return new ArrayType(V_0);
		}
	}
}