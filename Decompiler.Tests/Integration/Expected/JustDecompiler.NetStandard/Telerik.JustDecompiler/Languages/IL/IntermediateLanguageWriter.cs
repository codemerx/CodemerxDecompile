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
			MethodDefinition method = body.Method;
			if (method.DeclaringType.Module.Assembly.EntryPoint == method)
			{
				this.WriteKeyword(".entrypoint");
				this.WriteLine();
			}
			if (method.Body.HasVariables)
			{
				this.WriteMethodVariables(method);
				this.WriteLine();
			}
			if (base.Settings.ShouldGenerateBlocks && body.Instructions.Count > 0)
			{
				Instruction item = body.Instructions[0];
				HashSet<int> branchTargets = this.GetBranchTargets(body.Instructions);
				this.WriteStructureBody(new ILBlock(body), branchTargets, ref item, methodMapping, method.Body.CodeSize);
				return;
			}
			foreach (Instruction instruction in method.Body.Instructions)
			{
				this.WriteInstruction(instruction);
				if (methodMapping != null)
				{
					methodMapping.MemberCodeMappings.Add(new SourceCodeMapping()
					{
						ILInstructionOffset = new ILRange()
						{
							From = instruction.Offset,
							To = (instruction.Next == null ? method.Body.CodeSize : instruction.Next.Offset)
						},
						MemberMapping = methodMapping
					});
				}
				this.WriteLine();
			}
			if (method.Body.HasExceptionHandlers)
			{
				this.WriteLine();
				foreach (ExceptionHandler exceptionHandler in method.Body.ExceptionHandlers)
				{
					this.WriteExceptionHandler(exceptionHandler);
					this.WriteLine();
				}
			}
		}

		private string GetAssemblyQualifiedName(TypeReference type)
		{
			AssemblyNameReference scope = type.Scope as AssemblyNameReference;
			if (scope == null)
			{
				ModuleDefinition moduleDefinition = type.Scope as ModuleDefinition;
				if (moduleDefinition != null)
				{
					scope = moduleDefinition.Assembly.Name;
				}
			}
			if (scope == null)
			{
				return type.FullName;
			}
			return String.Concat(type.FullName, ", ", scope.FullName);
		}

		private HashSet<int> GetBranchTargets(IEnumerable<Instruction> instructions)
		{
			HashSet<int> nums = new HashSet<int>();
			foreach (Instruction instruction in instructions)
			{
				Instruction operand = instruction.Operand as Instruction;
				if (operand != null)
				{
					nums.Add(operand.Offset);
				}
				Instruction[] instructionArray = instruction.Operand as Instruction[];
				if (instructionArray == null)
				{
					continue;
				}
				Instruction[] instructionArray1 = instructionArray;
				for (int i = 0; i < (int)instructionArray1.Length; i++)
				{
					nums.Add(instructionArray1[i].Offset);
				}
			}
			return nums;
		}

		private string GetFullName(object member)
		{
			string empty = String.Empty;
			if (member is IMemberDefinition)
			{
				empty = (base.Settings.RenameInvalidMembers ? ((IMemberDefinition)member).GetFriendlyFullName(base.Language) : ((IMemberDefinition)member).FullName);
			}
			else if (member is ParameterReference)
			{
				empty = (base.Settings.RenameInvalidMembers ? base.Language.ReplaceInvalidCharactersInIdentifier(((ParameterReference)member).Name) : ((ParameterReference)member).Name);
			}
			else if (!(member is MemberReference))
			{
				empty = (base.Settings.RenameInvalidMembers ? base.Language.ReplaceInvalidCharactersInIdentifier(member.ToString()) : member.ToString());
			}
			else
			{
				empty = (base.Settings.RenameInvalidMembers ? ((MemberReference)member).GetFriendlyFullName(base.Language) : ((MemberReference)member).FullName);
			}
			return empty;
		}

		private string GetName(object member)
		{
			string empty = String.Empty;
			if (member is IMemberDefinition)
			{
				empty = ((IMemberDefinition)member).Name;
			}
			else if (!(member is ParameterReference))
			{
				empty = (!(member is MemberReference) ? member.ToString() : ((MemberReference)member).Name);
			}
			else
			{
				empty = ((ParameterReference)member).Name;
			}
			return empty;
		}

		private bool HasParameterAttributes(ParameterDefinition p)
		{
			if (p.HasConstant)
			{
				return true;
			}
			return p.HasCustomAttributes;
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
			this.WriteAttributes(@event.CustomAttributes);
			this.WriteNestedMethod(".addon", @event.AddMethod);
			this.WriteLine();
			this.WriteNestedMethod(".removeon", @event.RemoveMethod);
			this.WriteNestedMethod(".fire", @event.InvokeMethod);
			foreach (MethodDefinition otherMethod in @event.OtherMethods)
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
			this.WriteAttributes(property.CustomAttributes);
			this.WriteNestedMethod(".get", property.GetMethod);
			if (property.GetMethod != null)
			{
				this.WriteLine();
			}
			this.WriteNestedMethod(".set", property.SetMethod);
			if (property.SetMethod != null)
			{
				this.WriteLine();
			}
			bool flag = false;
			foreach (MethodDefinition otherMethod in property.OtherMethods)
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
			if (field.HasMarshalInfo)
			{
				this.WriteMarshalInfo(field.MarshalInfo);
			}
			this.WriteTypeReference(field.FieldType, IntermediateLanguageWriter.ILNameSyntax.Signature);
			this.WriteSpace();
			int currentPosition = this.formatter.CurrentPosition;
			this.WriteReference(ILHelpers.Escape(field.Name), field);
			int num = this.formatter.CurrentPosition - 1;
			this.currentWritingInfo.MemberDeclarationToCodePostionMap[field] = new OffsetSpan(currentPosition, num);
			if ((field.Attributes & FieldAttributes.HasFieldRVA) == FieldAttributes.HasFieldRVA)
			{
				this.Write(String.Format(" at I_{0:x8}", field.RVA));
			}
			if (field.HasConstant)
			{
				this.WriteSpace();
				this.WriteToken("=");
				this.WriteSpace();
				this.WriteConstant(field.Constant.Value);
			}
			if (field.HasCustomAttributes && this.WriteAttributes(field.CustomAttributes))
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
			this.WriteTypeReference(arrayType.ElementType, syntaxForElementTypes);
			this.WriteToken("[");
			for (int i = 0; i < arrayType.Dimensions.Count; i++)
			{
				if (i > 0)
				{
					this.WriteTokenPostSpace(",");
				}
				this.WriteToken(arrayType.Dimensions[i].ToString());
			}
			this.WriteToken("]");
		}

		private bool WriteAttributes(Collection<CustomAttribute> attributes)
		{
			if (attributes.Count == 0)
			{
				return false;
			}
			foreach (CustomAttribute attribute in attributes)
			{
				this.WriteKeyWordPostSpace(".custom");
				this.WriteMethodReference(attribute.Constructor, true);
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
			this.WriteTypeReference(@event.EventType, IntermediateLanguageWriter.ILNameSyntax.TypeName);
			this.WriteSpace();
			int currentPosition = this.formatter.CurrentPosition;
			this.WriteReference(ILHelpers.Escape(@event.Name), @event);
			int num = this.formatter.CurrentPosition - 1;
			this.currentWritingInfo.MemberDeclarationToCodePostionMap[@event] = new OffsetSpan(currentPosition, num);
		}

		private void WriteExceptionHandler(ExceptionHandler exceptionHandler)
		{
			this.WriteKeyWordPostSpace("Try");
			this.WriteOffsetReference(exceptionHandler.TryStart);
			this.WriteToken("-");
			this.WriteOffsetReference(exceptionHandler.TryEnd);
			this.WriteSpace();
			this.WriteKeyword(exceptionHandler.HandlerType.ToString());
			if (exceptionHandler.FilterStart != null)
			{
				this.WriteSpace();
				this.WriteOffsetReference(exceptionHandler.FilterStart);
				this.WriteSpace();
				this.WriteKeyword("handler");
				this.WriteSpace();
			}
			if (exceptionHandler.CatchType != null)
			{
				this.WriteSpace();
				this.WriteTypeReference(exceptionHandler.CatchType, IntermediateLanguageWriter.ILNameSyntax.Signature);
			}
			this.WriteSpace();
			this.WriteOffsetReference(exceptionHandler.HandlerStart);
			this.WriteSpace();
			this.WriteOffsetReference(exceptionHandler.HandlerEnd);
		}

		private void WriteFieldReference(FieldReference field)
		{
			this.WriteTypeReference(field.FieldType, IntermediateLanguageWriter.ILNameSyntax.SignatureNoNamedTypeParameters);
			this.WriteSpace();
			this.WriteTypeReference(field.DeclaringType, IntermediateLanguageWriter.ILNameSyntax.TypeName);
			this.WriteToken("::");
			this.WriteReference(ILHelpers.Escape(field.Name), field);
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
			if (gp.HasReferenceTypeConstraint)
			{
				this.WriteKeyWordPostSpace("class");
			}
			else if (gp.HasNotNullableValueTypeConstraint)
			{
				this.WriteKeyWordPostSpace("valuetype");
			}
			if (gp.HasDefaultConstructorConstraint)
			{
				this.WriteKeyWordPostSpace(".ctor");
			}
			if (gp.HasConstraints)
			{
				this.WriteToken("(");
				for (int i = 0; i < gp.Constraints.Count; i++)
				{
					if (i > 0)
					{
						this.WriteTokenPostSpace(",");
					}
					this.WriteTypeReference(gp.Constraints[i], IntermediateLanguageWriter.ILNameSyntax.TypeName);
				}
				this.WriteTokenPostSpace(")");
			}
			if (gp.IsContravariant)
			{
				this.WriteToken("-");
			}
			else if (gp.IsCovariant)
			{
				this.WriteToken("+");
			}
			this.WriteEscaped(gp.Name);
		}

		private void WriteGenericTypeParameter(TypeReference type, IntermediateLanguageWriter.ILNameSyntax syntax)
		{
			this.WriteToken("!");
			if (((GenericParameter)type).Owner.GenericParameterType == GenericParameterType.Method)
			{
				this.WriteToken("!");
			}
			if (!String.IsNullOrEmpty(type.Name) && type.Name[0] != '!' && syntax != IntermediateLanguageWriter.ILNameSyntax.SignatureNoNamedTypeParameters)
			{
				this.WriteReference(ILHelpers.Escape(type.Name), null);
				return;
			}
			this.WriteReference(((GenericParameter)type).Position.ToString(), null);
		}

		private void WriteInstruction(Instruction instruction)
		{
			this.WriteDefinition(this.OffsetToString(instruction.Offset), instruction);
			this.WriteTokenPostSpace(":");
			this.WriteReference(instruction.OpCode.Name, instruction.OpCode);
			if (instruction.Operand != null)
			{
				this.WriteSpace();
				if (instruction.OpCode == OpCodes.Ldtoken)
				{
					if (instruction.Operand is MethodReference)
					{
						this.WriteKeyWordPostSpace("method");
					}
					else if (instruction.Operand is FieldReference)
					{
						this.WriteKeyWordPostSpace("field");
					}
				}
				this.WriteOperand(instruction.Operand);
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
			this.WriteNativeType(marshalInfo.NativeType, marshalInfo);
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
				this.formatter.Write((base.Settings.RenameInvalidMembers ? typeReference.GetFriendlyTypeName(base.Language, "<", ">") : typeReference.Name));
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
				obj3 = (base.Settings.RenameInvalidMembers ? methodDefinition.ReturnType.GetFriendlyFullName(base.Language) : methodDefinition.ReturnType.FullName);
				formatter.Write(String.Format("{0} : {1}", (object)str, obj3));
				return;
			}
			if (memberDefinition is PropertyDefinition)
			{
				PropertyDefinition propertyDefinition = memberDefinition as PropertyDefinition;
				IFormatter formatter1 = this.formatter;
				string str1 = fullName;
				obj2 = (base.Settings.RenameInvalidMembers ? propertyDefinition.PropertyType.GetFriendlyFullName(base.Language) : propertyDefinition.PropertyType.FullName);
				formatter1.Write(String.Format("{0} : {1}", (object)str1, obj2));
				return;
			}
			if (memberDefinition is FieldDefinition)
			{
				FieldDefinition fieldDefinition = memberDefinition as FieldDefinition;
				IFormatter formatter2 = this.formatter;
				string str2 = fullName;
				obj1 = (base.Settings.RenameInvalidMembers ? fieldDefinition.FieldType.GetFriendlyFullName(base.Language) : fieldDefinition.FieldType.FullName);
				formatter2.Write(String.Format("{0} : {1}", (object)str2, obj1));
				return;
			}
			if (memberDefinition is EventDefinition)
			{
				EventDefinition eventDefinition = memberDefinition as EventDefinition;
				IFormatter formatter3 = this.formatter;
				string str3 = fullName;
				obj = (base.Settings.RenameInvalidMembers ? eventDefinition.EventType.GetFriendlyFullName(base.Language) : eventDefinition.EventType.FullName);
				formatter3.Write(String.Format("{0} : {1}", (object)str3, obj));
				return;
			}
			if (memberDefinition is ParameterReference)
			{
				this.formatter.Write(((ParameterReference)memberDefinition).Name);
				return;
			}
			if (memberDefinition is MemberReference)
			{
				this.formatter.Write((base.Settings.RenameInvalidMembers ? ((MemberReference)memberDefinition).GetFriendlyFullName(base.Language) : ((MemberReference)memberDefinition).FullName));
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
				this.formatter.Write(((TypeReference)member).Name);
				return;
			}
			string[] strArray = this.GetName(member).Split(new String[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
			if ((int)strArray.Length > 1)
			{
				string str = strArray[1];
			}
			if (member is ParameterReference)
			{
				this.formatter.Write(((ParameterReference)member).Name);
				return;
			}
			if (member is MemberReference)
			{
				this.formatter.Write(((MemberReference)member).GetFriendlyFullName(base.Language));
			}
		}

		private void WriteMethodBody(MethodDefinition method)
		{
			this.WriteAttributes(method.CustomAttributes);
			if (method.HasOverrides)
			{
				foreach (MethodReference @override in method.Overrides)
				{
					this.WriteKeyWordPostSpace(".override");
					this.WriteKeyWordPostSpace("method");
					this.WriteMethodReference(@override, true);
					this.WriteLine();
				}
			}
			foreach (ParameterDefinition parameter in method.Parameters)
			{
				this.WriteParameterAttributes(parameter);
			}
			this.WriteSecurityDeclarations(method);
			if (method.HasBody)
			{
				MemberMapping memberMapping = ILHelpers.CreateCodeMapping(method, this.CodeMappings);
				this.Disassemble(method.Body, memberMapping);
			}
		}

		protected override void WriteMethodDeclaration(MethodDefinition method, bool writeDocumentation = false)
		{
			this.WriteKeyWordPostSpace(".method");
			this.flagsWriter.WriteMethodVisibility(method);
			this.flagsWriter.WriteMethodFlags(method);
			if (method.IsCompilerControlled)
			{
				this.WriteKeyWordPostSpace("privatescope");
			}
			if ((method.Attributes & MethodAttributes.PInvokeImpl) == MethodAttributes.PInvokeImpl)
			{
				this.WriteKeyword("pinvokeimpl");
				if (method.HasPInvokeInfo && method.PInvokeInfo != null)
				{
					this.WritePInvokeAttributes(method);
				}
				this.WriteSpace();
			}
			this.Indent();
			if (method.ExplicitThis)
			{
				this.WriteKeyWordPostSpace("instance");
				this.WriteKeyWordPostSpace("explicit");
			}
			else if (method.HasThis)
			{
				this.WriteKeyWordPostSpace("instance");
			}
			this.flagsWriter.WriteMethodCallingConvention(method);
			this.WriteTypeReference(method.ReturnType, IntermediateLanguageWriter.ILNameSyntax.Signature);
			this.WriteSpace();
			if (method.MethodReturnType.HasMarshalInfo)
			{
				this.WriteMarshalInfo(method.MethodReturnType.MarshalInfo);
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
			// File path: C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\Decompiler.Tests\bin\Release\netcoreapp2.1\JustDecompiler.NetStandard.dll
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
			if (!method.IsCompilerControlled)
			{
				this.WriteReference(ILHelpers.Escape(method.Name), method);
				return;
			}
			string name = method.Name;
			int num = method.MetadataToken.ToInt32();
			this.WriteReference(ILHelpers.Escape(String.Concat(name, "$PST", num.ToString("X8"))), method);
		}

		private void WriteMethodName(MethodReference method, bool writeAsReference)
		{
			string str;
			MethodDefinition methodDefinition = method as MethodDefinition;
			if (methodDefinition == null || !methodDefinition.IsCompilerControlled)
			{
				str = ILHelpers.Escape(method.Name);
			}
			else
			{
				string name = method.Name;
				int num = method.MetadataToken.ToInt32();
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
			if (method.HasParameters)
			{
				this.WriteLine();
				this.Indent();
				this.WriteParameters(method.Parameters);
				this.Outdent();
			}
			this.WriteTokenPostSpace(")");
		}

		protected void WriteMethodReference(MethodReference method, bool writeAsReference)
		{
			if (method.ExplicitThis)
			{
				this.WriteKeyWordPostSpace("instance");
				this.WriteKeyWordPostSpace("explicit");
			}
			else if (method.HasThis)
			{
				this.WriteKeyWordPostSpace("instance");
			}
			this.WriteTypeReference(method.FixedReturnType, IntermediateLanguageWriter.ILNameSyntax.SignatureNoNamedTypeParameters);
			this.WriteSpace();
			if (method.DeclaringType != null)
			{
				this.WriteTypeReference(method.DeclaringType, IntermediateLanguageWriter.ILNameSyntax.TypeName);
				this.WriteToken("::");
			}
			this.WriteMethodName(method, writeAsReference);
			GenericInstanceMethod genericInstanceMethod = method as GenericInstanceMethod;
			if (genericInstanceMethod != null)
			{
				this.WriteToken("<");
				for (int i = 0; i < genericInstanceMethod.GenericArguments.Count; i++)
				{
					if (i > 0)
					{
						this.WriteTokenPostSpace(",");
					}
					this.WriteTypeReference(genericInstanceMethod.GenericArguments[i], IntermediateLanguageWriter.ILNameSyntax.Signature);
				}
				this.WriteToken(">");
			}
			this.WriteToken("(");
			Collection<ParameterDefinition> parameters = method.Parameters;
			for (int j = 0; j < parameters.Count; j++)
			{
				if (j > 0)
				{
					this.WriteTokenPostSpace(", ");
				}
				this.WriteTypeReference(parameters[j].ParameterType, IntermediateLanguageWriter.ILNameSyntax.SignatureNoNamedTypeParameters);
			}
			this.WriteToken(")");
		}

		private void WriteMethodVariables(MethodDefinition method)
		{
			this.WriteKeyWordPostSpace(".locals");
			if (method.Body.InitLocals)
			{
				this.WriteKeyWordPostSpace("init");
			}
			this.WriteToken("(");
			this.WriteLine();
			this.Indent();
			foreach (VariableDefinition variable in method.Body.Variables)
			{
				int index = variable.Index;
				this.WriteDefinition(String.Concat("[", index.ToString(), "] "), variable);
				this.WriteTypeReference(variable.VariableType, IntermediateLanguageWriter.ILNameSyntax.Signature);
				if (!String.IsNullOrEmpty(variable.Name))
				{
					this.WriteSpace();
					this.Write(ILHelpers.Escape(variable.Name));
				}
				if (variable.Index + 1 < method.Body.Variables.Count)
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
				case NativeType.Boolean:
				{
					this.WriteKeyword("bool");
					return;
				}
				case NativeType.I1:
				{
					this.WriteKeyword("int8");
					return;
				}
				case NativeType.U1:
				{
					this.WriteKeyword("unsigned int8");
					return;
				}
				case NativeType.I2:
				{
					this.WriteKeyword("int16");
					return;
				}
				case NativeType.U2:
				{
					this.WriteKeyword("unsigned int16");
					return;
				}
				case NativeType.I4:
				{
					this.WriteKeyword("int32");
					return;
				}
				case NativeType.U4:
				{
					this.WriteKeyword("unsigned int32");
					return;
				}
				case NativeType.I8:
				{
					this.WriteKeyword("int64");
					return;
				}
				case NativeType.U8:
				{
					this.WriteKeyword("unsigned int64");
					return;
				}
				case NativeType.R4:
				{
					this.WriteKeyword("float32");
					return;
				}
				case NativeType.R8:
				{
					this.WriteKeyword("float64");
					return;
				}
				case NativeType.U1 | NativeType.I2 | NativeType.U4 | NativeType.I8 | NativeType.R8:
				case NativeType.Boolean | NativeType.U1 | NativeType.U2 | NativeType.U4 | NativeType.U8 | NativeType.R8:
				case 16:
				case 17:
				case 18:
				case 24:
				case 33:
				case NativeType.Func:
				case NativeType.Boolean | NativeType.I1 | NativeType.U1 | NativeType.I2 | NativeType.U2 | NativeType.I4 | NativeType.UInt | NativeType.Func | NativeType.ByValStr | NativeType.ANSIBStr | NativeType.TBStr | NativeType.VariantBool:
				case NativeType.U4 | NativeType.I8 | NativeType.UInt | NativeType.ASAny:
				{
					this.WriteKeyword(nativeType.ToString());
					break;
				}
				case NativeType.Currency:
				{
					this.WriteKeyword("currency");
					return;
				}
				case NativeType.BStr:
				{
					this.WriteKeyword("bstr");
					return;
				}
				case NativeType.LPStr:
				{
					this.WriteKeyword("lpstr");
					return;
				}
				case NativeType.LPWStr:
				{
					this.WriteKeyword("lpwstr");
					return;
				}
				case NativeType.LPTStr:
				{
					this.WriteKeyword("lptstr");
					return;
				}
				case NativeType.FixedSysString:
				{
					this.WriteKeyword(String.Format("fixed sysstring[{0}]", ((FixedSysStringMarshalInfo)marshalInfo).Size));
					return;
				}
				case NativeType.IUnknown:
				{
					this.WriteKeyword("iunknown");
					return;
				}
				case NativeType.IDispatch:
				{
					this.WriteKeyword("idispatch");
					return;
				}
				case NativeType.Struct:
				{
					this.WriteKeyword("struct");
					return;
				}
				case NativeType.IntF:
				{
					this.WriteKeyword("interface");
					return;
				}
				case NativeType.SafeArray:
				{
					this.WriteKeyWordPostSpace("safearray");
					SafeArrayMarshalInfo safeArrayMarshalInfo = marshalInfo as SafeArrayMarshalInfo;
					if (safeArrayMarshalInfo == null)
					{
						break;
					}
					switch (safeArrayMarshalInfo.ElementType)
					{
						case VariantType.None:
						{
							break;
						}
						case 1:
						case VariantType.I2 | VariantType.I4 | VariantType.R4 | VariantType.R8 | VariantType.CY | VariantType.Date | VariantType.BStr | VariantType.Dispatch | VariantType.Error | VariantType.Bool | VariantType.Variant | VariantType.Unknown | VariantType.Decimal:
						case VariantType.R4 | VariantType.I1:
						case VariantType.R4 | VariantType.R8 | VariantType.I1 | VariantType.UI1:
						{
							this.WriteKeyword(safeArrayMarshalInfo.ElementType.ToString());
							return;
						}
						case VariantType.I2:
						{
							this.WriteKeyword("int16");
							return;
						}
						case VariantType.I4:
						{
							this.WriteKeyword("int32");
							return;
						}
						case VariantType.R4:
						{
							this.WriteKeyword("float32");
							return;
						}
						case VariantType.R8:
						{
							this.WriteKeyword("float64");
							return;
						}
						case VariantType.CY:
						{
							this.WriteKeyword("currency");
							return;
						}
						case VariantType.Date:
						{
							this.WriteKeyword("date");
							return;
						}
						case VariantType.BStr:
						{
							this.WriteKeyword("bstr");
							return;
						}
						case VariantType.Dispatch:
						{
							this.WriteKeyword("idispatch");
							return;
						}
						case VariantType.Error:
						{
							this.WriteKeyword("error");
							return;
						}
						case VariantType.Bool:
						{
							this.WriteKeyword("bool");
							return;
						}
						case VariantType.Variant:
						{
							this.WriteKeyword("variant");
							return;
						}
						case VariantType.Unknown:
						{
							this.WriteKeyword("iunknown");
							return;
						}
						case VariantType.Decimal:
						{
							this.WriteKeyword("decimal");
							return;
						}
						case VariantType.I1:
						{
							this.WriteKeyword("int8");
							return;
						}
						case VariantType.UI1:
						{
							this.WriteKeyword("unsigned int8");
							return;
						}
						case VariantType.UI2:
						{
							this.WriteKeyword("unsigned int16");
							return;
						}
						case VariantType.UI4:
						{
							this.WriteKeyword("unsigned int32");
							return;
						}
						case VariantType.Int:
						{
							this.WriteKeyword("int");
							return;
						}
						case VariantType.UInt:
						{
							this.WriteKeyword("unsigned int");
							return;
						}
						default:
						{
							this.WriteKeyword(safeArrayMarshalInfo.ElementType.ToString());
							return;
						}
					}
					break;
				}
				case NativeType.FixedArray:
				{
					this.WriteKeyword("fixed array");
					FixedArrayMarshalInfo fixedArrayMarshalInfo = marshalInfo as FixedArrayMarshalInfo;
					if (fixedArrayMarshalInfo == null)
					{
						break;
					}
					this.WriteToken("[");
					this.WriteLiteral(fixedArrayMarshalInfo.Size.ToString());
					this.WriteToken("]");
					if (fixedArrayMarshalInfo.ElementType == NativeType.None)
					{
						break;
					}
					this.WriteSpace();
					this.WriteNativeType(fixedArrayMarshalInfo.ElementType, null);
					return;
				}
				case NativeType.Int:
				{
					this.WriteKeyword("int");
					return;
				}
				case NativeType.UInt:
				{
					this.WriteKeyword("unsigned int");
					return;
				}
				case NativeType.ByValStr:
				{
					this.WriteKeyword("byvalstr");
					return;
				}
				case NativeType.ANSIBStr:
				{
					this.WriteKeyword("ansi bstr");
					return;
				}
				case NativeType.TBStr:
				{
					this.WriteKeyword("tbstr");
					return;
				}
				case NativeType.VariantBool:
				{
					this.WriteKeyword("variant bool");
					return;
				}
				case NativeType.ASAny:
				{
					this.WriteKeyword("as any");
					return;
				}
				case NativeType.Array:
				{
					ArrayMarshalInfo arrayMarshalInfo = (ArrayMarshalInfo)marshalInfo;
					if (arrayMarshalInfo == null)
					{
						goto case NativeType.U4 | NativeType.I8 | NativeType.UInt | NativeType.ASAny;
					}
					if (arrayMarshalInfo.ElementType != NativeType.Max)
					{
						this.WriteNativeType(arrayMarshalInfo.ElementType, null);
					}
					this.WriteToken("[");
					if (arrayMarshalInfo.SizeParameterMultiplier != 0)
					{
						if (arrayMarshalInfo.Size >= 0)
						{
							this.WriteLiteral(arrayMarshalInfo.Size.ToString());
						}
						this.WriteSpace();
						this.WriteToken("+");
						this.WriteSpace();
						this.WriteLiteral(arrayMarshalInfo.SizeParameterIndex.ToString());
					}
					else
					{
						this.WriteLiteral(arrayMarshalInfo.Size.ToString());
					}
					this.WriteToken("]");
					return;
				}
				case NativeType.LPStruct:
				{
					this.WriteKeyword("lpstruct");
					return;
				}
				case NativeType.CustomMarshaler:
				{
					CustomMarshalInfo customMarshalInfo = marshalInfo as CustomMarshalInfo;
					if (customMarshalInfo == null)
					{
						goto case NativeType.U4 | NativeType.I8 | NativeType.UInt | NativeType.ASAny;
					}
					this.WriteKeyword("custom");
					this.WriteToken("(");
					this.WriteLiteralInQuotes(BaseLanguageWriter.ConvertString(customMarshalInfo.ManagedType.FullName));
					this.WriteTokenPostSpace(",");
					this.WriteLiteralInQuotes(BaseLanguageWriter.ConvertString(customMarshalInfo.Cookie));
					if (customMarshalInfo.Guid != Guid.Empty || !String.IsNullOrEmpty(customMarshalInfo.UnmanagedType))
					{
						this.WriteTokenPostSpace(",");
						this.WriteLiteralInQuotes(customMarshalInfo.Guid.ToString());
						this.WriteTokenPostSpace(",");
						this.WriteLiteralInQuotes(BaseLanguageWriter.ConvertString(customMarshalInfo.UnmanagedType));
					}
					this.WriteToken(")");
					return;
				}
				case NativeType.Error:
				{
					this.WriteKeyword("error");
					return;
				}
				default:
				{
					if (nativeType == NativeType.None)
					{
						break;
					}
					goto case NativeType.U4 | NativeType.I8 | NativeType.UInt | NativeType.ASAny;
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
			this.Write(this.OffsetToString(instruction.Offset));
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
				if (!String.IsNullOrEmpty(variableReference.Name))
				{
					this.WriteReference(ILHelpers.Escape(variableReference.Name), variableReference);
					return;
				}
				index = variableReference.Index;
				this.WriteReference(index.ToString(), variableReference);
				return;
			}
			ParameterReference parameterReference = operand as ParameterReference;
			if (parameterReference != null)
			{
				if (this.method != null && this.method.Body != null && parameterReference == this.method.Body.ThisParameter)
				{
					this.WriteReference(0.ToString(), parameterReference);
					return;
				}
				if (!String.IsNullOrEmpty(parameterReference.Name))
				{
					this.WriteReference(ILHelpers.Escape(parameterReference.Name), parameterReference);
					return;
				}
				index = parameterReference.Index;
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
			this.WriteToken(String.Format("[{0}]", p.Index + 1));
			if (p.HasConstant)
			{
				this.WriteSpace();
				this.WriteToken("=");
				this.WriteSpace();
				this.WriteConstant(p.Constant.Value);
			}
			this.WriteLine();
			this.WriteAttributes(p.CustomAttributes);
		}

		private void WriteParameters(Collection<ParameterDefinition> parameters)
		{
			for (int i = 0; i < parameters.Count; i++)
			{
				ParameterDefinition item = parameters[i];
				if (item.IsIn)
				{
					this.WriteKeyWordPostSpace("[in]");
				}
				if (item.IsOut)
				{
					this.WriteKeyWordPostSpace("[out]");
				}
				if (item.IsOptional)
				{
					this.WriteKeyWordPostSpace("[opt]");
				}
				this.WriteTypeReference(item.ParameterType, IntermediateLanguageWriter.ILNameSyntax.Signature);
				this.WriteSpace();
				if (item.HasMarshalInfo)
				{
					this.WriteMarshalInfo(item.MarshalInfo);
				}
				this.WriteEscaped(item.Name);
				if (i < parameters.Count - 1)
				{
					this.WriteToken(",");
				}
				this.WriteLine();
			}
		}

		private void WritePInvokeAttributes(MethodDefinition method)
		{
			PInvokeInfo pInvokeInfo = method.PInvokeInfo;
			this.WriteToken("(");
			this.WriteLiteralInQuotes(BaseLanguageWriter.ConvertString(pInvokeInfo.Module.Name));
			this.WriteToken(")");
			if (!String.IsNullOrEmpty(pInvokeInfo.EntryPoint) && pInvokeInfo.EntryPoint != method.Name)
			{
				this.WriteSpace();
				this.WriteKeyword("as");
				this.WriteSpace();
				this.WriteLiteralInQuotes(BaseLanguageWriter.ConvertString(pInvokeInfo.EntryPoint));
			}
			if (pInvokeInfo.IsNoMangle)
			{
				this.WriteKeyWordPreSpace("nomangle");
			}
			if (pInvokeInfo.IsCharSetAnsi)
			{
				this.WriteKeyWordPreSpace("ansi");
			}
			else if (pInvokeInfo.IsCharSetAuto)
			{
				this.WriteKeyWordPreSpace("autochar");
			}
			else if (pInvokeInfo.IsCharSetUnicode)
			{
				this.WriteKeyWordPreSpace("unicode");
			}
			if (pInvokeInfo.SupportsLastError)
			{
				this.WriteKeyWordPreSpace("lasterr");
			}
			if (pInvokeInfo.IsCallConvCdecl)
			{
				this.WriteKeyWordPreSpace("cdecl");
			}
			else if (pInvokeInfo.IsCallConvFastcall)
			{
				this.WriteKeyWordPreSpace("fastcall");
			}
			else if (pInvokeInfo.IsCallConvStdCall)
			{
				this.WriteKeyWordPreSpace("stdcall");
			}
			else if (pInvokeInfo.IsCallConvThiscall)
			{
				this.WriteKeyWordPreSpace("thiscall");
			}
			else if (pInvokeInfo.IsCallConvWinapi)
			{
				this.WriteKeyWordPreSpace("winapi");
			}
			this.WriteToken(")");
		}

		protected override void WritePropertyDeclaration(PropertyDefinition property)
		{
			this.WriteKeyWordPostSpace(".property");
			this.flagsWriter.WritePropertyFlags(property);
			if (property.HasThis)
			{
				this.WriteKeyWordPostSpace("instance");
			}
			this.WriteTypeReference(property.PropertyType, IntermediateLanguageWriter.ILNameSyntax.Signature);
			this.WriteSpace();
			int currentPosition = this.formatter.CurrentPosition;
			this.WriteReference(ILHelpers.Escape(property.Name), property);
			int num = this.formatter.CurrentPosition - 1;
			this.currentWritingInfo.MemberDeclarationToCodePostionMap[property] = new OffsetSpan(currentPosition, num);
			this.WriteToken("(");
			if (property.HasParameters)
			{
				this.WriteLine();
				this.Indent();
				this.WriteParameters(property.Parameters);
				this.Outdent();
			}
			this.WriteToken(")");
		}

		private void WriteSecurityDeclarationArgument(CustomAttributeNamedArgument na)
		{
			CustomAttributeArgument argument = na.Argument;
			TypeReference type = argument.Type;
			if (type.MetadataType == MetadataType.Class || type.MetadataType == MetadataType.ValueType)
			{
				this.WriteKeyWordPostSpace("enum");
				if (type.Scope == type.Module)
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
			this.WriteReference(ILHelpers.Escape(na.Name), type);
			this.WriteSpace();
			this.WriteToken("=");
			this.WriteSpace();
			if (!(na.Argument.Value is String))
			{
				argument = na.Argument;
				this.WriteConstant(argument.Value);
				return;
			}
			argument = na.Argument;
			this.Write(String.Format("string('{0}')", BaseLanguageWriter.ConvertString((String)argument.Value).Replace("'", "'")));
		}

		private void WriteSecurityDeclarations(ISecurityDeclarationProvider secDeclProvider)
		{
			if (!secDeclProvider.HasSecurityDeclarations)
			{
				return;
			}
			foreach (SecurityDeclaration securityDeclaration in secDeclProvider.SecurityDeclarations)
			{
				this.WriteKeyWordPostSpace(".permissionset");
				switch (securityDeclaration.Action)
				{
					case SecurityAction.Request:
					{
						this.WriteKeyword("request");
						break;
					}
					case SecurityAction.Demand:
					{
						this.WriteKeyword("demand");
						break;
					}
					case SecurityAction.Assert:
					{
						this.WriteKeyword("assert");
						break;
					}
					case SecurityAction.Deny:
					{
						this.WriteKeyword("deny");
						break;
					}
					case SecurityAction.PermitOnly:
					{
						this.WriteKeyword("permitonly");
						break;
					}
					case SecurityAction.LinkDemand:
					{
						this.WriteKeyword("linkcheck");
						break;
					}
					case SecurityAction.InheritDemand:
					{
						this.WriteKeyword("inheritcheck");
						break;
					}
					case SecurityAction.RequestMinimum:
					{
						this.WriteKeyword("reqmin");
						break;
					}
					case SecurityAction.RequestOptional:
					{
						this.WriteKeyword("reqopt");
						break;
					}
					case SecurityAction.RequestRefuse:
					{
						this.WriteKeyword("reqrefuse");
						break;
					}
					case SecurityAction.PreJitGrant:
					{
						this.WriteKeyword("prejitgrant");
						break;
					}
					case SecurityAction.PreJitDeny:
					{
						this.WriteKeyword("prejitdeny");
						break;
					}
					case SecurityAction.NonCasDemand:
					{
						this.WriteKeyword("noncasdemand");
						break;
					}
					case SecurityAction.NonCasLinkDemand:
					{
						this.WriteKeyword("noncaslinkdemand");
						break;
					}
					case SecurityAction.NonCasInheritance:
					{
						this.WriteKeyword("noncasinheritance");
						break;
					}
					default:
					{
						this.WriteKeyword(securityDeclaration.Action.ToString());
						break;
					}
				}
				this.WriteTokenPreSpace("=");
				this.WriteTokenPreSpace("{");
				this.WriteLine();
				this.Indent();
				for (int i = 0; i < securityDeclaration.SecurityAttributes.Count; i++)
				{
					SecurityAttribute item = securityDeclaration.SecurityAttributes[i];
					if (item.AttributeType.Scope != item.AttributeType.Module)
					{
						this.WriteTypeReference(item.AttributeType, IntermediateLanguageWriter.ILNameSyntax.TypeName);
					}
					else
					{
						this.WriteKeyWordPostSpace("class");
						this.WriteEscaped(this.GetAssemblyQualifiedName(item.AttributeType));
					}
					this.WriteTokenPreSpace("=");
					this.WriteTokenPreSpace("{");
					if (item.HasFields || item.HasProperties)
					{
						this.WriteLine();
						this.Indent();
						foreach (CustomAttributeNamedArgument field in item.Fields)
						{
							this.WriteKeyWordPostSpace("field");
							this.WriteSecurityDeclarationArgument(field);
							this.WriteLine();
						}
						foreach (CustomAttributeNamedArgument property in item.Properties)
						{
							this.WriteKeyWordPostSpace("property");
							this.WriteSecurityDeclarationArgument(property);
							this.WriteLine();
						}
						this.Outdent();
					}
					this.WriteToken("}");
					if (i + 1 < securityDeclaration.SecurityAttributes.Count)
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
			bool flag = true;
			bool flag1 = false;
			int num = 0;
			while (inst != null && inst.Offset < s.EndOffset)
			{
				int offset = inst.Offset;
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
								From = inst.Offset,
								To = (inst.Next == null ? codeSize : inst.Next.Offset)
							},
							MemberMapping = currentMethodMapping
						});
					}
					this.WriteLine();
					flag1 = (inst.OpCode.FlowControl == FlowControl.Branch || inst.OpCode.FlowControl == FlowControl.Cond_Branch || inst.OpCode.FlowControl == FlowControl.Return ? true : inst.OpCode.FlowControl == FlowControl.Throw);
					inst = inst.Next;
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
					switch (s.ExceptionHandler.HandlerType)
					{
						case ExceptionHandlerType.Catch:
						case ExceptionHandlerType.Filter:
						{
							this.WriteKeyword("catch");
							if (s.ExceptionHandler.CatchType != null)
							{
								this.WriteSpace();
								this.WriteTypeReference(s.ExceptionHandler.CatchType, IntermediateLanguageWriter.ILNameSyntax.TypeName);
							}
							this.WriteLine();
							break;
						}
						case ExceptionHandlerType.Finally:
						{
							this.WriteKeyword("finally");
							this.WriteLine();
							break;
						}
						case ExceptionHandlerType.Filter | ExceptionHandlerType.Finally:
						{
							throw new NotSupportedException();
						}
						case ExceptionHandlerType.Fault:
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
				this.WriteReference(ILHelpers.Escape(type.Name), type);
				return;
			}
			if ((syntax == IntermediateLanguageWriter.ILNameSyntax.Signature || syntax == IntermediateLanguageWriter.ILNameSyntax.SignatureNoNamedTypeParameters) && str != null)
			{
				this.WriteReference(str, type);
				return;
			}
			if (syntax == IntermediateLanguageWriter.ILNameSyntax.Signature || syntax == IntermediateLanguageWriter.ILNameSyntax.SignatureNoNamedTypeParameters)
			{
				this.WriteKeyWordPostSpace((type.IsValueType ? "valuetype" : "class"));
			}
			if (type.DeclaringType != null)
			{
				this.WriteTypeReference(type.DeclaringType, IntermediateLanguageWriter.ILNameSyntax.TypeName);
				this.WriteKeyword("/");
				this.WriteReference(ILHelpers.Escape(type.Name), type);
				return;
			}
			if (!type.IsDefinition && type.Scope != null && !(type is TypeSpecification))
			{
				this.WriteToken("[");
				this.WriteEscaped(type.Scope.Name);
				this.WriteToken("]");
			}
			this.WriteReference(ILHelpers.Escape(type.FullName), type);
		}

		protected override string WriteTypeDeclaration(TypeDefinition type, bool isPartial = false)
		{
			if (this.CodeMappings == null)
			{
				this.CodeMappings = new Telerik.JustDecompiler.Languages.IL.CodeMappings()
				{
					FullName = type.FullName,
					Mapping = new List<MemberMapping>()
				};
			}
			this.WriteKeyWordPostSpace(".class");
			if ((type.Attributes & TypeAttributes.ClassSemanticMask) == TypeAttributes.ClassSemanticMask)
			{
				this.WriteKeyWordPostSpace("interface");
			}
			this.flagsWriter.WriteTypeVisibility(type);
			this.flagsWriter.WriteTypeLayoutFlags(type);
			this.flagsWriter.WriteTypeStringFormat(type);
			this.flagsWriter.WriteTypeAttributes(type);
			int currentPosition = this.formatter.CurrentPosition;
			this.WriteReference(ILHelpers.Escape((type.DeclaringType != null ? type.Name : type.FullName)), type);
			int num = this.formatter.CurrentPosition - 1;
			this.currentWritingInfo.MemberDeclarationToCodePostionMap[type] = new OffsetSpan(currentPosition, num);
			this.WriteTypeParameters(type);
			this.WriteLine();
			if (type.BaseType != null)
			{
				this.Indent();
				this.WriteKeyWordPostSpace("extends");
				this.WriteTypeReference(type.BaseType, IntermediateLanguageWriter.ILNameSyntax.TypeName);
				this.WriteLine();
				this.Outdent();
			}
			if (type.HasInterfaces)
			{
				this.Indent();
				for (int i = 0; i < type.Interfaces.Count; i++)
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
					this.WriteTypeReference(type.Interfaces[i], IntermediateLanguageWriter.ILNameSyntax.TypeName);
				}
				this.WriteLine();
				this.Outdent();
			}
			return String.Empty;
		}

		protected override void WriteTypeOpeningBlock(TypeDefinition type)
		{
			this.WriteAttributes(type.CustomAttributes);
			this.WriteSecurityDeclarations(type);
			if (type.HasLayoutInfo)
			{
				this.Write(String.Format(".pack {0}", type.PackingSize));
				this.WriteLine();
				this.Write(String.Format(".size {0}", type.ClassSize));
				this.WriteLine();
				this.WriteLine();
			}
		}

		private void WriteTypeParameters(IGenericParameterProvider p)
		{
			if (!p.HasGenericParameters)
			{
				return;
			}
			this.WriteToken("<");
			for (int i = 0; i < p.GenericParameters.Count; i++)
			{
				if (i > 0)
				{
					this.WriteTokenPostSpace(",");
				}
				this.WriteGenericTypeParameter(p.GenericParameters[i]);
			}
			this.WriteToken(">");
		}

		private void WriteTypeReference(TypeReference type, IntermediateLanguageWriter.ILNameSyntax syntax = 0)
		{
			IntermediateLanguageWriter.ILNameSyntax lNameSyntax = (syntax == IntermediateLanguageWriter.ILNameSyntax.SignatureNoNamedTypeParameters ? syntax : IntermediateLanguageWriter.ILNameSyntax.Signature);
			if (type is PinnedType)
			{
				this.WriteTypeReference(((PinnedType)type).ElementType, lNameSyntax);
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
				this.WriteTypeReference(((ByReferenceType)type).ElementType, lNameSyntax);
				this.WriteToken("&");
				return;
			}
			if (type is PointerType)
			{
				this.WriteTypeReference(((PointerType)type).ElementType, lNameSyntax);
				this.WriteToken("*");
				return;
			}
			if (!(type is GenericInstanceType))
			{
				if (type is OptionalModifierType)
				{
					this.WriteTypeReference(((OptionalModifierType)type).ElementType, syntax);
					this.WriteKeyWordPreSpace("modopt");
					this.WriteToken("(");
					this.WriteTypeReference(((OptionalModifierType)type).ModifierType, IntermediateLanguageWriter.ILNameSyntax.TypeName);
					this.WriteTokenPostSpace(")");
					return;
				}
				if (!(type is RequiredModifierType))
				{
					this.WriteType(type, syntax);
					return;
				}
				this.WriteTypeReference(((RequiredModifierType)type).ElementType, syntax);
				this.WriteKeyWordPreSpace("modreq");
				this.WriteToken("(");
				this.WriteTypeReference(((RequiredModifierType)type).ModifierType, IntermediateLanguageWriter.ILNameSyntax.TypeName);
				this.WriteTokenPostSpace(")");
				return;
			}
			this.WriteTypeReference(type.GetElementType(), lNameSyntax);
			this.WriteToken("<");
			Collection<TypeReference> genericArguments = ((GenericInstanceType)type).GenericArguments;
			for (int i = 0; i < genericArguments.Count; i++)
			{
				TypeReference item = genericArguments[i];
				if (((GenericInstanceType)type).PostionToArgument.ContainsKey(i))
				{
					item = ((GenericInstanceType)type).PostionToArgument[i];
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