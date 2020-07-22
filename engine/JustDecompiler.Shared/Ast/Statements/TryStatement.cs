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

namespace Telerik.JustDecompiler.Ast.Statements
{
    public class TryStatement : Statement
    {
        private BlockStatement @try;
        private CatchClauseCollection catchClauses = new CatchClauseCollection();
        private BlockStatement fault;
        private FinallyClause @finally;

        public TryStatement()
        {
        }

        public TryStatement(BlockStatement @try, BlockStatement fault, FinallyClause @finally)
        {
            this.Try = @try;
            this.Fault = fault;
            this.Finally = @finally;
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                if (@try != null)
                {
                    yield return @try;
                }
                if (@finally != null)
                {
                    yield return @finally;
                }
                if (fault != null)
                {
                    yield return @fault;
                }
                foreach (CatchClause cc in CatchClauses)
                {
                    yield return cc;
                }
            }
        }

		public override Statement Clone()
		{
            BlockStatement clonnedTry = @try != null ? @try.Clone() as BlockStatement : null;
            BlockStatement clonnedfault = fault != null ? fault.Clone() as BlockStatement : null;
            FinallyClause clonnedFinally = @finally != null ? @finally.Clone() as FinallyClause : null;
            TryStatement result = new TryStatement(clonnedTry, clonnedfault, clonnedFinally);
            foreach (CatchClause @catch in this.catchClauses)
            {
                result.AddToCatchClauses((CatchClause)@catch.Clone());
            }
            CopyParentAndLabel(result);
            return result;
        }

        public override Statement CloneStatementOnly()
        {
            BlockStatement clonnedTry = @try != null ? @try.CloneStatementOnly() as BlockStatement : null;
            BlockStatement clonnedfault = fault != null ? fault.CloneStatementOnly() as BlockStatement : null;
            FinallyClause clonnedFinally = @finally != null ? @finally.CloneStatementOnly() as FinallyClause : null;
            TryStatement result = new TryStatement(clonnedTry, clonnedfault, clonnedFinally);
            foreach (CatchClause @catch in this.catchClauses)
            {
                result.AddToCatchClauses((CatchClause)@catch.CloneStatementOnly());
            }
            CopyParentAndLabel(result);
            return result;
        }

        public BlockStatement Try
        {
            get { return @try; }
            set
            {
                this.@try = value;
                if (this.@try != null)
                {
                    this.@try.Parent = this;
                }
            }
        }

        public CatchClauseCollection CatchClauses
        {
            get { return catchClauses; }
            set
            {
                this.catchClauses = value;
                foreach (CatchClause catchClause in this.catchClauses)
                {
                    SetParentToThis(catchClause);
                }
            }
        }

        public void AddToCatchClauses(CatchClause catchClause)
        {
            SetParentToThis(catchClause);
            CatchClauses.Add(catchClause);
        }

        private void SetParentToThis(CatchClause catchClause)
        {
            if (catchClause.Body != null)
            {
                catchClause.Body.Parent = this;
            }
            if (catchClause.Filter != null)
            {
                catchClause.Filter.Parent = this;
            }
        }

        public BlockStatement Fault
        {
            get { return fault; }
            set
            {
                this.fault = value;
                if (this.fault != null)
                {
                    this.fault.Parent = this;
                }
            }
        }

        public FinallyClause Finally
        {
            get { return @finally; }
            set
            {
                this.@finally = value;
                if (this.@finally != null)
                {
                    this.@finally.Parent = this;
                }
            }
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.TryStatement; }
        }
    }
}