using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class ReturnExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				ReturnExpression returnExpression = null;
				if (returnExpression.Value != null)
				{
					yield return returnExpression.Value;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.ReturnExpression;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				throw new NotSupportedException("Return expressions have no type.");
			}
			set
			{
				throw new NotSupportedException("Return expressions cannot have type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return false;
			}
		}

		public Expression Value
		{
			get;
			set;
		}

		public ReturnExpression(Expression value, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Value = value;
		}

		public override Expression Clone()
		{
			Expression expression;
			if (this.Value != null)
			{
				expression = this.Value.Clone();
			}
			else
			{
				expression = null;
			}
			return new ReturnExpression(expression, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			Expression expression;
			if (this.Value != null)
			{
				expression = this.Value.CloneExpressionOnly();
			}
			else
			{
				expression = null;
			}
			return new ReturnExpression(expression, null);
		}

		public override bool Equals(Expression other)
		{
			if (!(other is ReturnExpression))
			{
				return false;
			}
			ReturnExpression returnExpression = other as ReturnExpression;
			if (this.Value == null)
			{
				return returnExpression.Value == null;
			}
			return this.Value.Equals(returnExpression.Value);
		}
	}
}