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
	public class YieldReturnExpression : Telerik.JustDecompiler.Ast.Expressions.Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				YieldReturnExpression yieldReturnExpression = null;
				yield return yieldReturnExpression.Expression;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.YieldReturnExpression;
			}
		}

		public Telerik.JustDecompiler.Ast.Expressions.Expression Expression
		{
			get;
			set;
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.Expression.ExpressionType;
			}
			set
			{
				this.Expression.ExpressionType = value;
			}
		}

		public YieldReturnExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Expression = expression;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new YieldReturnExpression(this.Expression.Clone(), this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new YieldReturnExpression(this.Expression.CloneExpressionOnly(), null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			if (!(other is YieldReturnExpression))
			{
				return false;
			}
			YieldReturnExpression yieldReturnExpression = other as YieldReturnExpression;
			if (this.Expression == null)
			{
				return yieldReturnExpression.Expression == null;
			}
			return this.Expression.Equals(yieldReturnExpression.Expression);
		}
	}
}