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
using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class SwitchStatement : ConditionStatement
	{
		private SwitchCaseCollection cases = new SwitchCaseCollection ();
        private SwitchCase[] casesAsArray;
        private bool needsRefreshing;
		private readonly Instruction switchInstruction;

        public Instruction SwitchInstruction
        {
            get { return switchInstruction; }
        }

        public SwitchStatement(Expression condition, Instruction instruction) : base(condition)
        {
            needsRefreshing = true;
			this.switchInstruction = instruction;
		}

        protected override IEnumerable<Instruction> GetOwnInstructions()
        {
            if (switchInstruction != null)
            {
                yield return switchInstruction;
            }
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                yield return this.Condition;
                foreach (SwitchCase @case in this.Cases)
                {
                    yield return @case;
                }
            }
        }

        public override Statement Clone()
        {
            return CloneStatement(true);
        }

        public override Statement CloneStatementOnly()
        {
            return CloneStatement(false);
        }

        private Statement CloneStatement(bool copyInstructions)
        {
            SwitchStatement result = copyInstructions ? new SwitchStatement(Condition.Clone(), switchInstruction) : 
                new SwitchStatement(Condition.CloneExpressionOnly(), null);

            foreach (SwitchCase sc in this.cases)
            {
                result.AddCase((SwitchCase)(copyInstructions ? sc.Clone() : sc.CloneStatementOnly()));
            }

            CopyParentAndLabel(result);
            return result;
        }

        public IEnumerable<SwitchCase> Cases
        {
			get 
            {
                if (needsRefreshing)
                {
                    casesAsArray = new SwitchCase[cases.Count];
                    int i = 0;
                    foreach (SwitchCase @case in cases)
                    {
                        casesAsArray[i++] = @case;
                    }
                    needsRefreshing = false;
                }
                return casesAsArray; 
            }
			set 
            { 
                this.cases = new SwitchCaseCollection();
                foreach (SwitchCase switchCase in value)
                {
                    AddCase(switchCase);
                }
                needsRefreshing = true;
            }
		}

        public void AddCase(SwitchCase @case)
        {
            cases.Add(@case);
            @case.Parent = this;
            needsRefreshing = true;
        }

		public override CodeNodeType CodeNodeType
		{
			get { return CodeNodeType.SwitchStatement; }
		}
	}
}