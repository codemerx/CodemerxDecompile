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
    public class GotoStatement : BasePdbStatement
	{
		private List<Instruction> jumps;

		public GotoStatement(string label, IEnumerable<Instruction> gotoJumps)
		{
			this.TargetLabel = label;
			this.jumps = new List<Instruction>();
			if (gotoJumps != null)
			{
				jumps.AddRange(gotoJumps);
                jumps.Sort((x, y) => x.Offset.CompareTo(y.Offset));
			}
		}

        protected override IEnumerable<Instruction> GetOwnInstructions()
        {
            foreach (Instruction instruction in jumps)
            {
                yield return instruction;
            }
        }

        public override IEnumerable<ICodeNode> Children
        {
            get { yield break; }
        }

        public override Statement Clone()
        {
            GotoStatement result = new GotoStatement(TargetLabel, jumps);
            CopyParentAndLabel(result);
            return result;
        }

        public override Statement CloneStatementOnly()
        {
            GotoStatement result = new GotoStatement(TargetLabel, null);
            CopyParentAndLabel(result);
            return result;
        }

		public string TargetLabel { get; set; }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.GotoStatement; }
        }
    }
}