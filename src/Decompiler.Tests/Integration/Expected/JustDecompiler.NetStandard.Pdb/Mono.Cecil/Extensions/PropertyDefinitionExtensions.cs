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
			TypeReference propertyType = self.get_PropertyType();
			if (propertyType is GenericParameter)
			{
				GenericParameter genericParameter = propertyType as GenericParameter;
				if (genericParameter.get_Owner() is GenericInstanceType)
				{
					GenericInstanceType owner = genericParameter.get_Owner() as GenericInstanceType;
					if (owner.get_PostionToArgument().ContainsKey(genericParameter.get_Position()))
					{
						propertyType = owner.get_PostionToArgument()[genericParameter.get_Position()];
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
			if (self.get_GetMethod() != null)
			{
				foreach (ImplementedMember explicitlyImplementedMethod in self.get_GetMethod().GetExplicitlyImplementedMethods())
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
			if (self.get_SetMethod() == null)
			{
				return implementedMembers;
			}
			foreach (ImplementedMember implementedMember in self.get_SetMethod().GetExplicitlyImplementedMethods())
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
			if (propertyDefinition.get_GetMethod() != null)
			{
				foreach (ImplementedMember implementedMethod in propertyDefinition.get_GetMethod().GetImplementedMethods())
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
			if (propertyDefinition.get_SetMethod() == null)
			{
				return implementedMembers;
			}
			foreach (ImplementedMember implementedMember in propertyDefinition.get_SetMethod().GetImplementedMethods())
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
			TypeDefinition typeDefinition = method.get_DeclaringType().Resolve();
			if (typeDefinition != null)
			{
				Collection<PropertyDefinition>.Enumerator enumerator = typeDefinition.get_Properties().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						PropertyDefinition current = enumerator.get_Current();
						if ((current.get_GetMethod() == null || !current.get_GetMethod().HasSameSignatureWith(method)) && (current.get_SetMethod() == null || !current.get_SetMethod().HasSameSignatureWith(method)))
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
					enumerator.Dispose();
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
			if (property.get_GetMethod() != null)
			{
				foreach (MethodDefinition overridenAndImplementedMethod in property.get_GetMethod().GetOverridenAndImplementedMethods())
				{
					PropertyDefinition methodDeclaringProperty = PropertyDefinitionExtensions.GetMethodDeclaringProperty(overridenAndImplementedMethod);
					if (methodDeclaringProperty == null)
					{
						continue;
					}
					propertyDefinitions.Add(methodDeclaringProperty);
				}
			}
			else if (property.get_SetMethod() != null)
			{
				foreach (MethodDefinition methodDefinition in property.get_SetMethod().GetOverridenAndImplementedMethods())
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
			Collection<CustomAttribute>.Enumerator enumerator = declaringType.get_CustomAttributes().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					CustomAttribute current = enumerator.get_Current();
					if (current.get_AttributeType().get_FullName() != attributeType)
					{
						continue;
					}
					attribute = current;
					if (!attribute.get_IsResolved())
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
				enumerator.Dispose();
			}
			return flag;
		}

		public static bool IsAbstract(this PropertyDefinition self)
		{
			if (self.get_GetMethod() != null && !self.get_GetMethod().get_IsAbstract())
			{
				return false;
			}
			if (self.get_SetMethod() == null)
			{
				return true;
			}
			return self.get_SetMethod().get_IsAbstract();
		}

		public static bool IsExplicitImplementation(this PropertyReference self)
		{
			PropertyDefinition propertyDefinition = self.Resolve();
			if (propertyDefinition == null)
			{
				return false;
			}
			if (propertyDefinition.get_GetMethod() != null && propertyDefinition.get_GetMethod().get_HasOverrides() && propertyDefinition.get_GetMethod().get_IsPrivate())
			{
				return true;
			}
			if (propertyDefinition.get_SetMethod() != null && propertyDefinition.get_SetMethod().get_HasOverrides() && propertyDefinition.get_SetMethod().get_IsPrivate())
			{
				return true;
			}
			return false;
		}

		public static bool IsExplicitImplementationOf(this PropertyDefinition self, TypeDefinition @interface)
		{
			if (self.get_GetMethod() != null && self.get_GetMethod().IsExplicitImplementationOf(@interface))
			{
				return true;
			}
			if (self.get_SetMethod() == null)
			{
				return false;
			}
			return self.get_SetMethod().IsExplicitImplementationOf(@interface);
		}

		public static bool IsFinal(this PropertyDefinition self)
		{
			if (self.get_GetMethod() != null && !self.get_GetMethod().get_IsFinal())
			{
				return false;
			}
			if (self.get_SetMethod() == null)
			{
				return true;
			}
			return self.get_SetMethod().get_IsFinal();
		}

		public static bool IsIndexer(this PropertyReference self)
		{
			CustomAttribute customAttribute;
			Collection<MethodReference>.Enumerator enumerator;
			bool flag;
			TypeDefinition typeDefinition;
			if (self.get_DeclaringType() != null)
			{
				typeDefinition = self.get_DeclaringType().Resolve();
			}
			else
			{
				typeDefinition = null;
			}
			TypeDefinition typeDefinition1 = typeDefinition;
			if (typeDefinition1 != null && typeDefinition1.get_HasCustomAttributes() && PropertyDefinitionExtensions.HasAttribute(typeDefinition1, "System.Reflection.DefaultMemberAttribute", out customAttribute))
			{
				CustomAttributeArgument item = customAttribute.get_ConstructorArguments().get_Item(0);
				string str = item.get_Value().ToString();
				if (self.get_Name() == str)
				{
					return true;
				}
				if (!self.IsExplicitImplementation())
				{
					return false;
				}
				return self.get_Name().EndsWith(String.Concat(".", str));
			}
			PropertyDefinition propertyDefinition = self.Resolve();
			if (propertyDefinition != null)
			{
				if (propertyDefinition.get_GetMethod() != null && propertyDefinition.get_GetMethod().get_HasOverrides())
				{
					foreach (MethodReference @override in propertyDefinition.get_GetMethod().get_Overrides())
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
				if (propertyDefinition.get_SetMethod() != null && propertyDefinition.get_SetMethod().get_HasOverrides())
				{
					enumerator = propertyDefinition.get_SetMethod().get_Overrides().GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							PropertyDefinition methodDeclaringProperty1 = PropertyDefinitionExtensions.GetMethodDeclaringProperty(enumerator.get_Current());
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
						enumerator.Dispose();
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
			if (self.get_GetMethod() != null && !self.get_GetMethod().get_IsNewSlot())
			{
				return false;
			}
			if (self.get_SetMethod() == null)
			{
				return true;
			}
			return self.get_SetMethod().get_IsNewSlot();
		}

		public static bool IsPrivate(this PropertyDefinition self)
		{
			if (self.get_GetMethod() != null && !self.get_GetMethod().get_IsPrivate())
			{
				return false;
			}
			if (self.get_SetMethod() == null)
			{
				return true;
			}
			return self.get_SetMethod().get_IsPrivate();
		}

		public static bool IsStatic(this PropertyDefinition self)
		{
			if (self.get_GetMethod() != null && !self.get_GetMethod().get_IsStatic())
			{
				return false;
			}
			if (self.get_SetMethod() == null)
			{
				return true;
			}
			return self.get_SetMethod().get_IsStatic();
		}

		public static bool IsVirtual(this PropertyDefinition self)
		{
			if (self.get_GetMethod() != null && !self.get_GetMethod().get_IsVirtual())
			{
				return false;
			}
			if (self.get_SetMethod() == null)
			{
				return true;
			}
			return self.get_SetMethod().get_IsVirtual();
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
			if (self.get_GetMethod() != null && self.get_GetMethod().get_IsAbstract())
			{
				return true;
			}
			if (self.get_SetMethod() == null)
			{
				return false;
			}
			return self.get_SetMethod().get_IsAbstract();
		}

		private static bool SplitFinal(this PropertyDefinition self)
		{
			if (self.IsFinal())
			{
				return false;
			}
			if (self.get_GetMethod() != null && self.get_GetMethod().get_IsFinal())
			{
				return true;
			}
			if (self.get_SetMethod() == null)
			{
				return false;
			}
			return self.get_SetMethod().get_IsFinal();
		}

		private static bool SplitNewslot(this PropertyDefinition self)
		{
			if (self.IsNewSlot())
			{
				return false;
			}
			if (self.get_GetMethod() != null && self.get_GetMethod().get_IsNewSlot())
			{
				return true;
			}
			if (self.get_SetMethod() == null)
			{
				return false;
			}
			return self.get_SetMethod().get_IsNewSlot();
		}

		private static bool SplitStatic(this PropertyDefinition self)
		{
			if (self.IsStatic())
			{
				return false;
			}
			if (self.get_GetMethod() != null && self.get_GetMethod().get_IsStatic())
			{
				return true;
			}
			if (self.get_SetMethod() == null)
			{
				return false;
			}
			return self.get_SetMethod().get_IsStatic();
		}

		private static bool SplitVirtual(this PropertyDefinition self)
		{
			if (self.IsVirtual())
			{
				return false;
			}
			if (self.get_GetMethod() != null && self.get_GetMethod().get_IsVirtual())
			{
				return true;
			}
			if (self.get_SetMethod() == null)
			{
				return false;
			}
			return self.get_SetMethod().get_IsVirtual();
		}
	}
}