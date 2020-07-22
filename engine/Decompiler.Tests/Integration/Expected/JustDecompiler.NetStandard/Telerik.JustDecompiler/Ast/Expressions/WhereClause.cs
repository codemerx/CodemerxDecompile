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
				stackVariable1 = new WhereClause.u003cget_Childrenu003ed__11(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 75;
			}
		}

		public Expression Condition
		{
			get;
			set;
		}

		public WhereClause(Expression condition, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Condition(condition);
			return;
		}

		public override Expression Clone()
		{
			return new WhereClause(this.get_Condition().Clone(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new WhereClause(this.get_Condition().CloneExpressionOnly(), null);
		}

		public override bool Equals(Expression other)
		{
			V_0 = other as WhereClause;
			if (V_0 == null)
			{
				return false;
			}
			return this.get_Condition().Equals(V_0.get_Condition());
		}
	}
}