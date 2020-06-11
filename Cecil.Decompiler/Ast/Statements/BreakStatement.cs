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
    public class BreakStatement : BasePdbStatement
    {
		private readonly List<Instruction> breakJumps;

		public BreakStatement(IEnumerable<Instruction> jumps)
		{
			this.breakJumps = new List<Instruction>();
			if (jumps != null)
			{
				this.breakJumps.AddRange(jumps);
                this.breakJumps.Sort((x, y) => x.Offset.CompareTo(y.Offset));
			}
		}

        public override IEnumerable<ICodeNode> Children
        {
            get { yield break; }
        }

        protected override IEnumerable<Instruction> GetOwnInstructions()
        {
            if (breakJumps != null)
            {
                foreach (Instruction instruction in breakJumps)
                {
                    yield return instruction;
                }
            }
        }

		public override Statement Clone()
		{
            BreakStatement result = new BreakStatement(breakJumps);
            CopyParentAndLabel(result);
            return result;
        }

        public override Statement CloneStatementOnly()
        {
            BreakStatement result = new BreakStatement(null);
            CopyParentAndLabel(result);
            return result;
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.BreakStatement; }
        }
    }
}