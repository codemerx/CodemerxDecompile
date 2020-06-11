using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Decompiler;

namespace Mono.Cecil.Extensions
{
    public static class FieldReferenceExtensions
    {
        public static bool IsCompilerGeneratedDelegate(this FieldReference fieldReference)
        {
            FieldDefinition fieldDefinition;

            if (fieldReference.IsDefinition)
            {
                fieldDefinition = (FieldDefinition)fieldReference;
            }
            else
            {
                fieldDefinition = fieldReference.Resolve();
            }
            if (fieldDefinition == null)
            {
                return false;
            }
            bool compilerGenerated = IsCompilerGenerated(fieldDefinition);

            TypeReference baseType;
            TypeDefinition fieldTypeDefinition;

            if (fieldDefinition.FieldType.IsDefinition)
            {
                fieldTypeDefinition = (TypeDefinition)fieldDefinition.FieldType;
            }
            else
            {
                fieldTypeDefinition = fieldDefinition.FieldType.Resolve();
            }
            if (fieldTypeDefinition != null)
            {
                baseType = fieldTypeDefinition.BaseType;
            }
            else
            {
                baseType = null;
            }
            return compilerGenerated 
                   && baseType != null
                   && baseType.FullName == typeof(MulticastDelegate).FullName;
        }

        public static bool IsCompilerGenerated(this FieldReference fieldReference, bool isAssemblyResolverChacheEnabled = true)
        {
            FieldDefinition fieldDefinition = fieldReference as FieldDefinition;
            if (fieldDefinition == null)
            {
                fieldDefinition = fieldReference.Resolve();
            }
            if (fieldDefinition == null)
            {
                return false;
            }
            //return IsCompilerGenerated(fieldDefinition);
            return fieldDefinition.HasCompilerGeneratedAttribute();
        }
    }
}