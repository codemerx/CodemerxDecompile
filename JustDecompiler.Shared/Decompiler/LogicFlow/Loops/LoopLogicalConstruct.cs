using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions;
using System.Text;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Loops
{
    class LoopLogicalConstruct : LogicalConstructBase, IBreaksContainer
    {
        /// <summary>
        /// Gets the entry child of the loop construct.
        /// </summary>
        public override Cil.ISingleEntrySubGraph Entry
        {
            get
            {
                if (this.LoopType == LoopType.PreTestedLoop)
                {
                    return this.LoopCondition;
                }
                else
                {
                    return this.LoopBodyBlock;
                }
            }
        }

        /// <summary>
        /// Gets the CFG entry of the construct to which a continue edge leads.
        /// </summary>
        public CFGBlockLogicalConstruct LoopContinueEndPoint
        {
            get
            {
                if (this.LoopType == LoopType.InfiniteLoop)
                {
                    return this.LoopBodyBlock.FirstBlock;
                }
                else
                {
                    return this.LoopCondition.FirstBlock;
                }
            }
        }

        /// <summary>
        /// Gets the body of the loop.
        /// </summary>
        public BlockLogicalConstruct LoopBodyBlock { get; private set; }

        /// <summary>
        /// Gets the type of the loop.
        /// </summary>
        public LoopType LoopType { get; private set; }

        /// <summary>
        /// Gets the condition of the loop.
        /// </summary>
        public ConditionLogicalConstruct LoopCondition { get; private set; }

        /// <summary>
        /// Creates a new loop construct and attaches it to the logical tree.
        /// </summary>
        /// <param name="entry">The entry to the loop construct.</param>
        /// <param name="loopBody">Collection containing all of the constructs in the loop body.</param>
        /// <param name="loopType">The type of the loop.</param>
        /// <param name="loopCondition">The condition of the loop.</param>
        public LoopLogicalConstruct(ILogicalConstruct entry,
            HashSet<ILogicalConstruct> loopBody, LoopType loopType, ConditionLogicalConstruct loopCondition, TypeSystem typeSystem)
        {
			if (loopCondition != null)
			{
				loopCondition.LogicalContainer = this;
			}

            LoopType = loopType;
            LoopCondition = loopCondition;
            
            if(this.LoopType != LoopType.InfiniteLoop)
            {
                loopBody.Remove(LoopCondition);
            }

            DetermineLoopBodyBlock(entry, loopBody);

            RedirectChildrenToNewParent(GetLoopChildrenCollection());

            FixLoopCondition(typeSystem);
        }

        /// <summary>
        /// Creates the loop body block.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="loopBodyNodes"></param>
        private void DetermineLoopBodyBlock(ILogicalConstruct entry, HashSet<ILogicalConstruct> loopBodyNodes)
        {
            this.LoopBodyBlock = null;
            if (loopBodyNodes.Count > 0)
            {
                if (this.LoopType != LoopType.PreTestedLoop)
                {
                    //If the loop is not pretested then the entry should be the entry of the loop body
                    if(!loopBodyNodes.Contains(entry))
                    {
                        //sanity check
                        throw new Exception("Invalid entry of loop body.");
                    }
                    this.LoopBodyBlock = new BlockLogicalConstruct(entry, loopBodyNodes);
                }
                else
                {
                    //Otherwise the entry should be the successor of the condition that is in the collection.
                    ILogicalConstruct loopBodyEntry = LoopCondition.TrueSuccessor;
                    if (loopBodyEntry == null || !loopBodyNodes.Contains(loopBodyEntry))
                    {
                        loopBodyEntry = LoopCondition.FalseSuccessor;
                    }

                    if (loopBodyEntry == null || !loopBodyNodes.Contains(loopBodyEntry))
                    {
                        //sanity check
                        throw new Exception("Invalid entry of loop body.");
                    }
                    this.LoopBodyBlock = new BlockLogicalConstruct(loopBodyEntry, loopBodyNodes);
                }
            }
        }

        /// <summary>
        /// Gets a collection containing the children (to be) of the loop construct.
        /// </summary>
        /// <returns></returns>
        private ICollection<ILogicalConstruct> GetLoopChildrenCollection()
        {
            List<ILogicalConstruct> loopChildren = new List<ILogicalConstruct>();
            if(this.LoopBodyBlock != null)
            {
                loopChildren.Add(this.LoopBodyBlock);
            }

            if (this.LoopCondition != null)
            {
                loopChildren.Add(this.LoopCondition);
            }

            return loopChildren;
        }

        /// <summary>
        /// Fixes the loop condition, so that the true successor should be the loop body.
        /// </summary>
        private void FixLoopCondition(TypeSystem typeSystem)
        {
            if(this.LoopType == LoopType.InfiniteLoop)
            {
                return;
            }

            //If the TrueSuccessor is null then it means that the true successor is not in the loop body
            if (LoopCondition.TrueSuccessor == null)
            {
                LoopCondition.Negate(typeSystem);
            }
            if(LoopCondition.TrueSuccessor == null)
            {
                //sanity check
                throw new ArgumentException("The loop condition must have a true successor inside of the loop");
            }
        }

        protected override string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedBlocks, LogicalFlowBuilderContext context)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(LoopType);
            sb.AppendLine("LogicalConstruct");
            sb.AppendLine("{");
            
            foreach (LogicalConstructBase child in GetSortedArrayFromCollection(this.Children))
            {
                string childString = child.ToString(context);
                foreach (string childStringLine in childString.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                {
                    sb.Append('\t');
                    sb.AppendLine(childStringLine);
                }
                printedBlocks.UnionWith(child.CFGBlocks);
            }

            string followNodeString = string.Format("\tFollowNode: {0}", NodeILOffset(context, CFGFollowNode));
            sb.AppendLine(followNodeString);
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
