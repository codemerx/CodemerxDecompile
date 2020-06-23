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
				foreach (SecurityDeclaration securityDeclaration in (member as ISecurityDeclarationMemberDefinition).SecurityDeclarations)
				{
					customAttributes.AddRange(this.GetSecurityDeclaration(securityDeclaration));
				}
			}
			return customAttributes;
		}

		private int CompareAttributes(ICustomAttribute x, ICustomAttribute y)
		{
			string name = x.AttributeType.Name;
			string str = y.AttributeType.Name;
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
			if (first == second)
			{
				return 0;
			}
			SecurityDeclaration item = this.securityAttributeToDeclaration[first];
			SecurityDeclaration securityDeclaration = this.securityAttributeToDeclaration[second];
			return first.CompareToSecurityAttribute(second, item, securityDeclaration);
		}

		private string GetElementTypeName(CustomAttributeArgument argument)
		{
			return this.genericWriter.ToEscapedTypeString((argument.Type as ArrayType).ElementType);
		}

		protected virtual CustomAttribute GetInAttribute(ParameterDefinition parameter)
		{
			if (parameter == null || !parameter.IsIn)
			{
				return null;
			}
			return this.GetInOrOutAttribute(parameter, true);
		}

		protected CustomAttribute GetInOrOutAttribute(ParameterDefinition parameter, bool isInAttribute)
		{
			ModuleDefinition module = parameter.Method.ReturnType.Module;
			IMetadataScope corlib = module.TypeSystem.Corlib;
			TypeReference typeReference = new TypeReference("System.Runtime.InteropServices", (isInAttribute ? "InAttribute" : "OutAttribute"), module, corlib);
			if (typeReference == null)
			{
				return null;
			}
			return new CustomAttribute(new MethodReference(".ctor", module.TypeSystem.Void, typeReference));
		}

		private ModuleDefinition GetModuleDefinition(IMemberDefinition member)
		{
			if (member is TypeDefinition)
			{
				return (member as TypeDefinition).Module;
			}
			return member.DeclaringType.Module;
		}

		protected virtual CustomAttribute GetOutAttribute(ParameterDefinition parameter)
		{
			if (parameter.IsOutParameter())
			{
				return null;
			}
			if (parameter == null || !parameter.IsOut)
			{
				return null;
			}
			return this.GetInOrOutAttribute(parameter, false);
		}

		private IEnumerable<ICustomAttribute> GetSecurityDeclaration(SecurityDeclaration securityDeclaration)
		{
			List<ICustomAttribute> customAttributes = new List<ICustomAttribute>();
			if (securityDeclaration.HasSecurityAttributes)
			{
				foreach (SecurityAttribute securityAttribute in securityDeclaration.SecurityAttributes)
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
			if (member != null && member.MethodReturnType.HasCustomAttributes)
			{
				customAttributes.AddRange(member.MethodReturnType.CustomAttributes);
			}
			this.SortAttributes(customAttributes);
			return customAttributes;
		}

		private bool IsWinRTActivatableAttribute(CustomAttribute attribute)
		{
			if (attribute.AttributeType.FullName != "Windows.Foundation.Metadata.ActivatableAttribute" || attribute.ConstructorArguments.Count != 2)
			{
				return false;
			}
			TypeDefinition value = attribute.ConstructorArguments[0].Value as TypeDefinition;
			if (value == null)
			{
				return false;
			}
			return value.IsWindowsRuntime;
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
			foreach (CustomAttribute customAttribute in assembly.CustomAttributes)
			{
				customAttribute.Resolve();
				customAttributes.Add(customAttribute);
			}
			if (assembly.HasSecurityDeclarations)
			{
				foreach (SecurityDeclaration securityDeclaration in assembly.SecurityDeclarations)
				{
					customAttributes.AddRange(this.GetSecurityDeclaration(securityDeclaration));
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
			customAttributes.Sort((ICustomAttribute x, ICustomAttribute y) => this.CompareAttributes(x, y));
			foreach (ICustomAttribute customAttribute1 in customAttributes)
			{
				if (attributesToIgnore != null && attributesToIgnore.Contains(customAttribute1.AttributeType.FullName))
				{
					continue;
				}
				if (!(customAttribute1 is CustomAttribute))
				{
					if (!(customAttribute1 is SecurityAttribute))
					{
						continue;
					}
					this.WriteSecurityAttribute(assembly.MainModule, true, customAttribute1 as SecurityAttribute, this.securityAttributeToDeclaration[customAttribute1 as SecurityAttribute], out flag, false, false);
				}
				else
				{
					this.WriteAssemblyAttribute(customAttribute1 as CustomAttribute);
				}
			}
		}

		protected void WriteAttribute(CustomAttribute attribute, bool skipNewLine = false, bool isReturnValueAtrribute = false)
		{
			if (this.attributesNotToShow.Contains(attribute.AttributeType.FullName))
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
			CustomAttributeArgument[] value = argument.Value as CustomAttributeArgument[];
			this.genericWriter.WriteKeyword(this.genericWriter.KeyWordWriter.New);
			this.genericWriter.WriteSpace();
			string str = String.Format("{0}{1}{2}", (object)this.GetElementTypeName(argument), this.genericWriter.IndexLeftBracket, this.genericWriter.IndexRightBracket);
			this.genericWriter.WriteNamespaceIfTypeInCollision(argument.Type);
			this.genericWriter.WriteReference(str, argument.Type);
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
			if (argument.Value is CustomAttributeArgument)
			{
				this.WriteAttributeArgumentValue((CustomAttributeArgument)argument.Value);
				return;
			}
			if (argument.Value is CustomAttributeArgument[])
			{
				this.WriteAttributeArgumentArray(argument);
				return;
			}
			TypeDefinition typeDefinition = (argument.Type.IsDefinition ? argument.Type as TypeDefinition : argument.Type.Resolve());
			if (typeDefinition == null || !typeDefinition.IsEnum)
			{
				if (argument.Type.Name != "Type" || argument.Type.Namespace != "System")
				{
					this.genericWriter.WriteLiteralInLanguageSyntax(argument.Value);
					return;
				}
				this.genericWriter.WriteKeyword(this.genericWriter.KeyWordWriter.TypeOf);
				this.genericWriter.WriteToken("(");
				this.genericWriter.WriteGenericReference(argument.Value as TypeReference);
				this.genericWriter.WriteToken(")");
				return;
			}
			List<FieldDefinition> enumFieldDefinitionByValue = EnumValueToFieldCombinationMatcher.GetEnumFieldDefinitionByValue(typeDefinition.Fields, argument.Value, typeDefinition.CustomAttributes);
			if (enumFieldDefinitionByValue.Count == 0)
			{
				this.genericWriter.WriteLiteralInLanguageSyntax(argument.Value);
				return;
			}
			for (int i = 0; i < enumFieldDefinitionByValue.Count; i++)
			{
				this.genericWriter.WriteReferenceAndNamespaceIfInCollision(enumFieldDefinitionByValue[i].DeclaringType);
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
			for (int i = 0; i < namedArguments.Count; i++)
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
					if (memberReference == null)
					{
						NamespaceImperativeLanguageWriter namespaceImperativeLanguageWriter = this.genericWriter;
						item = namedArguments[i];
						namespaceImperativeLanguageWriter.Write(item.Name);
					}
					else
					{
						this.genericWriter.WriteReference(memberReference.Name, memberReference);
					}
				}
				else
				{
					NamespaceImperativeLanguageWriter namespaceImperativeLanguageWriter1 = this.genericWriter;
					item = namedArguments[i];
					namespaceImperativeLanguageWriter1.Write(item.Name);
				}
				this.genericWriter.WriteToken(this.EqualsSign);
				item = namedArguments[i];
				this.WriteAttributeArgumentValue(item.Argument);
				if (i + 1 < namedArguments.Count)
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
			string name = (attribute.AttributeType.Name.EndsWith("Attribute") ? attribute.AttributeType.Name.Remove(attribute.AttributeType.Name.LastIndexOf("Attribute")) : attribute.AttributeType.Name);
			if (this.genericWriter.Language.IsGlobalKeyword(name))
			{
				name = attribute.AttributeType.Name;
			}
			this.genericWriter.WriteNamespaceIfTypeInCollision(attribute.AttributeType);
			this.genericWriter.WriteReference(name, attribute.AttributeType);
			if (attribute.HasConstructorArguments || attribute.HasFields || attribute.HasProperties)
			{
				this.genericWriter.WriteToken("(");
				bool flag = false;
				for (int i = 0; i < attribute.ConstructorArguments.Count; i++)
				{
					flag = true;
					this.WriteAttributeArgumentValue(attribute.ConstructorArguments[i]);
					if (i + 1 < attribute.ConstructorArguments.Count)
					{
						this.genericWriter.Write(",");
						this.genericWriter.WriteSpace();
					}
				}
				if (attribute.HasProperties)
				{
					TypeDefinition typeDefinition = attribute.AttributeType.Resolve();
					flag = this.WriteAttributeNamedArgs(typeDefinition, attribute.Properties, false, flag);
				}
				if (attribute.HasFields)
				{
					TypeDefinition typeDefinition1 = attribute.AttributeType.Resolve();
					this.WriteAttributeNamedArgs(typeDefinition1, attribute.Fields, true, flag);
				}
				this.genericWriter.WriteToken(")");
			}
			else if (!attribute.IsResolved && (int)attribute.GetBlob().Length > 4)
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
			if (member.IsNotSerialized)
			{
				customAttributes.Add(AttributesUtilities.GetFieldNotSerializedAttribute(member));
			}
			if (member.DeclaringType.IsExplicitLayout)
			{
				customAttributes.Add(AttributesUtilities.GetFieldFieldOffsetAttribute(member));
			}
			return customAttributes;
		}

		private void WriteGlobalAttribute(CustomAttribute attribute, string keyword)
		{
			if (this.attributesNotToShow.Contains(attribute.AttributeType.FullName))
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
			foreach (CustomAttribute customAttribute in member.CustomAttributes)
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
			if (method.HasPInvokeInfo)
			{
				customAttributes.Add(AttributesUtilities.GetMethodDllImportAttribute(method));
			}
			if (method.HasImplAttributes && AttributesUtilities.ShouldWriteMethodImplAttribute(method))
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
			foreach (CustomAttribute customAttribute in module.CustomAttributes)
			{
				customAttribute.Resolve();
				customAttributes.Add(customAttribute);
			}
			customAttributes.Sort((CustomAttribute x, CustomAttribute y) => this.CompareAttributes(x, y));
			foreach (CustomAttribute customAttribute1 in customAttributes)
			{
				if (attributesToIgnore != null && attributesToIgnore.Contains(customAttribute1.AttributeType.FullName))
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
			customAttributes.AddRange(parameter.CustomAttributes);
			int num = 0;
			foreach (CustomAttribute customAttribute2 in customAttributes)
			{
				if (this.attributesNotToShow.Contains(customAttribute2.AttributeType.FullName))
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
			string str = (attribute.AttributeType.Name.EndsWith("Attribute") ? attribute.AttributeType.Name.Remove(attribute.AttributeType.Name.LastIndexOf("Attribute")) : attribute.AttributeType.Name);
			this.genericWriter.WriteNamespaceIfTypeInCollision(attribute.AttributeType);
			this.genericWriter.WriteReference(str, attribute.AttributeType);
			this.genericWriter.WriteToken("(");
			TypeReference securityActionTypeReference = securityDeclaration.GetSecurityActionTypeReference(module);
			TypeDefinition typeDefinition = (securityActionTypeReference.IsDefinition ? securityActionTypeReference as TypeDefinition : securityActionTypeReference.Resolve());
			if (typeDefinition != null && typeDefinition.IsEnum)
			{
				List<FieldDefinition> enumFieldDefinitionByValue = EnumValueToFieldCombinationMatcher.GetEnumFieldDefinitionByValue(typeDefinition.Fields, (Int32)securityDeclaration.Action, typeDefinition.CustomAttributes);
				if (enumFieldDefinitionByValue.Count == 0)
				{
					this.WriteSecurityAttributeAction(securityDeclaration.Action);
				}
				else
				{
					for (int i = 0; i < enumFieldDefinitionByValue.Count; i++)
					{
						this.genericWriter.WriteReferenceAndNamespaceIfInCollision(enumFieldDefinitionByValue[i].DeclaringType);
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
			if (attribute.HasFields || attribute.HasProperties)
			{
				TypeDefinition typeDefinition1 = attribute.AttributeType.Resolve();
				if (attribute.HasProperties)
				{
					wroteArgument = this.WriteAttributeNamedArgs(typeDefinition1, attribute.Properties, false, wroteArgument);
				}
				if (attribute.HasFields)
				{
					this.WriteAttributeNamedArgs(typeDefinition1, attribute.Fields, true, wroteArgument);
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
				case SecurityAction.Request:
				{
					this.genericWriter.Write("");
					return;
				}
				case SecurityAction.Demand:
				case SecurityAction.NonCasDemand:
				{
					this.genericWriter.Write("Demand");
					return;
				}
				case SecurityAction.Assert:
				case SecurityAction.PreJitGrant:
				{
					this.genericWriter.Write("Assert");
					return;
				}
				case SecurityAction.Deny:
				case SecurityAction.PreJitDeny:
				{
					this.genericWriter.Write("Deny");
					return;
				}
				case SecurityAction.PermitOnly:
				{
					this.genericWriter.Write("PermitOnly");
					return;
				}
				case SecurityAction.LinkDemand:
				case SecurityAction.NonCasLinkDemand:
				{
					this.genericWriter.Write("LinkDemand");
					return;
				}
				case SecurityAction.InheritDemand:
				case SecurityAction.NonCasInheritance:
				{
					this.genericWriter.Write("InheritanceDemand");
					return;
				}
				case SecurityAction.RequestMinimum:
				{
					this.genericWriter.Write("RequestMinimum");
					return;
				}
				case SecurityAction.RequestOptional:
				{
					this.genericWriter.Write("RequestOptional");
					return;
				}
				case SecurityAction.RequestRefuse:
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
			if (member.IsSerializable)
			{
				customAttributes.Add(AttributesUtilities.GetTypeSerializableAttribute(member));
			}
			if (member.IsExplicitLayout)
			{
				customAttributes.Add(AttributesUtilities.GetTypeExplicitLayoutAttribute(member));
			}
			return customAttributes;
		}
	}
}