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
    public class ThrowExpression : Expression
    {
        public ThrowExpression(Expression expression, IEnumerable<Instruction> instructions)
            :base(instructions)
        {
            this.Expression = expression;
        }

        public Expression Expression { get; set; }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                if (Expression != null)
                {
                    yield return Expression;
                }
            }
        }

		public override bool Equals(Expression other)
		{
            if (!(other is ThrowExpression)) 
            {
                return false;
            }
            return this.Expression.Equals((other as ThrowExpression).Expression);
        }

        public override Expression Clone()
        {
            Expression clonedExpression = this.Expression != null ? this.Expression.Clone() : null;
			return new ThrowExpression(clonedExpression, instructions);
        }

        public override Expression CloneExpressionOnly()
        {
            Expression clonedExpression = this.Expression != null ? this.Expression.CloneExpressionOnly() : null;
            return new ThrowExpression(clonedExpression, null);
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
                throw new NotSupportedException("Throw expressions have no type.");
            }
            set
            {
                throw new NotSupportedException("Throw expressions cannot have type.");
            }
        }

        public override CodeNodeType CodeNodeType
        {
            get
            {
                return CodeNodeType.ThrowExpression;
            }
        }
    }
}