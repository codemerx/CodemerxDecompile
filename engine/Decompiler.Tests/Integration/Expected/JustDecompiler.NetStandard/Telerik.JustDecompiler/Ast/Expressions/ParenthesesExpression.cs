using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class ParenthesesExpression : Telerik.JustDecompiler.Ast.Expressions.Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				ParenthesesExpression parenthesesExpression = null;
				yield return parenthesesExpression.Expression;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.ParenthesesExpression;
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
				throw new NotSupportedException("Parentheses Expression cannot change its type.");
			}
		}

		public ParenthesesExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression) : this(expression, Enumerable.Empty<Instruction>())
		{
		}

		public ParenthesesExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Expression = expression;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new ParenthesesExpression(this.Expression.Clone(), this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new ParenthesesExpression(this.Expression.Clone(), null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			ParenthesesExpression parenthesesExpression = other as ParenthesesExpression;
			if (parenthesesExpression == null)
			{
				return false;
			}
			return this.Expression.Equals(parenthesesExpression.Expression);
		}
	}
}