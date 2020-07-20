using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler
{
	public static class AttributesUtilities
	{
		private static void AddMethodCodeType(MethodDefinition method, CustomAttribute attribute)
		{
			if (method.get_IsNative() || method.get_IsOPTIL() || method.get_IsRuntime())
			{
				V_0 = 0;
				if (method.get_IsNative())
				{
					V_0 = V_0 | 1;
				}
				if (method.get_IsOPTIL())
				{
					V_0 = V_0 | 2;
				}
				if (method.get_IsRuntime())
				{
					V_0 = V_0 | 3;
				}
				attribute.get_Fields().Add(new CustomAttributeNamedArgument("MethodCodeType", AttributesUtilities.GetMethodImplAttributeArgument(method, V_0)));
			}
			return;
		}

		private static void AddMethodImplOptions(MethodDefinition method, CustomAttribute attribute)
		{
			if (AttributesUtilities.DoesMethodHaveMethodImplOptions(method))
			{
				V_0 = 0;
				if (method.get_AggressiveInlining())
				{
					V_0 = V_0 | 0x100;
				}
				if (method.get_IsForwardRef())
				{
					V_0 = V_0 | 16;
				}
				if (method.get_IsInternalCall())
				{
					V_0 = V_0 | 0x1000;
				}
				if (method.get_NoInlining())
				{
					V_0 = V_0 | 8;
				}
				if (method.get_NoOptimization())
				{
					V_0 = V_0 | 64;
				}
				if (method.get_IsPreserveSig() && !method.get_HasPInvokeInfo())
				{
					V_0 = V_0 | 128;
				}
				if (method.get_IsSynchronized())
				{
					V_0 = V_0 | 32;
				}
				if (method.get_IsUnmanaged())
				{
					V_0 = V_0 | 4;
				}
				attribute.get_ConstructorArguments().Add(AttributesUtilities.GetMethodImplAttributeArgument(method, V_0));
			}
			return;
		}

		private static void CreateAndAddBestFitMappingFieldArgument(MethodDefinition method, CustomAttribute dllImportAttr)
		{
			if (method.get_PInvokeInfo().get_IsBestFitDisabled())
			{
				V_0 = method.get_DeclaringType().get_Module().get_TypeSystem().get_Boolean();
				V_1 = new CustomAttributeArgument(V_0, false);
				V_2 = new CustomAttributeNamedArgument("BestFitMapping", V_1);
				dllImportAttr.get_Fields().Add(V_2);
			}
			return;
		}

		private static void CreateAndAddCallingConventionFieldArgument(MethodDefinition method, CustomAttribute dllImportAttr)
		{
			if (!method.get_PInvokeInfo().get_IsCallConvWinapi())
			{
				V_0 = Utilities.GetCorlibTypeReference(Type.GetTypeFromHandle(// 
				// Current member / type: System.Void Telerik.JustDecompiler.Decompiler.AttributesUtilities::CreateAndAddCallingConventionFieldArgument(Mono.Cecil.MethodDefinition,Mono.Cecil.CustomAttribute)
				// Exception in: System.Void CreateAndAddCallingConventionFieldArgument(Mono.Cecil.MethodDefinition,Mono.Cecil.CustomAttribute)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		private static void CreateAndAddCharSetFieldArgument(MethodDefinition method, CustomAttribute dllImportAttr)
		{
			V_0 = 1;
			if (method.get_PInvokeInfo().get_IsCharSetAnsi())
			{
				V_0 = 2;
			}
			if (method.get_PInvokeInfo().get_IsCharSetUnicode())
			{
				V_0 = 3;
			}
			if (method.get_PInvokeInfo().get_IsCharSetAuto())
			{
				V_0 = 4;
			}
			V_1 = Utilities.GetCorlibTypeReference(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Telerik.JustDecompiler.Decompiler.AttributesUtilities::CreateAndAddCharSetFieldArgument(Mono.Cecil.MethodDefinition,Mono.Cecil.CustomAttribute)
			// Exception in: System.Void CreateAndAddCharSetFieldArgument(Mono.Cecil.MethodDefinition,Mono.Cecil.CustomAttribute)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private static void CreateAndAddEntryPointFieldArgument(MethodDefinition method, CustomAttribute dllImportAttr)
		{
			if (String.op_Inequality(method.get_PInvokeInfo().get_EntryPoint(), method.get_Name()))
			{
				V_0 = method.get_DeclaringType().get_Module().get_TypeSystem().get_String();
				V_1 = new CustomAttributeArgument(V_0, method.get_PInvokeInfo().get_EntryPoint());
				V_2 = new CustomAttributeNamedArgument("EntryPoint", V_1);
				dllImportAttr.get_Fields().Add(V_2);
			}
			return;
		}

		private static void CreateAndAddExactSpellingFieldArgument(MethodDefinition method, CustomAttribute dllImportAttr)
		{
			V_0 = method.get_DeclaringType().get_Module().get_TypeSystem().get_Boolean();
			V_1 = new CustomAttributeArgument(V_0, (object)method.get_PInvokeInfo().get_IsNoMangle());
			V_2 = new CustomAttributeNamedArgument("ExactSpelling", V_1);
			dllImportAttr.get_Fields().Add(V_2);
			return;
		}

		private static void CreateAndAddPreserveSigFieldArgument(MethodDefinition method, CustomAttribute dllImportAttr)
		{
			if (!method.get_IsPreserveSig())
			{
				V_0 = method.get_DeclaringType().get_Module().get_TypeSystem().get_Boolean();
				V_1 = new CustomAttributeArgument(V_0, false);
				V_2 = new CustomAttributeNamedArgument("PreserveSig", V_1);
				dllImportAttr.get_Fields().Add(V_2);
			}
			return;
		}

		private static void CreateAndAddSetLastErrorFieldArgument(MethodDefinition method, CustomAttribute dllImportAttr)
		{
			if (method.get_PInvokeInfo().get_SupportsLastError())
			{
				V_0 = method.get_DeclaringType().get_Module().get_TypeSystem().get_Boolean();
				V_1 = new CustomAttributeArgument(V_0, true);
				V_2 = new CustomAttributeNamedArgument("SetLastError", V_1);
				dllImportAttr.get_Fields().Add(V_2);
			}
			return;
		}

		private static void CreateAndAddThrowOnUnmappableCharFieldArgument(MethodDefinition method, CustomAttribute dllImportAttr)
		{
			if (method.get_PInvokeInfo().get_IsThrowOnUnmappableCharEnabled())
			{
				V_0 = method.get_DeclaringType().get_Module().get_TypeSystem().get_Boolean();
				V_1 = new CustomAttributeArgument(V_0, true);
				V_2 = new CustomAttributeNamedArgument("ThrowOnUnmappableChar", V_1);
				dllImportAttr.get_Fields().Add(V_2);
			}
			return;
		}

		private static bool DoesMethodHaveMethodImplOptions(MethodDefinition method)
		{
			if (method.get_AggressiveInlining() || method.get_IsForwardRef() || method.get_IsInternalCall() || method.get_NoInlining() || method.get_NoOptimization() || method.get_IsPreserveSig() && !method.get_HasPInvokeInfo() || method.get_IsSynchronized())
			{
				return true;
			}
			return method.get_IsUnmanaged();
		}

		public static ICollection<TypeReference> GetAssemblyAttributesUsedTypes(AssemblyDefinition assembly)
		{
			V_0 = new List<TypeReference>();
			V_1 = new List<ICustomAttribute>();
			V_1.Add(AttributesUtilities.GetAssemblyVersionAttribute(assembly));
			V_3 = assembly.get_CustomAttributes().GetEnumerator();
			try
			{
				while (V_3.MoveNext())
				{
					V_4 = V_3.get_Current();
					V_4.Resolve();
					V_1.Add(V_4);
				}
			}
			finally
			{
				V_3.Dispose();
			}
			if (assembly.get_HasSecurityDeclarations())
			{
				V_5 = assembly.get_SecurityDeclarations().GetEnumerator();
				try
				{
					while (V_5.MoveNext())
					{
						V_6 = V_5.get_Current();
						if (!V_6.get_HasSecurityAttributes())
						{
							continue;
						}
						V_7 = V_6.get_SecurityAttributes().GetEnumerator();
						try
						{
							while (V_7.MoveNext())
							{
								V_8 = V_7.get_Current();
								V_1.Add(V_8);
							}
						}
						finally
						{
							V_7.Dispose();
						}
					}
				}
				finally
				{
					V_5.Dispose();
				}
			}
			if (assembly.get_MainModule().get_HasExportedTypes())
			{
				V_9 = assembly.get_MainModule().get_ExportedTypes().GetEnumerator();
				try
				{
					while (V_9.MoveNext())
					{
						V_10 = V_9.get_Current();
						if (V_10.get_Scope() as ModuleReference != null)
						{
							continue;
						}
						V_1.Add(AttributesUtilities.GetExportedTypeAttribute(V_10, assembly.get_MainModule()));
					}
				}
				finally
				{
					V_9.Dispose();
				}
			}
			V_11 = V_1.GetEnumerator();
			try
			{
				while (V_11.MoveNext())
				{
					V_12 = V_11.get_Current();
					if (V_12 as CustomAttribute == null)
					{
						if (V_12 as SecurityAttribute == null)
						{
							continue;
						}
						V_13 = AttributesUtilities.GetSecurityAttributeUsedTypes(V_12 as SecurityAttribute).GetEnumerator();
						try
						{
							while (V_13.MoveNext())
							{
								V_15 = V_13.get_Current();
								V_0.Add(V_15);
							}
						}
						finally
						{
							if (V_13 != null)
							{
								V_13.Dispose();
							}
						}
					}
					else
					{
						V_13 = AttributesUtilities.GetCustomAttributeUsedTypes(V_12 as CustomAttribute).GetEnumerator();
						try
						{
							while (V_13.MoveNext())
							{
								V_14 = V_13.get_Current();
								V_0.Add(V_14);
							}
						}
						finally
						{
							if (V_13 != null)
							{
								V_13.Dispose();
							}
						}
					}
				}
			}
			finally
			{
				((IDisposable)V_11).Dispose();
			}
			return V_0;
		}

		public static CustomAttribute GetAssemblyVersionAttribute(AssemblyDefinition assembly)
		{
			V_0 = assembly.get_MainModule().get_TypeSystem().get_Boolean().get_Scope();
			V_1 = new TypeReference("System.Reflection", "AssemblyVersionAttribute", assembly.get_MainModule(), V_0);
			stackVariable17 = new MethodReference(".ctor", assembly.get_MainModule().get_TypeSystem().get_Void(), V_1);
			stackVariable17.get_Parameters().Add(new ParameterDefinition(assembly.get_MainModule().get_TypeSystem().get_String()));
			stackVariable24 = new CustomAttribute(stackVariable17);
			V_2 = assembly.get_Name().get_Version().ToString(4);
			V_3 = new CustomAttributeArgument(assembly.get_MainModule().get_TypeSystem().get_String(), V_2);
			stackVariable24.get_ConstructorArguments().Add(V_3);
			return stackVariable24;
		}

		private static ICollection<TypeReference> GetAttributeArgumentArrayUsedTypes(CustomAttributeArgument argument)
		{
			V_0 = new List<TypeReference>();
			V_1 = argument.get_Value() as CustomAttributeArgument[];
			V_0.Add(argument.get_Type());
			V_2 = 0;
			while (V_2 < (int)V_1.Length)
			{
				V_0.AddRange(AttributesUtilities.GetAttributeArgumentValueUsedTypes(V_1[V_2]));
				V_2 = V_2 + 1;
			}
			return V_0;
		}

		private static ICollection<TypeReference> GetAttributeArgumentValueUsedTypes(CustomAttributeArgument argument)
		{
			if (argument.get_Value() as CustomAttributeArgument != null)
			{
				return AttributesUtilities.GetAttributeArgumentValueUsedTypes((CustomAttributeArgument)argument.get_Value());
			}
			if (argument.get_Value() as CustomAttributeArgument[] != null)
			{
				return AttributesUtilities.GetAttributeArgumentArrayUsedTypes(argument);
			}
			V_0 = new List<TypeReference>();
			if (argument.get_Type().get_IsDefinition())
			{
				stackVariable12 = argument.get_Type() as TypeDefinition;
			}
			else
			{
				stackVariable12 = argument.get_Type().Resolve();
			}
			V_1 = stackVariable12;
			if (V_1 == null || !V_1.get_IsEnum())
			{
				if (String.op_Equality(argument.get_Type().get_Name(), "Type") && String.op_Equality(argument.get_Type().get_Namespace(), "System"))
				{
					V_0.AddRange(Utilities.GetTypeReferenceTypesDepedningOn(argument.get_Value() as TypeReference));
				}
			}
			else
			{
				V_2 = EnumValueToFieldCombinationMatcher.GetEnumFieldDefinitionByValue(V_1.get_Fields(), argument.get_Value(), V_1.get_CustomAttributes());
				if (V_2.get_Count() != 0)
				{
					V_3 = 0;
					while (V_3 < V_2.get_Count())
					{
						V_0.AddRange(Utilities.GetTypeReferenceTypesDepedningOn(V_2.get_Item(V_3).get_DeclaringType()));
						V_3 = V_3 + 1;
					}
				}
			}
			return V_0;
		}

		private static ICollection<TypeReference> GetAttributeNamedArgsUsedTypes(TypeDefinition attributeType, Collection<CustomAttributeNamedArgument> namedArguments, bool fields)
		{
			V_0 = new List<TypeReference>();
			V_1 = 0;
			while (V_1 < namedArguments.get_Count())
			{
				if (attributeType != null)
				{
					if (fields)
					{
						stackVariable19 = attributeType.get_Fields();
					}
					else
					{
						stackVariable19 = attributeType.get_Properties();
					}
					V_2 = null;
					V_3 = stackVariable19;
					V_4 = attributeType;
					do
					{
						V_2 = Utilities.FindMemberArgumentRefersTo(V_3, namedArguments.get_Item(V_1));
						if (V_4.get_BaseType() == null)
						{
							break;
						}
						V_4 = V_4.get_BaseType().Resolve();
						if (V_4 == null)
						{
							break;
						}
						if (fields)
						{
							stackVariable40 = V_4.get_Fields();
						}
						else
						{
							stackVariable40 = V_4.get_Properties();
						}
						V_3 = stackVariable40;
					}
					while (V_2 == null);
					if (V_2 != null)
					{
						V_0.Add(V_2.get_DeclaringType());
					}
				}
				V_6 = namedArguments.get_Item(V_1);
				V_0.AddRange(AttributesUtilities.GetAttributeArgumentValueUsedTypes(V_6.get_Argument()));
				V_1 = V_1 + 1;
			}
			return V_0;
		}

		private static ICollection<TypeReference> GetCustomAttributeUsedTypes(CustomAttribute attribute)
		{
			V_0 = new List<TypeReference>();
			attribute.Resolve();
			V_0.AddRange(Utilities.GetTypeReferenceTypesDepedningOn(attribute.get_AttributeType()));
			V_1 = 0;
			while (V_1 < attribute.get_ConstructorArguments().get_Count())
			{
				V_0.AddRange(AttributesUtilities.GetAttributeArgumentValueUsedTypes(attribute.get_ConstructorArguments().get_Item(V_1)));
				V_1 = V_1 + 1;
			}
			if (attribute.get_HasConstructorArguments() || attribute.get_HasFields() || attribute.get_HasProperties())
			{
				if (attribute.get_HasProperties())
				{
					V_2 = attribute.get_AttributeType().Resolve();
					V_0.AddRange(AttributesUtilities.GetAttributeNamedArgsUsedTypes(V_2, attribute.get_Properties(), false));
				}
				if (attribute.get_HasFields())
				{
					V_3 = attribute.get_AttributeType().Resolve();
					V_0.AddRange(AttributesUtilities.GetAttributeNamedArgsUsedTypes(V_3, attribute.get_Fields(), true));
				}
			}
			return V_0;
		}

		public static ICollection<TypeReference> GetEventAttributesUsedTypes(EventDefinition event, ILanguage language)
		{
			V_0 = new List<TypeReference>();
			V_1 = event.get_CustomAttributes().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(V_2));
				}
			}
			finally
			{
				V_1.Dispose();
			}
			if (event.get_AddMethod() != null)
			{
				V_0.AddRange(AttributesUtilities.GetMethodAttributesUsedTypes(event.get_AddMethod(), language));
			}
			if (event.get_RemoveMethod() != null)
			{
				V_0.AddRange(AttributesUtilities.GetMethodAttributesUsedTypes(event.get_RemoveMethod(), language));
			}
			if (event.get_InvokeMethod() != null)
			{
				V_0.AddRange(AttributesUtilities.GetMethodAttributesUsedTypes(event.get_InvokeMethod(), language));
			}
			return V_0;
		}

		public static CustomAttribute GetExportedTypeAttribute(ExportedType exportedType, ModuleDefinition module)
		{
			stackVariable1 = Type.GetTypeFromHandle(// 
			// Current member / type: Mono.Cecil.CustomAttribute Telerik.JustDecompiler.Decompiler.AttributesUtilities::GetExportedTypeAttribute(Mono.Cecil.ExportedType,Mono.Cecil.ModuleDefinition)
			// Exception in: Mono.Cecil.CustomAttribute GetExportedTypeAttribute(Mono.Cecil.ExportedType,Mono.Cecil.ModuleDefinition)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public static ICollection<TypeReference> GetFieldAttributesUsedTypes(FieldDefinition field)
		{
			V_0 = new List<TypeReference>();
			V_1 = field.get_CustomAttributes().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(V_2));
				}
			}
			finally
			{
				V_1.Dispose();
			}
			if (field.get_IsNotSerialized())
			{
				V_0.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(AttributesUtilities.GetFieldNotSerializedAttribute(field)));
			}
			if (field.get_DeclaringType().get_IsExplicitLayout())
			{
				V_0.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(AttributesUtilities.GetFieldFieldOffsetAttribute(field)));
			}
			return V_0;
		}

		public static CustomAttribute GetFieldFieldOffsetAttribute(FieldDefinition field)
		{
			stackVariable1 = Type.GetTypeFromHandle(// 
			// Current member / type: Mono.Cecil.CustomAttribute Telerik.JustDecompiler.Decompiler.AttributesUtilities::GetFieldFieldOffsetAttribute(Mono.Cecil.FieldDefinition)
			// Exception in: Mono.Cecil.CustomAttribute GetFieldFieldOffsetAttribute(Mono.Cecil.FieldDefinition)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public static CustomAttribute GetFieldNotSerializedAttribute(FieldDefinition field)
		{
			return new CustomAttribute(Utilities.GetEmptyConstructor(Type.GetTypeFromHandle(// 
			// Current member / type: Mono.Cecil.CustomAttribute Telerik.JustDecompiler.Decompiler.AttributesUtilities::GetFieldNotSerializedAttribute(Mono.Cecil.FieldDefinition)
			// Exception in: Mono.Cecil.CustomAttribute GetFieldNotSerializedAttribute(Mono.Cecil.FieldDefinition)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public static ICollection<TypeReference> GetMethodAttributesUsedTypes(MethodDefinition method, ILanguage language)
		{
			V_0 = new List<TypeReference>();
			V_2 = method.get_CustomAttributes().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					V_0.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(V_3));
				}
			}
			finally
			{
				V_2.Dispose();
			}
			V_1 = method.get_DeclaringType().get_Module();
			if (method.get_HasSecurityDeclarations())
			{
				V_4 = method.get_SecurityDeclarations().GetEnumerator();
				try
				{
					while (V_4.MoveNext())
					{
						V_5 = V_4.get_Current();
						if (V_5.get_HasSecurityAttributes())
						{
							V_6 = V_5.get_SecurityAttributes().GetEnumerator();
							try
							{
								while (V_6.MoveNext())
								{
									V_7 = V_6.get_Current();
									V_0.AddRange(AttributesUtilities.GetSecurityAttributeUsedTypes(V_7));
								}
							}
							finally
							{
								V_6.Dispose();
							}
						}
						V_0.Add(V_5.GetSecurityActionTypeReference(V_1));
					}
				}
				finally
				{
					V_4.Dispose();
				}
			}
			V_8 = method.get_Parameters().GetEnumerator();
			try
			{
				while (V_8.MoveNext())
				{
					stackVariable22 = V_8.get_Current();
					if (stackVariable22.IsOutParameter() && !language.get_HasOutKeyword())
					{
						V_9 = AttributesUtilities.GetOutAttributeTypeReference(method.get_DeclaringType().get_Module());
						V_0.Add(V_9);
					}
					V_2 = stackVariable22.get_CustomAttributes().GetEnumerator();
					try
					{
						while (V_2.MoveNext())
						{
							V_10 = V_2.get_Current();
							V_0.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(V_10));
						}
					}
					finally
					{
						V_2.Dispose();
					}
				}
			}
			finally
			{
				V_8.Dispose();
			}
			if (method.get_HasPInvokeInfo() && method.get_PInvokeInfo() != null)
			{
				V_0.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(AttributesUtilities.GetMethodDllImportAttribute(method)));
			}
			if (method.get_HasImplAttributes() && AttributesUtilities.ShouldWriteMethodImplAttribute(method))
			{
				V_0.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(AttributesUtilities.GetMethodImplAttribute(method)));
			}
			return V_0;
		}

		public static CustomAttribute GetMethodDllImportAttribute(MethodDefinition method)
		{
			stackVariable1 = Type.GetTypeFromHandle(// 
			// Current member / type: Mono.Cecil.CustomAttribute Telerik.JustDecompiler.Decompiler.AttributesUtilities::GetMethodDllImportAttribute(Mono.Cecil.MethodDefinition)
			// Exception in: Mono.Cecil.CustomAttribute GetMethodDllImportAttribute(Mono.Cecil.MethodDefinition)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public static CustomAttribute GetMethodImplAttribute(MethodDefinition method)
		{
			if (AttributesUtilities.DoesMethodHaveMethodImplOptions(method))
			{
				stackVariable3 = new Type[1];
				stackVariable3[0] = Type.GetTypeFromHandle(// 
				// Current member / type: Mono.Cecil.CustomAttribute Telerik.JustDecompiler.Decompiler.AttributesUtilities::GetMethodImplAttribute(Mono.Cecil.MethodDefinition)
				// Exception in: Mono.Cecil.CustomAttribute GetMethodImplAttribute(Mono.Cecil.MethodDefinition)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		private static CustomAttributeArgument GetMethodImplAttributeArgument(MethodDefinition method, object value)
		{
			V_0 = method.get_DeclaringType().get_Module();
			V_1 = V_0.ReferencedMscorlibRef();
			V_2 = value.GetType();
			return new CustomAttributeArgument(new TypeReference(V_2.get_Namespace(), V_2.get_Name(), V_0, V_1), (object)((Int32)value));
		}

		public static ICollection<TypeReference> GetModuleAttributesUsedTypes(ModuleDefinition module)
		{
			V_0 = new List<TypeReference>();
			V_1 = new List<CustomAttribute>();
			V_2 = module.get_CustomAttributes().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					V_3.Resolve();
					V_1.Add(V_3);
				}
			}
			finally
			{
				V_2.Dispose();
			}
			V_4 = V_1.GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_5 = V_4.get_Current();
					if (V_5.get_AttributeType().get_FullName().Equals("System.Security.UnverifiableCodeAttribute", 4))
					{
						continue;
					}
					V_6 = AttributesUtilities.GetCustomAttributeUsedTypes(V_5).GetEnumerator();
					try
					{
						while (V_6.MoveNext())
						{
							V_7 = V_6.get_Current();
							V_0.Add(V_7);
						}
					}
					finally
					{
						if (V_6 != null)
						{
							V_6.Dispose();
						}
					}
				}
			}
			finally
			{
				((IDisposable)V_4).Dispose();
			}
			return V_0;
		}

		public static TypeReference GetOutAttributeTypeReference(ModuleDefinition module)
		{
			return new TypeReference("System.Runtime.InteropServices", "OutAttribute", module, module.ReferencedMscorlibRef());
		}

		public static ICollection<TypeReference> GetPropertyAttributesUsedTypes(PropertyDefinition property, ILanguage language)
		{
			V_0 = new List<TypeReference>();
			V_1 = property.get_CustomAttributes().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(V_2));
				}
			}
			finally
			{
				V_1.Dispose();
			}
			if (property.get_GetMethod() != null)
			{
				V_0.AddRange(AttributesUtilities.GetMethodAttributesUsedTypes(property.get_GetMethod(), language));
			}
			if (property.get_SetMethod() != null)
			{
				V_0.AddRange(AttributesUtilities.GetMethodAttributesUsedTypes(property.get_SetMethod(), language));
			}
			return V_0;
		}

		private static ICollection<TypeReference> GetSecurityAttributeUsedTypes(SecurityAttribute attribute)
		{
			V_0 = new List<TypeReference>();
			V_0.Add(attribute.get_AttributeType());
			if (attribute.get_HasFields() || attribute.get_HasProperties())
			{
				V_1 = attribute.get_AttributeType().Resolve();
				if (attribute.get_HasProperties())
				{
					V_0.AddRange(AttributesUtilities.GetAttributeNamedArgsUsedTypes(V_1, attribute.get_Properties(), false));
				}
				if (attribute.get_HasFields())
				{
					V_0.AddRange(AttributesUtilities.GetAttributeNamedArgsUsedTypes(V_1, attribute.get_Fields(), true));
				}
			}
			return V_0;
		}

		public static ICollection<TypeReference> GetTypeAttributesUsedTypes(TypeDefinition type)
		{
			V_0 = new List<TypeReference>();
			V_1 = type.get_CustomAttributes().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_2.Resolve();
					V_0.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(V_2));
				}
			}
			finally
			{
				V_1.Dispose();
			}
			if (type.get_IsSerializable())
			{
				V_0.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(AttributesUtilities.GetTypeSerializableAttribute(type)));
			}
			if (type.get_IsExplicitLayout())
			{
				V_0.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(AttributesUtilities.GetTypeExplicitLayoutAttribute(type)));
			}
			if (type.get_HasSecurityDeclarations())
			{
				V_3 = type.get_Module();
				V_4 = type.get_SecurityDeclarations().GetEnumerator();
				try
				{
					while (V_4.MoveNext())
					{
						V_5 = V_4.get_Current();
						if (V_5.get_HasSecurityAttributes())
						{
							V_6 = V_5.get_SecurityAttributes().GetEnumerator();
							try
							{
								while (V_6.MoveNext())
								{
									V_7 = V_6.get_Current();
									V_0.AddRange(AttributesUtilities.GetSecurityAttributeUsedTypes(V_7));
								}
							}
							finally
							{
								V_6.Dispose();
							}
						}
						V_0.Add(V_5.GetSecurityActionTypeReference(V_3));
					}
				}
				finally
				{
					V_4.Dispose();
				}
			}
			return V_0;
		}

		public static CustomAttribute GetTypeExplicitLayoutAttribute(TypeDefinition type)
		{
			stackVariable1 = Type.GetTypeFromHandle(// 
			// Current member / type: Mono.Cecil.CustomAttribute Telerik.JustDecompiler.Decompiler.AttributesUtilities::GetTypeExplicitLayoutAttribute(Mono.Cecil.TypeDefinition)
			// Exception in: Mono.Cecil.CustomAttribute GetTypeExplicitLayoutAttribute(Mono.Cecil.TypeDefinition)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public static CustomAttribute GetTypeSerializableAttribute(TypeDefinition type)
		{
			return new CustomAttribute(Utilities.GetEmptyConstructor(Type.GetTypeFromHandle(// 
			// Current member / type: Mono.Cecil.CustomAttribute Telerik.JustDecompiler.Decompiler.AttributesUtilities::GetTypeSerializableAttribute(Mono.Cecil.TypeDefinition)
			// Exception in: Mono.Cecil.CustomAttribute GetTypeSerializableAttribute(Mono.Cecil.TypeDefinition)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		internal static bool ShouldWriteMethodImplAttribute(MethodDefinition method)
		{
			if (method.get_DeclaringType().IsDelegate())
			{
				return false;
			}
			if (method.get_HasPInvokeInfo() && method.get_ImplAttributes() == 128)
			{
				return false;
			}
			return true;
		}
	}
}