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
				return Telerik.JustDecompiler.Ast.CodeNodeType.RefVariableDeclarationExpression;
			}
		}

		public RefVariableDeclarationExpression(VariableDefinition variable, IEnumerable<Instruction> instructions) : base(variable, instructions)
		{
		}

		public override Expression Clone()
		{
			return new RefVariableDeclarationExpression(base.Variable, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new RefVariableDeclarationExpression(base.Variable, null);
		}

		public override bool Equals(Expression other)
		{
			if (!(other is RefVariableDeclarationExpression))
			{
				return false;
			}
			RefVariableDeclarationExpression refVariableDeclarationExpression = other as RefVariableDeclarationExpression;
			return (object)base.Variable.Resolve() == (object)refVariableDeclarationExpression.Variable.Resolve();
		}
	}
}