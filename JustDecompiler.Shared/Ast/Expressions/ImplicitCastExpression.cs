using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class ImplicitCastExpression : CastExpressionBase
    {
        public ImplicitCastExpression(Expression expression, TypeReference targetType, IEnumerable<Instruction> instructions)
            : base(expression, targetType, instructions)
        {
        }

        public override CodeNodeType CodeNodeType
        {
            get
            {
                return CodeNodeType.ImplicitCastExpression;
            }
        }

        public override Expression Clone()
        {
            ImplicitCastExpression result = new ImplicitCastExpression(this.Expression.Clone(), this.TargetType, this.instructions);

            return result;
        }

        public override Expression CloneExpressionOnly()
        {
            ImplicitCastExpression result = new ImplicitCastExpression(this.Expression.Clone(), this.TargetType, null);

            return result;
        }
    }
}
