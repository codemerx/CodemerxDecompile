using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using System.Runtime.InteropServices;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Languages;
using System.Collections;
using Mono.Collections.Generic;
using Telerik.JustDecompiler.Common;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Decompiler
{
	public static class AttributesUtilities
	{
		public static ICollection<TypeReference> GetAssemblyAttributesUsedTypes(AssemblyDefinition assembly)
		{
			List<TypeReference> usedTypes = new List<TypeReference>();
			List<ICustomAttribute> assemblyAttributes = new List<ICustomAttribute>();

			CustomAttribute assemblyVersionAttribute = GetAssemblyVersionAttribute(assembly);
			assemblyAttributes.Add(assemblyVersionAttribute);

			foreach (CustomAttribute attribute in assembly.CustomAttributes)
			{
				attribute.Resolve();
				assemblyAttributes.Add(attribute);
			}

			if (assembly.HasSecurityDeclarations)
			{
				foreach (SecurityDeclaration securityDeclaration in assembly.SecurityDeclarations)
				{
					if (securityDeclaration.HasSecurityAttributes)
					{
						foreach (SecurityAttribute attribute in securityDeclaration.SecurityAttributes)
						{
							assemblyAttributes.Add(attribute);
						}
					}
				}
			}

			if (assembly.MainModule.HasExportedTypes)
			{
				foreach (ExportedType exportedType in assembly.MainModule.ExportedTypes)
				{
					if (!(exportedType.Scope is ModuleReference))
					{
						assemblyAttributes.Add(GetExportedTypeAttribute(exportedType, assembly.MainModule));
					}
				}
			}

			foreach (ICustomAttribute attribute in assemblyAttributes)
			{
				if (attribute is CustomAttribute)
				{
					ICollection<TypeReference> attributeTypesDependingOn = GetCustomAttributeUsedTypes(attribute as CustomAttribute);
					foreach (TypeReference type in attributeTypesDependingOn)
					{
						usedTypes.Add(type);
					}
				}
				else if (attribute is SecurityAttribute)
				{
					ICollection<TypeReference> attributeTypesDependingOn = GetSecurityAttributeUsedTypes(attribute as SecurityAttribute);
					foreach (TypeReference type in attributeTypesDependingOn)
					{
						usedTypes.Add(type);
					}
				}
			}

			return usedTypes;
		}

		public static ICollection<TypeReference> GetModuleAttributesUsedTypes(ModuleDefinition module)
		{
			List<TypeReference> usedTypes = new List<TypeReference>();
			List<CustomAttribute> moduleAttributes = new List<CustomAttribute>();

			foreach (CustomAttribute attribute in module.CustomAttributes)
			{
				attribute.Resolve();
				moduleAttributes.Add(attribute);
			}

			foreach (CustomAttribute attribute in moduleAttributes)
			{
				if (attribute.AttributeType.FullName.Equals("System.Security.UnverifiableCodeAttribute", StringComparison.Ordinal))
				{
					continue;
				}

				ICollection<TypeReference> attributeTypesDependingOn = GetCustomAttributeUsedTypes(attribute);
				foreach (TypeReference type in attributeTypesDependingOn)
				{
					usedTypes.Add(type);
				}
			}

			return usedTypes;
		}

		public static ICollection<TypeReference> GetTypeAttributesUsedTypes(TypeDefinition type)
		{
			List<TypeReference> usedTypes = new List<TypeReference>();

			foreach (CustomAttribute attribute in type.CustomAttributes)
			{
				attribute.Resolve();
				usedTypes.AddRange(GetCustomAttributeUsedTypes(attribute));
			}

			// [Serializable] attribute
			if (type.IsSerializable)
			{
				usedTypes.AddRange(GetCustomAttributeUsedTypes(GetTypeSerializableAttribute(type)));
			}

			// [StructLayout(LayoutKind.Explicit)] attribute
			if (type.IsExplicitLayout)
			{
				usedTypes.AddRange(GetCustomAttributeUsedTypes(GetTypeExplicitLayoutAttribute(type)));
			}

			if (type.HasSecurityDeclarations)
			{
				ModuleDefinition module = type.Module;

				foreach (SecurityDeclaration securityDeclaration in type.SecurityDeclarations)
				{
					if (securityDeclaration.HasSecurityAttributes)
					{
						foreach (SecurityAttribute attribute in securityDeclaration.SecurityAttributes)
						{
							usedTypes.AddRange(GetSecurityAttributeUsedTypes(attribute));
						}
					}

					usedTypes.Add(securityDeclaration.GetSecurityActionTypeReference(module));
				}
			}

			return usedTypes;
		}

		public static ICollection<TypeReference> GetFieldAttributesUsedTypes(FieldDefinition field)
		{
			List<TypeReference> usedTypes = new List<TypeReference>();

			foreach (CustomAttribute attribute in field.CustomAttributes)
			{
				usedTypes.AddRange(GetCustomAttributeUsedTypes(attribute));
			}

			// [NotSerialized] attribute
			if (field.IsNotSerialized)
			{
				usedTypes.AddRange(GetCustomAttributeUsedTypes(GetFieldNotSerializedAttribute(field)));
			}
			// [FieldOffset(x)] attribute
			if (field.DeclaringType.IsExplicitLayout)
			{
				usedTypes.AddRange(GetCustomAttributeUsedTypes(GetFieldFieldOffsetAttribute(field)));
			}

			return usedTypes;
		}

		public static ICollection<TypeReference> GetPropertyAttributesUsedTypes(PropertyDefinition property, ILanguage language)
		{
			List<TypeReference> usedTypes = new List<TypeReference>();

			foreach (CustomAttribute attribute in property.CustomAttributes)
			{
				usedTypes.AddRange(GetCustomAttributeUsedTypes(attribute));
			}

			if (property.GetMethod != null)
			{
				usedTypes.AddRange(GetMethodAttributesUsedTypes(property.GetMethod, language));
			}

			if (property.SetMethod != null)
			{
				usedTypes.AddRange(GetMethodAttributesUsedTypes(property.SetMethod, language));
			}

			return usedTypes;
		}

		public static ICollection<TypeReference> GetEventAttributesUsedTypes(EventDefinition @event, ILanguage language)
		{
			List<TypeReference> usedTypes = new List<TypeReference>();

			foreach (CustomAttribute attribute in @event.CustomAttributes)
			{
				usedTypes.AddRange(GetCustomAttributeUsedTypes(attribute));
			}

			if (@event.AddMethod != null)
			{
				usedTypes.AddRange(GetMethodAttributesUsedTypes(@event.AddMethod, language));
			}

			if (@event.RemoveMethod != null)
			{
				usedTypes.AddRange(GetMethodAttributesUsedTypes(@event.RemoveMethod, language));
			}

			if (@event.InvokeMethod != null)
			{
				usedTypes.AddRange(GetMethodAttributesUsedTypes(@event.InvokeMethod, language));
			}

			return usedTypes;
		}

		public static ICollection<TypeReference> GetMethodAttributesUsedTypes(MethodDefinition method, ILanguage language)
		{
			List<TypeReference> usedTypes = new List<TypeReference>();

			foreach (CustomAttribute attribute in method.CustomAttributes)
			{
				usedTypes.AddRange(GetCustomAttributeUsedTypes(attribute));
			}

			ModuleDefinition module = method.DeclaringType.Module;

			if (method.HasSecurityDeclarations)
			{
				foreach (SecurityDeclaration securityDeclaration in method.SecurityDeclarations)
				{
					if (securityDeclaration.HasSecurityAttributes)
					{
						foreach (SecurityAttribute attribute in securityDeclaration.SecurityAttributes)
						{
							usedTypes.AddRange(GetSecurityAttributeUsedTypes(attribute));
						}
					}

					usedTypes.Add(securityDeclaration.GetSecurityActionTypeReference(module));
				}
			}

			foreach (ParameterDefinition parameter in method.Parameters)
			{
				if (parameter.IsOutParameter() && !language.HasOutKeyword)
				{
					TypeReference outAttributeTypeRef = GetOutAttributeTypeReference(method.DeclaringType.Module);

					usedTypes.Add(outAttributeTypeRef);
				}

				foreach (CustomAttribute attribute in parameter.CustomAttributes)
				{
					usedTypes.AddRange(GetCustomAttributeUsedTypes(attribute));
				}
			}

			// add [DllImport(dllName,CharSet=CharSet.*)] attribute analysis
			if (method.HasPInvokeInfo && method.PInvokeInfo != null)
			{
				usedTypes.AddRange(GetCustomAttributeUsedTypes(GetMethodDllImportAttribute(method)));
			}

            if (method.HasImplAttributes &&
                ShouldWriteMethodImplAttribute(method))
            {
                usedTypes.AddRange(GetCustomAttributeUsedTypes(GetMethodImplAttribute(method)));
            }

            return usedTypes;
		}

		public static TypeReference GetOutAttributeTypeReference(ModuleDefinition module)
		{
			AssemblyNameReference mscorlib = module.ReferencedMscorlibRef();

			return new TypeReference("System.Runtime.InteropServices", "OutAttribute", module, mscorlib);
		}

		public static CustomAttribute GetFieldFieldOffsetAttribute(FieldDefinition field)
		{
			MethodReference attrCtor = Utilities.GetEmptyConstructor(typeof(FieldOffsetAttribute), field.DeclaringType.Module, new Type[] { typeof(int) });
			CustomAttribute fieldOffsetAttr = new CustomAttribute(attrCtor);
			TypeReference fieldOffsetType = field.FieldType.Module.TypeSystem.Int32;
			CustomAttributeArgument offsetArg = new CustomAttributeArgument(fieldOffsetType, field.Offset);
			fieldOffsetAttr.ConstructorArguments.Add(offsetArg);

			return fieldOffsetAttr;
		}

		public static CustomAttribute GetFieldNotSerializedAttribute(FieldDefinition field)
		{
			MethodReference attCtor = Utilities.GetEmptyConstructor(typeof(NonSerializedAttribute), field.DeclaringType.Module);
			return new CustomAttribute(attCtor);
		}

		#region DllImport Attribute

		public static CustomAttribute GetMethodDllImportAttribute(MethodDefinition method)
		{
			MethodReference attributeConstructor = Utilities.GetEmptyConstructor(typeof(System.Runtime.InteropServices.DllImportAttribute),
											method.DeclaringType.Module, new Type[] { typeof(string) });
			CustomAttribute dllImportAttr = new CustomAttribute(attributeConstructor);

			string dllName = method.PInvokeInfo.Module.Name;
			TypeReference dllNameArgumentType = method.DeclaringType.Module.TypeSystem.String;
			CustomAttributeArgument dllNameArg = new CustomAttributeArgument(dllNameArgumentType, dllName);
			dllImportAttr.ConstructorArguments.Add(dllNameArg);

			CreateAndAddBestFitMappingFieldArgument(method, dllImportAttr);

			CreateAndAddCallingConventionFieldArgument(method, dllImportAttr);

			CreateAndAddCharSetFieldArgument(method, dllImportAttr);

			CreateAndAddEntryPointFieldArgument(method, dllImportAttr);

			CreateAndAddExactSpellingFieldArgument(method, dllImportAttr);

			CreateAndAddPreserveSigFieldArgument(method, dllImportAttr);

			CreateAndAddSetLastErrorFieldArgument(method, dllImportAttr);

			CreateAndAddThrowOnUnmappableCharFieldArgument(method, dllImportAttr);

			return dllImportAttr;
		}

		private static void CreateAndAddThrowOnUnmappableCharFieldArgument(MethodDefinition method, CustomAttribute dllImportAttr)
		{
			// ThrowOnUnmappableChar is false by default
			if (method.PInvokeInfo.IsThrowOnUnmappableCharEnabled)
			{
				TypeReference throwOnUnmappableCharArgumentType = method.DeclaringType.Module.TypeSystem.Boolean;
				CustomAttributeArgument throwOnUnmappableCharArgument = new CustomAttributeArgument(throwOnUnmappableCharArgumentType, true);
				CustomAttributeNamedArgument namedThrowOnUnmappableCharArgument = new CustomAttributeNamedArgument("ThrowOnUnmappableChar", throwOnUnmappableCharArgument);
				dllImportAttr.Fields.Add(namedThrowOnUnmappableCharArgument);
			}

		}

		private static void CreateAndAddSetLastErrorFieldArgument(MethodDefinition method, CustomAttribute dllImportAttr)
		{
			//SetLastError is false by default
			if (method.PInvokeInfo.SupportsLastError)
			{
				TypeReference setLastErrorArgumentType = method.DeclaringType.Module.TypeSystem.Boolean;
				CustomAttributeArgument setLastErrorArgument = new CustomAttributeArgument(setLastErrorArgumentType, true);
				CustomAttributeNamedArgument namedSetLastErrorArgument = new CustomAttributeNamedArgument("SetLastError", setLastErrorArgument);
				dllImportAttr.Fields.Add(namedSetLastErrorArgument);
			}
		}

		private static void CreateAndAddPreserveSigFieldArgument(MethodDefinition method, CustomAttribute dllImportAttr)
		{
			/// PreserveSig is true by default
			if (!method.IsPreserveSig)
			{
				TypeReference preserveSigArgumentType = method.DeclaringType.Module.TypeSystem.Boolean;
				CustomAttributeArgument preserveSigArgument = new CustomAttributeArgument(preserveSigArgumentType, false);
				CustomAttributeNamedArgument namedPreserveSigArgument = new CustomAttributeNamedArgument("PreserveSig", preserveSigArgument);
				dllImportAttr.Fields.Add(namedPreserveSigArgument);
			}
		}

		private static void CreateAndAddExactSpellingFieldArgument(MethodDefinition method, CustomAttribute dllImportAttr)
		{
			TypeReference exactSpellingArgumentType = method.DeclaringType.Module.TypeSystem.Boolean;
			CustomAttributeArgument exactSpellingArgument = new CustomAttributeArgument(exactSpellingArgumentType, method.PInvokeInfo.IsNoMangle);
			CustomAttributeNamedArgument namedExactSpellingArgument = new CustomAttributeNamedArgument("ExactSpelling", exactSpellingArgument);
			dllImportAttr.Fields.Add(namedExactSpellingArgument);
		}

		private static void CreateAndAddBestFitMappingFieldArgument(MethodDefinition method, CustomAttribute dllImportAttr)
		{
			// BestFitMapping is true by default
			if (method.PInvokeInfo.IsBestFitDisabled)
			{
				TypeReference bestFitMappingArgumentType = method.DeclaringType.Module.TypeSystem.Boolean;
				CustomAttributeArgument bestFitMappingArgument = new CustomAttributeArgument(bestFitMappingArgumentType, false);
				CustomAttributeNamedArgument namedBestFitMappingArgument = new CustomAttributeNamedArgument("BestFitMapping", bestFitMappingArgument);
				dllImportAttr.Fields.Add(namedBestFitMappingArgument);
			}
		}

		private static void CreateAndAddEntryPointFieldArgument(MethodDefinition method, CustomAttribute dllImportAttr)
		{
			// EntryPoint is the name of the method by default
			if (method.PInvokeInfo.EntryPoint != method.Name)
			{
				TypeReference entryPointArgumentType = method.DeclaringType.Module.TypeSystem.String;
				CustomAttributeArgument entryPointArgument = new CustomAttributeArgument(entryPointArgumentType, method.PInvokeInfo.EntryPoint);
				CustomAttributeNamedArgument namedEntryPointArgument = new CustomAttributeNamedArgument("EntryPoint", entryPointArgument);
				dllImportAttr.Fields.Add(namedEntryPointArgument);
			}
		}

		private static void CreateAndAddCallingConventionFieldArgument(MethodDefinition method, CustomAttribute dllImportAttr)
		{
			/// this StdCall is the default calling conventon (according to http://msdn.microsoft.com/en-us/library/system.runtime.interopservices.dllimportattribute.callingconvention.aspx)
			if (!method.PInvokeInfo.IsCallConvWinapi)
			{
				TypeReference callingConventionArgumentType = Utilities.GetCorlibTypeReference(typeof(System.Runtime.InteropServices.CallingConvention), method.DeclaringType.Module);
				/// set the integer value of the field, so that RenameEnumValues can pick it up and fix it accordinly.
				int callingConventionValue = 0;
				if (method.PInvokeInfo.IsCallConvFastcall)
				{
					callingConventionValue = 5;
				}
				else if (method.PInvokeInfo.IsCallConvThiscall)
				{
					callingConventionValue = 4;
				}
				else if (method.PInvokeInfo.IsCallConvStdCall)
				{
					callingConventionValue = 3;
				}
				else if (method.PInvokeInfo.IsCallConvCdecl)
				{
					callingConventionValue = 2;
				}
				else
				{
					//default case, shouldn't be reached
					callingConventionValue = 1;
				}
				CustomAttributeArgument callingConventionArgument = new CustomAttributeArgument(callingConventionArgumentType, callingConventionValue);
				CustomAttributeNamedArgument namedCallingConventionArgument = new CustomAttributeNamedArgument("CallingConvention", callingConventionArgument);
				dllImportAttr.Fields.Add(namedCallingConventionArgument);
			}
		}

        internal static bool ShouldWriteMethodImplAttribute(MethodDefinition method)
        {
            if (method.DeclaringType.IsDelegate())
            {
                return false;
            }

            // The preservesig flag can be controlled using the DllImport attribute.
            if (method.HasPInvokeInfo &&
                method.ImplAttributes == MethodImplAttributes.PreserveSig)
            {
                return false;
            }

            return true;
        }

        private static void CreateAndAddCharSetFieldArgument(MethodDefinition method, CustomAttribute dllImportAttr)
		{
			System.Runtime.InteropServices.CharSet charSet = System.Runtime.InteropServices.CharSet.None;
			if (method.PInvokeInfo.IsCharSetAnsi)
			{
				charSet = System.Runtime.InteropServices.CharSet.Ansi;
			}
			if (method.PInvokeInfo.IsCharSetUnicode)
			{
				charSet = System.Runtime.InteropServices.CharSet.Unicode;
			}
			if (method.PInvokeInfo.IsCharSetAuto)
			{
				charSet = System.Runtime.InteropServices.CharSet.Auto;
			}

			TypeReference charSetArgumentType = Utilities.GetCorlibTypeReference(typeof(System.Runtime.InteropServices.CharSet), method.DeclaringType.Module);
			//Casted to int, so that it can be resolved to enum member by EnumValueToFieldCombinationMatcher.GetEnumFieldDefinitionByValue
			//invokd in WeiteAttributeNamedArgs
			CustomAttributeArgument charSetArgument = new CustomAttributeArgument(charSetArgumentType, (int)(charSet));
			CustomAttributeNamedArgument namedCharSetArgument = new CustomAttributeNamedArgument("CharSet", charSetArgument);
			dllImportAttr.Fields.Add(namedCharSetArgument);
		}

		#endregion

		public static CustomAttribute GetTypeExplicitLayoutAttribute(TypeDefinition type)
		{
			MethodReference attCtor = Utilities.GetEmptyConstructor(typeof(StructLayoutAttribute), type.Module, new Type[] { typeof(LayoutKind) });
			CustomAttribute structLayoutAttr = new CustomAttribute(attCtor);
			TypeReference layoutTypeRef = JustDecompiler.Decompiler.Utilities.GetCorlibTypeReference(typeof(LayoutKind), type.Module);
			CustomAttributeArgument layoutType = new CustomAttributeArgument(layoutTypeRef, (int)(LayoutKind.Explicit));
			structLayoutAttr.ConstructorArguments.Add(layoutType);

			return structLayoutAttr;
		}

		public static CustomAttribute GetTypeSerializableAttribute(TypeDefinition type)
		{
			MethodReference attCtor = Utilities.GetEmptyConstructor(typeof(SerializableAttribute), type.Module);
			return new CustomAttribute(attCtor);
		}

		public static CustomAttribute GetExportedTypeAttribute(ExportedType exportedType, ModuleDefinition module)
		{
			MethodReference ctor = Utilities.GetEmptyConstructor(typeof(System.Runtime.CompilerServices.TypeForwardedToAttribute), module, new[] { typeof(System.Type) });

			CustomAttribute exportedTypeAttribute = new CustomAttribute(ctor);
			TypeReference systemType = Utilities.GetCorlibTypeReference(typeof(Type), module);
			TypeReference type = exportedType.CreateReference();
			exportedTypeAttribute.ConstructorArguments.Add(new CustomAttributeArgument(systemType, type));

			return exportedTypeAttribute;
		}

		public static CustomAttribute GetAssemblyVersionAttribute(AssemblyDefinition assembly)
		{
			/// Get the same scope as the type system does for the core types.
			IMetadataScope scope = assembly.MainModule.TypeSystem.Boolean.Scope;
			TypeReference typeRef = new TypeReference("System.Reflection", "AssemblyVersionAttribute", assembly.MainModule, scope);
			MethodReference ctor = new MethodReference(".ctor", assembly.MainModule.TypeSystem.Void, typeRef);
			ctor.Parameters.Add(new ParameterDefinition(assembly.MainModule.TypeSystem.String));

			CustomAttribute version = new CustomAttribute(ctor);

			string versionString = assembly.Name.Version.ToString(4);
			CustomAttributeArgument versionArgument = new CustomAttributeArgument(assembly.MainModule.TypeSystem.String, versionString);
			version.ConstructorArguments.Add(versionArgument);

			return version;
		}

		private static ICollection<TypeReference> GetAttributeArgumentArrayUsedTypes(CustomAttributeArgument argument)
		{
			List<TypeReference> usedTypes = new List<TypeReference>();

			CustomAttributeArgument[] elements = argument.Value as CustomAttributeArgument[];

			usedTypes.Add(argument.Type);
			for (int i = 0; i < elements.Length; i++)
			{
				usedTypes.AddRange(GetAttributeArgumentValueUsedTypes(elements[i]));
			}

			return usedTypes;
		}

		private static ICollection<TypeReference> GetAttributeArgumentValueUsedTypes(CustomAttributeArgument argument)
		{
			if (argument.Value is CustomAttributeArgument)
			{
				return GetAttributeArgumentValueUsedTypes((CustomAttributeArgument)argument.Value);
			}
			else if (argument.Value is CustomAttributeArgument[])
			{
				return GetAttributeArgumentArrayUsedTypes(argument);
			}

			List<TypeReference> usedTypes = new List<TypeReference>();

			TypeDefinition argumentTypeDefinition = argument.Type.IsDefinition ? argument.Type as TypeDefinition : argument.Type.Resolve();
			if (argumentTypeDefinition != null && argumentTypeDefinition.IsEnum)
			{
				List<FieldDefinition> fields = EnumValueToFieldCombinationMatcher.GetEnumFieldDefinitionByValue(argumentTypeDefinition.Fields, argument.Value, argumentTypeDefinition.CustomAttributes);
				if (fields.Count != 0)
				{
					for (int i = 0; i < fields.Count; i++)
					{
						usedTypes.AddRange(Utilities.GetTypeReferenceTypesDepedningOn(fields[i].DeclaringType));
					}
				}
			}
			else
			{
				if (argument.Type.Name == "Type" && argument.Type.Namespace == "System")
				{
					usedTypes.AddRange(Utilities.GetTypeReferenceTypesDepedningOn(argument.Value as TypeReference));
				}
			}

			return usedTypes;
		}

		private static ICollection<TypeReference> GetAttributeNamedArgsUsedTypes(TypeDefinition attributeType, Collection<CustomAttributeNamedArgument> namedArguments, bool fields)
		{
			List<TypeReference> usedTypes = new List<TypeReference>();

			for (int propertyIndex = 0; propertyIndex < namedArguments.Count; propertyIndex++)
			{
				if (attributeType != null)
				{
					IList fieldsOrProperties = fields ? (IList)attributeType.Fields : (IList)attributeType.Properties;

					MemberReference memberArgumentRefersTo = null;
					IList currentFieldsOrProperties = fieldsOrProperties;
					TypeDefinition currentTypeInInheritanceChain = attributeType;
					do
					{
						memberArgumentRefersTo = Utilities.FindMemberArgumentRefersTo(currentFieldsOrProperties, namedArguments[propertyIndex]);


						if (currentTypeInInheritanceChain.BaseType != null)
						{
							currentTypeInInheritanceChain = currentTypeInInheritanceChain.BaseType.Resolve();
							if (currentTypeInInheritanceChain == null)
							{
								break;
							}
							currentFieldsOrProperties = fields ? (IList)currentTypeInInheritanceChain.Fields : (IList)currentTypeInInheritanceChain.Properties;
						}
						else
						{
							break;
						}
					}
					while (memberArgumentRefersTo == null);

					if (memberArgumentRefersTo != null)
					{
						usedTypes.Add(memberArgumentRefersTo.DeclaringType);
					}
				}

				usedTypes.AddRange(GetAttributeArgumentValueUsedTypes(namedArguments[propertyIndex].Argument));
			}

			return usedTypes;
		}

		private static ICollection<TypeReference> GetCustomAttributeUsedTypes(CustomAttribute attribute)
		{
			List<TypeReference> usedTypes = new List<TypeReference>();

			attribute.Resolve();

			usedTypes.AddRange(Utilities.GetTypeReferenceTypesDepedningOn(attribute.AttributeType));

			for (int argIndex = 0; argIndex < attribute.ConstructorArguments.Count; argIndex++)
			{
				usedTypes.AddRange(GetAttributeArgumentValueUsedTypes(attribute.ConstructorArguments[argIndex]));
			}

			if (attribute.HasConstructorArguments || attribute.HasFields || attribute.HasProperties)
			{

				if (attribute.HasProperties)
				{
					TypeDefinition attributeType = attribute.AttributeType.Resolve();
					usedTypes.AddRange(GetAttributeNamedArgsUsedTypes(attributeType, attribute.Properties, false));
				}
				if (attribute.HasFields)
				{
					TypeDefinition attributeType = attribute.AttributeType.Resolve();
					usedTypes.AddRange(GetAttributeNamedArgsUsedTypes(attributeType, attribute.Fields, true));
				}
			}

			return usedTypes;
		}

		private static ICollection<TypeReference> GetSecurityAttributeUsedTypes(SecurityAttribute attribute)
		{
			List<TypeReference> typesDependingOn = new List<TypeReference>();

			typesDependingOn.Add(attribute.AttributeType);
			if (attribute.HasFields || attribute.HasProperties)
			{
				var attributeType = attribute.AttributeType.Resolve();
				if (attribute.HasProperties)
				{
					typesDependingOn.AddRange(GetAttributeNamedArgsUsedTypes(attributeType, attribute.Properties, false));
				}
				if (attribute.HasFields)
				{
					typesDependingOn.AddRange(GetAttributeNamedArgsUsedTypes(attributeType, attribute.Fields, true));
				}
			}

			return typesDependingOn;
		}

        #region MethodImplAttribute

        public static CustomAttribute GetMethodImplAttribute(MethodDefinition method)
        {
            Type[] argumentTypes = DoesMethodHaveMethodImplOptions(method) ? new Type[] { typeof(MethodImplOptions) } : new Type[0];
            MethodReference constructor = Utilities.GetEmptyConstructor(typeof(MethodImplAttribute), method.DeclaringType.Module, argumentTypes);
            CustomAttribute attribute = new CustomAttribute(constructor);
            
            AddMethodImplOptions(method, attribute);
            AddMethodCodeType(method, attribute);

            return attribute;
        }
        
        private static void AddMethodImplOptions(MethodDefinition method, CustomAttribute attribute)
        {
            if (DoesMethodHaveMethodImplOptions(method))
            {
                MethodImplOptions value = default(MethodImplOptions);
                
                if (method.AggressiveInlining)
                {
                    value |= MethodImplOptions.AggressiveInlining;
                }

                if (method.IsForwardRef)
                {
                    value |= MethodImplOptions.ForwardRef;
                }

                if (method.IsInternalCall)
                {
                    value |= MethodImplOptions.InternalCall;
                }

                if (method.NoInlining)
                {
                    value |= MethodImplOptions.NoInlining;
                }

                if (method.NoOptimization)
                {
                    value |= MethodImplOptions.NoOptimization;
                }

                if (method.IsPreserveSig && !method.HasPInvokeInfo)
                {
                    value |= MethodImplOptions.PreserveSig;
                }

                if (method.IsSynchronized)
                {
                    value |= MethodImplOptions.Synchronized;
                }

                if (method.IsUnmanaged)
                {
                    value |= MethodImplOptions.Unmanaged;
                }

                attribute.ConstructorArguments.Add(GetMethodImplAttributeArgument(method, value));
            }
        }

        private static void AddMethodCodeType(MethodDefinition method, CustomAttribute attribute)
        {
            if (method.IsNative || method.IsOPTIL || method.IsRuntime)
            {
                MethodCodeType value = default(MethodCodeType);
                
                if (method.IsNative)
                {
                    value |= MethodCodeType.Native;
                }

                if (method.IsOPTIL)
                {
                    value |= MethodCodeType.OPTIL;
                }

                if (method.IsRuntime)
                {
                    value |= MethodCodeType.Runtime;
                }

                attribute.Fields.Add(new CustomAttributeNamedArgument("MethodCodeType", GetMethodImplAttributeArgument(method, value)));
            }
        }

        private static CustomAttributeArgument GetMethodImplAttributeArgument(MethodDefinition method, object value)
        {
            ModuleDefinition module = method.DeclaringType.Module;
            AssemblyNameReference mscorlib = module.ReferencedMscorlibRef();

            Type argumentType = value.GetType();
            TypeReference cecilArgumentType = new TypeReference(argumentType.Namespace, argumentType.Name, module, mscorlib);

            return new CustomAttributeArgument(cecilArgumentType, (int)value);
        }
        
        private static bool DoesMethodHaveMethodImplOptions(MethodDefinition method)
        {
            return method.AggressiveInlining ||
                   method.IsForwardRef ||
                   method.IsInternalCall ||
                   method.NoInlining ||
                   method.NoOptimization ||
                   (method.IsPreserveSig && !method.HasPInvokeInfo) ||
                   method.IsSynchronized ||
                   method.IsUnmanaged;
        }

        #endregion
    }
}
