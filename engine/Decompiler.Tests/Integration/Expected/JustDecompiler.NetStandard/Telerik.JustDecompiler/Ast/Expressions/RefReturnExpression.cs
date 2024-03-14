using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class RefReturnExpression : ReturnExpression
	{
		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.RefReturnExpression;
			}
		}

		public RefReturnExpression(Expression value, IEnumerable<Instruction> instructions) : base(value, instructions)
		{
		}

		public override Expression Clone()
		{
			Expression expression;
			if (base.Value != null)
			{
				expression = base.Value.Clone();
			}
			else
			{
				expression = null;
			}
			return new RefReturnExpression(expression, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			Expression expression;
			if (base.Value != null)
			{
				expression = base.Value.CloneExpressionOnly();
			}
			else
			{
				expression = null;
			}
			return new RefReturnExpression(expression, null);
		}

		public override bool Equals(Expression other)
		{
			if (!(other is RefReturnExpression))
			{
				return false;
			}
			RefReturnExpression refReturnExpression = other as RefReturnExpression;
			if (base.Value == null)
			{
				return refReturnExpression.Value == null;
			}
			return base.Value.Equals(refReturnExpression.Value);
		}
	}
}