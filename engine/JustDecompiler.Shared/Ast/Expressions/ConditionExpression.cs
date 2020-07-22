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
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class ConditionExpression : Expression
    {
        public ConditionExpression(Expression condition, Expression then, Expression @else, IEnumerable<Instruction> instructions)
            :base(instructions)
        {
            this.Condition = condition;
            this.Then = then;
            this.Else = @else;
		}

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                yield return Condition;
                yield return Then;
                yield return Else;
            }
        }

		public override bool Equals(Expression other)
		{
            ConditionExpression otherCondition = other as ConditionExpression;
            if (otherCondition == null)
            {
                return false;
            }

            return this.Condition.Equals(otherCondition.Condition) && this.Then.Equals(otherCondition.Then) && this.Else.Equals(otherCondition.Else);
        }

        public override Expression Clone()
        {
            ConditionExpression result = new ConditionExpression(Condition.Clone(), Then.Clone(), Else.Clone(), instructions);
			return result;
        }

        public override Expression CloneExpressionOnly()
        {
            ConditionExpression result = new ConditionExpression(Condition.CloneExpressionOnly(), Then.CloneExpressionOnly(), Else.CloneExpressionOnly(), null);
            return result;
        }

        public Expression Condition { get; set; }

        public Expression Then { get; set; }

        public Expression Else { get; set; }

        public override TypeReference ExpressionType
        {
            get
            {
				return Then.ExpressionType; // the then and else should be of the same type
            }
        }

        public override bool HasType
        {
            get
            {
                return Then.HasType; // the then and else should be of the same type
            }
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.ConditionExpression; }
        }
    }
}