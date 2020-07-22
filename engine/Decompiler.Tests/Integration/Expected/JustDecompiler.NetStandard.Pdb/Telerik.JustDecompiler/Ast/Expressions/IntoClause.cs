using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class IntoClause : QueryClause
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new IntoClause.u003cget_Childrenu003ed__11(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 80;
			}
		}

		public VariableReferenceExpression Identifier
		{
			get;
			set;
		}

		public IntoClause(VariableReferenceExpression identifier, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Identifier(identifier);
			return;
		}

		public override Expression Clone()
		{
			return new IntoClause((VariableReferenceExpression)this.get_Identifier().Clone(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new IntoClause((VariableReferenceExpression)this.get_Identifier().CloneExpressionOnly(), null);
		}

		public override bool Equals(Expression other)
		{
			V_0 = other as IntoClause;
			if (V_0 == null)
			{
				return false;
			}
			return this.get_Identifier().Equals(V_0.get_Identifier());
		}
	}
}