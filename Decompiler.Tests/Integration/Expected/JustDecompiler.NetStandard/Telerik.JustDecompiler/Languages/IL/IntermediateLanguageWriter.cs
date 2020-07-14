using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Mono.Cecil.Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Globalization;
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

		public IntermediateLanguageWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings) : base(language, formatter, exceptionFormatter, settings)
		{
			this.flagsWriter = new FlagsWriter(this);
		}

		private void Disassemble(MethodBody body, MemberMapping methodMapping)
		{
			MethodDefinition method = body.get_Method();
			if ((object)method.get_DeclaringType().get_Module().get_Assembly().get_EntryPoint() == (object)method)
			{
				this.WriteKeyword(".entrypoint");
				this.WriteLine();
			}
			if (method.get_Body().get_HasVariables())
			{
				this.WriteMethodVariables(method);
				this.WriteLine();
			}
			if (base.Settings.ShouldGenerateBlocks && body.get_Instructions().get_Count() > 0)
			{
				Instruction item = body.get_Instructions().get_Item(0);
				HashSet<int> branchTargets = this.GetBranchTargets(body.get_Instructions());
				this.WriteStructureBody(new ILBlock(body), branchTargets, ref item, methodMapping, method.get_Body().get_CodeSize());
				return;
			}
			foreach (Instruction instruction in method.get_Body().get_Instructions())
			{
				this.WriteInstruction(instruction);
				if (methodMapping != null)
				{
					methodMapping.MemberCodeMappings.Add(new SourceCodeMapping()
					{
						ILInstructionOffset = new ILRange()
						{
							From = instruction.get_Offset(),
							To = (instruction.get_Next() == null ? method.get_Body().get_CodeSize() : instruction.get_Next().get_Offset())
						},
						MemberMapping = methodMapping
					});
				}
				this.WriteLine();
			}
			if (method.get_Body().get_HasExceptionHandlers())
			{
				this.WriteLine();
				foreach (ExceptionHandler exceptionHandler in method.get_Body().get_ExceptionHandlers())
				{
					this.WriteExceptionHandler(exceptionHandler);
					this.WriteLine();
				}
			}
		}

		private string GetAssemblyQualifiedName(TypeReference type)
		{
			AssemblyNameReference scope = type.get_Scope() as AssemblyNameReference;
			if (scope == null)
			{
				ModuleDefinition moduleDefinition = type.get_Scope() as ModuleDefinition;
				if (moduleDefinition != null)
				{
					scope = moduleDefinition.get_Assembly().get_Name();
				}
			}
			if (scope == null)
			{
				return type.get_FullName();
			}
			return String.Concat(type.get_FullName(), ", ", scope.get_FullName());
		}

		private HashSet<int> GetBranchTargets(IEnumerable<Instruction> instructions)
		{
			HashSet<int> nums = new HashSet<int>();
			foreach (Instruction instruction in instructions)
			{
				Instruction operand = instruction.get_Operand() as Instruction;
				if (operand != null)
				{
					nums.Add(operand.get_Offset());
				}
				Instruction[] instructionArray = instruction.get_Operand() as Instruction[];
				if (instructionArray == null)
				{
					continue;
				}
				Instruction[] instructionArray1 = instructionArray;
				for (int i = 0; i < (int)instructionArray1.Length; i++)
				{
					nums.Add(instructionArray1[i].get_Offset());
				}
			}
			return nums;
		}

		private string GetFullName(object member)
		{
			string empty = String.Empty;
			if (member is IMemberDefinition)
			{
				empty = (base.Settings.RenameInvalidMembers ? ((IMemberDefinition)member).GetFriendlyFullName(base.Language) : ((IMemberDefinition)member).get_FullName());
			}
			else if (member is ParameterReference)
			{
				empty = (base.Settings.RenameInvalidMembers ? base.Language.ReplaceInvalidCharactersInIdentifier(((ParameterReference)member).get_Name()) : ((ParameterReference)member).get_Name());
			}
			else if (!(member is MemberReference))
			{
				empty = (base.Settings.RenameInvalidMembers ? base.Language.ReplaceInvalidCharactersInIdentifier(member.ToString()) : member.ToString());
			}
			else
			{
				empty = (base.Settings.RenameInvalidMembers ? ((MemberReference)member).GetFriendlyFullName(base.Language) : ((MemberReference)member).get_FullName());
			}
			return empty;
		}

		private string GetName(object member)
		{
			string empty = String.Empty;
			if (member is IMemberDefinition)
			{
				empty = ((IMemberDefinition)member).get_Name();
			}
			else if (!(member is ParameterReference))
			{
				empty = (!(member is MemberReference) ? member.ToString() : ((MemberReference)member).get_Name());
			}
			else
			{
				empty = ((ParameterReference)member).get_Name();
			}
			return empty;
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

		protected override void Write(EventDefinition @event)
		{
			this.WriteEventDeclaration(@event);
			int currentPosition = this.formatter.CurrentPosition;
			this.WriteStartBlock();
			this.WriteAttributes(@event.get_CustomAttributes());
			this.WriteNestedMethod(".addon", @event.get_AddMethod());
			this.WriteLine();
			this.WriteNestedMethod(".removeon", @event.get_RemoveMethod());
			this.WriteNestedMethod(".fire", @event.get_InvokeMethod());
			foreach (MethodDefinition otherMethod in @event.get_OtherMethods())
			{
				this.WriteNestedMethod(".other", otherMethod);
			}
			this.WriteEndBlock();
			this.currentWritingInfo.MemberDefinitionToFoldingPositionMap[@event] = new OffsetSpan(currentPosition, this.formatter.CurrentPosition - 1);
		}

		protected override void Write(PropertyDefinition property)
		{
			this.WritePropertyDeclaration(property);
			int currentPosition = this.formatter.CurrentPosition;
			this.WriteStartBlock();
			this.WriteAttributes(property.get_CustomAttributes());
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
			bool flag = false;
			foreach (MethodDefinition otherMethod in property.get_OtherMethods())
			{
				if (flag)
				{
					this.WriteLine();
				}
				this.WriteNestedMethod(".other", otherMethod);
				flag = true;
			}
			if (flag)
			{
				this.WriteLine();
			}
			this.WriteEndBlock();
			this.currentWritingInfo.MemberDefinitionToFoldingPositionMap[property] = new OffsetSpan(currentPosition, this.formatter.CurrentPosition - 1);
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
			this.WriteTypeReference(field.get_FieldType(), IntermediateLanguageWriter.ILNameSyntax.Signature);
			this.WriteSpace();
			int currentPosition = this.formatter.CurrentPosition;
			this.WriteReference(ILHelpers.Escape(field.get_Name()), field);
			int num = this.formatter.CurrentPosition - 1;
			this.currentWritingInfo.MemberDeclarationToCodePostionMap[field] = new OffsetSpan(currentPosition, num);
			if ((field.get_Attributes() & 0x100) == 0x100)
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
		}

		protected override void Write(MethodDefinition method)
		{
			this.WriteMethodDeclaration(method, false);
			int currentPosition = this.formatter.CurrentPosition;
			this.WriteStartBlock();
			this.method = method;
			this.WriteMethodBody(method);
			this.WriteEndBlock();
			this.currentWritingInfo.MemberDefinitionToFoldingPositionMap[method] = new OffsetSpan(currentPosition, this.formatter.CurrentPosition - 1);
		}

		private void WriteArrayType(TypeReference type, IntermediateLanguageWriter.ILNameSyntax syntaxForElementTypes)
		{
			ArrayType arrayType = (ArrayType)type;
			this.WriteTypeReference(arrayType.get_ElementType(), syntaxForElementTypes);
			this.WriteToken("[");
			for (int i = 0; i < arrayType.get_Dimensions().get_Count(); i++)
			{
				if (i > 0)
				{
					this.WriteTokenPostSpace(",");
				}
				ArrayDimension item = arrayType.get_Dimensions().get_Item(i);
				this.WriteToken(item.ToString());
			}
			this.WriteToken("]");
		}

		private bool WriteAttributes(Collection<CustomAttribute> attributes)
		{
			if (attributes.get_Count() == 0)
			{
				return false;
			}
			foreach (CustomAttribute attribute in attributes)
			{
				this.WriteKeyWordPostSpace(".custom");
				this.WriteMethodReference(attribute.get_Constructor(), true);
				byte[] blob = attribute.GetBlob();
				if (blob != null)
				{
					this.WriteSpace();
					this.WriteToken("=");
					this.WriteSpace();
					this.WriteBlob(blob);
				}
				this.WriteLine();
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
		}

		private void WriteBlob(byte[] blob)
		{
			this.WriteToken("(");
			this.Indent();
			for (int i = 0; i < (int)blob.Length; i++)
			{
				if (i % 16 != 0 || i >= (int)blob.Length - 1)
				{
					this.WriteSpace();
				}
				else
				{
					this.WriteLine();
				}
				this.Write(blob[i].ToString("x2"));
			}
			this.WriteLine();
			this.Outdent();
			this.WriteToken(")");
		}

		private void WriteConstant(object constant)
		{
			if (constant == null)
			{
				this.WriteKeyword("nullref");
				return;
			}
			string str = ILHelpers.PrimitiveTypeName(constant.GetType().FullName);
			if (str == null || !(str != "string"))
			{
				this.WriteOperand(constant);
				return;
			}
			this.Write(str);
			this.WriteToken("(");
			float? nullable = (float?)(constant as float?);
			double? nullable1 = (double?)(constant as double?);
			if (nullable.HasValue && (Single.IsNaN(nullable.Value) || Single.IsInfinity(nullable.Value)))
			{
				this.Write(String.Format("0x{0:x8}", BitConverter.ToInt32(BitConverter.GetBytes(nullable.Value), 0)));
			}
			else if (!nullable1.HasValue || !Double.IsNaN(nullable1.Value) && !Double.IsInfinity(nullable1.Value))
			{
				this.WriteOperand(constant);
			}
			else
			{
				this.Write(String.Format("0x{0:x16}", BitConverter.DoubleToInt64Bits(nullable1.Value)));
			}
			this.WriteToken(")");
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
				this.WriteLiteral(val.ToString("R", CultureInfo.InvariantCulture));
				return;
			}
			byte[] bytes = BitConverter.GetBytes(val);
			this.WriteToken("(");
			for (int i = 0; i < (int)bytes.Length; i++)
			{
				if (i > 0)
				{
					this.WriteSpace();
				}
				this.WriteLiteral(bytes[i].ToString("X2"));
			}
			this.WriteToken(")");
		}

		protected override void WriteEndBlock(string statementName)
		{
			this.WriteToken("}");
		}

		private void WriteEndBlock()
		{
			this.Outdent();
			this.WriteToken("}");
			this.formatter.WriteEndBlock();
		}

		private void WriteEscaped(string identifier)
		{
			string str = ILHelpers.Escape(identifier);
			if (str.StartsWith("'"))
			{
				this.WriteKeyword(str);
				return;
			}
			this.Write(str);
		}

		protected override void WriteEventDeclaration(EventDefinition @event)
		{
			this.WriteKeyWordPostSpace(".event");
			this.flagsWriter.WriteEventFlags(@event);
			this.WriteTypeReference(@event.get_EventType(), IntermediateLanguageWriter.ILNameSyntax.TypeName);
			this.WriteSpace();
			int currentPosition = this.formatter.CurrentPosition;
			this.WriteReference(ILHelpers.Escape(@event.get_Name()), @event);
			int num = this.formatter.CurrentPosition - 1;
			this.currentWritingInfo.MemberDeclarationToCodePostionMap[@event] = new OffsetSpan(currentPosition, num);
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
				this.WriteTypeReference(exceptionHandler.get_CatchType(), IntermediateLanguageWriter.ILNameSyntax.Signature);
			}
			this.WriteSpace();
			this.WriteOffsetReference(exceptionHandler.get_HandlerStart());
			this.WriteSpace();
			this.WriteOffsetReference(exceptionHandler.get_HandlerEnd());
		}

		private void WriteFieldReference(FieldReference field)
		{
			this.WriteTypeReference(field.get_FieldType(), IntermediateLanguageWriter.ILNameSyntax.SignatureNoNamedTypeParameters);
			this.WriteSpace();
			this.WriteTypeReference(field.get_DeclaringType(), IntermediateLanguageWriter.ILNameSyntax.TypeName);
			this.WriteToken("::");
			this.WriteReference(ILHelpers.Escape(field.get_Name()), field);
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
				this.WriteLiteral(val.ToString("R", CultureInfo.InvariantCulture));
				return;
			}
			byte[] bytes = BitConverter.GetBytes(val);
			this.WriteToken("(");
			for (int i = 0; i < (int)bytes.Length; i++)
			{
				if (i > 0)
				{
					this.WriteSpace();
				}
				this.WriteLiteral(bytes[i].ToString("X2"));
			}
			this.WriteToken(")");
		}

		private void WriteGenericTypeParameter(GenericParameter gp)
		{
			if (gp.get_HasReferenceTypeConstraint())
			{
				this.WriteKeyWordPostSpace("class");
			}
			else if (gp.get_HasNotNullableValueTypeConstraint())
			{
				this.WriteKeyWordPostSpace("valuetype");
			}
			if (gp.get_HasDefaultConstructorConstraint())
			{
				this.WriteKeyWordPostSpace(".ctor");
			}
			if (gp.get_HasConstraints())
			{
				this.WriteToken("(");
				for (int i = 0; i < gp.get_Constraints().get_Count(); i++)
				{
					if (i > 0)
					{
						this.WriteTokenPostSpace(",");
					}
					this.WriteTypeReference(gp.get_Constraints().get_Item(i), IntermediateLanguageWriter.ILNameSyntax.TypeName);
				}
				this.WriteTokenPostSpace(")");
			}
			if (gp.get_IsContravariant())
			{
				this.WriteToken("-");
			}
			else if (gp.get_IsCovariant())
			{
				this.WriteToken("+");
			}
			this.WriteEscaped(gp.get_Name());
		}

		private void WriteGenericTypeParameter(TypeReference type, IntermediateLanguageWriter.ILNameSyntax syntax)
		{
			this.WriteToken("!");
			if (((GenericParameter)type).get_Owner().get_GenericParameterType() == 1)
			{
				this.WriteToken("!");
			}
			if (!String.IsNullOrEmpty(type.get_Name()) && type.get_Name()[0] != '!' && syntax != IntermediateLanguageWriter.ILNameSyntax.SignatureNoNamedTypeParameters)
			{
				this.WriteReference(ILHelpers.Escape(type.get_Name()), null);
				return;
			}
			int position = ((GenericParameter)type).get_Position();
			this.WriteReference(position.ToString(), null);
		}

		private void WriteInstruction(Instruction instruction)
		{
			this.WriteDefinition(this.OffsetToString(instruction.get_Offset()), instruction);
			this.WriteTokenPostSpace(":");
			OpCode opCode = instruction.get_OpCode();
			this.WriteReference(opCode.get_Name(), instruction.get_OpCode());
			if (instruction.get_Operand() != null)
			{
				this.WriteSpace();
				if (instruction.get_OpCode() == OpCodes.Ldtoken)
				{
					if (instruction.get_Operand() is MethodReference)
					{
						this.WriteKeyWordPostSpace("method");
					}
					else if (instruction.get_Operand() is FieldReference)
					{
						this.WriteKeyWordPostSpace("field");
					}
				}
				this.WriteOperand(instruction.get_Operand());
			}
		}

		private void WriteKeyWordPostSpace(string keyWord)
		{
			this.WriteKeyword(keyWord);
			this.WriteSpace();
		}

		private void WriteKeyWordPreSpace(string keyWord)
		{
			this.WriteSpace();
			this.WriteKeyword(keyWord);
		}

		private void WriteLabelList(Instruction[] instructions)
		{
			this.WriteToken("(");
			for (int i = 0; i < (int)instructions.Length; i++)
			{
				if (i != 0)
				{
					this.WriteTokenPostSpace(",");
				}
				this.WriteOffsetReference(instructions[i]);
			}
			this.WriteToken(")");
		}

		private void WriteLiteralInQuotes(string literal)
		{
			this.WriteLiteral(String.Format("\"{0}\"", literal));
		}

		private void WriteMarshalInfo(MarshalInfo marshalInfo)
		{
			this.WriteKeyword("marshal");
			this.WriteToken("(");
			this.WriteNativeType(marshalInfo.get_NativeType(), marshalInfo);
			this.WriteToken("(");
			this.WriteSpace();
		}

		public override void WriteMemberNavigationName(object memberDefinition)
		{
			object obj;
			object obj1;
			object obj2;
			object obj3;
			if (memberDefinition == null)
			{
				return;
			}
			if (memberDefinition is TypeReference)
			{
				TypeReference typeReference = (TypeReference)memberDefinition;
				this.formatter.Write((base.Settings.RenameInvalidMembers ? typeReference.GetFriendlyTypeName(base.Language, "<", ">") : typeReference.get_Name()));
				return;
			}
			string fullName = this.GetFullName(memberDefinition);
			string[] strArray = fullName.Split(new String[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
			if ((int)strArray.Length > 1)
			{
				fullName = strArray[1];
			}
			if (memberDefinition is MethodDefinition)
			{
				MethodDefinition methodDefinition = memberDefinition as MethodDefinition;
				IFormatter formatter = this.formatter;
				string str = fullName;
				obj3 = (base.Settings.RenameInvalidMembers ? methodDefinition.get_ReturnType().GetFriendlyFullName(base.Language) : methodDefinition.get_ReturnType().get_FullName());
				formatter.Write(String.Format("{0} : {1}", (object)str, obj3));
				return;
			}
			if (memberDefinition is PropertyDefinition)
			{
				PropertyDefinition propertyDefinition = memberDefinition as PropertyDefinition;
				IFormatter formatter1 = this.formatter;
				string str1 = fullName;
				obj2 = (base.Settings.RenameInvalidMembers ? propertyDefinition.get_PropertyType().GetFriendlyFullName(base.Language) : propertyDefinition.get_PropertyType().get_FullName());
				formatter1.Write(String.Format("{0} : {1}", (object)str1, obj2));
				return;
			}
			if (memberDefinition is FieldDefinition)
			{
				FieldDefinition fieldDefinition = memberDefinition as FieldDefinition;
				IFormatter formatter2 = this.formatter;
				string str2 = fullName;
				obj1 = (base.Settings.RenameInvalidMembers ? fieldDefinition.get_FieldType().GetFriendlyFullName(base.Language) : fieldDefinition.get_FieldType().get_FullName());
				formatter2.Write(String.Format("{0} : {1}", (object)str2, obj1));
				return;
			}
			if (memberDefinition is EventDefinition)
			{
				EventDefinition eventDefinition = memberDefinition as EventDefinition;
				IFormatter formatter3 = this.formatter;
				string str3 = fullName;
				obj = (base.Settings.RenameInvalidMembers ? eventDefinition.get_EventType().GetFriendlyFullName(base.Language) : eventDefinition.get_EventType().get_FullName());
				formatter3.Write(String.Format("{0} : {1}", (object)str3, obj));
				return;
			}
			if (memberDefinition is ParameterReference)
			{
				this.formatter.Write(((ParameterReference)memberDefinition).get_Name());
				return;
			}
			if (memberDefinition is MemberReference)
			{
				this.formatter.Write((base.Settings.RenameInvalidMembers ? ((MemberReference)memberDefinition).GetFriendlyFullName(base.Language) : ((MemberReference)memberDefinition).get_FullName()));
			}
		}

		public override void WriteMemberNavigationPathFullName(object member)
		{
			if (member == null)
			{
				return;
			}
			if (member is TypeReference)
			{
				this.formatter.Write(((TypeReference)member).get_Name());
				return;
			}
			string[] strArray = this.GetName(member).Split(new String[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
			if ((int)strArray.Length > 1)
			{
				string str = strArray[1];
			}
			if (member is ParameterReference)
			{
				this.formatter.Write(((ParameterReference)member).get_Name());
				return;
			}
			if (member is MemberReference)
			{
				this.formatter.Write(((MemberReference)member).GetFriendlyFullName(base.Language));
			}
		}

		private void WriteMethodBody(MethodDefinition method)
		{
			this.WriteAttributes(method.get_CustomAttributes());
			if (method.get_HasOverrides())
			{
				foreach (MethodReference @override in method.get_Overrides())
				{
					this.WriteKeyWordPostSpace(".override");
					this.WriteKeyWordPostSpace("method");
					this.WriteMethodReference(@override, true);
					this.WriteLine();
				}
			}
			foreach (ParameterDefinition parameter in method.get_Parameters())
			{
				this.WriteParameterAttributes(parameter);
			}
			this.WriteSecurityDeclarations(method);
			if (method.get_HasBody())
			{
				MemberMapping memberMapping = ILHelpers.CreateCodeMapping(method, this.CodeMappings);
				this.Disassemble(method.get_Body(), memberMapping);
			}
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
			if ((method.get_Attributes() & 0x2000) == 0x2000)
			{
				this.WriteKeyword("pinvokeimpl");
				if (method.get_HasPInvokeInfo() && method.get_PInvokeInfo() != null)
				{
					this.WritePInvokeAttributes(method);
				}
				this.WriteSpace();
			}
			this.Indent();
			if (method.get_ExplicitThis())
			{
				this.WriteKeyWordPostSpace("instance");
				this.WriteKeyWordPostSpace("explicit");
			}
			else if (method.get_HasThis())
			{
				this.WriteKeyWordPostSpace("instance");
			}
			this.flagsWriter.WriteMethodCallingConvention(method);
			this.WriteTypeReference(method.get_ReturnType(), IntermediateLanguageWriter.ILNameSyntax.Signature);
			this.WriteSpace();
			if (method.get_MethodReturnType().get_HasMarshalInfo())
			{
				this.WriteMarshalInfo(method.get_MethodReturnType().get_MarshalInfo());
			}
			int currentPosition = this.formatter.CurrentPosition;
			this.WriteMethodName(method);
			int num = this.formatter.CurrentPosition - 1;
			this.currentWritingInfo.MemberDeclarationToCodePostionMap[method] = new OffsetSpan(currentPosition, num);
			this.WriteTypeParameters(method);
			this.WriteMethodParameters(method);
			this.flagsWriter.WriteMethodCallType(method);
			this.WriteMethodManagedType(method);
			this.flagsWriter.WriteMethodImplementationAttribute(method);
			this.Outdent();
		}

		private void WriteMethodManagedType(MethodDefinition method)
		{
			// 
			// Current member / type: System.Void Telerik.JustDecompiler.Languages.IL.IntermediateLanguageWriter::WriteMethodManagedType(Mono.Cecil.MethodDefinition)
			// File path: C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\Decompiler.Tests\bin\Release\netcoreapp2.1\Integration\Actual\JustDecompiler.NetStandard.dll
			// 
			// Product version: 0.0.0.0
			// Exception in: System.Void WriteMethodManagedType(Mono.Cecil.MethodDefinition)
			// 
			// Object reference not set to an instance of an object.
			//    at Telerik.JustDecompiler.Steps.FixBinaryExpressionsStep.FixBranchingExpression(Expression expression, Instruction branch) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Steps\FixBinaryExpressionsStep.cs:line 291
			//    at Telerik.JustDecompiler.Steps.FixBinaryExpressionsStep.Process(DecompilationContext context, BlockStatement body) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Steps\FixBinaryExpressionsStep.cs:line 48
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.Process(DecompilationContext theContext, BlockStatement body) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\ExpressionDecompilerStep.cs:line 91
			//    at Telerik.JustDecompiler.Decompiler.DecompilationPipeline.RunInternal(MethodBody body, BlockStatement block, ILanguage language) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\DecompilationPipeline.cs:line 81
			//    at Telerik.JustDecompiler.Decompiler.DecompilationPipeline.Run(MethodBody body, ILanguage language) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.Decompile(MethodBody body, ILanguage language, DecompilationContext& context, TypeSpecificContext typeContext) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\Extensions.cs:line 61
			//    at Telerik.JustDecompiler.Decompiler.WriterContextServices.BaseWriterContextService.DecompileMethod(ILanguage language, MethodDefinition method, TypeSpecificContext typeContext) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
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
			string name = method.get_Name();
			int num = method.get_MetadataToken().ToInt32();
			this.WriteReference(ILHelpers.Escape(String.Concat(name, "$PST", num.ToString("X8"))), method);
		}

		private void WriteMethodName(MethodReference method, bool writeAsReference)
		{
			string str;
			MethodDefinition methodDefinition = method as MethodDefinition;
			if (methodDefinition == null || !methodDefinition.get_IsCompilerControlled())
			{
				str = ILHelpers.Escape(method.get_Name());
			}
			else
			{
				string name = method.get_Name();
				int num = method.get_MetadataToken().ToInt32();
				str = ILHelpers.Escape(String.Concat(name, "$PST", num.ToString("X8")));
			}
			if (!writeAsReference)
			{
				this.Write(str);
				return;
			}
			this.WriteReference(str, method);
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
		}

		protected void WriteMethodReference(MethodReference method, bool writeAsReference)
		{
			if (method.get_ExplicitThis())
			{
				this.WriteKeyWordPostSpace("instance");
				this.WriteKeyWordPostSpace("explicit");
			}
			else if (method.get_HasThis())
			{
				this.WriteKeyWordPostSpace("instance");
			}
			this.WriteTypeReference(method.get_FixedReturnType(), IntermediateLanguageWriter.ILNameSyntax.SignatureNoNamedTypeParameters);
			this.WriteSpace();
			if (method.get_DeclaringType() != null)
			{
				this.WriteTypeReference(method.get_DeclaringType(), IntermediateLanguageWriter.ILNameSyntax.TypeName);
				this.WriteToken("::");
			}
			this.WriteMethodName(method, writeAsReference);
			GenericInstanceMethod genericInstanceMethod = method as GenericInstanceMethod;
			if (genericInstanceMethod != null)
			{
				this.WriteToken("<");
				for (int i = 0; i < genericInstanceMethod.get_GenericArguments().get_Count(); i++)
				{
					if (i > 0)
					{
						this.WriteTokenPostSpace(",");
					}
					this.WriteTypeReference(genericInstanceMethod.get_GenericArguments().get_Item(i), IntermediateLanguageWriter.ILNameSyntax.Signature);
				}
				this.WriteToken(">");
			}
			this.WriteToken("(");
			Collection<ParameterDefinition> parameters = method.get_Parameters();
			for (int j = 0; j < parameters.get_Count(); j++)
			{
				if (j > 0)
				{
					this.WriteTokenPostSpace(", ");
				}
				this.WriteTypeReference(parameters.get_Item(j).get_ParameterType(), IntermediateLanguageWriter.ILNameSyntax.SignatureNoNamedTypeParameters);
			}
			this.WriteToken(")");
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
			foreach (VariableDefinition variable in method.get_Body().get_Variables())
			{
				int index = variable.get_Index();
				this.WriteDefinition(String.Concat("[", index.ToString(), "] "), variable);
				this.WriteTypeReference(variable.get_VariableType(), IntermediateLanguageWriter.ILNameSyntax.Signature);
				if (!String.IsNullOrEmpty(variable.get_Name()))
				{
					this.WriteSpace();
					this.Write(ILHelpers.Escape(variable.get_Name()));
				}
				if (variable.get_Index() + 1 < method.get_Body().get_Variables().get_Count())
				{
					this.WriteToken(",");
				}
				this.WriteLine();
			}
			this.Outdent();
			this.WriteToken(")");
			this.WriteLine();
		}

		private void WriteNativeType(NativeType nativeType, MarshalInfo marshalInfo = null)
		{
			switch (nativeType)
			{
				case 2:
				{
					this.WriteKeyword("bool");
					return;
				}
				case 3:
				{
					this.WriteKeyword("int8");
					return;
				}
				case 4:
				{
					this.WriteKeyword("unsigned int8");
					return;
				}
				case 5:
				{
					this.WriteKeyword("int16");
					return;
				}
				case 6:
				{
					this.WriteKeyword("unsigned int16");
					return;
				}
				case 7:
				{
					this.WriteKeyword("int32");
					return;
				}
				case 8:
				{
					this.WriteKeyword("unsigned int32");
					return;
				}
				case 9:
				{
					this.WriteKeyword("int64");
					return;
				}
				case 10:
				{
					this.WriteKeyword("unsigned int64");
					return;
				}
				case 11:
				{
					this.WriteKeyword("float32");
					return;
				}
				case 12:
				{
					this.WriteKeyword("float64");
					return;
				}
				case 13:
				case 14:
				case 16:
				case 17:
				case 18:
				case 24:
				case 33:
				case 38:
				case 39:
				case 41:
				{
					this.WriteKeyword(nativeType.ToString());
					break;
				}
				case 15:
				{
					this.WriteKeyword("currency");
					return;
				}
				case 19:
				{
					this.WriteKeyword("bstr");
					return;
				}
				case 20:
				{
					this.WriteKeyword("lpstr");
					return;
				}
				case 21:
				{
					this.WriteKeyword("lpwstr");
					return;
				}
				case 22:
				{
					this.WriteKeyword("lptstr");
					return;
				}
				case 23:
				{
					this.WriteKeyword(String.Format("fixed sysstring[{0}]", ((FixedSysStringMarshalInfo)marshalInfo).get_Size()));
					return;
				}
				case 25:
				{
					this.WriteKeyword("iunknown");
					return;
				}
				case 26:
				{
					this.WriteKeyword("idispatch");
					return;
				}
				case 27:
				{
					this.WriteKeyword("struct");
					return;
				}
				case 28:
				{
					this.WriteKeyword("interface");
					return;
				}
				case 29:
				{
					this.WriteKeyWordPostSpace("safearray");
					SafeArrayMarshalInfo safeArrayMarshalInfo = marshalInfo as SafeArrayMarshalInfo;
					if (safeArrayMarshalInfo == null)
					{
						break;
					}
					switch (safeArrayMarshalInfo.get_ElementType())
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
							this.WriteKeyword(safeArrayMarshalInfo.get_ElementType().ToString());
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
							this.WriteKeyword(safeArrayMarshalInfo.get_ElementType().ToString());
							return;
						}
					}
					break;
				}
				case 30:
				{
					this.WriteKeyword("fixed array");
					FixedArrayMarshalInfo fixedArrayMarshalInfo = marshalInfo as FixedArrayMarshalInfo;
					if (fixedArrayMarshalInfo == null)
					{
						break;
					}
					this.WriteToken("[");
					this.WriteLiteral(fixedArrayMarshalInfo.get_Size().ToString());
					this.WriteToken("]");
					if (fixedArrayMarshalInfo.get_ElementType() == 102)
					{
						break;
					}
					this.WriteSpace();
					this.WriteNativeType(fixedArrayMarshalInfo.get_ElementType(), null);
					return;
				}
				case 31:
				{
					this.WriteKeyword("int");
					return;
				}
				case 32:
				{
					this.WriteKeyword("unsigned int");
					return;
				}
				case 34:
				{
					this.WriteKeyword("byvalstr");
					return;
				}
				case 35:
				{
					this.WriteKeyword("ansi bstr");
					return;
				}
				case 36:
				{
					this.WriteKeyword("tbstr");
					return;
				}
				case 37:
				{
					this.WriteKeyword("variant bool");
					return;
				}
				case 40:
				{
					this.WriteKeyword("as any");
					return;
				}
				case 42:
				{
					ArrayMarshalInfo arrayMarshalInfo = (ArrayMarshalInfo)marshalInfo;
					if (arrayMarshalInfo == null)
					{
						goto case 41;
					}
					if (arrayMarshalInfo.get_ElementType() != 80)
					{
						this.WriteNativeType(arrayMarshalInfo.get_ElementType(), null);
					}
					this.WriteToken("[");
					if (arrayMarshalInfo.get_SizeParameterMultiplier() != 0)
					{
						if (arrayMarshalInfo.get_Size() >= 0)
						{
							this.WriteLiteral(arrayMarshalInfo.get_Size().ToString());
						}
						this.WriteSpace();
						this.WriteToken("+");
						this.WriteSpace();
						this.WriteLiteral(arrayMarshalInfo.get_SizeParameterIndex().ToString());
					}
					else
					{
						this.WriteLiteral(arrayMarshalInfo.get_Size().ToString());
					}
					this.WriteToken("]");
					return;
				}
				case 43:
				{
					this.WriteKeyword("lpstruct");
					return;
				}
				case 44:
				{
					CustomMarshalInfo customMarshalInfo = marshalInfo as CustomMarshalInfo;
					if (customMarshalInfo == null)
					{
						goto case 41;
					}
					this.WriteKeyword("custom");
					this.WriteToken("(");
					this.WriteLiteralInQuotes(BaseLanguageWriter.ConvertString(customMarshalInfo.get_ManagedType().get_FullName()));
					this.WriteTokenPostSpace(",");
					this.WriteLiteralInQuotes(BaseLanguageWriter.ConvertString(customMarshalInfo.get_Cookie()));
					if (customMarshalInfo.get_Guid() != Guid.Empty || !String.IsNullOrEmpty(customMarshalInfo.get_UnmanagedType()))
					{
						this.WriteTokenPostSpace(",");
						this.WriteLiteralInQuotes(customMarshalInfo.get_Guid().ToString());
						this.WriteTokenPostSpace(",");
						this.WriteLiteralInQuotes(BaseLanguageWriter.ConvertString(customMarshalInfo.get_UnmanagedType()));
					}
					this.WriteToken(")");
					return;
				}
				case 45:
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
					goto case 41;
				}
			}
		}

		private void WriteNestedMethod(string keyword, MethodDefinition method)
		{
			if (method == null)
			{
				return;
			}
			this.WriteKeyWordPostSpace(keyword);
			this.WriteMethodReference(method, false);
			int currentPosition = this.formatter.CurrentPosition;
			this.WriteStartBlock();
			this.WriteMethodBody(method);
			this.WriteEndBlock();
			this.currentWritingInfo.MemberDefinitionToFoldingPositionMap[method] = new OffsetSpan(currentPosition, this.formatter.CurrentPosition - 1);
			this.currentWritingInfo.MemberDeclarationToCodePostionMap[method] = new OffsetSpan(currentPosition, this.formatter.CurrentPosition - 1);
		}

		private void WriteOffsetReference(Instruction instruction)
		{
			this.Write(this.OffsetToString(instruction.get_Offset()));
		}

		private void WriteOpeningBrace()
		{
			this.WriteToken("{");
		}

		private void WriteOpeningBraceAndNewLine()
		{
			this.WriteOpeningBrace();
			this.WriteLine();
		}

		private void WriteOpeningBraceBetweenLines()
		{
			this.WriteLine();
			this.WriteOpeningBraceAndNewLine();
		}

		private void WriteOperand(object operand)
		{
			int index;
			if (operand == null)
			{
				throw new ArgumentNullException("operand");
			}
			Instruction instruction = operand as Instruction;
			if (instruction != null)
			{
				this.WriteOffsetReference(instruction);
				return;
			}
			Instruction[] instructionArray = operand as Instruction[];
			if (instructionArray != null)
			{
				this.WriteLabelList(instructionArray);
				return;
			}
			VariableReference variableReference = operand as VariableReference;
			if (variableReference != null)
			{
				if (!String.IsNullOrEmpty(variableReference.get_Name()))
				{
					this.WriteReference(ILHelpers.Escape(variableReference.get_Name()), variableReference);
					return;
				}
				index = variableReference.get_Index();
				this.WriteReference(index.ToString(), variableReference);
				return;
			}
			ParameterReference parameterReference = operand as ParameterReference;
			if (parameterReference != null)
			{
				if (this.method != null && this.method.get_Body() != null && parameterReference == this.method.get_Body().get_ThisParameter())
				{
					this.WriteReference(0.ToString(), parameterReference);
					return;
				}
				if (!String.IsNullOrEmpty(parameterReference.get_Name()))
				{
					this.WriteReference(ILHelpers.Escape(parameterReference.get_Name()), parameterReference);
					return;
				}
				index = parameterReference.get_Index();
				this.WriteReference(index.ToString(), parameterReference);
				return;
			}
			MethodReference methodReference = operand as MethodReference;
			if (methodReference != null)
			{
				this.WriteMethodReference(methodReference, true);
				return;
			}
			TypeReference typeReference = operand as TypeReference;
			if (typeReference != null)
			{
				this.WriteTypeReference(typeReference, IntermediateLanguageWriter.ILNameSyntax.TypeName);
				return;
			}
			FieldReference fieldReference = operand as FieldReference;
			if (fieldReference != null)
			{
				this.WriteFieldReference(fieldReference);
				return;
			}
			string invariantCultureString = operand as String;
			if (invariantCultureString != null)
			{
				this.WriteLiteralInQuotes(BaseLanguageWriter.ConvertString(invariantCultureString));
				return;
			}
			if (operand is Char)
			{
				this.WriteLiteral(((Char)operand).ToString());
				return;
			}
			if (operand is Single)
			{
				this.WriteFloatOperand((Single)operand);
				return;
			}
			if (operand is Double)
			{
				this.WriteDoubleOperand((Double)operand);
				return;
			}
			if (!(operand is Boolean))
			{
				invariantCultureString = ILHelpers.ToInvariantCultureString(operand);
				this.Write(invariantCultureString);
				return;
			}
			this.WriteKeyword(((Boolean)operand ? "true" : "false"));
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
			this.WriteAttributes(p.get_CustomAttributes());
		}

		private void WriteParameters(Collection<ParameterDefinition> parameters)
		{
			for (int i = 0; i < parameters.get_Count(); i++)
			{
				ParameterDefinition item = parameters.get_Item(i);
				if (item.get_IsIn())
				{
					this.WriteKeyWordPostSpace("[in]");
				}
				if (item.get_IsOut())
				{
					this.WriteKeyWordPostSpace("[out]");
				}
				if (item.get_IsOptional())
				{
					this.WriteKeyWordPostSpace("[opt]");
				}
				this.WriteTypeReference(item.get_ParameterType(), IntermediateLanguageWriter.ILNameSyntax.Signature);
				this.WriteSpace();
				if (item.get_HasMarshalInfo())
				{
					this.WriteMarshalInfo(item.get_MarshalInfo());
				}
				this.WriteEscaped(item.get_Name());
				if (i < parameters.get_Count() - 1)
				{
					this.WriteToken(",");
				}
				this.WriteLine();
			}
		}

		private void WritePInvokeAttributes(MethodDefinition method)
		{
			PInvokeInfo pInvokeInfo = method.get_PInvokeInfo();
			this.WriteToken("(");
			this.WriteLiteralInQuotes(BaseLanguageWriter.ConvertString(pInvokeInfo.get_Module().get_Name()));
			this.WriteToken(")");
			if (!String.IsNullOrEmpty(pInvokeInfo.get_EntryPoint()) && pInvokeInfo.get_EntryPoint() != method.get_Name())
			{
				this.WriteSpace();
				this.WriteKeyword("as");
				this.WriteSpace();
				this.WriteLiteralInQuotes(BaseLanguageWriter.ConvertString(pInvokeInfo.get_EntryPoint()));
			}
			if (pInvokeInfo.get_IsNoMangle())
			{
				this.WriteKeyWordPreSpace("nomangle");
			}
			if (pInvokeInfo.get_IsCharSetAnsi())
			{
				this.WriteKeyWordPreSpace("ansi");
			}
			else if (pInvokeInfo.get_IsCharSetAuto())
			{
				this.WriteKeyWordPreSpace("autochar");
			}
			else if (pInvokeInfo.get_IsCharSetUnicode())
			{
				this.WriteKeyWordPreSpace("unicode");
			}
			if (pInvokeInfo.get_SupportsLastError())
			{
				this.WriteKeyWordPreSpace("lasterr");
			}
			if (pInvokeInfo.get_IsCallConvCdecl())
			{
				this.WriteKeyWordPreSpace("cdecl");
			}
			else if (pInvokeInfo.get_IsCallConvFastcall())
			{
				this.WriteKeyWordPreSpace("fastcall");
			}
			else if (pInvokeInfo.get_IsCallConvStdCall())
			{
				this.WriteKeyWordPreSpace("stdcall");
			}
			else if (pInvokeInfo.get_IsCallConvThiscall())
			{
				this.WriteKeyWordPreSpace("thiscall");
			}
			else if (pInvokeInfo.get_IsCallConvWinapi())
			{
				this.WriteKeyWordPreSpace("winapi");
			}
			this.WriteToken(")");
		}

		protected override void WritePropertyDeclaration(PropertyDefinition property)
		{
			this.WriteKeyWordPostSpace(".property");
			this.flagsWriter.WritePropertyFlags(property);
			if (property.get_HasThis())
			{
				this.WriteKeyWordPostSpace("instance");
			}
			this.WriteTypeReference(property.get_PropertyType(), IntermediateLanguageWriter.ILNameSyntax.Signature);
			this.WriteSpace();
			int currentPosition = this.formatter.CurrentPosition;
			this.WriteReference(ILHelpers.Escape(property.get_Name()), property);
			int num = this.formatter.CurrentPosition - 1;
			this.currentWritingInfo.MemberDeclarationToCodePostionMap[property] = new OffsetSpan(currentPosition, num);
			this.WriteToken("(");
			if (property.get_HasParameters())
			{
				this.WriteLine();
				this.Indent();
				this.WriteParameters(property.get_Parameters());
				this.Outdent();
			}
			this.WriteToken(")");
		}

		private void WriteSecurityDeclarationArgument(CustomAttributeNamedArgument na)
		{
			CustomAttributeArgument argument = na.get_Argument();
			TypeReference type = argument.get_Type();
			if (type.get_MetadataType() == 18 || type.get_MetadataType() == 17)
			{
				this.WriteKeyWordPostSpace("enum");
				if (type.get_Scope() == type.get_Module())
				{
					this.WriteTypeReference(type, IntermediateLanguageWriter.ILNameSyntax.TypeName);
				}
				else
				{
					this.WriteKeyWordPostSpace("class");
					this.Write(ILHelpers.Escape(this.GetAssemblyQualifiedName(type)));
				}
			}
			else
			{
				this.WriteTypeReference(type, IntermediateLanguageWriter.ILNameSyntax.Signature);
			}
			this.WriteSpace();
			this.WriteReference(ILHelpers.Escape(na.get_Name()), type);
			this.WriteSpace();
			this.WriteToken("=");
			this.WriteSpace();
			if (!(na.get_Argument().get_Value() is String))
			{
				argument = na.get_Argument();
				this.WriteConstant(argument.get_Value());
				return;
			}
			argument = na.get_Argument();
			this.Write(String.Format("string('{0}')", BaseLanguageWriter.ConvertString((String)argument.get_Value()).Replace("'", "'")));
		}

		private void WriteSecurityDeclarations(ISecurityDeclarationProvider secDeclProvider)
		{
			if (!secDeclProvider.get_HasSecurityDeclarations())
			{
				return;
			}
			foreach (SecurityDeclaration securityDeclaration in secDeclProvider.get_SecurityDeclarations())
			{
				this.WriteKeyWordPostSpace(".permissionset");
				switch (securityDeclaration.get_Action())
				{
					case 1:
					{
						this.WriteKeyword("request");
						break;
					}
					case 2:
					{
						this.WriteKeyword("demand");
						break;
					}
					case 3:
					{
						this.WriteKeyword("assert");
						break;
					}
					case 4:
					{
						this.WriteKeyword("deny");
						break;
					}
					case 5:
					{
						this.WriteKeyword("permitonly");
						break;
					}
					case 6:
					{
						this.WriteKeyword("linkcheck");
						break;
					}
					case 7:
					{
						this.WriteKeyword("inheritcheck");
						break;
					}
					case 8:
					{
						this.WriteKeyword("reqmin");
						break;
					}
					case 9:
					{
						this.WriteKeyword("reqopt");
						break;
					}
					case 10:
					{
						this.WriteKeyword("reqrefuse");
						break;
					}
					case 11:
					{
						this.WriteKeyword("prejitgrant");
						break;
					}
					case 12:
					{
						this.WriteKeyword("prejitdeny");
						break;
					}
					case 13:
					{
						this.WriteKeyword("noncasdemand");
						break;
					}
					case 14:
					{
						this.WriteKeyword("noncaslinkdemand");
						break;
					}
					case 15:
					{
						this.WriteKeyword("noncasinheritance");
						break;
					}
					default:
					{
						this.WriteKeyword(securityDeclaration.get_Action().ToString());
						break;
					}
				}
				this.WriteTokenPreSpace("=");
				this.WriteTokenPreSpace("{");
				this.WriteLine();
				this.Indent();
				for (int i = 0; i < securityDeclaration.get_SecurityAttributes().get_Count(); i++)
				{
					SecurityAttribute item = securityDeclaration.get_SecurityAttributes().get_Item(i);
					if (item.get_AttributeType().get_Scope() != item.get_AttributeType().get_Module())
					{
						this.WriteTypeReference(item.get_AttributeType(), IntermediateLanguageWriter.ILNameSyntax.TypeName);
					}
					else
					{
						this.WriteKeyWordPostSpace("class");
						this.WriteEscaped(this.GetAssemblyQualifiedName(item.get_AttributeType()));
					}
					this.WriteTokenPreSpace("=");
					this.WriteTokenPreSpace("{");
					if (item.get_HasFields() || item.get_HasProperties())
					{
						this.WriteLine();
						this.Indent();
						foreach (CustomAttributeNamedArgument field in item.get_Fields())
						{
							this.WriteKeyWordPostSpace("field");
							this.WriteSecurityDeclarationArgument(field);
							this.WriteLine();
						}
						foreach (CustomAttributeNamedArgument property in item.get_Properties())
						{
							this.WriteKeyWordPostSpace("property");
							this.WriteSecurityDeclarationArgument(property);
							this.WriteLine();
						}
						this.Outdent();
					}
					this.WriteToken("}");
					if (i + 1 < securityDeclaration.get_SecurityAttributes().get_Count())
					{
						this.WriteToken(",");
					}
					this.WriteLine();
				}
				this.Outdent();
				this.WriteToken("}");
				this.WriteLine();
			}
		}

		private void WriteStartBlock()
		{
			this.formatter.WriteStartBlock();
			this.WriteOpeningBraceBetweenLines();
			this.Indent();
		}

		private void WriteStructureBody(ILBlock s, HashSet<int> branchTargets, ref Instruction inst, MemberMapping currentMethodMapping, int codeSize)
		{
			bool flowControl;
			bool flag = true;
			bool flag1 = false;
			int num = 0;
			while (inst != null && inst.get_Offset() < s.EndOffset)
			{
				int offset = inst.get_Offset();
				if (num >= s.Children.Count || s.Children[num].StartOffset > offset || offset >= s.Children[num].EndOffset)
				{
					if (!flag && (flag1 || branchTargets.Contains(offset)))
					{
						this.WriteLine();
					}
					this.WriteInstruction(inst);
					if (currentMethodMapping != null)
					{
						currentMethodMapping.MemberCodeMappings.Add(new SourceCodeMapping()
						{
							ILInstructionOffset = new ILRange()
							{
								From = inst.get_Offset(),
								To = (inst.get_Next() == null ? codeSize : inst.get_Next().get_Offset())
							},
							MemberMapping = currentMethodMapping
						});
					}
					this.WriteLine();
					if (inst.get_OpCode().get_FlowControl() == null || inst.get_OpCode().get_FlowControl() == 3 || inst.get_OpCode().get_FlowControl() == 7)
					{
						flowControl = true;
					}
					else
					{
						OpCode opCode = inst.get_OpCode();
						flowControl = opCode.get_FlowControl() == 8;
					}
					flag1 = flowControl;
					inst = inst.get_Next();
				}
				else
				{
					int num1 = num;
					num = num1 + 1;
					ILBlock item = s.Children[num1];
					this.WriteStructureHeader(item);
					this.WriteStructureBody(item, branchTargets, ref inst, currentMethodMapping, codeSize);
					this.WriteStructureFooter(item);
				}
				flag = false;
			}
		}

		private void WriteStructureFooter(ILBlock s)
		{
			this.Outdent();
			switch (s.Type)
			{
				case ILBlockType.Loop:
				{
					this.WriteToken("}");
					this.WriteLine();
					return;
				}
				case ILBlockType.Try:
				{
					this.WriteToken("}");
					this.WriteLine();
					return;
				}
				case ILBlockType.Handler:
				{
					this.WriteToken("}");
					this.WriteLine();
					return;
				}
				case ILBlockType.Filter:
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
			switch (s.Type)
			{
				case ILBlockType.Loop:
				{
					Instruction loopEntryPoint = s.LoopEntryPoint;
					this.WriteKeyword(".loop");
					this.WriteOpeningBraceBetweenLines();
					break;
				}
				case ILBlockType.Try:
				{
					this.WriteKeyword(".try");
					this.WriteOpeningBraceBetweenLines();
					break;
				}
				case ILBlockType.Handler:
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
								this.WriteTypeReference(s.ExceptionHandler.get_CatchType(), IntermediateLanguageWriter.ILNameSyntax.TypeName);
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
							throw new NotSupportedException();
						}
					}
					this.WriteToken("{");
					this.WriteLine();
					break;
				}
				case ILBlockType.Filter:
				{
					this.WriteKeyword("filter");
					this.WriteOpeningBraceBetweenLines();
					break;
				}
				default:
				{
					throw new NotSupportedException();
				}
			}
			this.Indent();
		}

		private void WriteTokenPostSpace(string token)
		{
			this.WriteToken(token);
			this.WriteSpace();
		}

		private void WriteTokenPreSpace(string token)
		{
			this.WriteSpace();
			this.WriteToken(token);
		}

		protected void WriteType(TypeReference type, IntermediateLanguageWriter.ILNameSyntax syntax)
		{
			if (type is ArrayType)
			{
				this.WriteArrayType(type, syntax);
				return;
			}
			string str = ILHelpers.PrimitiveTypeName(type.GetFriendlyFullName(base.Language));
			if (syntax == IntermediateLanguageWriter.ILNameSyntax.ShortTypeName)
			{
				if (str != null)
				{
					this.WriteReference(str, type);
					return;
				}
				this.WriteReference(ILHelpers.Escape(type.get_Name()), type);
				return;
			}
			if ((syntax == IntermediateLanguageWriter.ILNameSyntax.Signature || syntax == IntermediateLanguageWriter.ILNameSyntax.SignatureNoNamedTypeParameters) && str != null)
			{
				this.WriteReference(str, type);
				return;
			}
			if (syntax == IntermediateLanguageWriter.ILNameSyntax.Signature || syntax == IntermediateLanguageWriter.ILNameSyntax.SignatureNoNamedTypeParameters)
			{
				this.WriteKeyWordPostSpace((type.get_IsValueType() ? "valuetype" : "class"));
			}
			if (type.get_DeclaringType() != null)
			{
				this.WriteTypeReference(type.get_DeclaringType(), IntermediateLanguageWriter.ILNameSyntax.TypeName);
				this.WriteKeyword("/");
				this.WriteReference(ILHelpers.Escape(type.get_Name()), type);
				return;
			}
			if (!type.get_IsDefinition() && type.get_Scope() != null && !(type is TypeSpecification))
			{
				this.WriteToken("[");
				this.WriteEscaped(type.get_Scope().get_Name());
				this.WriteToken("]");
			}
			this.WriteReference(ILHelpers.Escape(type.get_FullName()), type);
		}

		protected override string WriteTypeDeclaration(TypeDefinition type, bool isPartial = false)
		{
			if (this.CodeMappings == null)
			{
				this.CodeMappings = new Telerik.JustDecompiler.Languages.IL.CodeMappings()
				{
					FullName = type.get_FullName(),
					Mapping = new List<MemberMapping>()
				};
			}
			this.WriteKeyWordPostSpace(".class");
			if ((type.get_Attributes() & 32) == 32)
			{
				this.WriteKeyWordPostSpace("interface");
			}
			this.flagsWriter.WriteTypeVisibility(type);
			this.flagsWriter.WriteTypeLayoutFlags(type);
			this.flagsWriter.WriteTypeStringFormat(type);
			this.flagsWriter.WriteTypeAttributes(type);
			int currentPosition = this.formatter.CurrentPosition;
			this.WriteReference(ILHelpers.Escape((type.get_DeclaringType() != null ? type.get_Name() : type.get_FullName())), type);
			int num = this.formatter.CurrentPosition - 1;
			this.currentWritingInfo.MemberDeclarationToCodePostionMap[type] = new OffsetSpan(currentPosition, num);
			this.WriteTypeParameters(type);
			this.WriteLine();
			if (type.get_BaseType() != null)
			{
				this.Indent();
				this.WriteKeyWordPostSpace("extends");
				this.WriteTypeReference(type.get_BaseType(), IntermediateLanguageWriter.ILNameSyntax.TypeName);
				this.WriteLine();
				this.Outdent();
			}
			if (type.get_HasInterfaces())
			{
				this.Indent();
				for (int i = 0; i < type.get_Interfaces().get_Count(); i++)
				{
					if (i > 0)
					{
						this.WriteToken(",");
						this.WriteLine();
					}
					if (i != 0)
					{
						this.Write("           ");
					}
					else
					{
						this.WriteKeyWordPostSpace("implements");
					}
					this.WriteTypeReference(type.get_Interfaces().get_Item(i), IntermediateLanguageWriter.ILNameSyntax.TypeName);
				}
				this.WriteLine();
				this.Outdent();
			}
			return String.Empty;
		}

		protected override void WriteTypeOpeningBlock(TypeDefinition type)
		{
			this.WriteAttributes(type.get_CustomAttributes());
			this.WriteSecurityDeclarations(type);
			if (type.get_HasLayoutInfo())
			{
				this.Write(String.Format(".pack {0}", type.get_PackingSize()));
				this.WriteLine();
				this.Write(String.Format(".size {0}", type.get_ClassSize()));
				this.WriteLine();
				this.WriteLine();
			}
		}

		private void WriteTypeParameters(IGenericParameterProvider p)
		{
			if (!p.get_HasGenericParameters())
			{
				return;
			}
			this.WriteToken("<");
			for (int i = 0; i < p.get_GenericParameters().get_Count(); i++)
			{
				if (i > 0)
				{
					this.WriteTokenPostSpace(",");
				}
				this.WriteGenericTypeParameter(p.get_GenericParameters().get_Item(i));
			}
			this.WriteToken(">");
		}

		private void WriteTypeReference(TypeReference type, IntermediateLanguageWriter.ILNameSyntax syntax = 0)
		{
			IntermediateLanguageWriter.ILNameSyntax lNameSyntax = (syntax == IntermediateLanguageWriter.ILNameSyntax.SignatureNoNamedTypeParameters ? syntax : IntermediateLanguageWriter.ILNameSyntax.Signature);
			if (type is PinnedType)
			{
				this.WriteTypeReference(((PinnedType)type).get_ElementType(), lNameSyntax);
				this.WriteKeyWordPreSpace("pinned");
				return;
			}
			if (type is ArrayType)
			{
				this.WriteArrayType(type, lNameSyntax);
				return;
			}
			if (type is GenericParameter)
			{
				this.WriteGenericTypeParameter(type, syntax);
				return;
			}
			if (type is ByReferenceType)
			{
				this.WriteTypeReference(((ByReferenceType)type).get_ElementType(), lNameSyntax);
				this.WriteToken("&");
				return;
			}
			if (type is PointerType)
			{
				this.WriteTypeReference(((PointerType)type).get_ElementType(), lNameSyntax);
				this.WriteToken("*");
				return;
			}
			if (!(type is GenericInstanceType))
			{
				if (type is OptionalModifierType)
				{
					this.WriteTypeReference(((OptionalModifierType)type).get_ElementType(), syntax);
					this.WriteKeyWordPreSpace("modopt");
					this.WriteToken("(");
					this.WriteTypeReference(((OptionalModifierType)type).get_ModifierType(), IntermediateLanguageWriter.ILNameSyntax.TypeName);
					this.WriteTokenPostSpace(")");
					return;
				}
				if (!(type is RequiredModifierType))
				{
					this.WriteType(type, syntax);
					return;
				}
				this.WriteTypeReference(((RequiredModifierType)type).get_ElementType(), syntax);
				this.WriteKeyWordPreSpace("modreq");
				this.WriteToken("(");
				this.WriteTypeReference(((RequiredModifierType)type).get_ModifierType(), IntermediateLanguageWriter.ILNameSyntax.TypeName);
				this.WriteTokenPostSpace(")");
				return;
			}
			this.WriteTypeReference(type.GetElementType(), lNameSyntax);
			this.WriteToken("<");
			Collection<TypeReference> genericArguments = ((GenericInstanceType)type).get_GenericArguments();
			for (int i = 0; i < genericArguments.get_Count(); i++)
			{
				TypeReference item = genericArguments.get_Item(i);
				if (((GenericInstanceType)type).get_PostionToArgument().ContainsKey(i))
				{
					item = ((GenericInstanceType)type).get_PostionToArgument()[i];
				}
				if (i > 0)
				{
					this.WriteTokenPostSpace(",");
				}
				this.WriteTypeReference(item, lNameSyntax);
			}
			this.WriteToken(">");
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