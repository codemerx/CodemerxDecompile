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
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
    public class ForStatement : ConditionStatement
    {
        private BlockStatement body;

        public ForStatement(Expression initializer, Expression condition, Expression increment, BlockStatement body)
            : base(condition)
        {
            this.Initializer = initializer;
            this.Increment = increment;
            this.Body = body;
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                yield return Condition;
                if (Initializer != null)
                {
                    yield return Initializer;
                }
                if (Increment != null)
                {
                    yield return Increment;
                }
                if (this.Body != null)
                {
                    yield return Body;
                }
            }
        }

        public override Statement Clone()
        {
            BlockStatement clonedBody = body != null ? body.Clone() as BlockStatement : null;
            ForStatement result = new ForStatement(Initializer.Clone(), Condition.Clone(), Increment.Clone(), clonedBody);
            result.ConditionBlock = this.ConditionBlock;
            CopyParentAndLabel(result);
            return result;
        }

        public override Statement CloneStatementOnly()
        {
            BlockStatement clonedBody = body != null ? body.CloneStatementOnly() as BlockStatement : null;
            ForStatement result = new ForStatement(Initializer.CloneExpressionOnly(), Condition.CloneExpressionOnly(), Increment.CloneExpressionOnly(), clonedBody);
            result.ConditionBlock = null;
            CopyParentAndLabel(result);
            return result;
        }

        public Expression Initializer { get; set; }

        public Expression Increment { get; set; }

        public BlockStatement Body
        {
            get { return this.body; }
            set
            {
                this.body = value;
                if (this.body != null)
                {
                    this.body.Parent = this;
                }
            }
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.ForStatement; }
        }
    }
}