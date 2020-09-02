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
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Telerik.JustDecompiler.Ast;
using Mono.Cecil;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using System.Collections;
using Telerik.JustDecompiler.Common;
using System.Linq;
using Telerik.JustDecompiler.Decompiler;
using Mono.Cecil.Cil;


namespace Telerik.JustDecompiler.Languages.CSharp
{
    public class CSharpWriter : NamespaceImperativeLanguageWriter
    {
        protected const string RefOutResolvementError = "Out parameters might be shown as ref. Please, locate the assembly where the method is defined.";


        public CSharpWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
            : base(language, formatter, exceptionFormatter, settings)
        {
        }

        protected override void WriteDestructor(MethodDefinition method)
        {
            membersStack.Push(method);
            bool writedOpenBrecket = false;
            try
            {
                string destructorName = "~" + GetTypeName(method.DeclaringType);
                WriteReference(destructorName, method);
                WriteToken("(");
                WriteToken(")");

                int startIndex = this.formatter.CurrentPosition;

                this.formatter.WriteStartBlock();

                writedOpenBrecket = true;

                WriteLine();

                Visit(MethodContext.DestructorStatements);

                this.currentWritingInfo.MemberDefinitionToFoldingPositionMap[method] = new OffsetSpan(startIndex, formatter.CurrentPosition - 1);

                this.formatter.WriteEndBlock();
            }
            catch (Exception)
            {
                if (writedOpenBrecket)
                {
                    this.formatter.WriteEndBlock();
                }
                membersStack.Pop();
                throw;
            }
            membersStack.Pop();
        }

        protected override bool WritePropertyAsIndexer(PropertyDefinition property)
        {
            string propertyName = KeyWordWriter.This;
            if (property.IsExplicitImplementation())
            {
                int lastDotIndex = property.Name.LastIndexOf(".");
                propertyName = property.Name.Replace(property.Name.Substring(lastDotIndex + 1), KeyWordWriter.This);
            }
            WriteTypeAndName(property.PropertyType, propertyName, property);

            Write(IndexLeftBracket);
            WritePropertyParameters(property);
            Write(IndexRightBracket);

            return true;
        }

        protected override void WriteIndexerKeywords()
        {
        }

        protected override AttributeWriter CreateAttributeWriter()
        {
            return new CSharpAttributeWriter(this);
        }

        protected override IKeyWordWriter CreateKeyWordWriter()
        {
            return new CSharpKeyWordWriter();
        }

        protected override void WriteBeginBlock(bool inline = false)
        {
            if (inline)
            {
                WriteSpace();
            }
            else
            {
                WriteLine();
            }

            WriteToken("{");
        }

        protected override void WriteEndBlock(string statementName)
        {
            WriteToken("}");
        }

        protected override void WriteNestedTypeWriteLine()
        {
            WriteLine();
        }

        protected override void WriteBaseTypeInheritColon()
        {
            WriteSpace();
            WriteToken(":");
            WriteSpace();
        }

        protected override void WriteSpecialBetweenParenthesis(Expression expression)
        {
            WriteToken("(");
            Visit(expression);
            WriteToken(")");
        }

        protected override void WriteSpecialBetweenParenthesis(Action action)
        {
            WriteToken("(");
            action();
            WriteToken(")");
        }

        protected override void WriteInterfacesInheritColon(TypeDefinition type)
        {
            WriteBaseTypeInheritColon();
        }

        protected override void WriteTypeInterfaces(TypeDefinition type, bool isPartial, bool baseTypeWritten)
        {
            List<TypeReference> interfaces;
            if (TypeContext.CurrentType == type && TypeContext.IsWinRTImplementation)
            {
                interfaces = type.Interfaces.Where(@interface => {
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
                            if (baseTypeWritten)
                            {
                                WriteInheritComma();
                            }
                            else
                            {
                                WriteInterfacesInheritColon(type);
                            }
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

        protected override void WriteEnumValueSeparator()
        {
            WriteToken(",");
        }

        protected override void WriteEndOfStatement()
        {
            WriteToken(";");
        }

        public override string ToTypeString(TypeReference type)
        {
            /// There might be user generated classes with this name. Only the ones declared in mscorlib should be replaced by the language keyword.
            if (IsReferenceFromMscorlib(type))
            {
                switch (type.Name)
                {
                    case "Decimal":
                        return "decimal";
                    case "Single":
                        return "float";
                    case "Byte":
                        return "byte";
                    case "SByte":
                        return "sbyte";
                    case "Char":
                        return "char";
                    case "Double":
                        return "double";
                    case "Boolean":
                        return "bool";
                    case "Int16":
                        return "short";
                    case "Int32":
                        return "int";
                    case "Int64":
                        return "long";
                    case "UInt16":
                        return "ushort";
                    case "UInt32":
                        return "uint";
                    case "UInt64":
                        return "ulong";
                    case "String":
                        return "string";
                    case "Void":
                        return "void";
                    case "Object":
                        return "object";
                    case "RuntimeArgumentHandle":
                        return string.Empty;
                    default:
                        return GetGenericNameFromMemberReference(type);
                }
            }
            return GetGenericNameFromMemberReference(type);
        }

        protected override void DoWriteTypeAndName(TypeReference typeReference, string name, object reference)
        {
            WriteReferenceAndNamespaceIfInCollision(typeReference);
            WriteSpace();
            int startIndex = this.formatter.CurrentPosition;
            /* AGPL */
            CodeSpan codeSpan = this.Write(() => WriteReference(name, reference));
            /* End AGPL */
            if (reference is IMemberDefinition)
            {
                int endIndex = this.formatter.CurrentPosition - 1;
                /* AGPL */
                this.currentWritingInfo.MemberDeclarationToCodeSpan[(IMemberDefinition)reference] = codeSpan;
                /* End AGPL */
                this.currentWritingInfo.MemberDeclarationToCodePostionMap[(IMemberDefinition)reference] = new OffsetSpan(startIndex, endIndex);
            }
        }

        protected override void DoWriteTypeAndName(TypeReference typeReference, string name)
        {
            WriteReferenceAndNamespaceIfInCollision(typeReference);
            WriteSpace();
            Write(name);
        }

        protected override void DoWriteVariableTypeAndName(VariableDefinition variable)
        {
            WriteReferenceAndNamespaceIfInCollision(variable.VariableType);
            WriteSpace();
            WriteVariableName(variable);
        }
        
        public override void VisitRefVariableDeclarationExpression(RefVariableDeclarationExpression node)
        {
            WriteKeyword(KeyWordWriter.ByRef);
            WriteSpace();
            WriteReferenceAndNamespaceIfInCollision(node.Variable.VariableType.GetElementType());
            WriteSpace();
            WriteVariableName(node.Variable);
        }

        private void WriteVariableName(VariableDefinition variable)
        {
            this.WriteAndMapVariableToCode(() => Write(GetVariableName(variable)), variable);
        }

        protected override void WriteFieldTypeAndName(FieldDefinition field)
        {
            if (field.IsUnsafe)
            {
                WriteKeyword(KeyWordWriter.Unsafe);
                WriteSpace();
            }

            TypeReference fieldType = field.FieldType;
            if (fieldType.IsRequiredModifier && (fieldType as RequiredModifierType).ModifierType.FullName == "System.Runtime.CompilerServices.IsVolatile")
            {
                WriteKeyword(KeyWordWriter.Volatile);
                WriteSpace();
                fieldType = (fieldType as RequiredModifierType).ElementType;
            }

            string fieldName = GetFieldName(field);

            CustomAttribute dynamicAttribute;
            if (field.TryGetDynamicAttribute(out dynamicAttribute))
            {
                WriteDynamicType(fieldType, dynamicAttribute);
                WriteSpace();
                int startIndex = this.formatter.CurrentPosition;
                /* AGPL */
                CodeSpan codeSpan = this.Write(() => WriteReference(fieldName, field));
                /* End AGPL */
                int endIndex = this.formatter.CurrentPosition - 1;
                /* AGPL */
                this.currentWritingInfo.MemberDeclarationToCodeSpan[field] = codeSpan;
                /* End AGPL */
                this.currentWritingInfo.MemberDeclarationToCodePostionMap[field] = new OffsetSpan(startIndex, endIndex);
            }
            else
            {
                WriteTypeAndName(fieldType, fieldName, field);
            }
        }

        protected override void DoWriteParameterTypeAndName(TypeReference type, string name, ParameterDefinition reference)
        {
            if (!Language.IsValidIdentifier(name))
            {
                name = Language.ReplaceInvalidCharactersInIdentifier(name);
            }

            if (reference.IsParamArray())
            {
                WriteKeyword(KeyWordWriter.ParamArray);
                WriteSpace();
            }

            CustomAttribute dynamicAttribute;
            if (reference.TryGetDynamicAttribute(out dynamicAttribute))
            {
                IEnumerator positioningFlagsEnumerator = DynamicHelper.GetDynamicPositioningFlags(dynamicAttribute).GetEnumerator();
                if (reference.ParameterType.IsByReference && (reference.ParameterType as ByReferenceType).ElementType == type)
                {
                    if (!positioningFlagsEnumerator.MoveNext() || (bool)positioningFlagsEnumerator.Current)
                    {
                        throw new Exception("Invalid argument type for DynamicAttribute");
                    }
                }

                WriteDynamicType(type, positioningFlagsEnumerator);
                WriteSpace();
            }
            else
            {
                // undocumented C# keyword like __arglist
                if (!string.IsNullOrEmpty(ToTypeString(type)))
                {
                    WriteReferenceAndNamespaceIfInCollision(type);
                    WriteSpace();
                }
            }

            if (Language.IsGlobalKeyword(name))
            {
                name = Utilities.EscapeNameIfNeeded(name, this.Language);
            }

            this.WriteAndMapParameterToCode(() => Write(name), reference.Index);
        }

        public override void VisitUnsafeBlockStatement(UnsafeBlockStatement unsafeBlock)
        {
            WriteKeyword(KeyWordWriter.Unsafe);
            WriteLine();
            VisitBlockStatement(unsafeBlock);
        }

        public override void WriteMemberNavigationPathFullName(object member)
        {
            if (member is ParameterReference)
            {
                this.formatter.Write(((ParameterReference)member).Name);
            }
            else if (member is TypeReference)
            {
                this.formatter.Write(((TypeReference)member).GetFriendlyTypeName(Language));
            }
            else if (member is MemberReference)
            {
                this.formatter.Write(((MemberReference)member).GetFriendlyFullName(Language));
            }
        }

        protected override void WriteBlock(Action action, string label)
        {
            WriteToken("{");
            WriteLine();

            if (label != "")
            {
                WriteLabel(label);
            }

            Indent();

            action();

            Outdent();
            WriteToken("}");
        }

        protected override bool SupportsAutoProperties
        {
            get
            {
                return true;
            }
        }

        protected override bool SupportsSpecialNullable
        {
            get
            {
                return true;
            }
        }

        protected override bool ShouldWriteOutAndRefOnInvocation
        {
            get
            {
                return true;
            }
        }

        protected override bool RemoveBaseConstructorInvocation
        {
            get
            {
                return true;
            }
        }

        protected override string CharStart
        {
            get
            {
                return "'";
            }
        }

        protected override string CharEnd
        {
            get
            {
                return "'";
            }
        }

        protected override string HexValuePrefix
        {
            get
            {
                return "0x";
            }
        }

        public override string IndexLeftBracket
        {
            get
            {
                return "[";
            }
        }

        public override string IndexRightBracket
        {
            get
            {
                return "]";
            }
        }

        protected override string GenericLeftBracket
        {
            get
            {
                return "<";
            }
        }

        protected override string GenericRightBracket
        {
            get
            {
                return ">";
            }
        }

        protected override string ToString(BinaryOperator op, bool isOneSideNull = false)
        {
            switch (op)
            {
                case BinaryOperator.Add:
                    return "+";
                case BinaryOperator.BitwiseAnd:
                    return "&";
                case BinaryOperator.BitwiseOr:
                    return "|";
                case BinaryOperator.BitwiseXor:
                    return "^";
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
                    return "&&";
                case BinaryOperator.LogicalOr:
                    return "||";
                case BinaryOperator.Modulo:
                    return "%";
                case BinaryOperator.Multiply:
                    return "*";
                case BinaryOperator.RightShift:
                    return ">>";
                case BinaryOperator.Subtract:
                    return "-";
                case BinaryOperator.ValueEquality:
                    return "==";
                case BinaryOperator.ValueInequality:
                    return "!=";
                case BinaryOperator.Assign:
                    return "=";
                case BinaryOperator.AddAssign:
                    return "+=";
                case BinaryOperator.SubtractAssign:
                    return "-=";
                case BinaryOperator.AndAssign:
                    return "&=";
                case BinaryOperator.DivideAssign:
                    return "/=";
                case BinaryOperator.LeftShiftAssign:
                    return "<<=";
                case BinaryOperator.ModuloAssign:
                    return "%=";
                case BinaryOperator.MultiplyAssign:
                    return "*=";
                case BinaryOperator.OrAssign:
                    return "|=";
                case BinaryOperator.RightShiftAssign:
                    return ">>=";
                case BinaryOperator.XorAssign:
                    return "^=";
                case BinaryOperator.NullCoalesce:
                    return "??";
                default:
                    throw new ArgumentException();
            }
        }

        protected override string ToString(UnaryOperator op)
        {
            switch (op)
            {
                case UnaryOperator.BitwiseNot:
                    return "~";
                case UnaryOperator.LogicalNot:
                    return "!";
                case UnaryOperator.Negate:
                    return "-";
                case UnaryOperator.PostDecrement:
                case UnaryOperator.PreDecrement:
                    return "--";
                case UnaryOperator.PostIncrement:
                case UnaryOperator.PreIncrement:
                    return "++";
                case UnaryOperator.AddressDereference:
                    return "*";
                case UnaryOperator.AddressReference:
                case UnaryOperator.AddressOf:
                    return "&";
                case UnaryOperator.UnaryPlus:
                    return "+";
                case UnaryOperator.None:
                    return string.Empty;
                default:
                    throw new ArgumentException();
            }
        }

        private void VisitAddressOfExpression(UnaryExpression node)
        {
            if (MethodReferences.Count == 0)
            {
                WriteToken(KeyWordWriter.AddressOf);
            }
            Visit(node.Operand);
        }

        public override void VisitConditionExpression(ConditionExpression node)
        {
            WriteToken("(");
            Visit(node.Condition);
            WriteTokenBetweenSpace("?");
            Visit(node.Then);
            WriteTokenBetweenSpace(":");
            Visit(node.Else);
            WriteToken(")");
        }

        public override void VisitBinaryExpression(BinaryExpression node)
        {
            if (node.Operator == BinaryOperator.NullCoalesce)
            {
                VisitNullCoalesceExpression(node);
                return;
            }
            if (node.IsChecked)
            {
                WriteKeyword(KeyWordWriter.Checked);
                Write("(");
                base.VisitBinaryExpression(node);
                Write(")");
                return;
            }
            base.VisitBinaryExpression(node);
        }

        protected override void WriteRightPartOfBinaryExpression(BinaryExpression binaryExpression)
        {
            if (binaryExpression.IsAssignmentExpression &&
                binaryExpression.Left.CodeNodeType == CodeNodeType.RefVariableDeclarationExpression)
            {
                WriteKeyword(KeyWordWriter.ByRef);
                WriteSpace();

                if (binaryExpression.Right.CodeNodeType == CodeNodeType.UnaryExpression)
                {
                    UnaryExpression unary = binaryExpression.Right as UnaryExpression;
                    if (unary.Operator == UnaryOperator.AddressReference)
                    {
                        Visit(unary.Operand);

                        return;
                    }
                }
            }

            Visit(binaryExpression.Right);
        }

        public override void VisitRefReturnExpression(RefReturnExpression node)
        {
            WriteKeyword(KeyWordWriter.Return);
            WriteSpace();
            WriteKeyword(KeyWordWriter.ByRef);
            WriteSpace();

            if (node.Value.CodeNodeType == CodeNodeType.UnaryExpression &&
                (node.Value as UnaryExpression).Operator == UnaryOperator.AddressReference)
            {
                Visit((node.Value as UnaryExpression).Operand);
            }
            else
            {
                Visit(node.Value);
            }
        }

        public override void VisitBoxExpression(BoxExpression node)
        {
            if (!node.IsAutoBox)
            {
                WriteToken("(");
                WriteReferenceAndNamespaceIfInCollision(node.ExpressionType); // the cast to object
                WriteToken(")");
            }
            bool isComplexCastTarget = IsComplexTarget(node.BoxedExpression);

            if (isComplexCastTarget && !node.IsAutoBox)
            {
                WriteToken("(");
            }

            Visit(node.BoxedExpression);

            if (isComplexCastTarget && !node.IsAutoBox)
            {
                WriteToken(")");
            }
        }

        private void VisitNullCoalesceExpression(BinaryExpression node)
        {
            Visit(node.Left);
            WriteTokenBetweenSpace("??");
            Visit(node.Right);
        }

        public override void VisitSwitchStatement(SwitchStatement node)
        {
            WriteKeyword(KeyWordWriter.Switch);

            WriteSpace();

            WriteToken("(");
            Visit(node.Condition);
            WriteToken(")");
            WriteLine();

            WriteBlock(() =>
                            {
                                Visit(node.Cases);
                                WriteLine();
                            }
            , "");
        }

        public override void VisitConditionCase(ConditionCase node)
        {
            WriteKeyword(KeyWordWriter.Case);
            WriteSpace();
            Visit(node.Condition);
            WriteToken(":");

            if (node.Body != null)
            {
                WriteLine();
                Visit(node.Body);
            }
        }

        public override void VisitDefaultCase(DefaultCase node)
        {
            WriteKeyword(KeyWordWriter.Default);
            WriteToken(":");
            WriteLine();

            Visit(node.Body);
        }

        public override void VisitForEachStatement(ForEachStatement node)
        {
            WriteKeyword(KeyWordWriter.ForEach);
            WriteSpace();
            WriteToken("(");
            Visit(node.Variable);
            WriteSpace();
            WriteKeyword(KeyWordWriter.In);
            WriteSpace();
            Visit(node.Collection);
            WriteToken(")");
            WriteLine();
            Visit(node.Body);
        }

        public override void VisitExplicitCastExpression(ExplicitCastExpression node)
        {
            if (node.IsChecked)
            {
                WriteKeyword(KeyWordWriter.Checked);
                WriteToken("(");
            }
            WriteToken("(");
            if (node.IsDynamic)
            {
                WriteDynamicType(node.TargetType, node.DynamicPositioningFlags);
            }
            else
            {
                if (node.UnresolvedReferenceForAmbiguousCastToObject == null)
                {
                    WriteReferenceAndNamespaceIfInCollision(node.TargetType);
                }
                else
                {
                    TypeReference lastResolvedType;
                    Common.Extensions.ResolveToOverloadedEqualityOperator(node.Expression.ExpressionType, out lastResolvedType);
                    WriteNotResolvedReference("object", lastResolvedType, string.Format(CastToObjectResolvementError, lastResolvedType.Name));
                }
            }
            WriteToken(")");
            
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
            if (node.IsChecked)
            {
                WriteToken(")");
            }
        }

        public override void VisitSafeCastExpression(SafeCastExpression node)
        {
            Visit(node.Expression);
            WriteSpace();
            WriteKeyword(KeyWordWriter.TryCast);
            WriteSpace();
            WriteReferenceAndNamespaceIfInCollision(node.TargetType);
        }

        public override void VisitUnaryExpression(UnaryExpression node)
        {
            if (node.Operator == UnaryOperator.Negate ||
                node.Operator == UnaryOperator.LogicalNot ||
                node.Operator == UnaryOperator.BitwiseNot ||
                node.Operator == UnaryOperator.UnaryPlus)
            {
                Write(ToString(node.Operator));

                if (node.Operand is SafeCastExpression || node.Operand is CanCastExpression)
                {
                    WriteToken("(");
                }

                base.VisitUnaryExpression(node);

                if (node.Operand is SafeCastExpression || node.Operand is CanCastExpression)
                {
                    WriteToken(")");
                }

                return;
            }

            if (node.Operator == UnaryOperator.AddressOf)
            {
                VisitAddressOfExpression(node);
                return;
            }

            if (node.Operator == UnaryOperator.AddressDereference)
            {
                if (node.Operand.CodeNodeType == CodeNodeType.VariableReferenceExpression &&
                    (node.Operand as VariableReferenceExpression).IsByReference)
                {
                    this.Visit(node.Operand);

                    return;
                }
                else if (node.Operand.CodeNodeType == CodeNodeType.ParenthesesExpression)
                {
                    ParenthesesExpression parentheses = node.Operand as ParenthesesExpression;
                    if (parentheses.Expression.CodeNodeType == CodeNodeType.MethodInvocationExpression &&
                        (parentheses.Expression as MethodInvocationExpression).IsByReference)
                    {
                        this.Visit(parentheses.Expression);

                        return;
                    }
                }

                base.VisitUnaryExpression(node);
                return;
            }

            bool isPostOp = IsPostUnaryOperator(node.Operator);

            if (!isPostOp)
                Write(ToString(node.Operator));

            Visit(node.Operand);

            if (isPostOp)
                Write(ToString(node.Operator));
        }

        public override void VisitBaseCtorExpression(BaseCtorExpression node)
        {
            VisitCtorExpression(node, "base");
        }

        public override void VisitThisCtorExpression(ThisCtorExpression node)
        {
            VisitCtorExpression(node, "this");
        }

        private void VisitCtorExpression(MethodInvocationExpression node, string ctorKeyword)
        {
            if (node.MethodExpression.CodeNodeType == CodeNodeType.MethodReferenceExpression)
            {
                WriteReference(ctorKeyword, node.MethodExpression.Method);
            }
            else
            {
                WriteKeyword(ctorKeyword);
            }

            WriteToken("(");

            EnterMethodInvocation(node.MethodExpression.Method);
            VisitMethodParameters(node.Arguments);
            LeaveMethodInvocation();

            WriteToken(")");
        }

        public override void VisitForStatement(ForStatement node)
        {
            WriteKeyword("for");
            WriteSpace();
            WriteToken("(");
            Visit(node.Initializer);
            WriteToken(";");
            WriteSpace();
            Visit(node.Condition);
            WriteToken(";");
            WriteSpace();
            Visit(node.Increment);
            WriteToken(")");
            WriteLine();
            Visit(node.Body);
        }

        public override void VisitContinueStatement(ContinueStatement node)
        {
            WriteKeyword("continue");
            WriteToken(";");
        }

        public override void VisitBreakStatement(BreakStatement node)
        {
            WriteKeyword("break");
            WriteEndOfStatement();
        }

        public override void VisitYieldReturnExpression(YieldReturnExpression node)
        {
            WriteKeyword("yield");
            WriteSpace();
            WriteKeyword("return");
            WriteSpace();
            Visit(node.Expression);
        }

        public override void VisitYieldBreakExpression(YieldBreakExpression node)
        {
            WriteKeyword("yield");
            WriteSpace();
            WriteKeyword("break");
        }

        public override void VisitDefaultObjectExpression(DefaultObjectExpression node)
        {
            WriteKeyword("default");
            WriteToken("(");
            WriteReferenceAndNamespaceIfInCollision(node.Type);
            WriteToken(")");
        }

        public override void VisitLambdaExpression(LambdaExpression node)
        {
            base.VisitLambdaExpression(node);
            WriteToken("(");
            VisitMethodParameters(node.Arguments);
            WriteToken(")");
            WriteSpace();
            WriteToken("=>");
            WriteSpace();

            if (node.Body.Statements.Count == 1)
            {
                if (node.Body.Statements[0].CodeNodeType == CodeNodeType.ExpressionStatement)
                {
                    ShouldOmitSemicolon.Push(true);
                    Visit(node.Body.Statements[0] as ExpressionStatement);
                }
                else
                {
                    ShouldOmitSemicolon.Push(false);
                    Visit(node.Body);
                }
            }
            else
            {
                ShouldOmitSemicolon.Push(false);
                Visit(node.Body);
            }

            ShouldOmitSemicolon.Pop();
        }

        public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
        {
            if (this.Language.Version != 0)
            {
                string variableName = GetVariableName(node.Variable);
                if (node.Variable.VariableType.ContainsAnonymousType())
                {
                    WriteKeyword(KeyWordWriter.Dim);
                    WriteSpace();
                    this.WriteAndMapVariableToCode(() => Write(variableName), node.Variable);
                }
                else if (node.Variable.Resolve().IsDynamic)
                {
                    WriteDynamicType(node.Variable.VariableType, node.Variable.DynamicPositioningFlags);
                    WriteSpace();
                    this.WriteAndMapVariableToCode(() => Write(variableName), node.Variable);
                }
                else
                {
                    base.VisitVariableDeclarationExpression(node);
                }
            }
            else
            {
                base.VisitVariableDeclarationExpression(node);
            }
        }

        public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
        {
            if (this.Language.Version == 0)
            {
                Write(node.Variable.Name);
            }
            else
            {
                base.VisitVariableReferenceExpression(node);
            }
        }

        //public override void VisitFieldReferenceExpression(FieldReferenceExpression node)
        //{
        //    if (this.Language.GetType() != typeof(CSharp))
        //    {
        //        string oldName = node.Field.Name;
        //        string escapedFieldName = EscapeName(oldName);
        //        node.Field.Name = escapedFieldName;
        //        base.VisitFieldReferenceExpression(node);
        //        node.Field.Name = oldName;
        //    }
        //    else
        //    {
        //        base.VisitFieldReferenceExpression(node);
        //    }
        //}

        protected override void WriteFieldName(FieldReference field)
        {
            string fieldName = GetFieldName(field);
            WriteReference(fieldName, field);
        }

        public override void VisitCatchClause(CatchClause node)
        {
            WriteKeyword(KeyWordWriter.Catch);

            if (node.Type == null && !this.Language.SupportsExceptionFilters)
            {
                throw new Exception(string.Format("Exception filters are not supported in {0}.", this.Language.Name));
            }

            if (node.Type.FullName != Constants.Object)
            {
                WriteSpace();
                WriteSpecialBetweenParenthesis(() =>
                                               {
                                                   if (node.Variable != null)
                                                       Visit(node.Variable);
                                                   else
                                                       WriteReferenceAndNamespaceIfInCollision(node.Type);
                                               });
            }

            if (node.Filter != null && this.Language.SupportsExceptionFilters)
            {
                WriteSpace();
                WriteKeyword(KeyWordWriter.When);
                WriteSpace();
                WriteSpecialBetweenParenthesis((node.Filter as ExpressionStatement).Expression);
            }

            WriteLine();
            Visit(node.Body);
        }

        public override void VisitUsingStatement(UsingStatement node)
        {
            WriteKeyword(KeyWordWriter.Using);
            WriteSpace();
            WriteSpecialBetweenParenthesis(node.Expression);
            WriteLine();
            Visit(node.Body);
            WriteSpecialEndBlock(KeyWordWriter.Using);
        }

        //commented because at the moment there is no way to find out wheather the argument is from method or property
        //thus, in order to have correct properties, some methods whit escaped parameters will decompile without the escaping character

        //public override void VisitArgumentReferenceExpression(ArgumentReferenceExpression node)
        //{
        //    string oldArgumentName = node.Parameter.Name;
        //    if (!language.IsGlobalKeyword(oldArgumentName))
        //    { 
        //        base.VisitArgumentReferenceExpression(node);
        //        return;
        //    }
        //    string escapedArgumentName = EscapeName(node.Parameter.Name);
        //    node.Parameter.Name = escapedArgumentName;
        //    base.VisitArgumentReferenceExpression(node);
        //    node.Parameter.Name = oldArgumentName;
        //}

        //protected override void WritePropertyDeclaration(PropertyDefinition property)
        //{
        //    string oldName = property.Name;
        //    string escapedPropertyName = EscapeName(property.Name);
        //    property.Name = escapedPropertyName;
        //    base.WritePropertyDeclaration(property);
        //    property.Name = oldName;
        //}

        protected override void WritePropertyName(PropertyDefinition property)
        {
            string propertyName = GetPropertyName(property);
            //if (!Language.IsValidIdentifier(propertyName))
            //{
            //	if (property.IsExplicitImplementation())
            //	{
            //		string[] nameParts = propertyName.Split('.');
            //		for (int i = 0; i < nameParts.Length; i++)
            //		{
            //			nameParts[i] = Language.ReplaceInvalidCharactersInIdentifier(nameParts[i]);
            //		}
            //		propertyName = nameParts[0];
            //		for (int i = 1; i < nameParts.Length; i++)
            //		{
            //			propertyName += "." + nameParts[i];
            //		}
            //	}
            //	else
            //	{
            //		propertyName = Language.ReplaceInvalidCharactersInIdentifier(propertyName);
            //	}
            //}

            WriteReference(propertyName, property);
        }

        //public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
        //{
        //    string oldPropertyName = node.Property.Name;
        //    string escapedPropertyName = EscapeName(oldPropertyName);
        //    node.Property.Name = escapedPropertyName;
        //    base.VisitPropertyReferenceExpression(node);
        //    node.Property.Name = oldPropertyName;
        //}

        //TODO: make it print { get; set; } on single line when they are autogenerated
        //protected override void Write(PropertyDefinition property)
        //{
        //    if (property.GetMethod != null && property.GetMethod.HasBody == false|| /// needs check for attributes count
        //        property.SetMethod != null && property.SetMethod.HasBody == false)  /// If the getter/setter have no attributes, they can be printed on single line
        //    {
        //        WritePropertyDeclaration(property);

        //        WriteSpace();
        //        Write("{ ");

        //        if (property.GetMethod != null)
        //        {
        //            AttributeWriter.WriteMemberAttributesAndNewLine(property.GetMethod);
        //            WriteMoreRestrictiveMethodVisibility(property.GetMethod, property.SetMethod);
        //            WriteKeyword(KeyWordWriter.Get);
        //            WriteEndOfStatement();
        //            WriteSpace();
        //        }

        //        if (property.SetMethod != null)
        //        {
        //            AttributeWriter.WriteMemberAttributesAndNewLine(property.GetMethod);
        //            WriteMoreRestrictiveMethodVisibility(property.SetMethod, property.GetMethod);
        //            WriteKeyword(KeyWordWriter.Set);
        //            WriteEndOfStatement();
        //            WriteSpace();
        //        }
        //        Write("}");
        //        WriteLine();
        //    }
        //    else
        //    {
        //        base.Write(property);
        //    }
        //}

        protected override void WriteEscapeCharLiteral(char c)
        {
            string theString = null;
            switch (c)
            {
                case '\'':
                    theString = @"\'";
                    break;
                case '\"':
                    theString = "\\\"";
                    break;
                case '\\':
                    theString = @"\\";
                    break;
                case '\0':
                    theString = @"\0";
                    break;
                case '\a':
                    theString = @"\a";
                    break;
                case '\b':
                    theString = @"\b";
                    break;
                case '\f':
                    theString = @"\f";
                    break;
                case '\n':
                    theString = @"\n";
                    break;
                case '\r':
                    theString = @"\r";
                    break;
                case '\t':
                    theString = @"\t";
                    break;
                case '\v':
                    theString = @"\v";
                    break;
                case ' ':
                    theString = @" ";
                    break;
            }

            if (theString != null)
            {
                WriteLiteral(CharStart);
                formatter.WriteLiteral(theString);
                WriteLiteral(CharEnd);
                return;
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
                theString = string.Format("\\u{0:X4}", (int)c);
            }
            else
            {
                theString = c.ToString();
            }

            WriteLiteral(CharStart);
            formatter.WriteLiteral(theString);
            WriteLiteral(CharEnd);
        }

        protected override bool IsComplexTarget(Expression target)
        {
            if (target.CodeNodeType == CodeNodeType.UnaryExpression)
            {
                UnaryExpression unary = target as UnaryExpression;
                if (unary.Operator == UnaryOperator.AddressDereference &&
                    unary.Operand.CodeNodeType == CodeNodeType.VariableReferenceExpression)
                {
                    VariableReferenceExpression variableReference = unary.Operand as VariableReferenceExpression;
                    if (variableReference.IsByReference)
                    {
                        // derefenced ref local
                        return false;
                    }
                }
            }

            return base.IsComplexTarget(target) ||
                   target.CodeNodeType == CodeNodeType.DynamicConstructorInvocationExpression ||
                   target.CodeNodeType == CodeNodeType.LinqQueryExpression ||
                   target.CodeNodeType == CodeNodeType.ConditionExpression ||
                   target.CodeNodeType == CodeNodeType.ExplicitCastExpression ||
                   target.CodeNodeType == CodeNodeType.SafeCastExpression;
        }

        public override void VisitDynamicMemberReferenceExpression(DynamicMemberReferenceExpression node)
        {
            bool isComplexTarget = IsComplexTarget(node.Target);
            if (isComplexTarget)
            {
                WriteToken("(");
            }
            Visit(node.Target);
            if (isComplexTarget)
            {
                WriteToken(")");
            }


            if (node.MemberName != null)
            {
                WriteToken(".");
                Write(node.MemberName);
            }

            if (node.IsMethodInvocation)
            {
                if (node.IsGenericMethod)
                {
                    WriteToken(GenericLeftBracket);
                    for (int i = 0; i < node.GenericTypeArguments.Count; i++)
                    {
                        if (i != 0)
                        {
                            WriteToken(",");
                            WriteSpace();
                        }

                        WriteReferenceAndNamespaceIfInCollision(node.GenericTypeArguments[i]);
                    }
                    WriteToken(GenericRightBracket);
                }

                WriteToken("(");
                for (int i = 0; i < node.InvocationArguments.Count; i++)
                {
                    if (i != 0)
                    {
                        WriteToken(",");
                        WriteSpace();
                    }

                    Visit(node.InvocationArguments[i]);
                }
                WriteToken(")");
            }
        }

        public override void VisitDynamicConstructorInvocationExpression(DynamicConstructorInvocationExpression node)
        {
            WriteKeyword(KeyWordWriter.New);
            WriteSpace();
            WriteReferenceAndNamespaceIfInCollision(node.ConstructorType);
            WriteToken("(");
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                if (i != 0)
                {
                    WriteToken(",");
                    WriteSpace();
                }

                Visit(node.Arguments[i]);
            }
            WriteToken(")");
        }

        IEnumerator isDynamicEnumerator = null;
        private void WriteDynamicType(TypeReference typeRef, CustomAttribute dynamicAttribute)
        {
            WriteDynamicType(typeRef, DynamicHelper.GetDynamicPositioningFlags(dynamicAttribute));
        }

        private void WriteDynamicType(TypeReference typeRef, bool[] positioningFlags)
        {
            WriteDynamicType(typeRef, positioningFlags.GetEnumerator());
        }

        private void WriteDynamicType(TypeReference typeRef, IEnumerator positioningFlagsEnumerator)
        {
            isDynamicEnumerator = positioningFlagsEnumerator;
            try
            {
                WriteReferenceAndNamespaceIfInCollision(typeRef);

                if (isDynamicEnumerator.MoveNext())
                {
                    throw new Exception("Invalid number of flags for DynamicAttribute.");
                }
            }
            finally
            {
                isDynamicEnumerator = null;
            }
        }

        internal override void WriteReference(string name, object reference)
        {
            if (isDynamicEnumerator != null)
            {
                if (!isDynamicEnumerator.MoveNext())
                {
                    throw new Exception("Invalid argument type for DynamicAttribute");
                }

                if ((bool)isDynamicEnumerator.Current)
                {
                    WriteKeyword(KeyWordWriter.Dynamic);
                    return;
                }
            }

            base.WriteReference(name, reference);
        }

        protected override void WriteMethodReference(string name, MethodReference reference)
        {
            // if the name of the class was escaped, then the name of the constructor must be escaped as well
            if (name == "this" || name == "base")
            {
                base.WriteMethodReference(name, reference);
                return;
            }

            if ((reference.Name == ".ctor" || reference.Name == ".cctor") &&
                name != "this" && name != "base")
            {
                name = Utilities.EscapeTypeNameIfNeeded(name, this.Language);
            }
            else
            {
                name = Utilities.EscapeNameIfNeeded(name, this.Language);
            }
            base.WriteMethodReference(name, reference);
        }

        protected override void WriteTypeSpecification(TypeSpecification typeSpecification, int startingArgument = 0)
        {
            if (isDynamicEnumerator != null && !typeSpecification.IsGenericInstance)
            {
                if (typeSpecification.IsArray)
                {
                    TypeReference currentTypeRef = typeSpecification;
                    while (currentTypeRef.IsArray)
                    {
                        if (!isDynamicEnumerator.MoveNext() || (bool)isDynamicEnumerator.Current)
                        {
                            throw new Exception("Invalid argument type for DynamicAttribute");
                        }
                        currentTypeRef = (currentTypeRef as ArrayType).ElementType;
                    }
                }
                else if (!isDynamicEnumerator.MoveNext() || (bool)isDynamicEnumerator.Current)
                {
                    throw new Exception("Invalid argument type for DynamicAttribute");
                }
            }

            if (typeSpecification.IsRequiredModifier || typeSpecification.IsOptionalModifier)
            {
                WriteModifier(typeSpecification);
                return;
            }

            base.WriteTypeSpecification(typeSpecification, startingArgument);
        }

        private void WriteModifier(TypeSpecification typeSpecification)
        {
            StringBuilder sb = new StringBuilder();
            TypeReference elementType = typeSpecification;
            IModifierType currentType = typeSpecification as IModifierType;
            bool wroteModifier = false;
            while (currentType != null)
            {
                if (currentType.ModifierType.FullName == "System.Runtime.CompilerServices.IsVolatile")
                {
                    elementType = currentType.ElementType;
                    currentType = elementType as IModifierType;
                    continue;
                }

                if (wroteModifier)
                {
                    sb.Append(", ");
                }

                sb.Append(currentType is RequiredModifierType ? "modreq" : "modopt");
                sb.Append("(");
                sb.Append(currentType.ModifierType.FullName);
                sb.Append(")");
                wroteModifier = true;

                elementType = currentType.ElementType;
                currentType = elementType as IModifierType;
            }

            if (wroteModifier)
            {
                WriteCommentBlock(sb.ToString());
                WriteSpace();
            }

            WriteReferenceAndNamespaceIfInCollision(elementType);
        }

        private void WriteCommentBlock(string commentedText)
        {
            StartWritingComment(true);
            Write(commentedText);
            EndWritingComment(true);
        }

        protected override void WriteMethodReturnType(MethodDefinition method)
        {
            CustomAttribute dynamicAttribute;
            if (method.MethodReturnType.TryGetDynamicAttribute(out dynamicAttribute))
            {
                WriteDynamicType(method.ReturnType, dynamicAttribute);
                return;
            }

            if (method.ReturnType.IsByReference)
            {
                this.WriteKeyword(this.KeyWordWriter.ByRef);
                this.WriteSpace();
                WriteReferenceAndNamespaceIfInCollision(method.ReturnType.GetElementType());

                return;
            }

            base.WriteMethodReturnType(method);
        }

        protected override void WritePropertyTypeAndNameWithArguments(PropertyDefinition property)
        {
            string name = GetPropertyName(property);

            CustomAttribute dynamicAttribute;
            if (property.TryGetDynamicAttribute(out dynamicAttribute))
            {
                WriteDynamicType(property.PropertyType, dynamicAttribute);
                WriteSpace();
                WriteReference(name, property);
                return;
            }
            base.WriteTypeAndName(property.PropertyType, name, property);

            if (HasArguments(property))
            {
                WriteToken("(");
                WritePropertyParameters(property);
                WriteToken(")");
            }
        }

        protected override void WriteMethodReference(MethodReferenceExpression methodReferenceExpression)
        {
            MethodReference methodReference = methodReferenceExpression.Method;

            string methodName = GetMethodName(methodReference);

            if (methodReferenceExpression.Method is GenericInstanceMethod)
            {
                WriteGenericInstanceMethod(methodReference as GenericInstanceMethod);
                return;
            }
            if (HasRefOrOutParameter(methodReferenceExpression.Method))
            {
                MethodDefinition md = methodReference.Resolve();
                if (md == null)
                {
                    WriteNotResolvedReference(methodName, methodReference, RefOutResolvementError);
                    return;
                }
            }
            WriteReference(methodName, methodReference);
        }

        protected override void WriteGenericInstanceMethod(GenericInstanceMethod genericMethod)
        {
            MethodReference method = genericMethod.ElementMethod;

            string methodName = GetMethodName(method);

            if (HasRefOrOutParameter(method))
            {
                MethodDefinition methodDef = genericMethod.ElementMethod.Resolve();
                if (methodDef == null)
                {
                    WriteNotResolvedReference(methodName, genericMethod, RefOutResolvementError);
                }
                else
                {
                    WriteReference(methodName, methodDef);
                }
            }
            else
            {
                WriteReference(methodName, genericMethod);
            }

            if (genericMethod.HasAnonymousArgument())
            {
                return;
            }

            WriteToken(GenericLeftBracket);

            Mono.Collections.Generic.Collection<TypeReference> arguments = genericMethod.GenericArguments;
            for (int i = 0; i < arguments.Count; i++)
            {
                if (i > 0)
                {
                    WriteToken(",");
                    WriteSpace();
                }
                WriteReferenceAndNamespaceIfInCollision(arguments[i]);
            }
            WriteToken(GenericRightBracket);
            return;
        }

        private bool HasRefOrOutParameter(MethodReference method)
        {
            foreach (ParameterDefinition parameter in method.Parameters)
            {
                if (parameter.ParameterType.IsByReference)
                {
                    return true;
                }
            }
            return false;
        }

        protected override void WriteFire(EventDefinition @event)
        {
        }

        protected override bool WriteMethodVisibility(MethodDefinition method)
        {
            if (method.HasOverrides && method.IsPrivate)
            {
                return false;
            }
            return base.WriteMethodVisibility(method);
        }

        protected override void PostWriteGenericParametersConstraints(IGenericDefinition genericDefinition)
        {
            if (genericDefinition.HasGenericParameters)
            {
                foreach (GenericParameter genericParameter in genericDefinition.GenericParameters)
                {
                    if (genericParameter.HasNotNullableValueTypeConstraint || genericParameter.HasDefaultConstructorConstraint ||
                        genericParameter.HasConstraints || genericParameter.HasReferenceTypeConstraint)
                    {
                        WriteConstraints(genericParameter);
                    }
                }
            }
        }

        private void WriteConstraints(GenericParameter genericParameter)
        {
            if (!IsTypeParameterRedeclaration(genericParameter))
            {
                WriteLine();
                DoWriteGenericConstraints(genericParameter);
            }
            else if (ShouldWriteConstraintsAsComment(genericParameter))
            {
                WriteLine();
                StartWritingComment();
                DoWriteGenericConstraints(genericParameter);
                EndWritingComment();
            }
        }

        private void DoWriteGenericConstraints(GenericParameter genericParameter)
        {
            WriteKeyword("where");
            WriteSpace();
            WriteReferenceAndNamespaceIfInCollision(genericParameter);
            WriteTokenBetweenSpace(":");
            WriteSingleGenericParameterConstraintsList(genericParameter);
        }

        protected override void WriteConstructorGenericConstraint()
        {
            base.WriteConstructorGenericConstraint();
            WriteToken("(");
            WriteToken(")");
        }

        protected override void WriteBaseConstructorInvokation(MethodInvocationExpression baseConstructorInvokation)
        {
            if (baseConstructorInvokation.Arguments.Count > 0 || baseConstructorInvokation.CodeNodeType == CodeNodeType.ThisCtorExpression)
            {
                WriteSpace();
                WriteToken(":");
                WriteSpace();
                Visit(baseConstructorInvokation);
            }
        }

        protected override void StartWritingComment()
        {
            StartWritingComment(false);
        }

        protected override void EndWritingComment()
        {
            EndWritingComment(false);
        }

        private void StartWritingComment(bool isBlock)
        {
            this.isWritingComment = true;

            if (isBlock)
            {
                formatter.WriteComment("/* ");
            }
            else
            {
                formatter.WriteComment(Language.CommentLineSymbol + " ");
            }
            base.StartWritingComment();
        }

        private void EndWritingComment(bool isBlock)
        {
            this.isWritingComment = false;

            if (isBlock)
            {
                formatter.WriteComment(" */");
            }
            base.EndWritingComment();
        }

        private string GetGenericNameFromMemberReference(TypeReference type)
        {
            IGenericDefinition genericInstance = type.Resolve();
            if (genericInstance != null)
            {
                return genericInstance.GetGenericName(Language);
            }
            else
            {
                return type.GetGenericName(Language);
            }
        }

        public override void VisitBreakSwitchCaseStatement(BreakSwitchCaseStatement node)
        {
            WriteKeyword("break");
            WriteEndOfStatement();
        }

        public override void VisitCaseGotoStatement(CaseGotoStatement node)
        {
            WriteKeyword(KeyWordWriter.GoTo);
            WriteSpace();
            SwitchCase targetedCase = node.TargetedSwitchCase;
            if (targetedCase.CodeNodeType == CodeNodeType.ConditionCase)
            {
                WriteKeyword(KeyWordWriter.Case);
                WriteSpace();
                Visit((targetedCase as ConditionCase).Condition);
            }
            else
            {
                WriteKeyword(KeyWordWriter.Default);
            }
            WriteEndOfStatement();
        }

        public override void VisitLinqQueryExpression(LinqQueryExpression node)
        {
            Indent();
            foreach (QueryClause clause in node.Clauses)
            {
                if (clause.CodeNodeType != CodeNodeType.IntoClause)
                {
                    WriteLine();
                }
                else
                {
                    WriteSpace();
                }
                Visit(clause);
            }
            Outdent();
        }

        protected override bool TypeSupportsExplicitStaticMembers(TypeDefinition type)
        {
            // All C# classes (static or instance) support explicit static members.
            return true;
        }

        /// <summary>
        /// Gets the type string for given type reference. If the type string is a system type and it's in collision with
        /// some keyword, it's escaped.
        /// </summary>
        internal override string ToEscapedTypeString(TypeReference reference)
        {
            // There is no need to escape the system types in C#, since all type names are starting with capital letter,
            // and all keywords are starting with non-capital letter.
            return ToTypeString(reference);
        }
    }
}