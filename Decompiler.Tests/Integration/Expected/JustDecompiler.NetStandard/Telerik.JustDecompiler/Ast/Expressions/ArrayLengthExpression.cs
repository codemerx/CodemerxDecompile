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
	public class ArrayLengthExpression : Expression
	{
		private readonly TypeSystem theTypeSystem;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				ArrayLengthExpression arrayLengthExpression = null;
				yield return arrayLengthExpression.Target;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.ArrayLengthExpression;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.theTypeSystem.Int32;
			}
			set
			{
				base.ExpressionType = value;
			}
		}

		public Expression Target
		{
			get;
			set;
		}

		public ArrayLengthExpression(Expression target, TypeSystem theTypeSystem, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Target = target;
			this.theTypeSystem = theTypeSystem;
		}

		public override Expression Clone()
		{
			return new ArrayLengthExpression(this.Target.Clone(), this.theTypeSystem, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new ArrayLengthExpression(this.Target.CloneExpressionOnly(), this.theTypeSystem, null);
		}

		public override bool Equals(Expression other)
		{
			if (other == null || !(other is ArrayLengthExpression))
			{
				return false;
			}
			return this.Target.Equals((other as ArrayLengthExpression).Target);
		}
	}
}