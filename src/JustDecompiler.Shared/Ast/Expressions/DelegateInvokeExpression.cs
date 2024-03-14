#region license
//
//	(C) 2005 - 2007 db4objects Inc. http://www.db4o.com
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion
using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class DelegateInvokeExpression : Expression
    {
		public DelegateInvokeExpression(Expression target, ExpressionCollection arguments, MethodReference invokeMethodReference, IEnumerable<Instruction> instructions)
            :base(instructions)
		{
            this.Target = target;
            this.Arguments = arguments;
            this.InvokeMethodReference = invokeMethodReference;
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                yield return Target;
                foreach (ICodeNode argument in Arguments)
                {
                    yield return argument;
                }
            }
        }

		public override bool Equals(Expression other)
		{
            if (!(other is DelegateInvokeExpression))
            {
                return false;
            }
            DelegateInvokeExpression delegateInvoke = other as DelegateInvokeExpression;

            if (this.Target == null)
            {
                if (delegateInvoke.Target != null)
                {
                    return false;
                }
            }
            else if (!this.Target.Equals(delegateInvoke.Target))
            {
                return false;
            }

            return this.Arguments.Equals(delegateInvoke.Arguments);
        }

        public override Expression Clone()
        {
            DelegateInvokeExpression result = new DelegateInvokeExpression(Target.Clone(), Arguments.Clone(), InvokeMethodReference, instructions);
			return result;
        }

        public override Expression CloneExpressionOnly()
        {
            DelegateInvokeExpression result = new DelegateInvokeExpression(Target.CloneExpressionOnly(), Arguments.CloneExpressionsOnly(),
                InvokeMethodReference, null);
            return result;
        }

        public override TypeReference ExpressionType
        {
            get
            {
                return this.InvokeMethodReference.FixedReturnType;
            }
            set
            {
                throw new NotSupportedException("Expression type of delegate invocation expression can not be changed.");
            }
        }

        public Expression Target { get; set; }

        public ExpressionCollection Arguments { get; set; }

        public MethodReference InvokeMethodReference { get; set; }

        public override CodeNodeType CodeNodeType
        {
            get
            {
                return CodeNodeType.DelegateInvokeExpression;
            }
        }
    }
}