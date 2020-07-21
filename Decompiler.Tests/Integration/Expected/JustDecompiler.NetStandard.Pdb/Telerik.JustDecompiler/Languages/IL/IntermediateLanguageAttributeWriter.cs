using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Languages.IL
{
	public class IntermediateLanguageAttributeWriter : IntermediateLanguageWriter
	{
		private const string SyntaxProblemsDueToNotResolvedType = "Enum keyword might be missing. Please, locate the assembly where the type is defined.";

		public IntermediateLanguageAttributeWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
		{
			base(language, formatter, exceptionFormatter, settings);
			return;
		}

		private int CompareSecurityDeclaration(SecurityDeclaration first, SecurityDeclaration second)
		{
			if ((object)first == (object)second)
			{
				return 0;
			}
			stackVariable5 = this.GetActionKeyword(first.get_Action());
			return stackVariable5.CompareTo(this.GetActionKeyword(second.get_Action()));
		}

		private string GetActionKeyword(SecurityAction action)
		{
			switch (action - 1)
			{
				case 0:
				{
					return "request";
				}
				case 1:
				{
					return "demand";
				}
				case 2:
				{
					return "assert";
				}
				case 3:
				{
					return "deny";
				}
				case 4:
				{
					return "permitonly";
				}
				case 5:
				{
					return "linkcheck";
				}
				case 6:
				{
					return "inheritcheck";
				}
				case 7:
				{
					return "reqmin";
				}
				case 8:
				{
					return "reqopt";
				}
				case 9:
				{
					return "reqrefuse";
				}
				case 10:
				{
					return "prejitgrant";
				}
				case 11:
				{
					return "prejitdeny";
				}
				case 12:
				{
					return "noncasdemand";
				}
				case 13:
				{
					return "noncaslinkdemand";
				}
				case 14:
				{
					return "noncasinheritance";
				}
			}
			return "";
		}

		private TypeReference GetEnumerationUnderlayingType(TypeDefinition argumetType)
		{
			stackVariable1 = argumetType.get_Fields();
			stackVariable2 = IntermediateLanguageAttributeWriter.u003cu003ec.u003cu003e9__20_0;
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable2 = new Func<FieldDefinition, bool>(IntermediateLanguageAttributeWriter.u003cu003ec.u003cu003e9.u003cGetEnumerationUnderlayingTypeu003eb__20_0);
				IntermediateLanguageAttributeWriter.u003cu003ec.u003cu003e9__20_0 = stackVariable2;
			}
			return stackVariable1.First<FieldDefinition>(stackVariable2).get_FieldType();
		}

		private void WriteArrayValues(CustomAttributeArgument[] array)
		{
			V_0 = 0;
			while (V_0 < (int)array.Length)
			{
				if (array[V_0].get_Value() as Boolean == false)
				{
					if (array[V_0].get_Value() as String == null)
					{
						if (array[V_0].get_Value() as CustomAttributeArgument[] == null)
						{
							this.WriteLiteral(array[V_0].get_Value().ToString());
						}
						else
						{
							this.WriteArrayValues(array[V_0].get_Value() as CustomAttributeArgument[]);
						}
					}
					else
					{
						this.WriteStringLiteral((String)array[V_0].get_Value());
					}
				}
				else
				{
					this.WriteBooleanLiteral((Boolean)array[V_0].get_Value());
				}
				if (V_0 < (int)array.Length - 1)
				{
					this.WriteSpace();
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		public void WriteAssemblyAttributes(AssemblyDefinition assembly, ICollection<string> attributesToIgnore = null)
		{
			this.WriteAssemblyDeclarationOpening(assembly);
			this.WriteKeywordAttributes(assembly);
			this.WriteSecurityAttributes(assembly);
			this.WriteCustomAttributes(assembly);
			this.WriteAssemblyDeclarationClosing();
			return;
		}

		private void WriteAssemblyDeclarationClosing()
		{
			this.Outdent();
			this.WriteLine();
			this.WriteEndBreckets();
			return;
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
			return;
		}

		private void WriteAssemblyLocale(AssemblyDefinition assembly)
		{
			this.WriteDot();
			this.WriteKeyword("locale");
			this.WriteSpace();
			this.WriteLiteral("\"");
			this.WriteLiteral(assembly.get_Name().get_Culture());
			this.WriteLiteral("\"");
			return;
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
			return;
		}

		private void WriteAssemblyVersion(AssemblyDefinition assembly)
		{
			this.WriteDot();
			this.WriteKeyword("ver");
			this.WriteSpace();
			V_0 = assembly.get_Name().get_Version().get_Major();
			this.WriteLiteral(V_0.ToString());
			this.Write(":");
			V_0 = assembly.get_Name().get_Version().get_Minor();
			this.WriteLiteral(V_0.ToString());
			this.Write(":");
			V_0 = assembly.get_Name().get_Version().get_Build();
			this.WriteLiteral(V_0.ToString());
			this.Write(":");
			V_0 = assembly.get_Name().get_Version().get_Revision();
			this.WriteLiteral(V_0.ToString());
			return;
		}

		private void WriteBooleanLiteral(bool value)
		{
			if (value)
			{
				this.WriteLiteral("true");
				return;
			}
			this.WriteLiteral("false");
			return;
		}

		private void WriteByteArray(byte[] blob)
		{
			V_0 = 0;
			while (V_0 < (int)blob.Length)
			{
				this.WriteLiteral(blob[V_0].ToString("X2"));
				this.WriteSpace();
				if (V_0 + 1 % 16 == 0 && V_0 + 1 < (int)blob.Length)
				{
					this.WriteLine();
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		private void WriteCustomAttribute(CustomAttribute custom)
		{
			custom.Resolve();
			this.WriteDot();
			this.WriteKeyword("custom");
			this.WriteSpace();
			this.WriteMethodReference(custom.get_Constructor(), true);
			this.WriteSpace();
			this.Write("=");
			this.WriteLine();
			if (!custom.get_HasConstructorArguments() && !custom.get_HasProperties() && !custom.get_HasFields())
			{
				V_0 = custom.GetBlob();
				this.Write("(");
				this.WriteLine();
				this.Indent();
				this.WriteByteArray(V_0);
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
			return;
		}

		private void WriteCustomAttributeConstructorArguments(Collection<CustomAttributeArgument> constructorArguments)
		{
			V_0 = constructorArguments.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = V_1.get_Type();
					if (!V_2.get_IsArray())
					{
						V_5 = 2;
						if (V_2.get_IsPrimitive() || String.op_Equality(V_2.get_FullName(), "System.String"))
						{
							V_5 = 3;
						}
						V_6 = V_2.Resolve();
						if (V_6 == null)
						{
							this.WriteNotResolvedReference(V_2.get_FullName(), V_2, "Enum keyword might be missing. Please, locate the assembly where the type is defined.");
						}
						else
						{
							if (V_6.get_IsEnum())
							{
								V_2 = this.GetEnumerationUnderlayingType(V_6);
								V_5 = 3;
							}
							this.WriteType(V_2, V_5);
						}
					}
					else
					{
						if ((V_2 as ArrayType).get_ElementType().get_IsPrimitive() || String.op_Equality(V_2.get_FullName(), "System.String"))
						{
							V_3 = 3;
						}
						else
						{
							V_3 = 2;
						}
						this.WriteType((V_2 as ArrayType).get_ElementType(), V_3);
						this.Write("[");
						V_4 = (V_1.get_Value() as Array).get_Length();
						this.WriteLiteral(V_4.ToString());
						this.Write("]");
					}
					this.Write("(");
					this.WriteSpace();
					if (V_1.get_Value() as Boolean == false)
					{
						if (V_1.get_Value() as String == null)
						{
							if (V_1.get_Value() as CustomAttributeArgument[] == null)
							{
								this.WriteLiteral(V_1.get_Value().ToString());
							}
							else
							{
								this.WriteArrayValues(V_1.get_Value() as CustomAttributeArgument[]);
							}
						}
						else
						{
							this.WriteStringLiteral((String)V_1.get_Value());
						}
					}
					else
					{
						this.WriteBooleanLiteral((Boolean)V_1.get_Value());
					}
					this.WriteSpace();
					this.Write(")");
					this.WriteLine();
				}
			}
			finally
			{
				V_0.Dispose();
			}
			return;
		}

		private void WriteCustomAttributeFields(CustomAttribute attribute)
		{
			V_0 = attribute.get_Fields().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = new IntermediateLanguageAttributeWriter.u003cu003ec__DisplayClass19_0();
					V_1.field = V_0.get_Current();
					this.WriteKeyword("field");
					this.WriteSpace();
					V_7 = V_1.field.get_Argument();
					V_2 = V_7.get_Type().Resolve();
					if (V_1.field.get_Argument().get_Type().get_IsPrimitive() || String.op_Equality(V_1.field.get_Argument().get_Type().get_FullName(), "System.String"))
					{
						V_3 = 3;
					}
					else
					{
						V_3 = 2;
					}
					V_7 = V_1.field.get_Argument();
					V_4 = V_7.get_Type();
					if (V_2 == null)
					{
						V_7 = V_1.field.get_Argument();
						stackVariable39 = V_7.get_Type().GetFriendlyFullName(this.get_Language());
						V_7 = V_1.field.get_Argument();
						this.WriteNotResolvedReference(stackVariable39, V_7.get_Type(), "Enum keyword might be missing. Please, locate the assembly where the type is defined.");
					}
					else
					{
						if (!V_2.get_IsEnum())
						{
							V_7 = V_1.field.get_Argument();
							this.WriteType(V_7.get_Type(), V_3);
						}
						else
						{
							V_4 = this.GetEnumerationUnderlayingType(V_2);
							this.WriteKeyword("enum");
							this.WriteSpace();
							V_7 = V_1.field.get_Argument();
							this.WriteType(V_7.get_Type(), V_3);
							V_3 = 3;
						}
					}
					this.WriteSpace();
					V_5 = null;
					V_6 = attribute.get_AttributeType().Resolve();
					if (V_6 != null)
					{
						V_5 = V_6.get_Fields().First<FieldDefinition>(new Func<FieldDefinition, bool>(V_1.u003cWriteCustomAttributeFieldsu003eb__0));
					}
					this.WriteReference(V_1.field.get_Name(), V_5);
					this.WriteSpace();
					this.Write("=");
					this.WriteSpace();
					this.WriteType(V_4, V_3);
					this.Write("(");
					V_7 = V_1.field.get_Argument();
					this.Write(V_7.get_Value().ToString());
					this.Write(")");
					this.WriteLine();
				}
			}
			finally
			{
				V_0.Dispose();
			}
			return;
		}

		private void WriteCustomAttributeProperties(CustomAttribute attribute)
		{
			V_0 = attribute.get_Properties().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = new IntermediateLanguageAttributeWriter.u003cu003ec__DisplayClass21_0();
					V_1.property = V_0.get_Current();
					this.WriteKeyword("property");
					this.WriteSpace();
					if (V_1.property.get_Argument().get_Type().get_IsPrimitive() || String.op_Equality(V_1.property.get_Argument().get_Type().get_FullName(), "System.String"))
					{
						V_2 = 3;
					}
					else
					{
						V_2 = 2;
					}
					V_5 = V_1.property.get_Argument();
					this.WriteType(V_5.get_Type(), V_2);
					this.WriteSpace();
					V_3 = attribute.get_AttributeType().Resolve();
					V_4 = null;
					if (V_3 != null)
					{
						V_4 = V_3.get_Properties().First<PropertyDefinition>(new Func<PropertyDefinition, bool>(V_1.u003cWriteCustomAttributePropertiesu003eb__0));
					}
					this.WriteReference(V_1.property.get_Name(), V_4);
					this.WriteSpace();
					this.Write("=");
					this.WriteSpace();
					V_5 = V_1.property.get_Argument();
					this.WriteType(V_5.get_Type(), V_2);
					this.Write("(");
					if (V_1.property.get_Argument().get_Value() as Boolean == false)
					{
						if (V_1.property.get_Argument().get_Value() as String == null)
						{
							V_5 = V_1.property.get_Argument();
							this.WriteLiteral(V_5.get_Value().ToString());
						}
						else
						{
							V_5 = V_1.property.get_Argument();
							this.WriteStringLiteral((String)V_5.get_Value());
						}
					}
					else
					{
						V_5 = V_1.property.get_Argument();
						this.WriteBooleanLiteral((Boolean)V_5.get_Value());
					}
					this.Write(")");
					this.WriteLine();
				}
			}
			finally
			{
				V_0.Dispose();
			}
			return;
		}

		private void WriteCustomAttributes(AssemblyDefinition assembly)
		{
			V_0 = new List<CustomAttribute>(assembly.get_CustomAttributes());
			V_1 = V_0.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_1.get_Current().Resolve();
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			stackVariable9 = V_0;
			stackVariable10 = IntermediateLanguageAttributeWriter.u003cu003ec.u003cu003e9__17_0;
			if (stackVariable10 == null)
			{
				dummyVar0 = stackVariable10;
				stackVariable10 = new Comparison<CustomAttribute>(IntermediateLanguageAttributeWriter.u003cu003ec.u003cu003e9.u003cWriteCustomAttributesu003eb__17_0);
				IntermediateLanguageAttributeWriter.u003cu003ec.u003cu003e9__17_0 = stackVariable10;
			}
			stackVariable9.Sort(stackVariable10);
			V_1 = V_0.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					this.WriteLine();
					this.WriteCustomAttribute(V_2);
					this.WriteLine();
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return;
		}

		private void WriteDot()
		{
			this.Write(".");
			return;
		}

		private void WriteEndBreckets()
		{
			this.Write("}");
			this.formatter.WriteEndBlock();
			return;
		}

		private void WriteHashAlgorithm(AssemblyDefinition assembly)
		{
			this.WriteDot();
			this.WriteKeyword("hash algorithm");
			this.WriteSpace();
			this.WriteLiteral("0x");
			V_0 = assembly.get_Name().get_HashAlgorithm();
			this.WriteLiteral(V_0.ToString("X8"));
			this.WriteSpace();
			this.Write("//");
			this.WriteSpace();
			V_1 = assembly.get_Name().get_HashAlgorithm();
			this.Write(V_1.ToString());
			return;
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
			if (String.op_Inequality(assembly.get_Name().get_Culture(), String.Empty))
			{
				this.WriteLine();
				this.WriteAssemblyLocale(assembly);
				this.WriteLine();
			}
			return;
		}

		private void WriteOpenBreckets()
		{
			this.formatter.WriteStartBlock();
			this.Write("{");
			return;
		}

		private void WriteSecurityAttributes(AssemblyDefinition assembly)
		{
			if (!assembly.get_HasSecurityDeclarations())
			{
				return;
			}
			stackVariable4 = new List<SecurityDeclaration>(assembly.get_SecurityDeclarations());
			stackVariable4.Sort(new Comparison<SecurityDeclaration>(this.u003cWriteSecurityAttributesu003eb__10_0));
			V_0 = stackVariable4.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.WriteSecurityDeclaration(V_1);
					this.WriteLine();
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private void WriteSecurityDeclaration(SecurityDeclaration declaration)
		{
			V_0 = new IntermediateLanguageAttributeWriter.u003cu003ec__DisplayClass12_0();
			V_0.declaration = declaration;
			this.WriteDot();
			this.WriteKeyword("permissionset");
			this.WriteSpace();
			this.WriteKeyword(this.GetActionKeyword(V_0.declaration.get_Action()));
			this.WriteSpace();
			this.Write("=");
			this.WriteLine();
			this.WriteOpenBreckets();
			this.WriteLine();
			this.Indent();
			if (V_0.declaration.get_HasSecurityAttributes())
			{
				stackVariable29 = new List<SecurityAttribute>(V_0.declaration.get_SecurityAttributes());
				stackVariable29.Sort(new Comparison<SecurityAttribute>(V_0.u003cWriteSecurityDeclarationu003eb__0));
				V_2 = stackVariable29.GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = V_2.get_Current();
						this.WriteSingleSecurityAttributes(V_3);
						this.WriteLine();
					}
				}
				finally
				{
					((IDisposable)V_2).Dispose();
				}
			}
			this.Outdent();
			this.WriteEndBreckets();
			return;
		}

		private void WriteSingleSecurityAttributes(SecurityAttribute attribute)
		{
			this.WriteType(attribute.get_AttributeType(), 2);
			if (attribute.get_HasProperties())
			{
				this.WriteSpace();
				this.Write("=");
				this.WriteLine();
				this.WriteOpenBreckets();
				this.WriteLine();
				this.Indent();
				V_0 = attribute.get_Properties().GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_1 = V_0.get_Current();
						this.WriteKeyword("property");
						this.WriteSpace();
						V_2 = V_1.get_Argument();
						this.WriteType(V_2.get_Type(), 2);
						this.WriteSpace();
						this.WriteLiteral("'");
						this.WriteLiteral(V_1.get_Name());
						this.WriteLiteral("'");
						this.WriteSpace();
						this.Write("=");
						this.WriteSpace();
						V_2 = V_1.get_Argument();
						this.WriteType(V_2.get_Type(), 2);
						this.WriteSpace();
						this.Write("(");
						if (V_1.get_Argument().get_Value() as Boolean == false)
						{
							if (V_1.get_Argument().get_Value() as String == null)
							{
								V_2 = V_1.get_Argument();
								this.WriteLiteral(V_2.get_Value().ToString());
							}
							else
							{
								V_2 = V_1.get_Argument();
								this.WriteStringLiteral((String)V_2.get_Value());
							}
						}
						else
						{
							V_2 = V_1.get_Argument();
							this.WriteBooleanLiteral((Boolean)V_2.get_Value());
						}
						this.Write(")");
						this.WriteLine();
					}
				}
				finally
				{
					V_0.Dispose();
				}
				this.Outdent();
				this.WriteEndBreckets();
			}
			return;
		}

		private void WriteStringLiteral(string str)
		{
			this.Write("'");
			this.WriteLiteral(str);
			this.Write("'");
			return;
		}
	}
}