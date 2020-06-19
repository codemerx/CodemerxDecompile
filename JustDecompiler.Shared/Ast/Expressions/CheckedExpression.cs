using System;
using System.Collections.Generic;
using System.Linq;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class CheckedExpression : Expression
    {
        public Expression Expression { get; set; }

        public CheckedExpression(Expression expression, IEnumerable<Mono.Cecil.Cil.Instruction> instructions)
            : base(instructions)
        {
            this.Expression = expression;
        }

        public override bool Equals(Expression other)
        {
            CheckedExpression otherExpression = other as CheckedExpression;
            return otherExpression != null && this.Expression.Equals(otherExpression.Expression);
        }

        public override Expression Clone()
        {
            return new CheckedExpression(this.Expression.Clone(), this.instructions);
        }

        public override Expression CloneExpressionOnly()
        {
            return new CheckedExpression(this.Expression.CloneExpressionOnly(), null);
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.CheckedExpression; }
        }

        public override IEnumerable<ICodeNode> Children
        {
            get { yield return this.Expression; }
        }
    }
}
