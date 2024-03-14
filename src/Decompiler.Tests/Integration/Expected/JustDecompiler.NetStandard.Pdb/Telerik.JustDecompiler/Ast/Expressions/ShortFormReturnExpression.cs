using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class ShortFormReturnExpression : ReturnExpression
	{
		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.ShortFormReturnExpression;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				throw new NotSupportedException("Short form return expressions have no type.");
			}
			set
			{
				throw new NotSupportedException("Short form return expressions cannot have type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return false;
			}
		}

		public ShortFormReturnExpression(Expression value, IEnumerable<Instruction> instructions) : base(value, instructions)
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
			return new ShortFormReturnExpression(expression, this.instructions);
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
			return new ShortFormReturnExpression(expression, null);
		}

		public override bool Equals(Expression other)
		{
			if (!(other is ShortFormReturnExpression))
			{
				return false;
			}
			ReturnExpression returnExpression = other as ShortFormReturnExpression;
			if (base.Value == null)
			{
				return returnExpression.Value == null;
			}
			return base.Value.Equals(returnExpression.Value);
		}
	}
}