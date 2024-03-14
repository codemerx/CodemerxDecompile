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
	public class ThrowExpression : Telerik.JustDecompiler.Ast.Expressions.Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				ThrowExpression throwExpression = null;
				if (throwExpression.Expression != null)
				{
					yield return throwExpression.Expression;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.ThrowExpression;
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
				throw new NotSupportedException("Throw expressions have no type.");
			}
			set
			{
				throw new NotSupportedException("Throw expressions cannot have type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return false;
			}
		}

		public ThrowExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Expression = expression;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			Telerik.JustDecompiler.Ast.Expressions.Expression expression;
			if (this.Expression != null)
			{
				expression = this.Expression.Clone();
			}
			else
			{
				expression = null;
			}
			return new ThrowExpression(expression, this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			Telerik.JustDecompiler.Ast.Expressions.Expression expression;
			if (this.Expression != null)
			{
				expression = this.Expression.CloneExpressionOnly();
			}
			else
			{
				expression = null;
			}
			return new ThrowExpression(expression, null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			if (!(other is ThrowExpression))
			{
				return false;
			}
			return this.Expression.Equals((other as ThrowExpression).Expression);
		}
	}
}