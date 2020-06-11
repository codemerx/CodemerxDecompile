using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class LetClause : QueryClause
    {
        public VariableReferenceExpression Identifier { get; set; }
        public Expression Expression { get; set; }

        public LetClause(VariableReferenceExpression identifier, Expression expression, IEnumerable<Instruction> instructions) : base(instructions)
        {
            this.Identifier = identifier;
            this.Expression = expression;
        }

        public override bool Equals(Expression other)
        {
            LetClause letClause = other as LetClause;
            return letClause != null &&
                this.Identifier.Equals(letClause.Identifier) &&
                this.Expression.Equals(letClause.Expression);
        }

        public override Expression Clone()
        {
            return new LetClause((VariableReferenceExpression)this.Identifier.Clone(), this.Expression.Clone(), instructions);
        }

        public override Expression CloneExpressionOnly()
        {
            return new LetClause((VariableReferenceExpression)this.Identifier.CloneExpressionOnly(), this.Expression.CloneExpressionOnly(), null);
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.LetClause; }
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                yield return Identifier;
                yield return Expression;
            }
        }
    }
}
