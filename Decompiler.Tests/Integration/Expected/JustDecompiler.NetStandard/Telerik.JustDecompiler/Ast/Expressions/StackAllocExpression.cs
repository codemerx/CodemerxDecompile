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
	public class StackAllocExpression : Telerik.JustDecompiler.Ast.Expressions.Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				StackAllocExpression stackAllocExpression = null;
				yield return stackAllocExpression.Expression;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.StackAllocExpression;
			}
		}

		public Telerik.JustDecompiler.Ast.Expressions.Expression Expression
		{
			get;
			set;
		}

		public override TypeReference ExpressionType
		{
			get;
			set;
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public StackAllocExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression, TypeReference expressionType, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Expression = expression;
			this.ExpressionType = expressionType;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new StackAllocExpression(this.Expression.Clone(), this.ExpressionType, this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new StackAllocExpression(this.Expression.CloneExpressionOnly(), this.ExpressionType, null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			if (!(other is StackAllocExpression))
			{
				return false;
			}
			return this.Expression.Equals((other as StackAllocExpression).Expression);
		}
	}
}