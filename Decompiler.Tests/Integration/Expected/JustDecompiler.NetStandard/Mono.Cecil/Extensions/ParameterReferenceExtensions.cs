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
			TypeReference parameterType = param.ParameterType;
			GenericParameter elementType = parameterType as GenericParameter;
			bool flag = false;
			bool flag1 = false;
			if (parameterType.IsByReference)
			{
				elementType = parameterType.GetElementType() as GenericParameter;
				flag = true;
			}
			if (parameterType.IsArray)
			{
				elementType = parameterType.GetElementType() as GenericParameter;
				flag1 = true;
			}
			if (elementType == null)
			{
				return parameterType;
			}
			int position = elementType.Position;
			if (elementType.Owner is MethodReference && method.IsGenericInstance)
			{
				GenericInstanceMethod genericInstanceMethod = method as GenericInstanceMethod;
				if (position >= 0 && position < genericInstanceMethod.GenericArguments.Count)
				{
					parameterType = genericInstanceMethod.GenericArguments[position];
					if (genericInstanceMethod.PostionToArgument.ContainsKey(position))
					{
						parameterType = genericInstanceMethod.PostionToArgument[position];
					}
				}
			}
			else if (elementType.Owner is TypeReference && method.DeclaringType.IsGenericInstance)
			{
				GenericInstanceType declaringType = method.DeclaringType as GenericInstanceType;
				if (position >= 0 && position < declaringType.GenericArguments.Count)
				{
					parameterType = declaringType.GenericArguments[position];
					if (declaringType.PostionToArgument.ContainsKey(position))
					{
						parameterType = declaringType.PostionToArgument[position];
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