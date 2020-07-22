using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class SelectClause : QueryClause
    {
        public Expression Expression { get; set; }

        public SelectClause(Expression selectExpression, IEnumerable<Instruction> instructions) : base(instructions)
        {
            this.Expression = selectExpression;
        }

        public override bool Equals(Expression other)
        {
            SelectClause selectClause = other as SelectClause;
            return selectClause != null && this.Expression.Equals(selectClause.Expression);
        }

        public override Expression Clone()
        {
            return new SelectClause(Expression.Clone(), this.instructions);
        }

        public override Expression CloneExpressionOnly()
        {
            return new SelectClause(Expression.CloneExpressionOnly(), null);
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.SelectClause; }
        }

        public override IEnumerable<ICodeNode> Children
        {
            get { yield return Expression; }
        }
    }
}
