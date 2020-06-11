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
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class CatchClause : BasePdbStatement
	{
		public CatchClause()
		{
		}

        public CatchClause(BlockStatement body, TypeReference type, VariableDeclarationExpression variable, Statement filter = null)
        {
            this.Body = body;
            this.Type = type;
            this.Variable = variable;
            this.Filter = filter;
		}

		public Statement Filter { get; set; }

		public BlockStatement Body { get; set; }

		public TypeReference Type { get; set; }

		public VariableDeclarationExpression Variable { get; set; }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                if (Variable != null)
                {
                    yield return Variable;
                }
                if (Filter != null)
                {
                    yield return Filter;
                }
                if (Body != null)
                {
                    yield return Body;
                }
            }
        }

		public override CodeNodeType CodeNodeType
		{
			get { return CodeNodeType.CatchClause; }
		}

        public override Statement Clone()
        {
            BlockStatement bodyClone = Body != null ? (BlockStatement)Body.Clone() : null;
            VariableDeclarationExpression variableClone = Variable != null ? (VariableDeclarationExpression)Variable.Clone() : null;
            Statement filterClone = Filter != null ? (Statement)Filter.Clone() : null;

            return new CatchClause(bodyClone, Type, variableClone, filterClone);
        }

        public override Statement CloneStatementOnly()
        {
            BlockStatement bodyClone = Body != null ? (BlockStatement)Body.CloneStatementOnly() : null;
            VariableDeclarationExpression variableClone = Variable != null ? (VariableDeclarationExpression)Variable.CloneExpressionOnly() : null;
            Statement filterClone = Filter != null ? (Statement)Filter.CloneStatementOnly() : null;

            return new CatchClause(bodyClone, Type, variableClone, filterClone);
        }
    }
}
