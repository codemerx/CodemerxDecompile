using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
    public static class MethodReferenceExtentions
    {
        private static string compilerGeneratedAttributeName = typeof(CompilerGeneratedAttribute).FullName;

        /// <summary>
        /// Selectively resolves the definiatoin from a method reference.
        /// </summary>
        /// <param name="refernece">Target reference</param>
        /// <returns></returns>
        public static MethodDefinition ResolveDefinition(this MethodReference refernece, bool isAssemblyResolverChacheEnabled = true)
        {
			var typeDefination = refernece.DeclaringType as TypeDefinition;

            if (typeDefination == null && refernece.DeclaringType is TypeReference)
            {
                typeDefination = refernece.DeclaringType.Resolve();
            }
            if (typeDefination != null)
            {
                var methodDefination = typeDefination.Methods
                                                .FirstOrDefault(x => x.Name == refernece.Name && FilterParameter(x.Parameters, refernece.Parameters));
                return methodDefination;
            }
            return null;
        }

        private static bool FilterParameter(IList<ParameterDefinition> source, IList<ParameterDefinition> desitnation)
        {
            bool result = true;

            if (source.Count == desitnation.Count)
            {
                for (int index = 0; index < source.Count; index++)
                {
                    if (source[index].ParameterType.Name != desitnation[index].ParameterType.Name)
                    {
                        result &= false;
                        break;
                    }
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        public static bool IsCompilerGenerated(this MethodReference method, bool isAssemblyResolverChacheEnabled = true)
        {
            var methodDef = method as MethodDefinition;
            if (methodDef == null)
            {
                methodDef = method.ResolveDefinition(isAssemblyResolverChacheEnabled);
            }
            if (methodDef == null)
            {
                return false;
            }
            return methodDef.HasCompilerGeneratedAttribute();
        }

        public static bool HasCompilerGeneratedAttribute(this ICustomAttributeProvider attributeProvider)
        {
            if (attributeProvider.CustomAttributes == null)
                return false;

            foreach (var attribute in attributeProvider.CustomAttributes)
            {
                if (attribute.Constructor != null && attribute.AttributeType != null && attribute.AttributeType.FullName == compilerGeneratedAttributeName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Decides if <paramref name="method"/> is an implementation to some of the methods declared in <paramref name="@interface"/>.
        /// </summary>
        /// <param name="method">The target method.</param>
        /// <param name="interface">The interface supposedly containing the target method's declaration.</param>
        /// <returns>Returns true, if the target method implements one of interface's methods.</returns>
        public static bool IsImplementationOf(this MethodReference method, TypeDefinition @interface)
        {
            if (@interface == null)
            {
                throw new ArgumentNullException("@interface can not be null.");
            }
            if (!@interface.IsInterface)
            {
                throw new ArgumentOutOfRangeException("The @interface argument is not an interface definition.");
            }

			if (method.DeclaringType.FullName == @interface.FullName)
			{
				return true;
			}

			if (method.IsExplicitImplementationOf(@interface))
			{
				return true;
			}

			bool hasSameSignatureMethod = false;
			string signature = method.GetMethodSignature();
			foreach (MethodDefinition interfaceMethod in @interface.Methods)
			{
				if (interfaceMethod.GetMethodSignature() == signature)
				{
					hasSameSignatureMethod = true;
				}
			}
			if (!hasSameSignatureMethod)
			{
				return false;
			}
			TypeDefinition declaringType = method.DeclaringType.Resolve();
			if (declaringType == null)
			{
				return false;
			}
			List<TypeDefinition> baseTypes = declaringType.GetBaseTypes();
			foreach (TypeDefinition t in baseTypes)
			{
				if (t.FullName == @interface.FullName)
				{
					return true;
				}
			}
			return false;
        }

		public static bool IsExplicitImplementationOf(this MethodReference method, TypeDefinition @interface)
		{
			if (@interface == null)
			{
				throw new ArgumentNullException("@interface can not be null.");
			}
			if (!@interface.IsInterface)
			{
				throw new ArgumentOutOfRangeException("The @interface argument is not an interface definition.");
			}

			if (method.DeclaringType.FullName == @interface.FullName)
			{
				return true;
			}

			MethodDefinition methodDef = method.Resolve();
			if (methodDef == null)
			{
				return false;
			}
			if (methodDef.HasOverrides)
			{
				foreach (MethodReference overridenMethodRef in methodDef.Overrides)
				{
					MethodDefinition overridenMethodDef = overridenMethodRef.Resolve();
					if (@interface.Methods.Contains(overridenMethodDef))
					{
						return true;
					}
				}
			}

			return false;
		}

		public static bool IsExplicitImplementation(this MethodReference method)
		{
			MethodDefinition methodDef = method.Resolve();
			if (methodDef == null)
			{
				return false;
			}

			return methodDef.IsPrivate && methodDef.HasOverrides;
		}

		private static MethodReference GetImplementedMethodFromInterface(this MethodReference method, TypeDefinition @interface)
		{
			if (@interface == null)
			{
				throw new ArgumentNullException("@interface can not be null.");
			}
			if (!@interface.IsInterface)
			{
				throw new ArgumentOutOfRangeException("The @interface argument is not an interface definition.");
			}

			TypeDefinition declaringType = method.DeclaringType.Resolve();
			if (declaringType == null)
			{
				return null;
			}

			bool isDeclaringTypeImplementingInterface = false;
			List<TypeDefinition> baseTypes = declaringType.GetBaseTypes();
			foreach (TypeDefinition t in baseTypes)
			{
				if (t.FullName == @interface.FullName)
				{
					isDeclaringTypeImplementingInterface = true;
					break;
				}
			}

			if (!isDeclaringTypeImplementingInterface)
			{
				return null;
			}

			if (method.DeclaringType.FullName == @interface.FullName)
			{
				return method;
			}

			MethodReference explictlyImplementedMethod = method.GetExplicitlyImplementedMethodFromInterface(@interface);
			if (explictlyImplementedMethod != null)
			{
				return explictlyImplementedMethod;
			}

			foreach (MethodDefinition interfaceMethod in @interface.Methods)
			{
				if (method.HasSameSignatureWith(interfaceMethod))
				{
					return interfaceMethod;
				}
			}

			return null;
		}

		private static MethodReference GetExplicitlyImplementedMethodFromInterface(this MethodReference method, TypeDefinition @interface)
		{
			if (@interface == null)
			{
				throw new ArgumentNullException("@interface can not be null.");
			}
			if (!@interface.IsInterface)
			{
				throw new ArgumentOutOfRangeException("The @interface argument is not an interface definition.");
			}

			if (method.DeclaringType.FullName == @interface.FullName)
			{
				return method;
			}

			MethodDefinition methodDef = method.Resolve();
			if (methodDef == null)
			{
				return null;
			}

			if (methodDef.HasOverrides)
			{
				foreach (MethodDefinition interfaceMethod in @interface.Methods)
				{
					foreach (MethodReference overridenMethodRef in methodDef.Overrides)
					{
						MethodDefinition overridenMethodDef = overridenMethodRef.Resolve();
						if (overridenMethodDef.FullName == interfaceMethod.FullName)
						{
							return interfaceMethod;
						}
					}
				}
			}

			return null;
		}

		private static MethodDefinition GetImplementedMethodFromGenericInstanceType(this MethodDefinition self, GenericInstanceType type)
		{
			TypeDefinition typeDef = type.Resolve();

			if (typeDef == null)
			{
				return null;
			}

			foreach (MethodDefinition method in typeDef.Methods)
			{
				if (method.Name == self.Name)
				{
					if (method.HasParameters && self.HasParameters && method.Parameters.Count == self.Parameters.Count)
					{
						if (method.ReturnType.IsGenericParameter)
						{
							int parameterPosition = (method.ReturnType as GenericParameter).Position;

							TypeReference genericArgument;
							if (!type.PostionToArgument.TryGetValue(parameterPosition, out genericArgument))
							{
								continue;
							}

							if (genericArgument.FullName != self.ReturnType.FullName)
							{
								continue;
							}
						}
						else if (method.ReturnType.FullName != self.ReturnType.FullName)
						{
							continue;
						}

						for (int i = 0; i < method.Parameters.Count; i++)
						{
							TypeReference parameterType = method.Parameters[i].ParameterType;

							if (parameterType.IsGenericParameter)
							{
								int parameterPosition = (parameterType as GenericParameter).Position;

								TypeReference genericArgument;
								if (!type.PostionToArgument.TryGetValue(parameterPosition, out genericArgument))
								{
									continue;
								}

								if (genericArgument.FullName != self.Parameters[i].ParameterType.FullName)
								{
									continue;
								}
							}
							else if (parameterType.FullName != self.Parameters[i].ParameterType.FullName)
							{
								continue;
							}
						}
					}

					return method;
				}
			}

			return null;
		}

		public static ICollection<MethodDefinition> GetOverridenAndImplementedMethods(this MethodDefinition method)
		{
			HashSet<MethodDefinition> result = new HashSet<MethodDefinition>();

			result.Add(method);

			ICollection<ImplementedMember> implementedMethods = method.GetImplementedMethods();
			foreach (ImplementedMember implementedMember in implementedMethods)
			{
				if (implementedMember.Member is MethodReference)
				{
					MethodDefinition implementedMethod = (implementedMember.Member as MethodReference).Resolve();
					if (implementedMethod != null)
					{
						result.Add(implementedMethod);
					}
				}
			}

			TypeReference currentBaseType = method.DeclaringType.BaseType;
			
			while (currentBaseType != null)
			{
				TypeDefinition currentBaseTypeDefinition = currentBaseType.Resolve();

				if (currentBaseType is GenericInstanceType)
				{
					MethodDefinition implementedMethod = method.GetImplementedMethodFromGenericInstanceType(currentBaseType as GenericInstanceType);
					if (!result.Contains(implementedMethod))
					{
						result.Add(implementedMethod);
					}
				}
				else
				{
					if (currentBaseTypeDefinition != null)
					{
						if (currentBaseTypeDefinition.HasMethods)
						{
							foreach (MethodDefinition baseMethod in currentBaseTypeDefinition.Methods)
							{
								if (method.HasSameSignatureWith(baseMethod))
								{
									if (!result.Contains(baseMethod))
									{
										result.Add(baseMethod);
									}
								}
							}
						}
					}
				}

				currentBaseType = (currentBaseTypeDefinition == null || currentBaseTypeDefinition.BaseType == null) ? null : currentBaseTypeDefinition.BaseType;
			}

			return result.ToList();
		}

		public static ICollection<ImplementedMember> GetImplementedMethods(this MethodDefinition method)
		{
			HashSet<ImplementedMember> result = new HashSet<ImplementedMember>();

			TypeDefinition declaringType = method.DeclaringType;
			foreach (TypeReference interfaceRef in declaringType.Interfaces)
			{
				if (interfaceRef is GenericInstanceType)
				{
					GenericInstanceType genericInstanceInterface = interfaceRef as GenericInstanceType;

					MethodReference implementedMethod = method.GetImplementedMethodFromGenericInstanceType(genericInstanceInterface);

					if (implementedMethod != null)
					{
						ImplementedMember implementedMember = new ImplementedMember(interfaceRef, implementedMethod);

						if (!result.Contains(implementedMember))
						{
							result.Add(implementedMember);
							continue;
						}
					}
				}

				TypeDefinition interfaceDef = interfaceRef.Resolve();
				if (interfaceDef != null)
				{
					MethodReference implementedMethod = method.GetImplementedMethodFromInterface(interfaceDef);

					if (implementedMethod != null)
					{
						ImplementedMember implementedMember = new ImplementedMember(interfaceRef, implementedMethod);

						if (!result.Contains(implementedMember))
						{
							result.Add(implementedMember);
						}
					}
				}
			}

			return result.ToList();
		}

		public static ICollection<ImplementedMember> GetExplicitlyImplementedMethods(this MethodDefinition method)
		{
			HashSet<ImplementedMember> result = new HashSet<ImplementedMember>();

			if (!method.IsExplicitImplementation())
			{
				return result;
			}

			TypeDefinition declaringType = method.DeclaringType;
			foreach (TypeReference interfaceRef in declaringType.Interfaces)
			{
				TypeDefinition interfaceDef = interfaceRef.Resolve();
				if (interfaceDef != null)
				{
					MethodReference explicitlyImplementedMethod = method.GetExplicitlyImplementedMethodFromInterface(interfaceDef);

					if (explicitlyImplementedMethod != null)
					{
						ImplementedMember explicitlyImplementedMember = new ImplementedMember(interfaceRef, explicitlyImplementedMethod);

						if (!result.Contains(explicitlyImplementedMember))
						{
							result.Add(explicitlyImplementedMember);
						}
					}
				}
			}

			return result.ToList();
		}

        private static string GetMethodSignature(this MethodReference method)
        {
            string result = method.FullName;
            int classSeparatorIndex = result.IndexOf("::");
            return result.Substring(classSeparatorIndex);
        }

		public static bool HasSameSignatureWith(this MethodReference self, MethodReference other)
		{
			if (!(self.GetMethodSignature() == other.GetMethodSignature()))
			{
				return false;
			}

			if (self.ReturnType.FullName != other.ReturnType.FullName)
			{
				if (self.ReturnType is GenericParameter && other.ReturnType is GenericParameter)
				{
					if ((self.ReturnType as GenericParameter).Position == (other.ReturnType as GenericParameter).Position)
					{
						return true;
					}
				}

				return false;
			}

			return true;
		}
    }
    
}
