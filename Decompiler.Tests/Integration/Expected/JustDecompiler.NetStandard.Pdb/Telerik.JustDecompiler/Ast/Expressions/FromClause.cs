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
				stackVariable1 = new FromClause.u003cget_Childrenu003ed__15(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 73;
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

		public FromClause(Expression identifier, Expression collectionExpression, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Identifier(identifier);
			this.set_Collection(collectionExpression);
			return;
		}

		public override Expression Clone()
		{
			return new FromClause(this.get_Identifier().Clone(), this.get_Collection().Clone(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new FromClause(this.get_Identifier().CloneExpressionOnly(), this.get_Collection().CloneExpressionOnly(), null);
		}

		public override bool Equals(Expression other)
		{
			V_0 = other as FromClause;
			if (V_0 == null || !this.get_Identifier().Equals(V_0.get_Identifier()))
			{
				return false;
			}
			return this.get_Collection().Equals(V_0.get_Collection());
		}
	}
}