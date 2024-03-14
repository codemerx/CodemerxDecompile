using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using System.Text;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	public class CFGBlockLogicalConstruct : LogicalConstructBase, IInstructionBlockContainer
	{
        private readonly InstructionBlock theBlock;
        private readonly List<Expression> expressions;

        public List<Expression> LogicalConstructExpressions
        {
            get
            {
                return expressions;
            }
        }

        public InstructionBlock TheBlock
        {
            get
            {
                return theBlock;
            }
        }

		public CFGBlockLogicalConstruct(InstructionBlock theBlock, IEnumerable<Expression> expressionsEnumeration)
		{
            this.theBlock = theBlock;
            this.expressions = new List<Expression>(expressionsEnumeration);
		}

        public override int CompareTo(ISingleEntrySubGraph other)
        {
            return this.Index.CompareTo(other.Index);
        }

        public override HashSet<ISingleEntrySubGraph> Children
		{
			get
			{
				return LogicalConstructBase.EmptyISingleEntrySubGraphSet;
			}
		}

        public override int Index
        {
            get
            {
                return TheBlock.Index;
            }
        }

		public override ISingleEntrySubGraph Entry
		{
			get
			{
				return this;
			}
		}

        public override CFGBlockLogicalConstruct FirstBlock
        {
            get
            {
                return this;
            }
        }

        public override HashSet<CFGBlockLogicalConstruct> CFGBlocks
        {
            get
            {
                HashSet<CFGBlockLogicalConstruct> thisBlock = new HashSet<CFGBlockLogicalConstruct>();
                thisBlock.Add(this);
                return thisBlock;
            }
        }

        protected override string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedBlocks, LogicalFlowBuilderContext context)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.GetType().Name);
            sb.AppendLine("{");

            Instruction currentInstruction = TheBlock.First;
            while (currentInstruction != TheBlock.Last)
            {
                sb.AppendLine(string.Format("\t{0}",currentInstruction.ToString()));
                currentInstruction = currentInstruction.Next;
            }
            sb.AppendLine(string.Format("\t{0}", currentInstruction.ToString()));

            string followNodeString = string.Format("\tFollowNode: {0}", NodeILOffset(context, CFGFollowNode));
            sb.AppendLine(followNodeString);
            sb.AppendLine("}");

            printedBlocks.Add(this);

            return sb.ToString();
        }
    }
}
