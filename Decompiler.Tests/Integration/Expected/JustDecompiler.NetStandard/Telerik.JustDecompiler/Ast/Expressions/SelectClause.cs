using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class SelectClause : QueryClause
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				SelectClause selectClause = null;
				yield return selectClause.Expression;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.SelectClause;
			}
		}

		public Telerik.JustDecompiler.Ast.Expressions.Expression Expression
		{
			get;
			set;
		}

		public SelectClause(Telerik.JustDecompiler.Ast.Expressions.Expression selectExpression, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Expression = selectExpression;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new SelectClause(this.Expression.Clone(), this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new SelectClause(this.Expression.CloneExpressionOnly(), null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			SelectClause selectClause = other as SelectClause;
			if (selectClause == null)
			{
				return false;
			}
			return this.Expression.Equals(selectClause.Expression);
		}
	}
}