using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class AwaitExpression : Expression
    {
        private Expression expression;
        public Expression Expression
        {
            get
            {
                return this.expression;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("expression");
                }

                this.expression = value;
            }
        }

        public AwaitExpression(Expression expression, TypeReference type, IEnumerable<Instruction> instructions)
            :base(instructions)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            this.expression = expression;
            this.ExpressionType = type;
        }

        public override bool Equals(Expression other)
        {
            return other.CodeNodeType == CodeNodeType.AwaitExpression && (other as AwaitExpression).expression.Equals(this.expression);
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.AwaitExpression; }
        }

        public override Expression Clone()
        {
            return new AwaitExpression(this.Expression.Clone(), this.ExpressionType, this.instructions);
        }

        public override Expression CloneExpressionOnly()
        {
            return new AwaitExpression(this.Expression.CloneExpressionOnly(), this.ExpressionType, null);
        }

        public override IEnumerable<ICodeNode> Children
        {
            get { yield return this.expression; }
        }
    }
}
