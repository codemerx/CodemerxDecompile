using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class BoxExpression : Expression
	{
		private readonly TypeSystem theTypeSystem;

		public BoxExpression(Expression boxedExpression, TypeSystem theTypeSystem, TypeReference boxedAsType, IEnumerable<Instruction> instructions)
            :base(instructions)
		{
			this.theTypeSystem = theTypeSystem;
			this.BoxedExpression = boxedExpression;
			this.BoxedAs = boxedAsType;
			this.IsAutoBox = false;
		}

		public Expression BoxedExpression { get; set; }

		public TypeReference BoxedAs { get; private set; }

		public bool IsAutoBox { get; set; }

        public override IEnumerable<ICodeNode> Children
        {
            get { yield return BoxedExpression; }
        }

		public override CodeNodeType CodeNodeType
		{
			get
			{
				return Ast.CodeNodeType.BoxExpression;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return theTypeSystem.Object;
			}
			set
			{
				throw new NotSupportedException("Expression type of box expression can not be changed.");
			}
		}

		public override Expression Clone()
		{
            BoxExpression result = new BoxExpression(this.BoxedExpression.Clone(), theTypeSystem, this.BoxedAs, instructions) { IsAutoBox = this.IsAutoBox };
			return result;
		}

        public override Expression CloneExpressionOnly()
        {
            BoxExpression result = new BoxExpression(this.BoxedExpression.CloneExpressionOnly(), theTypeSystem, this.BoxedAs, null) { IsAutoBox = this.IsAutoBox };
            return result;
        }

		public override bool Equals(Expression other)
		{
			BoxExpression otherBox = other as BoxExpression;
			if (otherBox == null)
			{
				return false;
			}
			if (this.BoxedExpression.Equals(otherBox.BoxedExpression))
			{
				return this.BoxedAs.FullName == otherBox.BoxedAs.FullName;
			}
			return false;
		}

	}
}
