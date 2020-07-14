using Mono.Cecil;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class FieldReferenceExtensions
	{
		public static bool IsCompilerGenerated(this FieldReference fieldReference, bool isAssemblyResolverChacheEnabled = true)
		{
			FieldDefinition fieldDefinition = fieldReference as FieldDefinition ?? fieldReference.Resolve();
			if (fieldDefinition == null)
			{
				return false;
			}
			return fieldDefinition.HasCompilerGeneratedAttribute();
		}

		public static bool IsCompilerGeneratedDelegate(this FieldReference fieldReference)
		{
			FieldDefinition fieldDefinition;
			TypeReference baseType;
			TypeDefinition typeDefinition;
			fieldDefinition = (!fieldReference.get_IsDefinition() ? fieldReference.Resolve() : (FieldDefinition)fieldReference);
			if (fieldDefinition == null)
			{
				return false;
			}
			bool flag = fieldDefinition.IsCompilerGenerated(true);
			typeDefinition = (!fieldDefinition.get_FieldType().get_IsDefinition() ? fieldDefinition.get_FieldType().Resolve() : (TypeDefinition)fieldDefinition.get_FieldType());
			if (typeDefinition == null)
			{
				baseType = null;
			}
			else
			{
				baseType = typeDefinition.get_BaseType();
			}
			if (!flag || baseType == null)
			{
				return false;
			}
			return baseType.get_FullName() == typeof(MulticastDelegate).FullName;
		}
	}
}