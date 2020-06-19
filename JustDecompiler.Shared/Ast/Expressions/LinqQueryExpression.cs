using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class LinqQueryExpression : Expression
    {
        public List<QueryClause> Clauses { get; set; }

        public LinqQueryExpression(List<QueryClause> clauses, TypeReference linqQueryType, IEnumerable<Instruction> instructions) : base(instructions)
        {
            this.Clauses = clauses;
            this.ExpressionType = linqQueryType;
        }

        public override bool Equals(Expression other)
        {
            LinqQueryExpression linqExpression = other as LinqQueryExpression;
            if (linqExpression == null || this.Clauses.Count != linqExpression.Clauses.Count)
            {
                return false;
            }

            for (int i = 0; i < this.Clauses.Count; i++)
            {
                if (!this.Clauses[i].Equals(linqExpression.Clauses[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override Expression Clone()
        {
            return new LinqQueryExpression(new List<QueryClause>(this.Clauses.Select(clause => (QueryClause)clause.Clone())), this.ExpressionType, this.instructions);
        }

        public override Expression CloneExpressionOnly()
        {
            return new LinqQueryExpression(new List<QueryClause>(this.Clauses.Select(clause => (QueryClause)clause.CloneExpressionOnly())), this.ExpressionType, null);
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.LinqQueryExpression; }
        }

        public override IEnumerable<ICodeNode> Children
        {
            get { yield break; }
        }
    }
}
