using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Common;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class OrderByClause : QueryClause
    {
        public PairList<Expression, OrderDirection> ExpressionToOrderDirectionMap { get; set; }

        public OrderByClause(PairList<Expression, OrderDirection> expressionToOrderMap, IEnumerable<Instruction> instructions) : base(instructions)
        {
            this.ExpressionToOrderDirectionMap = expressionToOrderMap;
        }

        public override bool Equals(Expression other)
        {
            OrderByClause orderByClause = other as OrderByClause;
            if (orderByClause == null || this.ExpressionToOrderDirectionMap.Count != orderByClause.ExpressionToOrderDirectionMap.Count)
            {
                return false;
            }

            for (int i = 0; i < this.ExpressionToOrderDirectionMap.Count; i++)
            {
                KeyValuePair<Expression, OrderDirection> thisPair = this.ExpressionToOrderDirectionMap[i];
                KeyValuePair<Expression, OrderDirection> otherPair = orderByClause.ExpressionToOrderDirectionMap[i];

                if (!thisPair.Key.Equals(otherPair.Key) || thisPair.Value != otherPair.Value)
                {
                    return false;
                }
            }

            return true;
        }

        public override Expression Clone()
        {
            PairList<Expression, OrderDirection> collectionClone =
                new PairList<Expression, OrderDirection>(this.ExpressionToOrderDirectionMap.Select(pair => new KeyValuePair<Expression, OrderDirection>(pair.Key.Clone(), pair.Value)));
            return new OrderByClause(collectionClone, instructions);
        }

        public override Expression CloneExpressionOnly()
        {
            PairList<Expression, OrderDirection> collectionClone =
                new PairList<Expression, OrderDirection>(this.ExpressionToOrderDirectionMap.Select(pair => new KeyValuePair<Expression, OrderDirection>(pair.Key.CloneExpressionOnly(), pair.Value)));
            return new OrderByClause(collectionClone, null);
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.OrderByClause; }
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                return this.ExpressionToOrderDirectionMap.Select(pair => (ICodeNode)pair.Key);
            }
        }
    }
}
