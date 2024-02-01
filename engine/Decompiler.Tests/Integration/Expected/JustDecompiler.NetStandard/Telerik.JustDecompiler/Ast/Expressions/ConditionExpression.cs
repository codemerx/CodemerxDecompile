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
	public class ConditionExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				ConditionExpression conditionExpression = null;
				yield return conditionExpression.Condition;
				yield return conditionExpression.Then;
				yield return conditionExpression.Else;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.ConditionExpression;
			}
		}

		public Expression Condition
		{
			get;
			set;
		}

		public Expression Else
		{
			get;
			set;
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.Then.ExpressionType;
			}
		}

		public override bool HasType
		{
			get
			{
				return this.Then.HasType;
			}
		}

		public Expression Then
		{
			get;
			set;
		}

		public ConditionExpression(Expression condition, Expression then, Expression @else, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Condition = condition;
			this.Then = then;
			this.Else = @else;
		}

		public override Expression Clone()
		{
			return new ConditionExpression(this.Condition.Clone(), this.Then.Clone(), this.Else.Clone(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new ConditionExpression(this.Condition.CloneExpressionOnly(), this.Then.CloneExpressionOnly(), this.Else.CloneExpressionOnly(), null);
		}

		public override bool Equals(Expression other)
		{
			ConditionExpression conditionExpression = other as ConditionExpression;
			if (conditionExpression == null)
			{
				return false;
			}
			if (!this.Condition.Equals(conditionExpression.Condition) || !this.Then.Equals(conditionExpression.Then))
			{
				return false;
			}
			return this.Else.Equals(conditionExpression.Else);
		}
	}
}