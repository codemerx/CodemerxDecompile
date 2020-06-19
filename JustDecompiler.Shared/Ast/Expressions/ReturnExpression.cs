using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class ReturnExpression : Expression
    {
		public Expression Value { get; set; }

        public ReturnExpression(Expression value, IEnumerable<Instruction> instructions)
            :base(instructions)
        {
            this.Value = value;
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                if (Value != null)
                {
                    yield return Value;
                }
            }
        }

		public override bool Equals(Expression other)
		{ 
            if(!(other is ReturnExpression))
            {
                return false;
            }
            ReturnExpression returnExp = other as ReturnExpression;
            if (this.Value == null)
            {
                return returnExp.Value == null;
            }
            return this.Value.Equals(returnExp.Value);
        }

        public override Expression Clone()
        {
            Expression clonnedValue = Value != null ? Value.Clone() : null;
			ReturnExpression result = new ReturnExpression(clonnedValue, instructions);
			return result;
        }

        public override Expression CloneExpressionOnly()
        {
            Expression clonnedValue = Value != null ? Value.CloneExpressionOnly() : null;
            ReturnExpression result = new ReturnExpression(clonnedValue, null);
            return result;
        }

        public override bool HasType
        {
            get
            {
                return false;
            }
        }

        public override TypeReference ExpressionType
        {
            get
            {
                throw new NotSupportedException("Return expressions have no type.");
            }
            set
            {
                throw new NotSupportedException("Return expressions cannot have type.");
            }
        }

        public override CodeNodeType CodeNodeType
        {
            get
            {
                return CodeNodeType.ReturnExpression;
            }
        }
    }
}
