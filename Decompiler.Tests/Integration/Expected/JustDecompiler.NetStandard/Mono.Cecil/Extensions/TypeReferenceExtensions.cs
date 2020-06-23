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
				return (self as TypeSpecification).ElementType.ContainsAnonymousType();
			}
			return false;
		}

		public static Expression GetDefaultValueExpression(this TypeReference typeReference, TypeSystem typeSystem)
		{
			if (typeReference.IsPrimitive)
			{
				string fullName = typeReference.FullName;
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
			if (typeReference.IsGenericParameter)
			{
				return new DefaultObjectExpression(typeReference, null);
			}
			if (typeReference.IsArray)
			{
				return new LiteralExpression(null, typeSystem, null);
			}
			if (!typeReference.IsValueType)
			{
				if (!typeReference.IsRequiredModifier)
				{
					return new LiteralExpression(null, typeSystem, null);
				}
				return (typeReference as RequiredModifierType).ElementType.GetDefaultValueExpression(typeSystem);
			}
			TypeDefinition typeDefinition = typeReference.Resolve();
			if (typeDefinition != null && typeDefinition.IsEnum)
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
			if (self.IsGenericInstance)
			{
				return null;
			}
			TypeDefinition typeDefinition = self.Resolve();
			if (typeDefinition == null)
			{
				return null;
			}
			Collection<MethodDefinition>.Enumerator enumerator = typeDefinition.Methods.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					MethodDefinition current = enumerator.Current;
					if (!current.IsConstructor || current.HasParameters)
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

		public static string GetNamespace(this TypeReference self)
		{
			if (!self.IsNested)
			{
				return self.Namespace;
			}
			return self.DeclaringType.GetNamespace();
		}

		public static bool IsIntegerType(this TypeReference self)
		{
			if (self == null)
			{
				return false;
			}
			string fullName = self.FullName;
			if (!(fullName == self.Module.TypeSystem.Byte.FullName) && !(fullName == self.Module.TypeSystem.SByte.FullName) && !(fullName == self.Module.TypeSystem.Int16.FullName) && !(fullName == self.Module.TypeSystem.UInt16.FullName) && !(fullName == self.Module.TypeSystem.Int32.FullName) && !(fullName == self.Module.TypeSystem.UInt32.FullName) && !(fullName == self.Module.TypeSystem.Int64.FullName) && !(fullName == self.Module.TypeSystem.UInt64.FullName))
			{
				return false;
			}
			return true;
		}
	}
}