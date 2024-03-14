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
	public class AwaitExpression : Telerik.JustDecompiler.Ast.Expressions.Expression
	{
		private Telerik.JustDecompiler.Ast.Expressions.Expression expression;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				AwaitExpression awaitExpression = null;
				yield return awaitExpression.expression;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.AwaitExpression;
			}
		}

		public Telerik.JustDecompiler.Ast.Expressions.Expression Expression
		{
			get
			{
				return this.expression;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("expression");
				}
				this.expression = value;
			}
		}

		public AwaitExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression, TypeReference type, IEnumerable<Instruction> instructions) : base(instructions)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			this.expression = expression;
			this.ExpressionType = type;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new AwaitExpression(this.Expression.Clone(), this.ExpressionType, this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new AwaitExpression(this.Expression.CloneExpressionOnly(), this.ExpressionType, null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			if (other.CodeNodeType != Telerik.JustDecompiler.Ast.CodeNodeType.AwaitExpression)
			{
				return false;
			}
			return (other as AwaitExpression).expression.Equals(this.expression);
		}
	}
}