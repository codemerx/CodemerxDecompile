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
				return 94;
			}
		}

		public RefReturnExpression(Expression value, IEnumerable<Instruction> instructions)
		{
			base(value, instructions);
			return;
		}

		public override Expression Clone()
		{
			if (this.get_Value() != null)
			{
				stackVariable4 = this.get_Value().Clone();
			}
			else
			{
				stackVariable4 = null;
			}
			return new RefReturnExpression(stackVariable4, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			if (this.get_Value() != null)
			{
				stackVariable4 = this.get_Value().CloneExpressionOnly();
			}
			else
			{
				stackVariable4 = null;
			}
			return new RefReturnExpression(stackVariable4, null);
		}

		public override bool Equals(Expression other)
		{
			if (other as RefReturnExpression == null)
			{
				return false;
			}
			V_0 = other as RefReturnExpression;
			if (this.get_Value() == null)
			{
				return V_0.get_Value() == null;
			}
			return this.get_Value().Equals(V_0.get_Value());
		}
	}
}