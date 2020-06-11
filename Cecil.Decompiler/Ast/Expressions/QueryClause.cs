using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public abstract class QueryClause : Expression
    {
        public QueryClause(IEnumerable<Mono.Cecil.Cil.Instruction> instructions) : base(instructions)
        {
        }

        public override bool HasType
        {
            get
            {
                return false;
            }
        }

        public override Mono.Cecil.TypeReference ExpressionType
        {
            get
            {
                throw new NotSupportedException("Query clauses have no type.");
            }
            set
            {
                throw new NotSupportedException("Query clauses cannot have type.");
            }
        }
    }
}
