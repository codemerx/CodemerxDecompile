using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler
{
	public static class AttributesUtilities
	{
		private static void AddMethodCodeType(MethodDefinition method, Mono.Cecil.CustomAttribute attribute)
		{
			if (method.get_IsNative() || method.get_IsOPTIL() || method.get_IsRuntime())
			{
				MethodCodeType methodCodeType = MethodCodeType.IL;
				if (method.get_IsNative())
				{
					methodCodeType |= MethodCodeType.Native;
				}
				if (method.get_IsOPTIL())
				{
					methodCodeType |= MethodCodeType.OPTIL;
				}
				if (method.get_IsRuntime())
				{
					methodCodeType |= MethodCodeType.Runtime;
				}
				attribute.get_Fields().Add(new Mono.Cecil.CustomAttributeNamedArgument("MethodCodeType", AttributesUtilities.GetMethodImplAttributeArgument(method, methodCodeType)));
			}
		}

		private static void AddMethodImplOptions(MethodDefinition method, Mono.Cecil.CustomAttribute attribute)
		{
			if (AttributesUtilities.DoesMethodHaveMethodImplOptions(method))
			{
				MethodImplOptions methodImplOption = 0;
				if (method.get_AggressiveInlining())
				{
					methodImplOption |= MethodImplOptions.AggressiveInlining;
				}
				if (method.get_IsForwardRef())
				{
					methodImplOption |= MethodImplOptions.ForwardRef;
				}
				if (method.get_IsInternalCall())
				{
					methodImplOption |= MethodImplOptions.InternalCall;
				}
				if (method.get_NoInlining())
				{
					methodImplOption |= MethodImplOptions.NoInlining;
				}
				if (method.get_NoOptimization())
				{
					methodImplOption |= MethodImplOptions.NoOptimization;
				}
				if (method.get_IsPreserveSig() && !method.get_HasPInvokeInfo())
				{
					methodImplOption |= MethodImplOptions.PreserveSig;
				}
				if (method.get_IsSynchronized())
				{
					methodImplOption |= MethodImplOptions.Synchronized;
				}
				if (method.get_IsUnmanaged())
				{
					methodImplOption |= MethodImplOptions.Unmanaged;
				}
				attribute.get_ConstructorArguments().Add(AttributesUtilities.GetMethodImplAttributeArgument(method, methodImplOption));
			}
		}

		private static void CreateAndAddBestFitMappingFieldArgument(MethodDefinition method, Mono.Cecil.CustomAttribute dllImportAttr)
		{
			if (method.get_PInvokeInfo().get_IsBestFitDisabled())
			{
				TypeReference flag = method.get_DeclaringType().get_Module().get_TypeSystem().get_Boolean();
				Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument = new Mono.Cecil.CustomAttributeNamedArgument("BestFitMapping", new CustomAttributeArgument(flag, false));
				dllImportAttr.get_Fields().Add(customAttributeNamedArgument);
			}
		}

		private static void CreateAndAddCallingConventionFieldArgument(MethodDefinition method, Mono.Cecil.CustomAttribute dllImportAttr)
		{
			if (!method.get_PInvokeInfo().get_IsCallConvWinapi())
			{
				TypeReference corlibTypeReference = Utilities.GetCorlibTypeReference(typeof(CallingConvention), method.get_DeclaringType().get_Module());
				int num = 0;
				if (method.get_PInvokeInfo().get_IsCallConvFastcall())
				{
					num = 5;
				}
				else if (method.get_PInvokeInfo().get_IsCallConvThiscall())
				{
					num = 4;
				}
				else if (!method.get_PInvokeInfo().get_IsCallConvStdCall())
				{
					num = (!method.get_PInvokeInfo().get_IsCallConvCdecl() ? 1 : 2);
				}
				else
				{
					num = 3;
				}
				CustomAttributeArgument customAttributeArgument = new CustomAttributeArgument(corlibTypeReference, (object)num);
				Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument = new Mono.Cecil.CustomAttributeNamedArgument("CallingConvention", customAttributeArgument);
				dllImportAttr.get_Fields().Add(customAttributeNamedArgument);
			}
		}

		private static void CreateAndAddCharSetFieldArgument(MethodDefinition method, Mono.Cecil.CustomAttribute dllImportAttr)
		{
			CharSet charSet = CharSet.None;
			if (method.get_PInvokeInfo().get_IsCharSetAnsi())
			{
				charSet = CharSet.Ansi;
			}
			if (method.get_PInvokeInfo().get_IsCharSetUnicode())
			{
				charSet = CharSet.Unicode;
			}
			if (method.get_PInvokeInfo().get_IsCharSetAuto())
			{
				charSet = CharSet.Auto;
			}
			TypeReference corlibTypeReference = Utilities.GetCorlibTypeReference(typeof(CharSet), method.get_DeclaringType().get_Module());
			CustomAttributeArgument customAttributeArgument = new CustomAttributeArgument(corlibTypeReference, (object)((Int32)charSet));
			Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument = new Mono.Cecil.CustomAttributeNamedArgument("CharSet", customAttributeArgument);
			dllImportAttr.get_Fields().Add(customAttributeNamedArgument);
		}

		private static void CreateAndAddEntryPointFieldArgument(MethodDefinition method, Mono.Cecil.CustomAttribute dllImportAttr)
		{
			if (method.get_PInvokeInfo().get_EntryPoint() != method.get_Name())
			{
				TypeReference str = method.get_DeclaringType().get_Module().get_TypeSystem().get_String();
				CustomAttributeArgument customAttributeArgument = new CustomAttributeArgument(str, method.get_PInvokeInfo().get_EntryPoint());
				Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument = new Mono.Cecil.CustomAttributeNamedArgument("EntryPoint", customAttributeArgument);
				dllImportAttr.get_Fields().Add(customAttributeNamedArgument);
			}
		}

		private static void CreateAndAddExactSpellingFieldArgument(MethodDefinition method, Mono.Cecil.CustomAttribute dllImportAttr)
		{
			TypeReference flag = method.get_DeclaringType().get_Module().get_TypeSystem().get_Boolean();
			CustomAttributeArgument customAttributeArgument = new CustomAttributeArgument(flag, (object)method.get_PInvokeInfo().get_IsNoMangle());
			Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument = new Mono.Cecil.CustomAttributeNamedArgument("ExactSpelling", customAttributeArgument);
			dllImportAttr.get_Fields().Add(customAttributeNamedArgument);
		}

		private static void CreateAndAddPreserveSigFieldArgument(MethodDefinition method, Mono.Cecil.CustomAttribute dllImportAttr)
		{
			if (!method.get_IsPreserveSig())
			{
				TypeReference flag = method.get_DeclaringType().get_Module().get_TypeSystem().get_Boolean();
				Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument = new Mono.Cecil.CustomAttributeNamedArgument("PreserveSig", new CustomAttributeArgument(flag, false));
				dllImportAttr.get_Fields().Add(customAttributeNamedArgument);
			}
		}

		private static void CreateAndAddSetLastErrorFieldArgument(MethodDefinition method, Mono.Cecil.CustomAttribute dllImportAttr)
		{
			if (method.get_PInvokeInfo().get_SupportsLastError())
			{
				TypeReference flag = method.get_DeclaringType().get_Module().get_TypeSystem().get_Boolean();
				Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument = new Mono.Cecil.CustomAttributeNamedArgument("SetLastError", new CustomAttributeArgument(flag, true));
				dllImportAttr.get_Fields().Add(customAttributeNamedArgument);
			}
		}

		private static void CreateAndAddThrowOnUnmappableCharFieldArgument(MethodDefinition method, Mono.Cecil.CustomAttribute dllImportAttr)
		{
			if (method.get_PInvokeInfo().get_IsThrowOnUnmappableCharEnabled())
			{
				TypeReference flag = method.get_DeclaringType().get_Module().get_TypeSystem().get_Boolean();
				Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument = new Mono.Cecil.CustomAttributeNamedArgument("ThrowOnUnmappableChar", new CustomAttributeArgument(flag, true));
				dllImportAttr.get_Fields().Add(customAttributeNamedArgument);
			}
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
			List<TypeReference> typeReferences = new List<TypeReference>();
			List<ICustomAttribute> customAttributes = new List<ICustomAttribute>()
			{
				AttributesUtilities.GetAssemblyVersionAttribute(assembly)
			};
			foreach (Mono.Cecil.CustomAttribute customAttribute in assembly.get_CustomAttributes())
			{
				customAttribute.Resolve();
				customAttributes.Add(customAttribute);
			}
			if (assembly.get_HasSecurityDeclarations())
			{
				foreach (SecurityDeclaration securityDeclaration in assembly.get_SecurityDeclarations())
				{
					if (!securityDeclaration.get_HasSecurityAttributes())
					{
						continue;
					}
					foreach (SecurityAttribute securityAttribute in securityDeclaration.get_SecurityAttributes())
					{
						customAttributes.Add(securityAttribute);
					}
				}
			}
			if (assembly.get_MainModule().get_HasExportedTypes())
			{
				foreach (ExportedType exportedType in assembly.get_MainModule().get_ExportedTypes())
				{
					if (exportedType.get_Scope() is ModuleReference)
					{
						continue;
					}
					customAttributes.Add(AttributesUtilities.GetExportedTypeAttribute(exportedType, assembly.get_MainModule()));
				}
			}
			foreach (ICustomAttribute customAttribute1 in customAttributes)
			{
				if (!(customAttribute1 is Mono.Cecil.CustomAttribute))
				{
					if (!(customAttribute1 is SecurityAttribute))
					{
						continue;
					}
					foreach (TypeReference securityAttributeUsedType in AttributesUtilities.GetSecurityAttributeUsedTypes(customAttribute1 as SecurityAttribute))
					{
						typeReferences.Add(securityAttributeUsedType);
					}
				}
				else
				{
					foreach (TypeReference customAttributeUsedType in AttributesUtilities.GetCustomAttributeUsedTypes(customAttribute1 as Mono.Cecil.CustomAttribute))
					{
						typeReferences.Add(customAttributeUsedType);
					}
				}
			}
			return typeReferences;
		}

		public static Mono.Cecil.CustomAttribute GetAssemblyVersionAttribute(AssemblyDefinition assembly)
		{
			IMetadataScope scope = assembly.get_MainModule().get_TypeSystem().get_Boolean().get_Scope();
			TypeReference typeReference = new TypeReference("System.Reflection", "AssemblyVersionAttribute", assembly.get_MainModule(), scope);
			MethodReference methodReference = new MethodReference(".ctor", assembly.get_MainModule().get_TypeSystem().get_Void(), typeReference);
			methodReference.get_Parameters().Add(new ParameterDefinition(assembly.get_MainModule().get_TypeSystem().get_String()));
			Mono.Cecil.CustomAttribute customAttribute = new Mono.Cecil.CustomAttribute(methodReference);
			string str = assembly.get_Name().get_Version().ToString(4);
			CustomAttributeArgument customAttributeArgument = new CustomAttributeArgument(assembly.get_MainModule().get_TypeSystem().get_String(), str);
			customAttribute.get_ConstructorArguments().Add(customAttributeArgument);
			return customAttribute;
		}

		private static ICollection<TypeReference> GetAttributeArgumentArrayUsedTypes(CustomAttributeArgument argument)
		{
			List<TypeReference> typeReferences = new List<TypeReference>();
			CustomAttributeArgument[] value = argument.get_Value() as CustomAttributeArgument[];
			typeReferences.Add(argument.get_Type());
			for (int i = 0; i < (int)value.Length; i++)
			{
				typeReferences.AddRange(AttributesUtilities.GetAttributeArgumentValueUsedTypes(value[i]));
			}
			return typeReferences;
		}

		private static ICollection<TypeReference> GetAttributeArgumentValueUsedTypes(CustomAttributeArgument argument)
		{
			if (argument.get_Value() is CustomAttributeArgument)
			{
				return AttributesUtilities.GetAttributeArgumentValueUsedTypes((CustomAttributeArgument)argument.get_Value());
			}
			if (argument.get_Value() is CustomAttributeArgument[])
			{
				return AttributesUtilities.GetAttributeArgumentArrayUsedTypes(argument);
			}
			List<TypeReference> typeReferences = new List<TypeReference>();
			TypeDefinition typeDefinition = (argument.get_Type().get_IsDefinition() ? argument.get_Type() as TypeDefinition : argument.get_Type().Resolve());
			if (typeDefinition != null && typeDefinition.get_IsEnum())
			{
				List<FieldDefinition> enumFieldDefinitionByValue = EnumValueToFieldCombinationMatcher.GetEnumFieldDefinitionByValue(typeDefinition.get_Fields(), argument.get_Value(), typeDefinition.get_CustomAttributes());
				if (enumFieldDefinitionByValue.Count != 0)
				{
					for (int i = 0; i < enumFieldDefinitionByValue.Count; i++)
					{
						typeReferences.AddRange(Utilities.GetTypeReferenceTypesDepedningOn(enumFieldDefinitionByValue[i].get_DeclaringType()));
					}
				}
			}
			else if (argument.get_Type().get_Name() == "Type" && argument.get_Type().get_Namespace() == "System")
			{
				typeReferences.AddRange(Utilities.GetTypeReferenceTypesDepedningOn(argument.get_Value() as TypeReference));
			}
			return typeReferences;
		}

		private static ICollection<TypeReference> GetAttributeNamedArgsUsedTypes(TypeDefinition attributeType, Collection<Mono.Cecil.CustomAttributeNamedArgument> namedArguments, bool fields)
		{
			IList properties;
			IList lists;
			List<TypeReference> typeReferences = new List<TypeReference>();
			for (int i = 0; i < namedArguments.get_Count(); i++)
			{
				if (attributeType != null)
				{
					if (fields)
					{
						properties = attributeType.get_Fields();
					}
					else
					{
						properties = attributeType.get_Properties();
					}
					MemberReference memberReference = null;
					IList lists1 = properties;
					TypeDefinition typeDefinition = attributeType;
					do
					{
						memberReference = Utilities.FindMemberArgumentRefersTo(lists1, namedArguments.get_Item(i));
						if (typeDefinition.get_BaseType() == null)
						{
							break;
						}
						typeDefinition = typeDefinition.get_BaseType().Resolve();
						if (typeDefinition == null)
						{
							break;
						}
						if (fields)
						{
							lists = typeDefinition.get_Fields();
						}
						else
						{
							lists = typeDefinition.get_Properties();
						}
						lists1 = lists;
					}
					while (memberReference == null);
					if (memberReference != null)
					{
						typeReferences.Add(memberReference.get_DeclaringType());
					}
				}
				Mono.Cecil.CustomAttributeNamedArgument item = namedArguments.get_Item(i);
				typeReferences.AddRange(AttributesUtilities.GetAttributeArgumentValueUsedTypes(item.get_Argument()));
			}
			return typeReferences;
		}

		private static ICollection<TypeReference> GetCustomAttributeUsedTypes(Mono.Cecil.CustomAttribute attribute)
		{
			List<TypeReference> typeReferences = new List<TypeReference>();
			attribute.Resolve();
			typeReferences.AddRange(Utilities.GetTypeReferenceTypesDepedningOn(attribute.get_AttributeType()));
			for (int i = 0; i < attribute.get_ConstructorArguments().get_Count(); i++)
			{
				typeReferences.AddRange(AttributesUtilities.GetAttributeArgumentValueUsedTypes(attribute.get_ConstructorArguments().get_Item(i)));
			}
			if (attribute.get_HasConstructorArguments() || attribute.get_HasFields() || attribute.get_HasProperties())
			{
				if (attribute.get_HasProperties())
				{
					TypeDefinition typeDefinition = attribute.get_AttributeType().Resolve();
					typeReferences.AddRange(AttributesUtilities.GetAttributeNamedArgsUsedTypes(typeDefinition, attribute.get_Properties(), false));
				}
				if (attribute.get_HasFields())
				{
					TypeDefinition typeDefinition1 = attribute.get_AttributeType().Resolve();
					typeReferences.AddRange(AttributesUtilities.GetAttributeNamedArgsUsedTypes(typeDefinition1, attribute.get_Fields(), true));
				}
			}
			return typeReferences;
		}

		public static ICollection<TypeReference> GetEventAttributesUsedTypes(EventDefinition @event, ILanguage language)
		{
			List<TypeReference> typeReferences = new List<TypeReference>();
			foreach (Mono.Cecil.CustomAttribute customAttribute in @event.get_CustomAttributes())
			{
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(customAttribute));
			}
			if (@event.get_AddMethod() != null)
			{
				typeReferences.AddRange(AttributesUtilities.GetMethodAttributesUsedTypes(@event.get_AddMethod(), language));
			}
			if (@event.get_RemoveMethod() != null)
			{
				typeReferences.AddRange(AttributesUtilities.GetMethodAttributesUsedTypes(@event.get_RemoveMethod(), language));
			}
			if (@event.get_InvokeMethod() != null)
			{
				typeReferences.AddRange(AttributesUtilities.GetMethodAttributesUsedTypes(@event.get_InvokeMethod(), language));
			}
			return typeReferences;
		}

		public static Mono.Cecil.CustomAttribute GetExportedTypeAttribute(ExportedType exportedType, ModuleDefinition module)
		{
			Mono.Cecil.CustomAttribute customAttribute = new Mono.Cecil.CustomAttribute(Utilities.GetEmptyConstructor(typeof(TypeForwardedToAttribute), module, (IList<Type>)(new Type[] { typeof(Type) })));
			TypeReference corlibTypeReference = Utilities.GetCorlibTypeReference(typeof(Type), module);
			TypeReference typeReference = exportedType.CreateReference();
			customAttribute.get_ConstructorArguments().Add(new CustomAttributeArgument(corlibTypeReference, typeReference));
			return customAttribute;
		}

		public static ICollection<TypeReference> GetFieldAttributesUsedTypes(FieldDefinition field)
		{
			List<TypeReference> typeReferences = new List<TypeReference>();
			foreach (Mono.Cecil.CustomAttribute customAttribute in field.get_CustomAttributes())
			{
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(customAttribute));
			}
			if (field.get_IsNotSerialized())
			{
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(AttributesUtilities.GetFieldNotSerializedAttribute(field)));
			}
			if (field.get_DeclaringType().get_IsExplicitLayout())
			{
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(AttributesUtilities.GetFieldFieldOffsetAttribute(field)));
			}
			return typeReferences;
		}

		public static Mono.Cecil.CustomAttribute GetFieldFieldOffsetAttribute(FieldDefinition field)
		{
			Mono.Cecil.CustomAttribute customAttribute = new Mono.Cecil.CustomAttribute(Utilities.GetEmptyConstructor(typeof(FieldOffsetAttribute), field.get_DeclaringType().get_Module(), (IList<Type>)(new Type[] { typeof(Int32) })));
			TypeReference num = field.get_FieldType().get_Module().get_TypeSystem().get_Int32();
			CustomAttributeArgument customAttributeArgument = new CustomAttributeArgument(num, (object)field.get_Offset());
			customAttribute.get_ConstructorArguments().Add(customAttributeArgument);
			return customAttribute;
		}

		public static Mono.Cecil.CustomAttribute GetFieldNotSerializedAttribute(FieldDefinition field)
		{
			return new Mono.Cecil.CustomAttribute(Utilities.GetEmptyConstructor(typeof(NonSerializedAttribute), field.get_DeclaringType().get_Module(), null));
		}

		public static ICollection<TypeReference> GetMethodAttributesUsedTypes(MethodDefinition method, ILanguage language)
		{
			List<TypeReference> typeReferences = new List<TypeReference>();
			foreach (Mono.Cecil.CustomAttribute customAttribute in method.get_CustomAttributes())
			{
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(customAttribute));
			}
			ModuleDefinition module = method.get_DeclaringType().get_Module();
			if (method.get_HasSecurityDeclarations())
			{
				foreach (SecurityDeclaration securityDeclaration in method.get_SecurityDeclarations())
				{
					if (securityDeclaration.get_HasSecurityAttributes())
					{
						foreach (SecurityAttribute securityAttribute in securityDeclaration.get_SecurityAttributes())
						{
							typeReferences.AddRange(AttributesUtilities.GetSecurityAttributeUsedTypes(securityAttribute));
						}
					}
					typeReferences.Add(securityDeclaration.GetSecurityActionTypeReference(module));
				}
			}
			foreach (ParameterDefinition parameter in method.get_Parameters())
			{
				if (parameter.IsOutParameter() && !language.HasOutKeyword)
				{
					TypeReference outAttributeTypeReference = AttributesUtilities.GetOutAttributeTypeReference(method.get_DeclaringType().get_Module());
					typeReferences.Add(outAttributeTypeReference);
				}
				foreach (Mono.Cecil.CustomAttribute customAttribute1 in parameter.get_CustomAttributes())
				{
					typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(customAttribute1));
				}
			}
			if (method.get_HasPInvokeInfo() && method.get_PInvokeInfo() != null)
			{
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(AttributesUtilities.GetMethodDllImportAttribute(method)));
			}
			if (method.get_HasImplAttributes() && AttributesUtilities.ShouldWriteMethodImplAttribute(method))
			{
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(AttributesUtilities.GetMethodImplAttribute(method)));
			}
			return typeReferences;
		}

		public static Mono.Cecil.CustomAttribute GetMethodDllImportAttribute(MethodDefinition method)
		{
			Mono.Cecil.CustomAttribute customAttribute = new Mono.Cecil.CustomAttribute(Utilities.GetEmptyConstructor(typeof(DllImportAttribute), method.get_DeclaringType().get_Module(), (IList<Type>)(new Type[] { typeof(String) })));
			string name = method.get_PInvokeInfo().get_Module().get_Name();
			TypeReference str = method.get_DeclaringType().get_Module().get_TypeSystem().get_String();
			CustomAttributeArgument customAttributeArgument = new CustomAttributeArgument(str, name);
			customAttribute.get_ConstructorArguments().Add(customAttributeArgument);
			AttributesUtilities.CreateAndAddBestFitMappingFieldArgument(method, customAttribute);
			AttributesUtilities.CreateAndAddCallingConventionFieldArgument(method, customAttribute);
			AttributesUtilities.CreateAndAddCharSetFieldArgument(method, customAttribute);
			AttributesUtilities.CreateAndAddEntryPointFieldArgument(method, customAttribute);
			AttributesUtilities.CreateAndAddExactSpellingFieldArgument(method, customAttribute);
			AttributesUtilities.CreateAndAddPreserveSigFieldArgument(method, customAttribute);
			AttributesUtilities.CreateAndAddSetLastErrorFieldArgument(method, customAttribute);
			AttributesUtilities.CreateAndAddThrowOnUnmappableCharFieldArgument(method, customAttribute);
			return customAttribute;
		}

		public static Mono.Cecil.CustomAttribute GetMethodImplAttribute(MethodDefinition method)
		{
			Type[] typeArray = (AttributesUtilities.DoesMethodHaveMethodImplOptions(method) ? new Type[] { typeof(MethodImplOptions) } : new Type[0]);
			Mono.Cecil.CustomAttribute customAttribute = new Mono.Cecil.CustomAttribute(Utilities.GetEmptyConstructor(typeof(MethodImplAttribute), method.get_DeclaringType().get_Module(), typeArray));
			AttributesUtilities.AddMethodImplOptions(method, customAttribute);
			AttributesUtilities.AddMethodCodeType(method, customAttribute);
			return customAttribute;
		}

		private static CustomAttributeArgument GetMethodImplAttributeArgument(MethodDefinition method, object value)
		{
			ModuleDefinition module = method.get_DeclaringType().get_Module();
			AssemblyNameReference assemblyNameReference = module.ReferencedMscorlibRef();
			Type type = value.GetType();
			return new CustomAttributeArgument(new TypeReference(type.Namespace, type.Name, module, assemblyNameReference), (object)((Int32)value));
		}

		public static ICollection<TypeReference> GetModuleAttributesUsedTypes(ModuleDefinition module)
		{
			List<TypeReference> typeReferences = new List<TypeReference>();
			List<Mono.Cecil.CustomAttribute> customAttributes = new List<Mono.Cecil.CustomAttribute>();
			foreach (Mono.Cecil.CustomAttribute customAttribute in module.get_CustomAttributes())
			{
				customAttribute.Resolve();
				customAttributes.Add(customAttribute);
			}
			foreach (Mono.Cecil.CustomAttribute customAttribute1 in customAttributes)
			{
				if (customAttribute1.get_AttributeType().get_FullName().Equals("System.Security.UnverifiableCodeAttribute", StringComparison.Ordinal))
				{
					continue;
				}
				foreach (TypeReference customAttributeUsedType in AttributesUtilities.GetCustomAttributeUsedTypes(customAttribute1))
				{
					typeReferences.Add(customAttributeUsedType);
				}
			}
			return typeReferences;
		}

		public static TypeReference GetOutAttributeTypeReference(ModuleDefinition module)
		{
			return new TypeReference("System.Runtime.InteropServices", "OutAttribute", module, module.ReferencedMscorlibRef());
		}

		public static ICollection<TypeReference> GetPropertyAttributesUsedTypes(PropertyDefinition property, ILanguage language)
		{
			List<TypeReference> typeReferences = new List<TypeReference>();
			foreach (Mono.Cecil.CustomAttribute customAttribute in property.get_CustomAttributes())
			{
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(customAttribute));
			}
			if (property.get_GetMethod() != null)
			{
				typeReferences.AddRange(AttributesUtilities.GetMethodAttributesUsedTypes(property.get_GetMethod(), language));
			}
			if (property.get_SetMethod() != null)
			{
				typeReferences.AddRange(AttributesUtilities.GetMethodAttributesUsedTypes(property.get_SetMethod(), language));
			}
			return typeReferences;
		}

		private static ICollection<TypeReference> GetSecurityAttributeUsedTypes(SecurityAttribute attribute)
		{
			List<TypeReference> typeReferences = new List<TypeReference>()
			{
				attribute.get_AttributeType()
			};
			if (attribute.get_HasFields() || attribute.get_HasProperties())
			{
				TypeDefinition typeDefinition = attribute.get_AttributeType().Resolve();
				if (attribute.get_HasProperties())
				{
					typeReferences.AddRange(AttributesUtilities.GetAttributeNamedArgsUsedTypes(typeDefinition, attribute.get_Properties(), false));
				}
				if (attribute.get_HasFields())
				{
					typeReferences.AddRange(AttributesUtilities.GetAttributeNamedArgsUsedTypes(typeDefinition, attribute.get_Fields(), true));
				}
			}
			return typeReferences;
		}

		public static ICollection<TypeReference> GetTypeAttributesUsedTypes(TypeDefinition type)
		{
			List<TypeReference> typeReferences = new List<TypeReference>();
			foreach (Mono.Cecil.CustomAttribute customAttribute in type.get_CustomAttributes())
			{
				customAttribute.Resolve();
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(customAttribute));
			}
			if (type.get_IsSerializable())
			{
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(AttributesUtilities.GetTypeSerializableAttribute(type)));
			}
			if (type.get_IsExplicitLayout())
			{
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(AttributesUtilities.GetTypeExplicitLayoutAttribute(type)));
			}
			if (type.get_HasSecurityDeclarations())
			{
				ModuleDefinition module = type.get_Module();
				foreach (SecurityDeclaration securityDeclaration in type.get_SecurityDeclarations())
				{
					if (securityDeclaration.get_HasSecurityAttributes())
					{
						foreach (SecurityAttribute securityAttribute in securityDeclaration.get_SecurityAttributes())
						{
							typeReferences.AddRange(AttributesUtilities.GetSecurityAttributeUsedTypes(securityAttribute));
						}
					}
					typeReferences.Add(securityDeclaration.GetSecurityActionTypeReference(module));
				}
			}
			return typeReferences;
		}

		public static Mono.Cecil.CustomAttribute GetTypeExplicitLayoutAttribute(TypeDefinition type)
		{
			Mono.Cecil.CustomAttribute customAttribute = new Mono.Cecil.CustomAttribute(Utilities.GetEmptyConstructor(typeof(StructLayoutAttribute), type.get_Module(), (IList<Type>)(new Type[] { typeof(LayoutKind) })));
			TypeReference corlibTypeReference = Utilities.GetCorlibTypeReference(typeof(LayoutKind), type.get_Module());
			CustomAttributeArgument customAttributeArgument = new CustomAttributeArgument(corlibTypeReference, (object)2);
			customAttribute.get_ConstructorArguments().Add(customAttributeArgument);
			return customAttribute;
		}

		public static Mono.Cecil.CustomAttribute GetTypeSerializableAttribute(TypeDefinition type)
		{
			return new Mono.Cecil.CustomAttribute(Utilities.GetEmptyConstructor(typeof(SerializableAttribute), type.get_Module(), null));
		}

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