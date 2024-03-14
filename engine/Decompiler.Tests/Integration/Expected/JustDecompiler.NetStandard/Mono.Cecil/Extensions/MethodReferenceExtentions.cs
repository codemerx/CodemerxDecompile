using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class MethodReferenceExtentions
	{
		private static string compilerGeneratedAttributeName;

		static MethodReferenceExtentions()
		{
			MethodReferenceExtentions.compilerGeneratedAttributeName = typeof(CompilerGeneratedAttribute).FullName;
		}

		private static bool FilterParameter(IList<ParameterDefinition> source, IList<ParameterDefinition> desitnation)
		{
			bool flag = true;
			if (source.Count != desitnation.Count)
			{
				flag = false;
			}
			else
			{
				int num = 0;
				while (num < source.Count)
				{
					if (source[num].get_ParameterType().get_Name() == desitnation[num].get_ParameterType().get_Name())
					{
						num++;
					}
					else
					{
						flag = false;
						return flag;
					}
				}
			}
			return flag;
		}

		private static MethodReference GetExplicitlyImplementedMethodFromInterface(this MethodReference method, TypeDefinition @interface)
		{
			MethodReference methodReference;
			if (@interface == null)
			{
				throw new ArgumentNullException("@interface can not be null.");
			}
			if (!@interface.get_IsInterface())
			{
				throw new ArgumentOutOfRangeException("The @interface argument is not an interface definition.");
			}
			if (method.get_DeclaringType().get_FullName() == @interface.get_FullName())
			{
				return method;
			}
			MethodDefinition methodDefinition = method.Resolve();
			if (methodDefinition == null)
			{
				return null;
			}
			if (methodDefinition.get_HasOverrides())
			{
				Collection<MethodDefinition>.Enumerator enumerator = @interface.get_Methods().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						MethodDefinition current = enumerator.get_Current();
						Collection<MethodReference>.Enumerator enumerator1 = methodDefinition.get_Overrides().GetEnumerator();
						try
						{
							while (enumerator1.MoveNext())
							{
								if (enumerator1.get_Current().Resolve().get_FullName() != current.get_FullName())
								{
									continue;
								}
								methodReference = current;
								return methodReference;
							}
						}
						finally
						{
							enumerator1.Dispose();
						}
					}
					return null;
				}
				finally
				{
					enumerator.Dispose();
				}
				return methodReference;
			}
			return null;
		}

		public static ICollection<ImplementedMember> GetExplicitlyImplementedMethods(this MethodDefinition method)
		{
			HashSet<ImplementedMember> implementedMembers = new HashSet<ImplementedMember>();
			if (!method.IsExplicitImplementation())
			{
				return implementedMembers;
			}
			foreach (TypeReference @interface in method.get_DeclaringType().get_Interfaces())
			{
				TypeDefinition typeDefinition = @interface.Resolve();
				if (typeDefinition == null)
				{
					continue;
				}
				MethodReference explicitlyImplementedMethodFromInterface = method.GetExplicitlyImplementedMethodFromInterface(typeDefinition);
				if (explicitlyImplementedMethodFromInterface == null)
				{
					continue;
				}
				ImplementedMember implementedMember = new ImplementedMember(@interface, explicitlyImplementedMethodFromInterface);
				if (implementedMembers.Contains(implementedMember))
				{
					continue;
				}
				implementedMembers.Add(implementedMember);
			}
			return implementedMembers.ToList<ImplementedMember>();
		}

		private static MethodDefinition GetImplementedMethodFromGenericInstanceType(this MethodDefinition self, GenericInstanceType type)
		{
			MethodDefinition current;
			TypeReference typeReference;
			TypeReference typeReference1;
			MethodDefinition methodDefinition;
			TypeDefinition typeDefinition = type.Resolve();
			if (typeDefinition == null)
			{
				return null;
			}
			Collection<MethodDefinition>.Enumerator enumerator = typeDefinition.get_Methods().GetEnumerator();
			try
			{
				do
				{
				Label2:
					if (enumerator.MoveNext())
					{
						current = enumerator.get_Current();
						if (current.get_Name() == self.get_Name())
						{
							if (!current.get_HasParameters() || !self.get_HasParameters() || current.get_Parameters().get_Count() != self.get_Parameters().get_Count())
							{
								goto Label1;
							}
							if (!current.get_ReturnType().get_IsGenericParameter())
							{
								continue;
							}
							int position = (current.get_ReturnType() as GenericParameter).get_Position();
							if (type.get_PostionToArgument().TryGetValue(position, out typeReference))
							{
								if (typeReference.get_FullName() == self.get_ReturnType().get_FullName())
								{
									break;
								}
								goto Label2;
							}
							else
							{
								goto Label2;
							}
						}
						else
						{
							goto Label2;
						}
					}
					else
					{
						return null;
					}
				}
				while (current.get_ReturnType().get_FullName() != self.get_ReturnType().get_FullName());
				for (int i = 0; i < current.get_Parameters().get_Count(); i++)
				{
					TypeReference parameterType = current.get_Parameters().get_Item(i).get_ParameterType();
					if (!parameterType.get_IsGenericParameter())
					{
						bool fullName = parameterType.get_FullName() != self.get_Parameters().get_Item(i).get_ParameterType().get_FullName();
					}
					else
					{
						int num = (parameterType as GenericParameter).get_Position();
						if (type.get_PostionToArgument().TryGetValue(num, out typeReference1) && typeReference1.get_FullName() != self.get_Parameters().get_Item(i).get_ParameterType().get_FullName())
						{
						}
					}
				}
			Label1:
				methodDefinition = current;
			}
			finally
			{
				enumerator.Dispose();
			}
			return methodDefinition;
		}

		private static MethodReference GetImplementedMethodFromInterface(this MethodReference method, TypeDefinition @interface)
		{
			MethodReference methodReference;
			if (@interface == null)
			{
				throw new ArgumentNullException("@interface can not be null.");
			}
			if (!@interface.get_IsInterface())
			{
				throw new ArgumentOutOfRangeException("The @interface argument is not an interface definition.");
			}
			TypeDefinition typeDefinition = method.get_DeclaringType().Resolve();
			if (typeDefinition == null)
			{
				return null;
			}
			bool flag = false;
			foreach (TypeDefinition baseType in typeDefinition.GetBaseTypes())
			{
				if (baseType.get_FullName() != @interface.get_FullName())
				{
					continue;
				}
				flag = true;
				goto Label0;
			}
		Label0:
			if (!flag)
			{
				return null;
			}
			if (method.get_DeclaringType().get_FullName() == @interface.get_FullName())
			{
				return method;
			}
			MethodReference explicitlyImplementedMethodFromInterface = method.GetExplicitlyImplementedMethodFromInterface(@interface);
			if (explicitlyImplementedMethodFromInterface != null)
			{
				return explicitlyImplementedMethodFromInterface;
			}
			Collection<MethodDefinition>.Enumerator enumerator = @interface.get_Methods().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					MethodDefinition current = enumerator.get_Current();
					if (!method.HasSameSignatureWith(current))
					{
						continue;
					}
					methodReference = current;
					return methodReference;
				}
				return null;
			}
			finally
			{
				enumerator.Dispose();
			}
			return methodReference;
		}

		public static ICollection<ImplementedMember> GetImplementedMethods(this MethodDefinition method)
		{
			HashSet<ImplementedMember> implementedMembers = new HashSet<ImplementedMember>();
			foreach (TypeReference @interface in method.get_DeclaringType().get_Interfaces())
			{
				if (@interface is GenericInstanceType)
				{
					MethodReference implementedMethodFromGenericInstanceType = method.GetImplementedMethodFromGenericInstanceType(@interface as GenericInstanceType);
					if (implementedMethodFromGenericInstanceType != null)
					{
						ImplementedMember implementedMember = new ImplementedMember(@interface, implementedMethodFromGenericInstanceType);
						if (!implementedMembers.Contains(implementedMember))
						{
							implementedMembers.Add(implementedMember);
							continue;
						}
					}
				}
				TypeDefinition typeDefinition = @interface.Resolve();
				if (typeDefinition == null)
				{
					continue;
				}
				MethodReference implementedMethodFromInterface = method.GetImplementedMethodFromInterface(typeDefinition);
				if (implementedMethodFromInterface == null)
				{
					continue;
				}
				ImplementedMember implementedMember1 = new ImplementedMember(@interface, implementedMethodFromInterface);
				if (implementedMembers.Contains(implementedMember1))
				{
					continue;
				}
				implementedMembers.Add(implementedMember1);
			}
			return implementedMembers.ToList<ImplementedMember>();
		}

		private static string GetMethodSignature(this MethodReference method)
		{
			string fullName = method.get_FullName();
			return fullName.Substring(fullName.IndexOf("::"));
		}

		public static ICollection<MethodDefinition> GetOverridenAndImplementedMethods(this MethodDefinition method)
		{
			TypeReference baseType = null;
			HashSet<MethodDefinition> methodDefinitions = new HashSet<MethodDefinition>();
			methodDefinitions.Add(method);
			foreach (ImplementedMember implementedMethod in method.GetImplementedMethods())
			{
				if (!(implementedMethod.Member is MethodReference))
				{
					continue;
				}
				MethodDefinition methodDefinition = (implementedMethod.Member as MethodReference).Resolve();
				if (methodDefinition == null)
				{
					continue;
				}
				methodDefinitions.Add(methodDefinition);
			}
			for (TypeReference i = method.get_DeclaringType().get_BaseType(); i != null; i = baseType)
			{
				TypeDefinition typeDefinition = i.Resolve();
				if (i is GenericInstanceType)
				{
					MethodDefinition implementedMethodFromGenericInstanceType = method.GetImplementedMethodFromGenericInstanceType(i as GenericInstanceType);
					if (!methodDefinitions.Contains(implementedMethodFromGenericInstanceType))
					{
						methodDefinitions.Add(implementedMethodFromGenericInstanceType);
					}
				}
				else if (typeDefinition != null && typeDefinition.get_HasMethods())
				{
					foreach (MethodDefinition methodDefinition1 in typeDefinition.get_Methods())
					{
						if (!method.HasSameSignatureWith(methodDefinition1) || methodDefinitions.Contains(methodDefinition1))
						{
							continue;
						}
						methodDefinitions.Add(methodDefinition1);
					}
				}
				if (typeDefinition == null || typeDefinition.get_BaseType() == null)
				{
					baseType = null;
				}
				else
				{
					baseType = typeDefinition.get_BaseType();
				}
			}
			return methodDefinitions.ToList<MethodDefinition>();
		}

		public static bool HasCompilerGeneratedAttribute(this ICustomAttributeProvider attributeProvider)
		{
			bool flag;
			if (attributeProvider.get_CustomAttributes() == null)
			{
				return false;
			}
			Collection<CustomAttribute>.Enumerator enumerator = attributeProvider.get_CustomAttributes().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					CustomAttribute current = enumerator.get_Current();
					if (current.get_Constructor() == null || current.get_AttributeType() == null || !(current.get_AttributeType().get_FullName() == MethodReferenceExtentions.compilerGeneratedAttributeName))
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				enumerator.Dispose();
			}
			return flag;
		}

		public static bool HasSameSignatureWith(this MethodReference self, MethodReference other)
		{
			if (self.GetMethodSignature() != other.GetMethodSignature())
			{
				return false;
			}
			if (self.get_ReturnType().get_FullName() == other.get_ReturnType().get_FullName())
			{
				return true;
			}
			if (self.get_ReturnType() is GenericParameter && other.get_ReturnType() is GenericParameter && (self.get_ReturnType() as GenericParameter).get_Position() == (other.get_ReturnType() as GenericParameter).get_Position())
			{
				return true;
			}
			return false;
		}

		public static bool IsCompilerGenerated(this MethodReference method, bool isAssemblyResolverChacheEnabled = true)
		{
			MethodDefinition methodDefinition = method as MethodDefinition ?? method.ResolveDefinition(isAssemblyResolverChacheEnabled);
			if (methodDefinition == null)
			{
				return false;
			}
			return methodDefinition.HasCompilerGeneratedAttribute();
		}

		public static bool IsExplicitImplementation(this MethodReference method)
		{
			MethodDefinition methodDefinition = method.Resolve();
			if (methodDefinition == null)
			{
				return false;
			}
			if (!methodDefinition.get_IsPrivate())
			{
				return false;
			}
			return methodDefinition.get_HasOverrides();
		}

		public static bool IsExplicitImplementationOf(this MethodReference method, TypeDefinition @interface)
		{
			bool flag;
			if (@interface == null)
			{
				throw new ArgumentNullException("@interface can not be null.");
			}
			if (!@interface.get_IsInterface())
			{
				throw new ArgumentOutOfRangeException("The @interface argument is not an interface definition.");
			}
			if (method.get_DeclaringType().get_FullName() == @interface.get_FullName())
			{
				return true;
			}
			MethodDefinition methodDefinition = method.Resolve();
			if (methodDefinition == null)
			{
				return false;
			}
			if (methodDefinition.get_HasOverrides())
			{
				Collection<MethodReference>.Enumerator enumerator = methodDefinition.get_Overrides().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						MethodDefinition methodDefinition1 = enumerator.get_Current().Resolve();
						if (!@interface.get_Methods().Contains(methodDefinition1))
						{
							continue;
						}
						flag = true;
						return flag;
					}
					return false;
				}
				finally
				{
					enumerator.Dispose();
				}
				return flag;
			}
			return false;
		}

		public static bool IsImplementationOf(this MethodReference method, TypeDefinition @interface)
		{
			bool flag;
			if (@interface == null)
			{
				throw new ArgumentNullException("@interface can not be null.");
			}
			if (!@interface.get_IsInterface())
			{
				throw new ArgumentOutOfRangeException("The @interface argument is not an interface definition.");
			}
			if (method.get_DeclaringType().get_FullName() == @interface.get_FullName())
			{
				return true;
			}
			if (method.IsExplicitImplementationOf(@interface))
			{
				return true;
			}
			bool flag1 = false;
			string methodSignature = method.GetMethodSignature();
			foreach (MethodDefinition methodDefinition in @interface.get_Methods())
			{
				if (methodDefinition.GetMethodSignature() != methodSignature)
				{
					continue;
				}
				flag1 = true;
			}
			if (!flag1)
			{
				return false;
			}
			TypeDefinition typeDefinition = method.get_DeclaringType().Resolve();
			if (typeDefinition == null)
			{
				return false;
			}
			List<TypeDefinition>.Enumerator enumerator = typeDefinition.GetBaseTypes().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.get_FullName() != @interface.get_FullName())
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		public static MethodDefinition ResolveDefinition(this MethodReference refernece, bool isAssemblyResolverChacheEnabled = true)
		{
			TypeDefinition declaringType = refernece.get_DeclaringType() as TypeDefinition;
			if (declaringType == null && refernece.get_DeclaringType() != null)
			{
				declaringType = refernece.get_DeclaringType().Resolve();
			}
			if (declaringType == null)
			{
				return null;
			}
			return declaringType.get_Methods().FirstOrDefault<MethodDefinition>((MethodDefinition x) => {
				if (x.get_Name() != refernece.get_Name())
				{
					return false;
				}
				return MethodReferenceExtentions.FilterParameter(x.get_Parameters(), refernece.get_Parameters());
			});
		}
	}
}