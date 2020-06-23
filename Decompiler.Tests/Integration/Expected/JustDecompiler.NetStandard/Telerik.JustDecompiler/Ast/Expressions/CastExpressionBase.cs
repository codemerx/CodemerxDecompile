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
	public abstract class CastExpressionBase : Telerik.JustDecompiler.Ast.Expressions.Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				CastExpressionBase castExpressionBase = null;
				yield return castExpressionBase.Expression;
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
				return true;
			}
		}

		public TypeReference TargetType
		{
			get;
			set;
		}

		public CastExpressionBase(Telerik.JustDecompiler.Ast.Expressions.Expression expression, TypeReference targetType, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Expression = expression;
			this.TargetType = targetType;
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			if (!(other is CastExpressionBase))
			{
				return false;
			}
			if (this.TargetType.FullName != (other as CastExpressionBase).TargetType.FullName)
			{
				return false;
			}
			return this.Expression.Equals((other as CastExpressionBase).Expression);
		}
	}
}