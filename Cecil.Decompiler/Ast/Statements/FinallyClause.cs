using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Statements
{
    public class FinallyClause : Statement
    {
        private readonly List<Instruction> mappedInstructions;
        private BlockStatement body;

        public FinallyClause(BlockStatement body, IEnumerable<Instruction> mappedInstructions = null)
        {
            if (body == null)
            {
                throw new ArgumentNullException("body");
            }

            this.body = body;
            body.Parent = this;

            if (mappedInstructions != null)
            {
                this.mappedInstructions = new List<Instruction>(mappedInstructions);
                this.mappedInstructions.Sort((x, y) => x.Offset.CompareTo(y.Offset));
            }
        }

        internal bool IsSpecialYieldFinally
        {
            get
            {
                return this.mappedInstructions != null;
            }
        }

        public BlockStatement Body
        {
            get
            {
                return this.body;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.body = value;
                this.body.Parent = this;
            }
        }

        protected override IEnumerable<Instruction> GetOwnInstructions()
        {
            if (IsSpecialYieldFinally)
            {
                foreach (Instruction instruction in mappedInstructions)
                {
                    yield return instruction;
                }
            }
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                if (!this.IsSpecialYieldFinally)
                {
                    yield return body;
                }
            }
        }

        public override CodeNodeType CodeNodeType
        {
            get { return Ast.CodeNodeType.FinallyClause; }
        }

        public override Statement Clone()
        {
            return new FinallyClause(this.body.Clone() as BlockStatement, this.mappedInstructions);
        }

        public override Statement CloneStatementOnly()
        {
            return new FinallyClause(this.body.CloneStatementOnly() as BlockStatement, null);
        }
    }
}
