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
					if (source[num].ParameterType.Name == desitnation[num].ParameterType.Name)
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
			if (!@interface.IsInterface)
			{
				throw new ArgumentOutOfRangeException("The @interface argument is not an interface definition.");
			}
			if (method.DeclaringType.FullName == @interface.FullName)
			{
				return method;
			}
			MethodDefinition methodDefinition = method.Resolve();
			if (methodDefinition == null)
			{
				return null;
			}
			if (methodDefinition.HasOverrides)
			{
				Collection<MethodDefinition>.Enumerator enumerator = @interface.Methods.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						MethodDefinition current = enumerator.Current;
						Collection<MethodReference>.Enumerator enumerator1 = methodDefinition.Overrides.GetEnumerator();
						try
						{
							while (enumerator1.MoveNext())
							{
								if (enumerator1.Current.Resolve().FullName != current.FullName)
								{
									continue;
								}
								methodReference = current;
								return methodReference;
							}
						}
						finally
						{
							((IDisposable)enumerator1).Dispose();
						}
					}
					return null;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
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
			foreach (TypeReference @interface in method.DeclaringType.Interfaces)
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
			Collection<MethodDefinition>.Enumerator enumerator = typeDefinition.Methods.GetEnumerator();
			try
			{
				do
				{
				Label2:
					if (enumerator.MoveNext())
					{
						current = enumerator.Current;
						if (current.Name == self.Name)
						{
							if (!current.HasParameters || !self.HasParameters || current.Parameters.Count != self.Parameters.Count)
							{
								goto Label1;
							}
							if (!current.ReturnType.IsGenericParameter)
							{
								continue;
							}
							int position = (current.ReturnType as GenericParameter).Position;
							if (type.PostionToArgument.TryGetValue(position, out typeReference))
							{
								if (typeReference.FullName == self.ReturnType.FullName)
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
				while (current.ReturnType.FullName != self.ReturnType.FullName);
				for (int i = 0; i < current.Parameters.Count; i++)
				{
					TypeReference parameterType = current.Parameters[i].ParameterType;
					if (!parameterType.IsGenericParameter)
					{
						bool fullName = parameterType.FullName != self.Parameters[i].ParameterType.FullName;
					}
					else
					{
						int num = (parameterType as GenericParameter).Position;
						if (type.PostionToArgument.TryGetValue(num, out typeReference1) && typeReference1.FullName != self.Parameters[i].ParameterType.FullName)
						{
						}
					}
				}
			Label1:
				methodDefinition = current;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
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
			if (!@interface.IsInterface)
			{
				throw new ArgumentOutOfRangeException("The @interface argument is not an interface definition.");
			}
			TypeDefinition typeDefinition = method.DeclaringType.Resolve();
			if (typeDefinition == null)
			{
				return null;
			}
			bool flag = false;
			foreach (TypeDefinition baseType in typeDefinition.GetBaseTypes())
			{
				if (baseType.FullName != @interface.FullName)
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
			if (method.DeclaringType.FullName == @interface.FullName)
			{
				return method;
			}
			MethodReference explicitlyImplementedMethodFromInterface = method.GetExplicitlyImplementedMethodFromInterface(@interface);
			if (explicitlyImplementedMethodFromInterface != null)
			{
				return explicitlyImplementedMethodFromInterface;
			}
			Collection<MethodDefinition>.Enumerator enumerator = @interface.Methods.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					MethodDefinition current = enumerator.Current;
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
				((IDisposable)enumerator).Dispose();
			}
			return methodReference;
		}

		public static ICollection<ImplementedMember> GetImplementedMethods(this MethodDefinition method)
		{
			HashSet<ImplementedMember> implementedMembers = new HashSet<ImplementedMember>();
			foreach (TypeReference @interface in method.DeclaringType.Interfaces)
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
			string fullName = method.FullName;
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
			for (TypeReference i = method.DeclaringType.BaseType; i != null; i = baseType)
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
				else if (typeDefinition != null && typeDefinition.HasMethods)
				{
					foreach (MethodDefinition methodDefinition1 in typeDefinition.Methods)
					{
						if (!method.HasSameSignatureWith(methodDefinition1) || methodDefinitions.Contains(methodDefinition1))
						{
							continue;
						}
						methodDefinitions.Add(methodDefinition1);
					}
				}
				if (typeDefinition == null || typeDefinition.BaseType == null)
				{
					baseType = null;
				}
				else
				{
					baseType = typeDefinition.BaseType;
				}
			}
			return methodDefinitions.ToList<MethodDefinition>();
		}

		public static bool HasCompilerGeneratedAttribute(this ICustomAttributeProvider attributeProvider)
		{
			bool flag;
			if (attributeProvider.CustomAttributes == null)
			{
				return false;
			}
			Collection<CustomAttribute>.Enumerator enumerator = attributeProvider.CustomAttributes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					CustomAttribute current = enumerator.Current;
					if (current.Constructor == null || current.AttributeType == null || !(current.AttributeType.FullName == MethodReferenceExtentions.compilerGeneratedAttributeName))
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

		public static bool HasSameSignatureWith(this MethodReference self, MethodReference other)
		{
			if (self.GetMethodSignature() != other.GetMethodSignature())
			{
				return false;
			}
			if (self.ReturnType.FullName == other.ReturnType.FullName)
			{
				return true;
			}
			if (self.ReturnType is GenericParameter && other.ReturnType is GenericParameter && (self.ReturnType as GenericParameter).Position == (other.ReturnType as GenericParameter).Position)
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
			if (!methodDefinition.IsPrivate)
			{
				return false;
			}
			return methodDefinition.HasOverrides;
		}

		public static bool IsExplicitImplementationOf(this MethodReference method, TypeDefinition @interface)
		{
			bool flag;
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
			MethodDefinition methodDefinition = method.Resolve();
			if (methodDefinition == null)
			{
				return false;
			}
			if (methodDefinition.HasOverrides)
			{
				Collection<MethodReference>.Enumerator enumerator = methodDefinition.Overrides.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						MethodDefinition methodDefinition1 = enumerator.Current.Resolve();
						if (!@interface.Methods.Contains(methodDefinition1))
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
			return false;
		}

		public static bool IsImplementationOf(this MethodReference method, TypeDefinition @interface)
		{
			bool flag;
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
			bool flag1 = false;
			string methodSignature = method.GetMethodSignature();
			foreach (MethodDefinition methodDefinition in @interface.Methods)
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
			TypeDefinition typeDefinition = method.DeclaringType.Resolve();
			if (typeDefinition == null)
			{
				return false;
			}
			List<TypeDefinition>.Enumerator enumerator = typeDefinition.GetBaseTypes().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.FullName != @interface.FullName)
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
			TypeDefinition declaringType = refernece.DeclaringType as TypeDefinition;
			if (declaringType == null && refernece.DeclaringType != null)
			{
				declaringType = refernece.DeclaringType.Resolve();
			}
			if (declaringType == null)
			{
				return null;
			}
			return declaringType.Methods.FirstOrDefault<MethodDefinition>((MethodDefinition x) => {
				if (x.Name != refernece.Name)
				{
					return false;
				}
				return MethodReferenceExtentions.FilterParameter(x.Parameters, refernece.Parameters);
			});
		}
	}
}