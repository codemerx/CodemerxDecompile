using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class MethodReferenceExtentions
	{
		private static string compilerGeneratedAttributeName;

		static MethodReferenceExtentions()
		{
			MethodReferenceExtentions.compilerGeneratedAttributeName = Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mono.Cecil.Extensions.MethodReferenceExtentions::.cctor()
			// Exception in: System.Void .cctor()
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private static bool FilterParameter(IList<ParameterDefinition> source, IList<ParameterDefinition> desitnation)
		{
			V_0 = true;
			if (source.get_Count() != desitnation.get_Count())
			{
				V_0 = false;
			}
			else
			{
				V_1 = 0;
				while (V_1 < source.get_Count())
				{
					if (!String.op_Inequality(source.get_Item(V_1).get_ParameterType().get_Name(), desitnation.get_Item(V_1).get_ParameterType().get_Name()))
					{
						V_1 = V_1 + 1;
					}
					else
					{
						V_0 = false;
						goto Label0;
					}
				}
			}
		Label0:
			return V_0;
		}

		private static MethodReference GetExplicitlyImplementedMethodFromInterface(this MethodReference method, TypeDefinition interface)
		{
			if (interface == null)
			{
				throw new ArgumentNullException("@interface can not be null.");
			}
			if (!interface.get_IsInterface())
			{
				throw new ArgumentOutOfRangeException("The @interface argument is not an interface definition.");
			}
			if (String.op_Equality(method.get_DeclaringType().get_FullName(), interface.get_FullName()))
			{
				return method;
			}
			V_0 = method.Resolve();
			if (V_0 == null)
			{
				return null;
			}
			if (V_0.get_HasOverrides())
			{
				V_1 = interface.get_Methods().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						V_3 = V_0.get_Overrides().GetEnumerator();
						try
						{
							while (V_3.MoveNext())
							{
								if (!String.op_Equality(V_3.get_Current().Resolve().get_FullName(), V_2.get_FullName()))
								{
									continue;
								}
								V_4 = V_2;
								goto Label1;
							}
						}
						finally
						{
							V_3.Dispose();
						}
					}
					goto Label0;
				}
				finally
				{
					V_1.Dispose();
				}
			Label1:
				return V_4;
			}
		Label0:
			return null;
		}

		public static ICollection<ImplementedMember> GetExplicitlyImplementedMethods(this MethodDefinition method)
		{
			V_0 = new HashSet<ImplementedMember>();
			if (!method.IsExplicitImplementation())
			{
				return V_0;
			}
			V_1 = method.get_DeclaringType().get_Interfaces().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_3 = V_2.Resolve();
					if (V_3 == null)
					{
						continue;
					}
					V_4 = method.GetExplicitlyImplementedMethodFromInterface(V_3);
					if (V_4 == null)
					{
						continue;
					}
					V_5 = new ImplementedMember(V_2, V_4);
					if (V_0.Contains(V_5))
					{
						continue;
					}
					dummyVar0 = V_0.Add(V_5);
				}
			}
			finally
			{
				V_1.Dispose();
			}
			return V_0.ToList<ImplementedMember>();
		}

		private static MethodDefinition GetImplementedMethodFromGenericInstanceType(this MethodDefinition self, GenericInstanceType type)
		{
			V_0 = type.Resolve();
			if (V_0 == null)
			{
				return null;
			}
			V_1 = V_0.get_Methods().GetEnumerator();
			try
			{
				do
				{
				Label2:
					if (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						if (String.op_Equality(V_2.get_Name(), self.get_Name()))
						{
							if (!V_2.get_HasParameters() || !self.get_HasParameters() || V_2.get_Parameters().get_Count() != self.get_Parameters().get_Count())
							{
								goto Label1;
							}
							if (!V_2.get_ReturnType().get_IsGenericParameter())
							{
								continue;
							}
							V_3 = (V_2.get_ReturnType() as GenericParameter).get_Position();
							if (type.get_PostionToArgument().TryGetValue(V_3, out V_4))
							{
								if (!String.op_Inequality(V_4.get_FullName(), self.get_ReturnType().get_FullName()))
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
						goto Label0;
					}
				}
				while (String.op_Inequality(V_2.get_ReturnType().get_FullName(), self.get_ReturnType().get_FullName()));
				V_5 = 0;
				while (V_5 < V_2.get_Parameters().get_Count())
				{
					V_6 = V_2.get_Parameters().get_Item(V_5).get_ParameterType();
					if (!V_6.get_IsGenericParameter())
					{
						dummyVar0 = String.op_Inequality(V_6.get_FullName(), self.get_Parameters().get_Item(V_5).get_ParameterType().get_FullName());
					}
					else
					{
						V_7 = (V_6 as GenericParameter).get_Position();
						if (type.get_PostionToArgument().TryGetValue(V_7, out V_8) && String.op_Inequality(V_8.get_FullName(), self.get_Parameters().get_Item(V_5).get_ParameterType().get_FullName()))
						{
						}
					}
					V_5 = V_5 + 1;
				}
			Label1:
				V_9 = V_2;
			}
			finally
			{
				V_1.Dispose();
			}
			return V_9;
		Label0:
			return null;
		}

		private static MethodReference GetImplementedMethodFromInterface(this MethodReference method, TypeDefinition interface)
		{
			if (interface == null)
			{
				throw new ArgumentNullException("@interface can not be null.");
			}
			if (!interface.get_IsInterface())
			{
				throw new ArgumentOutOfRangeException("The @interface argument is not an interface definition.");
			}
			V_0 = method.get_DeclaringType().Resolve();
			if (V_0 == null)
			{
				return null;
			}
			V_1 = false;
			V_3 = V_0.GetBaseTypes().GetEnumerator();
			try
			{
				while (V_3.MoveNext())
				{
					if (!String.op_Equality(V_3.get_Current().get_FullName(), interface.get_FullName()))
					{
						continue;
					}
					V_1 = true;
					goto Label0;
				}
			}
			finally
			{
				((IDisposable)V_3).Dispose();
			}
		Label0:
			if (!V_1)
			{
				return null;
			}
			if (String.op_Equality(method.get_DeclaringType().get_FullName(), interface.get_FullName()))
			{
				return method;
			}
			V_2 = method.GetExplicitlyImplementedMethodFromInterface(interface);
			if (V_2 != null)
			{
				return V_2;
			}
			V_4 = interface.get_Methods().GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_5 = V_4.get_Current();
					if (!method.HasSameSignatureWith(V_5))
					{
						continue;
					}
					V_6 = V_5;
					goto Label2;
				}
				goto Label1;
			}
			finally
			{
				V_4.Dispose();
			}
		Label2:
			return V_6;
		Label1:
			return null;
		}

		public static ICollection<ImplementedMember> GetImplementedMethods(this MethodDefinition method)
		{
			V_0 = new HashSet<ImplementedMember>();
			V_1 = method.get_DeclaringType().get_Interfaces().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2 as GenericInstanceType != null)
					{
						V_5 = method.GetImplementedMethodFromGenericInstanceType(V_2 as GenericInstanceType);
						if (V_5 != null)
						{
							V_6 = new ImplementedMember(V_2, V_5);
							if (!V_0.Contains(V_6))
							{
								dummyVar0 = V_0.Add(V_6);
								continue;
							}
						}
					}
					V_3 = V_2.Resolve();
					if (V_3 == null)
					{
						continue;
					}
					V_7 = method.GetImplementedMethodFromInterface(V_3);
					if (V_7 == null)
					{
						continue;
					}
					V_8 = new ImplementedMember(V_2, V_7);
					if (V_0.Contains(V_8))
					{
						continue;
					}
					dummyVar1 = V_0.Add(V_8);
				}
			}
			finally
			{
				V_1.Dispose();
			}
			return V_0.ToList<ImplementedMember>();
		}

		private static string GetMethodSignature(this MethodReference method)
		{
			stackVariable1 = method.get_FullName();
			return stackVariable1.Substring(stackVariable1.IndexOf("::"));
		}

		public static ICollection<MethodDefinition> GetOverridenAndImplementedMethods(this MethodDefinition method)
		{
			V_0 = new HashSet<MethodDefinition>();
			dummyVar0 = V_0.Add(method);
			V_2 = method.GetImplementedMethods().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					if (V_3.get_Member() as MethodReference == null)
					{
						continue;
					}
					V_4 = (V_3.get_Member() as MethodReference).Resolve();
					if (V_4 == null)
					{
						continue;
					}
					dummyVar1 = V_0.Add(V_4);
				}
			}
			finally
			{
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
			V_1 = method.get_DeclaringType().get_BaseType();
			while (V_1 != null)
			{
				V_5 = V_1.Resolve();
				if (V_1 as GenericInstanceType == null)
				{
					if (V_5 != null && V_5.get_HasMethods())
					{
						V_7 = V_5.get_Methods().GetEnumerator();
						try
						{
							while (V_7.MoveNext())
							{
								V_8 = V_7.get_Current();
								if (!method.HasSameSignatureWith(V_8) || V_0.Contains(V_8))
								{
									continue;
								}
								dummyVar3 = V_0.Add(V_8);
							}
						}
						finally
						{
							V_7.Dispose();
						}
					}
				}
				else
				{
					V_6 = method.GetImplementedMethodFromGenericInstanceType(V_1 as GenericInstanceType);
					if (!V_0.Contains(V_6))
					{
						dummyVar2 = V_0.Add(V_6);
					}
				}
				if (V_5 == null || V_5.get_BaseType() == null)
				{
					stackVariable32 = null;
				}
				else
				{
					stackVariable32 = V_5.get_BaseType();
				}
				V_1 = stackVariable32;
			}
			return V_0.ToList<MethodDefinition>();
		}

		public static bool HasCompilerGeneratedAttribute(this ICustomAttributeProvider attributeProvider)
		{
			if (attributeProvider.get_CustomAttributes() == null)
			{
				return false;
			}
			V_0 = attributeProvider.get_CustomAttributes().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1.get_Constructor() == null || V_1.get_AttributeType() == null || !String.op_Equality(V_1.get_AttributeType().get_FullName(), MethodReferenceExtentions.compilerGeneratedAttributeName))
					{
						continue;
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
			return false;
		}

		public static bool HasSameSignatureWith(this MethodReference self, MethodReference other)
		{
			if (!String.op_Equality(self.GetMethodSignature(), other.GetMethodSignature()))
			{
				return false;
			}
			if (!String.op_Inequality(self.get_ReturnType().get_FullName(), other.get_ReturnType().get_FullName()))
			{
				return true;
			}
			if (self.get_ReturnType() as GenericParameter != null && other.get_ReturnType() as GenericParameter != null && (self.get_ReturnType() as GenericParameter).get_Position() == (other.get_ReturnType() as GenericParameter).get_Position())
			{
				return true;
			}
			return false;
		}

		public static bool IsCompilerGenerated(this MethodReference method, bool isAssemblyResolverChacheEnabled = true)
		{
			V_0 = method as MethodDefinition;
			if (V_0 == null)
			{
				V_0 = method.ResolveDefinition(isAssemblyResolverChacheEnabled);
			}
			if (V_0 == null)
			{
				return false;
			}
			return V_0.HasCompilerGeneratedAttribute();
		}

		public static bool IsExplicitImplementation(this MethodReference method)
		{
			V_0 = method.Resolve();
			if (V_0 == null)
			{
				return false;
			}
			if (!V_0.get_IsPrivate())
			{
				return false;
			}
			return V_0.get_HasOverrides();
		}

		public static bool IsExplicitImplementationOf(this MethodReference method, TypeDefinition interface)
		{
			if (interface == null)
			{
				throw new ArgumentNullException("@interface can not be null.");
			}
			if (!interface.get_IsInterface())
			{
				throw new ArgumentOutOfRangeException("The @interface argument is not an interface definition.");
			}
			if (String.op_Equality(method.get_DeclaringType().get_FullName(), interface.get_FullName()))
			{
				return true;
			}
			V_0 = method.Resolve();
			if (V_0 == null)
			{
				return false;
			}
			if (V_0.get_HasOverrides())
			{
				V_1 = V_0.get_Overrides().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current().Resolve();
						if (!interface.get_Methods().Contains(V_2))
						{
							continue;
						}
						V_3 = true;
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
			return false;
		}

		public static bool IsImplementationOf(this MethodReference method, TypeDefinition interface)
		{
			if (interface == null)
			{
				throw new ArgumentNullException("@interface can not be null.");
			}
			if (!interface.get_IsInterface())
			{
				throw new ArgumentOutOfRangeException("The @interface argument is not an interface definition.");
			}
			if (String.op_Equality(method.get_DeclaringType().get_FullName(), interface.get_FullName()))
			{
				return true;
			}
			if (method.IsExplicitImplementationOf(interface))
			{
				return true;
			}
			V_0 = false;
			V_1 = method.GetMethodSignature();
			V_3 = interface.get_Methods().GetEnumerator();
			try
			{
				while (V_3.MoveNext())
				{
					if (!String.op_Equality(V_3.get_Current().GetMethodSignature(), V_1))
					{
						continue;
					}
					V_0 = true;
				}
			}
			finally
			{
				V_3.Dispose();
			}
			if (!V_0)
			{
				return false;
			}
			V_2 = method.get_DeclaringType().Resolve();
			if (V_2 == null)
			{
				return false;
			}
			V_4 = V_2.GetBaseTypes().GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					if (!String.op_Equality(V_4.get_Current().get_FullName(), interface.get_FullName()))
					{
						continue;
					}
					V_5 = true;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_4).Dispose();
			}
		Label1:
			return V_5;
		Label0:
			return false;
		}

		public static MethodDefinition ResolveDefinition(this MethodReference refernece, bool isAssemblyResolverChacheEnabled = true)
		{
			V_0 = new MethodReferenceExtentions.u003cu003ec__DisplayClass1_0();
			V_0.refernece = refernece;
			V_1 = V_0.refernece.get_DeclaringType() as TypeDefinition;
			if (V_1 == null && V_0.refernece.get_DeclaringType() != null)
			{
				V_1 = V_0.refernece.get_DeclaringType().Resolve();
			}
			if (V_1 == null)
			{
				return null;
			}
			return V_1.get_Methods().FirstOrDefault<MethodDefinition>(new Func<MethodDefinition, bool>(V_0.u003cResolveDefinitionu003eb__0));
		}
	}
}