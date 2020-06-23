using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class PropertyDefinitionExtensions
	{
		public static TypeReference FixedPropertyType(this PropertyReference self)
		{
			TypeReference propertyType = self.PropertyType;
			if (propertyType is GenericParameter)
			{
				GenericParameter genericParameter = propertyType as GenericParameter;
				if (genericParameter.Owner is GenericInstanceType)
				{
					GenericInstanceType owner = genericParameter.Owner as GenericInstanceType;
					if (owner.PostionToArgument.ContainsKey(genericParameter.Position))
					{
						propertyType = owner.PostionToArgument[genericParameter.Position];
					}
				}
			}
			return propertyType;
		}

		public static ICollection<ImplementedMember> GetExplicitlyImplementedProperties(this PropertyDefinition self)
		{
			List<ImplementedMember> implementedMembers = new List<ImplementedMember>();
			if (!self.IsExplicitImplementation())
			{
				return implementedMembers;
			}
			if (self.GetMethod != null)
			{
				foreach (ImplementedMember explicitlyImplementedMethod in self.GetMethod.GetExplicitlyImplementedMethods())
				{
					PropertyDefinition methodDeclaringProperty = PropertyDefinitionExtensions.GetMethodDeclaringProperty(explicitlyImplementedMethod.Member as MethodReference);
					if (methodDeclaringProperty == null)
					{
						continue;
					}
					implementedMembers.Add(new ImplementedMember(explicitlyImplementedMethod.DeclaringType, methodDeclaringProperty));
				}
				return implementedMembers;
			}
			if (self.SetMethod == null)
			{
				return implementedMembers;
			}
			foreach (ImplementedMember implementedMember in self.SetMethod.GetExplicitlyImplementedMethods())
			{
				PropertyDefinition propertyDefinition = PropertyDefinitionExtensions.GetMethodDeclaringProperty(implementedMember.Member as MethodReference);
				if (propertyDefinition == null)
				{
					continue;
				}
				implementedMembers.Add(new ImplementedMember(implementedMember.DeclaringType, propertyDefinition));
			}
			return implementedMembers;
		}

		public static ICollection<ImplementedMember> GetImplementedProperties(this PropertyReference self)
		{
			List<ImplementedMember> implementedMembers = new List<ImplementedMember>();
			PropertyDefinition propertyDefinition = self.Resolve();
			if (propertyDefinition == null)
			{
				return implementedMembers;
			}
			if (propertyDefinition.GetMethod != null)
			{
				foreach (ImplementedMember implementedMethod in propertyDefinition.GetMethod.GetImplementedMethods())
				{
					PropertyDefinition methodDeclaringProperty = PropertyDefinitionExtensions.GetMethodDeclaringProperty(implementedMethod.Member as MethodReference);
					if (methodDeclaringProperty == null)
					{
						continue;
					}
					implementedMembers.Add(new ImplementedMember(implementedMethod.DeclaringType, methodDeclaringProperty));
				}
				return implementedMembers;
			}
			if (propertyDefinition.SetMethod == null)
			{
				return implementedMembers;
			}
			foreach (ImplementedMember implementedMember in propertyDefinition.SetMethod.GetImplementedMethods())
			{
				PropertyDefinition methodDeclaringProperty1 = PropertyDefinitionExtensions.GetMethodDeclaringProperty(implementedMember.Member as MethodReference);
				if (methodDeclaringProperty1 == null)
				{
					continue;
				}
				implementedMembers.Add(new ImplementedMember(implementedMember.DeclaringType, methodDeclaringProperty1));
			}
			return implementedMembers;
		}

		private static PropertyDefinition GetMethodDeclaringProperty(MethodReference method)
		{
			PropertyDefinition propertyDefinition;
			if (method == null)
			{
				return null;
			}
			TypeDefinition typeDefinition = method.DeclaringType.Resolve();
			if (typeDefinition != null)
			{
				Collection<PropertyDefinition>.Enumerator enumerator = typeDefinition.Properties.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						PropertyDefinition current = enumerator.Current;
						if ((current.GetMethod == null || !current.GetMethod.HasSameSignatureWith(method)) && (current.SetMethod == null || !current.SetMethod.HasSameSignatureWith(method)))
						{
							continue;
						}
						propertyDefinition = current;
						return propertyDefinition;
					}
					return null;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return propertyDefinition;
			}
			return null;
		}

		public static ICollection<PropertyDefinition> GetOverridenAndImplementedProperties(this PropertyDefinition property)
		{
			List<PropertyDefinition> propertyDefinitions = new List<PropertyDefinition>()
			{
				property
			};
			if (property.GetMethod != null)
			{
				foreach (MethodDefinition overridenAndImplementedMethod in property.GetMethod.GetOverridenAndImplementedMethods())
				{
					PropertyDefinition methodDeclaringProperty = PropertyDefinitionExtensions.GetMethodDeclaringProperty(overridenAndImplementedMethod);
					if (methodDeclaringProperty == null)
					{
						continue;
					}
					propertyDefinitions.Add(methodDeclaringProperty);
				}
			}
			else if (property.SetMethod != null)
			{
				foreach (MethodDefinition methodDefinition in property.SetMethod.GetOverridenAndImplementedMethods())
				{
					PropertyDefinition propertyDefinition = PropertyDefinitionExtensions.GetMethodDeclaringProperty(methodDefinition);
					if (propertyDefinition == null)
					{
						continue;
					}
					propertyDefinitions.Add(propertyDefinition);
				}
			}
			return propertyDefinitions;
		}

		private static bool HasAttribute(TypeDefinition declaringType, string attributeType, out CustomAttribute attribute)
		{
			bool flag;
			Collection<CustomAttribute>.Enumerator enumerator = declaringType.CustomAttributes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					CustomAttribute current = enumerator.Current;
					if (current.AttributeType.FullName != attributeType)
					{
						continue;
					}
					attribute = current;
					if (!attribute.IsResolved)
					{
						attribute.Resolve();
					}
					flag = true;
					return flag;
				}
				attribute = null;
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		public static bool IsAbstract(this PropertyDefinition self)
		{
			if (self.GetMethod != null && !self.GetMethod.IsAbstract)
			{
				return false;
			}
			if (self.SetMethod == null)
			{
				return true;
			}
			return self.SetMethod.IsAbstract;
		}

		public static bool IsExplicitImplementation(this PropertyReference self)
		{
			PropertyDefinition propertyDefinition = self.Resolve();
			if (propertyDefinition == null)
			{
				return false;
			}
			if (propertyDefinition.GetMethod != null && propertyDefinition.GetMethod.HasOverrides && propertyDefinition.GetMethod.IsPrivate)
			{
				return true;
			}
			if (propertyDefinition.SetMethod != null && propertyDefinition.SetMethod.HasOverrides && propertyDefinition.SetMethod.IsPrivate)
			{
				return true;
			}
			return false;
		}

		public static bool IsExplicitImplementationOf(this PropertyDefinition self, TypeDefinition @interface)
		{
			if (self.GetMethod != null && self.GetMethod.IsExplicitImplementationOf(@interface))
			{
				return true;
			}
			if (self.SetMethod == null)
			{
				return false;
			}
			return self.SetMethod.IsExplicitImplementationOf(@interface);
		}

		public static bool IsFinal(this PropertyDefinition self)
		{
			if (self.GetMethod != null && !self.GetMethod.IsFinal)
			{
				return false;
			}
			if (self.SetMethod == null)
			{
				return true;
			}
			return self.SetMethod.IsFinal;
		}

		public static bool IsIndexer(this PropertyReference self)
		{
			CustomAttribute customAttribute;
			Collection<MethodReference>.Enumerator enumerator;
			bool flag;
			TypeDefinition typeDefinition;
			if (self.DeclaringType != null)
			{
				typeDefinition = self.DeclaringType.Resolve();
			}
			else
			{
				typeDefinition = null;
			}
			TypeDefinition typeDefinition1 = typeDefinition;
			if (typeDefinition1 != null && typeDefinition1.HasCustomAttributes && PropertyDefinitionExtensions.HasAttribute(typeDefinition1, "System.Reflection.DefaultMemberAttribute", out customAttribute))
			{
				string str = customAttribute.ConstructorArguments[0].Value.ToString();
				if (self.Name == str)
				{
					return true;
				}
				if (!self.IsExplicitImplementation())
				{
					return false;
				}
				return self.Name.EndsWith(String.Concat(".", str));
			}
			PropertyDefinition propertyDefinition = self.Resolve();
			if (propertyDefinition != null)
			{
				if (propertyDefinition.GetMethod != null && propertyDefinition.GetMethod.HasOverrides)
				{
					foreach (MethodReference @override in propertyDefinition.GetMethod.Overrides)
					{
						PropertyDefinition methodDeclaringProperty = PropertyDefinitionExtensions.GetMethodDeclaringProperty(@override);
						if (methodDeclaringProperty == null || !methodDeclaringProperty.IsIndexer())
						{
							continue;
						}
						flag = true;
						return flag;
					}
				}
				if (propertyDefinition.SetMethod != null && propertyDefinition.SetMethod.HasOverrides)
				{
					enumerator = propertyDefinition.SetMethod.Overrides.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							PropertyDefinition methodDeclaringProperty1 = PropertyDefinitionExtensions.GetMethodDeclaringProperty(enumerator.Current);
							if (methodDeclaringProperty1 == null || !methodDeclaringProperty1.IsIndexer())
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
				}
				else
				{
					return false;
				}
				return flag;
			}
			return false;
		}

		public static bool IsNewSlot(this PropertyDefinition self)
		{
			if (self.GetMethod != null && !self.GetMethod.IsNewSlot)
			{
				return false;
			}
			if (self.SetMethod == null)
			{
				return true;
			}
			return self.SetMethod.IsNewSlot;
		}

		public static bool IsPrivate(this PropertyDefinition self)
		{
			if (self.GetMethod != null && !self.GetMethod.IsPrivate)
			{
				return false;
			}
			if (self.SetMethod == null)
			{
				return true;
			}
			return self.SetMethod.IsPrivate;
		}

		public static bool IsStatic(this PropertyDefinition self)
		{
			if (self.GetMethod != null && !self.GetMethod.IsStatic)
			{
				return false;
			}
			if (self.SetMethod == null)
			{
				return true;
			}
			return self.SetMethod.IsStatic;
		}

		public static bool IsVirtual(this PropertyDefinition self)
		{
			if (self.GetMethod != null && !self.GetMethod.IsVirtual)
			{
				return false;
			}
			if (self.SetMethod == null)
			{
				return true;
			}
			return self.SetMethod.IsVirtual;
		}

		public static bool ShouldStaySplit(this PropertyDefinition self)
		{
			if (self.SplitVirtual() || self.SplitFinal() || self.SplitNewslot() || self.SplitAbstract())
			{
				return true;
			}
			return self.SplitStatic();
		}

		private static bool SplitAbstract(this PropertyDefinition self)
		{
			if (self.IsAbstract())
			{
				return false;
			}
			if (self.GetMethod != null && self.GetMethod.IsAbstract)
			{
				return true;
			}
			if (self.SetMethod == null)
			{
				return false;
			}
			return self.SetMethod.IsAbstract;
		}

		private static bool SplitFinal(this PropertyDefinition self)
		{
			if (self.IsFinal())
			{
				return false;
			}
			if (self.GetMethod != null && self.GetMethod.IsFinal)
			{
				return true;
			}
			if (self.SetMethod == null)
			{
				return false;
			}
			return self.SetMethod.IsFinal;
		}

		private static bool SplitNewslot(this PropertyDefinition self)
		{
			if (self.IsNewSlot())
			{
				return false;
			}
			if (self.GetMethod != null && self.GetMethod.IsNewSlot)
			{
				return true;
			}
			if (self.SetMethod == null)
			{
				return false;
			}
			return self.SetMethod.IsNewSlot;
		}

		private static bool SplitStatic(this PropertyDefinition self)
		{
			if (self.IsStatic())
			{
				return false;
			}
			if (self.GetMethod != null && self.GetMethod.IsStatic)
			{
				return true;
			}
			if (self.SetMethod == null)
			{
				return false;
			}
			return self.SetMethod.IsStatic;
		}

		private static bool SplitVirtual(this PropertyDefinition self)
		{
			if (self.IsVirtual())
			{
				return false;
			}
			if (self.GetMethod != null && self.GetMethod.IsVirtual)
			{
				return true;
			}
			if (self.SetMethod == null)
			{
				return false;
			}
			return self.SetMethod.IsVirtual;
		}
	}
}