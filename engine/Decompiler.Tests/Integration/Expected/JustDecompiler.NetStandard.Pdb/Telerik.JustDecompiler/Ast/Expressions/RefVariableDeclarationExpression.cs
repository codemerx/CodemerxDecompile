using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class RefVariableDeclarationExpression : VariableDeclarationExpression
	{
		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 93;
			}
		}

		public RefVariableDeclarationExpression(VariableDefinition variable, IEnumerable<Instruction> instructions)
		{
			base(variable, instructions);
			return;
		}

		public override Expression Clone()
		{
			return new RefVariableDeclarationExpression(this.get_Variable(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new RefVariableDeclarationExpression(this.get_Variable(), null);
		}

		public override bool Equals(Expression other)
		{
			if (other as RefVariableDeclarationExpression == null)
			{
				return false;
			}
			V_0 = other as RefVariableDeclarationExpression;
			return (object)this.get_Variable().Resolve() == (object)V_0.get_Variable().Resolve();
		}
	}
}