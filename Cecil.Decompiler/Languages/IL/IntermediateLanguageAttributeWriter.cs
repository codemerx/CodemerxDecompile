using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Collections.Generic;
using Mono.Cecil.Extensions;

namespace Telerik.JustDecompiler.Languages.IL
{
    public class IntermediateLanguageAttributeWriter : IntermediateLanguageWriter
    {
        ///Follows the format specified on page 98 from "Expert .NET 2.0 IL Assembler" by Serge Lidin

        private const string SyntaxProblemsDueToNotResolvedType = "Enum keyword might be missing. Please, locate the assembly where the type is defined.";

        public IntermediateLanguageAttributeWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
			: base(language, formatter, exceptionFormatter, settings) { }

        public void WriteAssemblyAttributes(AssemblyDefinition assembly, ICollection<string> attributesToIgnore = null)
        {
            WriteAssemblyDeclarationOpening(assembly);
            WriteKeywordAttributes(assembly);
            WriteSecurityAttributes(assembly);
            WriteCustomAttributes(assembly);
            WriteAssemblyDeclarationClosing();
        }

        private void WriteDot()
        {
            Write(".");
        }

        private void WriteAssemblyDeclarationOpening(AssemblyDefinition assembly)
        {
            WriteDot();
            WriteKeyword("assembly");
            WriteSpace();
            if (assembly.Name.IsRetargetable)
            {
                WriteKeyword("retargetable");
                WriteSpace();
            }
            Write(assembly.Name.Name);
            WriteLine();
            WriteOpenBreckets();
            WriteLine();
            Indent();
        }

        #region Assembly keyword attributes

        private void WriteKeywordAttributes(AssemblyDefinition assembly)
        {
            WriteHashAlgorithm(assembly);
            WriteLine();

            WriteLine();
            WriteAssemblyVersion(assembly);
            WriteLine();

            if (assembly.Name.HasPublicKey)
            {
                WriteLine();
                WriteAssemblyPublicKey(assembly);
                WriteLine();
            }
            if (assembly.Name.Culture != string.Empty)
            {
                WriteLine();
                WriteAssemblyLocale(assembly);
                WriteLine();
            }
        }

        private void WriteHashAlgorithm(AssemblyDefinition assembly)
        {
            WriteDot();
            WriteKeyword("hash algorithm");
            WriteSpace();
            WriteLiteral("0x");
            int hashAlgorithmCode = (int)assembly.Name.HashAlgorithm;
            WriteLiteral(hashAlgorithmCode.ToString("X8"));
            WriteSpace();
            Write(@"//");
            WriteSpace();
            Write(assembly.Name.HashAlgorithm.ToString());
        }

        private void WriteAssemblyLocale(AssemblyDefinition assembly)
        {
            WriteDot();
            WriteKeyword("locale");
            WriteSpace();
            WriteLiteral("\"");
            WriteLiteral(assembly.Name.Culture);
            WriteLiteral("\"");
        }

        private void WriteAssemblyVersion(AssemblyDefinition assembly)
        {
            WriteDot();
            WriteKeyword("ver");
            WriteSpace();

            WriteLiteral(assembly.Name.Version.Major.ToString());
            Write(":");
            WriteLiteral(assembly.Name.Version.Minor.ToString());
            Write(":");
            WriteLiteral(assembly.Name.Version.Build.ToString());
            Write(":");
            WriteLiteral(assembly.Name.Version.Revision.ToString());
        }

        private void WriteAssemblyPublicKey(AssemblyDefinition assembly)
        {
            WriteDot();
            WriteKeyword("publickey");
            WriteLine();
            Write("(");
            WriteLine();
            Indent();
            //foreach (byte b in assembly.Name.PublicKey)
            //{
            //    WriteLiteral(b.ToString("X2"));
            //    WriteSpace();
            //}
            WriteByteArray(assembly.Name.PublicKey);
            WriteLine();
            Outdent();
            Write(")");
        }

        #endregion

        #region Security declarations

        private void WriteSecurityAttributes(AssemblyDefinition assembly)
        {
            if (!assembly.HasSecurityDeclarations)
            {
                return;
            }
            List<SecurityDeclaration> declarations = new List<SecurityDeclaration>(assembly.SecurityDeclarations);
            declarations.Sort((x, y) => CompareSecurityDeclaration(x, y));
            foreach (SecurityDeclaration declaration in declarations)
            {
                WriteSecurityDeclaration(declaration);
                WriteLine();
            }
        }

        private int CompareSecurityDeclaration(SecurityDeclaration first, SecurityDeclaration second)
        {
            if (first == second)
            {
                return 0;
            }
            string firstKeyword = GetActionKeyword(first.Action);
            string secondKeyword = GetActionKeyword(second.Action);
            return firstKeyword.CompareTo(secondKeyword);
        }

        private void WriteSecurityDeclaration(SecurityDeclaration declaration)
        {
            WriteDot();
            WriteKeyword("permissionset");
            WriteSpace();

            string actionKeyword = GetActionKeyword(declaration.Action);
            WriteKeyword(actionKeyword);
            WriteSpace();
            Write("=");
            WriteLine();
            WriteOpenBreckets();
            WriteLine();
            Indent();

            if (declaration.HasSecurityAttributes) /// this should always be true, but just in case
            {
                List<SecurityAttribute> securityAttributes = new List<SecurityAttribute>(declaration.SecurityAttributes);
                securityAttributes.Sort((x, y) => x.CompareToSecurityAttribute(y, declaration, declaration));

                foreach (SecurityAttribute attr in securityAttributes)
                {
                    WriteSingleSecurityAttributes(attr);
                    WriteLine();
                }

            }
            Outdent();
            WriteEndBreckets();
        }

        private void WriteSingleSecurityAttributes(SecurityAttribute attribute)
        {
            WriteType(attribute.AttributeType, ILNameSyntax.TypeName);
            if (attribute.HasProperties)
            {
                WriteSpace();

                Write("=");
                WriteLine();
                WriteOpenBreckets();
                WriteLine();
                Indent();
                foreach (CustomAttributeNamedArgument property in attribute.Properties)
                {
                    WriteKeyword("property");
                    WriteSpace();
                    WriteType(property.Argument.Type, ILNameSyntax.TypeName);
                    WriteSpace();
                    WriteLiteral("'");
                    WriteLiteral(property.Name);
                    WriteLiteral("'");
                    WriteSpace();
                    Write("=");
                    WriteSpace();
                    WriteType(property.Argument.Type, ILNameSyntax.TypeName);
                    WriteSpace();
                    Write("(");
                    if (property.Argument.Value is bool)
                    {
                        WriteBooleanLiteral((bool)property.Argument.Value);
                    }
                    else if (property.Argument.Value is string)
                    {
                        WriteStringLiteral((string)property.Argument.Value);
                    }
                    else
                    {
                        WriteLiteral(property.Argument.Value.ToString());
                    }
                    Write(")");
                    WriteLine();
                }
                Outdent();
                WriteEndBreckets();
            }
        }
  
        private string GetActionKeyword(SecurityAction action)
        {
            /// Taken from page 358 from "Expert .NET 2.0 IL Assembler" by Serge Lidin
            switch (action)
            {
                case SecurityAction.Request:
                    return "request";
                case SecurityAction.Demand:
                    return "demand";
                case SecurityAction.Assert:
                    return "assert";
                case SecurityAction.Deny:
                    return "deny";
                case SecurityAction.PermitOnly:
                    return "permitonly";
                case SecurityAction.LinkDemand:
                    return "linkcheck";
                case SecurityAction.InheritDemand:
                    return "inheritcheck";
                case SecurityAction.RequestMinimum:
                    return "reqmin";
                case SecurityAction.RequestOptional:
                    return "reqopt";
                case SecurityAction.RequestRefuse:
                    return "reqrefuse";
                case SecurityAction.PreJitGrant:
                    return "prejitgrant";
                case SecurityAction.PreJitDeny:
                    return "prejitdeny";
                case SecurityAction.NonCasDemand:
                    return "noncasdemand";
                case SecurityAction.NonCasLinkDemand:
                    return "noncaslinkdemand";
                case SecurityAction.NonCasInheritance:
                    return "noncasinheritance";
                default:
                    return "";
            }
        }

        #endregion

        private void WriteBooleanLiteral(bool value)
        {
            if (value)
            {
                WriteLiteral("true");
            }
            else
            {
                WriteLiteral("false");
            }
        }

        private void WriteStringLiteral(string str)
        {
            Write("\'");
            WriteLiteral(str);
            Write("\'");
        }

        #region Custom attributes

        private void WriteCustomAttributes(AssemblyDefinition assembly)
        {
            List<CustomAttribute> customAttributesOrdered = new List<CustomAttribute>(assembly.CustomAttributes);
            foreach (var attr in customAttributesOrdered)
            {
                attr.Resolve();
            }

            customAttributesOrdered.Sort((x, y) => x.CompareToCustomAttribute(y, true));
            foreach (CustomAttribute custom in customAttributesOrdered)
            {
                WriteLine();
                WriteCustomAttribute(custom);
                WriteLine();
            }
        }

        private void WriteCustomAttribute(CustomAttribute custom)
        {
            custom.Resolve();
            WriteDot();
            WriteKeyword("custom");
            WriteSpace();
            WriteMethodReference(custom.Constructor, true);
            WriteSpace();
            Write("=");
            WriteLine();
            if (custom.HasConstructorArguments || custom.HasProperties || custom.HasFields)
            {
                WriteOpenBreckets();
                WriteLine();
                Indent();
                if (custom.HasConstructorArguments)
                {
                    WriteCustomAttributeConstructorArguments(custom.ConstructorArguments);
                }
                if (custom.HasProperties)
                {
                    WriteCustomAttributeProperties(custom);
                }
                if (custom.HasFields)
                {
                    WriteCustomAttributeFields(custom);
                }
                Outdent();
                WriteEndBreckets();
            }

            else
            {
                Byte[] blob = custom.GetBlob();
                Write("(");
                WriteLine();
                Indent();
                WriteByteArray(blob);
                WriteLine();
                Outdent();
                Write(")");
                WriteLine();
            }
        }
  
        private void WriteCustomAttributeFields(CustomAttribute attribute)
        {
            Collection<CustomAttributeNamedArgument> fields = attribute.Fields;
            foreach (CustomAttributeNamedArgument field in fields)
            {
                WriteKeyword("field");
                WriteSpace();
                TypeDefinition argumetType = field.Argument.Type.Resolve();

                ILNameSyntax syntax;
                if (field.Argument.Type.IsPrimitive || field.Argument.Type.FullName == "System.String")
                {
                    syntax = ILNameSyntax.ShortTypeName;
                }
                else
                {
                    syntax = ILNameSyntax.TypeName;
                }

                TypeReference actualAegumentValueType = field.Argument.Type;

                if (argumetType != null)
                {
                    if (argumetType.IsEnum)
                    {
                        actualAegumentValueType = GetEnumerationUnderlayingType(argumetType);
                        WriteKeyword("enum");
                        WriteSpace();
                        WriteType(field.Argument.Type, syntax);
                        syntax = ILNameSyntax.ShortTypeName;
                    }
                    else
                    {
                        WriteType(field.Argument.Type, syntax);
                    }
                }
                else
                {
                    WriteNotResolvedReference(field.Argument.Type.GetFriendlyFullName(Language), field.Argument.Type, SyntaxProblemsDueToNotResolvedType);
                }
                WriteSpace();
                FieldDefinition fieldReference = null;
                TypeDefinition attributeType = attribute.AttributeType.Resolve();
                if (attributeType != null)
                {
                    fieldReference = attributeType.Fields.First(x => x.Name == field.Name);
                }
                WriteReference(field.Name, fieldReference);
                WriteSpace();
                Write("=");
                WriteSpace();
                WriteType(actualAegumentValueType, syntax);
                Write("(");
                Write(field.Argument.Value.ToString());
                Write(")");
                WriteLine();
            }
        }
  
        private TypeReference GetEnumerationUnderlayingType(TypeDefinition argumetType)
        {
            var valueField = argumetType.Fields.First(x => x.Name == "value__");
            return valueField.FieldType;
        }
  
        private void WriteCustomAttributeProperties(CustomAttribute attribute)
        {
            foreach (CustomAttributeNamedArgument property in attribute.Properties)
            {
                WriteKeyword("property");
                WriteSpace();

                ILNameSyntax syntax;
                if (property.Argument.Type.IsPrimitive || property.Argument.Type.FullName == "System.String")
                {
                    syntax = ILNameSyntax.ShortTypeName;
                }
                else
                {
                    syntax = ILNameSyntax.TypeName;
                }

                WriteType(property.Argument.Type, syntax);
                WriteSpace();
                TypeDefinition attributeType = attribute.AttributeType.Resolve();
                PropertyDefinition propertyDef = null;
                if (attributeType != null)
                {
                    propertyDef = attributeType.Properties.First(x => x.Name == property.Name);
                }
                WriteReference(property.Name, propertyDef);
                WriteSpace();
                Write("=");
                WriteSpace();
                WriteType(property.Argument.Type, syntax);
                Write("(");
                if (property.Argument.Value is bool)
                {
                    WriteBooleanLiteral((bool)property.Argument.Value);
                }
                else if (property.Argument.Value is string)
                {
                    WriteStringLiteral((string)property.Argument.Value);
                }
                else
                {
                    WriteLiteral(property.Argument.Value.ToString());
                }
                Write(")");
                WriteLine();
            }
        }

        private void WriteCustomAttributeConstructorArguments(Collection<CustomAttributeArgument> constructorArguments)
        {
            foreach (CustomAttributeArgument argument in constructorArguments)
            {
                TypeReference argType = argument.Type;
                if (argType.IsArray)
                {
                    ILNameSyntax syntax;
                    if ((argType as ArrayType).ElementType.IsPrimitive || argType.FullName == "System.String")
                    {
                        syntax = ILNameSyntax.ShortTypeName;
                    }
                    else
                    {
                        syntax = ILNameSyntax.TypeName;
                    }
                    WriteType((argType as ArrayType).ElementType, syntax);
                    Write("[");
                    WriteLiteral((argument.Value as Array).Length.ToString());
                    Write("]");
                }
                else
                {
                    ILNameSyntax syntax = ILNameSyntax.TypeName;
                    if (argType.IsPrimitive || argType.FullName == "System.String")
                    {
                        syntax = ILNameSyntax.ShortTypeName;
                    }

                    TypeDefinition argTypeDefinition = argType.Resolve();
                    if (argTypeDefinition != null)
                    {
                        if (argTypeDefinition.IsEnum)
                        {
                            argType = GetEnumerationUnderlayingType(argTypeDefinition);
                            syntax = ILNameSyntax.ShortTypeName;
                        }

                        WriteType(argType, syntax);
                    }
                    else
                    {
                        WriteNotResolvedReference(argType.FullName, argType, SyntaxProblemsDueToNotResolvedType);
                    }
                }
                Write("(");
                WriteSpace();
                if (argument.Value is bool)
                {
                    WriteBooleanLiteral((bool)argument.Value);
                }
                else if (argument.Value is string)
                {
                    WriteStringLiteral((string)argument.Value);
                }
                else if (argument.Value is CustomAttributeArgument[])
                {
                    WriteArrayValues(argument.Value as CustomAttributeArgument[]);
                }
                else
                {
                    WriteLiteral(argument.Value.ToString());
                }
                WriteSpace();
                Write(")");
                WriteLine();
            }
        }
  
        private void WriteArrayValues(CustomAttributeArgument[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Value is bool)
                {
                    WriteBooleanLiteral((bool)array[i].Value);
                }
                else if (array[i].Value is string)
                {
                    WriteStringLiteral((string)array[i].Value);
                }
                else if (array[i].Value is CustomAttributeArgument[])
                {
                    WriteArrayValues(array[i].Value as CustomAttributeArgument[]);
                }
                else
                {
                    WriteLiteral(array[i].Value.ToString());
                }
                if (i < array.Length - 1)
                {
                    WriteSpace();
                }
            }
        }
  
        private void WriteByteArray(byte[] blob)
        {
            for (int i = 0; i < blob.Length; i++)
            {
                WriteLiteral(blob[i].ToString("X2"));
                WriteSpace();
                if ((i + 1) % 16 == 0 && i + 1 < blob.Length) /// 16 because ILDasm prints 16 bytes per line
                {
                    WriteLine();
                }
            }
        }

        private void WriteAssemblyDeclarationClosing()
        {
            Outdent();
            WriteLine();
            WriteEndBreckets();
        }

        #endregion

        private void WriteOpenBreckets()
        {
            this.formatter.WriteStartBlock();

            Write("{");
        }

        private void WriteEndBreckets()
        {
            Write("}");

            this.formatter.WriteEndBlock();
        }
    }
}
