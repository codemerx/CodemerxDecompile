using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class RefReturnExpression : ReturnExpression
    {
        public RefReturnExpression(Expression value, IEnumerable<Instruction> instructions)
            : base(value, instructions)
        {
        }

        public override bool Equals(Expression other)
        {
            if (!(other is RefReturnExpression))
            {
                return false;
            }
            RefReturnExpression refReturnExp = other as RefReturnExpression;
            if (this.Value == null)
            {
                return refReturnExp.Value == null;
            }
            return this.Value.Equals(refReturnExp.Value);
        }

        public override Expression Clone()
        {
            Expression clonnedValue = Value != null ? Value.Clone() : null;
            RefReturnExpression result = new RefReturnExpression(clonnedValue, instructions);
            return result;
        }

        public override Expression CloneExpressionOnly()
        {
            Expression clonnedValue = Value != null ? Value.CloneExpressionOnly() : null;
            RefReturnExpression result = new RefReturnExpression(clonnedValue, null);
            return result;
        }

        public override CodeNodeType CodeNodeType
        {
            get
            {
                return CodeNodeType.RefReturnExpression;
            }
        }
    }
}
