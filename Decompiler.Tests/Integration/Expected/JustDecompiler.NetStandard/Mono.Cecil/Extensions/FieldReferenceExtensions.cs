using Mono.Cecil;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class FieldReferenceExtensions
	{
		public static bool IsCompilerGenerated(this FieldReference fieldReference, bool isAssemblyResolverChacheEnabled = true)
		{
			V_0 = fieldReference as FieldDefinition;
			if (V_0 == null)
			{
				V_0 = fieldReference.Resolve();
			}
			if (V_0 == null)
			{
				return false;
			}
			return V_0.HasCompilerGeneratedAttribute();
		}

		public static bool IsCompilerGeneratedDelegate(this FieldReference fieldReference)
		{
			if (!fieldReference.get_IsDefinition())
			{
				V_0 = fieldReference.Resolve();
			}
			else
			{
				V_0 = (FieldDefinition)fieldReference;
			}
			if (V_0 == null)
			{
				return false;
			}
			V_1 = V_0.IsCompilerGenerated(true);
			if (!V_0.get_FieldType().get_IsDefinition())
			{
				V_3 = V_0.get_FieldType().Resolve();
			}
			else
			{
				V_3 = (TypeDefinition)V_0.get_FieldType();
			}
			if (V_3 == null)
			{
				V_2 = null;
			}
			else
			{
				V_2 = V_3.get_BaseType();
			}
			if (!V_1 || V_2 == null)
			{
				return false;
			}
			return String.op_Equality(V_2.get_FullName(), Type.GetTypeFromHandle(// 
			// Current member / type: System.Boolean Mono.Cecil.Extensions.FieldReferenceExtensions::IsCompilerGeneratedDelegate(Mono.Cecil.FieldReference)
			// Exception in: System.Boolean IsCompilerGeneratedDelegate(Mono.Cecil.FieldReference)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

	}
}