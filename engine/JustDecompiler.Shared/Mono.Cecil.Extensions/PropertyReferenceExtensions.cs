using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler;

namespace Mono.Cecil.Extensions
{
	public static class PropertyDefinitionExtensions
	{
        public static bool IsIndexer(this PropertyReference self)
        {
            TypeDefinition declaringType = self.DeclaringType != null ? self.DeclaringType.Resolve() : null;

            CustomAttribute attribute;
			if (declaringType != null && declaringType.HasCustomAttributes && HasAttribute(declaringType, "System.Reflection.DefaultMemberAttribute", out attribute))
			{
				string defaultPropertyName = attribute.ConstructorArguments[0].Value.ToString();
				if (self.Name == defaultPropertyName)
				{
					return true;
				}
				if (self.IsExplicitImplementation())
				{
					//return this.Name == DeclaringType.FullName + "." + defaultPropertyName;
					return self.Name.EndsWith("." + defaultPropertyName);
				}
				return false;
			}
			else
			{
				PropertyDefinition property = self.Resolve();
				if (property != null)
				{
					if (property.GetMethod != null && property.GetMethod.HasOverrides)
					{
						foreach (MethodReference overridenMethod in property.GetMethod.Overrides)
						{
							PropertyDefinition overridenProperty = GetMethodDeclaringProperty(overridenMethod);
							if (overridenProperty != null && overridenProperty.IsIndexer())
							{
								return true;
							}
						}
					}

					if (property.SetMethod != null && property.SetMethod.HasOverrides)
					{
						foreach (MethodReference overridenMethod in property.SetMethod.Overrides)
						{
							PropertyDefinition overridenProperty = GetMethodDeclaringProperty(overridenMethod);
							if (overridenProperty != null && overridenProperty.IsIndexer())
							{
								return true;
							}
						}
					}
				}

				return false;
			}
        }

		private static PropertyDefinition GetMethodDeclaringProperty(MethodReference method)
		{
			if (method == null)
			{
				return null;
			}

			TypeDefinition type = method.DeclaringType.Resolve();
			if (type != null)
			{
				foreach (PropertyDefinition property in type.Properties)
				{
					if ((property.GetMethod != null && property.GetMethod.HasSameSignatureWith(method)) ||
						(property.SetMethod != null && property.SetMethod.HasSameSignatureWith(method)))
					{
						return property;
					}
				}
			}

			return null;
		}

        private static bool HasAttribute(TypeDefinition declaringType, string attributeType, out CustomAttribute attribute)
        {
            foreach (CustomAttribute typeAttribute in declaringType.CustomAttributes)
            {
                if (typeAttribute.AttributeType.FullName == attributeType)
                {
                    attribute = typeAttribute;
                    if (!attribute.IsResolved)
                    {
                        attribute.Resolve();
                    }
                    return true;
                }
            }
            attribute = null;
            return false;
        }

		public static ICollection<PropertyDefinition> GetOverridenAndImplementedProperties(this PropertyDefinition property)
		{
			List<PropertyDefinition> result = new List<PropertyDefinition>();

			result.Add(property);

			if (property.GetMethod != null)
			{
				ICollection<MethodDefinition> implementedGetMethods = property.GetMethod.GetOverridenAndImplementedMethods();
				foreach (MethodDefinition implementedGetMethod in implementedGetMethods)
				{
					PropertyDefinition implementedProperty = GetMethodDeclaringProperty(implementedGetMethod);

					if (implementedProperty != null)
					{
						result.Add(implementedProperty);
					}
				}
			}
			else if (property.SetMethod != null)
			{
				ICollection<MethodDefinition> implementedSetMethods = property.SetMethod.GetOverridenAndImplementedMethods();
				foreach (MethodDefinition implementedSetMethod in implementedSetMethods)
				{
					PropertyDefinition implementedProperty = GetMethodDeclaringProperty(implementedSetMethod);

					if (implementedProperty != null)
					{
						result.Add(implementedProperty);
					}
				}
			}

			return result;
		}

		public static ICollection<ImplementedMember> GetImplementedProperties(this PropertyReference self)
		{
			List<ImplementedMember> result = new List<ImplementedMember>();

			PropertyDefinition property = self.Resolve();
			if (property == null)
			{
				return result;
			}

			if (property.GetMethod != null)
			{
				ICollection<ImplementedMember> implementedGetMethods = property.GetMethod.GetImplementedMethods();
				foreach (ImplementedMember implementedGetMethod in implementedGetMethods)
				{
					PropertyDefinition implementedProperty = GetMethodDeclaringProperty(implementedGetMethod.Member as MethodReference);
					if (implementedProperty != null)
					{
						result.Add(new ImplementedMember(implementedGetMethod.DeclaringType, implementedProperty));
					}
				}

				return result;
			}

			if (property.SetMethod != null)
			{
				ICollection<ImplementedMember> implementedSetMethods = property.SetMethod.GetImplementedMethods();
				foreach (ImplementedMember implementedSetMethod in implementedSetMethods)
				{
					PropertyDefinition implementedProperty = GetMethodDeclaringProperty(implementedSetMethod.Member as MethodReference);
					if (implementedProperty != null)
					{
						result.Add(new ImplementedMember(implementedSetMethod.DeclaringType, implementedProperty));
					}
				}

				return result;
			}

			return result;
		}

		public static ICollection<ImplementedMember> GetExplicitlyImplementedProperties(this PropertyDefinition self)
		{
			List<ImplementedMember> result = new List<ImplementedMember>();

			if (!self.IsExplicitImplementation())
			{
				return result;
			}

			if (self.GetMethod != null)
			{
				ICollection<ImplementedMember> explicitlyImplementedGetMethods = self.GetMethod.GetExplicitlyImplementedMethods();
				foreach (ImplementedMember explicitlyImplementedGetMethod in explicitlyImplementedGetMethods)
				{
					PropertyDefinition implementedProperty = GetMethodDeclaringProperty(explicitlyImplementedGetMethod.Member as MethodReference);
					if (implementedProperty != null)
					{
						result.Add(new ImplementedMember(explicitlyImplementedGetMethod.DeclaringType, implementedProperty));
					}
				}

				return result;
			}

			if (self.SetMethod != null)
			{
				ICollection<ImplementedMember> explicitlyImplementedSetMethods = self.SetMethod.GetExplicitlyImplementedMethods();
				foreach (ImplementedMember explicitlyImplementedSetMethod in explicitlyImplementedSetMethods)
				{
					PropertyDefinition implementedProperty = GetMethodDeclaringProperty(explicitlyImplementedSetMethod.Member as MethodReference);
					if (implementedProperty != null)
					{
						result.Add(new ImplementedMember(explicitlyImplementedSetMethod.DeclaringType, implementedProperty));
					}
				}

				return result;
			}

			return result;
		}


		public static bool IsExplicitImplementationOf(this PropertyDefinition self, TypeDefinition @interface)
		{
			return (self.GetMethod != null && self.GetMethod.IsExplicitImplementationOf(@interface)) ||
				(self.SetMethod != null && self.SetMethod.IsExplicitImplementationOf(@interface));
		}

        public static bool IsExplicitImplementation(this PropertyReference self)
        {
            PropertyDefinition property = self.Resolve();
            if (property == null)
            {
                return false;
            }
            if (property.GetMethod != null)
            {
                if (property.GetMethod.HasOverrides && property.GetMethod.IsPrivate)
                {
                    return true;
                }
            }
            if (property.SetMethod != null)
            {
                if (property.SetMethod.HasOverrides && property.SetMethod.IsPrivate)
                {
                    return true;
                }
            }
            return false;
        }

        public static TypeReference FixedPropertyType(this PropertyReference self)
        {
            TypeReference result = self.PropertyType;
            if (result is GenericParameter)
            {
                GenericParameter genRes = result as GenericParameter;
                if (genRes.Owner is GenericInstanceType)
                {
                    GenericInstanceType genericInstanceOwner = genRes.Owner as GenericInstanceType;
                    if (genericInstanceOwner.PostionToArgument.ContainsKey(genRes.Position))
                    {
                        result = genericInstanceOwner.PostionToArgument[genRes.Position];
                    }
                }
            }
            return result;
        }

		public static bool IsPrivate(this PropertyDefinition self)
		{
			return (self.GetMethod == null || self.GetMethod.IsPrivate) && (self.SetMethod == null || self.SetMethod.IsPrivate);
		}

        public static bool IsFinal(this PropertyDefinition self)
        {
            return (self.GetMethod == null || self.GetMethod.IsFinal) && (self.SetMethod == null || self.SetMethod.IsFinal);
        }

        public static bool IsNewSlot(this PropertyDefinition self)
        {
            return (self.GetMethod == null || self.GetMethod.IsNewSlot) && (self.SetMethod == null || self.SetMethod.IsNewSlot);
        }

        public static bool IsAbstract(this PropertyDefinition self)
        {
            return (self.GetMethod == null || self.GetMethod.IsAbstract) && (self.SetMethod == null || self.SetMethod.IsAbstract);
        }

        public static bool IsVirtual(this PropertyDefinition self)
        {
            return (self.GetMethod == null || self.GetMethod.IsVirtual) && (self.SetMethod == null || self.SetMethod.IsVirtual);
        }

        public static bool IsStatic(this PropertyDefinition self)
        {
            return (self.GetMethod == null || self.GetMethod.IsStatic) && (self.SetMethod == null || self.SetMethod.IsStatic);
        }

        public static bool ShouldStaySplit(this PropertyDefinition self)
        {
            return self.SplitVirtual() || self.SplitFinal() || self.SplitNewslot() || self.SplitAbstract() || self.SplitStatic();
        }

        #region Split Property
        private static bool SplitVirtual(this PropertyDefinition self)
        {
            if (self.IsVirtual())
            {
                return false;
            }
            return self.GetMethod != null && self.GetMethod.IsVirtual || self.SetMethod != null && self.SetMethod.IsVirtual;
        }

        private static bool SplitFinal(this PropertyDefinition self)
        {
            if (self.IsFinal())
            {
                return false;
            }
            return self.GetMethod != null && self.GetMethod.IsFinal || self.SetMethod != null && self.SetMethod.IsFinal;
        }

        private static bool SplitNewslot(this PropertyDefinition self)
        {
            if (self.IsNewSlot())
            {
                return false;
            }
            return self.GetMethod != null && self.GetMethod.IsNewSlot || self.SetMethod != null && self.SetMethod.IsNewSlot;
        }

        private static bool SplitAbstract(this PropertyDefinition self)
        {
            if (self.IsAbstract())
            {
                return false;
            }
            return self.GetMethod != null && self.GetMethod.IsAbstract || self.SetMethod != null && self.SetMethod.IsAbstract;
        }

        private static bool SplitStatic(this PropertyDefinition self)
        {
            if (self.IsStatic())
            {
                return false;
            }
            return self.GetMethod != null && self.GetMethod.IsStatic || self.SetMethod != null && self.SetMethod.IsStatic;
        }
        #endregion
	}
}
