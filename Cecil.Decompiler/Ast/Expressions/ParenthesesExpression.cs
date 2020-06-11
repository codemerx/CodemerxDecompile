using System;
using System.Linq;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class ParenthesesExpression : Expression
	{
		public ParenthesesExpression(Expression expression)
			: this(expression, Enumerable.Empty<Instruction>())
		{
		}

		public ParenthesesExpression(Expression expression, IEnumerable<Instruction> instructions)
			: base(instructions)
		{
			this.Expression = expression;
		}

		public Expression Expression { get; set; }

		public override bool Equals(Expression other)
		{
			ParenthesesExpression parentheses = other as ParenthesesExpression;
			bool result = parentheses != null && this.Expression.Equals(parentheses.Expression);
			return result;
		}

		public override Expression Clone()
		{
			ParenthesesExpression result = new ParenthesesExpression(Expression.Clone(), this.instructions);
			return result;
		}

		public override Expression CloneExpressionOnly()
		{
			ParenthesesExpression result = new ParenthesesExpression(Expression.Clone(), null);
			return result;
		}

		public override CodeNodeType CodeNodeType
		{
			get { return CodeNodeType.ParenthesesExpression; }
		}

		public override IEnumerable<ICodeNode> Children
		{
			get { yield return this.Expression; }
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return Expression.ExpressionType;
			}
			set
			{
				throw new NotSupportedException("Parentheses Expression cannot change its type.");
			}
		}
	}
}
