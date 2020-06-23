using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class WhereClause : QueryClause
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				WhereClause whereClause = null;
				yield return whereClause.Condition;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.WhereClause;
			}
		}

		public Expression Condition
		{
			get;
			set;
		}

		public WhereClause(Expression condition, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Condition = condition;
		}

		public override Expression Clone()
		{
			return new WhereClause(this.Condition.Clone(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new WhereClause(this.Condition.CloneExpressionOnly(), null);
		}

		public override bool Equals(Expression other)
		{
			WhereClause whereClause = other as WhereClause;
			if (whereClause == null)
			{
				return false;
			}
			return this.Condition.Equals(whereClause.Condition);
		}
	}
}