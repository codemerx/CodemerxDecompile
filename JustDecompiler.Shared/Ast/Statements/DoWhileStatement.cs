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
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class DoWhileStatement : ConditionStatement
	{
		private BlockStatement body;

        public DoWhileStatement(Expression condition, BlockStatement body) : base(condition)
        {
            this.Body = body;
		}

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                yield return Condition;
                if (this.body != null)
                {
                    yield return this.body;
                }
            }
        }

		public override Statement Clone()
		{
            BlockStatement clonedBody = body != null ? body.Clone() as BlockStatement : null;
            DoWhileStatement result = new DoWhileStatement(this.Condition.Clone() , clonedBody);
            result.ConditionBlock = this.ConditionBlock;
            CopyParentAndLabel(result);
            return result;
        }

        public override Statement CloneStatementOnly()
        {
            BlockStatement clonedBody = body != null ? body.CloneStatementOnly() as BlockStatement : null;
            DoWhileStatement result = new DoWhileStatement(this.Condition.CloneExpressionOnly(), clonedBody);
            result.ConditionBlock = null;
            CopyParentAndLabel(result);
            return result;
        }

		public BlockStatement Body
		{
			get { return body; }
            set
            {
                this.body = value;
                if(this.body != null)
                {
                    this.body.Parent = this;
                }
            }
		}

		public override CodeNodeType CodeNodeType
		{
			get { return CodeNodeType.DoWhileStatement; }
		}
	}
}