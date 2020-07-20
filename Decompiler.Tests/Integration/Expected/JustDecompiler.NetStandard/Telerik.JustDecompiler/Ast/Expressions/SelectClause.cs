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
				stackVariable1 = new SelectClause.u003cget_Childrenu003ed__11(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 74;
			}
		}

		public Telerik.JustDecompiler.Ast.Expressions.Expression Expression
		{
			get;
			set;
		}

		public SelectClause(Telerik.JustDecompiler.Ast.Expressions.Expression selectExpression, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Expression(selectExpression);
			return;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new SelectClause(this.get_Expression().Clone(), this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new SelectClause(this.get_Expression().CloneExpressionOnly(), null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			V_0 = other as SelectClause;
			if (V_0 == null)
			{
				return false;
			}
			return this.get_Expression().Equals(V_0.get_Expression());
		}
	}
}