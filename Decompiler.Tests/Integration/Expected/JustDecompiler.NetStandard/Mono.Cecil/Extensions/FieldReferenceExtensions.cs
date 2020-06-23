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
			fieldDefinition = (!fieldReference.IsDefinition ? fieldReference.Resolve() : (FieldDefinition)fieldReference);
			if (fieldDefinition == null)
			{
				return false;
			}
			bool flag = fieldDefinition.IsCompilerGenerated(true);
			typeDefinition = (!fieldDefinition.FieldType.IsDefinition ? fieldDefinition.FieldType.Resolve() : (TypeDefinition)fieldDefinition.FieldType);
			if (typeDefinition == null)
			{
				baseType = null;
			}
			else
			{
				baseType = typeDefinition.BaseType;
			}
			if (!flag || baseType == null)
			{
				return false;
			}
			return baseType.FullName == typeof(MulticastDelegate).FullName;
		}
	}
}