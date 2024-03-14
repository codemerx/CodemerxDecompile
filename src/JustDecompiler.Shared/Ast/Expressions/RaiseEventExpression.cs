using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class RaiseEventExpression : Expression
    {
        public RaiseEventExpression(EventReference @event, MethodReference invokeMethodReference, ExpressionCollection arguments, IEnumerable<Instruction> instructions)
            : base(instructions)
        {
            this.Event = @event;
            this.InvokeMethodReference = invokeMethodReference;
            this.Arguments = arguments;
        }

        public EventReference Event { get; set; }

        public MethodReference InvokeMethodReference { get; set; }

        public ExpressionCollection Arguments { get; set; }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                return this.Arguments;
            }
        }

        public override CodeNodeType CodeNodeType
        {
            get
            {
                return CodeNodeType.RaiseEventExpression;
            }
        }

        public override Expression Clone()
        {
            return new RaiseEventExpression(this.Event, this.InvokeMethodReference, this.Arguments.Clone(), instructions);
        }

        public override Expression CloneExpressionOnly()
        {
            return new RaiseEventExpression(this.Event, this.InvokeMethodReference, this.Arguments.CloneExpressionsOnly(), null);
        }

        public override bool Equals(Expression other)
        {
            if (other.CodeNodeType != CodeNodeType.RaiseEventExpression)
            {
                return false;
            }

            RaiseEventExpression raiseEvent = other as RaiseEventExpression;

            return this.Event.FullName == raiseEvent.Event.FullName &&
                   this.InvokeMethodReference.FullName == raiseEvent.InvokeMethodReference.FullName &&
                   this.Arguments.Equals(raiseEvent.Arguments);
        }
    }
}
