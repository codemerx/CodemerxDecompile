using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Languages.IL
{
	public class IntermediateLanguageAttributeWriter : IntermediateLanguageWriter
	{
		private const string SyntaxProblemsDueToNotResolvedType = "Enum keyword might be missing. Please, locate the assembly where the type is defined.";

		public IntermediateLanguageAttributeWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings) : base(language, formatter, exceptionFormatter, settings)
		{
		}

		private int CompareSecurityDeclaration(SecurityDeclaration first, SecurityDeclaration second)
		{
			if ((object)first == (object)second)
			{
				return 0;
			}
			return this.GetActionKeyword(first.get_Action()).CompareTo(this.GetActionKeyword(second.get_Action()));
		}

		private string GetActionKeyword(SecurityAction action)
		{
			switch (action)
			{
				case 1:
				{
					return "request";
				}
				case 2:
				{
					return "demand";
				}
				case 3:
				{
					return "assert";
				}
				case 4:
				{
					return "deny";
				}
				case 5:
				{
					return "permitonly";
				}
				case 6:
				{
					return "linkcheck";
				}
				case 7:
				{
					return "inheritcheck";
				}
				case 8:
				{
					return "reqmin";
				}
				case 9:
				{
					return "reqopt";
				}
				case 10:
				{
					return "reqrefuse";
				}
				case 11:
				{
					return "prejitgrant";
				}
				case 12:
				{
					return "prejitdeny";
				}
				case 13:
				{
					return "noncasdemand";
				}
				case 14:
				{
					return "noncaslinkdemand";
				}
				case 15:
				{
					return "noncasinheritance";
				}
			}
			return "";
		}

		private TypeReference GetEnumerationUnderlayingType(TypeDefinition argumetType)
		{
			return argumetType.get_Fields().First<FieldDefinition>((FieldDefinition x) => x.get_Name() == "value__").get_FieldType();
		}

		private void WriteArrayValues(CustomAttributeArgument[] array)
		{
			for (int i = 0; i < (int)array.Length; i++)
			{
				if (array[i].get_Value() is Boolean)
				{
					this.WriteBooleanLiteral((Boolean)array[i].get_Value());
				}
				else if (array[i].get_Value() is String)
				{
					this.WriteStringLiteral((String)array[i].get_Value());
				}
				else if (!(array[i].get_Value() is CustomAttributeArgument[]))
				{
					this.WriteLiteral(array[i].get_Value().ToString());
				}
				else
				{
					this.WriteArrayValues(array[i].get_Value() as CustomAttributeArgument[]);
				}
				if (i < (int)array.Length - 1)
				{
					this.WriteSpace();
				}
			}
		}

		public void WriteAssemblyAttributes(AssemblyDefinition assembly, ICollection<string> attributesToIgnore = null)
		{
			this.WriteAssemblyDeclarationOpening(assembly);
			this.WriteKeywordAttributes(assembly);
			this.WriteSecurityAttributes(assembly);
			this.WriteCustomAttributes(assembly);
			this.WriteAssemblyDeclarationClosing();
		}

		private void WriteAssemblyDeclarationClosing()
		{
			this.Outdent();
			this.WriteLine();
			this.WriteEndBreckets();
		}

		private void WriteAssemblyDeclarationOpening(AssemblyDefinition assembly)
		{
			this.WriteDot();
			this.WriteKeyword("assembly");
			this.WriteSpace();
			if (assembly.get_Name().get_IsRetargetable())
			{
				this.WriteKeyword("retargetable");
				this.WriteSpace();
			}
			this.Write(assembly.get_Name().get_Name());
			this.WriteLine();
			this.WriteOpenBreckets();
			this.WriteLine();
			this.Indent();
		}

		private void WriteAssemblyLocale(AssemblyDefinition assembly)
		{
			this.WriteDot();
			this.WriteKeyword("locale");
			this.WriteSpace();
			this.WriteLiteral("\"");
			this.WriteLiteral(assembly.get_Name().get_Culture());
			this.WriteLiteral("\"");
		}

		private void WriteAssemblyPublicKey(AssemblyDefinition assembly)
		{
			this.WriteDot();
			this.WriteKeyword("publickey");
			this.WriteLine();
			this.Write("(");
			this.WriteLine();
			this.Indent();
			this.WriteByteArray(assembly.get_Name().get_PublicKey());
			this.WriteLine();
			this.Outdent();
			this.Write(")");
		}

		private void WriteAssemblyVersion(AssemblyDefinition assembly)
		{
			this.WriteDot();
			this.WriteKeyword("ver");
			this.WriteSpace();
			int major = assembly.get_Name().get_Version().Major;
			this.WriteLiteral(major.ToString());
			this.Write(":");
			major = assembly.get_Name().get_Version().Minor;
			this.WriteLiteral(major.ToString());
			this.Write(":");
			major = assembly.get_Name().get_Version().Build;
			this.WriteLiteral(major.ToString());
			this.Write(":");
			major = assembly.get_Name().get_Version().Revision;
			this.WriteLiteral(major.ToString());
		}

		private void WriteBooleanLiteral(bool value)
		{
			if (value)
			{
				this.WriteLiteral("true");
				return;
			}
			this.WriteLiteral("false");
		}

		private void WriteByteArray(byte[] blob)
		{
			for (int i = 0; i < (int)blob.Length; i++)
			{
				this.WriteLiteral(blob[i].ToString("X2"));
				this.WriteSpace();
				if ((i + 1) % 16 == 0 && i + 1 < (int)blob.Length)
				{
					this.WriteLine();
				}
			}
		}

		private void WriteCustomAttribute(CustomAttribute custom)
		{
			custom.Resolve();
			this.WriteDot();
			this.WriteKeyword("custom");
			this.WriteSpace();
			base.WriteMethodReference(custom.get_Constructor(), true);
			this.WriteSpace();
			this.Write("=");
			this.WriteLine();
			if (!custom.get_HasConstructorArguments() && !custom.get_HasProperties() && !custom.get_HasFields())
			{
				byte[] blob = custom.GetBlob();
				this.Write("(");
				this.WriteLine();
				this.Indent();
				this.WriteByteArray(blob);
				this.WriteLine();
				this.Outdent();
				this.Write(")");
				this.WriteLine();
				return;
			}
			this.WriteOpenBreckets();
			this.WriteLine();
			this.Indent();
			if (custom.get_HasConstructorArguments())
			{
				this.WriteCustomAttributeConstructorArguments(custom.get_ConstructorArguments());
			}
			if (custom.get_HasProperties())
			{
				this.WriteCustomAttributeProperties(custom);
			}
			if (custom.get_HasFields())
			{
				this.WriteCustomAttributeFields(custom);
			}
			this.Outdent();
			this.WriteEndBreckets();
		}

		private void WriteCustomAttributeConstructorArguments(Collection<CustomAttributeArgument> constructorArguments)
		{
			IntermediateLanguageWriter.ILNameSyntax lNameSyntax;
			foreach (CustomAttributeArgument constructorArgument in constructorArguments)
			{
				TypeReference type = constructorArgument.get_Type();
				if (!type.get_IsArray())
				{
					IntermediateLanguageWriter.ILNameSyntax lNameSyntax1 = IntermediateLanguageWriter.ILNameSyntax.TypeName;
					if (type.get_IsPrimitive() || type.get_FullName() == "System.String")
					{
						lNameSyntax1 = IntermediateLanguageWriter.ILNameSyntax.ShortTypeName;
					}
					TypeDefinition typeDefinition = type.Resolve();
					if (typeDefinition == null)
					{
						this.WriteNotResolvedReference(type.get_FullName(), type, "Enum keyword might be missing. Please, locate the assembly where the type is defined.");
					}
					else
					{
						if (typeDefinition.get_IsEnum())
						{
							type = this.GetEnumerationUnderlayingType(typeDefinition);
							lNameSyntax1 = IntermediateLanguageWriter.ILNameSyntax.ShortTypeName;
						}
						base.WriteType(type, lNameSyntax1);
					}
				}
				else
				{
					lNameSyntax = ((type as ArrayType).get_ElementType().get_IsPrimitive() || type.get_FullName() == "System.String" ? IntermediateLanguageWriter.ILNameSyntax.ShortTypeName : IntermediateLanguageWriter.ILNameSyntax.TypeName);
					base.WriteType((type as ArrayType).get_ElementType(), lNameSyntax);
					this.Write("[");
					int length = (constructorArgument.get_Value() as Array).Length;
					this.WriteLiteral(length.ToString());
					this.Write("]");
				}
				this.Write("(");
				this.WriteSpace();
				if (constructorArgument.get_Value() is Boolean)
				{
					this.WriteBooleanLiteral((Boolean)constructorArgument.get_Value());
				}
				else if (constructorArgument.get_Value() is String)
				{
					this.WriteStringLiteral((String)constructorArgument.get_Value());
				}
				else if (!(constructorArgument.get_Value() is CustomAttributeArgument[]))
				{
					this.WriteLiteral(constructorArgument.get_Value().ToString());
				}
				else
				{
					this.WriteArrayValues(constructorArgument.get_Value() as CustomAttributeArgument[]);
				}
				this.WriteSpace();
				this.Write(")");
				this.WriteLine();
			}
		}

		private void WriteCustomAttributeFields(CustomAttribute attribute)
		{
			IntermediateLanguageWriter.ILNameSyntax lNameSyntax;
			foreach (CustomAttributeNamedArgument field in attribute.get_Fields())
			{
				this.WriteKeyword("field");
				this.WriteSpace();
				CustomAttributeArgument argument = field.get_Argument();
				TypeDefinition typeDefinition = argument.get_Type().Resolve();
				lNameSyntax = (field.get_Argument().get_Type().get_IsPrimitive() || field.get_Argument().get_Type().get_FullName() == "System.String" ? IntermediateLanguageWriter.ILNameSyntax.ShortTypeName : IntermediateLanguageWriter.ILNameSyntax.TypeName);
				argument = field.get_Argument();
				TypeReference type = argument.get_Type();
				if (typeDefinition == null)
				{
					argument = field.get_Argument();
					string friendlyFullName = argument.get_Type().GetFriendlyFullName(base.Language);
					argument = field.get_Argument();
					this.WriteNotResolvedReference(friendlyFullName, argument.get_Type(), "Enum keyword might be missing. Please, locate the assembly where the type is defined.");
				}
				else if (!typeDefinition.get_IsEnum())
				{
					argument = field.get_Argument();
					base.WriteType(argument.get_Type(), lNameSyntax);
				}
				else
				{
					type = this.GetEnumerationUnderlayingType(typeDefinition);
					this.WriteKeyword("enum");
					this.WriteSpace();
					argument = field.get_Argument();
					base.WriteType(argument.get_Type(), lNameSyntax);
					lNameSyntax = IntermediateLanguageWriter.ILNameSyntax.ShortTypeName;
				}
				this.WriteSpace();
				FieldDefinition fieldDefinition = null;
				TypeDefinition typeDefinition1 = attribute.get_AttributeType().Resolve();
				if (typeDefinition1 != null)
				{
					fieldDefinition = typeDefinition1.get_Fields().First<FieldDefinition>((FieldDefinition x) => x.get_Name() == field.get_Name());
				}
				this.WriteReference(field.get_Name(), fieldDefinition);
				this.WriteSpace();
				this.Write("=");
				this.WriteSpace();
				base.WriteType(type, lNameSyntax);
				this.Write("(");
				argument = field.get_Argument();
				this.Write(argument.get_Value().ToString());
				this.Write(")");
				this.WriteLine();
			}
		}

		private void WriteCustomAttributeProperties(CustomAttribute attribute)
		{
			IntermediateLanguageWriter.ILNameSyntax lNameSyntax;
			foreach (CustomAttributeNamedArgument property in attribute.get_Properties())
			{
				this.WriteKeyword("property");
				this.WriteSpace();
				lNameSyntax = (property.get_Argument().get_Type().get_IsPrimitive() || property.get_Argument().get_Type().get_FullName() == "System.String" ? IntermediateLanguageWriter.ILNameSyntax.ShortTypeName : IntermediateLanguageWriter.ILNameSyntax.TypeName);
				CustomAttributeArgument argument = property.get_Argument();
				base.WriteType(argument.get_Type(), lNameSyntax);
				this.WriteSpace();
				TypeDefinition typeDefinition = attribute.get_AttributeType().Resolve();
				PropertyDefinition propertyDefinition = null;
				if (typeDefinition != null)
				{
					propertyDefinition = typeDefinition.get_Properties().First<PropertyDefinition>((PropertyDefinition x) => x.get_Name() == property.get_Name());
				}
				this.WriteReference(property.get_Name(), propertyDefinition);
				this.WriteSpace();
				this.Write("=");
				this.WriteSpace();
				argument = property.get_Argument();
				base.WriteType(argument.get_Type(), lNameSyntax);
				this.Write("(");
				if (property.get_Argument().get_Value() is Boolean)
				{
					argument = property.get_Argument();
					this.WriteBooleanLiteral((Boolean)argument.get_Value());
				}
				else if (!(property.get_Argument().get_Value() is String))
				{
					argument = property.get_Argument();
					this.WriteLiteral(argument.get_Value().ToString());
				}
				else
				{
					argument = property.get_Argument();
					this.WriteStringLiteral((String)argument.get_Value());
				}
				this.Write(")");
				this.WriteLine();
			}
		}

		private void WriteCustomAttributes(AssemblyDefinition assembly)
		{
			List<CustomAttribute> customAttributes = new List<CustomAttribute>(assembly.get_CustomAttributes());
			foreach (CustomAttribute customAttribute in customAttributes)
			{
				customAttribute.Resolve();
			}
			customAttributes.Sort((CustomAttribute x, CustomAttribute y) => x.CompareToCustomAttribute(y, true));
			foreach (CustomAttribute customAttribute1 in customAttributes)
			{
				this.WriteLine();
				this.WriteCustomAttribute(customAttribute1);
				this.WriteLine();
			}
		}

		private void WriteDot()
		{
			this.Write(".");
		}

		private void WriteEndBreckets()
		{
			this.Write("}");
			this.formatter.WriteEndBlock();
		}

		private void WriteHashAlgorithm(AssemblyDefinition assembly)
		{
			this.WriteDot();
			this.WriteKeyword("hash algorithm");
			this.WriteSpace();
			this.WriteLiteral("0x");
			int hashAlgorithm = assembly.get_Name().get_HashAlgorithm();
			this.WriteLiteral(hashAlgorithm.ToString("X8"));
			this.WriteSpace();
			this.Write("//");
			this.WriteSpace();
			AssemblyHashAlgorithm assemblyHashAlgorithm = assembly.get_Name().get_HashAlgorithm();
			this.Write(assemblyHashAlgorithm.ToString());
		}

		private void WriteKeywordAttributes(AssemblyDefinition assembly)
		{
			this.WriteHashAlgorithm(assembly);
			this.WriteLine();
			this.WriteLine();
			this.WriteAssemblyVersion(assembly);
			this.WriteLine();
			if (assembly.get_Name().get_HasPublicKey())
			{
				this.WriteLine();
				this.WriteAssemblyPublicKey(assembly);
				this.WriteLine();
			}
			if (assembly.get_Name().get_Culture() != String.Empty)
			{
				this.WriteLine();
				this.WriteAssemblyLocale(assembly);
				this.WriteLine();
			}
		}

		private void WriteOpenBreckets()
		{
			this.formatter.WriteStartBlock();
			this.Write("{");
		}

		private void WriteSecurityAttributes(AssemblyDefinition assembly)
		{
			if (!assembly.get_HasSecurityDeclarations())
			{
				return;
			}
			List<SecurityDeclaration> securityDeclarations = new List<SecurityDeclaration>(assembly.get_SecurityDeclarations());
			securityDeclarations.Sort((SecurityDeclaration x, SecurityDeclaration y) => this.CompareSecurityDeclaration(x, y));
			foreach (SecurityDeclaration securityDeclaration in securityDeclarations)
			{
				this.WriteSecurityDeclaration(securityDeclaration);
				this.WriteLine();
			}
		}

		private void WriteSecurityDeclaration(SecurityDeclaration declaration)
		{
			this.WriteDot();
			this.WriteKeyword("permissionset");
			this.WriteSpace();
			this.WriteKeyword(this.GetActionKeyword(declaration.get_Action()));
			this.WriteSpace();
			this.Write("=");
			this.WriteLine();
			this.WriteOpenBreckets();
			this.WriteLine();
			this.Indent();
			if (declaration.get_HasSecurityAttributes())
			{
				List<SecurityAttribute> securityAttributes = new List<SecurityAttribute>(declaration.get_SecurityAttributes());
				securityAttributes.Sort((SecurityAttribute x, SecurityAttribute y) => x.CompareToSecurityAttribute(y, declaration, declaration));
				foreach (SecurityAttribute securityAttribute in securityAttributes)
				{
					this.WriteSingleSecurityAttributes(securityAttribute);
					this.WriteLine();
				}
			}
			this.Outdent();
			this.WriteEndBreckets();
		}

		private void WriteSingleSecurityAttributes(SecurityAttribute attribute)
		{
			base.WriteType(attribute.get_AttributeType(), IntermediateLanguageWriter.ILNameSyntax.TypeName);
			if (attribute.get_HasProperties())
			{
				this.WriteSpace();
				this.Write("=");
				this.WriteLine();
				this.WriteOpenBreckets();
				this.WriteLine();
				this.Indent();
				foreach (CustomAttributeNamedArgument property in attribute.get_Properties())
				{
					this.WriteKeyword("property");
					this.WriteSpace();
					CustomAttributeArgument argument = property.get_Argument();
					base.WriteType(argument.get_Type(), IntermediateLanguageWriter.ILNameSyntax.TypeName);
					this.WriteSpace();
					this.WriteLiteral("'");
					this.WriteLiteral(property.get_Name());
					this.WriteLiteral("'");
					this.WriteSpace();
					this.Write("=");
					this.WriteSpace();
					argument = property.get_Argument();
					base.WriteType(argument.get_Type(), IntermediateLanguageWriter.ILNameSyntax.TypeName);
					this.WriteSpace();
					this.Write("(");
					if (property.get_Argument().get_Value() is Boolean)
					{
						argument = property.get_Argument();
						this.WriteBooleanLiteral((Boolean)argument.get_Value());
					}
					else if (!(property.get_Argument().get_Value() is String))
					{
						argument = property.get_Argument();
						this.WriteLiteral(argument.get_Value().ToString());
					}
					else
					{
						argument = property.get_Argument();
						this.WriteStringLiteral((String)argument.get_Value());
					}
					this.Write(")");
					this.WriteLine();
				}
				this.Outdent();
				this.WriteEndBreckets();
			}
		}

		private void WriteStringLiteral(string str)
		{
			this.Write("'");
			this.WriteLiteral(str);
			this.Write("'");
		}
	}
}