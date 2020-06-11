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
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class MethodReferenceExpression : MemberReferenceExpresion
    {
        private MethodDefinition methodDefinition;
        private bool resolved = false;
        private TypeReference expressionType;

		public MethodReferenceExpression(Expression target, MethodReference method, IEnumerable<Instruction> instructions)
			: base(target, method, instructions)
		{
			/// This constructor is used by HandleVirtualMethodInvocations step.
			/// As such, the <paramref="instruction"> argument doesn't contain a reference to the method
			/// Instead, it is usually a call to a method in a parent class. Thus the operand of the instruction must be ignored.
			this.expressionType = null;
			this.Target = target;
			this.Method = method;
			this.Member = method;
		}

        public MethodReference Method { get; set; }

        public override bool Equals(Expression other)
        {
            if (other == null)
            {
                return false;
            }
            if (other is MethodReferenceExpression == false)
            {
                return false;
            }
            MethodReferenceExpression otherRef = other as MethodReferenceExpression;

            #region CompareTargets
            if (this.Target == null)
            {
                if (otherRef.Target != null)
                {
                    return false;
                }
            }
            else if (!this.Target.Equals(otherRef.Target))
            {
                return false;
            }
            #endregion
            /// At this point we know that either both targets are null, or both targets are equal expressions
            #region MethodSignature
            if (this.Method.FullName != otherRef.Method.FullName)
            {
                return false;
            }
            #endregion

            return true;
        }

        public override Expression Clone()
        {
            Expression targetClone = Target != null ? Target.Clone() : null;
            MethodReferenceExpression result = new MethodReferenceExpression(targetClone, this.Method, this.instructions);
            CopyFields(result);
			return result;
        }

        public override Expression CloneExpressionOnly()
        {
            Expression targetClone = Target != null ? Target.CloneExpressionOnly() : null;
            MethodReferenceExpression result = new MethodReferenceExpression(targetClone, this.Method, null);
            CopyFields(result);
            return result;
        }

        private void CopyFields(MethodReferenceExpression clone)
        {
            clone.resolved = this.resolved;
            clone.methodDefinition = this.methodDefinition;
            clone.expressionType = this.expressionType;
        }

        /// <summary>
        /// Gets the resolved method definition.
        /// </summary>
        public MethodDefinition MethodDefinition
        {
            get
            {
                if (!resolved)
                {
                    this.methodDefinition = Method.Resolve();
                    resolved = true;
                }
                return methodDefinition;
            }
        }

        public override TypeReference ExpressionType
        {
            get
            {
                if (this.expressionType == null)
                {
                    this.expressionType = Method.FixedReturnType;
                }
                return expressionType;
            }
            set
            {
                throw new System.NotSupportedException("Type of MethodReferenceExpression can not be changed.");
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
                return CodeNodeType.MethodReferenceExpression;
            }
        }
    }
}
