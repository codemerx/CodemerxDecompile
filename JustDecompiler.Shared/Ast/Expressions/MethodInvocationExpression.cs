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
// Warning: generated do not edit
using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class MethodInvocationExpression : Expression
    {
        public bool VirtualCall { get; set; }

        public TypeReference ConstraintType { get; set; }

        public bool IsConstrained
        {
            get
            {
                return this.ConstraintType != null;
            }
        }

        public ExpressionCollection Arguments { get; set; }

        public MethodReferenceExpression MethodExpression { get; set; }

        public MethodInvocationExpression(MethodReferenceExpression method, IEnumerable<Instruction> instructions)
            :base(instructions)
        {
            this.MethodExpression = method;
            this.Arguments = new ExpressionCollection();
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                yield return MethodExpression;
                foreach (ICodeNode argument in Arguments)
                {
                    yield return argument;
                }
            }
        }

        public IList<Instruction> InvocationInstructions
        {
            get
            {
                List<Instruction> result = new List<Instruction>(this.instructions);
                result.AddRange(this.MethodExpression.MappedInstructions);
                return result;
            }
        }

		public override bool Equals(Expression other)
		{
            if (!(other is MethodInvocationExpression))
            {
                return false;
            }
            MethodInvocationExpression methodInvocation = other as MethodInvocationExpression;
            if (!this.MethodExpression.Equals(methodInvocation.MethodExpression))
            {
                return false;
            }

            return this.VirtualCall == methodInvocation.VirtualCall && this.Arguments.Equals(methodInvocation.Arguments) &&
                this.ConstraintType == methodInvocation.ConstraintType;
        }

        public override Expression Clone()
        {
            MethodInvocationExpression result = new MethodInvocationExpression(MethodExpression.Clone() as MethodReferenceExpression, this.instructions);
            result.Arguments = Arguments.Clone();
            result.VirtualCall = this.VirtualCall;
            result.ConstraintType = this.ConstraintType;
			return result;
        }

        public override Expression CloneExpressionOnly()
        {
            MethodInvocationExpression result = new MethodInvocationExpression(MethodExpression.CloneExpressionOnly() as MethodReferenceExpression, null)
                { Arguments = this.Arguments.CloneExpressionsOnly(), VirtualCall = this.VirtualCall, ConstraintType = this.ConstraintType };
            return result;
        }

        public override TypeReference ExpressionType
        {
            get
            {
                return MethodExpression.ExpressionType;
            }
            set
            {
                throw new NotSupportedException("Expression type of method invocation expression can not be changed.");
            }
        }

        public override bool HasType
        {
            get
            {
                return true;
            }
        }

        public override CodeNodeType CodeNodeType
        {
            get
            {
                return CodeNodeType.MethodInvocationExpression;
            }
        }

        public Expression GetTarget()
        {
            if (this.MethodExpression.MethodDefinition != null &&
                this.MethodExpression.MethodDefinition.IsExtensionMethod)
            {
                if (this.Arguments.Count < 1)
                {
                    throw new Exception("Extension methods invocations should have at least 1 argument.");
                }

                return this.Arguments[0];
            }
            else
            {
                return this.MethodExpression.Target;
            }
        }

        public bool IsByReference
        {
            get
            {
                return this.MethodExpression.Method.ReturnType.IsByReference;
            }
        }
    }
}