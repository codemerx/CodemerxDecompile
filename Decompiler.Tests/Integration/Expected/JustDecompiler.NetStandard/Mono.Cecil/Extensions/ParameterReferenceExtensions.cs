using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class ParameterReferenceExtensions
	{
		public static TypeReference ResolveParameterType(this ParameterReference param, MethodReference method)
		{
			TypeReference parameterType = param.get_ParameterType();
			GenericParameter elementType = parameterType as GenericParameter;
			bool flag = false;
			bool flag1 = false;
			if (parameterType.get_IsByReference())
			{
				elementType = parameterType.GetElementType() as GenericParameter;
				flag = true;
			}
			if (parameterType.get_IsArray())
			{
				elementType = parameterType.GetElementType() as GenericParameter;
				flag1 = true;
			}
			if (elementType == null)
			{
				return parameterType;
			}
			int position = elementType.get_Position();
			if (elementType.get_Owner() is MethodReference && method.get_IsGenericInstance())
			{
				GenericInstanceMethod genericInstanceMethod = method as GenericInstanceMethod;
				if (position >= 0 && position < genericInstanceMethod.get_GenericArguments().get_Count())
				{
					parameterType = genericInstanceMethod.get_GenericArguments().get_Item(position);
					if (genericInstanceMethod.get_PostionToArgument().ContainsKey(position))
					{
						parameterType = genericInstanceMethod.get_PostionToArgument()[position];
					}
				}
			}
			else if (elementType.get_Owner() is TypeReference && method.get_DeclaringType().get_IsGenericInstance())
			{
				GenericInstanceType declaringType = method.get_DeclaringType() as GenericInstanceType;
				if (position >= 0 && position < declaringType.get_GenericArguments().get_Count())
				{
					parameterType = declaringType.get_GenericArguments().get_Item(position);
					if (declaringType.get_PostionToArgument().ContainsKey(position))
					{
						parameterType = declaringType.get_PostionToArgument()[position];
					}
				}
			}
			if (flag)
			{
				return new ByReferenceType(parameterType);
			}
			if (!flag1)
			{
				return parameterType;
			}
			return new ArrayType(parameterType);
		}
	}
}