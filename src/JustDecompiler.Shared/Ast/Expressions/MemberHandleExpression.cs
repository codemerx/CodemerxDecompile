using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class MemberHandleExpression : Expression
    {
        public MemberReference MemberReference { get; private set; }

        public MemberHandleExpression(MemberReference memberRef, IEnumerable<Mono.Cecil.Cil.Instruction> instructions)
            : base(instructions)
        {
            this.MemberReference = memberRef;
        }

        public override bool Equals(Expression other)
        {
            MemberHandleExpression otherExpression = other as MemberHandleExpression;
            return otherExpression != null && this.MemberReference.FullName == otherExpression.MemberReference.FullName;
        }

        public override Expression Clone()
        {
            return new MemberHandleExpression(this.MemberReference, this.instructions);
        }

        public override Expression CloneExpressionOnly()
        {
            return new MemberHandleExpression(this.MemberReference, null);
        }

        public override CodeNodeType CodeNodeType
        {
            get { return Ast.CodeNodeType.MemberHandleExpression; }
        }

        public override IEnumerable<ICodeNode> Children
        {
            get { return Enumerable.Empty<ICodeNode>(); }
        }
    }
}
