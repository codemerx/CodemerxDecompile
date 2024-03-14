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
			if (self is GenericInstanceType)
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
			else if (self is TypeSpecification)
			{
				return (self as TypeSpecification).get_ElementType().ContainsAnonymousType();
			}
			return false;
		}

		public static Expression GetDefaultValueExpression(this TypeReference typeReference, TypeSystem typeSystem)
		{
			if (typeReference.get_IsPrimitive())
			{
				string fullName = typeReference.get_FullName();
				if (fullName != null)
				{
					if (fullName == "System.Boolean")
					{
						return new LiteralExpression(false, typeSystem, null);
					}
					if (fullName == "System.Char")
					{
						return new LiteralExpression((object)'\0', typeSystem, null);
					}
					if (fullName == "System.IntPtr")
					{
						return new DefaultObjectExpression(typeReference, null);
					}
				}
				return new LiteralExpression(Activator.CreateInstance(Type.GetType(fullName)), typeSystem, null);
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
			TypeDefinition typeDefinition = typeReference.Resolve();
			if (typeDefinition != null && typeDefinition.get_IsEnum())
			{
				return new LiteralExpression((object)0, typeSystem, null);
			}
			return new ObjectCreationExpression(typeReference.GetEmptyConstructorReference(), typeReference, null, null);
		}

		internal static MethodReference GetEmptyConstructorReference(this TypeReference self)
		{
			MethodReference methodReference;
			if (self == null)
			{
				throw new ArgumentNullException("self");
			}
			if (self.get_IsGenericInstance())
			{
				return null;
			}
			TypeDefinition typeDefinition = self.Resolve();
			if (typeDefinition == null)
			{
				return null;
			}
			Collection<MethodDefinition>.Enumerator enumerator = typeDefinition.get_Methods().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					MethodDefinition current = enumerator.get_Current();
					if (!current.get_IsConstructor() || current.get_HasParameters())
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
			string fullName = self.get_FullName();
			if (!(fullName == self.get_Module().get_TypeSystem().get_Byte().get_FullName()) && !(fullName == self.get_Module().get_TypeSystem().get_SByte().get_FullName()) && !(fullName == self.get_Module().get_TypeSystem().get_Int16().get_FullName()) && !(fullName == self.get_Module().get_TypeSystem().get_UInt16().get_FullName()) && !(fullName == self.get_Module().get_TypeSystem().get_Int32().get_FullName()) && !(fullName == self.get_Module().get_TypeSystem().get_UInt32().get_FullName()) && !(fullName == self.get_Module().get_TypeSystem().get_Int64().get_FullName()) && !(fullName == self.get_Module().get_TypeSystem().get_UInt64().get_FullName()))
			{
				return false;
			}
			return true;
		}
	}
}