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
			if (method.IsNative || method.IsOPTIL || method.IsRuntime)
			{
				MethodCodeType methodCodeType = MethodCodeType.IL;
				if (method.IsNative)
				{
					methodCodeType |= MethodCodeType.Native;
				}
				if (method.IsOPTIL)
				{
					methodCodeType |= MethodCodeType.OPTIL;
				}
				if (method.IsRuntime)
				{
					methodCodeType |= MethodCodeType.Runtime;
				}
				attribute.Fields.Add(new Mono.Cecil.CustomAttributeNamedArgument("MethodCodeType", AttributesUtilities.GetMethodImplAttributeArgument(method, methodCodeType)));
			}
		}

		private static void AddMethodImplOptions(MethodDefinition method, Mono.Cecil.CustomAttribute attribute)
		{
			if (AttributesUtilities.DoesMethodHaveMethodImplOptions(method))
			{
				MethodImplOptions methodImplOption = 0;
				if (method.AggressiveInlining)
				{
					methodImplOption |= MethodImplOptions.AggressiveInlining;
				}
				if (method.IsForwardRef)
				{
					methodImplOption |= MethodImplOptions.ForwardRef;
				}
				if (method.IsInternalCall)
				{
					methodImplOption |= MethodImplOptions.InternalCall;
				}
				if (method.NoInlining)
				{
					methodImplOption |= MethodImplOptions.NoInlining;
				}
				if (method.NoOptimization)
				{
					methodImplOption |= MethodImplOptions.NoOptimization;
				}
				if (method.IsPreserveSig && !method.HasPInvokeInfo)
				{
					methodImplOption |= MethodImplOptions.PreserveSig;
				}
				if (method.IsSynchronized)
				{
					methodImplOption |= MethodImplOptions.Synchronized;
				}
				if (method.IsUnmanaged)
				{
					methodImplOption |= MethodImplOptions.Unmanaged;
				}
				attribute.ConstructorArguments.Add(AttributesUtilities.GetMethodImplAttributeArgument(method, methodImplOption));
			}
		}

		private static void CreateAndAddBestFitMappingFieldArgument(MethodDefinition method, Mono.Cecil.CustomAttribute dllImportAttr)
		{
			if (method.PInvokeInfo.IsBestFitDisabled)
			{
				TypeReference flag = method.DeclaringType.Module.TypeSystem.Boolean;
				Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument = new Mono.Cecil.CustomAttributeNamedArgument("BestFitMapping", new CustomAttributeArgument(flag, false));
				dllImportAttr.Fields.Add(customAttributeNamedArgument);
			}
		}

		private static void CreateAndAddCallingConventionFieldArgument(MethodDefinition method, Mono.Cecil.CustomAttribute dllImportAttr)
		{
			if (!method.PInvokeInfo.IsCallConvWinapi)
			{
				TypeReference corlibTypeReference = Utilities.GetCorlibTypeReference(typeof(CallingConvention), method.DeclaringType.Module);
				int num = 0;
				if (method.PInvokeInfo.IsCallConvFastcall)
				{
					num = 5;
				}
				else if (method.PInvokeInfo.IsCallConvThiscall)
				{
					num = 4;
				}
				else if (!method.PInvokeInfo.IsCallConvStdCall)
				{
					num = (!method.PInvokeInfo.IsCallConvCdecl ? 1 : 2);
				}
				else
				{
					num = 3;
				}
				CustomAttributeArgument customAttributeArgument = new CustomAttributeArgument(corlibTypeReference, (object)num);
				Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument = new Mono.Cecil.CustomAttributeNamedArgument("CallingConvention", customAttributeArgument);
				dllImportAttr.Fields.Add(customAttributeNamedArgument);
			}
		}

		private static void CreateAndAddCharSetFieldArgument(MethodDefinition method, Mono.Cecil.CustomAttribute dllImportAttr)
		{
			CharSet charSet = CharSet.None;
			if (method.PInvokeInfo.IsCharSetAnsi)
			{
				charSet = CharSet.Ansi;
			}
			if (method.PInvokeInfo.IsCharSetUnicode)
			{
				charSet = CharSet.Unicode;
			}
			if (method.PInvokeInfo.IsCharSetAuto)
			{
				charSet = CharSet.Auto;
			}
			TypeReference corlibTypeReference = Utilities.GetCorlibTypeReference(typeof(CharSet), method.DeclaringType.Module);
			CustomAttributeArgument customAttributeArgument = new CustomAttributeArgument(corlibTypeReference, (object)((Int32)charSet));
			Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument = new Mono.Cecil.CustomAttributeNamedArgument("CharSet", customAttributeArgument);
			dllImportAttr.Fields.Add(customAttributeNamedArgument);
		}

		private static void CreateAndAddEntryPointFieldArgument(MethodDefinition method, Mono.Cecil.CustomAttribute dllImportAttr)
		{
			if (method.PInvokeInfo.EntryPoint != method.Name)
			{
				TypeReference str = method.DeclaringType.Module.TypeSystem.String;
				CustomAttributeArgument customAttributeArgument = new CustomAttributeArgument(str, method.PInvokeInfo.EntryPoint);
				Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument = new Mono.Cecil.CustomAttributeNamedArgument("EntryPoint", customAttributeArgument);
				dllImportAttr.Fields.Add(customAttributeNamedArgument);
			}
		}

		private static void CreateAndAddExactSpellingFieldArgument(MethodDefinition method, Mono.Cecil.CustomAttribute dllImportAttr)
		{
			TypeReference flag = method.DeclaringType.Module.TypeSystem.Boolean;
			CustomAttributeArgument customAttributeArgument = new CustomAttributeArgument(flag, (object)method.PInvokeInfo.IsNoMangle);
			Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument = new Mono.Cecil.CustomAttributeNamedArgument("ExactSpelling", customAttributeArgument);
			dllImportAttr.Fields.Add(customAttributeNamedArgument);
		}

		private static void CreateAndAddPreserveSigFieldArgument(MethodDefinition method, Mono.Cecil.CustomAttribute dllImportAttr)
		{
			if (!method.IsPreserveSig)
			{
				TypeReference flag = method.DeclaringType.Module.TypeSystem.Boolean;
				Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument = new Mono.Cecil.CustomAttributeNamedArgument("PreserveSig", new CustomAttributeArgument(flag, false));
				dllImportAttr.Fields.Add(customAttributeNamedArgument);
			}
		}

		private static void CreateAndAddSetLastErrorFieldArgument(MethodDefinition method, Mono.Cecil.CustomAttribute dllImportAttr)
		{
			if (method.PInvokeInfo.SupportsLastError)
			{
				TypeReference flag = method.DeclaringType.Module.TypeSystem.Boolean;
				Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument = new Mono.Cecil.CustomAttributeNamedArgument("SetLastError", new CustomAttributeArgument(flag, true));
				dllImportAttr.Fields.Add(customAttributeNamedArgument);
			}
		}

		private static void CreateAndAddThrowOnUnmappableCharFieldArgument(MethodDefinition method, Mono.Cecil.CustomAttribute dllImportAttr)
		{
			if (method.PInvokeInfo.IsThrowOnUnmappableCharEnabled)
			{
				TypeReference flag = method.DeclaringType.Module.TypeSystem.Boolean;
				Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument = new Mono.Cecil.CustomAttributeNamedArgument("ThrowOnUnmappableChar", new CustomAttributeArgument(flag, true));
				dllImportAttr.Fields.Add(customAttributeNamedArgument);
			}
		}

		private static bool DoesMethodHaveMethodImplOptions(MethodDefinition method)
		{
			if (method.AggressiveInlining || method.IsForwardRef || method.IsInternalCall || method.NoInlining || method.NoOptimization || method.IsPreserveSig && !method.HasPInvokeInfo || method.IsSynchronized)
			{
				return true;
			}
			return method.IsUnmanaged;
		}

		public static ICollection<TypeReference> GetAssemblyAttributesUsedTypes(AssemblyDefinition assembly)
		{
			List<TypeReference> typeReferences = new List<TypeReference>();
			List<ICustomAttribute> customAttributes = new List<ICustomAttribute>()
			{
				AttributesUtilities.GetAssemblyVersionAttribute(assembly)
			};
			foreach (Mono.Cecil.CustomAttribute customAttribute in assembly.CustomAttributes)
			{
				customAttribute.Resolve();
				customAttributes.Add(customAttribute);
			}
			if (assembly.HasSecurityDeclarations)
			{
				foreach (SecurityDeclaration securityDeclaration in assembly.SecurityDeclarations)
				{
					if (!securityDeclaration.HasSecurityAttributes)
					{
						continue;
					}
					foreach (SecurityAttribute securityAttribute in securityDeclaration.SecurityAttributes)
					{
						customAttributes.Add(securityAttribute);
					}
				}
			}
			if (assembly.MainModule.HasExportedTypes)
			{
				foreach (ExportedType exportedType in assembly.MainModule.ExportedTypes)
				{
					if (exportedType.Scope is ModuleReference)
					{
						continue;
					}
					customAttributes.Add(AttributesUtilities.GetExportedTypeAttribute(exportedType, assembly.MainModule));
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
			IMetadataScope scope = assembly.MainModule.TypeSystem.Boolean.Scope;
			TypeReference typeReference = new TypeReference("System.Reflection", "AssemblyVersionAttribute", assembly.MainModule, scope);
			MethodReference methodReference = new MethodReference(".ctor", assembly.MainModule.TypeSystem.Void, typeReference);
			methodReference.Parameters.Add(new ParameterDefinition(assembly.MainModule.TypeSystem.String));
			Mono.Cecil.CustomAttribute customAttribute = new Mono.Cecil.CustomAttribute(methodReference);
			string str = assembly.Name.Version.ToString(4);
			CustomAttributeArgument customAttributeArgument = new CustomAttributeArgument(assembly.MainModule.TypeSystem.String, str);
			customAttribute.ConstructorArguments.Add(customAttributeArgument);
			return customAttribute;
		}

		private static ICollection<TypeReference> GetAttributeArgumentArrayUsedTypes(CustomAttributeArgument argument)
		{
			List<TypeReference> typeReferences = new List<TypeReference>();
			CustomAttributeArgument[] value = argument.Value as CustomAttributeArgument[];
			typeReferences.Add(argument.Type);
			for (int i = 0; i < (int)value.Length; i++)
			{
				typeReferences.AddRange(AttributesUtilities.GetAttributeArgumentValueUsedTypes(value[i]));
			}
			return typeReferences;
		}

		private static ICollection<TypeReference> GetAttributeArgumentValueUsedTypes(CustomAttributeArgument argument)
		{
			if (argument.Value is CustomAttributeArgument)
			{
				return AttributesUtilities.GetAttributeArgumentValueUsedTypes((CustomAttributeArgument)argument.Value);
			}
			if (argument.Value is CustomAttributeArgument[])
			{
				return AttributesUtilities.GetAttributeArgumentArrayUsedTypes(argument);
			}
			List<TypeReference> typeReferences = new List<TypeReference>();
			TypeDefinition typeDefinition = (argument.Type.IsDefinition ? argument.Type as TypeDefinition : argument.Type.Resolve());
			if (typeDefinition != null && typeDefinition.IsEnum)
			{
				List<FieldDefinition> enumFieldDefinitionByValue = EnumValueToFieldCombinationMatcher.GetEnumFieldDefinitionByValue(typeDefinition.Fields, argument.Value, typeDefinition.CustomAttributes);
				if (enumFieldDefinitionByValue.Count != 0)
				{
					for (int i = 0; i < enumFieldDefinitionByValue.Count; i++)
					{
						typeReferences.AddRange(Utilities.GetTypeReferenceTypesDepedningOn(enumFieldDefinitionByValue[i].DeclaringType));
					}
				}
			}
			else if (argument.Type.Name == "Type" && argument.Type.Namespace == "System")
			{
				typeReferences.AddRange(Utilities.GetTypeReferenceTypesDepedningOn(argument.Value as TypeReference));
			}
			return typeReferences;
		}

		private static ICollection<TypeReference> GetAttributeNamedArgsUsedTypes(TypeDefinition attributeType, Collection<Mono.Cecil.CustomAttributeNamedArgument> namedArguments, bool fields)
		{
			IList properties;
			IList lists;
			List<TypeReference> typeReferences = new List<TypeReference>();
			for (int i = 0; i < namedArguments.Count; i++)
			{
				if (attributeType != null)
				{
					if (fields)
					{
						properties = attributeType.Fields;
					}
					else
					{
						properties = attributeType.Properties;
					}
					MemberReference memberReference = null;
					IList lists1 = properties;
					TypeDefinition typeDefinition = attributeType;
					do
					{
						memberReference = Utilities.FindMemberArgumentRefersTo(lists1, namedArguments[i]);
						if (typeDefinition.BaseType == null)
						{
							break;
						}
						typeDefinition = typeDefinition.BaseType.Resolve();
						if (typeDefinition == null)
						{
							break;
						}
						if (fields)
						{
							lists = typeDefinition.Fields;
						}
						else
						{
							lists = typeDefinition.Properties;
						}
						lists1 = lists;
					}
					while (memberReference == null);
					if (memberReference != null)
					{
						typeReferences.Add(memberReference.DeclaringType);
					}
				}
				Mono.Cecil.CustomAttributeNamedArgument item = namedArguments[i];
				typeReferences.AddRange(AttributesUtilities.GetAttributeArgumentValueUsedTypes(item.Argument));
			}
			return typeReferences;
		}

		private static ICollection<TypeReference> GetCustomAttributeUsedTypes(Mono.Cecil.CustomAttribute attribute)
		{
			List<TypeReference> typeReferences = new List<TypeReference>();
			attribute.Resolve();
			typeReferences.AddRange(Utilities.GetTypeReferenceTypesDepedningOn(attribute.AttributeType));
			for (int i = 0; i < attribute.ConstructorArguments.Count; i++)
			{
				typeReferences.AddRange(AttributesUtilities.GetAttributeArgumentValueUsedTypes(attribute.ConstructorArguments[i]));
			}
			if (attribute.HasConstructorArguments || attribute.HasFields || attribute.HasProperties)
			{
				if (attribute.HasProperties)
				{
					TypeDefinition typeDefinition = attribute.AttributeType.Resolve();
					typeReferences.AddRange(AttributesUtilities.GetAttributeNamedArgsUsedTypes(typeDefinition, attribute.Properties, false));
				}
				if (attribute.HasFields)
				{
					TypeDefinition typeDefinition1 = attribute.AttributeType.Resolve();
					typeReferences.AddRange(AttributesUtilities.GetAttributeNamedArgsUsedTypes(typeDefinition1, attribute.Fields, true));
				}
			}
			return typeReferences;
		}

		public static ICollection<TypeReference> GetEventAttributesUsedTypes(EventDefinition @event, ILanguage language)
		{
			List<TypeReference> typeReferences = new List<TypeReference>();
			foreach (Mono.Cecil.CustomAttribute customAttribute in @event.CustomAttributes)
			{
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(customAttribute));
			}
			if (@event.AddMethod != null)
			{
				typeReferences.AddRange(AttributesUtilities.GetMethodAttributesUsedTypes(@event.AddMethod, language));
			}
			if (@event.RemoveMethod != null)
			{
				typeReferences.AddRange(AttributesUtilities.GetMethodAttributesUsedTypes(@event.RemoveMethod, language));
			}
			if (@event.InvokeMethod != null)
			{
				typeReferences.AddRange(AttributesUtilities.GetMethodAttributesUsedTypes(@event.InvokeMethod, language));
			}
			return typeReferences;
		}

		public static Mono.Cecil.CustomAttribute GetExportedTypeAttribute(ExportedType exportedType, ModuleDefinition module)
		{
			Mono.Cecil.CustomAttribute customAttribute = new Mono.Cecil.CustomAttribute(Utilities.GetEmptyConstructor(typeof(TypeForwardedToAttribute), module, (IList<Type>)(new Type[] { typeof(Type) })));
			TypeReference corlibTypeReference = Utilities.GetCorlibTypeReference(typeof(Type), module);
			TypeReference typeReference = exportedType.CreateReference();
			customAttribute.ConstructorArguments.Add(new CustomAttributeArgument(corlibTypeReference, typeReference));
			return customAttribute;
		}

		public static ICollection<TypeReference> GetFieldAttributesUsedTypes(FieldDefinition field)
		{
			List<TypeReference> typeReferences = new List<TypeReference>();
			foreach (Mono.Cecil.CustomAttribute customAttribute in field.CustomAttributes)
			{
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(customAttribute));
			}
			if (field.IsNotSerialized)
			{
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(AttributesUtilities.GetFieldNotSerializedAttribute(field)));
			}
			if (field.DeclaringType.IsExplicitLayout)
			{
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(AttributesUtilities.GetFieldFieldOffsetAttribute(field)));
			}
			return typeReferences;
		}

		public static Mono.Cecil.CustomAttribute GetFieldFieldOffsetAttribute(FieldDefinition field)
		{
			Mono.Cecil.CustomAttribute customAttribute = new Mono.Cecil.CustomAttribute(Utilities.GetEmptyConstructor(typeof(FieldOffsetAttribute), field.DeclaringType.Module, (IList<Type>)(new Type[] { typeof(Int32) })));
			TypeReference num = field.FieldType.Module.TypeSystem.Int32;
			CustomAttributeArgument customAttributeArgument = new CustomAttributeArgument(num, (object)field.Offset);
			customAttribute.ConstructorArguments.Add(customAttributeArgument);
			return customAttribute;
		}

		public static Mono.Cecil.CustomAttribute GetFieldNotSerializedAttribute(FieldDefinition field)
		{
			return new Mono.Cecil.CustomAttribute(Utilities.GetEmptyConstructor(typeof(NonSerializedAttribute), field.DeclaringType.Module, null));
		}

		public static ICollection<TypeReference> GetMethodAttributesUsedTypes(MethodDefinition method, ILanguage language)
		{
			List<TypeReference> typeReferences = new List<TypeReference>();
			foreach (Mono.Cecil.CustomAttribute customAttribute in method.CustomAttributes)
			{
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(customAttribute));
			}
			ModuleDefinition module = method.DeclaringType.Module;
			if (method.HasSecurityDeclarations)
			{
				foreach (SecurityDeclaration securityDeclaration in method.SecurityDeclarations)
				{
					if (securityDeclaration.HasSecurityAttributes)
					{
						foreach (SecurityAttribute securityAttribute in securityDeclaration.SecurityAttributes)
						{
							typeReferences.AddRange(AttributesUtilities.GetSecurityAttributeUsedTypes(securityAttribute));
						}
					}
					typeReferences.Add(securityDeclaration.GetSecurityActionTypeReference(module));
				}
			}
			foreach (ParameterDefinition parameter in method.Parameters)
			{
				if (parameter.IsOutParameter() && !language.HasOutKeyword)
				{
					typeReferences.Add(AttributesUtilities.GetOutAttributeTypeReference(method.DeclaringType.Module));
				}
				foreach (Mono.Cecil.CustomAttribute customAttribute1 in parameter.CustomAttributes)
				{
					typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(customAttribute1));
				}
			}
			if (method.HasPInvokeInfo && method.PInvokeInfo != null)
			{
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(AttributesUtilities.GetMethodDllImportAttribute(method)));
			}
			if (method.HasImplAttributes && AttributesUtilities.ShouldWriteMethodImplAttribute(method))
			{
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(AttributesUtilities.GetMethodImplAttribute(method)));
			}
			return typeReferences;
		}

		public static Mono.Cecil.CustomAttribute GetMethodDllImportAttribute(MethodDefinition method)
		{
			Mono.Cecil.CustomAttribute customAttribute = new Mono.Cecil.CustomAttribute(Utilities.GetEmptyConstructor(typeof(DllImportAttribute), method.DeclaringType.Module, (IList<Type>)(new Type[] { typeof(String) })));
			string name = method.PInvokeInfo.Module.Name;
			TypeReference str = method.DeclaringType.Module.TypeSystem.String;
			CustomAttributeArgument customAttributeArgument = new CustomAttributeArgument(str, name);
			customAttribute.ConstructorArguments.Add(customAttributeArgument);
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
			Mono.Cecil.CustomAttribute customAttribute = new Mono.Cecil.CustomAttribute(Utilities.GetEmptyConstructor(typeof(MethodImplAttribute), method.DeclaringType.Module, typeArray));
			AttributesUtilities.AddMethodImplOptions(method, customAttribute);
			AttributesUtilities.AddMethodCodeType(method, customAttribute);
			return customAttribute;
		}

		private static CustomAttributeArgument GetMethodImplAttributeArgument(MethodDefinition method, object value)
		{
			ModuleDefinition module = method.DeclaringType.Module;
			AssemblyNameReference assemblyNameReference = module.ReferencedMscorlibRef();
			Type type = value.GetType();
			return new CustomAttributeArgument(new TypeReference(type.Namespace, type.Name, module, assemblyNameReference), (object)((Int32)value));
		}

		public static ICollection<TypeReference> GetModuleAttributesUsedTypes(ModuleDefinition module)
		{
			List<TypeReference> typeReferences = new List<TypeReference>();
			List<Mono.Cecil.CustomAttribute> customAttributes = new List<Mono.Cecil.CustomAttribute>();
			foreach (Mono.Cecil.CustomAttribute customAttribute in module.CustomAttributes)
			{
				customAttribute.Resolve();
				customAttributes.Add(customAttribute);
			}
			foreach (Mono.Cecil.CustomAttribute customAttribute1 in customAttributes)
			{
				if (customAttribute1.AttributeType.FullName.Equals("System.Security.UnverifiableCodeAttribute", StringComparison.Ordinal))
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
			foreach (Mono.Cecil.CustomAttribute customAttribute in property.CustomAttributes)
			{
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(customAttribute));
			}
			if (property.GetMethod != null)
			{
				typeReferences.AddRange(AttributesUtilities.GetMethodAttributesUsedTypes(property.GetMethod, language));
			}
			if (property.SetMethod != null)
			{
				typeReferences.AddRange(AttributesUtilities.GetMethodAttributesUsedTypes(property.SetMethod, language));
			}
			return typeReferences;
		}

		private static ICollection<TypeReference> GetSecurityAttributeUsedTypes(SecurityAttribute attribute)
		{
			List<TypeReference> typeReferences = new List<TypeReference>()
			{
				attribute.AttributeType
			};
			if (attribute.HasFields || attribute.HasProperties)
			{
				TypeDefinition typeDefinition = attribute.AttributeType.Resolve();
				if (attribute.HasProperties)
				{
					typeReferences.AddRange(AttributesUtilities.GetAttributeNamedArgsUsedTypes(typeDefinition, attribute.Properties, false));
				}
				if (attribute.HasFields)
				{
					typeReferences.AddRange(AttributesUtilities.GetAttributeNamedArgsUsedTypes(typeDefinition, attribute.Fields, true));
				}
			}
			return typeReferences;
		}

		public static ICollection<TypeReference> GetTypeAttributesUsedTypes(TypeDefinition type)
		{
			List<TypeReference> typeReferences = new List<TypeReference>();
			foreach (Mono.Cecil.CustomAttribute customAttribute in type.CustomAttributes)
			{
				customAttribute.Resolve();
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(customAttribute));
			}
			if (type.IsSerializable)
			{
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(AttributesUtilities.GetTypeSerializableAttribute(type)));
			}
			if (type.IsExplicitLayout)
			{
				typeReferences.AddRange(AttributesUtilities.GetCustomAttributeUsedTypes(AttributesUtilities.GetTypeExplicitLayoutAttribute(type)));
			}
			if (type.HasSecurityDeclarations)
			{
				ModuleDefinition module = type.Module;
				foreach (SecurityDeclaration securityDeclaration in type.SecurityDeclarations)
				{
					if (securityDeclaration.HasSecurityAttributes)
					{
						foreach (SecurityAttribute securityAttribute in securityDeclaration.SecurityAttributes)
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
			Mono.Cecil.CustomAttribute customAttribute = new Mono.Cecil.CustomAttribute(Utilities.GetEmptyConstructor(typeof(StructLayoutAttribute), type.Module, (IList<Type>)(new Type[] { typeof(LayoutKind) })));
			TypeReference corlibTypeReference = Utilities.GetCorlibTypeReference(typeof(LayoutKind), type.Module);
			CustomAttributeArgument customAttributeArgument = new CustomAttributeArgument(corlibTypeReference, (object)2);
			customAttribute.ConstructorArguments.Add(customAttributeArgument);
			return customAttribute;
		}

		public static Mono.Cecil.CustomAttribute GetTypeSerializableAttribute(TypeDefinition type)
		{
			return new Mono.Cecil.CustomAttribute(Utilities.GetEmptyConstructor(typeof(SerializableAttribute), type.Module, null));
		}

		internal static bool ShouldWriteMethodImplAttribute(MethodDefinition method)
		{
			if (method.DeclaringType.IsDelegate())
			{
				return false;
			}
			if (method.HasPInvokeInfo && method.ImplAttributes == Mono.Cecil.MethodImplAttributes.PreserveSig)
			{
				return false;
			}
			return true;
		}
	}
}