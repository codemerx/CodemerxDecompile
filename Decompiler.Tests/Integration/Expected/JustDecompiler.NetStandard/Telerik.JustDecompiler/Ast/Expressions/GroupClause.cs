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
				stackVariable1 = new GroupClause.u003cget_Childrenu003ed__15(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 76;
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

		public GroupClause(Telerik.JustDecompiler.Ast.Expressions.Expression expression, Telerik.JustDecompiler.Ast.Expressions.Expression key, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Expression(expression);
			this.set_GroupKey(key);
			return;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new GroupClause(this.get_Expression().Clone(), this.get_GroupKey().Clone(), this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new GroupClause(this.get_Expression().CloneExpressionOnly(), this.get_GroupKey().CloneExpressionOnly(), null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			V_0 = other as GroupClause;
			if (V_0 == null || !this.get_Expression().Equals(V_0.get_Expression()))
			{
				return false;
			}
			return this.get_GroupKey().Equals(V_0.get_GroupKey());
		}
	}
}