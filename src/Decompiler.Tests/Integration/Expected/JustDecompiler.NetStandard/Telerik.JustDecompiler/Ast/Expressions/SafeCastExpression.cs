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
	public class SafeCastExpression : Telerik.JustDecompiler.Ast.Expressions.Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				SafeCastExpression safeCastExpression = null;
				yield return safeCastExpression.Expression;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.SafeCastExpression;
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
				return this.TargetType;
			}
			set
			{
				this.TargetType = value;
			}
		}

		public override bool HasType
		{
			get
			{
				return (object)this.TargetType != (object)null;
			}
		}

		public TypeReference TargetType
		{
			get;
			private set;
		}

		public SafeCastExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression, TypeReference targetType, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Expression = expression;
			this.TargetType = targetType;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new SafeCastExpression(this.Expression.Clone(), this.TargetType, this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new SafeCastExpression(this.Expression.CloneExpressionOnly(), this.TargetType, null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			if (!(other is SafeCastExpression))
			{
				return false;
			}
			SafeCastExpression safeCastExpression = other as SafeCastExpression;
			if (this.TargetType.get_FullName() != safeCastExpression.TargetType.get_FullName())
			{
				return false;
			}
			return this.Expression.Equals(safeCastExpression.Expression);
		}
	}
}