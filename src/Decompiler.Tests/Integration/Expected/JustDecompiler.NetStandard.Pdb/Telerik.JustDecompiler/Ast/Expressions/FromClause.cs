using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class FromClause : QueryClause
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				FromClause fromClause = null;
				yield return fromClause.Identifier;
				yield return fromClause.Collection;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.FromClause;
			}
		}

		public Expression Collection
		{
			get;
			set;
		}

		public Expression Identifier
		{
			get;
			set;
		}

		public FromClause(Expression identifier, Expression collectionExpression, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Identifier = identifier;
			this.Collection = collectionExpression;
		}

		public override Expression Clone()
		{
			return new FromClause(this.Identifier.Clone(), this.Collection.Clone(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new FromClause(this.Identifier.CloneExpressionOnly(), this.Collection.CloneExpressionOnly(), null);
		}

		public override bool Equals(Expression other)
		{
			FromClause fromClause = other as FromClause;
			if (fromClause == null || !this.Identifier.Equals(fromClause.Identifier))
			{
				return false;
			}
			return this.Collection.Equals(fromClause.Collection);
		}
	}
}