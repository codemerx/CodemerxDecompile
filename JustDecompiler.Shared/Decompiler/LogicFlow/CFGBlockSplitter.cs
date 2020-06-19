using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
    class CFGBlockSplitter
    {
        private readonly LogicalFlowBuilderContext logicalContext;

        public CFGBlockSplitter(LogicalFlowBuilderContext logicalContext)
        {
            this.logicalContext = logicalContext;
        }

        /// <summary>
        /// Splits each CFG construct that has more than one successor (i.e. condition block) and holds more than one expression.
        /// </summary>
        public void SplitConditionalCFGBlocks()
        {
            //We need to copy the dictionary, since the LogicalFlowUtilities.SplitCFGBlockAt method modifies it (and the enumerator explodes).
            //Also we copy it in a list, since we don't need the functionality of the dictionary.
            List<KeyValuePair<InstructionBlock, CFGBlockLogicalConstruct[]>> blockToConstructsPairList =
                new List<KeyValuePair<InstructionBlock, CFGBlockLogicalConstruct[]>>(logicalContext.CFGBlockToLogicalConstructMap);

            foreach (KeyValuePair<InstructionBlock, CFGBlockLogicalConstruct[]> blockToConstructsPair in blockToConstructsPairList)
            {
                if(blockToConstructsPair.Key.Successors.Length > 1)
                {
                    //For each instruction block there can be more than one partial CFG LC, so we take the last one, since it will hold the condition expression.
                    CFGBlockLogicalConstruct cfgConstruct = blockToConstructsPair.Value[blockToConstructsPair.Value.Length - 1];
                    if(cfgConstruct.LogicalConstructExpressions.Count > 1)
                    {
                        //If the last construct for this block holds more than one expression we split it at the last expression.
                        LogicalFlowUtilities.SplitCFGBlockAt(logicalContext, cfgConstruct, cfgConstruct.LogicalConstructExpressions.Count - 1);
                    }
                }
            }
        }
    }
}
