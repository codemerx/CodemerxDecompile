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

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public abstract class Expression : BaseCodeNode, IEquatable<Expression>
    {
        protected readonly List<Instruction> instructions = new List<Instruction>();

        protected Expression(IEnumerable<Instruction> instructions)
        {
            if (instructions != null)
            {
                this.instructions.AddRange(instructions);
            }
        }

        public IEnumerable<Instruction> MappedInstructions
        {
            get
            {
                return new List<Instruction>(this.instructions);
            }
        }

        protected override IEnumerable<Instruction> GetOwnInstructions()
        {
            return this.instructions;
        }

        public abstract bool Equals(Expression other);

        public virtual TypeReference ExpressionType { get; set; }

        public virtual bool HasType 
        {
            get
            {
                return ExpressionType != null;
            }
        }

        public void MapBranchInstructions(IEnumerable<Instruction> branchInstructions)
        {
            foreach (Instruction instruction in branchInstructions)
            {
                if (instruction.OpCode.Code != Code.Br && instruction.OpCode.Code != Code.Br_S)
                {
                    throw new InvalidOperationException("Only unconditional branch instructions are allowed");
                }
            }
            this.instructions.AddRange(branchInstructions);
        }

        public void MapDupInstructions(IEnumerable<Instruction> dupInstructions)
        {
            foreach (Instruction instruction in dupInstructions)
            {
                if (instruction.OpCode.Code != Code.Dup)
                {
                    throw new InvalidOperationException("Only dup instructions are allowed");
                }
            }
            this.instructions.AddRange(dupInstructions);
        }

        public abstract Expression Clone();

        public abstract Expression CloneExpressionOnly();

        public Expression CloneAndAttachInstructions(IEnumerable<Instruction> instructions)
        {
            Expression clone = this.Clone();
            clone.instructions.AddRange(instructions);
            return clone;
        }

        public Expression CloneExpressionOnlyAndAttachInstructions(IEnumerable<Instruction> instructions)
        {
            Expression clone = this.CloneExpressionOnly();
            clone.instructions.AddRange(instructions);
            return clone;
        }
    }
}