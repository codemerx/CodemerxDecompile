using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Languages.CSharp
{
	public class CSharpWriter : NamespaceImperativeLanguageWriter
	{
		protected const string RefOutResolvementError = "Out parameters might be shown as ref. Please, locate the assembly where the method is defined.";

		private IEnumerator isDynamicEnumerator;

		protected override string CharEnd
		{
			get
			{
				return "'";
			}
		}

		protected override string CharStart
		{
			get
			{
				return "'";
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

		protected override bool RemoveBaseConstructorInvocation
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

		public CSharpWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
		{
			base(language, formatter, exceptionFormatter, settings);
			return;
		}

		protected override Telerik.JustDecompiler.Languages.AttributeWriter CreateAttributeWriter()
		{
			return new CSharpAttributeWriter(this);
		}

		protected override IKeyWordWriter CreateKeyWordWriter()
		{
			return new CSharpKeyWordWriter();
		}

		private void DoWriteGenericConstraints(GenericParameter genericParameter)
		{
			this.WriteKeyword("where");
			this.WriteSpace();
			this.WriteReferenceAndNamespaceIfInCollision(genericParameter);
			this.WriteTokenBetweenSpace(":");
			this.WriteSingleGenericParameterConstraintsList(genericParameter);
			return;
		}

		protected override void DoWriteParameterTypeAndName(TypeReference type, string name, ParameterDefinition reference)
		{
			V_0 = new CSharpWriter.u003cu003ec__DisplayClass24_0();
			V_0.u003cu003e4__this = this;
			V_0.name = name;
			if (!this.get_Language().IsValidIdentifier(V_0.name))
			{
				V_0.name = this.get_Language().ReplaceInvalidCharactersInIdentifier(V_0.name);
			}
			if (reference.IsParamArray())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_ParamArray());
				this.WriteSpace();
			}
			if (!reference.TryGetDynamicAttribute(out V_1))
			{
				if (!String.IsNullOrEmpty(this.ToTypeString(type)))
				{
					this.WriteReferenceAndNamespaceIfInCollision(type);
					this.WriteSpace();
				}
			}
			else
			{
				V_2 = DynamicHelper.GetDynamicPositioningFlags(V_1).GetEnumerator();
				if (reference.get_ParameterType().get_IsByReference() && (object)(reference.get_ParameterType() as ByReferenceType).get_ElementType() == (object)type && !V_2.MoveNext() || (Boolean)V_2.get_Current())
				{
					throw new Exception("Invalid argument type for DynamicAttribute");
				}
				this.WriteDynamicType(type, V_2);
				this.WriteSpace();
			}
			if (this.get_Language().IsGlobalKeyword(V_0.name))
			{
				V_0.name = Utilities.EscapeNameIfNeeded(V_0.name, this.get_Language());
			}
			this.WriteAndMapParameterToCode(new Action(V_0.u003cDoWriteParameterTypeAndNameu003eb__0), reference.get_Index());
			return;
		}

		protected override void DoWriteTypeAndName(TypeReference typeReference, string name, object reference)
		{
			this.WriteReferenceAndNamespaceIfInCollision(typeReference);
			this.WriteSpace();
			V_0 = this.formatter.get_CurrentPosition();
			this.WriteReference(name, reference);
			if (reference as IMemberDefinition != null)
			{
				V_1 = this.formatter.get_CurrentPosition() - 1;
				this.currentWritingInfo.get_MemberDeclarationToCodePostionMap().set_Item((IMemberDefinition)reference, new OffsetSpan(V_0, V_1));
			}
			return;
		}

		protected override void DoWriteTypeAndName(TypeReference typeReference, string name)
		{
			this.WriteReferenceAndNamespaceIfInCollision(typeReference);
			this.WriteSpace();
			this.Write(name);
			return;
		}

		protected override void DoWriteVariableTypeAndName(VariableDefinition variable)
		{
			this.WriteReferenceAndNamespaceIfInCollision(variable.get_VariableType());
			this.WriteSpace();
			this.WriteVariableName(variable);
			return;
		}

		protected override void EndWritingComment()
		{
			this.EndWritingComment(false);
			return;
		}

		private void EndWritingComment(bool isBlock)
		{
			this.isWritingComment = false;
			if (isBlock)
			{
				this.formatter.WriteComment(" */");
			}
			this.EndWritingComment();
			return;
		}

		private string GetGenericNameFromMemberReference(TypeReference type)
		{
			V_0 = type.Resolve();
			if (V_0 != null)
			{
				return V_0.GetGenericName(this.get_Language(), "<", ">");
			}
			return type.GetGenericName(this.get_Language(), "<", ">");
		}

		private bool HasRefOrOutParameter(MethodReference method)
		{
			V_0 = method.get_Parameters().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					if (!V_0.get_Current().get_ParameterType().get_IsByReference())
					{
						continue;
					}
					V_1 = true;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				V_0.Dispose();
			}
		Label1:
			return V_1;
		Label0:
			return false;
		}

		protected override bool IsComplexTarget(Expression target)
		{
			if (target.get_CodeNodeType() == 23)
			{
				V_0 = target as UnaryExpression;
				if (V_0.get_Operator() == 8 && V_0.get_Operand().get_CodeNodeType() == 26 && (V_0.get_Operand() as VariableReferenceExpression).get_IsByReference())
				{
					return false;
				}
			}
			if (this.IsComplexTarget(target) || target.get_CodeNodeType() == 60 || target.get_CodeNodeType() == 81 || target.get_CodeNodeType() == 36 || target.get_CodeNodeType() == 31)
			{
				return true;
			}
			return target.get_CodeNodeType() == 33;
		}

		protected override void PostWriteGenericParametersConstraints(IGenericDefinition genericDefinition)
		{
			if (genericDefinition.get_HasGenericParameters())
			{
				V_0 = genericDefinition.get_GenericParameters().GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_1 = V_0.get_Current();
						if (!V_1.get_HasNotNullableValueTypeConstraint() && !V_1.get_HasDefaultConstructorConstraint() && !V_1.get_HasConstraints() && !V_1.get_HasReferenceTypeConstraint())
						{
							continue;
						}
						this.WriteConstraints(V_1);
					}
				}
				finally
				{
					V_0.Dispose();
				}
			}
			return;
		}

		protected override void StartWritingComment()
		{
			this.StartWritingComment(false);
			return;
		}

		private void StartWritingComment(bool isBlock)
		{
			this.isWritingComment = true;
			if (!isBlock)
			{
				this.formatter.WriteComment(String.Concat(this.get_Language().get_CommentLineSymbol(), " "));
			}
			else
			{
				this.formatter.WriteComment("/* ");
			}
			this.StartWritingComment();
			return;
		}

		internal override string ToEscapedTypeString(TypeReference reference)
		{
			return this.ToTypeString(reference);
		}

		protected override string ToString(BinaryOperator op, bool isOneSideNull = false)
		{
			switch (op - 1)
			{
				case 0:
				{
					return "+";
				}
				case 1:
				{
					return "+=";
				}
				case 2:
				{
					return "-";
				}
				case 3:
				{
					return "-=";
				}
				case 4:
				{
					return "*";
				}
				case 5:
				{
					return "*=";
				}
				case 6:
				{
					return "/";
				}
				case 7:
				{
					return "/=";
				}
				case 8:
				{
					return "==";
				}
				case 9:
				{
					return "!=";
				}
				case 10:
				{
					return "||";
				}
				case 11:
				{
					return "&&";
				}
				case 12:
				{
					return "<";
				}
				case 13:
				{
					return "<=";
				}
				case 14:
				{
					return ">";
				}
				case 15:
				{
					return ">=";
				}
				case 16:
				{
					return "<<";
				}
				case 17:
				{
					return "<<=";
				}
				case 18:
				{
					return ">>";
				}
				case 19:
				{
					return ">>=";
				}
				case 20:
				{
					return "|";
				}
				case 21:
				{
					return "&";
				}
				case 22:
				{
					return "^";
				}
				case 23:
				{
					return "%";
				}
				case 24:
				{
					return "%=";
				}
				case 25:
				{
					return "=";
				}
				case 26:
				{
					return "??";
				}
				case 27:
				{
					return "&=";
				}
				case 28:
				{
					return "|=";
				}
				case 29:
				{
					return "^=";
				}
			}
			throw new ArgumentException();
		}

		protected override string ToString(UnaryOperator op)
		{
			switch (op)
			{
				case 0:
				{
					return "-";
				}
				case 1:
				{
					return "!";
				}
				case 2:
				{
					return "~";
				}
				case 3:
				case 5:
				{
					return "--";
				}
				case 4:
				case 6:
				{
					return "++";
				}
				case 7:
				case 9:
				{
					return "&";
				}
				case 8:
				{
					return "*";
				}
				case 10:
				{
					return "+";
				}
				case 11:
				{
					return String.Empty;
				}
			}
			throw new ArgumentException();
		}

		public override string ToTypeString(TypeReference type)
		{
			if (!this.IsReferenceFromMscorlib(type))
			{
				return this.GetGenericNameFromMemberReference(type);
			}
			V_0 = type.get_Name();
			if (V_0 != null)
			{
				if (String.op_Equality(V_0, "Decimal"))
				{
					return "decimal";
				}
				if (String.op_Equality(V_0, "Single"))
				{
					return "float";
				}
				if (String.op_Equality(V_0, "Byte"))
				{
					return "byte";
				}
				if (String.op_Equality(V_0, "SByte"))
				{
					return "sbyte";
				}
				if (String.op_Equality(V_0, "Char"))
				{
					return "char";
				}
				if (String.op_Equality(V_0, "Double"))
				{
					return "double";
				}
				if (String.op_Equality(V_0, "Boolean"))
				{
					return "bool";
				}
				if (String.op_Equality(V_0, "Int16"))
				{
					return "short";
				}
				if (String.op_Equality(V_0, "Int32"))
				{
					return "int";
				}
				if (String.op_Equality(V_0, "Int64"))
				{
					return "long";
				}
				if (String.op_Equality(V_0, "UInt16"))
				{
					return "ushort";
				}
				if (String.op_Equality(V_0, "UInt32"))
				{
					return "uint";
				}
				if (String.op_Equality(V_0, "UInt64"))
				{
					return "ulong";
				}
				if (String.op_Equality(V_0, "String"))
				{
					return "string";
				}
				if (String.op_Equality(V_0, "Void"))
				{
					return "void";
				}
				if (String.op_Equality(V_0, "Object"))
				{
					return "object";
				}
				if (String.op_Equality(V_0, "RuntimeArgumentHandle"))
				{
					return String.Empty;
				}
			}
			return this.GetGenericNameFromMemberReference(type);
		}

		protected override bool TypeSupportsExplicitStaticMembers(TypeDefinition type)
		{
			return true;
		}

		private void VisitAddressOfExpression(UnaryExpression node)
		{
			if (this.get_MethodReferences().get_Count() == 0)
			{
				this.WriteToken(this.get_KeyWordWriter().get_AddressOf());
			}
			this.Visit(node.get_Operand());
			return;
		}

		public override void VisitBaseCtorExpression(BaseCtorExpression node)
		{
			this.VisitCtorExpression(node, "base");
			return;
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			if (node.get_Operator() == 27)
			{
				this.VisitNullCoalesceExpression(node);
				return;
			}
			if (!node.get_IsChecked())
			{
				this.VisitBinaryExpression(node);
				return;
			}
			this.WriteKeyword(this.get_KeyWordWriter().get_Checked());
			this.Write("(");
			this.VisitBinaryExpression(node);
			this.Write(")");
			return;
		}

		public override void VisitBoxExpression(BoxExpression node)
		{
			if (!node.get_IsAutoBox())
			{
				this.WriteToken("(");
				this.WriteReferenceAndNamespaceIfInCollision(node.get_ExpressionType());
				this.WriteToken(")");
			}
			stackVariable5 = this.IsComplexTarget(node.get_BoxedExpression());
			if (stackVariable5 && !node.get_IsAutoBox())
			{
				this.WriteToken("(");
			}
			this.Visit(node.get_BoxedExpression());
			if (stackVariable5 && !node.get_IsAutoBox())
			{
				this.WriteToken(")");
			}
			return;
		}

		public override void VisitBreakStatement(BreakStatement node)
		{
			this.WriteKeyword("break");
			this.WriteEndOfStatement();
			return;
		}

		public override void VisitBreakSwitchCaseStatement(BreakSwitchCaseStatement node)
		{
			this.WriteKeyword("break");
			this.WriteEndOfStatement();
			return;
		}

		public override void VisitCaseGotoStatement(CaseGotoStatement node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_GoTo());
			this.WriteSpace();
			V_0 = node.get_TargetedSwitchCase();
			if (V_0.get_CodeNodeType() != 13)
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Default());
			}
			else
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Case());
				this.WriteSpace();
				this.Visit((V_0 as ConditionCase).get_Condition());
			}
			this.WriteEndOfStatement();
			return;
		}

		public override void VisitCatchClause(CatchClause node)
		{
			V_0 = new CSharpWriter.u003cu003ec__DisplayClass79_0();
			V_0.node = node;
			V_0.u003cu003e4__this = this;
			this.WriteKeyword(this.get_KeyWordWriter().get_Catch());
			if (V_0.node.get_Type() == null && !this.get_Language().get_SupportsExceptionFilters())
			{
				throw new Exception(String.Format("Exception filters are not supported in {0}.", this.get_Language().get_Name()));
			}
			if (String.op_Inequality(V_0.node.get_Type().get_FullName(), "System.Object"))
			{
				this.WriteSpace();
				this.WriteSpecialBetweenParenthesis(new Action(V_0.u003cVisitCatchClauseu003eb__0));
			}
			if (V_0.node.get_Filter() != null && this.get_Language().get_SupportsExceptionFilters())
			{
				this.WriteSpace();
				this.WriteKeyword(this.get_KeyWordWriter().get_When());
				this.WriteSpace();
				this.WriteSpecialBetweenParenthesis((V_0.node.get_Filter() as ExpressionStatement).get_Expression());
			}
			this.WriteLine();
			this.Visit(V_0.node.get_Body());
			return;
		}

		public override void VisitConditionCase(ConditionCase node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Case());
			this.WriteSpace();
			this.Visit(node.get_Condition());
			this.WriteToken(":");
			if (node.get_Body() != null)
			{
				this.WriteLine();
				this.Visit(node.get_Body());
			}
			return;
		}

		public override void VisitConditionExpression(ConditionExpression node)
		{
			this.WriteToken("(");
			this.Visit(node.get_Condition());
			this.WriteTokenBetweenSpace("?");
			this.Visit(node.get_Then());
			this.WriteTokenBetweenSpace(":");
			this.Visit(node.get_Else());
			this.WriteToken(")");
			return;
		}

		public override void VisitContinueStatement(ContinueStatement node)
		{
			this.WriteKeyword("continue");
			this.WriteToken(";");
			return;
		}

		private void VisitCtorExpression(MethodInvocationExpression node, string ctorKeyword)
		{
			if (node.get_MethodExpression().get_CodeNodeType() != 20)
			{
				this.WriteKeyword(ctorKeyword);
			}
			else
			{
				this.WriteReference(ctorKeyword, node.get_MethodExpression().get_Method());
			}
			this.WriteToken("(");
			this.EnterMethodInvocation(node.get_MethodExpression().get_Method());
			this.VisitMethodParameters(node.get_Arguments());
			this.LeaveMethodInvocation();
			this.WriteToken(")");
			return;
		}

		public override void VisitDefaultCase(DefaultCase node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Default());
			this.WriteToken(":");
			this.WriteLine();
			this.Visit(node.get_Body());
			return;
		}

		public override void VisitDefaultObjectExpression(DefaultObjectExpression node)
		{
			this.WriteKeyword("default");
			this.WriteToken("(");
			this.WriteReferenceAndNamespaceIfInCollision(node.get_Type());
			this.WriteToken(")");
			return;
		}

		public override void VisitDynamicConstructorInvocationExpression(DynamicConstructorInvocationExpression node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_New());
			this.WriteSpace();
			this.WriteReferenceAndNamespaceIfInCollision(node.get_ConstructorType());
			this.WriteToken("(");
			V_0 = 0;
			while (V_0 < node.get_Arguments().get_Count())
			{
				if (V_0 != 0)
				{
					this.WriteToken(",");
					this.WriteSpace();
				}
				this.Visit(node.get_Arguments().get_Item(V_0));
				V_0 = V_0 + 1;
			}
			this.WriteToken(")");
			return;
		}

		public override void VisitDynamicMemberReferenceExpression(DynamicMemberReferenceExpression node)
		{
			stackVariable3 = this.IsComplexTarget(node.get_Target());
			if (stackVariable3)
			{
				this.WriteToken("(");
			}
			this.Visit(node.get_Target());
			if (stackVariable3)
			{
				this.WriteToken(")");
			}
			if (node.get_MemberName() != null)
			{
				this.WriteToken(".");
				this.Write(node.get_MemberName());
			}
			if (node.get_IsMethodInvocation())
			{
				if (node.get_IsGenericMethod())
				{
					this.WriteToken(this.get_GenericLeftBracket());
					V_0 = 0;
					while (V_0 < node.get_GenericTypeArguments().get_Count())
					{
						if (V_0 != 0)
						{
							this.WriteToken(",");
							this.WriteSpace();
						}
						this.WriteReferenceAndNamespaceIfInCollision(node.get_GenericTypeArguments().get_Item(V_0));
						V_0 = V_0 + 1;
					}
					this.WriteToken(this.get_GenericRightBracket());
				}
				this.WriteToken("(");
				V_1 = 0;
				while (V_1 < node.get_InvocationArguments().get_Count())
				{
					if (V_1 != 0)
					{
						this.WriteToken(",");
						this.WriteSpace();
					}
					this.Visit(node.get_InvocationArguments().get_Item(V_1));
					V_1 = V_1 + 1;
				}
				this.WriteToken(")");
			}
			return;
		}

		public override void VisitExplicitCastExpression(ExplicitCastExpression node)
		{
			if (node.get_IsChecked())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Checked());
				this.WriteToken("(");
			}
			this.WriteToken("(");
			if (!node.get_IsDynamic())
			{
				if (node.get_UnresolvedReferenceForAmbiguousCastToObject() != null)
				{
					dummyVar0 = Extensions.ResolveToOverloadedEqualityOperator(node.get_Expression().get_ExpressionType(), out V_0);
					this.WriteNotResolvedReference("object", V_0, String.Format("The cast to object might be unnecessary. Please, locate the assembly where \"{0}\" is defined.", V_0.get_Name()));
				}
				else
				{
					this.WriteReferenceAndNamespaceIfInCollision(node.get_TargetType());
				}
			}
			else
			{
				this.WriteDynamicType(node.get_TargetType(), node.get_DynamicPositioningFlags());
			}
			this.WriteToken(")");
			stackVariable25 = this.IsComplexTarget(node.get_Expression());
			if (stackVariable25)
			{
				this.WriteToken("(");
			}
			this.Visit(node.get_Expression());
			if (stackVariable25)
			{
				this.WriteToken(")");
			}
			if (node.get_IsChecked())
			{
				this.WriteToken(")");
			}
			return;
		}

		public override void VisitForEachStatement(ForEachStatement node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_ForEach());
			this.WriteSpace();
			this.WriteToken("(");
			this.Visit(node.get_Variable());
			this.WriteSpace();
			this.WriteKeyword(this.get_KeyWordWriter().get_In());
			this.WriteSpace();
			this.Visit(node.get_Collection());
			this.WriteToken(")");
			this.WriteLine();
			this.Visit(node.get_Body());
			return;
		}

		public override void VisitForStatement(ForStatement node)
		{
			this.WriteKeyword("for");
			this.WriteSpace();
			this.WriteToken("(");
			this.Visit(node.get_Initializer());
			this.WriteToken(";");
			this.WriteSpace();
			this.Visit(node.get_Condition());
			this.WriteToken(";");
			this.WriteSpace();
			this.Visit(node.get_Increment());
			this.WriteToken(")");
			this.WriteLine();
			this.Visit(node.get_Body());
			return;
		}

		public override void VisitLambdaExpression(LambdaExpression node)
		{
			this.VisitLambdaExpression(node);
			this.WriteToken("(");
			this.VisitMethodParameters(node.get_Arguments());
			this.WriteToken(")");
			this.WriteSpace();
			this.WriteToken("=>");
			this.WriteSpace();
			if (node.get_Body().get_Statements().get_Count() != 1)
			{
				this.get_ShouldOmitSemicolon().Push(false);
				this.Visit(node.get_Body());
			}
			else
			{
				if (node.get_Body().get_Statements().get_Item(0).get_CodeNodeType() != 5)
				{
					this.get_ShouldOmitSemicolon().Push(false);
					this.Visit(node.get_Body());
				}
				else
				{
					this.get_ShouldOmitSemicolon().Push(true);
					this.Visit(node.get_Body().get_Statements().get_Item(0) as ExpressionStatement);
				}
			}
			dummyVar0 = this.get_ShouldOmitSemicolon().Pop();
			return;
		}

		public override void VisitLinqQueryExpression(LinqQueryExpression node)
		{
			this.Indent();
			V_0 = node.get_Clauses().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1.get_CodeNodeType() == 80)
					{
						this.WriteSpace();
					}
					else
					{
						this.WriteLine();
					}
					this.Visit(V_1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			this.Outdent();
			return;
		}

		private void VisitNullCoalesceExpression(BinaryExpression node)
		{
			this.Visit(node.get_Left());
			this.WriteTokenBetweenSpace("??");
			this.Visit(node.get_Right());
			return;
		}

		public override void VisitRefReturnExpression(RefReturnExpression node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Return());
			this.WriteSpace();
			this.WriteKeyword(this.get_KeyWordWriter().get_ByRef());
			this.WriteSpace();
			if (node.get_Value().get_CodeNodeType() != 23 || (node.get_Value() as UnaryExpression).get_Operator() != 7)
			{
				this.Visit(node.get_Value());
				return;
			}
			this.Visit((node.get_Value() as UnaryExpression).get_Operand());
			return;
		}

		public override void VisitRefVariableDeclarationExpression(RefVariableDeclarationExpression node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_ByRef());
			this.WriteSpace();
			this.WriteReferenceAndNamespaceIfInCollision(node.get_Variable().get_VariableType().GetElementType());
			this.WriteSpace();
			this.WriteVariableName(node.get_Variable());
			return;
		}

		public override void VisitSafeCastExpression(SafeCastExpression node)
		{
			this.Visit(node.get_Expression());
			this.WriteSpace();
			this.WriteKeyword(this.get_KeyWordWriter().get_TryCast());
			this.WriteSpace();
			this.WriteReferenceAndNamespaceIfInCollision(node.get_TargetType());
			return;
		}

		public override void VisitSwitchStatement(SwitchStatement node)
		{
			V_0 = new CSharpWriter.u003cu003ec__DisplayClass59_0();
			V_0.u003cu003e4__this = this;
			V_0.node = node;
			this.WriteKeyword(this.get_KeyWordWriter().get_Switch());
			this.WriteSpace();
			this.WriteToken("(");
			this.Visit(V_0.node.get_Condition());
			this.WriteToken(")");
			this.WriteLine();
			this.WriteBlock(new Action(V_0.u003cVisitSwitchStatementu003eb__0), "");
			return;
		}

		public override void VisitThisCtorExpression(ThisCtorExpression node)
		{
			this.VisitCtorExpression(node, "this");
			return;
		}

		public override void VisitUnaryExpression(UnaryExpression node)
		{
			if (node.get_Operator() == UnaryOperator.Negate || node.get_Operator() == 1 || node.get_Operator() == 2 || node.get_Operator() == 10)
			{
				this.Write(this.ToString(node.get_Operator()));
				if (node.get_Operand() as SafeCastExpression != null || node.get_Operand() as CanCastExpression != null)
				{
					this.WriteToken("(");
				}
				this.VisitUnaryExpression(node);
				if (node.get_Operand() as SafeCastExpression != null || node.get_Operand() as CanCastExpression != null)
				{
					this.WriteToken(")");
				}
				return;
			}
			if (node.get_Operator() == 9)
			{
				this.VisitAddressOfExpression(node);
				return;
			}
			if (node.get_Operator() != 8)
			{
				stackVariable43 = this.IsPostUnaryOperator(node.get_Operator());
				if (!stackVariable43)
				{
					this.Write(this.ToString(node.get_Operator()));
				}
				this.Visit(node.get_Operand());
				if (stackVariable43)
				{
					this.Write(this.ToString(node.get_Operator()));
				}
				return;
			}
			if (node.get_Operand().get_CodeNodeType() == 26 && (node.get_Operand() as VariableReferenceExpression).get_IsByReference())
			{
				this.Visit(node.get_Operand());
				return;
			}
			if (node.get_Operand().get_CodeNodeType() == 87)
			{
				V_0 = node.get_Operand() as ParenthesesExpression;
				if (V_0.get_Expression().get_CodeNodeType() == 19 && (V_0.get_Expression() as MethodInvocationExpression).get_IsByReference())
				{
					this.Visit(V_0.get_Expression());
					return;
				}
			}
			this.VisitUnaryExpression(node);
			return;
		}

		public override void VisitUnsafeBlockStatement(UnsafeBlockStatement unsafeBlock)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Unsafe());
			this.WriteLine();
			this.VisitBlockStatement(unsafeBlock);
			return;
		}

		public override void VisitUsingStatement(UsingStatement node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Using());
			this.WriteSpace();
			this.WriteSpecialBetweenParenthesis(node.get_Expression());
			this.WriteLine();
			this.Visit(node.get_Body());
			this.WriteSpecialEndBlock(this.get_KeyWordWriter().get_Using());
			return;
		}

		public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			V_0 = new CSharpWriter.u003cu003ec__DisplayClass76_0();
			V_0.u003cu003e4__this = this;
			if (this.get_Language().get_Version() == 0)
			{
				this.VisitVariableDeclarationExpression(node);
				return;
			}
			V_0.variableName = this.GetVariableName(node.get_Variable());
			if (node.get_Variable().get_VariableType().ContainsAnonymousType())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Dim());
				this.WriteSpace();
				this.WriteAndMapVariableToCode(new Action(V_0.u003cVisitVariableDeclarationExpressionu003eb__0), node.get_Variable());
				return;
			}
			if (!node.get_Variable().Resolve().get_IsDynamic())
			{
				this.VisitVariableDeclarationExpression(node);
				return;
			}
			this.WriteDynamicType(node.get_Variable().get_VariableType(), node.get_Variable().get_DynamicPositioningFlags());
			this.WriteSpace();
			this.WriteAndMapVariableToCode(new Action(V_0.u003cVisitVariableDeclarationExpressionu003eb__1), node.get_Variable());
			return;
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if (this.get_Language().get_Version() != 0)
			{
				this.VisitVariableReferenceExpression(node);
				return;
			}
			this.Write(node.get_Variable().get_Name());
			return;
		}

		public override void VisitYieldBreakExpression(YieldBreakExpression node)
		{
			this.WriteKeyword("yield");
			this.WriteSpace();
			this.WriteKeyword("break");
			return;
		}

		public override void VisitYieldReturnExpression(YieldReturnExpression node)
		{
			this.WriteKeyword("yield");
			this.WriteSpace();
			this.WriteKeyword("return");
			this.WriteSpace();
			this.Visit(node.get_Expression());
			return;
		}

		protected override void WriteBaseConstructorInvokation(MethodInvocationExpression baseConstructorInvokation)
		{
			if (baseConstructorInvokation.get_Arguments().get_Count() > 0 || baseConstructorInvokation.get_CodeNodeType() == 53)
			{
				this.WriteSpace();
				this.WriteToken(":");
				this.WriteSpace();
				this.Visit(baseConstructorInvokation);
			}
			return;
		}

		protected override void WriteBaseTypeInheritColon()
		{
			this.WriteSpace();
			this.WriteToken(":");
			this.WriteSpace();
			return;
		}

		protected override void WriteBeginBlock(bool inline = false)
		{
			if (!inline)
			{
				this.WriteLine();
			}
			else
			{
				this.WriteSpace();
			}
			this.WriteToken("{");
			return;
		}

		protected override void WriteBlock(Action action, string label)
		{
			this.WriteToken("{");
			this.WriteLine();
			if (String.op_Inequality(label, ""))
			{
				this.WriteLabel(label);
			}
			this.Indent();
			action.Invoke();
			this.Outdent();
			this.WriteToken("}");
			return;
		}

		private void WriteCommentBlock(string commentedText)
		{
			this.StartWritingComment(true);
			this.Write(commentedText);
			this.EndWritingComment(true);
			return;
		}

		private void WriteConstraints(GenericParameter genericParameter)
		{
			if (!this.IsTypeParameterRedeclaration(genericParameter))
			{
				this.WriteLine();
				this.DoWriteGenericConstraints(genericParameter);
				return;
			}
			if (this.ShouldWriteConstraintsAsComment(genericParameter))
			{
				this.WriteLine();
				this.StartWritingComment();
				this.DoWriteGenericConstraints(genericParameter);
				this.EndWritingComment();
			}
			return;
		}

		protected override void WriteConstructorGenericConstraint()
		{
			this.WriteConstructorGenericConstraint();
			this.WriteToken("(");
			this.WriteToken(")");
			return;
		}

		protected override void WriteDestructor(MethodDefinition method)
		{
			this.membersStack.Push(method);
			V_0 = false;
			try
			{
				V_1 = String.Concat("~", this.GetTypeName(method.get_DeclaringType()));
				this.WriteReference(V_1, method);
				this.WriteToken("(");
				this.WriteToken(")");
				V_2 = this.formatter.get_CurrentPosition();
				this.formatter.WriteStartBlock();
				V_0 = true;
				this.WriteLine();
				this.Visit(this.get_MethodContext().get_DestructorStatements());
				this.currentWritingInfo.get_MemberDefinitionToFoldingPositionMap().set_Item(method, new OffsetSpan(V_2, this.formatter.get_CurrentPosition() - 1));
				this.formatter.WriteEndBlock();
			}
			catch (Exception exception_0)
			{
				dummyVar0 = exception_0;
				if (V_0)
				{
					this.formatter.WriteEndBlock();
				}
				dummyVar1 = this.membersStack.Pop();
				throw;
			}
			dummyVar2 = this.membersStack.Pop();
			return;
		}

		private void WriteDynamicType(TypeReference typeRef, CustomAttribute dynamicAttribute)
		{
			this.WriteDynamicType(typeRef, DynamicHelper.GetDynamicPositioningFlags(dynamicAttribute));
			return;
		}

		private void WriteDynamicType(TypeReference typeRef, bool[] positioningFlags)
		{
			this.WriteDynamicType(typeRef, positioningFlags.GetEnumerator());
			return;
		}

		private void WriteDynamicType(TypeReference typeRef, IEnumerator positioningFlagsEnumerator)
		{
			this.isDynamicEnumerator = positioningFlagsEnumerator;
			try
			{
				this.WriteReferenceAndNamespaceIfInCollision(typeRef);
				if (this.isDynamicEnumerator.MoveNext())
				{
					throw new Exception("Invalid number of flags for DynamicAttribute.");
				}
			}
			finally
			{
				this.isDynamicEnumerator = null;
			}
			return;
		}

		protected override void WriteEndBlock(string statementName)
		{
			this.WriteToken("}");
			return;
		}

		protected override void WriteEndOfStatement()
		{
			this.WriteToken(";");
			return;
		}

		protected override void WriteEnumValueSeparator()
		{
			this.WriteToken(",");
			return;
		}

		protected override void WriteEscapeCharLiteral(char c)
		{
			V_0 = null;
			if (c > ' ')
			{
				if (c == '\"')
				{
					V_0 = "\\\"";
				}
				else
				{
					if (c == '\'')
					{
						V_0 = "\\'";
					}
					else
					{
						if (c == '\\')
						{
							V_0 = "\\\\";
						}
					}
				}
			}
			else
			{
				switch (c)
				{
					case 0:
					{
						V_0 = "\\0";
						break;
					}
					case 1:
					case 2:
					case 3:
					case 4:
					case 5:
					case 6:
					{
						break;
					}
					case 7:
					{
						V_0 = "\\a";
						break;
					}
					case 8:
					{
						V_0 = "\\b";
						break;
					}
					case 9:
					{
						V_0 = "\\t";
						break;
					}
					case 10:
					{
						V_0 = "\\n";
						break;
					}
					case 11:
					{
						V_0 = "\\v";
						break;
					}
					case 12:
					{
						V_0 = "\\f";
						break;
					}
					case 13:
					{
						V_0 = "\\r";
						break;
					}
					default:
					{
						if (c == ' ')
						{
							V_0 = " ";
							break;
						}
						else
						{
							break;
						}
					}
				}
			}
			if (V_0 != null)
			{
				this.WriteLiteral(this.get_CharStart());
				this.formatter.WriteLiteral(V_0);
				this.WriteLiteral(this.get_CharEnd());
				return;
			}
			V_1 = Char.GetUnicodeCategory(c);
			V_2 = false;
			if (V_1 == 21 || V_1 == 26 || V_1 == 19 || V_1 == 8 || V_1 == 23 || V_1 == 22 || V_1 == 9 || V_1 == 1 || V_1 == 25 || V_1 == 20 || V_1 == 2 || V_1 == UnicodeCategory.UppercaseLetter || V_1 == 24)
			{
				V_2 = true;
			}
			if (V_2)
			{
				V_0 = c.ToString();
			}
			else
			{
				V_0 = String.Format("\\u{0:X4}", (Int32)c);
			}
			this.WriteLiteral(this.get_CharStart());
			this.formatter.WriteLiteral(V_0);
			this.WriteLiteral(this.get_CharEnd());
			return;
		}

		protected override void WriteFieldName(FieldReference field)
		{
			this.WriteReference(this.GetFieldName(field), field);
			return;
		}

		protected override void WriteFieldTypeAndName(FieldDefinition field)
		{
			if (field.get_IsUnsafe())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Unsafe());
				this.WriteSpace();
			}
			V_0 = field.get_FieldType();
			if (V_0.get_IsRequiredModifier() && String.op_Equality((V_0 as RequiredModifierType).get_ModifierType().get_FullName(), "System.Runtime.CompilerServices.IsVolatile"))
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Volatile());
				this.WriteSpace();
				V_0 = (V_0 as RequiredModifierType).get_ElementType();
			}
			V_1 = this.GetFieldName(field);
			if (!field.TryGetDynamicAttribute(out V_2))
			{
				this.WriteTypeAndName(V_0, V_1, field);
				return;
			}
			this.WriteDynamicType(V_0, V_2);
			this.WriteSpace();
			V_3 = this.formatter.get_CurrentPosition();
			this.WriteReference(V_1, field);
			V_4 = this.formatter.get_CurrentPosition() - 1;
			this.currentWritingInfo.get_MemberDeclarationToCodePostionMap().set_Item(field, new OffsetSpan(V_3, V_4));
			return;
		}

		protected override void WriteFire(EventDefinition event)
		{
			return;
		}

		protected override void WriteGenericInstanceMethod(GenericInstanceMethod genericMethod)
		{
			V_0 = genericMethod.get_ElementMethod();
			V_1 = this.GetMethodName(V_0);
			if (!this.HasRefOrOutParameter(V_0))
			{
				this.WriteReference(V_1, genericMethod);
			}
			else
			{
				V_3 = genericMethod.get_ElementMethod().Resolve();
				if (V_3 != null)
				{
					this.WriteReference(V_1, V_3);
				}
				else
				{
					this.WriteNotResolvedReference(V_1, genericMethod, "Out parameters might be shown as ref. Please, locate the assembly where the method is defined.");
				}
			}
			if (genericMethod.HasAnonymousArgument())
			{
				return;
			}
			this.WriteToken(this.get_GenericLeftBracket());
			V_2 = genericMethod.get_GenericArguments();
			V_4 = 0;
			while (V_4 < V_2.get_Count())
			{
				if (V_4 > 0)
				{
					this.WriteToken(",");
					this.WriteSpace();
				}
				this.WriteReferenceAndNamespaceIfInCollision(V_2.get_Item(V_4));
				V_4 = V_4 + 1;
			}
			this.WriteToken(this.get_GenericRightBracket());
			return;
		}

		protected override void WriteIndexerKeywords()
		{
			return;
		}

		protected override void WriteInterfacesInheritColon(TypeDefinition type)
		{
			this.WriteBaseTypeInheritColon();
			return;
		}

		public override void WriteMemberNavigationPathFullName(object member)
		{
			if (member as ParameterReference != null)
			{
				this.formatter.Write(((ParameterReference)member).get_Name());
				return;
			}
			if (member as TypeReference != null)
			{
				this.formatter.Write(((TypeReference)member).GetFriendlyTypeName(this.get_Language(), "<", ">"));
				return;
			}
			if (member as MemberReference != null)
			{
				this.formatter.Write(((MemberReference)member).GetFriendlyFullName(this.get_Language()));
			}
			return;
		}

		protected override void WriteMethodReference(string name, MethodReference reference)
		{
			if (String.op_Equality(name, "this") || String.op_Equality(name, "base"))
			{
				this.WriteMethodReference(name, reference);
				return;
			}
			if (String.op_Equality(reference.get_Name(), ".ctor") || String.op_Equality(reference.get_Name(), ".cctor") && String.op_Inequality(name, "this") && String.op_Inequality(name, "base"))
			{
				name = Utilities.EscapeTypeNameIfNeeded(name, this.get_Language());
			}
			else
			{
				name = Utilities.EscapeNameIfNeeded(name, this.get_Language());
			}
			this.WriteMethodReference(name, reference);
			return;
		}

		protected override void WriteMethodReference(MethodReferenceExpression methodReferenceExpression)
		{
			V_0 = methodReferenceExpression.get_Method();
			V_1 = this.GetMethodName(V_0);
			if (methodReferenceExpression.get_Method() as GenericInstanceMethod != null)
			{
				this.WriteGenericInstanceMethod(V_0 as GenericInstanceMethod);
				return;
			}
			if (!this.HasRefOrOutParameter(methodReferenceExpression.get_Method()) || V_0.Resolve() != null)
			{
				this.WriteReference(V_1, V_0);
				return;
			}
			this.WriteNotResolvedReference(V_1, V_0, "Out parameters might be shown as ref. Please, locate the assembly where the method is defined.");
			return;
		}

		protected override void WriteMethodReturnType(MethodDefinition method)
		{
			if (method.get_MethodReturnType().TryGetDynamicAttribute(out V_0))
			{
				this.WriteDynamicType(method.get_ReturnType(), V_0);
				return;
			}
			if (!method.get_ReturnType().get_IsByReference())
			{
				this.WriteMethodReturnType(method);
				return;
			}
			this.WriteKeyword(this.get_KeyWordWriter().get_ByRef());
			this.WriteSpace();
			this.WriteReferenceAndNamespaceIfInCollision(method.get_ReturnType().GetElementType());
			return;
		}

		protected override bool WriteMethodVisibility(MethodDefinition method)
		{
			if (method.get_HasOverrides() && method.get_IsPrivate())
			{
				return false;
			}
			return this.WriteMethodVisibility(method);
		}

		private void WriteModifier(TypeSpecification typeSpecification)
		{
			V_0 = new StringBuilder();
			V_1 = typeSpecification;
			V_2 = typeSpecification as IModifierType;
			V_3 = false;
			while (V_2 != null)
			{
				if (!String.op_Equality(V_2.get_ModifierType().get_FullName(), "System.Runtime.CompilerServices.IsVolatile"))
				{
					if (V_3)
					{
						dummyVar0 = V_0.Append(", ");
					}
					stackVariable12 = V_0;
					if (V_2 as RequiredModifierType != null)
					{
						stackVariable15 = "modreq";
					}
					else
					{
						stackVariable15 = "modopt";
					}
					dummyVar1 = stackVariable12.Append(stackVariable15);
					dummyVar2 = V_0.Append("(");
					dummyVar3 = V_0.Append(V_2.get_ModifierType().get_FullName());
					dummyVar4 = V_0.Append(")");
					V_3 = true;
					V_1 = V_2.get_ElementType();
					V_2 = V_1 as IModifierType;
				}
				else
				{
					V_1 = V_2.get_ElementType();
					V_2 = V_1 as IModifierType;
				}
			}
			if (V_3)
			{
				this.WriteCommentBlock(V_0.ToString());
				this.WriteSpace();
			}
			this.WriteReferenceAndNamespaceIfInCollision(V_1);
			return;
		}

		protected override void WriteNestedTypeWriteLine()
		{
			this.WriteLine();
			return;
		}

		protected override bool WritePropertyAsIndexer(PropertyDefinition property)
		{
			V_0 = this.get_KeyWordWriter().get_This();
			if (property.IsExplicitImplementation())
			{
				V_1 = property.get_Name().LastIndexOf(".");
				V_0 = property.get_Name().Replace(property.get_Name().Substring(V_1 + 1), this.get_KeyWordWriter().get_This());
			}
			this.WriteTypeAndName(property.get_PropertyType(), V_0, property);
			this.Write(this.get_IndexLeftBracket());
			this.WritePropertyParameters(property);
			this.Write(this.get_IndexRightBracket());
			return true;
		}

		protected override void WritePropertyName(PropertyDefinition property)
		{
			this.WriteReference(this.GetPropertyName(property), property);
			return;
		}

		protected override void WritePropertyTypeAndNameWithArguments(PropertyDefinition property)
		{
			V_0 = this.GetPropertyName(property);
			if (property.TryGetDynamicAttribute(out V_1))
			{
				this.WriteDynamicType(property.get_PropertyType(), V_1);
				this.WriteSpace();
				this.WriteReference(V_0, property);
				return;
			}
			this.WriteTypeAndName(property.get_PropertyType(), V_0, property);
			if (this.HasArguments(property))
			{
				this.WriteToken("(");
				this.WritePropertyParameters(property);
				this.WriteToken(")");
			}
			return;
		}

		internal override void WriteReference(string name, object reference)
		{
			if (this.isDynamicEnumerator != null)
			{
				if (!this.isDynamicEnumerator.MoveNext())
				{
					throw new Exception("Invalid argument type for DynamicAttribute");
				}
				if ((Boolean)this.isDynamicEnumerator.get_Current())
				{
					this.WriteKeyword(this.get_KeyWordWriter().get_Dynamic());
					return;
				}
			}
			this.WriteReference(name, reference);
			return;
		}

		protected override void WriteRightPartOfBinaryExpression(BinaryExpression binaryExpression)
		{
			if (binaryExpression.get_IsAssignmentExpression() && binaryExpression.get_Left().get_CodeNodeType() == 93)
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_ByRef());
				this.WriteSpace();
				if (binaryExpression.get_Right().get_CodeNodeType() == 23)
				{
					V_0 = binaryExpression.get_Right() as UnaryExpression;
					if (V_0.get_Operator() == 7)
					{
						this.Visit(V_0.get_Operand());
						return;
					}
				}
			}
			this.Visit(binaryExpression.get_Right());
			return;
		}

		protected override void WriteSpecialBetweenParenthesis(Expression expression)
		{
			this.WriteToken("(");
			this.Visit(expression);
			this.WriteToken(")");
			return;
		}

		protected override void WriteSpecialBetweenParenthesis(Action action)
		{
			this.WriteToken("(");
			action.Invoke();
			this.WriteToken(")");
			return;
		}

		protected override void WriteTypeInterfaces(TypeDefinition type, bool isPartial, bool baseTypeWritten)
		{
			if ((object)this.get_TypeContext().get_CurrentType() != (object)type || !this.get_TypeContext().get_IsWinRTImplementation())
			{
				V_0 = type.get_Interfaces().ToList<TypeReference>();
			}
			else
			{
				stackVariable45 = type.get_Interfaces();
				stackVariable46 = CSharpWriter.u003cu003ec.u003cu003e9__14_0;
				if (stackVariable46 == null)
				{
					dummyVar0 = stackVariable46;
					stackVariable46 = new Func<TypeReference, bool>(CSharpWriter.u003cu003ec.u003cu003e9.u003cWriteTypeInterfacesu003eb__14_0);
					CSharpWriter.u003cu003ec.u003cu003e9__14_0 = stackVariable46;
				}
				V_0 = stackVariable45.Where<TypeReference>(stackVariable46).ToList<TypeReference>();
			}
			if (V_0.get_Count() > 0)
			{
				V_1 = 0;
				V_2 = 0;
				while (V_2 < V_0.get_Count())
				{
					if (!isPartial || this.IsImplemented(type, V_0.get_Item(V_2).Resolve()))
					{
						if (V_1 == 0)
						{
							if (!baseTypeWritten)
							{
								this.WriteInterfacesInheritColon(type);
							}
							else
							{
								this.WriteInheritComma();
							}
						}
						if (V_1 > 0)
						{
							this.WriteInheritComma();
						}
						V_1 = V_1 + 1;
						this.WriteReferenceAndNamespaceIfInCollision(V_0.get_Item(V_2));
					}
					V_2 = V_2 + 1;
				}
			}
			return;
		}

		protected override void WriteTypeSpecification(TypeSpecification typeSpecification, int startingArgument = 0)
		{
			if (this.isDynamicEnumerator != null && !typeSpecification.get_IsGenericInstance())
			{
				if (!typeSpecification.get_IsArray())
				{
					if (!this.isDynamicEnumerator.MoveNext() || (Boolean)this.isDynamicEnumerator.get_Current())
					{
						throw new Exception("Invalid argument type for DynamicAttribute");
					}
				}
				else
				{
					V_0 = typeSpecification;
					while (V_0.get_IsArray())
					{
						if (!this.isDynamicEnumerator.MoveNext() || (Boolean)this.isDynamicEnumerator.get_Current())
						{
							throw new Exception("Invalid argument type for DynamicAttribute");
						}
						V_0 = (V_0 as ArrayType).get_ElementType();
					}
				}
			}
			if (typeSpecification.get_IsRequiredModifier() || typeSpecification.get_IsOptionalModifier())
			{
				this.WriteModifier(typeSpecification);
				return;
			}
			this.WriteTypeSpecification(typeSpecification, startingArgument);
			return;
		}

		private void WriteVariableName(VariableDefinition variable)
		{
			V_0 = new CSharpWriter.u003cu003ec__DisplayClass22_0();
			V_0.u003cu003e4__this = this;
			V_0.variable = variable;
			this.WriteAndMapVariableToCode(new Action(V_0.u003cWriteVariableNameu003eb__0), V_0.variable);
			return;
		}
	}
}