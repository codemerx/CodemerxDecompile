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
	public class CanCastExpression : Telerik.JustDecompiler.Ast.Expressions.Expression
	{
		private Telerik.JustDecompiler.Ast.Expressions.Expression expression;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				CanCastExpression canCastExpression = null;
				yield return canCastExpression.Expression;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.CanCastExpression;
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
				this.expression = value;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.TargetType.Module.TypeSystem.Boolean;
			}
			set
			{
				throw new NotSupportedException("Can cast expression is always boolean.");
			}
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public TypeReference TargetType
		{
			get;
			set;
		}

		public CanCastExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression, TypeReference targetType, IList<Instruction> instructions) : base(instructions)
		{
			this.Expression = expression;
			this.TargetType = targetType;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new CanCastExpression(this.Expression.Clone(), this.TargetType, this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new CanCastExpression(this.Expression.CloneExpressionOnly(), this.TargetType, null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			if (!(other is CanCastExpression))
			{
				return false;
			}
			CanCastExpression canCastExpression = other as CanCastExpression;
			if (this.TargetType.FullName != canCastExpression.TargetType.FullName)
			{
				return false;
			}
			return this.Expression.Equals(canCastExpression.Expression);
		}
	}
}