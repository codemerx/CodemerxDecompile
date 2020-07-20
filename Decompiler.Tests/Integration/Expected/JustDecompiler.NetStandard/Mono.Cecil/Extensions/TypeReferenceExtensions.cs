using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Mono.Cecil.Extensions
{
	public static class TypeReferenceExtensions
	{
		public static bool ContainsAnonymousType(this TypeReference self)
		{
			if (self as GenericInstanceType == null)
			{
				if (self as TypeSpecification != null)
				{
					return (self as TypeSpecification).get_ElementType().ContainsAnonymousType();
				}
			}
			else
			{
				if (self.Resolve().IsAnonymous())
				{
					return true;
				}
				if ((self as IGenericInstance).HasAnonymousArgument())
				{
					return true;
				}
			}
			return false;
		}

		public static Expression GetDefaultValueExpression(this TypeReference typeReference, TypeSystem typeSystem)
		{
			if (typeReference.get_IsPrimitive())
			{
				V_0 = typeReference.get_FullName();
				if (V_0 != null)
				{
					if (String.op_Equality(V_0, "System.Boolean"))
					{
						return new LiteralExpression(false, typeSystem, null);
					}
					if (String.op_Equality(V_0, "System.Char"))
					{
						return new LiteralExpression((object)'\0', typeSystem, null);
					}
					if (String.op_Equality(V_0, "System.IntPtr"))
					{
						return new DefaultObjectExpression(typeReference, null);
					}
				}
				return new LiteralExpression(Activator.CreateInstance(Type.GetType(V_0)), typeSystem, null);
			}
			if (typeReference.get_IsGenericParameter())
			{
				return new DefaultObjectExpression(typeReference, null);
			}
			if (typeReference.get_IsArray())
			{
				return new LiteralExpression(null, typeSystem, null);
			}
			if (!typeReference.get_IsValueType())
			{
				if (!typeReference.get_IsRequiredModifier())
				{
					return new LiteralExpression(null, typeSystem, null);
				}
				return (typeReference as RequiredModifierType).get_ElementType().GetDefaultValueExpression(typeSystem);
			}
			V_1 = typeReference.Resolve();
			if (V_1 != null && V_1.get_IsEnum())
			{
				return new LiteralExpression((object)0, typeSystem, null);
			}
			return new ObjectCreationExpression(typeReference.GetEmptyConstructorReference(), typeReference, null, null);
		}

		internal static MethodReference GetEmptyConstructorReference(this TypeReference self)
		{
			if (self == null)
			{
				throw new ArgumentNullException("self");
			}
			if (self.get_IsGenericInstance())
			{
				return null;
			}
			V_0 = self.Resolve();
			if (V_0 == null)
			{
				return null;
			}
			V_1 = V_0.get_Methods().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (!V_2.get_IsConstructor() || V_2.get_HasParameters())
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
		Label0:
			return null;
		}

		public static string GetNamespace(this TypeReference self)
		{
			if (!self.get_IsNested())
			{
				return self.get_Namespace();
			}
			return self.get_DeclaringType().GetNamespace();
		}

		public static bool IsIntegerType(this TypeReference self)
		{
			if (self == null)
			{
				return false;
			}
			V_0 = self.get_FullName();
			if (!String.op_Equality(V_0, self.get_Module().get_TypeSystem().get_Byte().get_FullName()) && !String.op_Equality(V_0, self.get_Module().get_TypeSystem().get_SByte().get_FullName()) && !String.op_Equality(V_0, self.get_Module().get_TypeSystem().get_Int16().get_FullName()) && !String.op_Equality(V_0, self.get_Module().get_TypeSystem().get_UInt16().get_FullName()) && !String.op_Equality(V_0, self.get_Module().get_TypeSystem().get_Int32().get_FullName()) && !String.op_Equality(V_0, self.get_Module().get_TypeSystem().get_UInt32().get_FullName()) && !String.op_Equality(V_0, self.get_Module().get_TypeSystem().get_Int64().get_FullName()) && !String.op_Equality(V_0, self.get_Module().get_TypeSystem().get_UInt64().get_FullName()))
			{
				return false;
			}
			return true;
		}
	}
}