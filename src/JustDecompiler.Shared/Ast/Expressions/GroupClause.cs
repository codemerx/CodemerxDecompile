using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class GroupClause : QueryClause
    {
        public Expression Expression { get; set; }
        public Expression GroupKey { get; set; }

        public GroupClause(Expression expression, Expression key, IEnumerable<Instruction> instructions) : base(instructions)
        {
            this.Expression = expression;
            this.GroupKey = key;
        }

        public override bool Equals(Expression other)
        {
            GroupClause groupClause = other as GroupClause;
            return groupClause != null && this.Expression.Equals(groupClause.Expression) && this.GroupKey.Equals(groupClause.GroupKey);
        }

        public override Expression Clone()
        {
            return new GroupClause(this.Expression.Clone(), this.GroupKey.Clone(), instructions);
        }

        public override Expression CloneExpressionOnly()
        {
            return new GroupClause(this.Expression.CloneExpressionOnly(), this.GroupKey.CloneExpressionOnly(), null);
        }

        public override CodeNodeType CodeNodeType
        {
            get { return Ast.CodeNodeType.GroupClause; }
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                yield return Expression;
                yield return GroupKey;
            }
        }
    }
}
