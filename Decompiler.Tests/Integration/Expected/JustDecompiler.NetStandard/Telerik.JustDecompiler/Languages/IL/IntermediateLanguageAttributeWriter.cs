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
			if (first == second)
			{
				return 0;
			}
			return this.GetActionKeyword(first.Action).CompareTo(this.GetActionKeyword(second.Action));
		}

		private string GetActionKeyword(SecurityAction action)
		{
			switch (action)
			{
				case SecurityAction.Request:
				{
					return "request";
				}
				case SecurityAction.Demand:
				{
					return "demand";
				}
				case SecurityAction.Assert:
				{
					return "assert";
				}
				case SecurityAction.Deny:
				{
					return "deny";
				}
				case SecurityAction.PermitOnly:
				{
					return "permitonly";
				}
				case SecurityAction.LinkDemand:
				{
					return "linkcheck";
				}
				case SecurityAction.InheritDemand:
				{
					return "inheritcheck";
				}
				case SecurityAction.RequestMinimum:
				{
					return "reqmin";
				}
				case SecurityAction.RequestOptional:
				{
					return "reqopt";
				}
				case SecurityAction.RequestRefuse:
				{
					return "reqrefuse";
				}
				case SecurityAction.PreJitGrant:
				{
					return "prejitgrant";
				}
				case SecurityAction.PreJitDeny:
				{
					return "prejitdeny";
				}
				case SecurityAction.NonCasDemand:
				{
					return "noncasdemand";
				}
				case SecurityAction.NonCasLinkDemand:
				{
					return "noncaslinkdemand";
				}
				case SecurityAction.NonCasInheritance:
				{
					return "noncasinheritance";
				}
			}
			return "";
		}

		private TypeReference GetEnumerationUnderlayingType(TypeDefinition argumetType)
		{
			return argumetType.Fields.First<FieldDefinition>((FieldDefinition x) => x.Name == "value__").FieldType;
		}

		private void WriteArrayValues(CustomAttributeArgument[] array)
		{
			for (int i = 0; i < (int)array.Length; i++)
			{
				if (array[i].Value is Boolean)
				{
					this.WriteBooleanLiteral((Boolean)array[i].Value);
				}
				else if (array[i].Value is String)
				{
					this.WriteStringLiteral((String)array[i].Value);
				}
				else if (!(array[i].Value is CustomAttributeArgument[]))
				{
					this.WriteLiteral(array[i].Value.ToString());
				}
				else
				{
					this.WriteArrayValues(array[i].Value as CustomAttributeArgument[]);
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
			if (assembly.Name.IsRetargetable)
			{
				this.WriteKeyword("retargetable");
				this.WriteSpace();
			}
			this.Write(assembly.Name.Name);
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
			this.WriteLiteral(assembly.Name.Culture);
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
			this.WriteByteArray(assembly.Name.PublicKey);
			this.WriteLine();
			this.Outdent();
			this.Write(")");
		}

		private void WriteAssemblyVersion(AssemblyDefinition assembly)
		{
			this.WriteDot();
			this.WriteKeyword("ver");
			this.WriteSpace();
			int major = assembly.Name.Version.Major;
			this.WriteLiteral(major.ToString());
			this.Write(":");
			major = assembly.Name.Version.Minor;
			this.WriteLiteral(major.ToString());
			this.Write(":");
			major = assembly.Name.Version.Build;
			this.WriteLiteral(major.ToString());
			this.Write(":");
			major = assembly.Name.Version.Revision;
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
			base.WriteMethodReference(custom.Constructor, true);
			this.WriteSpace();
			this.Write("=");
			this.WriteLine();
			if (!custom.HasConstructorArguments && !custom.HasProperties && !custom.HasFields)
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
			if (custom.HasConstructorArguments)
			{
				this.WriteCustomAttributeConstructorArguments(custom.ConstructorArguments);
			}
			if (custom.HasProperties)
			{
				this.WriteCustomAttributeProperties(custom);
			}
			if (custom.HasFields)
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
				TypeReference type = constructorArgument.Type;
				if (!type.IsArray)
				{
					IntermediateLanguageWriter.ILNameSyntax lNameSyntax1 = IntermediateLanguageWriter.ILNameSyntax.TypeName;
					if (type.IsPrimitive || type.FullName == "System.String")
					{
						lNameSyntax1 = IntermediateLanguageWriter.ILNameSyntax.ShortTypeName;
					}
					TypeDefinition typeDefinition = type.Resolve();
					if (typeDefinition == null)
					{
						this.WriteNotResolvedReference(type.FullName, type, "Enum keyword might be missing. Please, locate the assembly where the type is defined.");
					}
					else
					{
						if (typeDefinition.IsEnum)
						{
							type = this.GetEnumerationUnderlayingType(typeDefinition);
							lNameSyntax1 = IntermediateLanguageWriter.ILNameSyntax.ShortTypeName;
						}
						base.WriteType(type, lNameSyntax1);
					}
				}
				else
				{
					lNameSyntax = ((type as ArrayType).ElementType.IsPrimitive || type.FullName == "System.String" ? IntermediateLanguageWriter.ILNameSyntax.ShortTypeName : IntermediateLanguageWriter.ILNameSyntax.TypeName);
					base.WriteType((type as ArrayType).ElementType, lNameSyntax);
					this.Write("[");
					this.WriteLiteral((constructorArgument.Value as Array).Length.ToString());
					this.Write("]");
				}
				this.Write("(");
				this.WriteSpace();
				if (constructorArgument.Value is Boolean)
				{
					this.WriteBooleanLiteral((Boolean)constructorArgument.Value);
				}
				else if (constructorArgument.Value is String)
				{
					this.WriteStringLiteral((String)constructorArgument.Value);
				}
				else if (!(constructorArgument.Value is CustomAttributeArgument[]))
				{
					this.WriteLiteral(constructorArgument.Value.ToString());
				}
				else
				{
					this.WriteArrayValues(constructorArgument.Value as CustomAttributeArgument[]);
				}
				this.WriteSpace();
				this.Write(")");
				this.WriteLine();
			}
		}

		private void WriteCustomAttributeFields(CustomAttribute attribute)
		{
			IntermediateLanguageWriter.ILNameSyntax lNameSyntax;
			foreach (CustomAttributeNamedArgument field in attribute.Fields)
			{
				this.WriteKeyword("field");
				this.WriteSpace();
				CustomAttributeArgument argument = field.Argument;
				TypeDefinition typeDefinition = argument.Type.Resolve();
				lNameSyntax = (field.Argument.Type.IsPrimitive || field.Argument.Type.FullName == "System.String" ? IntermediateLanguageWriter.ILNameSyntax.ShortTypeName : IntermediateLanguageWriter.ILNameSyntax.TypeName);
				argument = field.Argument;
				TypeReference type = argument.Type;
				if (typeDefinition == null)
				{
					argument = field.Argument;
					string friendlyFullName = argument.Type.GetFriendlyFullName(base.Language);
					argument = field.Argument;
					this.WriteNotResolvedReference(friendlyFullName, argument.Type, "Enum keyword might be missing. Please, locate the assembly where the type is defined.");
				}
				else if (!typeDefinition.IsEnum)
				{
					argument = field.Argument;
					base.WriteType(argument.Type, lNameSyntax);
				}
				else
				{
					type = this.GetEnumerationUnderlayingType(typeDefinition);
					this.WriteKeyword("enum");
					this.WriteSpace();
					argument = field.Argument;
					base.WriteType(argument.Type, lNameSyntax);
					lNameSyntax = IntermediateLanguageWriter.ILNameSyntax.ShortTypeName;
				}
				this.WriteSpace();
				FieldDefinition fieldDefinition = null;
				TypeDefinition typeDefinition1 = attribute.AttributeType.Resolve();
				if (typeDefinition1 != null)
				{
					fieldDefinition = typeDefinition1.Fields.First<FieldDefinition>((FieldDefinition x) => x.Name == field.Name);
				}
				this.WriteReference(field.Name, fieldDefinition);
				this.WriteSpace();
				this.Write("=");
				this.WriteSpace();
				base.WriteType(type, lNameSyntax);
				this.Write("(");
				argument = field.Argument;
				this.Write(argument.Value.ToString());
				this.Write(")");
				this.WriteLine();
			}
		}

		private void WriteCustomAttributeProperties(CustomAttribute attribute)
		{
			IntermediateLanguageWriter.ILNameSyntax lNameSyntax;
			foreach (CustomAttributeNamedArgument property in attribute.Properties)
			{
				this.WriteKeyword("property");
				this.WriteSpace();
				lNameSyntax = (property.Argument.Type.IsPrimitive || property.Argument.Type.FullName == "System.String" ? IntermediateLanguageWriter.ILNameSyntax.ShortTypeName : IntermediateLanguageWriter.ILNameSyntax.TypeName);
				CustomAttributeArgument argument = property.Argument;
				base.WriteType(argument.Type, lNameSyntax);
				this.WriteSpace();
				TypeDefinition typeDefinition = attribute.AttributeType.Resolve();
				PropertyDefinition propertyDefinition = null;
				if (typeDefinition != null)
				{
					propertyDefinition = typeDefinition.Properties.First<PropertyDefinition>((PropertyDefinition x) => x.Name == property.Name);
				}
				this.WriteReference(property.Name, propertyDefinition);
				this.WriteSpace();
				this.Write("=");
				this.WriteSpace();
				argument = property.Argument;
				base.WriteType(argument.Type, lNameSyntax);
				this.Write("(");
				if (property.Argument.Value is Boolean)
				{
					argument = property.Argument;
					this.WriteBooleanLiteral((Boolean)argument.Value);
				}
				else if (!(property.Argument.Value is String))
				{
					argument = property.Argument;
					this.WriteLiteral(argument.Value.ToString());
				}
				else
				{
					argument = property.Argument;
					this.WriteStringLiteral((String)argument.Value);
				}
				this.Write(")");
				this.WriteLine();
			}
		}

		private void WriteCustomAttributes(AssemblyDefinition assembly)
		{
			List<CustomAttribute> customAttributes = new List<CustomAttribute>(assembly.CustomAttributes);
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
			this.WriteLiteral(assembly.Name.HashAlgorithm.ToString("X8"));
			this.WriteSpace();
			this.Write("//");
			this.WriteSpace();
			this.Write(assembly.Name.HashAlgorithm.ToString());
		}

		private void WriteKeywordAttributes(AssemblyDefinition assembly)
		{
			this.WriteHashAlgorithm(assembly);
			this.WriteLine();
			this.WriteLine();
			this.WriteAssemblyVersion(assembly);
			this.WriteLine();
			if (assembly.Name.HasPublicKey)
			{
				this.WriteLine();
				this.WriteAssemblyPublicKey(assembly);
				this.WriteLine();
			}
			if (assembly.Name.Culture != String.Empty)
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
			if (!assembly.HasSecurityDeclarations)
			{
				return;
			}
			List<SecurityDeclaration> securityDeclarations = new List<SecurityDeclaration>(assembly.SecurityDeclarations);
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
			this.WriteKeyword(this.GetActionKeyword(declaration.Action));
			this.WriteSpace();
			this.Write("=");
			this.WriteLine();
			this.WriteOpenBreckets();
			this.WriteLine();
			this.Indent();
			if (declaration.HasSecurityAttributes)
			{
				List<SecurityAttribute> securityAttributes = new List<SecurityAttribute>(declaration.SecurityAttributes);
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
			base.WriteType(attribute.AttributeType, IntermediateLanguageWriter.ILNameSyntax.TypeName);
			if (attribute.HasProperties)
			{
				this.WriteSpace();
				this.Write("=");
				this.WriteLine();
				this.WriteOpenBreckets();
				this.WriteLine();
				this.Indent();
				foreach (CustomAttributeNamedArgument property in attribute.Properties)
				{
					this.WriteKeyword("property");
					this.WriteSpace();
					CustomAttributeArgument argument = property.Argument;
					base.WriteType(argument.Type, IntermediateLanguageWriter.ILNameSyntax.TypeName);
					this.WriteSpace();
					this.WriteLiteral("'");
					this.WriteLiteral(property.Name);
					this.WriteLiteral("'");
					this.WriteSpace();
					this.Write("=");
					this.WriteSpace();
					argument = property.Argument;
					base.WriteType(argument.Type, IntermediateLanguageWriter.ILNameSyntax.TypeName);
					this.WriteSpace();
					this.Write("(");
					if (property.Argument.Value is Boolean)
					{
						argument = property.Argument;
						this.WriteBooleanLiteral((Boolean)argument.Value);
					}
					else if (!(property.Argument.Value is String))
					{
						argument = property.Argument;
						this.WriteLiteral(argument.Value.ToString());
					}
					else
					{
						argument = property.Argument;
						this.WriteStringLiteral((String)argument.Value);
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