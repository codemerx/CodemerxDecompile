using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class ArrayLengthExpression : Expression
	{
		private readonly TypeSystem theTypeSystem;

		public ArrayLengthExpression(Expression target, TypeSystem theTypeSystem, IEnumerable<Instruction> instructions)
            :base(instructions)
		{
			this.Target = target;
			this.theTypeSystem = theTypeSystem;
		}

		public override bool Equals(Expression other)
		{
			if (other == null || !(other is ArrayLengthExpression))
			{
				return false;
			}
			return this.Target.Equals((other as ArrayLengthExpression).Target);
		}

		public Expression Target { get; set; }

		public override CodeNodeType CodeNodeType
		{
			get
			{
				return Ast.CodeNodeType.ArrayLengthExpression;
			}
		}

		public override Expression Clone()
		{
			return new ArrayLengthExpression(Target.Clone(), theTypeSystem, instructions);
		}

        public override Expression CloneExpressionOnly()
        {
            return new ArrayLengthExpression(Target.CloneExpressionOnly(), theTypeSystem, null);
        }

        public override IEnumerable<ICodeNode> Children
        {
            get { yield return Target; }
        }

		public override TypeReference ExpressionType
		{
			get
			{
				return theTypeSystem.Int32;
			}
			set
			{
				base.ExpressionType = value;
			}
		}
	}
}
