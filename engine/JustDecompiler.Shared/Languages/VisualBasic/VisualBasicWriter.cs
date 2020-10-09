#region license
//
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion-
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using Telerik.JustDecompiler.Ast;
using Mono.Cecil;
using System.Collections.Generic;
using System.Text;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Mono.Cecil.Extensions;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Decompiler;
using Mono.Collections.Generic;
/* AGPL */
using JustDecompiler.Shared;
/* End AGPL */

namespace Telerik.JustDecompiler.Languages.VisualBasic
{
    public class VisualBasicWriter : NamespaceImperativeLanguageWriter
    {
        readonly Stack<StatementState> statementStates = new Stack<StatementState>();

        public VisualBasicWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
            : base(language, formatter, exceptionFormatter, settings)
        {
        }

		protected override void WriteDestructor(MethodDefinition method)
		{
			WriteMethod(method);
		}

		protected override bool WritePropertyAsIndexer(PropertyDefinition property)
		{
            return false;
        }

        protected override void WriteIndexerKeywords()
        {
            WriteKeyword("Default");
            WriteSpace();
        }

        protected override AttributeWriter CreateAttributeWriter()
        {
            return new VisualBasicAttributeWriter(this);
        }

        public override void VisitTryStatement(TryStatement node)
        {
            WriteKeyword(KeyWordWriter.Try);
            WriteLine();
            Visit(node.Try);

            //if (node.CatchClauses.Count != 0)
            //{
            //    Visit(node.CatchClauses);
            //}

            foreach (ICodeNode @catch in node.CatchClauses)
            {
                Visit(@catch);
            }

            if (node.Finally != null)
            {
                WriteKeyword(KeyWordWriter.Finally);
                WriteLine();
                Visit(node.Finally);
            }
            WriteSpecialEndBlock(KeyWordWriter.Try);
        }

        public override void VisitIfStatement(IfStatement node)
        {
            WriteKeyword(KeyWordWriter.If);
            WriteSpace();
            WriteBetweenParenthesis(node.Condition);
            if (KeyWordWriter.Then != null)
            {
                WriteSpace();
                WriteKeyword(KeyWordWriter.Then);
            }
            WriteLine();

            Visit(node.Then);

            if (node.Else == null)
            {
                WriteSpecialEndBlock(KeyWordWriter.If);
                return;
            }

            WriteKeyword(KeyWordWriter.Else);
            WriteLine();

            Visit(node.Else);
            WriteSpecialEndBlock(KeyWordWriter.If);
        }

        public override void VisitIfElseIfStatement(IfElseIfStatement node)
        {
            for (int i = 0; i < node.ConditionBlocks.Count; i++)
            {
                if (i == 0)
                {
                    WriteKeyword(KeyWordWriter.If);
                }
                else
                {

                    WriteKeyword(KeyWordWriter.ElseIf);
                }

                WriteSpace();
                WriteBetweenParenthesis(node.ConditionBlocks[i].Key);
                if (KeyWordWriter.Then != null)
                {
                    WriteSpace();
                    WriteKeyword(KeyWordWriter.Then);
                }
                WriteLine();

                Visit(node.ConditionBlocks[i].Value);
            }

            if (node.Else != null)
            {
                WriteKeyword(KeyWordWriter.Else);
                WriteLine();

                Visit(node.Else);
            }

            WriteSpecialEndBlock(KeyWordWriter.If);
        }

        protected override IKeyWordWriter CreateKeyWordWriter()
        {
            return new VisualBasicKeyWordWriter();
        }

        protected override void WriteEndBlock(string statementName)
        {
            WriteEndBlockWithoutNewLine(statementName);
            // WriteLine();
        }

        private void WriteEndBlockWithoutNewLine(string statementName)
        {
            WriteKeyword("End");
            WriteSpace();
            WriteKeyword(statementName);
        }

        protected override void WriteSpecialEndBlock(string statementName)
        {
            WriteEndBlock(statementName);
        }

		protected override void WriteEnumBaseTypeInheritColon()
		{
			WriteAsBetweenSpaces();
		}

        protected override void WriteBaseTypeInheritColon()
        {
            WriteLine();
            WriteKeyword("Inherits");
            WriteSpace();
        }

        protected override void WriteInterfacesInheritColon(TypeDefinition type)
        {
            WriteLine();

			if (type.IsInterface)
			{
				WriteKeyword(KeyWordWriter.Inherits);
			}
			else
			{
				WriteKeyword(KeyWordWriter.Implements);
			}

            WriteSpace();
        }

		protected override void WriteTypeStaticKeywordAndSpace()
		{
			// there aren't static types in VB
		}

        protected override bool WriteTypeBaseTypes(TypeDefinition type, bool isPartial = false)
        {
            Indent();
            bool result = base.WriteTypeBaseTypes(type, isPartial);
            Outdent();
            return result;
        }

        //protected override void WriteTypeBaseTypesAndInterfaces(TypeDefinition type, bool isPartial)
        //{
        //    bool baseTypeWritten = false;
        //    if (!type.IsEnum && !type.IsValueType)
        //    {
        //        baseTypeWritten = WriteTypeBaseTypes(type, isPartial);
        //    }
        //    if (!type.IsEnum)
        //    {
        //        if (type.Interfaces.Count > 0)
        //        {
        //            Indent();
        //            WriteInterfacesInheritColon();
        //            Outdent();
        //        }
        //        WriteTypeInterfaces(type, isPartial);
        //    }
        //}

        protected override void WriteTypeInterfaces(TypeDefinition type, bool isPartial, bool baseTypeWritten)
        {
            List<TypeReference> interfaces;
            if (TypeContext.CurrentType == type && TypeContext.IsWinRTImplementation)
            {
                interfaces = type.Interfaces.Where(@interface =>
                {
                    TypeDefinition interfaceDef = @interface.Resolve();
                    return interfaceDef == null || !interfaceDef.IsWindowsRuntime;
                }).ToList();
            }
            else
            {
                interfaces = type.Interfaces.ToList();
            }

            if (interfaces.Count > 0)
            {
                int interfacesWritten = 0;
                for (int index = 0; index < interfaces.Count; index++)
                {
                    if (!isPartial || (IsImplemented(type, interfaces[index].Resolve())))
                    {
                        if (interfacesWritten == 0)
                        {
                            Indent();
                            WriteInterfacesInheritColon(type);
                            Outdent();
                        }
                        if (interfacesWritten > 0)
                        {
                            WriteInheritComma();
                        }
                        interfacesWritten++;
                        WriteReferenceAndNamespaceIfInCollision(interfaces[index]);
                    }
                }
            }
        }

        protected override void WriteSpecialBetweenParenthesis(Expression expression)
        {
            Visit(expression);
        }

        protected override void WriteSpecialBetweenParenthesis(Action action)
        {
            WriteToken("(");
            action();
            WriteToken(")");
        }

        protected void WriteDim()
        {
            if (statementStates.Count > 0)
            {
                var lastState = statementStates.Peek();
                if ((lastState == StatementState.ForEachInitializer) ||
                    (lastState == StatementState.ForInitializer) ||
                    (lastState == StatementState.Catch) ||
                    (lastState == StatementState.Using))
                    return;
            }

            WriteKeyword(KeyWordWriter.Dim);
            WriteSpace();
        }

        public override string ToTypeString(TypeReference type)
        {
            /// There might be user generated classes with this name. Only the ones declared in mscorlib should be replaced by the language keyword.
            if (IsReferenceFromMscorlib(type))
            {
                switch (type.Name)
                {
                    case "Decimal":
                        return "Decimal";
                    case "Single":
                        return "Single";
                    case "Byte":
                        return "Byte";
                    case "SByte":
                        return "SByte";
                    case "Char":
                        return "Char";
                    case "Double":
                        return "Double";
                    case "Boolean":
                        return "Boolean";
                    case "Int16":
                        return "Short";
                    case "Int32":
                        return "Integer";
                    case "Int64":
                        return "Long";
                    case "UInt16":
                        return "UShort";
                    case "UInt32":
                        return "UInteger";
                    case "UInt64":
                        return "ULong";
                    case "String":
                        return "String";
                    case "Void":
                        return "Void";
                    case "Object":
                        return "Object";
                    case "RuntimeArgumentHandle":
                        return string.Empty;
                }
            }
            return GetGenericNameFromMemberReference(type);
        }


        private string GetGenericNameFromMemberReference(TypeReference type)
        {
            IGenericDefinition genericDefinition = type.Resolve();
            if (genericDefinition != null)
            {
                return genericDefinition.GetGenericName(Language, "(Of ", ")");
            }
            else
            {
                return type.GetGenericName(Language, "(Of ", ")");
            }
        }

        /* AGPL */
        protected override void DoWriteTypeAndName(TypeReference typeReference, string name, object reference, TypeReferenceType typeReferenceType)
        /* End AGPL */
        {
            int startIndex = this.formatter.CurrentPosition;
            /* AGPL */
            CodeSpan codeSpan = this.Write(() =>
            {
                if (reference != null)
                {
                    WriteReference(name, reference);
                }
                else
                {
                    Write(name);
                }
            });
            /* End AGPL */
            if (reference is IMemberDefinition)
            {
                int endIndex = this.formatter.CurrentPosition - 1;
                /* AGPL */
                this.currentWritingInfo.MemberDeclarationToCodeSpan[(IMemberDefinition)reference] = codeSpan;
                /* End AGPL */
                this.currentWritingInfo.MemberDeclarationToCodePostionMap[(IMemberDefinition)reference] = new OffsetSpan(startIndex, endIndex);
            }
            WriteAsBetweenSpaces();

            /* AGPL */
            CodeSpan typeCodeSpan = this.Write(() => WriteReferenceAndNamespaceIfInCollision(typeReference));

            if (reference is IMemberDefinition referenceAsMemberDefinition)
            {
                this.AddMemberDefinitionTypeCodeSpanToCache(referenceAsMemberDefinition, typeReferenceType, typeCodeSpan);
            }
            else if (reference is VariableDefinition variableReference)
            {
                this.currentWritingInfo.CodeMappingInfo.VariableDefinitionToVariableTypeCodeMap[variableReference] = typeCodeSpan;
            }
            /* End AGPL */
        }

        /* AGPL */
        protected override void DoWriteTypeAndName(TypeReference typeReference, string name, TypeReferenceType typeReferenceType)
        /* End AGPL */
        {
            Write(name);
            WriteAsBetweenSpaces();
            WriteReferenceAndNamespaceIfInCollision(typeReference);
        }

        protected override void DoWriteVariableTypeAndName(VariableDefinition variable)
        {
            if (this.isWritingComment)
            {
                Write(GetVariableName(variable));
            }
            else
            {
                this.WriteAndMapVariableToCode(() => Write(GetVariableName(variable)), variable);
            }

            WriteAsBetweenSpaces();

            /* AGPL */
            CodeSpan typeCodeSpan = this.Write(() => WriteReferenceAndNamespaceIfInCollision(variable.VariableType));
            this.currentWritingInfo.CodeMappingInfo.VariableDefinitionToVariableTypeCodeMap[variable] = typeCodeSpan;
            /* End AGPL */
        }

        protected override void DoWriteParameterTypeAndName(TypeReference type, string name, ParameterDefinition reference)
        {
            if (reference.IsParamArray())
            {
                WriteKeyword(KeyWordWriter.ParamArray);
                WriteSpace();
            }

			if (!Language.IsValidIdentifier(name))
			{
				name = Language.ReplaceInvalidCharactersInIdentifier(name);
			}

			if (Language.IsGlobalKeyword(name))
			{
				name = Utilities.EscapeNameIfNeeded(name, this.Language);
			}

            if (this.isWritingComment)
            {
                Write(name);
            }
            else
            {
                this.WriteAndMapParameterToCode(() => Write(name), reference.Index);
            }

            // undocumented C# keyword like __arglist

            if (!string.IsNullOrEmpty(ToTypeString(type)))
            {
                WriteAsBetweenSpaces();
                /* AGPL */
                CodeSpan typeCodeSpan = this.Write(() => WriteReferenceAndNamespaceIfInCollision(type));
                this.currentWritingInfo.CodeMappingInfo.ParameterDefinitionToParameterTypeCodeMap[reference] = typeCodeSpan;
                /* End AGPL */
            }
        }

        protected override void WriteBlock(Action action, string label)
        {
            //formatter.WriteStartBlock();

            Indent();

            if (label != "")
            {
                WriteLabel(label);
            }

            action();

            Outdent();

            //formatter.WriteEndBlock();
        }

		protected override void WritePropertyTypeAndNameWithArguments(PropertyDefinition property)
		{
			int startIndex = this.formatter.CurrentPosition;

			string name = GetPropertyName(property);
            /* AGPL */
            CodeSpan codeSpan = this.Write(() => WriteReference(name, property));
            /* End AGPL */
            int endIndex = this.formatter.CurrentPosition - 1;
            /* AGPL */
            this.currentWritingInfo.MemberDeclarationToCodeSpan[property] = codeSpan;
            /* End AGPL */
            this.currentWritingInfo.MemberDeclarationToCodePostionMap[property] = new OffsetSpan(startIndex, endIndex);

			if (HasArguments(property))
			{
				WriteToken("(");
				WritePropertyParameters(property);
				WriteToken(")");
			}

			WriteAsBetweenSpaces();
            this.AttributeWriter.WriteMemberReturnValueAttributes(property.GetMethod);
			WriteReferenceAndNamespaceIfInCollision(property.PropertyType);
		}

		private ICollection<ImplementedMember> GetNotExplicitlyImplementedMembers(ICollection<ImplementedMember> implementedMembers)
		{
			ICollection<ImplementedMember> result = new List<ImplementedMember>();

			foreach (ImplementedMember implementedMember in implementedMembers)
			{
				if (!this.TypeContext.ExplicitlyImplementedMembers.Contains(implementedMember.DeclaringType, implementedMember.Member.FullName))
				{
					result.Add(implementedMember);
				}
			}

			return result;
		}

		protected override void WritePropertyInterfaceImplementations(PropertyDefinition property)
		{
			ICollection<ImplementedMember> implementedProperties = property.GetImplementedProperties();

			if (property.IsExplicitImplementation())
			{
				if (implementedProperties.Any())
				{
					WriteImplementedMembers(implementedProperties);
				}
			}
			else
			{
				ICollection<ImplementedMember> notExplicitlyImplementedProperties = GetNotExplicitlyImplementedMembers(implementedProperties);

				if (notExplicitlyImplementedProperties.Any())
				{
					WriteImplementedMembers(notExplicitlyImplementedProperties);
				}
			}
		}

        protected override void WriteReadOnlyWriteOnlyProperty(PropertyDefinition property)
        {
            if (property.GetMethod == null)
            {
                WriteKeyword(KeyWordWriter.WriteOnly);
                WriteSpace();
                return;
            }
            if (property.SetMethod == null)
            {
                WriteKeyword(KeyWordWriter.ReadOnly);
                WriteSpace();
                return;
            }
        }

		private void WriteImplementedMembers(ICollection<ImplementedMember> implementedMembers)
		{
            List<ImplementedMember> implementedMembersList;
            if (TypeContext.IsWinRTImplementation)
            {
                implementedMembersList = implementedMembers.Where(member =>
                {
                    TypeDefinition declaringTypeDef = member.DeclaringType.Resolve();
                    return declaringTypeDef == null || !declaringTypeDef.IsInterface || !declaringTypeDef.IsWindowsRuntime;
                }).ToList();
                if (implementedMembersList.Count == 0)
                {
                    return;
                }
            }
            else
            {
                implementedMembersList = implementedMembers.ToList();
            }

			WriteSpace();
			WriteKeyword(KeyWordWriter.Implements);
			WriteSpace();

			bool isFirst = true;
            foreach (ImplementedMember implementedMember in implementedMembersList)
			{
				if (!isFirst)
				{
					Write(",");
					WriteSpace();
				}

				WriteReferenceAndNamespaceIfInCollision(implementedMember.DeclaringType);
				Write(".");

				string implementedMemberName = GetMemberName(implementedMember.Member);

				WriteReference(implementedMemberName, implementedMember.Member);

				isFirst = false;
			}
		}

		protected override void WriteEventInterfaceImplementations(EventDefinition @event)
		{
			ICollection<ImplementedMember> implementedEvents = @event.GetImplementedEvents();

			if (@event.IsExplicitImplementation())
			{
				if (implementedEvents.Any())
				{
					WriteImplementedMembers(implementedEvents);
				}
			}
			else
			{
				ICollection<ImplementedMember> notExplicitlyImplementedEvents = GetNotExplicitlyImplementedMembers(implementedEvents);

				if (notExplicitlyImplementedEvents.Any())
				{
					WriteImplementedMembers(notExplicitlyImplementedEvents);
				}
			}
		}

		protected override void WriteEventTypeAndName(EventDefinition @event)
		{
			if (@event.IsExplicitImplementation())
			{
				string eventName = GetEventName(@event);
                /* AGPL */
                WriteTypeAndName(@event.EventType, eventName, @event, TypeReferenceType.EventType);
                /* End AGPL */
            }
            else
			{
				base.WriteEventTypeAndName(@event);
			}
		}

		protected override void WriteEventAddOnParameters(EventDefinition @event)
		{
			if (@event.AddMethod.Parameters.Any())
			{
				Write("(");
				WriteParameters(@event.AddMethod);
				Write(")");
			}
		}

		protected override void WriteEventRemoveOnParameters(EventDefinition @event)
		{
			if (@event.RemoveMethod.Parameters.Any())
			{
				Write("(");
				WriteParameters(@event.RemoveMethod);
				Write(")");
			}
		}

        protected override void PostWriteMethodReturnType(MethodDefinition method)
        {
            if ((method.ReturnType != null) && (method.ReturnType.FullName != Constants.Void))
            {
                WriteAsBetweenSpaces();
                this.AttributeWriter.WriteMemberReturnValueAttributes(method);
                WriteMethodReturnType(method);
            }
        }

		protected override void WriteMethodInterfaceImplementations(MethodDefinition method)
		{
			ICollection<ImplementedMember> implementedMethods = method.GetImplementedMethods();

			if (method.IsExplicitImplementation())
			{
				if (implementedMethods.Any())
				{
					WriteImplementedMembers(implementedMethods);
				}
			}
			else
			{
				ICollection<ImplementedMember> notExplicitlyImplementedMethods = GetNotExplicitlyImplementedMembers(implementedMethods);

				if (notExplicitlyImplementedMethods.Any())
				{
					WriteImplementedMembers(notExplicitlyImplementedMethods);
				}
			}
		}

		protected override void WriteOutOrRefKeyWord(ParameterDefinition parameter)
		{
			WriteKeyword(KeyWordWriter.ByRef);
		}

        protected override string CharStart
        {
            get { return "\""; }
        }

        protected override string CharEnd
        {
            get { return "\"C"; }
        }

        protected override string HexValuePrefix
        {
            get { return "&H"; }
        }

        public override string IndexLeftBracket
        {
            get { return "("; }
        }

        public override string IndexRightBracket
        {
            get { return ")"; }
        }

		public override void WriteMemberNavigationPathFullName(object member)
		{
			if (member is ParameterReference)
			{
				this.formatter.Write(((ParameterReference)member).Name);
			}
			else if (member is TypeReference)
			{
				this.formatter.Write(((TypeReference)member).GetUIFriendlyTypeNameInVB(Language));
			}
			else if (member is MemberReference)
			{
				this.formatter.Write(((MemberReference)member).GetFriendlyFullNameInVB(Language));
			}
		}

        //public override void WriteMemberEscapedOnlyName(object memberDefinition)
        //{
        //    string name = GetMemberEscapedOnlyName(memberDefinition, "(Of ",")");

        //    this.formatter.Write(name);

        //    //if (memberDefinition is TypeDefinition)
        //    //{
        //    //    this.formatter.Write(((TypeDefinition)memberDefinition).GetUIFriendlyTypeNameInVB(Language));
        //    //}
        //    //else if (memberDefinition is MethodDefinition)
        //    //{
        //    //    var methodDefinition = memberDefinition as MethodDefinition;

        //    //    string value = methodDefinition.IsConstructor ? methodDefinition.GetFriendlyMemberNameInVB(null) : methodDefinition.GetFriendlyMemberNameInVB(Language);

        //    //    this.formatter.Write(value);
        //    //}
        //    //else if (memberDefinition is PropertyDefinition)
        //    //{
        //    //    var property = memberDefinition as PropertyDefinition;

        //    //    string value = string.Format(property.Name);

        //    //    this.formatter.Write(value);
        //    //}
        //    //else if (memberDefinition is FieldDefinition)
        //    //{
        //    //    var field = memberDefinition as FieldDefinition;

        //    //    this.formatter.Write(field.GetFriendlyMemberNameInVB(Language));
        //    //}
        //    //else if (memberDefinition is EventDefinition)
        //    //{
        //    //    var @event = memberDefinition as EventDefinition;

        //    //    this.formatter.Write(@event.GetFriendlyMemberNameInVB(Language));
        //    //}
        //    //else if (memberDefinition is ParameterReference)
        //    //{
        //    //    this.formatter.Write(((ParameterReference)memberDefinition).Name);
        //    //}
        //    //else if (memberDefinition is TypeReference)
        //    //{
        //    //    this.formatter.Write(((TypeReference)memberDefinition).GetUIFriendlyTypeNameInVB(Language));
        //    //}
        //    //else if (memberDefinition is MemberReference)
        //    //{
        //    //    this.formatter.Write(((MemberReference)memberDefinition).Name);
        //    //}
        //}

        protected override string GenericLeftBracket
        {
            get { return "(Of "; }
        }

        protected override string GenericRightBracket
        {
            get { return ")"; }
        }

        protected override string ToString(BinaryOperator op, bool isOneSideNull = false)
        {
            switch (op)
            {
                case BinaryOperator.Add:
                    return "+";
                case BinaryOperator.BitwiseAnd:
                    return "And";
                case BinaryOperator.BitwiseOr:
                    return "Or";
                case BinaryOperator.BitwiseXor:
                    return "Xor";
                case BinaryOperator.Divide:
                    return "/";
                case BinaryOperator.GreaterThan:
                    return ">";
                case BinaryOperator.GreaterThanOrEqual:
                    return ">=";
                case BinaryOperator.LeftShift:
                    return "<<";
                case BinaryOperator.LessThan:
                    return "<";
                case BinaryOperator.LessThanOrEqual:
                    return "<=";
                case BinaryOperator.LogicalAnd:
                    return "AndAlso";
                case BinaryOperator.LogicalOr:
                    return "OrElse";
                case BinaryOperator.Modulo:
                    return "Mod";
                case BinaryOperator.Multiply:
                    return "*";
                case BinaryOperator.RightShift:
                    return ">>";
                case BinaryOperator.Subtract:
                    return "-";
                case BinaryOperator.ValueEquality:
					if (isOneSideNull)
					{
						return "Is";
					}
					else
					{
						return "=";
					}
                case BinaryOperator.ValueInequality:
					if (isOneSideNull)
					{
						return "IsNot";
					}
					else
					{
						return "<>";
					}
                case BinaryOperator.Assign:
                    return "=";
                case BinaryOperator.AddAssign:
                    return "+=";
                case BinaryOperator.SubtractAssign:
                    return "-=";
                case BinaryOperator.MultiplyAssign:
                    return "*=";
                case BinaryOperator.DivideAssign:
                    return "/=";
                case BinaryOperator.LeftShiftAssign:
                    return "<<=";
                case BinaryOperator.RightShiftAssign:
                    return ">>=";
                default:
                    throw new ArgumentException();
            }
        }

        protected override string ToString(UnaryOperator op)
        {
            // TODO: We need better handling for post and pre decrements in VB.NET.
            // If it's a separate expression statement we should decompile it as +/- 1
            // and if it's used inside of an expression we should decompile it as
            // System.Threading.Interlocked.Increment
            switch (op)
            {
                case UnaryOperator.BitwiseNot:
                    return "Not ";
                case UnaryOperator.LogicalNot:
                    return "Not ";
                case UnaryOperator.Negate:
                    return "-";
                case UnaryOperator.PostDecrement:
                    return " - 1";
                case UnaryOperator.PreDecrement:
                    return "1 - ";
                case UnaryOperator.PostIncrement:
                    return " + 1";
                case UnaryOperator.PreIncrement:
                    return "1 + ";
                case UnaryOperator.UnaryPlus:
                    return "+";
                case UnaryOperator.None:
                    return string.Empty;
                default:
                    throw new ArgumentException(string.Format("The unary opperator {0} is not supported in VisualBasic", op));
            }
        }

        protected override void WriteVolatileType(TypeReference reference)
        {
            WriteReferenceAndNamespaceIfInCollision(reference);
            WriteSpace();
            WriteKeyword(KeyWordWriter.Volatile);
        }

        private void VisitAddressOfExpression(UnaryExpression node)
        {
            if (MethodReferences.Count == 0)
            {
                WriteKeyword(KeyWordWriter.AddressOf);
                WriteSpace();
            }
            Visit(node.Operand);
        }

        private void WriteAsBetweenSpaces()
        {
            WriteSpace();
            WriteKeyword(KeyWordWriter.As);
            WriteSpace();
        }

        public override void VisitConditionExpression(ConditionExpression node)
        {
            WriteKeyword(KeyWordWriter.If);
            WriteToken("(");
            Visit(node.Condition);
            WriteToken(",");
            WriteSpace();
            Visit(node.Then);
            WriteToken(",");
            WriteSpace();
            Visit(node.Else);
            WriteToken(")");
        }

		private bool IsArrayCreationToVariableDeclarationAssignment(BinaryExpression node)
		{
			return node.Operator == BinaryOperator.Assign && node.Left.CodeNodeType == CodeNodeType.ArrayVariableCreationExpression && node.Right.CodeNodeType == CodeNodeType.ArrayCreationExpression;
		}

		private bool IsArrayCreationToVariableReferenceAssignment(BinaryExpression node)
		{
			return node.Operator == BinaryOperator.Assign && node.Left.CodeNodeType == CodeNodeType.ArrayAssignmentVariableReferenceExpression && node.Right.CodeNodeType == CodeNodeType.ArrayCreationExpression;
		}

		private bool IsArrayCreationToFieldReferenceAssignment(BinaryExpression node)
		{
			return node.Operator == BinaryOperator.Assign && node.Left.CodeNodeType == CodeNodeType.ArrayAssignmentFieldReferenceExpression && node.Right.CodeNodeType == CodeNodeType.ArrayCreationExpression;
		}

		private bool IsArrayCreationAssignment(BinaryExpression node)
		{
			return IsArrayCreationToVariableDeclarationAssignment(node) || IsArrayCreationToVariableReferenceAssignment(node) || IsArrayCreationToFieldReferenceAssignment(node);
		}

        public override void VisitBinaryExpression(BinaryExpression node)
        {
            if (node.Operator == BinaryOperator.NullCoalesce)
            {
                VisitNullCoalesceExpression(node);
                return;
            }

            if (IsMulticastDelegate(node.Left))
            {
                string keyword = "";
                if (node.Operator == BinaryOperator.AddAssign)
                {
                    keyword = "AddHandler";
                }
                else if (node.Operator == BinaryOperator.SubtractAssign)
                {
                    keyword = "RemoveHandler";
                }
                else
                {
                    base.VisitBinaryExpression(node);
                    return;
                }
                WriteKeyword(keyword);
                WriteSpace();
                Visit(node.Left);
                Write(",");
                WriteSpace();
                //WriteKeyword(KeyWordWriter.AddressOf);
                WriteSpace();
                Visit(node.Right);
                return;
            }

			if (IsArrayCreationAssignment(node))
			{
				Visit(node.Left);

				ArrayCreationExpression arrayCreation = node.Right as ArrayCreationExpression;
				if (Utilities.IsInitializerPresent(arrayCreation.Initializer))
				{
					WriteSpace();
					Write(ToString(BinaryOperator.Assign));

					if (IsArrayCreationToVariableDeclarationAssignment(node))
					{
						StartInitializer(arrayCreation.Initializer);
						Visit(arrayCreation.Initializer);
					}
					else
					{
						WriteSpace();
						Visit(node.Right);
					}
				}

				return;
			}

            base.VisitBinaryExpression(node);
        }

        protected override void WriteDelegateCreation(ObjectCreationExpression node)
        {
            WriteKeyword(KeyWordWriter.New);
            WriteSpace();
            WriteReferenceAndNamespaceIfInCollision(node.ExpressionType);
            WriteToken("(");
            WriteKeyword(KeyWordWriter.AddressOf);
            WriteSpace();
            WriteDelegateArgument(node);
            WriteToken(")");
        }

        private bool IsMulticastDelegate(Expression expression)
        {
            if (expression is EventReferenceExpression)
            {
                return true;
            }
            TypeDefinition td = expression.ExpressionType.Resolve();
            if (td == null)
            {
                return false;
            }
            if (td.BaseType != null && td.BaseType.Name == "MulticastDelegate")
            {
                return true;
            }
            return false;
        }

        private void VisitNullCoalesceExpression(BinaryExpression node)
        {
            WriteKeyword(KeyWordWriter.If);
            WriteToken("(");
            Visit(node.Left);
            WriteToken(",");
            WriteSpace();
            Visit(node.Right);
            WriteToken(")");
        }

        private void VisitVBSwitchCases(IEnumerable collection)
        {
            foreach (ICodeNode node in collection)
            {
                Visit(node);
            }
        }

        public override void VisitSwitchStatement(SwitchStatement node)
        {
            statementStates.Push(StatementState.Switch);
            WriteKeyword(KeyWordWriter.Switch);
            WriteSpace();
            WriteKeyword(KeyWordWriter.Case);
            WriteSpace();

            Visit(node.Condition);

            WriteLine();
            WriteBlock(() => VisitVBSwitchCases(node.Cases), "");

            WriteEndBlock(KeyWordWriter.Switch);
            statementStates.Pop();
        }

        public override void VisitConditionCase(ConditionCase node)
        {
            WriteKeyword(KeyWordWriter.Case);
            WriteSpace();
            Visit(node.Condition);

            //if (node.Body == null)
            //{
            WriteLine();
            // }
            Visit(node.Body);

        }

        public override void VisitDefaultCase(DefaultCase node)
        {
            WriteKeyword(KeyWordWriter.Default);
            WriteSpace();
            WriteKeyword(KeyWordWriter.Else);
            WriteLine();

            Visit(node.Body);
        }

        public override void VisitForEachStatement(ForEachStatement node)
        {
            statementStates.Push(StatementState.ForEach);
            WriteKeyword(KeyWordWriter.ForEach);
            WriteSpace();
            statementStates.Push(StatementState.ForEachInitializer);
            Visit(node.Variable);
            statementStates.Pop();
            WriteSpace();
            WriteKeyword(KeyWordWriter.In);
            WriteSpace();
            Visit(node.Collection);
            WriteLine();
            Visit(node.Body);
            WriteKeyword(KeyWordWriter.Next);
            statementStates.Pop();
        }

        public override void VisitWhileStatement(WhileStatement node)
        {
            statementStates.Push(StatementState.While);
            base.VisitWhileStatement(node);
            statementStates.Pop();
        }

        public override void VisitDoWhileStatement(DoWhileStatement node)
        {
            statementStates.Push(StatementState.DoWhile);
            WriteKeyword(KeyWordWriter.Do);
            WriteLine();
            Visit(node.Body);
            WriteKeyword(KeyWordWriter.LoopWhile);
            WriteSpace();
            WriteSpecialBetweenParenthesis(node.Condition);
            WriteEndOfStatement();
            statementStates.Pop();
        }

        private string GetCastMethod(TypeReference type)
        {
            switch (type.Name)
            {
                case "Decimal":
                    return "CDec";
                case "Single":
                    return "CSng";
                case "Byte":
                    return "CByte";
                case "SByte":
                    return "CSByte";
                case "Char":
                    return "CChar";
                case "Double":
                    return "CDbl";
                case "Boolean":
                    return "CBool";
                case "Int16":
                    return "CShort";
                case "UInt16":
                    return "CUShort";
                case "Int32":
                    return "CInt";
                case "UInt32":
                    return "CUInt";
                case "Int64":
                    return "CLng";
                case "UInt64":
                    return "CULng";
                case "String":
                    return "CStr";
                case "Object":
                    return "CObj";
                case "RuntimeArgumentHandle":
                    return null;
                default:
                    return null;
            }
        }

        public override void VisitExplicitCastExpression(ExplicitCastExpression node)
        {
            var castMethod = GetCastMethod(node.TargetType);
            if (castMethod == null)
            {
                WriteKeyword("DirectCast");
                WriteToken("(");
                WriteCastExpression(node);
                WriteToken(",");
                WriteSpace();
                WriteReferenceAndNamespaceIfInCollision(node.TargetType);
            }
            else
            {
                if (node.UnresolvedReferenceForAmbiguousCastToObject == null)
                {
                    WriteKeyword(castMethod);
                }
                else
                {
                    TypeReference lastResolvedType;
                    Common.Extensions.ResolveToOverloadedEqualityOperator(node.Expression.ExpressionType, out lastResolvedType);
                    WriteNotResolvedReference(castMethod, lastResolvedType, string.Format(CastToObjectResolvementError, lastResolvedType.Name));
                }

                WriteToken("(");
                WriteCastExpression(node);
            }
            WriteToken(")");
        }

        private void WriteCastExpression(ExplicitCastExpression node)
        {
            bool isComplexCastTarget = IsComplexTarget(node.Expression);

            if (isComplexCastTarget)
            {
                WriteToken("(");
            }

            Visit(node.Expression);

            if (isComplexCastTarget)
            {
                WriteToken(")");
            }
        }

        public override void VisitSafeCastExpression(SafeCastExpression node)
        {
            WriteKeyword(KeyWordWriter.TryCast);
            WriteToken("(");
            Visit(node.Expression);
            WriteToken(",");
            WriteSpace();
            WriteReferenceAndNamespaceIfInCollision(node.TargetType);
            WriteToken(")");
        }

        public override void VisitUnaryExpression(UnaryExpression node)
        {
            if (node.Operator == UnaryOperator.Negate ||
                node.Operator == UnaryOperator.LogicalNot ||
                node.Operator == UnaryOperator.BitwiseNot ||
                node.Operator == UnaryOperator.UnaryPlus)
            {
                Write(ToString(node.Operator));
                base.VisitUnaryExpression(node);
                return;
            }

            if (node.Operator == UnaryOperator.AddressOf)
            {
                VisitAddressOfExpression(node);
                return;
            }
            if (node.Operator == UnaryOperator.AddressDereference)
            {
                base.VisitUnaryExpression(node);
                return;
            }
            bool isPostOp = IsPostUnaryOperator(node.Operator);

            bool shouldClone = false;
            if ((node.Operator == UnaryOperator.PostDecrement) ||
                (node.Operator == UnaryOperator.PostIncrement) ||
                (node.Operator == UnaryOperator.PreDecrement) ||
                (node.Operator == UnaryOperator.PreIncrement))
            {
                shouldClone = true;
                Visit(node.Operand);

                WriteSpace();
                WriteToken("=");
                WriteSpace();
            }

            if (!isPostOp)
                Write(ToString(node.Operator));

            Visit(shouldClone ? node.Operand.CloneExpressionOnly() : node.Operand);

            if (isPostOp)
                Write(ToString(node.Operator));
        }

        public override void VisitBaseCtorExpression(BaseCtorExpression node)
        {
            VisitCtorExpression(node, "MyBase");
        }

        public override void VisitThisCtorExpression(ThisCtorExpression node)
        {
            VisitCtorExpression(node, "MyClass");
        }

        private void VisitCtorExpression(MethodInvocationExpression node, string ctorKeyword)
        {
            WriteKeyword(ctorKeyword);
            WriteToken(".");
            if (node.MethodExpression.CodeNodeType == CodeNodeType.MethodReferenceExpression)
            {
                WriteReference("New", node.MethodExpression.Method);
            }
            else
            {
                WriteKeyword("New");
            }
            WriteToken("(");

            EnterMethodInvocation(node.MethodExpression.Method);
            VisitMethodParameters(node.Arguments);
            LeaveMethodInvocation();

            WriteToken(")");
        }

        public override void VisitForStatement(ForStatement node)
        {
            BinaryExpression binaryExpression = node.Condition as BinaryExpression;
            Expression incrementExpression = node.Increment;

            if (binaryExpression == null || incrementExpression == null)
            {
                throw new Exception("Unexpected null value.");
            }

            if (binaryExpression.IsAssignmentExpression)
            {
                return;
            }
            statementStates.Push(StatementState.For);
            var forStep = GetForStep(incrementExpression);
            WriteKeyword("For");
            WriteSpace();
            statementStates.Push(StatementState.ForEachInitializer);
            Visit(node.Initializer);
            statementStates.Pop();
            WriteSpace();
            WriteKeyword("To");
            WriteSpace();
            Visit(binaryExpression.Right);
            if (forStep != null)
            {
                WriteSpace();
                WriteKeyword("Step");
                WriteSpace();
                Visit(forStep);
            }
            WriteLine();
            Visit(node.Body);
            WriteKeyword(KeyWordWriter.Next);
            WriteLine();
            statementStates.Pop();
        }

        private Expression GetForStep(Expression incrementExpression)
        {
            if (incrementExpression.CodeNodeType == CodeNodeType.UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)incrementExpression;
                if ((unaryExpression.Operator == UnaryOperator.PostDecrement) ||
                    (unaryExpression.Operator == UnaryOperator.PreDecrement))
                {
                    return new LiteralExpression(-1, MethodContext.Method.Module.TypeSystem, null);
                }
            }
            if (incrementExpression.CodeNodeType == CodeNodeType.BinaryExpression &&
                (incrementExpression as BinaryExpression).Operator == BinaryOperator.SubtractAssign)
            {
                var subAssingExpression = (BinaryExpression)incrementExpression;
                Expression step = subAssingExpression.Right;
                return GetNegatedExpression(step);
            }
            if (incrementExpression.CodeNodeType == CodeNodeType.BinaryExpression &&
                (incrementExpression as BinaryExpression).Operator == BinaryOperator.AddAssign)
            {
                var addAssingExpression = (BinaryExpression)incrementExpression;
                return addAssingExpression.Right;
            }
            if (incrementExpression.CodeNodeType == CodeNodeType.BinaryExpression &&
                (incrementExpression as BinaryExpression).IsAssignmentExpression)
            {
                var assingExpression = (BinaryExpression)incrementExpression;

				BinaryExpression binaryExpression;
				if (assingExpression.Right.CodeNodeType == CodeNodeType.ExplicitCastExpression)
				{
					ExplicitCastExpression castExpression = assingExpression.Right as ExplicitCastExpression;
					while (castExpression.Expression.CodeNodeType == CodeNodeType.ExplicitCastExpression)
					{
						castExpression = castExpression.Expression as ExplicitCastExpression;
					}

					binaryExpression = castExpression.Expression as BinaryExpression;
				}
				else
				{
					binaryExpression = assingExpression.Right as BinaryExpression;
				}

				if (binaryExpression != null)
				{
					Expression step = binaryExpression.Right;
					if (binaryExpression.Operator == BinaryOperator.Subtract)
					{
						return GetNegatedExpression(step);
					}

					return step;
				}
            }

            return null;
        }

        private Expression GetNegatedExpression(Expression expression)
        {
			return new UnaryExpression(UnaryOperator.Negate, expression, null);
        }

        public override void VisitContinueStatement(ContinueStatement node)
        {
            WriteKeyword("Continue");
            WriteSpace();
            WriteInnermostParentFrom(GetContinuableParents());
            //  WriteLine();
        }

        private Dictionary<StatementState, string> GetContinuableParents()
        {
            return new Dictionary<StatementState, string>()
            {
                { StatementState.DoWhile, "Do" },
                { StatementState.For, "For" },
                { StatementState.ForEach, "For" },
                { StatementState.While, "While" }
            };
        }

        private Dictionary<StatementState, string> GetExitableParents()
        {
            return new Dictionary<StatementState, string>(GetContinuableParents())
            {
                { StatementState.Switch, "Select" }
            };
        }

        private void WriteInnermostParentFrom(Dictionary<StatementState, string> parents)
        {
            foreach (StatementState state in this.statementStates)
            {
                if (parents.ContainsKey(state))
                {
                    WriteKeyword(parents[state]);

                    return;
                }
            }

            throw new Exception("Suitable parent for Continue/Exit statement not found.");
        }

        public override void VisitBreakStatement(BreakStatement node)
        {
            WriteKeyword("Exit");
            WriteSpace();
            WriteInnermostParentFrom(GetExitableParents());
            //  WriteLine();
        }

        public override void VisitYieldReturnExpression(YieldReturnExpression node)
        {
            // TODO: We could take the return type from the method which we're decompiling
            WriteKeyword(KeyWordWriter.Return);
            WriteSpace();
            WriteKeyword(KeyWordWriter.New);
            WriteSpace();
            var expressionType = GetExpressionType(node.Expression);
            if (expressionType != null)
            {
                WriteReferenceAndNamespaceIfInCollision(expressionType);
            }
            else
            {
                string typeName = "Object";
                if (node.Expression.CodeNodeType == CodeNodeType.LiteralExpression)
                {
                    typeName = GetExpressionType((LiteralExpression)node.Expression);
                }
                WriteKeyword(typeName);
            }
            WriteToken("(");
            WriteToken(")");
            WriteSpace();
            WriteToken("{");
            WriteSpace();
            Visit(node.Expression);
            WriteSpace();
            WriteToken("}");
        }

        private TypeReference GetExpressionType(Expression expression)
        {
            switch (expression.CodeNodeType)
            {
                case CodeNodeType.FieldReferenceExpression:
                    {
                        return ((FieldReferenceExpression)expression).Field.FieldType;
                    }
                case CodeNodeType.PropertyReferenceExpression:
                    {
                        return ((PropertyReferenceExpression)expression).Property.PropertyType;
                    }
                case CodeNodeType.VariableReferenceExpression:
                    {
                        return ((VariableReferenceExpression)expression).Variable.VariableType;
                    }
                case CodeNodeType.MethodInvocationExpression:
                    {
                        MethodReferenceExpression method = ((MethodInvocationExpression)expression).MethodExpression;
                        if (method.CodeNodeType == CodeNodeType.MethodReferenceExpression)
                        {
                            return method.Method.ReturnType;
                        }
                        break;
                    }
            }
            return null;
        }

        private string GetExpressionType(LiteralExpression literalExpression)
        {
            if (literalExpression.Value is decimal)
                return "Decimal";
            if (literalExpression.Value is Single)
                return "Float";
            if (literalExpression.Value is byte)
                return "Byte";
            if (literalExpression.Value is sbyte)
                return "SByte";
            if (literalExpression.Value is char)
                return "Char";
            if (literalExpression.Value is double)
                return "Double";
            if (literalExpression.Value is bool)
                return "Boolean";
            if (literalExpression.Value is short)
                return "Short";
            if (literalExpression.Value is int)
                return "Integer";
            if (literalExpression.Value is long)
                return "Long";
            if (literalExpression.Value is ushort)
                return "UShort";
            if (literalExpression.Value is uint)
                return "UInteger";
            if (literalExpression.Value is ulong)
                return "ULong";
            if (literalExpression.Value is string)
                return "String";
            return "Object";
        }

        public override void VisitYieldBreakExpression(YieldBreakExpression node)
        {
            WriteKeyword(KeyWordWriter.Return);
        }

        public override void VisitDefaultObjectExpression(DefaultObjectExpression node)
        {
            WriteKeyword("Nothing");
        }

        public override void VisitLambdaExpression(LambdaExpression node)
        {
            base.VisitLambdaExpression(node);

			string lambdaMethodKeyword = node.IsFunction ? KeyWordWriter.Function : KeyWordWriter.Sub;

			WriteKeyword(lambdaMethodKeyword);
            WriteToken("(");
            VisitMethodParameters(node.Arguments);
            WriteToken(")");

			bool isShortFormLambda = false;
			bool newLineAfterStatemtentsWritten;
            if (node.Body.Statements.Count == 1)
            {
                if (node.Body.Statements[0].CodeNodeType == CodeNodeType.ExpressionStatement)
                {
					isShortFormLambda = true;
                    ShouldOmitSemicolon.Push(true);
                }
                else
                {
                    ShouldOmitSemicolon.Push(false);
                }

				if (isShortFormLambda)
				{
					WriteSpace();
				}
				else
				{
					WriteLine();
				}

                Visit(node.Body.Statements[0]);
				newLineAfterStatemtentsWritten = false;
            }
            else
            {
                ShouldOmitSemicolon.Push(false);
				WriteLine();
                Visit(node.Body);
				newLineAfterStatemtentsWritten = true;
            }

			if (!isShortFormLambda)
			{
				if (!newLineAfterStatemtentsWritten)
				{
					WriteLine();
				}

				WriteEndBlock(lambdaMethodKeyword);
			}

            ShouldOmitSemicolon.Pop();
        }

        public override void VisitCatchClause(CatchClause node)
        {
            WriteKeyword(KeyWordWriter.Catch);

            if (node.Type.FullName != Constants.Object)
            {
                WriteSpace();

				if (node.Variable != null)
				{
					statementStates.Push(StatementState.Catch);
					Visit(node.Variable);
					statementStates.Pop();
				}
				else
				{
					WriteReferenceAndNamespaceIfInCollision(node.Type);
				}
            }

            if (node.Filter != null)
            {
                WriteSpace();
                WriteKeyword(KeyWordWriter.When);
                WriteSpace();
                Visit((node.Filter as ExpressionStatement).Expression);
            }

            WriteLine();
            Visit(node.Body);
        }

        public override void VisitUsingStatement(UsingStatement node)
        {
            WriteKeyword(KeyWordWriter.Using);
            WriteSpace();
            statementStates.Push(StatementState.Using);
            WriteSpecialBetweenParenthesis(node.Expression);
            statementStates.Pop();
            WriteLine();
            Visit(node.Body);
            WriteSpecialEndBlock(KeyWordWriter.Using);
        }

        protected override string OnConvertString(string str)
        {
            ////if (str.Length > 0)
            ////{
            ////    str = str.Replace("\"", "\"\"").Replace("\r\n", "\" & VbCrLf & \"").Replace("\n", "\" & VbCrLf & \"");
            ////}
            ////return str;

            var sb = new StringBuilder();
            foreach (char ch in str)
            {
                if (ch == '"')
                {
                    sb.Append("\"\"");
                }
                else
                {
                    sb.Append(ConvertCharOnVB(ch));
                }
            }
            return sb.ToString();
        }

        protected string ConvertCharOnVB(char ch)
        {
            switch (ch)
            {
                case '\n':
                    return "\" & VbCrLf & \"";
                case '\r':
                    return string.Empty;
                default:
                    return ch.ToString();
            }
        }

        protected override void WriteEscapeCharLiteral(char c)
        {
            int? chrwArgument = null;

            switch (c)
            {
                case '\"':
                    WriteLiteral(CharStart);
                    formatter.WriteLiteral("\"\"");
                    WriteLiteral(CharEnd);
                    return;
                case '\0':
                case '\a':
                case '\b':
                case '\f':
                case '\n':
                case '\r':
                case '\t':
                case '\v':
                    chrwArgument = (int)c;
                    break;
            }

            UnicodeCategory unicodeCategory = Char.GetUnicodeCategory(c);

            bool canBePrinted = false;
            if (unicodeCategory == UnicodeCategory.ClosePunctuation || unicodeCategory == UnicodeCategory.CurrencySymbol ||
                unicodeCategory == UnicodeCategory.DashPunctuation || unicodeCategory == UnicodeCategory.DecimalDigitNumber ||
                unicodeCategory == UnicodeCategory.FinalQuotePunctuation || unicodeCategory == UnicodeCategory.InitialQuotePunctuation ||
                unicodeCategory == UnicodeCategory.LetterNumber || unicodeCategory == UnicodeCategory.LowercaseLetter ||
                unicodeCategory == UnicodeCategory.MathSymbol || unicodeCategory == UnicodeCategory.OpenPunctuation ||
                unicodeCategory == UnicodeCategory.TitlecaseLetter || unicodeCategory == UnicodeCategory.UppercaseLetter ||
                unicodeCategory == UnicodeCategory.OtherPunctuation)
            {
                canBePrinted = true;
            }
            if (!canBePrinted)
            {
                chrwArgument = (int)c;
            }

            if (chrwArgument.HasValue)
            {
                //TODO: Make ChrW a reference to Strings.ChrW from Microsoft.VisualBasic.dll
                //		Be sure to include the correct version of the dll.
                Write("Strings.ChrW(");
                WriteLiteral(chrwArgument.ToString());
                Write(")");
                return;
            }
            else
            {
                WriteLiteral(CharStart);
                formatter.WriteLiteral(c.ToString());
                WriteLiteral(CharEnd);
            }
        }

        protected override bool SupportsAutoProperties
        {
            get
            {
                return true;
            }
        }

        protected override void WriteOptional(ParameterDefinition parameter)
        {
            WriteKeyword("Optional");
            WriteSpace();
        }

        public override void VisitAnonymousPropertyInitializerExpression(AnonymousPropertyInitializerExpression node)
        {
            if (node.IsKey)
            {
                WriteKeyword(KeyWordWriter.Key);
                WriteSpace();
            }

            WriteToken(".");
			base.VisitAnonymousPropertyInitializerExpression(node);
        }

        public override void VisitPropertyInitializerExpression(PropertyInitializerExpression node)
        {
            WriteToken(".");
			base.VisitPropertyInitializerExpression(node);
		}

		public override void VisitFieldInitializerExpression(FieldInitializerExpression node)
		{
			WriteToken(".");
			base.VisitFieldInitializerExpression(node);
		}

        public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
        {
            if ((node.Variable.VariableType.IsOptionalModifier || node.Variable.VariableType.IsRequiredModifier) &&
                !this.isWritingComment)
            {
                StartWritingComment();
                VisitVariableDeclarationExpression(node);
                EndWritingComment();
                WriteLine();
            }

            WriteDim();

            VariableDefinition variable = node.Variable;
            if (variable.VariableType.ContainsAnonymousType())
            {
                this.WriteAndMapVariableToCode(() => WriteToken(GetVariableName(variable)), node.Variable);
            }
            else
            {
                base.VisitVariableDeclarationExpression(node);
            }
        }

		public override void VisitArrayAssignmentVariableReferenceExpression(ArrayVariableReferenceExpression node)
		{
			if (!node.HasInitializer)
			{
				WriteKeyword(KeyWordWriter.ReDim);
				WriteSpace();
			}

			Visit(node.Variable);

			if (!node.HasInitializer)
			{
				WriteArrayDimensions(node.Dimensions, node.ArrayType, node.HasInitializer);
			}
		}

		public override void VisitArrayAssignmentFieldReferenceExpression(ArrayAssignmentFieldReferenceExpression node)
		{
			if (!node.HasInitializer)
			{
				WriteKeyword(KeyWordWriter.ReDim);
				WriteSpace();
			}

			Visit(node.Field);

			if (!node.HasInitializer)
			{
				WriteArrayDimensions(node.Dimensions, node.ArrayType, node.HasInitializer);
			}
		}

		public override void VisitArrayVariableDeclarationExpression(ArrayVariableDeclarationExpression node)
		{
			if ((node.Variable.Variable.VariableType.IsOptionalModifier || node.Variable.Variable.VariableType.IsRequiredModifier) && !this.isWritingComment)
			{
				StartWritingComment();
				VisitVariableDeclarationExpression(node.Variable);
				EndWritingComment();
				WriteLine();
			}

			WriteDim();
			
			string variableName = GetVariableName(node.Variable.Variable);
			this.WriteAndMapVariableToCode(() => Write(variableName), node.Variable.Variable);

            WriteArrayDimensions(node.Dimensions, node.ArrayType, node.HasInitializer);
			WriteAsBetweenSpaces();
			WriteReferenceAndNamespaceIfInCollision(GetBaseElementType(node.ArrayType));
		}

        protected override void WriteArrayDimensions(ExpressionCollection dimensions, TypeReference arrayType, bool isInitializerPresent)
        {
            ExpressionCollection clonedDimensions = dimensions.Clone();
            TypeSystem typeSystem = this.ModuleContext.Module.TypeSystem;
            for (int i = 0; i < clonedDimensions.Count; i++)
            {
                if (clonedDimensions[i] is LiteralExpression)
                {
                    LiteralExpression literal = clonedDimensions[i] as LiteralExpression;
                    literal.Value = GetDecrementedValue(literal);
                }
                else
                {
                    clonedDimensions[i] = new BinaryExpression(BinaryOperator.Subtract, clonedDimensions[i], new LiteralExpression(1, typeSystem, null), typeSystem, null);
                }
            }

            base.WriteArrayDimensions(clonedDimensions, arrayType, isInitializerPresent);
        }

        private object GetDecrementedValue(LiteralExpression expression)
        {
            switch (expression.ExpressionType.Name)
            {
                case "Byte":
                    byte byteValue = (byte)expression.Value;
                    byteValue--;
                    return byteValue;
                case "SByte":
                    sbyte sbyteValue = (sbyte)expression.Value;
                    sbyteValue--;
                    return sbyteValue;
                case "Int16":
                    short shortValue = (short)expression.Value;
                    shortValue--;
                    return shortValue;
                case "UInt16":
                    ushort ushortValue = (ushort)expression.Value;
                    ushortValue--;
                    return ushortValue;
                case "Int32":
                    int intValue = (int)expression.Value;
                    intValue--;
                    return intValue;
                case "UInt32":
                    uint uintValue = (uint)expression.Value;
                    uintValue--;
                    return uintValue;
                case "Int64":
                    long longValue = (long)expression.Value;
                    longValue--;
                    return longValue;
                case "UInt64":
                    ulong ulongValue = (ulong)expression.Value;
                    ulongValue--;
                    return ulongValue;
                case "Char":
                    char charValue = (char)expression.Value;
                    charValue--;
                    return charValue;
                default:
                    throw new ArgumentException("Invalid data type for dimension of an array.");
            }
        }

		public override void VisitArrayCreationExpression(ArrayCreationExpression node)
		{
			base.VisitArrayCreationExpression(node);

			if (!Utilities.IsInitializerPresent(node.Initializer))
			{
				WriteSpace();
				Write("{}");
			}
		}

        protected override void StartWritingComment()
        {
            WriteComment("");//writes the commenting symbol at the start of the line
            base.StartWritingComment();
        }

        protected override void WriteGenericParameterConstraints(GenericParameter parameter)
        {
            int constraintsCount = CountConstraints(parameter);
            WriteSpace();
            WriteKeyword(KeyWordWriter.As);
            WriteSpace();
            if (constraintsCount > 1)
            {
                WriteToken("{");
            }
            //for (int i = 0; i < parameter.Constraints.Count; i++)
            //{
            //    if (i > 0)
            //    {
            //        WriteToken(",");
            //        WriteSpace();
            //    }
            //    WriteReferenceAndNamespaceIfInCollision(parameter.Constraints[i]);
            //}
            WriteSingleGenericParameterConstraintsList(parameter);
            if (constraintsCount > 1)
            {
                WriteToken("}");
            }
        }

        private int CountConstraints(GenericParameter parameter)
        {
            int constraintsCount = 0;
            if (parameter.HasConstraints)
            {
                constraintsCount += parameter.Constraints.Count;
            }
            if (parameter.HasDefaultConstructorConstraint)
            {
                constraintsCount++;
            }
            if (parameter.HasReferenceTypeConstraint)
            {
                constraintsCount++;
            }
            if (parameter.HasNotNullableValueTypeConstraint)
            {
                constraintsCount++;
            }
            return constraintsCount;
        }

        protected override void WriteTypeSpecification(TypeSpecification typeSpecification, int startingArgument = 0)
        {
            if ((typeSpecification.IsOptionalModifier || typeSpecification.IsRequiredModifier) && !isWritingComment)
            {
                WriteReferenceAndNamespaceIfInCollision(typeSpecification.ElementType);
                return;
            }

            base.WriteTypeSpecification(typeSpecification, startingArgument);
        }

        protected override void WriteFieldDeclaration(FieldDefinition field)
        {
            // VB has no equivalent of C# keyword volatile, thus the modopt(Volatile) should be shown.
            if (field.FieldType.IsOptionalModifier || field.FieldType.IsRequiredModifier)
            {
                StartWritingComment();
                base.WriteFieldDeclaration(field);
                EndWritingComment();
                WriteLine();
            }
            base.WriteFieldDeclaration(field);
        }

        protected override void WriteBaseConstructorInvokation(MethodInvocationExpression baseConstructorInvokation)
        {
            if (!MethodContext.Method.IsStatic)
            {
                WriteLine();
                Indent();
                Visit(MethodContext.CtorInvokeExpression);
                Outdent();
            }
        }

		protected override void WriteEmptyMethodEndOfStatement(MethodDefinition method)
		{
			if (!method.IsAbstract)
			{
				WriteLine();
				string methodKeyWord = GetMethodKeyWord(method);
				WriteSpecialEndBlock(methodKeyWord);
			}
		}

        protected override void WriteMethodDeclaration(MethodDefinition method, bool writeDocumentation)
        {
            bool hasModifierInSignature = false;
            if (method.ReturnType.IsOptionalModifier || method.ReturnType.IsRequiredModifier)
            {
                hasModifierInSignature = true;
            }
            for (int i = 0; i < method.Parameters.Count && !hasModifierInSignature; i++)
            {
                ParameterDefinition param = method.Parameters[i];
                if (param.ParameterType.IsOptionalModifier || param.ParameterType.IsRequiredModifier)
                {
                    hasModifierInSignature = true;
                }
            }

            if (hasModifierInSignature)
            {
                StartWritingComment();
                base.WriteMethodDeclaration(method, writeDocumentation);
                EndWritingComment();
                WriteLine();
            }

            base.WriteMethodDeclaration(method, writeDocumentation);
        }

		protected override void Write(PropertyDefinition property)
		{
			if (property.IsAbstract() || this.TypeContext.AutoImplementedProperties.Contains(property))
			{
				WritePropertyDeclaration(property);
				this.currentWritingInfo.MemberDefinitionToFoldingPositionMap[property] = new OffsetSpan(formatter.CurrentPosition, formatter.CurrentPosition);
			}
			else
			{
				base.Write(property);
			}
		}

        protected override void PostWriteGenericParametersConstraints(IGenericDefinition generic)
        {
            /// As TypeParameter's constraints in generic types are wrote besides the declaration of the type parameter
            /// this method will only print the comentaries for the contradicting type parameter's overriding constraints

            if (!generic.HasGenericParameters)
            {
                return;
            }

            foreach (GenericParameter parameter in generic.GenericParameters)
            {
                if (IsTypeParameterRedeclaration(parameter) && ShouldWriteConstraintsAsComment(parameter))
                {
                    WriteLine();
                    StartWritingComment();
                    WriteToken("Of ");
                    WriteReference(parameter.Name, null);
                    WriteGenericParameterConstraints(parameter);
                    EndWritingComment();
                }
            }
        }

        public override void VisitRaiseEventExpression(RaiseEventExpression node)
        {
            WriteKeyword(this.KeyWordWriter.Fire);
            WriteSpace();
            WriteReference(node.Event.Name, node.Event);
            EnterMethodInvocation(node.InvokeMethodReference);
            Write("(");
            VisitMethodParameters(node.Arguments);
            Write(")");
            LeaveMethodInvocation();
        }

        protected override bool TypeSupportsExplicitStaticMembers(TypeDefinition type)
        {
            // Modules in VB.NET don't support explicit static members. All members are static by default.
            return !type.IsStaticClass;
        }

        protected override string WriteTypeDeclaration(TypeDefinition type, bool isPartial = false)
        {
            if (type.IsNested && type.IsStaticClass)
            {
                throw new Exception("VB.NET does not support nested modules. Please, try using other language.");
            }

            return base.WriteTypeDeclaration(type, isPartial);
        }

        public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
        {
            GenericInstanceMethod genericMethod = node.MethodExpression.Method as GenericInstanceMethod;
            if (node.MethodExpression.Method.HasThis ||
                node.Arguments.Count == 0 ||
                genericMethod == null ||
                node.MethodExpression.MethodDefinition == null ||
                !node.MethodExpression.MethodDefinition.IsExtensionMethod)
            {
                base.VisitMethodInvocationExpression(node);

                return;
            }

            WriteMethodTarget(node.Arguments[0]);
            WriteGenericInstanceMethodWithArguments(genericMethod, GetGenericExtensionMethodArguments(genericMethod));
            WriteToken("(");
            VisitExtensionMethodParameters(node.Arguments);
            WriteToken(")");
        }

        private Collection<TypeReference> GetGenericExtensionMethodArguments(GenericInstanceMethod genericMethod)
        {
            TypeReference extendedType = genericMethod.ElementMethod.Parameters[0].ParameterType;
            if (!extendedType.IsGenericInstance && !extendedType.IsGenericParameter)
            {
                return genericMethod.GenericArguments;
            }

            HashSet<GenericParameter> extendedTypeGenericParameters = new HashSet<GenericParameter>();
            if (extendedType.IsGenericInstance)
            {
                Queue<GenericInstanceType> queue = new Queue<GenericInstanceType>();
                queue.Enqueue(extendedType as GenericInstanceType);
                while (queue.Count > 0)
                {
                    GenericInstanceType current = queue.Dequeue();
                    foreach (TypeReference genericArgument in current.GenericArguments)
                    {
                        if (genericArgument.IsGenericInstance)
                        {
                            queue.Enqueue(genericArgument as GenericInstanceType);
                        }
                        else if (genericArgument.IsGenericParameter)
                        {
                            extendedTypeGenericParameters.Add(genericArgument as GenericParameter);
                        }
                    }
                }
            }
            else
            {
                // GenericParameter
                extendedTypeGenericParameters.Add(extendedType as GenericParameter);
            }

            Collection<TypeReference> result = new Collection<TypeReference>();
            for (int i = 0; i < genericMethod.ElementMethod.GenericParameters.Count; i++)
            {
                GenericParameter currentGenericParameter = genericMethod.ElementMethod.GenericParameters[i];
                if (!extendedTypeGenericParameters.Contains(currentGenericParameter))
                {
                    result.Add(genericMethod.GenericArguments[i]);
                }
            }

            return result;
        }

        public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
        {
            if (node.Target != null &&
                node.Target.CodeNodeType == CodeNodeType.BaseReferenceExpression &&
                node.IsIndexer)
            {
                Visit(node.Target);
                WriteToken(".");
                WritePropertyName(node.Property);
                WriteIndexerArguments(node);
            }
            else
            {
                base.VisitPropertyReferenceExpression(node);
            }
        }

        /// <summary>
        /// Gets the type string for given type reference. If the type string is a system type and it's in collision with
        /// some keyword, it's escaped.
        /// </summary>
        internal override string ToEscapedTypeString(TypeReference reference)
        {
            if (IsReferenceFromMscorlib(reference))
            {
                string typeString = ToTypeString(reference);
                if (typeString == "Enum" || typeString == "Delegate")
                {
                    typeString = Utilities.Escape(typeString, this.Language);
                }

                return typeString;
            }
            else
            {
                return ToTypeString(reference);
            }
        }
    }

    public enum StatementState
    {
        ForEachInitializer,
        ForInitializer,
        For,
        ForEach,
        While,
        DoWhile,
        Switch,
        Catch,
        Using
    }
}