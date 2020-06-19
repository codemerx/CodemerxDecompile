using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class IntoClause : QueryClause
    {
        public VariableReferenceExpression Identifier { get; set; }

        public IntoClause(VariableReferenceExpression identifier, IEnumerable<Instruction> instructions) : base(instructions)
        {
            this.Identifier = identifier;
        }

        public override bool Equals(Expression other)
        {
            IntoClause intoClause = other as IntoClause;
            return intoClause != null && this.Identifier.Equals(intoClause.Identifier);
        }

        public override Expression Clone()
        {
            return new IntoClause((VariableReferenceExpression)this.Identifier.Clone(), this.instructions);
        }

        public override Expression CloneExpressionOnly()
        {
            return new IntoClause((VariableReferenceExpression)this.Identifier.CloneExpressionOnly(), null);
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.IntoClause; }
        }

        public override IEnumerable<ICodeNode> Children
        {
            get { yield return this.Identifier; }
        }
    }
}
