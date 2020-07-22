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
using System.Collections.Generic;
using Mono.Cecil;
using System;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class CanCastExpression : Expression
    {
		private Expression expression;

		public CanCastExpression(Expression expression, TypeReference targetType, IList<Instruction> instructions)
            :base(instructions)
		{
            this.Expression = expression;
            this.TargetType = targetType;
        }

        public override IEnumerable<ICodeNode> Children
        {
            get { yield return Expression; }
        }

		public override bool Equals(Expression other)
		{
            if (!(other is CanCastExpression))
            {
                return false;
            }
            CanCastExpression canCast = other as CanCastExpression;
            return this.TargetType.FullName == canCast.TargetType.FullName && this.Expression.Equals(canCast.Expression);
        }

        public override Expression Clone()
        {
			CanCastExpression result = new CanCastExpression(Expression.Clone(), TargetType, instructions);
			return result;
        }

        public override Expression CloneExpressionOnly()
        {
            CanCastExpression result = new CanCastExpression(Expression.CloneExpressionOnly(), TargetType, null);
            return result;
        }

		public Expression Expression
		{
			get
			{
				return expression;
			}
			set
			{
				expression = value;
			}
		}

        public TypeReference TargetType { get; set; }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.CanCastExpression; }
        }

        public override TypeReference ExpressionType
        {
            get
            {
                return TargetType.Module.TypeSystem.Boolean;
            }
            set
            {
                throw new NotSupportedException("Can cast expression is always boolean.");
            }
        }

        public override bool HasType
        {
            get
            {
                return true;
            }
        }
    }
}