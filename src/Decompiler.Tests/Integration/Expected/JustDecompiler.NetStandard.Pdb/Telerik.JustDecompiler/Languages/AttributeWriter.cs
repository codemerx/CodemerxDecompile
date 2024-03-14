using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Languages
{
	public abstract class AttributeWriter
	{
		protected readonly HashSet<string> attributesNotToShow = new HashSet<string>(new String[] { "System.ParamArrayAttribute", "System.Runtime.CompilerServices.IteratorStateMachineAttribute", "Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute" });

		protected NamespaceImperativeLanguageWriter genericWriter;

		private Dictionary<SecurityAttribute, SecurityDeclaration> securityAttributeToDeclaration = new Dictionary<SecurityAttribute, SecurityDeclaration>();

		private const string ASSEMBLYNOTRESOLVEDERROR = "JustDecompile was unable to locate the assembly where attribute parameters types are defined. Generating parameters values is impossible.";

		protected abstract string ClosingBracket
		{
			get;
		}

		protected abstract string EqualsSign
		{
			get;
		}

		protected abstract string OpeningBracket
		{
			get;
		}

		protected abstract string ParameterAttributeSeparator
		{
			get;
		}

		public AttributeWriter(NamespaceImperativeLanguageWriter writer)
		{
			this.genericWriter = writer;
		}

		public virtual List<ICustomAttribute> CollectSecurityAttributes(IMemberDefinition member)
		{
			List<ICustomAttribute> customAttributes = new List<ICustomAttribute>();
			if (member is ISecurityDeclarationMemberDefinition)
			{
				foreach (SecurityDeclaration securityDeclaration in (member as ISecurityDeclarationMemberDefinition).get_SecurityDeclarations())
				{
					customAttributes.AddRange(this.GetSecurityDeclaration(securityDeclaration));
				}
			}
			return customAttributes;
		}

		private int CompareAttributes(ICustomAttribute x, ICustomAttribute y)
		{
			string name = x.get_AttributeType().get_Name();
			string str = y.get_AttributeType().get_Name();
			if (name != str)
			{
				return name.CompareTo(str);
			}
			if (x is CustomAttribute && y is CustomAttribute)
			{
				return (x as CustomAttribute).CompareToCustomAttribute(y as CustomAttribute, false);
			}
			if (!(x is SecurityAttribute) || !(y is SecurityAttribute))
			{
				return 0;
			}
			return this.CompareSecurityAttributes(x as SecurityAttribute, y as SecurityAttribute);
		}

		private int CompareSecurityAttributes(SecurityAttribute first, SecurityAttribute second)
		{
			if ((object)first == (object)second)
			{
				return 0;
			}
			SecurityDeclaration item = this.securityAttributeToDeclaration[first];
			SecurityDeclaration securityDeclaration = this.securityAttributeToDeclaration[second];
			return first.CompareToSecurityAttribute(second, item, securityDeclaration);
		}

		private string GetElementTypeName(CustomAttributeArgument argument)
		{
			return this.genericWriter.ToEscapedTypeString((argument.get_Type() as ArrayType).get_ElementType());
		}

		protected virtual CustomAttribute GetInAttribute(ParameterDefinition parameter)
		{
			if (parameter == null || !parameter.get_IsIn())
			{
				return null;
			}
			return this.GetInOrOutAttribute(parameter, true);
		}

		protected CustomAttribute GetInOrOutAttribute(ParameterDefinition parameter, bool isInAttribute)
		{
			ModuleDefinition module = parameter.get_Method().get_ReturnType().get_Module();
			IMetadataScope corlib = module.get_TypeSystem().get_Corlib();
			TypeReference typeReference = new TypeReference("System.Runtime.InteropServices", (isInAttribute ? "InAttribute" : "OutAttribute"), module, corlib);
			if (typeReference == null)
			{
				return null;
			}
			return new CustomAttribute(new MethodReference(".ctor", module.get_TypeSystem().get_Void(), typeReference));
		}

		private ModuleDefinition GetModuleDefinition(IMemberDefinition member)
		{
			if (member is TypeDefinition)
			{
				return (member as TypeDefinition).get_Module();
			}
			return member.get_DeclaringType().get_Module();
		}

		protected virtual CustomAttribute GetOutAttribute(ParameterDefinition parameter)
		{
			if (parameter.IsOutParameter())
			{
				return null;
			}
			if (parameter == null || !parameter.get_IsOut())
			{
				return null;
			}
			return this.GetInOrOutAttribute(parameter, false);
		}

		private IEnumerable<ICustomAttribute> GetSecurityDeclaration(SecurityDeclaration securityDeclaration)
		{
			List<ICustomAttribute> customAttributes = new List<ICustomAttribute>();
			if (securityDeclaration.get_HasSecurityAttributes())
			{
				foreach (SecurityAttribute securityAttribute in securityDeclaration.get_SecurityAttributes())
				{
					customAttributes.Add(securityAttribute);
					this.securityAttributeToDeclaration.Add(securityAttribute, securityDeclaration);
				}
			}
			return customAttributes;
		}

		protected virtual List<ICustomAttribute> GetSortedReturnValueAttributes(IMethodSignature member)
		{
			List<ICustomAttribute> customAttributes = new List<ICustomAttribute>();
			if (member != null && member.get_MethodReturnType().get_HasCustomAttributes())
			{
				customAttributes.AddRange(member.get_MethodReturnType().get_CustomAttributes());
			}
			this.SortAttributes(customAttributes);
			return customAttributes;
		}

		private bool IsWinRTActivatableAttribute(CustomAttribute attribute)
		{
			if (attribute.get_AttributeType().get_FullName() != "Windows.Foundation.Metadata.ActivatableAttribute" || attribute.get_ConstructorArguments().get_Count() != 2)
			{
				return false;
			}
			CustomAttributeArgument item = attribute.get_ConstructorArguments().get_Item(0);
			TypeDefinition value = item.get_Value() as TypeDefinition;
			if (value == null)
			{
				return false;
			}
			return value.get_IsWindowsRuntime();
		}

		private void SortAttributes(List<ICustomAttribute> attributes)
		{
			attributes.Sort((ICustomAttribute x, ICustomAttribute y) => this.CompareAttributes(x, y));
		}

		private void WriteAssemblyAttribute(CustomAttribute attribute)
		{
			this.WriteGlobalAttribute(attribute, this.genericWriter.KeyWordWriter.Assembly);
		}

		public virtual void WriteAssemblyAttributes(AssemblyDefinition assembly, ICollection<string> attributesToIgnore = null)
		{
			bool flag;
			List<ICustomAttribute> customAttributes = new List<ICustomAttribute>();
			this.securityAttributeToDeclaration = new Dictionary<SecurityAttribute, SecurityDeclaration>();
			customAttributes.Add(AttributesUtilities.GetAssemblyVersionAttribute(assembly));
			foreach (CustomAttribute customAttribute in assembly.get_CustomAttributes())
			{
				customAttribute.Resolve();
				customAttributes.Add(customAttribute);
			}
			if (assembly.get_HasSecurityDeclarations())
			{
				foreach (SecurityDeclaration securityDeclaration in assembly.get_SecurityDeclarations())
				{
					customAttributes.AddRange(this.GetSecurityDeclaration(securityDeclaration));
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
			customAttributes.Sort((ICustomAttribute x, ICustomAttribute y) => this.CompareAttributes(x, y));
			foreach (ICustomAttribute customAttribute1 in customAttributes)
			{
				if (attributesToIgnore != null && attributesToIgnore.Contains(customAttribute1.get_AttributeType().get_FullName()))
				{
					continue;
				}
				if (!(customAttribute1 is CustomAttribute))
				{
					if (!(customAttribute1 is SecurityAttribute))
					{
						continue;
					}
					this.WriteSecurityAttribute(assembly.get_MainModule(), true, customAttribute1 as SecurityAttribute, this.securityAttributeToDeclaration[customAttribute1 as SecurityAttribute], out flag, false, false);
				}
				else
				{
					this.WriteAssemblyAttribute(customAttribute1 as CustomAttribute);
				}
			}
		}

		protected void WriteAttribute(CustomAttribute attribute, bool skipNewLine = false, bool isReturnValueAtrribute = false)
		{
			if (this.attributesNotToShow.Contains(attribute.get_AttributeType().get_FullName()))
			{
				return;
			}
			bool flag = false;
			attribute.Resolve();
			this.genericWriter.WriteToken(this.OpeningBracket);
			if (isReturnValueAtrribute)
			{
				this.WriteReturnValueAttributeKeyword();
			}
			flag = this.WriteAttributeSignature(attribute, flag);
			this.genericWriter.WriteToken(this.ClosingBracket);
			if (flag)
			{
				this.genericWriter.Write("    ");
				string str = this.genericWriter.Language.CommentLines("JustDecompile was unable to locate the assembly where attribute parameters types are defined. Generating parameters values is impossible.");
				this.genericWriter.Write(str.Remove(str.Length - 2));
			}
			if (!skipNewLine)
			{
				this.genericWriter.WriteLine();
			}
		}

		private void WriteAttributeArgumentArray(CustomAttributeArgument argument)
		{
			CustomAttributeArgument[] value = argument.get_Value() as CustomAttributeArgument[];
			this.genericWriter.WriteKeyword(this.genericWriter.KeyWordWriter.New);
			this.genericWriter.WriteSpace();
			string str = String.Format("{0}{1}{2}", (object)this.GetElementTypeName(argument), this.genericWriter.IndexLeftBracket, this.genericWriter.IndexRightBracket);
			this.genericWriter.WriteNamespaceIfTypeInCollision(argument.get_Type());
			this.genericWriter.WriteReference(str, argument.get_Type());
			this.genericWriter.WriteSpace();
			this.genericWriter.WriteToken("{");
			this.genericWriter.WriteSpace();
			for (int i = 0; i < (int)value.Length; i++)
			{
				this.WriteAttributeArgumentValue(value[i]);
				if (i + 1 < (int)value.Length)
				{
					this.genericWriter.WriteToken(",");
					this.genericWriter.WriteSpace();
				}
			}
			this.genericWriter.WriteSpace();
			this.genericWriter.WriteToken("}");
		}

		private void WriteAttributeArgumentValue(CustomAttributeArgument argument)
		{
			if (argument.get_Value() is CustomAttributeArgument)
			{
				this.WriteAttributeArgumentValue((CustomAttributeArgument)argument.get_Value());
				return;
			}
			if (argument.get_Value() is CustomAttributeArgument[])
			{
				this.WriteAttributeArgumentArray(argument);
				return;
			}
			TypeDefinition typeDefinition = (argument.get_Type().get_IsDefinition() ? argument.get_Type() as TypeDefinition : argument.get_Type().Resolve());
			if (typeDefinition == null || !typeDefinition.get_IsEnum())
			{
				if (argument.get_Type().get_Name() != "Type" || argument.get_Type().get_Namespace() != "System")
				{
					this.genericWriter.WriteLiteralInLanguageSyntax(argument.get_Value());
					return;
				}
				this.genericWriter.WriteKeyword(this.genericWriter.KeyWordWriter.TypeOf);
				this.genericWriter.WriteToken("(");
				this.genericWriter.WriteGenericReference(argument.get_Value() as TypeReference);
				this.genericWriter.WriteToken(")");
				return;
			}
			List<FieldDefinition> enumFieldDefinitionByValue = EnumValueToFieldCombinationMatcher.GetEnumFieldDefinitionByValue(typeDefinition.get_Fields(), argument.get_Value(), typeDefinition.get_CustomAttributes());
			if (enumFieldDefinitionByValue.Count == 0)
			{
				this.genericWriter.WriteLiteralInLanguageSyntax(argument.get_Value());
				return;
			}
			for (int i = 0; i < enumFieldDefinitionByValue.Count; i++)
			{
				this.genericWriter.WriteReferenceAndNamespaceIfInCollision(enumFieldDefinitionByValue[i].get_DeclaringType());
				this.genericWriter.WriteToken(".");
				this.genericWriter.WriteEnumValueField(enumFieldDefinitionByValue[i]);
				if (i + 1 < enumFieldDefinitionByValue.Count)
				{
					this.genericWriter.WriteSpace();
					this.genericWriter.WriteBitwiseOr();
					this.genericWriter.WriteSpace();
				}
			}
		}

		private bool WriteAttributeNamedArgs(TypeDefinition attributeType, Collection<CustomAttributeNamedArgument> namedArguments, bool fields, bool wroteArgument)
		{
			CustomAttributeNamedArgument item;
			IList properties;
			IList lists;
			for (int i = 0; i < namedArguments.get_Count(); i++)
			{
				if (i == 0 & wroteArgument)
				{
					this.genericWriter.WriteToken(",");
					this.genericWriter.WriteSpace();
				}
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
					if (memberReference == null)
					{
						NamespaceImperativeLanguageWriter namespaceImperativeLanguageWriter = this.genericWriter;
						item = namedArguments.get_Item(i);
						namespaceImperativeLanguageWriter.Write(item.get_Name());
					}
					else
					{
						this.genericWriter.WriteReference(memberReference.get_Name(), memberReference);
					}
				}
				else
				{
					NamespaceImperativeLanguageWriter namespaceImperativeLanguageWriter1 = this.genericWriter;
					item = namedArguments.get_Item(i);
					namespaceImperativeLanguageWriter1.Write(item.get_Name());
				}
				this.genericWriter.WriteToken(this.EqualsSign);
				item = namedArguments.get_Item(i);
				this.WriteAttributeArgumentValue(item.get_Argument());
				if (i + 1 < namedArguments.get_Count())
				{
					this.genericWriter.WriteToken(",");
					this.genericWriter.WriteSpace();
				}
				wroteArgument = true;
			}
			return wroteArgument;
		}

		private bool WriteAttributeSignature(CustomAttribute attribute, bool resolvingProblem)
		{
			string name = (attribute.get_AttributeType().get_Name().EndsWith("Attribute") ? attribute.get_AttributeType().get_Name().Remove(attribute.get_AttributeType().get_Name().LastIndexOf("Attribute")) : attribute.get_AttributeType().get_Name());
			if (this.genericWriter.Language.IsGlobalKeyword(name))
			{
				name = attribute.get_AttributeType().get_Name();
			}
			this.genericWriter.WriteNamespaceIfTypeInCollision(attribute.get_AttributeType());
			this.genericWriter.WriteReference(name, attribute.get_AttributeType());
			if (attribute.get_HasConstructorArguments() || attribute.get_HasFields() || attribute.get_HasProperties())
			{
				this.genericWriter.WriteToken("(");
				bool flag = false;
				for (int i = 0; i < attribute.get_ConstructorArguments().get_Count(); i++)
				{
					flag = true;
					this.WriteAttributeArgumentValue(attribute.get_ConstructorArguments().get_Item(i));
					if (i + 1 < attribute.get_ConstructorArguments().get_Count())
					{
						this.genericWriter.Write(",");
						this.genericWriter.WriteSpace();
					}
				}
				if (attribute.get_HasProperties())
				{
					TypeDefinition typeDefinition = attribute.get_AttributeType().Resolve();
					flag = this.WriteAttributeNamedArgs(typeDefinition, attribute.get_Properties(), false, flag);
				}
				if (attribute.get_HasFields())
				{
					TypeDefinition typeDefinition1 = attribute.get_AttributeType().Resolve();
					this.WriteAttributeNamedArgs(typeDefinition1, attribute.get_Fields(), true, flag);
				}
				this.genericWriter.WriteToken(")");
			}
			else if (!attribute.get_IsResolved() && (int)attribute.GetBlob().Length > 4)
			{
				this.genericWriter.WriteToken("(");
				this.genericWriter.Write(",");
				this.genericWriter.WriteToken(")");
				resolvingProblem = true;
			}
			return resolvingProblem;
		}

		protected void WriteAttributesInternal(IMemberDefinition member, List<ICustomAttribute> attributes, bool skipTheNewLine, bool areReturnValueAttributes)
		{
			bool flag;
			foreach (ICustomAttribute attribute in attributes)
			{
				if (attribute is CustomAttribute)
				{
					this.WriteAttribute(attribute as CustomAttribute, skipTheNewLine, areReturnValueAttributes);
				}
				else if (attribute is SecurityAttribute)
				{
					this.WriteSecurityAttribute(this.GetModuleDefinition(member), false, attribute as SecurityAttribute, this.securityAttributeToDeclaration[attribute as SecurityAttribute], out flag, skipTheNewLine, areReturnValueAttributes);
				}
				if (!(skipTheNewLine & areReturnValueAttributes))
				{
					continue;
				}
				this.genericWriter.WriteSpace();
			}
		}

		private IEnumerable<ICustomAttribute> WriteFieldDefinitionFieldsAsAttributes(FieldDefinition member)
		{
			List<ICustomAttribute> customAttributes = new List<ICustomAttribute>();
			if (member.get_IsNotSerialized())
			{
				customAttributes.Add(AttributesUtilities.GetFieldNotSerializedAttribute(member));
			}
			if (member.get_DeclaringType().get_IsExplicitLayout())
			{
				customAttributes.Add(AttributesUtilities.GetFieldFieldOffsetAttribute(member));
			}
			return customAttributes;
		}

		private void WriteGlobalAttribute(CustomAttribute attribute, string keyword)
		{
			if (this.attributesNotToShow.Contains(attribute.get_AttributeType().get_FullName()))
			{
				return;
			}
			bool flag = false;
			attribute.Resolve();
			this.genericWriter.WriteToken(this.OpeningBracket);
			this.genericWriter.WriteKeyword(keyword);
			this.genericWriter.Write(":");
			this.genericWriter.WriteSpace();
			flag = this.WriteAttributeSignature(attribute, flag);
			this.genericWriter.WriteToken(this.ClosingBracket);
			if (flag)
			{
				this.genericWriter.Write("    ");
				string str = this.genericWriter.Language.CommentLines("JustDecompile was unable to locate the assembly where attribute parameters types are defined. Generating parameters values is impossible.");
				this.genericWriter.Write(str.Remove(str.Length - 2));
			}
			this.genericWriter.WriteLine();
		}

		public virtual void WriteMemberAttributesAndNewLine(IMemberDefinition member, IEnumerable<string> ignored = null, bool isWinRTImplementation = false)
		{
			if (ignored != null)
			{
				foreach (string str in ignored)
				{
					this.attributesNotToShow.Add(str);
				}
			}
			this.WriteMemberAttributesInternal(member, isWinRTImplementation);
			if (ignored != null)
			{
				foreach (string str1 in ignored)
				{
					this.attributesNotToShow.Remove(str1);
				}
			}
		}

		protected virtual void WriteMemberAttributesInternal(IMemberDefinition member, bool isWinRTImplementation)
		{
			this.securityAttributeToDeclaration = new Dictionary<SecurityAttribute, SecurityDeclaration>();
			List<ICustomAttribute> customAttributes = this.CollectSecurityAttributes(member);
			foreach (CustomAttribute customAttribute in member.get_CustomAttributes())
			{
				customAttribute.Resolve();
				if (isWinRTImplementation && this.IsWinRTActivatableAttribute(customAttribute))
				{
					continue;
				}
				customAttributes.Add(customAttribute);
			}
			customAttributes.AddRange(this.WritePropertiesAsAttributes(member));
			this.SortAttributes(customAttributes);
			this.WriteAttributesInternal(member, customAttributes, false, false);
		}

		public abstract void WriteMemberReturnValueAttributes(IMemberDefinition member);

		private IEnumerable<ICustomAttribute> WriteMethodFieldsAsAttributes(MethodDefinition method)
		{
			List<ICustomAttribute> customAttributes = new List<ICustomAttribute>();
			if (method.get_HasPInvokeInfo())
			{
				customAttributes.Add(AttributesUtilities.GetMethodDllImportAttribute(method));
			}
			if (method.get_HasImplAttributes() && AttributesUtilities.ShouldWriteMethodImplAttribute(method))
			{
				customAttributes.Add(AttributesUtilities.GetMethodImplAttribute(method));
			}
			return customAttributes;
		}

		private void WriteModuleAttribute(CustomAttribute attr)
		{
			this.WriteGlobalAttribute(attr, this.genericWriter.KeyWordWriter.Module);
		}

		public void WriteModuleAttributes(ModuleDefinition module, ICollection<string> attributesToIgnore = null)
		{
			List<CustomAttribute> customAttributes = new List<CustomAttribute>();
			foreach (CustomAttribute customAttribute in module.get_CustomAttributes())
			{
				customAttribute.Resolve();
				customAttributes.Add(customAttribute);
			}
			customAttributes.Sort((CustomAttribute x, CustomAttribute y) => this.CompareAttributes(x, y));
			foreach (CustomAttribute customAttribute1 in customAttributes)
			{
				if (attributesToIgnore != null && attributesToIgnore.Contains(customAttribute1.get_AttributeType().get_FullName()))
				{
					continue;
				}
				this.WriteModuleAttribute(customAttribute1);
			}
		}

		public int WriteParameterAttributes(ParameterDefinition parameter, bool isWinRTMethodImplementation)
		{
			CustomAttribute inAttribute;
			CustomAttribute outAttribute;
			if (isWinRTMethodImplementation)
			{
				inAttribute = null;
			}
			else
			{
				inAttribute = this.GetInAttribute(parameter);
			}
			CustomAttribute customAttribute = inAttribute;
			if (isWinRTMethodImplementation)
			{
				outAttribute = null;
			}
			else
			{
				outAttribute = this.GetOutAttribute(parameter);
			}
			CustomAttribute customAttribute1 = outAttribute;
			List<CustomAttribute> customAttributes = new List<CustomAttribute>();
			if (customAttribute != null)
			{
				customAttributes.Add(customAttribute);
			}
			if (customAttribute1 != null)
			{
				customAttributes.Add(customAttribute1);
			}
			customAttributes.AddRange(parameter.get_CustomAttributes());
			int num = 0;
			foreach (CustomAttribute customAttribute2 in customAttributes)
			{
				if (this.attributesNotToShow.Contains(customAttribute2.get_AttributeType().get_FullName()))
				{
					continue;
				}
				if (num != 0)
				{
					this.genericWriter.Write(this.ParameterAttributeSeparator);
				}
				this.WriteAttribute(customAttribute2, true, false);
				num++;
			}
			return num;
		}

		protected virtual IEnumerable<ICustomAttribute> WritePropertiesAsAttributes(IMemberDefinition member)
		{
			List<ICustomAttribute> customAttributes = new List<ICustomAttribute>();
			if (member is FieldDefinition)
			{
				customAttributes.AddRange(this.WriteFieldDefinitionFieldsAsAttributes(member as FieldDefinition));
			}
			else if (member is TypeDefinition)
			{
				customAttributes.AddRange(this.WriteTypeDefinitionFieldsAsAttributes(member as TypeDefinition));
			}
			else if (member is MethodDefinition)
			{
				customAttributes.AddRange(this.WriteMethodFieldsAsAttributes(member as MethodDefinition));
			}
			return customAttributes;
		}

		protected abstract void WriteReturnValueAttributeKeyword();

		private bool WriteSecurityAttribute(ModuleDefinition module, bool isAssemblyDeclaration, SecurityAttribute attribute, SecurityDeclaration securityDeclaration, out bool wroteArgument, bool skipTheNewLine = false, bool isReturnValueAttribute = false)
		{
			this.genericWriter.WriteToken(this.OpeningBracket);
			if (isAssemblyDeclaration)
			{
				this.genericWriter.WriteKeyword(this.genericWriter.KeyWordWriter.Assembly);
				this.genericWriter.Write(":");
				this.genericWriter.WriteSpace();
			}
			else if (isReturnValueAttribute)
			{
				this.WriteReturnValueAttributeKeyword();
			}
			string str = (attribute.get_AttributeType().get_Name().EndsWith("Attribute") ? attribute.get_AttributeType().get_Name().Remove(attribute.get_AttributeType().get_Name().LastIndexOf("Attribute")) : attribute.get_AttributeType().get_Name());
			this.genericWriter.WriteNamespaceIfTypeInCollision(attribute.get_AttributeType());
			this.genericWriter.WriteReference(str, attribute.get_AttributeType());
			this.genericWriter.WriteToken("(");
			TypeReference securityActionTypeReference = securityDeclaration.GetSecurityActionTypeReference(module);
			TypeDefinition typeDefinition = (securityActionTypeReference.get_IsDefinition() ? securityActionTypeReference as TypeDefinition : securityActionTypeReference.Resolve());
			if (typeDefinition != null && typeDefinition.get_IsEnum())
			{
				List<FieldDefinition> enumFieldDefinitionByValue = EnumValueToFieldCombinationMatcher.GetEnumFieldDefinitionByValue(typeDefinition.get_Fields(), (Int32)securityDeclaration.get_Action(), typeDefinition.get_CustomAttributes());
				if (enumFieldDefinitionByValue.Count == 0)
				{
					this.WriteSecurityAttributeAction(securityDeclaration.get_Action());
				}
				else
				{
					for (int i = 0; i < enumFieldDefinitionByValue.Count; i++)
					{
						this.genericWriter.WriteReferenceAndNamespaceIfInCollision(enumFieldDefinitionByValue[i].get_DeclaringType());
						this.genericWriter.WriteToken(".");
						this.genericWriter.WriteEnumValueField(enumFieldDefinitionByValue[i]);
						if (i + 1 < enumFieldDefinitionByValue.Count)
						{
							this.genericWriter.WriteSpace();
							this.genericWriter.WriteBitwiseOr();
							this.genericWriter.WriteSpace();
						}
					}
				}
			}
			wroteArgument = true;
			if (attribute.get_HasFields() || attribute.get_HasProperties())
			{
				TypeDefinition typeDefinition1 = attribute.get_AttributeType().Resolve();
				if (attribute.get_HasProperties())
				{
					wroteArgument = this.WriteAttributeNamedArgs(typeDefinition1, attribute.get_Properties(), false, wroteArgument);
				}
				if (attribute.get_HasFields())
				{
					this.WriteAttributeNamedArgs(typeDefinition1, attribute.get_Fields(), true, wroteArgument);
				}
			}
			this.genericWriter.WriteToken(")");
			this.genericWriter.WriteToken(this.ClosingBracket);
			if (!skipTheNewLine)
			{
				this.genericWriter.WriteLine();
			}
			return wroteArgument;
		}

		private void WriteSecurityAttributeAction(SecurityAction action)
		{
			this.genericWriter.WriteSecurityDeclarationNamespaceIfNeeded();
			this.genericWriter.Write("SecurityAction");
			this.genericWriter.WriteToken(".");
			switch (action)
			{
				case 1:
				{
					this.genericWriter.Write("");
					return;
				}
				case 2:
				case 13:
				{
					this.genericWriter.Write("Demand");
					return;
				}
				case 3:
				case 11:
				{
					this.genericWriter.Write("Assert");
					return;
				}
				case 4:
				case 12:
				{
					this.genericWriter.Write("Deny");
					return;
				}
				case 5:
				{
					this.genericWriter.Write("PermitOnly");
					return;
				}
				case 6:
				case 14:
				{
					this.genericWriter.Write("LinkDemand");
					return;
				}
				case 7:
				case 15:
				{
					this.genericWriter.Write("InheritanceDemand");
					return;
				}
				case 8:
				{
					this.genericWriter.Write("RequestMinimum");
					return;
				}
				case 9:
				{
					this.genericWriter.Write("RequestOptional");
					return;
				}
				case 10:
				{
					this.genericWriter.Write("RequestRefuse");
					return;
				}
				default:
				{
					return;
				}
			}
		}

		private IEnumerable<ICustomAttribute> WriteTypeDefinitionFieldsAsAttributes(TypeDefinition member)
		{
			List<ICustomAttribute> customAttributes = new List<ICustomAttribute>();
			if (member.get_IsSerializable())
			{
				customAttributes.Add(AttributesUtilities.GetTypeSerializableAttribute(member));
			}
			if (member.get_IsExplicitLayout())
			{
				customAttributes.Add(AttributesUtilities.GetTypeExplicitLayoutAttribute(member));
			}
			return customAttributes;
		}
	}
}