using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class WhereClause : QueryClause
    {
        public Expression Condition { get; set; }

        public WhereClause(Expression condition, IEnumerable<Instruction> instructions) : base(instructions)
        {
            this.Condition = condition;
        }

        public override bool Equals(Expression other)
        {
            WhereClause whereClause = other as WhereClause;
            return whereClause != null && this.Condition.Equals(whereClause.Condition);
        }

        public override Expression Clone()
        {
            return new WhereClause(this.Condition.Clone(), instructions);
        }

        public override Expression CloneExpressionOnly()
        {
            return new WhereClause(this.Condition.CloneExpressionOnly(), null);
        }

        public override CodeNodeType CodeNodeType
        {
            get { return Ast.CodeNodeType.WhereClause; }
        }

        public override IEnumerable<ICodeNode> Children
        {
            get { yield return Condition; }
        }
    }
}
