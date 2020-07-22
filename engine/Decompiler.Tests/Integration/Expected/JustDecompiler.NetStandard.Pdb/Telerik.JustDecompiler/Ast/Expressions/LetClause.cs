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
				stackVariable1 = new LetClause.u003cget_Childrenu003ed__15(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 79;
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

		public LetClause(VariableReferenceExpression identifier, Telerik.JustDecompiler.Ast.Expressions.Expression expression, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Identifier(identifier);
			this.set_Expression(expression);
			return;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new LetClause((VariableReferenceExpression)this.get_Identifier().Clone(), this.get_Expression().Clone(), this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new LetClause((VariableReferenceExpression)this.get_Identifier().CloneExpressionOnly(), this.get_Expression().CloneExpressionOnly(), null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			V_0 = other as LetClause;
			if (V_0 == null || !this.get_Identifier().Equals(V_0.get_Identifier()))
			{
				return false;
			}
			return this.get_Expression().Equals(V_0.get_Expression());
		}
	}
}