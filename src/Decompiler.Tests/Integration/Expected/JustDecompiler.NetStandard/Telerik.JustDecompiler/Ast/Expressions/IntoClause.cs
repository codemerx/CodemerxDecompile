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
				IntoClause intoClause = null;
				yield return intoClause.Identifier;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.IntoClause;
			}
		}

		public VariableReferenceExpression Identifier
		{
			get;
			set;
		}

		public IntoClause(VariableReferenceExpression identifier, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Identifier = identifier;
		}

		public override Expression Clone()
		{
			return new IntoClause((VariableReferenceExpression)this.Identifier.Clone(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new IntoClause((VariableReferenceExpression)this.Identifier.CloneExpressionOnly(), null);
		}

		public override bool Equals(Expression other)
		{
			IntoClause intoClause = other as IntoClause;
			if (intoClause == null)
			{
				return false;
			}
			return this.Identifier.Equals(intoClause.Identifier);
		}
	}
}