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
using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Ast.Statements
{
    public class BlockStatement : Statement
    {
        private StatementCollection statements = new StatementCollection();

		public override Statement Clone()
		{
            BlockStatement result = new BlockStatement();
            CopyParentAndLabel(result);
            foreach (Statement s in statements)
            {
                result.AddStatement(s.Clone());
            }
            return result;
        }

        public override Statement CloneStatementOnly()
        {
            BlockStatement result = new BlockStatement();
            CopyParentAndLabel(result);
            foreach (Statement s in statements)
            {
                result.AddStatement(s.CloneStatementOnly());
            }
            return result;
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                foreach (ICodeNode statement in statements)
                {
                    yield return statement;
                }
            }
        }

        public StatementCollection Statements
        {
            get { return statements; }
            set
            {
                this.statements = value;
                foreach (Statement statement in this.statements)
                {
                    statement.Parent = this;
                }
            }
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.BlockStatement; }
        }

        public void AddStatement(Statement statement)
        {
            AddStatementAt(statements.Count, statement);
        }

        public void AddStatementAt(int index, Statement statement)
        {
            this.statements.Insert(index, statement);
            statement.Parent = this;
        }

#if DEBUG
        public override string ToString()
        {
            using (System.IO.StringWriter statementDecompilerStrWriter = new System.IO.StringWriter())
            {
                Telerik.JustDecompiler.Languages.ILanguageTestCaseWriter statementDecompilerLanguageWriter = 
					new Telerik.JustDecompiler.Languages.TestCaseWriters.IntermediateDecompilationCSharpLanguageWriter(new Telerik.JustDecompiler.Languages.PlainTextFormatter(statementDecompilerStrWriter));
                statementDecompilerLanguageWriter.Write(this);
                return statementDecompilerStrWriter.ToString();
            }
        }
#endif
    }
}