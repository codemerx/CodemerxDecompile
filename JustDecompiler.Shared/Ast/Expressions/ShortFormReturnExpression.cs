using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class ShortFormReturnExpression : ReturnExpression
	{
		public ShortFormReturnExpression(Expression value, IEnumerable<Instruction> instructions) 
			:base(value, instructions)
		{
		}

		public override bool Equals(Expression other)
		{
			if (!(other is ShortFormReturnExpression))
			{
				return false;
			}
			ReturnExpression returnExp = other as ShortFormReturnExpression;
			if (this.Value == null)
			{
				return returnExp.Value == null;
			}
			return this.Value.Equals(returnExp.Value);
		}

		public override Expression Clone()
		{
			Expression clonnedValue = Value != null ? Value.Clone() : null;
			ShortFormReturnExpression result = new ShortFormReturnExpression(clonnedValue, instructions);
			return result;
		}

		public override Expression CloneExpressionOnly()
		{
			Expression clonnedValue = Value != null ? Value.CloneExpressionOnly() : null;
			ShortFormReturnExpression result = new ShortFormReturnExpression(clonnedValue, null);
			return result;
		}

		public override bool HasType
		{
			get
			{
				return false;
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

		public override CodeNodeType CodeNodeType
		{
			get
			{
				return CodeNodeType.ShortFormReturnExpression;
			}
		}
	}
}
