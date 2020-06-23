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
	public class MakeRefExpression : Telerik.JustDecompiler.Ast.Expressions.Expression
	{
		private readonly TypeReference theType;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				MakeRefExpression makeRefExpression = null;
				yield return makeRefExpression.Expression;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.MakeRefExpression;
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
				return this.theType;
			}
			set
			{
				throw new NotSupportedException("Makeref expression cannot change its type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public MakeRefExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression, TypeReference makeRefType, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Expression = expression;
			this.theType = makeRefType;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new MakeRefExpression(this.Expression.Clone(), this.theType, this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new MakeRefExpression(this.Expression.CloneExpressionOnly(), this.theType, null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			if (!(other is MakeRefExpression))
			{
				return false;
			}
			MakeRefExpression makeRefExpression = other as MakeRefExpression;
			if (this.theType.FullName != makeRefExpression.theType.FullName)
			{
				return false;
			}
			return this.Expression.Equals(makeRefExpression.Expression);
		}
	}
}