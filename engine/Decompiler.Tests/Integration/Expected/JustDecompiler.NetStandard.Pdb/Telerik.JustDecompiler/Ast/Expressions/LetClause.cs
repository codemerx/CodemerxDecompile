using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class LetClause : QueryClause
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				LetClause letClause = null;
				yield return letClause.Identifier;
				yield return letClause.Expression;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.LetClause;
			}
		}

		public Telerik.JustDecompiler.Ast.Expressions.Expression Expression
		{
			get;
			set;
		}

		public VariableReferenceExpression Identifier
		{
			get;
			set;
		}

		public LetClause(VariableReferenceExpression identifier, Telerik.JustDecompiler.Ast.Expressions.Expression expression, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Identifier = identifier;
			this.Expression = expression;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new LetClause((VariableReferenceExpression)this.Identifier.Clone(), this.Expression.Clone(), this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new LetClause((VariableReferenceExpression)this.Identifier.CloneExpressionOnly(), this.Expression.CloneExpressionOnly(), null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			LetClause letClause = other as LetClause;
			if (letClause == null || !this.Identifier.Equals(letClause.Identifier))
			{
				return false;
			}
			return this.Expression.Equals(letClause.Expression);
		}
	}
}