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
			V_0 = self.get_PropertyType();
			if (V_0 as GenericParameter != null)
			{
				V_1 = V_0 as GenericParameter;
				if (V_1.get_Owner() as GenericInstanceType != null)
				{
					V_2 = V_1.get_Owner() as GenericInstanceType;
					if (V_2.get_PostionToArgument().ContainsKey(V_1.get_Position()))
					{
						V_0 = V_2.get_PostionToArgument().get_Item(V_1.get_Position());
					}
				}
			}
			return V_0;
		}

		public static ICollection<ImplementedMember> GetExplicitlyImplementedProperties(this PropertyDefinition self)
		{
			V_0 = new List<ImplementedMember>();
			if (!self.IsExplicitImplementation())
			{
				return V_0;
			}
			if (self.get_GetMethod() != null)
			{
				V_1 = self.get_GetMethod().GetExplicitlyImplementedMethods().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						V_3 = PropertyDefinitionExtensions.GetMethodDeclaringProperty(V_2.get_Member() as MethodReference);
						if (V_3 == null)
						{
							continue;
						}
						V_0.Add(new ImplementedMember(V_2.get_DeclaringType(), V_3));
					}
				}
				finally
				{
					if (V_1 != null)
					{
						V_1.Dispose();
					}
				}
				return V_0;
			}
			if (self.get_SetMethod() == null)
			{
				return V_0;
			}
			V_1 = self.get_SetMethod().GetExplicitlyImplementedMethods().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_4 = V_1.get_Current();
					V_5 = PropertyDefinitionExtensions.GetMethodDeclaringProperty(V_4.get_Member() as MethodReference);
					if (V_5 == null)
					{
						continue;
					}
					V_0.Add(new ImplementedMember(V_4.get_DeclaringType(), V_5));
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}

		public static ICollection<ImplementedMember> GetImplementedProperties(this PropertyReference self)
		{
			V_0 = new List<ImplementedMember>();
			V_1 = self.Resolve();
			if (V_1 == null)
			{
				return V_0;
			}
			if (V_1.get_GetMethod() != null)
			{
				V_2 = V_1.get_GetMethod().GetImplementedMethods().GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = V_2.get_Current();
						V_4 = PropertyDefinitionExtensions.GetMethodDeclaringProperty(V_3.get_Member() as MethodReference);
						if (V_4 == null)
						{
							continue;
						}
						V_0.Add(new ImplementedMember(V_3.get_DeclaringType(), V_4));
					}
				}
				finally
				{
					if (V_2 != null)
					{
						V_2.Dispose();
					}
				}
				return V_0;
			}
			if (V_1.get_SetMethod() == null)
			{
				return V_0;
			}
			V_2 = V_1.get_SetMethod().GetImplementedMethods().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_5 = V_2.get_Current();
					V_6 = PropertyDefinitionExtensions.GetMethodDeclaringProperty(V_5.get_Member() as MethodReference);
					if (V_6 == null)
					{
						continue;
					}
					V_0.Add(new ImplementedMember(V_5.get_DeclaringType(), V_6));
				}
			}
			finally
			{
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
			return V_0;
		}

		private static PropertyDefinition GetMethodDeclaringProperty(MethodReference method)
		{
			if (method == null)
			{
				return null;
			}
			V_0 = method.get_DeclaringType().Resolve();
			if (V_0 != null)
			{
				V_1 = V_0.get_Properties().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						if (V_2.get_GetMethod() == null || !V_2.get_GetMethod().HasSameSignatureWith(method) && V_2.get_SetMethod() == null || !V_2.get_SetMethod().HasSameSignatureWith(method))
						{
							continue;
						}
						V_3 = V_2;
						goto Label1;
					}
					goto Label0;
				}
				finally
				{
					V_1.Dispose();
				}
			Label1:
				return V_3;
			}
		Label0:
			return null;
		}

		public static ICollection<PropertyDefinition> GetOverridenAndImplementedProperties(this PropertyDefinition property)
		{
			V_0 = new List<PropertyDefinition>();
			V_0.Add(property);
			if (property.get_GetMethod() == null)
			{
				if (property.get_SetMethod() != null)
				{
					V_1 = property.get_SetMethod().GetOverridenAndImplementedMethods().GetEnumerator();
					try
					{
						while (V_1.MoveNext())
						{
							V_3 = PropertyDefinitionExtensions.GetMethodDeclaringProperty(V_1.get_Current());
							if (V_3 == null)
							{
								continue;
							}
							V_0.Add(V_3);
						}
					}
					finally
					{
						if (V_1 != null)
						{
							V_1.Dispose();
						}
					}
				}
			}
			else
			{
				V_1 = property.get_GetMethod().GetOverridenAndImplementedMethods().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = PropertyDefinitionExtensions.GetMethodDeclaringProperty(V_1.get_Current());
						if (V_2 == null)
						{
							continue;
						}
						V_0.Add(V_2);
					}
				}
				finally
				{
					if (V_1 != null)
					{
						V_1.Dispose();
					}
				}
			}
			return V_0;
		}

		private static bool HasAttribute(TypeDefinition declaringType, string attributeType, out CustomAttribute attribute)
		{
			V_0 = declaringType.get_CustomAttributes().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!String.op_Equality(V_1.get_AttributeType().get_FullName(), attributeType))
					{
						continue;
					}
					attribute = V_1;
					if (!attribute.get_IsResolved())
					{
						attribute.Resolve();
					}
					V_2 = true;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				V_0.Dispose();
			}
		Label1:
			return V_2;
		Label0:
			attribute = null;
			return false;
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
			V_0 = self.Resolve();
			if (V_0 == null)
			{
				return false;
			}
			if (V_0.get_GetMethod() != null && V_0.get_GetMethod().get_HasOverrides() && V_0.get_GetMethod().get_IsPrivate())
			{
				return true;
			}
			if (V_0.get_SetMethod() != null && V_0.get_SetMethod().get_HasOverrides() && V_0.get_SetMethod().get_IsPrivate())
			{
				return true;
			}
			return false;
		}

		public static bool IsExplicitImplementationOf(this PropertyDefinition self, TypeDefinition interface)
		{
			if (self.get_GetMethod() != null && self.get_GetMethod().IsExplicitImplementationOf(interface))
			{
				return true;
			}
			if (self.get_SetMethod() == null)
			{
				return false;
			}
			return self.get_SetMethod().IsExplicitImplementationOf(interface);
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
			if (self.get_DeclaringType() != null)
			{
				stackVariable4 = self.get_DeclaringType().Resolve();
			}
			else
			{
				stackVariable4 = null;
			}
			V_0 = stackVariable4;
			if (V_0 != null && V_0.get_HasCustomAttributes() && PropertyDefinitionExtensions.HasAttribute(V_0, "System.Reflection.DefaultMemberAttribute", out V_1))
			{
				V_3 = V_1.get_ConstructorArguments().get_Item(0);
				V_2 = V_3.get_Value().ToString();
				if (String.op_Equality(self.get_Name(), V_2))
				{
					return true;
				}
				if (!self.IsExplicitImplementation())
				{
					return false;
				}
				return self.get_Name().EndsWith(String.Concat(".", V_2));
			}
			V_4 = self.Resolve();
			if (V_4 != null)
			{
				if (V_4.get_GetMethod() != null && V_4.get_GetMethod().get_HasOverrides())
				{
					V_5 = V_4.get_GetMethod().get_Overrides().GetEnumerator();
					try
					{
						while (V_5.MoveNext())
						{
							V_6 = PropertyDefinitionExtensions.GetMethodDeclaringProperty(V_5.get_Current());
							if (V_6 == null || !V_6.IsIndexer())
							{
								continue;
							}
							V_7 = true;
							goto Label0;
						}
					}
					finally
					{
						V_5.Dispose();
					}
				}
				if (V_4.get_SetMethod() != null && V_4.get_SetMethod().get_HasOverrides())
				{
					V_5 = V_4.get_SetMethod().get_Overrides().GetEnumerator();
					try
					{
						while (V_5.MoveNext())
						{
							V_8 = PropertyDefinitionExtensions.GetMethodDeclaringProperty(V_5.get_Current());
							if (V_8 == null || !V_8.IsIndexer())
							{
								continue;
							}
							V_7 = true;
							goto Label0;
						}
						goto Label1;
					}
					finally
					{
						V_5.Dispose();
					}
				}
				else
				{
					goto Label1;
				}
			Label0:
				return V_7;
			}
		Label1:
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