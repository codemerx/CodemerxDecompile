using System;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Mono.Cecil.Extensions
{
    public static class TypeReferenceExtensions
    {
        public static Expression GetDefaultValueExpression(this TypeReference typeReference, TypeSystem typeSystem)
        {
            if (typeReference.IsPrimitive)
            {
                string typeName = typeReference.FullName;

                switch (typeName)
                {
                    case "System.Boolean":
                        {
                            return new LiteralExpression(false, typeSystem, null);
                        }
                    case "System.Char":
                        {
                            return new LiteralExpression((char)0, typeSystem, null);
                        }
                    case "System.IntPtr":
                        {
                            return new DefaultObjectExpression(typeReference, null);
                        }
                    default:
                        {
                            return new LiteralExpression(Activator.CreateInstance(Type.GetType(typeName)), typeSystem, null);
                        }
                }
            }
            if (typeReference.IsGenericParameter)
            {
                return new DefaultObjectExpression(typeReference, null);
            }
            if (typeReference.IsArray)
            {
                //return GetLiteralExpression(typeReference.GetElementType(), typeSystem);
                return new LiteralExpression(null, typeSystem, null);
            }
            if (typeReference.IsValueType)
            {
                var typeDefinition = typeReference.Resolve();
                if (typeDefinition != null && typeDefinition.IsEnum)
                {
                    return new LiteralExpression(0, typeSystem, null);
                }
                else
                {
                    return new ObjectCreationExpression(typeReference.GetEmptyConstructorReference(), typeReference, null, null);
                }
            }
            if (typeReference.IsRequiredModifier)
            {
                RequiredModifierType typeReferenceAsReqMod = typeReference as RequiredModifierType;
                return typeReferenceAsReqMod.ElementType.GetDefaultValueExpression(typeSystem);
            }

            return new LiteralExpression(null, typeSystem, null);
        }


        internal static MethodReference GetEmptyConstructorReference(this TypeReference self)
        {
            if (self == null)
            {
                throw new ArgumentNullException("self");
            }

            if(self.IsGenericInstance)
            {
                return null;
            }

            TypeDefinition typeDef = self.Resolve();
            if (typeDef == null)
            {
                return null;
            }

            foreach (MethodDefinition method in typeDef.Methods)
            {
                if (method.IsConstructor && !method.HasParameters)
                {
                    return method;
                }
            }

            return null;
        }

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

        public static string GetNamespace(this TypeReference self)
        {
            if (self.IsNested)
            {
                return GetNamespace(self.DeclaringType);
            }
            return self.Namespace;
        }

		public static bool IsIntegerType(this TypeReference self)
		{
			if (self == null)
			{
				return false;
			}
			string typeName = self.FullName;
			if (typeName == self.Module.TypeSystem.Byte.FullName ||
				typeName == self.Module.TypeSystem.SByte.FullName ||
				typeName == self.Module.TypeSystem.Int16.FullName ||
				typeName == self.Module.TypeSystem.UInt16.FullName ||
				typeName == self.Module.TypeSystem.Int32.FullName ||
				typeName == self.Module.TypeSystem.UInt32.FullName ||
				typeName == self.Module.TypeSystem.Int64.FullName ||
				typeName == self.Module.TypeSystem.UInt64.FullName)
			{
				return true;
			}
			return false;
		}
    }
}