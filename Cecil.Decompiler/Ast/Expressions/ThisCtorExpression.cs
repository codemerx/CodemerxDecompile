using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class ThisCtorExpression : MethodInvocationExpression
    {
        public ThisCtorExpression(MethodReferenceExpression methodReferenceExpression, IEnumerable<Instruction> instructions)
            : base(methodReferenceExpression, instructions)
        {
        }

        public Expression InstanceReference { get; set; }

        public override bool Equals(Expression other)
        {
            if (!(other is ThisCtorExpression)) 
            {
                return false;
            }
            return base.Equals(other);
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                if(this.InstanceReference != null)
                {
                    yield return this.InstanceReference;
                }

                foreach (ICodeNode child in base.Children)
                {
                    yield return child;
                }
            }
        }

        public override Expression Clone()
        {
            ThisCtorExpression result = new ThisCtorExpression(this.MethodExpression.Clone() as MethodReferenceExpression, instructions);
            result.Arguments = this.Arguments.Clone();
            result.VirtualCall = this.VirtualCall;
            result.InstanceReference = this.InstanceReference != null ? this.InstanceReference.Clone() : null;
			return result;
        }

        public override Expression CloneExpressionOnly()
        {
            ThisCtorExpression result = new ThisCtorExpression(this.MethodExpression.CloneExpressionOnly() as MethodReferenceExpression, instructions)
                { Arguments = this.Arguments.CloneExpressionsOnly(), VirtualCall = this.VirtualCall,
                    InstanceReference = this.InstanceReference != null ? this.InstanceReference.CloneExpressionOnly() : null };
            return result;
        }

        public override bool HasType
        {
            get
            {
                return false;
            }
        }

        public override TypeReference ExpressionType
        {
            get
            {
                throw new NotSupportedException("This constructor expression has no type.");
            }
            set
            {
                throw new NotSupportedException("This constructor expression cannot have type.");
            }
        }

        public override CodeNodeType CodeNodeType
        {
            get
            {
                return CodeNodeType.ThisCtorExpression;
            }
        }
    }
}
