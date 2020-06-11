using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class FromClause : QueryClause
    {
        public Expression Identifier { get; set; }
        public Expression Collection { get; set; }

        public FromClause(Expression identifier, Expression collectionExpression, IEnumerable<Instruction> instructions) : base(instructions)
        {
            this.Identifier = identifier;
            this.Collection = collectionExpression;
        }

        public override bool Equals(Expression other)
        {
            FromClause fromClause = other as FromClause;
            return fromClause != null && this.Identifier.Equals(fromClause.Identifier) && this.Collection.Equals(fromClause.Collection);
        }

        public override Expression Clone()
        {
            return new FromClause(this.Identifier.Clone(), this.Collection.Clone(), this.instructions);
        }

        public override Expression CloneExpressionOnly()
        {
            return new FromClause(this.Identifier.CloneExpressionOnly(), this.Collection.CloneExpressionOnly(), null);
        }

        public override CodeNodeType CodeNodeType
        {
            get { return Ast.CodeNodeType.FromClause; }
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                yield return Identifier;
                yield return Collection;
            }
        }
    }
}
