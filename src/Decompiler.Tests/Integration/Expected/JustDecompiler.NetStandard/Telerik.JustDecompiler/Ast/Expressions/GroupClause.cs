using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class GroupClause : QueryClause
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				GroupClause groupClause = null;
				yield return groupClause.Expression;
				yield return groupClause.GroupKey;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.GroupClause;
			}
		}

		public Telerik.JustDecompiler.Ast.Expressions.Expression Expression
		{
			get;
			set;
		}

		public Telerik.JustDecompiler.Ast.Expressions.Expression GroupKey
		{
			get;
			set;
		}

		public GroupClause(Telerik.JustDecompiler.Ast.Expressions.Expression expression, Telerik.JustDecompiler.Ast.Expressions.Expression key, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Expression = expression;
			this.GroupKey = key;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new GroupClause(this.Expression.Clone(), this.GroupKey.Clone(), this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new GroupClause(this.Expression.CloneExpressionOnly(), this.GroupKey.CloneExpressionOnly(), null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			GroupClause groupClause = other as GroupClause;
			if (groupClause == null || !this.Expression.Equals(groupClause.Expression))
			{
				return false;
			}
			return this.GroupKey.Equals(groupClause.GroupKey);
		}
	}
}