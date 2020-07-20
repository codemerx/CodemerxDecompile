using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Languages.IL
{
	public class IntermediateLanguageWriter : BaseLanguageWriter
	{
		private readonly FlagsWriter flagsWriter;

		private MethodDefinition method;

		private Telerik.JustDecompiler.Languages.IL.CodeMappings CodeMappings
		{
			get;
			set;
		}

		public IntermediateLanguageWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
		{
			base(language, formatter, exceptionFormatter, settings);
			this.flagsWriter = new FlagsWriter(this);
			return;
		}

		private void Disassemble(MethodBody body, MemberMapping methodMapping)
		{
			V_0 = body.get_Method();
			if ((object)V_0.get_DeclaringType().get_Module().get_Assembly().get_EntryPoint() == (object)V_0)
			{
				this.WriteKeyword(".entrypoint");
				this.WriteLine();
			}
			if (V_0.get_Body().get_HasVariables())
			{
				this.WriteMethodVariables(V_0);
				this.WriteLine();
			}
			if (this.get_Settings().get_ShouldGenerateBlocks() && body.get_Instructions().get_Count() > 0)
			{
				V_1 = body.get_Instructions().get_Item(0);
				V_2 = this.GetBranchTargets(body.get_Instructions());
				this.WriteStructureBody(new ILBlock(body), V_2, ref V_1, methodMapping, V_0.get_Body().get_CodeSize());
				return;
			}
			V_3 = V_0.get_Body().get_Instructions().GetEnumerator();
			try
			{
				while (V_3.MoveNext())
				{
					V_4 = V_3.get_Current();
					this.WriteInstruction(V_4);
					if (methodMapping != null)
					{
						stackVariable27 = methodMapping.get_MemberCodeMappings();
						stackVariable28 = new SourceCodeMapping();
						stackVariable29 = new ILRange();
						stackVariable29.From = V_4.get_Offset();
						if (V_4.get_Next() == null)
						{
							stackVariable36 = V_0.get_Body().get_CodeSize();
						}
						else
						{
							stackVariable36 = V_4.get_Next().get_Offset();
						}
						stackVariable29.To = stackVariable36;
						stackVariable28.set_ILInstructionOffset(stackVariable29);
						stackVariable28.set_MemberMapping(methodMapping);
						stackVariable27.Add(stackVariable28);
					}
					this.WriteLine();
				}
			}
			finally
			{
				V_3.Dispose();
			}
			if (V_0.get_Body().get_HasExceptionHandlers())
			{
				this.WriteLine();
				V_5 = V_0.get_Body().get_ExceptionHandlers().GetEnumerator();
				try
				{
					while (V_5.MoveNext())
					{
						V_6 = V_5.get_Current();
						this.WriteExceptionHandler(V_6);
						this.WriteLine();
					}
				}
				finally
				{
					V_5.Dispose();
				}
			}
			return;
		}

		private string GetAssemblyQualifiedName(TypeReference type)
		{
			V_0 = type.get_Scope() as AssemblyNameReference;
			if (V_0 == null)
			{
				V_1 = type.get_Scope() as ModuleDefinition;
				if (V_1 != null)
				{
					V_0 = V_1.get_Assembly().get_Name();
				}
			}
			if (V_0 == null)
			{
				return type.get_FullName();
			}
			return String.Concat(type.get_FullName(), ", ", V_0.get_FullName());
		}

		private HashSet<int> GetBranchTargets(IEnumerable<Instruction> instructions)
		{
			V_0 = new HashSet<int>();
			V_1 = instructions.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					stackVariable6 = V_1.get_Current();
					V_2 = stackVariable6.get_Operand() as Instruction;
					if (V_2 != null)
					{
						dummyVar0 = V_0.Add(V_2.get_Offset());
					}
					V_3 = stackVariable6.get_Operand() as Instruction[];
					if (V_3 == null)
					{
						continue;
					}
					V_4 = V_3;
					V_5 = 0;
					while (V_5 < (int)V_4.Length)
					{
						dummyVar1 = V_0.Add(V_4[V_5].get_Offset());
						V_5 = V_5 + 1;
					}
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}

		private string GetFullName(object member)
		{
			V_0 = String.Empty;
			if (member as IMemberDefinition == null)
			{
				if (member as ParameterReference == null)
				{
					if (member as MemberReference == null)
					{
						if (this.get_Settings().get_RenameInvalidMembers())
						{
							stackVariable14 = this.get_Language().ReplaceInvalidCharactersInIdentifier(member.ToString());
						}
						else
						{
							stackVariable14 = member.ToString();
						}
						V_0 = stackVariable14;
					}
					else
					{
						if (this.get_Settings().get_RenameInvalidMembers())
						{
							stackVariable24 = ((MemberReference)member).GetFriendlyFullName(this.get_Language());
						}
						else
						{
							stackVariable24 = ((MemberReference)member).get_FullName();
						}
						V_0 = stackVariable24;
					}
				}
				else
				{
					if (this.get_Settings().get_RenameInvalidMembers())
					{
						stackVariable35 = this.get_Language().ReplaceInvalidCharactersInIdentifier(((ParameterReference)member).get_Name());
					}
					else
					{
						stackVariable35 = ((ParameterReference)member).get_Name();
					}
					V_0 = stackVariable35;
				}
			}
			else
			{
				if (this.get_Settings().get_RenameInvalidMembers())
				{
					stackVariable45 = ((IMemberDefinition)member).GetFriendlyFullName(this.get_Language());
				}
				else
				{
					stackVariable45 = ((IMemberDefinition)member).get_FullName();
				}
				V_0 = stackVariable45;
			}
			return V_0;
		}

		private string GetName(object member)
		{
			V_0 = String.Empty;
			if (member as IMemberDefinition == null)
			{
				if (member as ParameterReference == null)
				{
					if (member as MemberReference == null)
					{
						V_0 = member.ToString();
					}
					else
					{
						V_0 = ((MemberReference)member).get_Name();
					}
				}
				else
				{
					V_0 = ((ParameterReference)member).get_Name();
				}
			}
			else
			{
				V_0 = ((IMemberDefinition)member).get_Name();
			}
			return V_0;
		}

		private bool HasParameterAttributes(ParameterDefinition p)
		{
			if (p.get_HasConstant())
			{
				return true;
			}
			return p.get_HasCustomAttributes();
		}

		private string OffsetToString(int offset)
		{
			return String.Format("IL_{0:x4}", offset);
		}

		protected override void Write(EventDefinition event)
		{
			this.WriteEventDeclaration(event);
			V_0 = this.formatter.get_CurrentPosition();
			this.WriteStartBlock();
			dummyVar0 = this.WriteAttributes(event.get_CustomAttributes());
			this.WriteNestedMethod(".addon", event.get_AddMethod());
			this.WriteLine();
			this.WriteNestedMethod(".removeon", event.get_RemoveMethod());
			this.WriteNestedMethod(".fire", event.get_InvokeMethod());
			V_1 = event.get_OtherMethods().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					this.WriteNestedMethod(".other", V_2);
				}
			}
			finally
			{
				V_1.Dispose();
			}
			this.WriteEndBlock();
			this.currentWritingInfo.get_MemberDefinitionToFoldingPositionMap().set_Item(event, new OffsetSpan(V_0, this.formatter.get_CurrentPosition() - 1));
			return;
		}

		protected override void Write(PropertyDefinition property)
		{
			this.WritePropertyDeclaration(property);
			V_0 = this.formatter.get_CurrentPosition();
			this.WriteStartBlock();
			dummyVar0 = this.WriteAttributes(property.get_CustomAttributes());
			this.WriteNestedMethod(".get", property.get_GetMethod());
			if (property.get_GetMethod() != null)
			{
				this.WriteLine();
			}
			this.WriteNestedMethod(".set", property.get_SetMethod());
			if (property.get_SetMethod() != null)
			{
				this.WriteLine();
			}
			V_1 = false;
			V_2 = property.get_OtherMethods().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					if (V_1)
					{
						this.WriteLine();
					}
					this.WriteNestedMethod(".other", V_3);
					V_1 = true;
				}
			}
			finally
			{
				V_2.Dispose();
			}
			if (V_1)
			{
				this.WriteLine();
			}
			this.WriteEndBlock();
			this.currentWritingInfo.get_MemberDefinitionToFoldingPositionMap().set_Item(property, new OffsetSpan(V_0, this.formatter.get_CurrentPosition() - 1));
			return;
		}

		protected override void Write(FieldDefinition field)
		{
			if (this.TryWriteEnumField(field))
			{
				return;
			}
			this.WriteKeyWordPostSpace(".field");
			this.flagsWriter.WriteFieldVisibility(field);
			this.flagsWriter.WriteFieldFlags(field);
			if (field.get_HasMarshalInfo())
			{
				this.WriteMarshalInfo(field.get_MarshalInfo());
			}
			this.WriteTypeReference(field.get_FieldType(), 0);
			this.WriteSpace();
			V_0 = this.formatter.get_CurrentPosition();
			this.WriteReference(ILHelpers.Escape(field.get_Name()), field);
			V_1 = this.formatter.get_CurrentPosition() - 1;
			this.currentWritingInfo.get_MemberDeclarationToCodePostionMap().set_Item(field, new OffsetSpan(V_0, V_1));
			if (field.get_Attributes() & 0x100 == 0x100)
			{
				this.Write(String.Format(" at I_{0:x8}", field.get_RVA()));
			}
			if (field.get_HasConstant())
			{
				this.WriteSpace();
				this.WriteToken("=");
				this.WriteSpace();
				this.WriteConstant(field.get_Constant().get_Value());
			}
			if (field.get_HasCustomAttributes() && this.WriteAttributes(field.get_CustomAttributes()))
			{
				this.WriteLine();
			}
			return;
		}

		protected override void Write(MethodDefinition method)
		{
			this.WriteMethodDeclaration(method, false);
			V_0 = this.formatter.get_CurrentPosition();
			this.WriteStartBlock();
			this.method = method;
			this.WriteMethodBody(method);
			this.WriteEndBlock();
			this.currentWritingInfo.get_MemberDefinitionToFoldingPositionMap().set_Item(method, new OffsetSpan(V_0, this.formatter.get_CurrentPosition() - 1));
			return;
		}

		private void WriteArrayType(TypeReference type, IntermediateLanguageWriter.ILNameSyntax syntaxForElementTypes)
		{
			V_0 = (ArrayType)type;
			this.WriteTypeReference(V_0.get_ElementType(), syntaxForElementTypes);
			this.WriteToken("[");
			V_1 = 0;
			while (V_1 < V_0.get_Dimensions().get_Count())
			{
				if (V_1 > 0)
				{
					this.WriteTokenPostSpace(",");
				}
				V_2 = V_0.get_Dimensions().get_Item(V_1);
				this.WriteToken(V_2.ToString());
				V_1 = V_1 + 1;
			}
			this.WriteToken("]");
			return;
		}

		private bool WriteAttributes(Collection<CustomAttribute> attributes)
		{
			if (attributes.get_Count() == 0)
			{
				return false;
			}
			V_0 = attributes.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.WriteKeyWordPostSpace(".custom");
					this.WriteMethodReference(V_1.get_Constructor(), true);
					V_2 = V_1.GetBlob();
					if (V_2 != null)
					{
						this.WriteSpace();
						this.WriteToken("=");
						this.WriteSpace();
						this.WriteBlob(V_2);
					}
					this.WriteLine();
				}
			}
			finally
			{
				V_0.Dispose();
			}
			return true;
		}

		protected override void WriteBeginBlock(bool inline = false)
		{
			if (inline)
			{
				this.WriteOpeningBrace();
				this.WriteSpace();
				return;
			}
			this.WriteOpeningBraceAndNewLine();
			this.Indent();
			return;
		}

		private void WriteBlob(byte[] blob)
		{
			this.WriteToken("(");
			this.Indent();
			V_0 = 0;
			while (V_0 < (int)blob.Length)
			{
				if (V_0 % 16 != 0 || V_0 >= (int)blob.Length - 1)
				{
					this.WriteSpace();
				}
				else
				{
					this.WriteLine();
				}
				this.Write(blob[V_0].ToString("x2"));
				V_0 = V_0 + 1;
			}
			this.WriteLine();
			this.Outdent();
			this.WriteToken(")");
			return;
		}

		private void WriteConstant(object constant)
		{
			if (constant == null)
			{
				this.WriteKeyword("nullref");
				return;
			}
			V_0 = ILHelpers.PrimitiveTypeName(constant.GetType().get_FullName());
			if (V_0 == null || !String.op_Inequality(V_0, "string"))
			{
				this.WriteOperand(constant);
				return;
			}
			this.Write(V_0);
			this.WriteToken("(");
			V_1 = (float?)(constant as float?);
			V_2 = (double?)(constant as double?);
			if (!V_1.get_HasValue() || !Single.IsNaN(V_1.get_Value()) && !Single.IsInfinity(V_1.get_Value()))
			{
				if (!V_2.get_HasValue() || !Double.IsNaN(V_2.get_Value()) && !Double.IsInfinity(V_2.get_Value()))
				{
					this.WriteOperand(constant);
				}
				else
				{
					this.Write(String.Format("0x{0:x16}", BitConverter.DoubleToInt64Bits(V_2.get_Value())));
				}
			}
			else
			{
				this.Write(String.Format("0x{0:x8}", BitConverter.ToInt32(BitConverter.GetBytes(V_1.get_Value()), 0)));
			}
			this.WriteToken(")");
			return;
		}

		private void WriteDoubleOperand(double val)
		{
			if (val == 0)
			{
				this.WriteLiteral("0.0");
				return;
			}
			if (!Double.IsInfinity(val) && !Double.IsNaN(val))
			{
				this.WriteLiteral(val.ToString("R", CultureInfo.get_InvariantCulture()));
				return;
			}
			V_0 = BitConverter.GetBytes(val);
			this.WriteToken("(");
			V_1 = 0;
			while (V_1 < (int)V_0.Length)
			{
				if (V_1 > 0)
				{
					this.WriteSpace();
				}
				this.WriteLiteral(V_0[V_1].ToString("X2"));
				V_1 = V_1 + 1;
			}
			this.WriteToken(")");
			return;
		}

		protected override void WriteEndBlock(string statementName)
		{
			this.WriteToken("}");
			return;
		}

		private void WriteEndBlock()
		{
			this.Outdent();
			this.WriteToken("}");
			this.formatter.WriteEndBlock();
			return;
		}

		private void WriteEscaped(string identifier)
		{
			V_0 = ILHelpers.Escape(identifier);
			if (V_0.StartsWith("'"))
			{
				this.WriteKeyword(V_0);
				return;
			}
			this.Write(V_0);
			return;
		}

		protected override void WriteEventDeclaration(EventDefinition event)
		{
			this.WriteKeyWordPostSpace(".event");
			this.flagsWriter.WriteEventFlags(event);
			this.WriteTypeReference(event.get_EventType(), 2);
			this.WriteSpace();
			V_0 = this.formatter.get_CurrentPosition();
			this.WriteReference(ILHelpers.Escape(event.get_Name()), event);
			V_1 = this.formatter.get_CurrentPosition() - 1;
			this.currentWritingInfo.get_MemberDeclarationToCodePostionMap().set_Item(event, new OffsetSpan(V_0, V_1));
			return;
		}

		private void WriteExceptionHandler(ExceptionHandler exceptionHandler)
		{
			this.WriteKeyWordPostSpace("Try");
			this.WriteOffsetReference(exceptionHandler.get_TryStart());
			this.WriteToken("-");
			this.WriteOffsetReference(exceptionHandler.get_TryEnd());
			this.WriteSpace();
			this.WriteKeyword(exceptionHandler.get_HandlerType().ToString());
			if (exceptionHandler.get_FilterStart() != null)
			{
				this.WriteSpace();
				this.WriteOffsetReference(exceptionHandler.get_FilterStart());
				this.WriteSpace();
				this.WriteKeyword("handler");
				this.WriteSpace();
			}
			if (exceptionHandler.get_CatchType() != null)
			{
				this.WriteSpace();
				this.WriteTypeReference(exceptionHandler.get_CatchType(), 0);
			}
			this.WriteSpace();
			this.WriteOffsetReference(exceptionHandler.get_HandlerStart());
			this.WriteSpace();
			this.WriteOffsetReference(exceptionHandler.get_HandlerEnd());
			return;
		}

		private void WriteFieldReference(FieldReference field)
		{
			this.WriteTypeReference(field.get_FieldType(), 1);
			this.WriteSpace();
			this.WriteTypeReference(field.get_DeclaringType(), 2);
			this.WriteToken("::");
			this.WriteReference(ILHelpers.Escape(field.get_Name()), field);
			return;
		}

		private void WriteFloatOperand(float val)
		{
			if (val == 0f)
			{
				this.WriteLiteral("0.0");
				return;
			}
			if (!Single.IsInfinity(val) && !Single.IsNaN(val))
			{
				this.WriteLiteral(val.ToString("R", CultureInfo.get_InvariantCulture()));
				return;
			}
			V_0 = BitConverter.GetBytes(val);
			this.WriteToken("(");
			V_1 = 0;
			while (V_1 < (int)V_0.Length)
			{
				if (V_1 > 0)
				{
					this.WriteSpace();
				}
				this.WriteLiteral(V_0[V_1].ToString("X2"));
				V_1 = V_1 + 1;
			}
			this.WriteToken(")");
			return;
		}

		private void WriteGenericTypeParameter(GenericParameter gp)
		{
			if (!gp.get_HasReferenceTypeConstraint())
			{
				if (gp.get_HasNotNullableValueTypeConstraint())
				{
					this.WriteKeyWordPostSpace("valuetype");
				}
			}
			else
			{
				this.WriteKeyWordPostSpace("class");
			}
			if (gp.get_HasDefaultConstructorConstraint())
			{
				this.WriteKeyWordPostSpace(".ctor");
			}
			if (gp.get_HasConstraints())
			{
				this.WriteToken("(");
				V_0 = 0;
				while (V_0 < gp.get_Constraints().get_Count())
				{
					if (V_0 > 0)
					{
						this.WriteTokenPostSpace(",");
					}
					this.WriteTypeReference(gp.get_Constraints().get_Item(V_0), 2);
					V_0 = V_0 + 1;
				}
				this.WriteTokenPostSpace(")");
			}
			if (!gp.get_IsContravariant())
			{
				if (gp.get_IsCovariant())
				{
					this.WriteToken("+");
				}
			}
			else
			{
				this.WriteToken("-");
			}
			this.WriteEscaped(gp.get_Name());
			return;
		}

		private void WriteGenericTypeParameter(TypeReference type, IntermediateLanguageWriter.ILNameSyntax syntax)
		{
			this.WriteToken("!");
			if (((GenericParameter)type).get_Owner().get_GenericParameterType() == 1)
			{
				this.WriteToken("!");
			}
			if (!String.IsNullOrEmpty(type.get_Name()) && type.get_Name().get_Chars(0) != '!' && syntax != 1)
			{
				this.WriteReference(ILHelpers.Escape(type.get_Name()), null);
				return;
			}
			V_0 = ((GenericParameter)type).get_Position();
			this.WriteReference(V_0.ToString(), null);
			return;
		}

		private void WriteInstruction(Instruction instruction)
		{
			this.WriteDefinition(this.OffsetToString(instruction.get_Offset()), instruction);
			this.WriteTokenPostSpace(":");
			V_0 = instruction.get_OpCode();
			this.WriteReference(V_0.get_Name(), instruction.get_OpCode());
			if (instruction.get_Operand() != null)
			{
				this.WriteSpace();
				if (OpCode.op_Equality(instruction.get_OpCode(), OpCodes.Ldtoken))
				{
					if (instruction.get_Operand() as MethodReference == null)
					{
						if (instruction.get_Operand() as FieldReference != null)
						{
							this.WriteKeyWordPostSpace("field");
						}
					}
					else
					{
						this.WriteKeyWordPostSpace("method");
					}
				}
				this.WriteOperand(instruction.get_Operand());
			}
			return;
		}

		private void WriteKeyWordPostSpace(string keyWord)
		{
			this.WriteKeyword(keyWord);
			this.WriteSpace();
			return;
		}

		private void WriteKeyWordPreSpace(string keyWord)
		{
			this.WriteSpace();
			this.WriteKeyword(keyWord);
			return;
		}

		private void WriteLabelList(Instruction[] instructions)
		{
			this.WriteToken("(");
			V_0 = 0;
			while (V_0 < (int)instructions.Length)
			{
				if (V_0 != 0)
				{
					this.WriteTokenPostSpace(",");
				}
				this.WriteOffsetReference(instructions[V_0]);
				V_0 = V_0 + 1;
			}
			this.WriteToken(")");
			return;
		}

		private void WriteLiteralInQuotes(string literal)
		{
			this.WriteLiteral(String.Format("\"{0}\"", literal));
			return;
		}

		private void WriteMarshalInfo(MarshalInfo marshalInfo)
		{
			this.WriteKeyword("marshal");
			this.WriteToken("(");
			this.WriteNativeType(marshalInfo.get_NativeType(), marshalInfo);
			this.WriteToken("(");
			this.WriteSpace();
			return;
		}

		public override void WriteMemberNavigationName(object memberDefinition)
		{
			if (memberDefinition == null)
			{
				return;
			}
			if (memberDefinition as TypeReference != null)
			{
				V_0 = (TypeReference)memberDefinition;
				stackVariable120 = this.formatter;
				if (this.get_Settings().get_RenameInvalidMembers())
				{
					stackVariable129 = V_0.GetFriendlyTypeName(this.get_Language(), "<", ">");
				}
				else
				{
					stackVariable129 = V_0.get_Name();
				}
				stackVariable120.Write(stackVariable129);
				return;
			}
			V_1 = this.GetFullName(memberDefinition);
			stackVariable8 = new String[1];
			stackVariable8[0] = "::";
			V_2 = V_1.Split(stackVariable8, 1);
			if ((int)V_2.Length > 1)
			{
				V_1 = V_2[1];
			}
			if (memberDefinition as MethodDefinition != null)
			{
				V_3 = memberDefinition as MethodDefinition;
				stackVariable100 = this.formatter;
				stackVariable102 = V_1;
				if (this.get_Settings().get_RenameInvalidMembers())
				{
					stackVariable110 = V_3.get_ReturnType().GetFriendlyFullName(this.get_Language());
				}
				else
				{
					stackVariable110 = V_3.get_ReturnType().get_FullName();
				}
				stackVariable100.Write(String.Format("{0} : {1}", stackVariable102, stackVariable110));
				return;
			}
			if (memberDefinition as PropertyDefinition != null)
			{
				V_4 = memberDefinition as PropertyDefinition;
				stackVariable83 = this.formatter;
				stackVariable85 = V_1;
				if (this.get_Settings().get_RenameInvalidMembers())
				{
					stackVariable93 = V_4.get_PropertyType().GetFriendlyFullName(this.get_Language());
				}
				else
				{
					stackVariable93 = V_4.get_PropertyType().get_FullName();
				}
				stackVariable83.Write(String.Format("{0} : {1}", stackVariable85, stackVariable93));
				return;
			}
			if (memberDefinition as FieldDefinition != null)
			{
				V_5 = memberDefinition as FieldDefinition;
				stackVariable66 = this.formatter;
				stackVariable68 = V_1;
				if (this.get_Settings().get_RenameInvalidMembers())
				{
					stackVariable76 = V_5.get_FieldType().GetFriendlyFullName(this.get_Language());
				}
				else
				{
					stackVariable76 = V_5.get_FieldType().get_FullName();
				}
				stackVariable66.Write(String.Format("{0} : {1}", stackVariable68, stackVariable76));
				return;
			}
			if (memberDefinition as EventDefinition != null)
			{
				V_6 = memberDefinition as EventDefinition;
				stackVariable49 = this.formatter;
				stackVariable51 = V_1;
				if (this.get_Settings().get_RenameInvalidMembers())
				{
					stackVariable59 = V_6.get_EventType().GetFriendlyFullName(this.get_Language());
				}
				else
				{
					stackVariable59 = V_6.get_EventType().get_FullName();
				}
				stackVariable49.Write(String.Format("{0} : {1}", stackVariable51, stackVariable59));
				return;
			}
			if (memberDefinition as ParameterReference != null)
			{
				this.formatter.Write(((ParameterReference)memberDefinition).get_Name());
				return;
			}
			if (memberDefinition as MemberReference != null)
			{
				stackVariable30 = this.formatter;
				if (this.get_Settings().get_RenameInvalidMembers())
				{
					stackVariable38 = ((MemberReference)memberDefinition).GetFriendlyFullName(this.get_Language());
				}
				else
				{
					stackVariable38 = ((MemberReference)memberDefinition).get_FullName();
				}
				stackVariable30.Write(stackVariable38);
			}
			return;
		}

		public override void WriteMemberNavigationPathFullName(object member)
		{
			if (member == null)
			{
				return;
			}
			if (member as TypeReference != null)
			{
				this.formatter.Write(((TypeReference)member).get_Name());
				return;
			}
			stackVariable5 = this.GetName(member);
			stackVariable7 = new String[1];
			stackVariable7[0] = "::";
			V_0 = stackVariable5.Split(stackVariable7, 1);
			if ((int)V_0.Length > 1)
			{
				dummyVar0 = V_0[1];
			}
			if (member as ParameterReference != null)
			{
				this.formatter.Write(((ParameterReference)member).get_Name());
				return;
			}
			if (member as MemberReference != null)
			{
				this.formatter.Write(((MemberReference)member).GetFriendlyFullName(this.get_Language()));
			}
			return;
		}

		private void WriteMethodBody(MethodDefinition method)
		{
			dummyVar0 = this.WriteAttributes(method.get_CustomAttributes());
			if (method.get_HasOverrides())
			{
				V_0 = method.get_Overrides().GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_1 = V_0.get_Current();
						this.WriteKeyWordPostSpace(".override");
						this.WriteKeyWordPostSpace("method");
						this.WriteMethodReference(V_1, true);
						this.WriteLine();
					}
				}
				finally
				{
					V_0.Dispose();
				}
			}
			V_2 = method.get_Parameters().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					this.WriteParameterAttributes(V_3);
				}
			}
			finally
			{
				V_2.Dispose();
			}
			this.WriteSecurityDeclarations(method);
			if (method.get_HasBody())
			{
				V_4 = ILHelpers.CreateCodeMapping(method, this.get_CodeMappings());
				this.Disassemble(method.get_Body(), V_4);
			}
			return;
		}

		protected override void WriteMethodDeclaration(MethodDefinition method, bool writeDocumentation = false)
		{
			this.WriteKeyWordPostSpace(".method");
			this.flagsWriter.WriteMethodVisibility(method);
			this.flagsWriter.WriteMethodFlags(method);
			if (method.get_IsCompilerControlled())
			{
				this.WriteKeyWordPostSpace("privatescope");
			}
			if (method.get_Attributes() & 0x2000 == 0x2000)
			{
				this.WriteKeyword("pinvokeimpl");
				if (method.get_HasPInvokeInfo() && method.get_PInvokeInfo() != null)
				{
					this.WritePInvokeAttributes(method);
				}
				this.WriteSpace();
			}
			this.Indent();
			if (!method.get_ExplicitThis())
			{
				if (method.get_HasThis())
				{
					this.WriteKeyWordPostSpace("instance");
				}
			}
			else
			{
				this.WriteKeyWordPostSpace("instance");
				this.WriteKeyWordPostSpace("explicit");
			}
			this.flagsWriter.WriteMethodCallingConvention(method);
			this.WriteTypeReference(method.get_ReturnType(), 0);
			this.WriteSpace();
			if (method.get_MethodReturnType().get_HasMarshalInfo())
			{
				this.WriteMarshalInfo(method.get_MethodReturnType().get_MarshalInfo());
			}
			V_0 = this.formatter.get_CurrentPosition();
			this.WriteMethodName(method);
			V_1 = this.formatter.get_CurrentPosition() - 1;
			this.currentWritingInfo.get_MemberDeclarationToCodePostionMap().set_Item(method, new OffsetSpan(V_0, V_1));
			this.WriteTypeParameters(method);
			this.WriteMethodParameters(method);
			this.flagsWriter.WriteMethodCallType(method);
			this.WriteMethodManagedType(method);
			this.flagsWriter.WriteMethodImplementationAttribute(method);
			this.Outdent();
			return;
		}

		private void WriteMethodManagedType(MethodDefinition method)
		{
			// 
			// Current member / type: System.Void Telerik.JustDecompiler.Languages.IL.IntermediateLanguageWriter::WriteMethodManagedType(Mono.Cecil.MethodDefinition)
			// Exception in: System.Void WriteMethodManagedType(Mono.Cecil.MethodDefinition)
			// Object reference not set to an instance of an object.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		private void WriteMethodName(MethodDefinition method)
		{
			if (!method.get_IsCompilerControlled())
			{
				this.WriteReference(ILHelpers.Escape(method.get_Name()), method);
				return;
			}
			stackVariable9 = method.get_Name();
			V_1 = method.get_MetadataToken().ToInt32();
			this.WriteReference(ILHelpers.Escape(String.Concat(stackVariable9, "$PST", V_1.ToString("X8"))), method);
			return;
		}

		private void WriteMethodName(MethodReference method, bool writeAsReference)
		{
			V_0 = method as MethodDefinition;
			if (V_0 == null || !V_0.get_IsCompilerControlled())
			{
				V_1 = ILHelpers.Escape(method.get_Name());
			}
			else
			{
				stackVariable15 = method.get_Name();
				V_3 = method.get_MetadataToken().ToInt32();
				V_1 = ILHelpers.Escape(String.Concat(stackVariable15, "$PST", V_3.ToString("X8")));
			}
			if (!writeAsReference)
			{
				this.Write(V_1);
				return;
			}
			this.WriteReference(V_1, method);
			return;
		}

		private void WriteMethodParameters(MethodDefinition method)
		{
			this.WriteTokenPreSpace("(");
			if (method.get_HasParameters())
			{
				this.WriteLine();
				this.Indent();
				this.WriteParameters(method.get_Parameters());
				this.Outdent();
			}
			this.WriteTokenPostSpace(")");
			return;
		}

		protected void WriteMethodReference(MethodReference method, bool writeAsReference)
		{
			if (!method.get_ExplicitThis())
			{
				if (method.get_HasThis())
				{
					this.WriteKeyWordPostSpace("instance");
				}
			}
			else
			{
				this.WriteKeyWordPostSpace("instance");
				this.WriteKeyWordPostSpace("explicit");
			}
			this.WriteTypeReference(method.get_FixedReturnType(), 1);
			this.WriteSpace();
			if (method.get_DeclaringType() != null)
			{
				this.WriteTypeReference(method.get_DeclaringType(), 2);
				this.WriteToken("::");
			}
			this.WriteMethodName(method, writeAsReference);
			V_0 = method as GenericInstanceMethod;
			if (V_0 != null)
			{
				this.WriteToken("<");
				V_2 = 0;
				while (V_2 < V_0.get_GenericArguments().get_Count())
				{
					if (V_2 > 0)
					{
						this.WriteTokenPostSpace(",");
					}
					this.WriteTypeReference(V_0.get_GenericArguments().get_Item(V_2), 0);
					V_2 = V_2 + 1;
				}
				this.WriteToken(">");
			}
			this.WriteToken("(");
			V_1 = method.get_Parameters();
			V_3 = 0;
			while (V_3 < V_1.get_Count())
			{
				if (V_3 > 0)
				{
					this.WriteTokenPostSpace(", ");
				}
				this.WriteTypeReference(V_1.get_Item(V_3).get_ParameterType(), 1);
				V_3 = V_3 + 1;
			}
			this.WriteToken(")");
			return;
		}

		private void WriteMethodVariables(MethodDefinition method)
		{
			this.WriteKeyWordPostSpace(".locals");
			if (method.get_Body().get_InitLocals())
			{
				this.WriteKeyWordPostSpace("init");
			}
			this.WriteToken("(");
			this.WriteLine();
			this.Indent();
			V_0 = method.get_Body().get_Variables().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = V_1.get_Index();
					this.WriteDefinition(String.Concat("[", V_2.ToString(), "] "), V_1);
					this.WriteTypeReference(V_1.get_VariableType(), 0);
					if (!String.IsNullOrEmpty(V_1.get_Name()))
					{
						this.WriteSpace();
						this.Write(ILHelpers.Escape(V_1.get_Name()));
					}
					if (V_1.get_Index() + 1 < method.get_Body().get_Variables().get_Count())
					{
						this.WriteToken(",");
					}
					this.WriteLine();
				}
			}
			finally
			{
				V_0.Dispose();
			}
			this.Outdent();
			this.WriteToken(")");
			this.WriteLine();
			return;
		}

		private void WriteNativeType(NativeType nativeType, MarshalInfo marshalInfo = null)
		{
			switch (nativeType - 2)
			{
				case 0:
				{
					this.WriteKeyword("bool");
					return;
				}
				case 1:
				{
					this.WriteKeyword("int8");
					return;
				}
				case 2:
				{
					this.WriteKeyword("unsigned int8");
					return;
				}
				case 3:
				{
					this.WriteKeyword("int16");
					return;
				}
				case 4:
				{
					this.WriteKeyword("unsigned int16");
					return;
				}
				case 5:
				{
					this.WriteKeyword("int32");
					return;
				}
				case 6:
				{
					this.WriteKeyword("unsigned int32");
					return;
				}
				case 7:
				{
					this.WriteKeyword("int64");
					return;
				}
				case 8:
				{
					this.WriteKeyword("unsigned int64");
					return;
				}
				case 9:
				{
					this.WriteKeyword("float32");
					return;
				}
				case 10:
				{
					this.WriteKeyword("float64");
					return;
				}
				case 11:
				case 12:
				case 14:
				case 15:
				case 16:
				case 22:
				case 31:
				case 36:
				case 37:
				case 39:
				{
				Label1:
					this.WriteKeyword(nativeType.ToString());
					break;
				}
				case 13:
				{
					this.WriteKeyword("currency");
					return;
				}
				case 17:
				{
					this.WriteKeyword("bstr");
					return;
				}
				case 18:
				{
					this.WriteKeyword("lpstr");
					return;
				}
				case 19:
				{
					this.WriteKeyword("lpwstr");
					return;
				}
				case 20:
				{
					this.WriteKeyword("lptstr");
					return;
				}
				case 21:
				{
					this.WriteKeyword(String.Format("fixed sysstring[{0}]", ((FixedSysStringMarshalInfo)marshalInfo).get_Size()));
					return;
				}
				case 23:
				{
					this.WriteKeyword("iunknown");
					return;
				}
				case 24:
				{
					this.WriteKeyword("idispatch");
					return;
				}
				case 25:
				{
					this.WriteKeyword("struct");
					return;
				}
				case 26:
				{
					this.WriteKeyword("interface");
					return;
				}
				case 27:
				{
					this.WriteKeyWordPostSpace("safearray");
					V_1 = marshalInfo as SafeArrayMarshalInfo;
					if (V_1 == null)
					{
						break;
					}
					switch (V_1.get_ElementType())
					{
						case 0:
						{
							break;
						}
						case 1:
						case 15:
						case 20:
						case 21:
						{
						Label0:
							this.WriteKeyword(V_1.get_ElementType().ToString());
							return;
						}
						case 2:
						{
							this.WriteKeyword("int16");
							return;
						}
						case 3:
						{
							this.WriteKeyword("int32");
							return;
						}
						case 4:
						{
							this.WriteKeyword("float32");
							return;
						}
						case 5:
						{
							this.WriteKeyword("float64");
							return;
						}
						case 6:
						{
							this.WriteKeyword("currency");
							return;
						}
						case 7:
						{
							this.WriteKeyword("date");
							return;
						}
						case 8:
						{
							this.WriteKeyword("bstr");
							return;
						}
						case 9:
						{
							this.WriteKeyword("idispatch");
							return;
						}
						case 10:
						{
							this.WriteKeyword("error");
							return;
						}
						case 11:
						{
							this.WriteKeyword("bool");
							return;
						}
						case 12:
						{
							this.WriteKeyword("variant");
							return;
						}
						case 13:
						{
							this.WriteKeyword("iunknown");
							return;
						}
						case 14:
						{
							this.WriteKeyword("decimal");
							return;
						}
						case 16:
						{
							this.WriteKeyword("int8");
							return;
						}
						case 17:
						{
							this.WriteKeyword("unsigned int8");
							return;
						}
						case 18:
						{
							this.WriteKeyword("unsigned int16");
							return;
						}
						case 19:
						{
							this.WriteKeyword("unsigned int32");
							return;
						}
						case 22:
						{
							this.WriteKeyword("int");
							return;
						}
						case 23:
						{
							this.WriteKeyword("unsigned int");
							return;
						}
						default:
						{
							goto Label0;
						}
					}
					break;
				}
				case 28:
				{
					this.WriteKeyword("fixed array");
					V_2 = marshalInfo as FixedArrayMarshalInfo;
					if (V_2 == null)
					{
						break;
					}
					this.WriteToken("[");
					this.WriteLiteral(V_2.get_Size().ToString());
					this.WriteToken("]");
					if (V_2.get_ElementType() == 102)
					{
						break;
					}
					this.WriteSpace();
					this.WriteNativeType(V_2.get_ElementType(), null);
					return;
				}
				case 29:
				{
					this.WriteKeyword("int");
					return;
				}
				case 30:
				{
					this.WriteKeyword("unsigned int");
					return;
				}
				case 32:
				{
					this.WriteKeyword("byvalstr");
					return;
				}
				case 33:
				{
					this.WriteKeyword("ansi bstr");
					return;
				}
				case 34:
				{
					this.WriteKeyword("tbstr");
					return;
				}
				case 35:
				{
					this.WriteKeyword("variant bool");
					return;
				}
				case 38:
				{
					this.WriteKeyword("as any");
					return;
				}
				case 40:
				{
					V_0 = (ArrayMarshalInfo)marshalInfo;
					if (V_0 == null)
					{
						goto Label1;
					}
					if (V_0.get_ElementType() != 80)
					{
						this.WriteNativeType(V_0.get_ElementType(), null);
					}
					this.WriteToken("[");
					if (V_0.get_SizeParameterMultiplier() != 0)
					{
						if (V_0.get_Size() >= 0)
						{
							this.WriteLiteral(V_0.get_Size().ToString());
						}
						this.WriteSpace();
						this.WriteToken("+");
						this.WriteSpace();
						this.WriteLiteral(V_0.get_SizeParameterIndex().ToString());
					}
					else
					{
						this.WriteLiteral(V_0.get_Size().ToString());
					}
					this.WriteToken("]");
					return;
				}
				case 41:
				{
					this.WriteKeyword("lpstruct");
					return;
				}
				case 42:
				{
					V_3 = marshalInfo as CustomMarshalInfo;
					if (V_3 == null)
					{
						goto Label1;
					}
					this.WriteKeyword("custom");
					this.WriteToken("(");
					this.WriteLiteralInQuotes(BaseLanguageWriter.ConvertString(V_3.get_ManagedType().get_FullName()));
					this.WriteTokenPostSpace(",");
					this.WriteLiteralInQuotes(BaseLanguageWriter.ConvertString(V_3.get_Cookie()));
					if (Guid.op_Inequality(V_3.get_Guid(), Guid.Empty) || !String.IsNullOrEmpty(V_3.get_UnmanagedType()))
					{
						this.WriteTokenPostSpace(",");
						this.WriteLiteralInQuotes(V_3.get_Guid().ToString());
						this.WriteTokenPostSpace(",");
						this.WriteLiteralInQuotes(BaseLanguageWriter.ConvertString(V_3.get_UnmanagedType()));
					}
					this.WriteToken(")");
					return;
				}
				case 43:
				{
					this.WriteKeyword("error");
					return;
				}
				default:
				{
					if (nativeType == 102)
					{
						break;
					}
					goto Label1;
				}
			}
			return;
		}

		private void WriteNestedMethod(string keyword, MethodDefinition method)
		{
			if (method == null)
			{
				return;
			}
			this.WriteKeyWordPostSpace(keyword);
			this.WriteMethodReference(method, false);
			V_0 = this.formatter.get_CurrentPosition();
			this.WriteStartBlock();
			this.WriteMethodBody(method);
			this.WriteEndBlock();
			this.currentWritingInfo.get_MemberDefinitionToFoldingPositionMap().set_Item(method, new OffsetSpan(V_0, this.formatter.get_CurrentPosition() - 1));
			this.currentWritingInfo.get_MemberDeclarationToCodePostionMap().set_Item(method, new OffsetSpan(V_0, this.formatter.get_CurrentPosition() - 1));
			return;
		}

		private void WriteOffsetReference(Instruction instruction)
		{
			this.Write(this.OffsetToString(instruction.get_Offset()));
			return;
		}

		private void WriteOpeningBrace()
		{
			this.WriteToken("{");
			return;
		}

		private void WriteOpeningBraceAndNewLine()
		{
			this.WriteOpeningBrace();
			this.WriteLine();
			return;
		}

		private void WriteOpeningBraceBetweenLines()
		{
			this.WriteLine();
			this.WriteOpeningBraceAndNewLine();
			return;
		}

		private void WriteOperand(object operand)
		{
			if (operand == null)
			{
				throw new ArgumentNullException("operand");
			}
			V_0 = operand as Instruction;
			if (V_0 != null)
			{
				this.WriteOffsetReference(V_0);
				return;
			}
			V_1 = operand as Instruction[];
			if (V_1 != null)
			{
				this.WriteLabelList(V_1);
				return;
			}
			V_2 = operand as VariableReference;
			if (V_2 != null)
			{
				if (!String.IsNullOrEmpty(V_2.get_Name()))
				{
					this.WriteReference(ILHelpers.Escape(V_2.get_Name()), V_2);
					return;
				}
				V_8 = V_2.get_Index();
				this.WriteReference(V_8.ToString(), V_2);
				return;
			}
			V_3 = operand as ParameterReference;
			if (V_3 != null)
			{
				if (this.method != null && this.method.get_Body() != null && V_3 == this.method.get_Body().get_ThisParameter())
				{
					this.WriteReference(0.ToString(), V_3);
					return;
				}
				if (!String.IsNullOrEmpty(V_3.get_Name()))
				{
					this.WriteReference(ILHelpers.Escape(V_3.get_Name()), V_3);
					return;
				}
				V_8 = V_3.get_Index();
				this.WriteReference(V_8.ToString(), V_3);
				return;
			}
			V_4 = operand as MethodReference;
			if (V_4 != null)
			{
				this.WriteMethodReference(V_4, true);
				return;
			}
			V_5 = operand as TypeReference;
			if (V_5 != null)
			{
				this.WriteTypeReference(V_5, 2);
				return;
			}
			V_6 = operand as FieldReference;
			if (V_6 != null)
			{
				this.WriteFieldReference(V_6);
				return;
			}
			V_7 = operand as String;
			if (V_7 != null)
			{
				this.WriteLiteralInQuotes(BaseLanguageWriter.ConvertString(V_7));
				return;
			}
			if (operand as Char != 0)
			{
				this.WriteLiteral(((Char)operand).ToString());
				return;
			}
			if (operand as Single != 0)
			{
				this.WriteFloatOperand((Single)operand);
				return;
			}
			if (operand as Double != 0)
			{
				this.WriteDoubleOperand((Double)operand);
				return;
			}
			if (operand as Boolean == false)
			{
				V_7 = ILHelpers.ToInvariantCultureString(operand);
				this.Write(V_7);
				return;
			}
			if ((Boolean)operand)
			{
				stackVariable40 = "true";
			}
			else
			{
				stackVariable40 = "false";
			}
			this.WriteKeyword(stackVariable40);
			return;
		}

		private void WriteParameterAttributes(ParameterDefinition p)
		{
			if (!this.HasParameterAttributes(p))
			{
				return;
			}
			this.WriteKeyWordPostSpace(".param");
			this.WriteToken(String.Format("[{0}]", p.get_Index() + 1));
			if (p.get_HasConstant())
			{
				this.WriteSpace();
				this.WriteToken("=");
				this.WriteSpace();
				this.WriteConstant(p.get_Constant().get_Value());
			}
			this.WriteLine();
			dummyVar0 = this.WriteAttributes(p.get_CustomAttributes());
			return;
		}

		private void WriteParameters(Collection<ParameterDefinition> parameters)
		{
			V_0 = 0;
			while (V_0 < parameters.get_Count())
			{
				V_1 = parameters.get_Item(V_0);
				if (V_1.get_IsIn())
				{
					this.WriteKeyWordPostSpace("[in]");
				}
				if (V_1.get_IsOut())
				{
					this.WriteKeyWordPostSpace("[out]");
				}
				if (V_1.get_IsOptional())
				{
					this.WriteKeyWordPostSpace("[opt]");
				}
				this.WriteTypeReference(V_1.get_ParameterType(), 0);
				this.WriteSpace();
				if (V_1.get_HasMarshalInfo())
				{
					this.WriteMarshalInfo(V_1.get_MarshalInfo());
				}
				this.WriteEscaped(V_1.get_Name());
				if (V_0 < parameters.get_Count() - 1)
				{
					this.WriteToken(",");
				}
				this.WriteLine();
				V_0 = V_0 + 1;
			}
			return;
		}

		private void WritePInvokeAttributes(MethodDefinition method)
		{
			V_0 = method.get_PInvokeInfo();
			this.WriteToken("(");
			this.WriteLiteralInQuotes(BaseLanguageWriter.ConvertString(V_0.get_Module().get_Name()));
			this.WriteToken(")");
			if (!String.IsNullOrEmpty(V_0.get_EntryPoint()) && String.op_Inequality(V_0.get_EntryPoint(), method.get_Name()))
			{
				this.WriteSpace();
				this.WriteKeyword("as");
				this.WriteSpace();
				this.WriteLiteralInQuotes(BaseLanguageWriter.ConvertString(V_0.get_EntryPoint()));
			}
			if (V_0.get_IsNoMangle())
			{
				this.WriteKeyWordPreSpace("nomangle");
			}
			if (!V_0.get_IsCharSetAnsi())
			{
				if (!V_0.get_IsCharSetAuto())
				{
					if (V_0.get_IsCharSetUnicode())
					{
						this.WriteKeyWordPreSpace("unicode");
					}
				}
				else
				{
					this.WriteKeyWordPreSpace("autochar");
				}
			}
			else
			{
				this.WriteKeyWordPreSpace("ansi");
			}
			if (V_0.get_SupportsLastError())
			{
				this.WriteKeyWordPreSpace("lasterr");
			}
			if (!V_0.get_IsCallConvCdecl())
			{
				if (!V_0.get_IsCallConvFastcall())
				{
					if (!V_0.get_IsCallConvStdCall())
					{
						if (!V_0.get_IsCallConvThiscall())
						{
							if (V_0.get_IsCallConvWinapi())
							{
								this.WriteKeyWordPreSpace("winapi");
							}
						}
						else
						{
							this.WriteKeyWordPreSpace("thiscall");
						}
					}
					else
					{
						this.WriteKeyWordPreSpace("stdcall");
					}
				}
				else
				{
					this.WriteKeyWordPreSpace("fastcall");
				}
			}
			else
			{
				this.WriteKeyWordPreSpace("cdecl");
			}
			this.WriteToken(")");
			return;
		}

		protected override void WritePropertyDeclaration(PropertyDefinition property)
		{
			this.WriteKeyWordPostSpace(".property");
			this.flagsWriter.WritePropertyFlags(property);
			if (property.get_HasThis())
			{
				this.WriteKeyWordPostSpace("instance");
			}
			this.WriteTypeReference(property.get_PropertyType(), 0);
			this.WriteSpace();
			V_0 = this.formatter.get_CurrentPosition();
			this.WriteReference(ILHelpers.Escape(property.get_Name()), property);
			V_1 = this.formatter.get_CurrentPosition() - 1;
			this.currentWritingInfo.get_MemberDeclarationToCodePostionMap().set_Item(property, new OffsetSpan(V_0, V_1));
			this.WriteToken("(");
			if (property.get_HasParameters())
			{
				this.WriteLine();
				this.Indent();
				this.WriteParameters(property.get_Parameters());
				this.Outdent();
			}
			this.WriteToken(")");
			return;
		}

		private void WriteSecurityDeclarationArgument(CustomAttributeNamedArgument na)
		{
			V_1 = na.get_Argument();
			V_0 = V_1.get_Type();
			if (V_0.get_MetadataType() == 18 || V_0.get_MetadataType() == 17)
			{
				this.WriteKeyWordPostSpace("enum");
				if (V_0.get_Scope() == V_0.get_Module())
				{
					this.WriteTypeReference(V_0, 2);
				}
				else
				{
					this.WriteKeyWordPostSpace("class");
					this.Write(ILHelpers.Escape(this.GetAssemblyQualifiedName(V_0)));
				}
			}
			else
			{
				this.WriteTypeReference(V_0, 0);
			}
			this.WriteSpace();
			this.WriteReference(ILHelpers.Escape(na.get_Name()), V_0);
			this.WriteSpace();
			this.WriteToken("=");
			this.WriteSpace();
			if (na.get_Argument().get_Value() as String == null)
			{
				V_1 = na.get_Argument();
				this.WriteConstant(V_1.get_Value());
				return;
			}
			V_1 = na.get_Argument();
			this.Write(String.Format("string('{0}')", BaseLanguageWriter.ConvertString((String)V_1.get_Value()).Replace("'", "'")));
			return;
		}

		private void WriteSecurityDeclarations(ISecurityDeclarationProvider secDeclProvider)
		{
			if (!secDeclProvider.get_HasSecurityDeclarations())
			{
				return;
			}
			V_0 = secDeclProvider.get_SecurityDeclarations().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.WriteKeyWordPostSpace(".permissionset");
					switch (V_1.get_Action() - 1)
					{
						case 0:
						{
							this.WriteKeyword("request");
							break;
						}
						case 1:
						{
							this.WriteKeyword("demand");
							break;
						}
						case 2:
						{
							this.WriteKeyword("assert");
							break;
						}
						case 3:
						{
							this.WriteKeyword("deny");
							break;
						}
						case 4:
						{
							this.WriteKeyword("permitonly");
							break;
						}
						case 5:
						{
							this.WriteKeyword("linkcheck");
							break;
						}
						case 6:
						{
							this.WriteKeyword("inheritcheck");
							break;
						}
						case 7:
						{
							this.WriteKeyword("reqmin");
							break;
						}
						case 8:
						{
							this.WriteKeyword("reqopt");
							break;
						}
						case 9:
						{
							this.WriteKeyword("reqrefuse");
							break;
						}
						case 10:
						{
							this.WriteKeyword("prejitgrant");
							break;
						}
						case 11:
						{
							this.WriteKeyword("prejitdeny");
							break;
						}
						case 12:
						{
							this.WriteKeyword("noncasdemand");
							break;
						}
						case 13:
						{
							this.WriteKeyword("noncaslinkdemand");
							break;
						}
						case 14:
						{
							this.WriteKeyword("noncasinheritance");
							break;
						}
						default:
						{
							this.WriteKeyword(V_1.get_Action().ToString());
							break;
						}
					}
					this.WriteTokenPreSpace("=");
					this.WriteTokenPreSpace("{");
					this.WriteLine();
					this.Indent();
					V_4 = 0;
					while (V_4 < V_1.get_SecurityAttributes().get_Count())
					{
						V_5 = V_1.get_SecurityAttributes().get_Item(V_4);
						if (V_5.get_AttributeType().get_Scope() != V_5.get_AttributeType().get_Module())
						{
							this.WriteTypeReference(V_5.get_AttributeType(), 2);
						}
						else
						{
							this.WriteKeyWordPostSpace("class");
							this.WriteEscaped(this.GetAssemblyQualifiedName(V_5.get_AttributeType()));
						}
						this.WriteTokenPreSpace("=");
						this.WriteTokenPreSpace("{");
						if (V_5.get_HasFields() || V_5.get_HasProperties())
						{
							this.WriteLine();
							this.Indent();
							V_6 = V_5.get_Fields().GetEnumerator();
							try
							{
								while (V_6.MoveNext())
								{
									V_7 = V_6.get_Current();
									this.WriteKeyWordPostSpace("field");
									this.WriteSecurityDeclarationArgument(V_7);
									this.WriteLine();
								}
							}
							finally
							{
								V_6.Dispose();
							}
							V_6 = V_5.get_Properties().GetEnumerator();
							try
							{
								while (V_6.MoveNext())
								{
									V_8 = V_6.get_Current();
									this.WriteKeyWordPostSpace("property");
									this.WriteSecurityDeclarationArgument(V_8);
									this.WriteLine();
								}
							}
							finally
							{
								V_6.Dispose();
							}
							this.Outdent();
						}
						this.WriteToken("}");
						if (V_4 + 1 < V_1.get_SecurityAttributes().get_Count())
						{
							this.WriteToken(",");
						}
						this.WriteLine();
						V_4 = V_4 + 1;
					}
					this.Outdent();
					this.WriteToken("}");
					this.WriteLine();
				}
			}
			finally
			{
				V_0.Dispose();
			}
			return;
		}

		private void WriteStartBlock()
		{
			this.formatter.WriteStartBlock();
			this.WriteOpeningBraceBetweenLines();
			this.Indent();
			return;
		}

		private void WriteStructureBody(ILBlock s, HashSet<int> branchTargets, ref Instruction inst, MemberMapping currentMethodMapping, int codeSize)
		{
			V_0 = true;
			V_1 = false;
			V_2 = 0;
			while (inst != null && inst.get_Offset() < s.EndOffset)
			{
				V_3 = inst.get_Offset();
				if (V_2 >= s.Children.get_Count() || s.Children.get_Item(V_2).StartOffset > V_3 || V_3 >= s.Children.get_Item(V_2).EndOffset)
				{
					if (!V_0 && V_1 || branchTargets.Contains(V_3))
					{
						this.WriteLine();
					}
					this.WriteInstruction(inst);
					if (currentMethodMapping != null)
					{
						stackVariable53 = currentMethodMapping.get_MemberCodeMappings();
						stackVariable54 = new SourceCodeMapping();
						stackVariable55 = new ILRange();
						stackVariable55.From = inst.get_Offset();
						if (inst.get_Next() == null)
						{
							stackVariable62 = codeSize;
						}
						else
						{
							stackVariable62 = inst.get_Next().get_Offset();
						}
						stackVariable55.To = stackVariable62;
						stackVariable54.set_ILInstructionOffset(stackVariable55);
						stackVariable54.set_MemberMapping(currentMethodMapping);
						stackVariable53.Add(stackVariable54);
					}
					this.WriteLine();
					if (inst.get_OpCode().get_FlowControl() == null || inst.get_OpCode().get_FlowControl() == 3 || inst.get_OpCode().get_FlowControl() == 7)
					{
						stackVariable28 = true;
					}
					else
					{
						V_5 = inst.get_OpCode();
						stackVariable28 = V_5.get_FlowControl() == 8;
					}
					V_1 = stackVariable28;
					inst = inst.get_Next();
				}
				else
				{
					stackVariable86 = V_2;
					V_2 = stackVariable86 + 1;
					V_4 = s.Children.get_Item(stackVariable86);
					this.WriteStructureHeader(V_4);
					this.WriteStructureBody(V_4, branchTargets, ref inst, currentMethodMapping, codeSize);
					this.WriteStructureFooter(V_4);
				}
				V_0 = false;
			}
			return;
		}

		private void WriteStructureFooter(ILBlock s)
		{
			this.Outdent();
			switch (s.Type - 1)
			{
				case 0:
				{
					this.WriteToken("}");
					this.WriteLine();
					return;
				}
				case 1:
				{
					this.WriteToken("}");
					this.WriteLine();
					return;
				}
				case 2:
				{
					this.WriteToken("}");
					this.WriteLine();
					return;
				}
				case 3:
				{
					this.WriteToken("}");
					this.WriteLine();
					return;
				}
			}
			throw new NotSupportedException();
		}

		private void WriteStructureHeader(ILBlock s)
		{
			switch (s.Type - 1)
			{
				case 0:
				{
					dummyVar0 = s.LoopEntryPoint;
					this.WriteKeyword(".loop");
					this.WriteOpeningBraceBetweenLines();
					break;
				}
				case 1:
				{
					this.WriteKeyword(".try");
					this.WriteOpeningBraceBetweenLines();
					break;
				}
				case 2:
				{
					switch (s.ExceptionHandler.get_HandlerType())
					{
						case 0:
						case 1:
						{
							this.WriteKeyword("catch");
							if (s.ExceptionHandler.get_CatchType() != null)
							{
								this.WriteSpace();
								this.WriteTypeReference(s.ExceptionHandler.get_CatchType(), 2);
							}
							this.WriteLine();
							break;
						}
						case 2:
						{
							this.WriteKeyword("finally");
							this.WriteLine();
							break;
						}
						case 3:
						{
						Label1:
							throw new NotSupportedException();
						}
						case 4:
						{
							this.WriteKeyword("fault");
							this.WriteLine();
							break;
						}
						default:
						{
							goto Label1;
						}
					}
					this.WriteToken("{");
					this.WriteLine();
					break;
				}
				case 3:
				{
					this.WriteKeyword("filter");
					this.WriteOpeningBraceBetweenLines();
					break;
				}
				default:
				{
					goto Label0;
				}
			}
			this.Indent();
			return;
		Label0:
			throw new NotSupportedException();
		}

		private void WriteTokenPostSpace(string token)
		{
			this.WriteToken(token);
			this.WriteSpace();
			return;
		}

		private void WriteTokenPreSpace(string token)
		{
			this.WriteSpace();
			this.WriteToken(token);
			return;
		}

		protected void WriteType(TypeReference type, IntermediateLanguageWriter.ILNameSyntax syntax)
		{
			if (type as ArrayType != null)
			{
				this.WriteArrayType(type, syntax);
				return;
			}
			V_0 = ILHelpers.PrimitiveTypeName(type.GetFriendlyFullName(this.get_Language()));
			if (syntax == 3)
			{
				if (V_0 != null)
				{
					this.WriteReference(V_0, type);
					return;
				}
				this.WriteReference(ILHelpers.Escape(type.get_Name()), type);
				return;
			}
			if (syntax == IntermediateLanguageWriter.ILNameSyntax.Signature || syntax == 1 && V_0 != null)
			{
				this.WriteReference(V_0, type);
				return;
			}
			if (syntax == IntermediateLanguageWriter.ILNameSyntax.Signature || syntax == 1)
			{
				if (type.get_IsValueType())
				{
					stackVariable15 = "valuetype";
				}
				else
				{
					stackVariable15 = "class";
				}
				this.WriteKeyWordPostSpace(stackVariable15);
			}
			if (type.get_DeclaringType() != null)
			{
				this.WriteTypeReference(type.get_DeclaringType(), 2);
				this.WriteKeyword("/");
				this.WriteReference(ILHelpers.Escape(type.get_Name()), type);
				return;
			}
			if (!type.get_IsDefinition() && type.get_Scope() != null && type as TypeSpecification == null)
			{
				this.WriteToken("[");
				this.WriteEscaped(type.get_Scope().get_Name());
				this.WriteToken("]");
			}
			this.WriteReference(ILHelpers.Escape(type.get_FullName()), type);
			return;
		}

		protected override string WriteTypeDeclaration(TypeDefinition type, bool isPartial = false)
		{
			if (this.get_CodeMappings() == null)
			{
				stackVariable91 = new Telerik.JustDecompiler.Languages.IL.CodeMappings();
				stackVariable91.set_FullName(type.get_FullName());
				stackVariable91.set_Mapping(new List<MemberMapping>());
				this.set_CodeMappings(stackVariable91);
			}
			this.WriteKeyWordPostSpace(".class");
			if (type.get_Attributes() & 32 == 32)
			{
				this.WriteKeyWordPostSpace("interface");
			}
			this.flagsWriter.WriteTypeVisibility(type);
			this.flagsWriter.WriteTypeLayoutFlags(type);
			this.flagsWriter.WriteTypeStringFormat(type);
			this.flagsWriter.WriteTypeAttributes(type);
			V_0 = this.formatter.get_CurrentPosition();
			if (type.get_DeclaringType() != null)
			{
				stackVariable28 = type.get_Name();
			}
			else
			{
				stackVariable28 = type.get_FullName();
			}
			this.WriteReference(ILHelpers.Escape(stackVariable28), type);
			V_1 = this.formatter.get_CurrentPosition() - 1;
			this.currentWritingInfo.get_MemberDeclarationToCodePostionMap().set_Item(type, new OffsetSpan(V_0, V_1));
			this.WriteTypeParameters(type);
			this.WriteLine();
			if (type.get_BaseType() != null)
			{
				this.Indent();
				this.WriteKeyWordPostSpace("extends");
				this.WriteTypeReference(type.get_BaseType(), 2);
				this.WriteLine();
				this.Outdent();
			}
			if (type.get_HasInterfaces())
			{
				this.Indent();
				V_2 = 0;
				while (V_2 < type.get_Interfaces().get_Count())
				{
					if (V_2 > 0)
					{
						this.WriteToken(",");
						this.WriteLine();
					}
					if (V_2 != 0)
					{
						this.Write("           ");
					}
					else
					{
						this.WriteKeyWordPostSpace("implements");
					}
					this.WriteTypeReference(type.get_Interfaces().get_Item(V_2), 2);
					V_2 = V_2 + 1;
				}
				this.WriteLine();
				this.Outdent();
			}
			return String.Empty;
		}

		protected override void WriteTypeOpeningBlock(TypeDefinition type)
		{
			dummyVar0 = this.WriteAttributes(type.get_CustomAttributes());
			this.WriteSecurityDeclarations(type);
			if (type.get_HasLayoutInfo())
			{
				this.Write(String.Format(".pack {0}", type.get_PackingSize()));
				this.WriteLine();
				this.Write(String.Format(".size {0}", type.get_ClassSize()));
				this.WriteLine();
				this.WriteLine();
			}
			return;
		}

		private void WriteTypeParameters(IGenericParameterProvider p)
		{
			if (!p.get_HasGenericParameters())
			{
				return;
			}
			this.WriteToken("<");
			V_0 = 0;
			while (V_0 < p.get_GenericParameters().get_Count())
			{
				if (V_0 > 0)
				{
					this.WriteTokenPostSpace(",");
				}
				this.WriteGenericTypeParameter(p.get_GenericParameters().get_Item(V_0));
				V_0 = V_0 + 1;
			}
			this.WriteToken(">");
			return;
		}

		private void WriteTypeReference(TypeReference type, IntermediateLanguageWriter.ILNameSyntax syntax = 0)
		{
			if (syntax == 1)
			{
				stackVariable2 = syntax;
			}
			else
			{
				stackVariable2 = 0;
			}
			V_0 = stackVariable2;
			if (type as PinnedType != null)
			{
				this.WriteTypeReference(((PinnedType)type).get_ElementType(), V_0);
				this.WriteKeyWordPreSpace("pinned");
				return;
			}
			if (type as ArrayType != null)
			{
				this.WriteArrayType(type, V_0);
				return;
			}
			if (type as GenericParameter != null)
			{
				this.WriteGenericTypeParameter(type, syntax);
				return;
			}
			if (type as ByReferenceType != null)
			{
				this.WriteTypeReference(((ByReferenceType)type).get_ElementType(), V_0);
				this.WriteToken("&");
				return;
			}
			if (type as PointerType != null)
			{
				this.WriteTypeReference(((PointerType)type).get_ElementType(), V_0);
				this.WriteToken("*");
				return;
			}
			if (type as GenericInstanceType == null)
			{
				if (type as OptionalModifierType != null)
				{
					this.WriteTypeReference(((OptionalModifierType)type).get_ElementType(), syntax);
					this.WriteKeyWordPreSpace("modopt");
					this.WriteToken("(");
					this.WriteTypeReference(((OptionalModifierType)type).get_ModifierType(), 2);
					this.WriteTokenPostSpace(")");
					return;
				}
				if (type as RequiredModifierType == null)
				{
					this.WriteType(type, syntax);
					return;
				}
				this.WriteTypeReference(((RequiredModifierType)type).get_ElementType(), syntax);
				this.WriteKeyWordPreSpace("modreq");
				this.WriteToken("(");
				this.WriteTypeReference(((RequiredModifierType)type).get_ModifierType(), 2);
				this.WriteTokenPostSpace(")");
				return;
			}
			this.WriteTypeReference(type.GetElementType(), V_0);
			this.WriteToken("<");
			V_1 = ((GenericInstanceType)type).get_GenericArguments();
			V_2 = 0;
			while (V_2 < V_1.get_Count())
			{
				V_3 = V_1.get_Item(V_2);
				if (((GenericInstanceType)type).get_PostionToArgument().ContainsKey(V_2))
				{
					V_3 = ((GenericInstanceType)type).get_PostionToArgument().get_Item(V_2);
				}
				if (V_2 > 0)
				{
					this.WriteTokenPostSpace(",");
				}
				this.WriteTypeReference(V_3, V_0);
				V_2 = V_2 + 1;
			}
			this.WriteToken(">");
			return;
		}

		protected enum ILNameSyntax
		{
			Signature,
			SignatureNoNamedTypeParameters,
			TypeName,
			ShortTypeName
		}
	}
}