using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler;
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

		public CSharpWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings) : base(language, formatter, exceptionFormatter, settings)
		{
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
			base.WriteTokenBetweenSpace(":");
			base.WriteSingleGenericParameterConstraintsList(genericParameter);
		}

		protected override void DoWriteParameterTypeAndName(TypeReference type, string name, ParameterDefinition reference)
		{
			CustomAttribute customAttribute;
			string str = name;
			if (!base.Language.IsValidIdentifier(str))
			{
				str = base.Language.ReplaceInvalidCharactersInIdentifier(str);
			}
			if (reference.IsParamArray())
			{
				this.WriteKeyword(base.KeyWordWriter.ParamArray);
				this.WriteSpace();
			}
			if (reference.TryGetDynamicAttribute(out customAttribute))
			{
				IEnumerator enumerator = DynamicHelper.GetDynamicPositioningFlags(customAttribute).GetEnumerator();
				if (reference.ParameterType.IsByReference && (reference.ParameterType as ByReferenceType).ElementType == type && (!enumerator.MoveNext() || (Boolean)enumerator.Current))
				{
					throw new Exception("Invalid argument type for DynamicAttribute");
				}
				this.WriteDynamicType(type, enumerator);
				this.WriteSpace();
			}
			else if (!String.IsNullOrEmpty(this.ToTypeString(type)))
			{
				this.WriteReferenceAndNamespaceIfInCollision(type);
				this.WriteSpace();
			}
			if (base.Language.IsGlobalKeyword(str))
			{
				str = Utilities.EscapeNameIfNeeded(str, base.Language);
			}
			base.WriteAndMapParameterToCode(() => this.Write(str), reference.Index);
		}

		protected override void DoWriteTypeAndName(TypeReference typeReference, string name, object reference)
		{
			this.WriteReferenceAndNamespaceIfInCollision(typeReference);
			this.WriteSpace();
			int currentPosition = this.formatter.CurrentPosition;
			this.WriteReference(name, reference);
			if (reference is IMemberDefinition)
			{
				int num = this.formatter.CurrentPosition - 1;
				this.currentWritingInfo.MemberDeclarationToCodePostionMap[(IMemberDefinition)reference] = new OffsetSpan(currentPosition, num);
			}
		}

		protected override void DoWriteTypeAndName(TypeReference typeReference, string name)
		{
			this.WriteReferenceAndNamespaceIfInCollision(typeReference);
			this.WriteSpace();
			this.Write(name);
		}

		protected override void DoWriteVariableTypeAndName(VariableDefinition variable)
		{
			this.WriteReferenceAndNamespaceIfInCollision(variable.VariableType);
			this.WriteSpace();
			this.WriteVariableName(variable);
		}

		protected override void EndWritingComment()
		{
			this.EndWritingComment(false);
		}

		private void EndWritingComment(bool isBlock)
		{
			this.isWritingComment = false;
			if (isBlock)
			{
				this.formatter.WriteComment(" */");
			}
			base.EndWritingComment();
		}

		private string GetGenericNameFromMemberReference(TypeReference type)
		{
			IGenericDefinition genericDefinition = type.Resolve();
			if (genericDefinition != null)
			{
				return genericDefinition.GetGenericName(base.Language, "<", ">");
			}
			return type.GetGenericName(base.Language, "<", ">");
		}

		private bool HasRefOrOutParameter(MethodReference method)
		{
			bool flag;
			Mono.Collections.Generic.Collection<ParameterDefinition>.Enumerator enumerator = method.Parameters.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.ParameterType.IsByReference)
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		protected override bool IsComplexTarget(Expression target)
		{
			if (target.CodeNodeType == CodeNodeType.UnaryExpression)
			{
				UnaryExpression unaryExpression = target as UnaryExpression;
				if (unaryExpression.Operator == UnaryOperator.AddressDereference && unaryExpression.Operand.CodeNodeType == CodeNodeType.VariableReferenceExpression && (unaryExpression.Operand as VariableReferenceExpression).IsByReference)
				{
					return false;
				}
			}
			if (base.IsComplexTarget(target) || target.CodeNodeType == CodeNodeType.DynamicConstructorInvocationExpression || target.CodeNodeType == CodeNodeType.LinqQueryExpression || target.CodeNodeType == CodeNodeType.ConditionExpression || target.CodeNodeType == CodeNodeType.ExplicitCastExpression)
			{
				return true;
			}
			return target.CodeNodeType == CodeNodeType.SafeCastExpression;
		}

		protected override void PostWriteGenericParametersConstraints(IGenericDefinition genericDefinition)
		{
			if (genericDefinition.HasGenericParameters)
			{
				foreach (GenericParameter genericParameter in genericDefinition.GenericParameters)
				{
					if (!genericParameter.HasNotNullableValueTypeConstraint && !genericParameter.HasDefaultConstructorConstraint && !genericParameter.HasConstraints && !genericParameter.HasReferenceTypeConstraint)
					{
						continue;
					}
					this.WriteConstraints(genericParameter);
				}
			}
		}

		protected override void StartWritingComment()
		{
			this.StartWritingComment(false);
		}

		private void StartWritingComment(bool isBlock)
		{
			this.isWritingComment = true;
			if (!isBlock)
			{
				this.formatter.WriteComment(String.Concat(base.Language.CommentLineSymbol, " "));
			}
			else
			{
				this.formatter.WriteComment("/* ");
			}
			base.StartWritingComment();
		}

		internal override string ToEscapedTypeString(TypeReference reference)
		{
			return this.ToTypeString(reference);
		}

		protected override string ToString(BinaryOperator op, bool isOneSideNull = false)
		{
			switch (op)
			{
				case BinaryOperator.Add:
				{
					return "+";
				}
				case BinaryOperator.AddAssign:
				{
					return "+=";
				}
				case BinaryOperator.Subtract:
				{
					return "-";
				}
				case BinaryOperator.SubtractAssign:
				{
					return "-=";
				}
				case BinaryOperator.Multiply:
				{
					return "*";
				}
				case BinaryOperator.MultiplyAssign:
				{
					return "*=";
				}
				case BinaryOperator.Divide:
				{
					return "/";
				}
				case BinaryOperator.DivideAssign:
				{
					return "/=";
				}
				case BinaryOperator.ValueEquality:
				{
					return "==";
				}
				case BinaryOperator.ValueInequality:
				{
					return "!=";
				}
				case BinaryOperator.LogicalOr:
				{
					return "||";
				}
				case BinaryOperator.LogicalAnd:
				{
					return "&&";
				}
				case BinaryOperator.LessThan:
				{
					return "<";
				}
				case BinaryOperator.LessThanOrEqual:
				{
					return "<=";
				}
				case BinaryOperator.GreaterThan:
				{
					return ">";
				}
				case BinaryOperator.GreaterThanOrEqual:
				{
					return ">=";
				}
				case BinaryOperator.LeftShift:
				{
					return "<<";
				}
				case BinaryOperator.LeftShiftAssign:
				{
					return "<<=";
				}
				case BinaryOperator.RightShift:
				{
					return ">>";
				}
				case BinaryOperator.RightShiftAssign:
				{
					return ">>=";
				}
				case BinaryOperator.BitwiseOr:
				{
					return "|";
				}
				case BinaryOperator.BitwiseAnd:
				{
					return "&";
				}
				case BinaryOperator.BitwiseXor:
				{
					return "^";
				}
				case BinaryOperator.Modulo:
				{
					return "%";
				}
				case BinaryOperator.ModuloAssign:
				{
					return "%=";
				}
				case BinaryOperator.Assign:
				{
					return "=";
				}
				case BinaryOperator.NullCoalesce:
				{
					return "??";
				}
				case BinaryOperator.AndAssign:
				{
					return "&=";
				}
				case BinaryOperator.OrAssign:
				{
					return "|=";
				}
				case BinaryOperator.XorAssign:
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
				case UnaryOperator.Negate:
				{
					return "-";
				}
				case UnaryOperator.LogicalNot:
				{
					return "!";
				}
				case UnaryOperator.BitwiseNot:
				{
					return "~";
				}
				case UnaryOperator.PostDecrement:
				case UnaryOperator.PreDecrement:
				{
					return "--";
				}
				case UnaryOperator.PostIncrement:
				case UnaryOperator.PreIncrement:
				{
					return "++";
				}
				case UnaryOperator.AddressReference:
				case UnaryOperator.AddressOf:
				{
					return "&";
				}
				case UnaryOperator.AddressDereference:
				{
					return "*";
				}
				case UnaryOperator.UnaryPlus:
				{
					return "+";
				}
				case UnaryOperator.None:
				{
					return String.Empty;
				}
			}
			throw new ArgumentException();
		}

		public override string ToTypeString(TypeReference type)
		{
			if (!base.IsReferenceFromMscorlib(type))
			{
				return this.GetGenericNameFromMemberReference(type);
			}
			string name = type.Name;
			if (name != null)
			{
				if (name == "Decimal")
				{
					return "decimal";
				}
				if (name == "Single")
				{
					return "float";
				}
				if (name == "Byte")
				{
					return "byte";
				}
				if (name == "SByte")
				{
					return "sbyte";
				}
				if (name == "Char")
				{
					return "char";
				}
				if (name == "Double")
				{
					return "double";
				}
				if (name == "Boolean")
				{
					return "bool";
				}
				if (name == "Int16")
				{
					return "short";
				}
				if (name == "Int32")
				{
					return "int";
				}
				if (name == "Int64")
				{
					return "long";
				}
				if (name == "UInt16")
				{
					return "ushort";
				}
				if (name == "UInt32")
				{
					return "uint";
				}
				if (name == "UInt64")
				{
					return "ulong";
				}
				if (name == "String")
				{
					return "string";
				}
				if (name == "Void")
				{
					return "void";
				}
				if (name == "Object")
				{
					return "object";
				}
				if (name == "RuntimeArgumentHandle")
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
			if (base.MethodReferences.Count == 0)
			{
				this.WriteToken(base.KeyWordWriter.AddressOf);
			}
			this.Visit(node.Operand);
		}

		public override void VisitBaseCtorExpression(BaseCtorExpression node)
		{
			this.VisitCtorExpression(node, "base");
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			if (node.Operator == BinaryOperator.NullCoalesce)
			{
				this.VisitNullCoalesceExpression(node);
				return;
			}
			if (!node.IsChecked)
			{
				base.VisitBinaryExpression(node);
				return;
			}
			this.WriteKeyword(base.KeyWordWriter.Checked);
			this.Write("(");
			base.VisitBinaryExpression(node);
			this.Write(")");
		}

		public override void VisitBoxExpression(BoxExpression node)
		{
			if (!node.IsAutoBox)
			{
				this.WriteToken("(");
				this.WriteReferenceAndNamespaceIfInCollision(node.ExpressionType);
				this.WriteToken(")");
			}
			bool flag = this.IsComplexTarget(node.BoxedExpression);
			if (flag && !node.IsAutoBox)
			{
				this.WriteToken("(");
			}
			this.Visit(node.BoxedExpression);
			if (flag && !node.IsAutoBox)
			{
				this.WriteToken(")");
			}
		}

		public override void VisitBreakStatement(BreakStatement node)
		{
			this.WriteKeyword("break");
			this.WriteEndOfStatement();
		}

		public override void VisitBreakSwitchCaseStatement(BreakSwitchCaseStatement node)
		{
			this.WriteKeyword("break");
			this.WriteEndOfStatement();
		}

		public override void VisitCaseGotoStatement(CaseGotoStatement node)
		{
			this.WriteKeyword(base.KeyWordWriter.GoTo);
			this.WriteSpace();
			SwitchCase targetedSwitchCase = node.TargetedSwitchCase;
			if (targetedSwitchCase.CodeNodeType != CodeNodeType.ConditionCase)
			{
				this.WriteKeyword(base.KeyWordWriter.Default);
			}
			else
			{
				this.WriteKeyword(base.KeyWordWriter.Case);
				this.WriteSpace();
				this.Visit((targetedSwitchCase as ConditionCase).Condition);
			}
			this.WriteEndOfStatement();
		}

		public override void VisitCatchClause(CatchClause node)
		{
			this.WriteKeyword(base.KeyWordWriter.Catch);
			if (node.Type == null && !base.Language.SupportsExceptionFilters)
			{
				throw new Exception(String.Format("Exception filters are not supported in {0}.", base.Language.Name));
			}
			if (node.Type.FullName != "System.Object")
			{
				this.WriteSpace();
				this.WriteSpecialBetweenParenthesis(() => {
					if (node.Variable != null)
					{
						this.Visit(node.Variable);
						return;
					}
					this.WriteReferenceAndNamespaceIfInCollision(node.Type);
				});
			}
			if (node.Filter != null && base.Language.SupportsExceptionFilters)
			{
				this.WriteSpace();
				this.WriteKeyword(base.KeyWordWriter.When);
				this.WriteSpace();
				this.WriteSpecialBetweenParenthesis((node.Filter as ExpressionStatement).Expression);
			}
			this.WriteLine();
			this.Visit(node.Body);
		}

		public override void VisitConditionCase(ConditionCase node)
		{
			this.WriteKeyword(base.KeyWordWriter.Case);
			this.WriteSpace();
			this.Visit(node.Condition);
			this.WriteToken(":");
			if (node.Body != null)
			{
				this.WriteLine();
				this.Visit(node.Body);
			}
		}

		public override void VisitConditionExpression(ConditionExpression node)
		{
			this.WriteToken("(");
			this.Visit(node.Condition);
			base.WriteTokenBetweenSpace("?");
			this.Visit(node.Then);
			base.WriteTokenBetweenSpace(":");
			this.Visit(node.Else);
			this.WriteToken(")");
		}

		public override void VisitContinueStatement(ContinueStatement node)
		{
			this.WriteKeyword("continue");
			this.WriteToken(";");
		}

		private void VisitCtorExpression(MethodInvocationExpression node, string ctorKeyword)
		{
			if (node.MethodExpression.CodeNodeType != CodeNodeType.MethodReferenceExpression)
			{
				this.WriteKeyword(ctorKeyword);
			}
			else
			{
				this.WriteReference(ctorKeyword, node.MethodExpression.Method);
			}
			this.WriteToken("(");
			base.EnterMethodInvocation(node.MethodExpression.Method);
			base.VisitMethodParameters(node.Arguments);
			base.LeaveMethodInvocation();
			this.WriteToken(")");
		}

		public override void VisitDefaultCase(DefaultCase node)
		{
			this.WriteKeyword(base.KeyWordWriter.Default);
			this.WriteToken(":");
			this.WriteLine();
			this.Visit(node.Body);
		}

		public override void VisitDefaultObjectExpression(DefaultObjectExpression node)
		{
			this.WriteKeyword("default");
			this.WriteToken("(");
			this.WriteReferenceAndNamespaceIfInCollision(node.Type);
			this.WriteToken(")");
		}

		public override void VisitDynamicConstructorInvocationExpression(DynamicConstructorInvocationExpression node)
		{
			this.WriteKeyword(base.KeyWordWriter.New);
			this.WriteSpace();
			this.WriteReferenceAndNamespaceIfInCollision(node.ConstructorType);
			this.WriteToken("(");
			for (int i = 0; i < node.Arguments.Count; i++)
			{
				if (i != 0)
				{
					this.WriteToken(",");
					this.WriteSpace();
				}
				this.Visit(node.Arguments[i]);
			}
			this.WriteToken(")");
		}

		public override void VisitDynamicMemberReferenceExpression(DynamicMemberReferenceExpression node)
		{
			bool flag = this.IsComplexTarget(node.Target);
			if (flag)
			{
				this.WriteToken("(");
			}
			this.Visit(node.Target);
			if (flag)
			{
				this.WriteToken(")");
			}
			if (node.MemberName != null)
			{
				this.WriteToken(".");
				this.Write(node.MemberName);
			}
			if (node.IsMethodInvocation)
			{
				if (node.IsGenericMethod)
				{
					this.WriteToken(this.GenericLeftBracket);
					for (int i = 0; i < node.GenericTypeArguments.Count; i++)
					{
						if (i != 0)
						{
							this.WriteToken(",");
							this.WriteSpace();
						}
						this.WriteReferenceAndNamespaceIfInCollision(node.GenericTypeArguments[i]);
					}
					this.WriteToken(this.GenericRightBracket);
				}
				this.WriteToken("(");
				for (int j = 0; j < node.InvocationArguments.Count; j++)
				{
					if (j != 0)
					{
						this.WriteToken(",");
						this.WriteSpace();
					}
					this.Visit(node.InvocationArguments[j]);
				}
				this.WriteToken(")");
			}
		}

		public override void VisitExplicitCastExpression(ExplicitCastExpression node)
		{
			TypeReference typeReference;
			if (node.IsChecked)
			{
				this.WriteKeyword(base.KeyWordWriter.Checked);
				this.WriteToken("(");
			}
			this.WriteToken("(");
			if (node.IsDynamic)
			{
				this.WriteDynamicType(node.TargetType, node.DynamicPositioningFlags);
			}
			else if (node.UnresolvedReferenceForAmbiguousCastToObject != null)
			{
				Telerik.JustDecompiler.Common.Extensions.ResolveToOverloadedEqualityOperator(node.Expression.ExpressionType, out typeReference);
				this.WriteNotResolvedReference("object", typeReference, String.Format("The cast to object might be unnecessary. Please, locate the assembly where \"{0}\" is defined.", typeReference.Name));
			}
			else
			{
				this.WriteReferenceAndNamespaceIfInCollision(node.TargetType);
			}
			this.WriteToken(")");
			bool flag = this.IsComplexTarget(node.Expression);
			if (flag)
			{
				this.WriteToken("(");
			}
			this.Visit(node.Expression);
			if (flag)
			{
				this.WriteToken(")");
			}
			if (node.IsChecked)
			{
				this.WriteToken(")");
			}
		}

		public override void VisitForEachStatement(ForEachStatement node)
		{
			this.WriteKeyword(base.KeyWordWriter.ForEach);
			this.WriteSpace();
			this.WriteToken("(");
			this.Visit(node.Variable);
			this.WriteSpace();
			this.WriteKeyword(base.KeyWordWriter.In);
			this.WriteSpace();
			this.Visit(node.Collection);
			this.WriteToken(")");
			this.WriteLine();
			this.Visit(node.Body);
		}

		public override void VisitForStatement(ForStatement node)
		{
			this.WriteKeyword("for");
			this.WriteSpace();
			this.WriteToken("(");
			this.Visit(node.Initializer);
			this.WriteToken(";");
			this.WriteSpace();
			this.Visit(node.Condition);
			this.WriteToken(";");
			this.WriteSpace();
			this.Visit(node.Increment);
			this.WriteToken(")");
			this.WriteLine();
			this.Visit(node.Body);
		}

		public override void VisitLambdaExpression(LambdaExpression node)
		{
			base.VisitLambdaExpression(node);
			this.WriteToken("(");
			base.VisitMethodParameters(node.Arguments);
			this.WriteToken(")");
			this.WriteSpace();
			this.WriteToken("=>");
			this.WriteSpace();
			if (node.Body.Statements.Count != 1)
			{
				base.ShouldOmitSemicolon.Push(false);
				this.Visit(node.Body);
			}
			else if (node.Body.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement)
			{
				base.ShouldOmitSemicolon.Push(false);
				this.Visit(node.Body);
			}
			else
			{
				base.ShouldOmitSemicolon.Push(true);
				this.Visit(node.Body.Statements[0] as ExpressionStatement);
			}
			base.ShouldOmitSemicolon.Pop();
		}

		public override void VisitLinqQueryExpression(LinqQueryExpression node)
		{
			this.Indent();
			foreach (QueryClause clause in node.Clauses)
			{
				if (clause.CodeNodeType == CodeNodeType.IntoClause)
				{
					this.WriteSpace();
				}
				else
				{
					this.WriteLine();
				}
				this.Visit(clause);
			}
			this.Outdent();
		}

		private void VisitNullCoalesceExpression(BinaryExpression node)
		{
			this.Visit(node.Left);
			base.WriteTokenBetweenSpace("??");
			this.Visit(node.Right);
		}

		public override void VisitRefReturnExpression(RefReturnExpression node)
		{
			this.WriteKeyword(base.KeyWordWriter.Return);
			this.WriteSpace();
			this.WriteKeyword(base.KeyWordWriter.ByRef);
			this.WriteSpace();
			if (node.Value.CodeNodeType != CodeNodeType.UnaryExpression || (node.Value as UnaryExpression).Operator != UnaryOperator.AddressReference)
			{
				this.Visit(node.Value);
				return;
			}
			this.Visit((node.Value as UnaryExpression).Operand);
		}

		public override void VisitRefVariableDeclarationExpression(RefVariableDeclarationExpression node)
		{
			this.WriteKeyword(base.KeyWordWriter.ByRef);
			this.WriteSpace();
			this.WriteReferenceAndNamespaceIfInCollision(node.Variable.VariableType.GetElementType());
			this.WriteSpace();
			this.WriteVariableName(node.Variable);
		}

		public override void VisitSafeCastExpression(SafeCastExpression node)
		{
			this.Visit(node.Expression);
			this.WriteSpace();
			this.WriteKeyword(base.KeyWordWriter.TryCast);
			this.WriteSpace();
			this.WriteReferenceAndNamespaceIfInCollision(node.TargetType);
		}

		public override void VisitSwitchStatement(SwitchStatement node)
		{
			this.WriteKeyword(base.KeyWordWriter.Switch);
			this.WriteSpace();
			this.WriteToken("(");
			this.Visit(node.Condition);
			this.WriteToken(")");
			this.WriteLine();
			this.WriteBlock(() => {
				this.Visit(node.Cases);
				this.WriteLine();
			}, "");
		}

		public override void VisitThisCtorExpression(ThisCtorExpression node)
		{
			this.VisitCtorExpression(node, "this");
		}

		public override void VisitUnaryExpression(UnaryExpression node)
		{
			if (node.Operator == UnaryOperator.Negate || node.Operator == UnaryOperator.LogicalNot || node.Operator == UnaryOperator.BitwiseNot || node.Operator == UnaryOperator.UnaryPlus)
			{
				this.Write(this.ToString(node.Operator));
				if (node.Operand is SafeCastExpression || node.Operand is CanCastExpression)
				{
					this.WriteToken("(");
				}
				base.VisitUnaryExpression(node);
				if (node.Operand is SafeCastExpression || node.Operand is CanCastExpression)
				{
					this.WriteToken(")");
				}
				return;
			}
			if (node.Operator == UnaryOperator.AddressOf)
			{
				this.VisitAddressOfExpression(node);
				return;
			}
			if (node.Operator != UnaryOperator.AddressDereference)
			{
				bool flag = base.IsPostUnaryOperator(node.Operator);
				if (!flag)
				{
					this.Write(this.ToString(node.Operator));
				}
				this.Visit(node.Operand);
				if (flag)
				{
					this.Write(this.ToString(node.Operator));
				}
				return;
			}
			if (node.Operand.CodeNodeType == CodeNodeType.VariableReferenceExpression && (node.Operand as VariableReferenceExpression).IsByReference)
			{
				this.Visit(node.Operand);
				return;
			}
			if (node.Operand.CodeNodeType == CodeNodeType.ParenthesesExpression)
			{
				ParenthesesExpression operand = node.Operand as ParenthesesExpression;
				if (operand.Expression.CodeNodeType == CodeNodeType.MethodInvocationExpression && (operand.Expression as MethodInvocationExpression).IsByReference)
				{
					this.Visit(operand.Expression);
					return;
				}
			}
			base.VisitUnaryExpression(node);
		}

		public override void VisitUnsafeBlockStatement(UnsafeBlockStatement unsafeBlock)
		{
			this.WriteKeyword(base.KeyWordWriter.Unsafe);
			this.WriteLine();
			this.VisitBlockStatement(unsafeBlock);
		}

		public override void VisitUsingStatement(UsingStatement node)
		{
			this.WriteKeyword(base.KeyWordWriter.Using);
			this.WriteSpace();
			this.WriteSpecialBetweenParenthesis(node.Expression);
			this.WriteLine();
			this.Visit(node.Body);
			this.WriteSpecialEndBlock(base.KeyWordWriter.Using);
		}

		public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			if (base.Language.Version == 0)
			{
				base.VisitVariableDeclarationExpression(node);
				return;
			}
			string variableName = base.GetVariableName(node.Variable);
			if (node.Variable.VariableType.ContainsAnonymousType())
			{
				this.WriteKeyword(base.KeyWordWriter.Dim);
				this.WriteSpace();
				base.WriteAndMapVariableToCode(() => this.Write(variableName), node.Variable);
				return;
			}
			if (!node.Variable.Resolve().IsDynamic)
			{
				base.VisitVariableDeclarationExpression(node);
				return;
			}
			this.WriteDynamicType(node.Variable.VariableType, node.Variable.DynamicPositioningFlags);
			this.WriteSpace();
			base.WriteAndMapVariableToCode(() => this.Write(variableName), node.Variable);
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if (base.Language.Version != 0)
			{
				base.VisitVariableReferenceExpression(node);
				return;
			}
			this.Write(node.Variable.Name);
		}

		public override void VisitYieldBreakExpression(YieldBreakExpression node)
		{
			this.WriteKeyword("yield");
			this.WriteSpace();
			this.WriteKeyword("break");
		}

		public override void VisitYieldReturnExpression(YieldReturnExpression node)
		{
			this.WriteKeyword("yield");
			this.WriteSpace();
			this.WriteKeyword("return");
			this.WriteSpace();
			this.Visit(node.Expression);
		}

		protected override void WriteBaseConstructorInvokation(MethodInvocationExpression baseConstructorInvokation)
		{
			if (baseConstructorInvokation.Arguments.Count > 0 || baseConstructorInvokation.CodeNodeType == CodeNodeType.ThisCtorExpression)
			{
				this.WriteSpace();
				this.WriteToken(":");
				this.WriteSpace();
				this.Visit(baseConstructorInvokation);
			}
		}

		protected override void WriteBaseTypeInheritColon()
		{
			this.WriteSpace();
			this.WriteToken(":");
			this.WriteSpace();
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
		}

		protected override void WriteBlock(Action action, string label)
		{
			this.WriteToken("{");
			this.WriteLine();
			if (label != "")
			{
				base.WriteLabel(label);
			}
			this.Indent();
			action();
			this.Outdent();
			this.WriteToken("}");
		}

		private void WriteCommentBlock(string commentedText)
		{
			this.StartWritingComment(true);
			this.Write(commentedText);
			this.EndWritingComment(true);
		}

		private void WriteConstraints(GenericParameter genericParameter)
		{
			if (!base.IsTypeParameterRedeclaration(genericParameter))
			{
				this.WriteLine();
				this.DoWriteGenericConstraints(genericParameter);
				return;
			}
			if (base.ShouldWriteConstraintsAsComment(genericParameter))
			{
				this.WriteLine();
				this.StartWritingComment();
				this.DoWriteGenericConstraints(genericParameter);
				this.EndWritingComment();
			}
		}

		protected override void WriteConstructorGenericConstraint()
		{
			base.WriteConstructorGenericConstraint();
			this.WriteToken("(");
			this.WriteToken(")");
		}

		protected override void WriteDestructor(MethodDefinition method)
		{
			this.membersStack.Push(method);
			bool flag = false;
			try
			{
				string str = String.Concat("~", this.GetTypeName(method.DeclaringType));
				this.WriteReference(str, method);
				this.WriteToken("(");
				this.WriteToken(")");
				int currentPosition = this.formatter.CurrentPosition;
				this.formatter.WriteStartBlock();
				flag = true;
				this.WriteLine();
				this.Visit(base.MethodContext.DestructorStatements);
				this.currentWritingInfo.MemberDefinitionToFoldingPositionMap[method] = new OffsetSpan(currentPosition, this.formatter.CurrentPosition - 1);
				this.formatter.WriteEndBlock();
			}
			catch (Exception exception)
			{
				if (flag)
				{
					this.formatter.WriteEndBlock();
				}
				this.membersStack.Pop();
				throw;
			}
			this.membersStack.Pop();
		}

		private void WriteDynamicType(TypeReference typeRef, CustomAttribute dynamicAttribute)
		{
			this.WriteDynamicType(typeRef, DynamicHelper.GetDynamicPositioningFlags(dynamicAttribute));
		}

		private void WriteDynamicType(TypeReference typeRef, bool[] positioningFlags)
		{
			this.WriteDynamicType(typeRef, positioningFlags.GetEnumerator());
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
		}

		protected override void WriteEndBlock(string statementName)
		{
			this.WriteToken("}");
		}

		protected override void WriteEndOfStatement()
		{
			this.WriteToken(";");
		}

		protected override void WriteEnumValueSeparator()
		{
			this.WriteToken(",");
		}

		protected override void WriteEscapeCharLiteral(char c)
		{
			string str = null;
			if (c <= ' ')
			{
				switch (c)
				{
					case '\0':
					{
						str = "\\0";
						break;
					}
					case '\u0001':
					case '\u0002':
					case '\u0003':
					case '\u0004':
					case '\u0005':
					case '\u0006':
					{
						break;
					}
					case '\a':
					{
						str = "\\a";
						break;
					}
					case '\b':
					{
						str = "\\b";
						break;
					}
					case '\t':
					{
						str = "\\t";
						break;
					}
					case '\n':
					{
						str = "\\n";
						break;
					}
					case '\v':
					{
						str = "\\v";
						break;
					}
					case '\f':
					{
						str = "\\f";
						break;
					}
					case '\r':
					{
						str = "\\r";
						break;
					}
					default:
					{
						if (c == ' ')
						{
							str = " ";
							break;
						}
						else
						{
							break;
						}
					}
				}
			}
			else if (c == '\"')
			{
				str = "\\\"";
			}
			else if (c == '\'')
			{
				str = "\\'";
			}
			else if (c == '\\')
			{
				str = "\\\\";
			}
			if (str != null)
			{
				this.WriteLiteral(this.CharStart);
				this.formatter.WriteLiteral(str);
				this.WriteLiteral(this.CharEnd);
				return;
			}
			UnicodeCategory unicodeCategory = Char.GetUnicodeCategory(c);
			bool flag = false;
			if (unicodeCategory == UnicodeCategory.ClosePunctuation || unicodeCategory == UnicodeCategory.CurrencySymbol || unicodeCategory == UnicodeCategory.DashPunctuation || unicodeCategory == UnicodeCategory.DecimalDigitNumber || unicodeCategory == UnicodeCategory.FinalQuotePunctuation || unicodeCategory == UnicodeCategory.InitialQuotePunctuation || unicodeCategory == UnicodeCategory.LetterNumber || unicodeCategory == UnicodeCategory.LowercaseLetter || unicodeCategory == UnicodeCategory.MathSymbol || unicodeCategory == UnicodeCategory.OpenPunctuation || unicodeCategory == UnicodeCategory.TitlecaseLetter || unicodeCategory == UnicodeCategory.UppercaseLetter || unicodeCategory == UnicodeCategory.OtherPunctuation)
			{
				flag = true;
			}
			str = (flag ? c.ToString() : String.Format("\\u{0:X4}", (Int32)c));
			this.WriteLiteral(this.CharStart);
			this.formatter.WriteLiteral(str);
			this.WriteLiteral(this.CharEnd);
		}

		protected override void WriteFieldName(FieldReference field)
		{
			this.WriteReference(this.GetFieldName(field), field);
		}

		protected override void WriteFieldTypeAndName(FieldDefinition field)
		{
			CustomAttribute customAttribute;
			if (field.IsUnsafe)
			{
				this.WriteKeyword(base.KeyWordWriter.Unsafe);
				this.WriteSpace();
			}
			TypeReference fieldType = field.FieldType;
			if (fieldType.IsRequiredModifier && (fieldType as RequiredModifierType).ModifierType.FullName == "System.Runtime.CompilerServices.IsVolatile")
			{
				this.WriteKeyword(base.KeyWordWriter.Volatile);
				this.WriteSpace();
				fieldType = (fieldType as RequiredModifierType).ElementType;
			}
			string fieldName = this.GetFieldName(field);
			if (!field.TryGetDynamicAttribute(out customAttribute))
			{
				this.WriteTypeAndName(fieldType, fieldName, field);
				return;
			}
			this.WriteDynamicType(fieldType, customAttribute);
			this.WriteSpace();
			int currentPosition = this.formatter.CurrentPosition;
			this.WriteReference(fieldName, field);
			int num = this.formatter.CurrentPosition - 1;
			this.currentWritingInfo.MemberDeclarationToCodePostionMap[field] = new OffsetSpan(currentPosition, num);
		}

		protected override void WriteFire(EventDefinition @event)
		{
		}

		protected override void WriteGenericInstanceMethod(GenericInstanceMethod genericMethod)
		{
			MethodReference elementMethod = genericMethod.ElementMethod;
			string methodName = this.GetMethodName(elementMethod);
			if (!this.HasRefOrOutParameter(elementMethod))
			{
				this.WriteReference(methodName, genericMethod);
			}
			else
			{
				MethodDefinition methodDefinition = genericMethod.ElementMethod.Resolve();
				if (methodDefinition != null)
				{
					this.WriteReference(methodName, methodDefinition);
				}
				else
				{
					this.WriteNotResolvedReference(methodName, genericMethod, "Out parameters might be shown as ref. Please, locate the assembly where the method is defined.");
				}
			}
			if (genericMethod.HasAnonymousArgument())
			{
				return;
			}
			this.WriteToken(this.GenericLeftBracket);
			Mono.Collections.Generic.Collection<TypeReference> genericArguments = genericMethod.GenericArguments;
			for (int i = 0; i < genericArguments.Count; i++)
			{
				if (i > 0)
				{
					this.WriteToken(",");
					this.WriteSpace();
				}
				this.WriteReferenceAndNamespaceIfInCollision(genericArguments[i]);
			}
			this.WriteToken(this.GenericRightBracket);
		}

		protected override void WriteIndexerKeywords()
		{
		}

		protected override void WriteInterfacesInheritColon(TypeDefinition type)
		{
			this.WriteBaseTypeInheritColon();
		}

		public override void WriteMemberNavigationPathFullName(object member)
		{
			if (member is ParameterReference)
			{
				this.formatter.Write(((ParameterReference)member).Name);
				return;
			}
			if (member is TypeReference)
			{
				this.formatter.Write(((TypeReference)member).GetFriendlyTypeName(base.Language, "<", ">"));
				return;
			}
			if (member is MemberReference)
			{
				this.formatter.Write(((MemberReference)member).GetFriendlyFullName(base.Language));
			}
		}

		protected override void WriteMethodReference(string name, MethodReference reference)
		{
			if (name == "this" || name == "base")
			{
				base.WriteMethodReference(name, reference);
				return;
			}
			if ((reference.Name == ".ctor" || reference.Name == ".cctor") && name != "this" && name != "base")
			{
				name = Utilities.EscapeTypeNameIfNeeded(name, base.Language);
			}
			else
			{
				name = Utilities.EscapeNameIfNeeded(name, base.Language);
			}
			base.WriteMethodReference(name, reference);
		}

		protected override void WriteMethodReference(MethodReferenceExpression methodReferenceExpression)
		{
			MethodReference method = methodReferenceExpression.Method;
			string methodName = this.GetMethodName(method);
			if (methodReferenceExpression.Method is GenericInstanceMethod)
			{
				this.WriteGenericInstanceMethod(method as GenericInstanceMethod);
				return;
			}
			if (!this.HasRefOrOutParameter(methodReferenceExpression.Method) || method.Resolve() != null)
			{
				this.WriteReference(methodName, method);
				return;
			}
			this.WriteNotResolvedReference(methodName, method, "Out parameters might be shown as ref. Please, locate the assembly where the method is defined.");
		}

		protected override void WriteMethodReturnType(MethodDefinition method)
		{
			CustomAttribute customAttribute;
			if (method.MethodReturnType.TryGetDynamicAttribute(out customAttribute))
			{
				this.WriteDynamicType(method.ReturnType, customAttribute);
				return;
			}
			if (!method.ReturnType.IsByReference)
			{
				base.WriteMethodReturnType(method);
				return;
			}
			this.WriteKeyword(base.KeyWordWriter.ByRef);
			this.WriteSpace();
			this.WriteReferenceAndNamespaceIfInCollision(method.ReturnType.GetElementType());
		}

		protected override bool WriteMethodVisibility(MethodDefinition method)
		{
			if (method.HasOverrides && method.IsPrivate)
			{
				return false;
			}
			return base.WriteMethodVisibility(method);
		}

		private void WriteModifier(TypeSpecification typeSpecification)
		{
			StringBuilder stringBuilder = new StringBuilder();
			TypeReference elementType = typeSpecification;
			IModifierType modifierType = typeSpecification as IModifierType;
			bool flag = false;
			while (modifierType != null)
			{
				if (modifierType.ModifierType.FullName != "System.Runtime.CompilerServices.IsVolatile")
				{
					if (flag)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append((modifierType is RequiredModifierType ? "modreq" : "modopt"));
					stringBuilder.Append("(");
					stringBuilder.Append(modifierType.ModifierType.FullName);
					stringBuilder.Append(")");
					flag = true;
					elementType = modifierType.ElementType;
					modifierType = elementType as IModifierType;
				}
				else
				{
					elementType = modifierType.ElementType;
					modifierType = elementType as IModifierType;
				}
			}
			if (flag)
			{
				this.WriteCommentBlock(stringBuilder.ToString());
				this.WriteSpace();
			}
			this.WriteReferenceAndNamespaceIfInCollision(elementType);
		}

		protected override void WriteNestedTypeWriteLine()
		{
			this.WriteLine();
		}

		protected override bool WritePropertyAsIndexer(PropertyDefinition property)
		{
			string @this = base.KeyWordWriter.This;
			if (property.IsExplicitImplementation())
			{
				int num = property.Name.LastIndexOf(".");
				@this = property.Name.Replace(property.Name.Substring(num + 1), base.KeyWordWriter.This);
			}
			this.WriteTypeAndName(property.PropertyType, @this, property);
			this.Write(this.IndexLeftBracket);
			base.WritePropertyParameters(property);
			this.Write(this.IndexRightBracket);
			return true;
		}

		protected override void WritePropertyName(PropertyDefinition property)
		{
			this.WriteReference(this.GetPropertyName(property), property);
		}

		protected override void WritePropertyTypeAndNameWithArguments(PropertyDefinition property)
		{
			CustomAttribute customAttribute;
			string propertyName = this.GetPropertyName(property);
			if (property.TryGetDynamicAttribute(out customAttribute))
			{
				this.WriteDynamicType(property.PropertyType, customAttribute);
				this.WriteSpace();
				this.WriteReference(propertyName, property);
				return;
			}
			base.WriteTypeAndName(property.PropertyType, propertyName, property);
			if (base.HasArguments(property))
			{
				this.WriteToken("(");
				base.WritePropertyParameters(property);
				this.WriteToken(")");
			}
		}

		internal override void WriteReference(string name, object reference)
		{
			if (this.isDynamicEnumerator != null)
			{
				if (!this.isDynamicEnumerator.MoveNext())
				{
					throw new Exception("Invalid argument type for DynamicAttribute");
				}
				if ((Boolean)this.isDynamicEnumerator.Current)
				{
					this.WriteKeyword(base.KeyWordWriter.Dynamic);
					return;
				}
			}
			base.WriteReference(name, reference);
		}

		protected override void WriteRightPartOfBinaryExpression(BinaryExpression binaryExpression)
		{
			if (binaryExpression.IsAssignmentExpression && binaryExpression.Left.CodeNodeType == CodeNodeType.RefVariableDeclarationExpression)
			{
				this.WriteKeyword(base.KeyWordWriter.ByRef);
				this.WriteSpace();
				if (binaryExpression.Right.CodeNodeType == CodeNodeType.UnaryExpression)
				{
					UnaryExpression right = binaryExpression.Right as UnaryExpression;
					if (right.Operator == UnaryOperator.AddressReference)
					{
						this.Visit(right.Operand);
						return;
					}
				}
			}
			this.Visit(binaryExpression.Right);
		}

		protected override void WriteSpecialBetweenParenthesis(Expression expression)
		{
			this.WriteToken("(");
			this.Visit(expression);
			this.WriteToken(")");
		}

		protected override void WriteSpecialBetweenParenthesis(Action action)
		{
			this.WriteToken("(");
			action();
			this.WriteToken(")");
		}

		protected override void WriteTypeInterfaces(TypeDefinition type, bool isPartial, bool baseTypeWritten)
		{
			List<TypeReference> typeReferences;
			typeReferences = (this.TypeContext.CurrentType != type || !this.TypeContext.IsWinRTImplementation ? type.Interfaces.ToList<TypeReference>() : type.Interfaces.Where<TypeReference>((TypeReference @interface) => {
				TypeDefinition typeDefinition = @interface.Resolve();
				if (typeDefinition == null)
				{
					return true;
				}
				return !typeDefinition.IsWindowsRuntime;
			}).ToList<TypeReference>());
			if (typeReferences.Count > 0)
			{
				int num = 0;
				for (int i = 0; i < typeReferences.Count; i++)
				{
					if (!isPartial || base.IsImplemented(type, typeReferences[i].Resolve()))
					{
						if (num == 0)
						{
							if (!baseTypeWritten)
							{
								this.WriteInterfacesInheritColon(type);
							}
							else
							{
								base.WriteInheritComma();
							}
						}
						if (num > 0)
						{
							base.WriteInheritComma();
						}
						num++;
						this.WriteReferenceAndNamespaceIfInCollision(typeReferences[i]);
					}
				}
			}
		}

		protected override void WriteTypeSpecification(TypeSpecification typeSpecification, int startingArgument = 0)
		{
			if (this.isDynamicEnumerator != null && !typeSpecification.IsGenericInstance)
			{
				if (typeSpecification.IsArray)
				{
					for (TypeReference i = typeSpecification; i.IsArray; i = (i as ArrayType).ElementType)
					{
						if (!this.isDynamicEnumerator.MoveNext() || (Boolean)this.isDynamicEnumerator.Current)
						{
							throw new Exception("Invalid argument type for DynamicAttribute");
						}
					}
				}
				else if (!this.isDynamicEnumerator.MoveNext() || (Boolean)this.isDynamicEnumerator.Current)
				{
					throw new Exception("Invalid argument type for DynamicAttribute");
				}
			}
			if (typeSpecification.IsRequiredModifier || typeSpecification.IsOptionalModifier)
			{
				this.WriteModifier(typeSpecification);
				return;
			}
			base.WriteTypeSpecification(typeSpecification, startingArgument);
		}

		private void WriteVariableName(VariableDefinition variable)
		{
			base.WriteAndMapVariableToCode(() => this.Write(base.GetVariableName(variable)), variable);
		}
	}
}