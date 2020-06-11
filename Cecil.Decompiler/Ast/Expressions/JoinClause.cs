using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class JoinClause : QueryClause
    {
        public Expression InnerIdentifier { get; set; }
        public Expression InnerCollection { get; set; }
        public Expression OuterKey { get; set; }
        public Expression InnerKey { get; set; }

        public JoinClause(Expression innerIdentifier, Expression innerCollection, Expression outerKey, Expression innerKey, IEnumerable<Instruction> instructions) : base(instructions)
        {
            this.InnerIdentifier = innerIdentifier;
            this.InnerCollection = innerCollection;
            this.OuterKey = outerKey;
            this.InnerKey = innerKey;
        }

        public override bool Equals(Expression other)
        {
            JoinClause joinClause = other as JoinClause;
            return joinClause != null &&
                this.InnerIdentifier.Equals(joinClause.InnerIdentifier) &&
                this.InnerCollection.Equals(joinClause.InnerCollection) &&
                this.OuterKey.Equals(joinClause.OuterKey) &&
                this.InnerKey.Equals(joinClause.InnerKey);
        }

        public override Expression Clone()
        {
            return new JoinClause(this.InnerIdentifier.Clone(), this.InnerCollection.Clone(),
                this.OuterKey.Clone(), this.InnerKey.Clone(), instructions);
        }

        public override Expression CloneExpressionOnly()
        {
            return new JoinClause(this.InnerIdentifier.CloneExpressionOnly(), this.InnerCollection.CloneExpressionOnly(),
                this.OuterKey.CloneExpressionOnly(), this.InnerKey.CloneExpressionOnly(), null);
        }

        public override CodeNodeType CodeNodeType
        {
            get { return Ast.CodeNodeType.JoinClause; }
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                yield return InnerIdentifier;
                yield return InnerCollection;
                yield return OuterKey;
                yield return InnerKey;
            }
        }
    }
}
