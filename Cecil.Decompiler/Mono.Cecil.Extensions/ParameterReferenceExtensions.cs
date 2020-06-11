using System;

namespace Mono.Cecil.Extensions
{
	public static class ParameterReferenceExtensions
	{
		public static TypeReference ResolveParameterType(this ParameterReference param, MethodReference method)
		{
			TypeReference parameterType = param.ParameterType;
			GenericParameter genericParameter = parameterType as GenericParameter;
			//if (parameterType is GenericInstanceType)
			//{
			//    return ResolveParameterGenericInstanceType(parameterType as GenericInstanceType);
			//}
			bool isByRef = false;
			bool isArray = false;

			if (parameterType.IsByReference)
			{
				genericParameter = parameterType.GetElementType() as GenericParameter;
				isByRef = true;
			}
			if (parameterType.IsArray)
			{
				genericParameter = parameterType.GetElementType() as GenericParameter;
				isArray = true;
			}

			if (genericParameter == null)
			{
				return parameterType;
			}
			int index = genericParameter.Position;
			if (genericParameter.Owner is MethodReference && method.IsGenericInstance)
			{
				GenericInstanceMethod generic = method as GenericInstanceMethod;
				if (index >= 0 && index < generic.GenericArguments.Count)
				{
					parameterType = generic.GenericArguments[index];
					if (generic.PostionToArgument.ContainsKey(index))
					{
						parameterType = generic.PostionToArgument[index];
					}
				}
			}
			else if (genericParameter.Owner is TypeReference && method.DeclaringType.IsGenericInstance)
			{
				GenericInstanceType generic = method.DeclaringType as GenericInstanceType;
				if (index >= 0 && index < generic.GenericArguments.Count)
				{
					parameterType = generic.GenericArguments[index];
					if (generic.PostionToArgument.ContainsKey(index))
					{
						parameterType = generic.PostionToArgument[index];
					}
				}
			}
			if (isByRef)
			{
				return new ByReferenceType(parameterType);
			}
			if (isArray)
			{
				return new ArrayType(parameterType);
			}
			return parameterType;
		}
  
		//private static TypeReference ResolveParameterGenericInstanceType(GenericInstanceType genericInstanceType)
		//{
		//    foreach (GenericParameter param in genericInstanceType.GenericParameters)
		//    {
		//        IGenericParameterProvider owner = param.Owner;
		//        if (owner is GenericInstanceType)
		//        { 
		//            //genericInstanceType
		//        }
		//    }
		//    return genericInstanceType;
		//}
	}
}
