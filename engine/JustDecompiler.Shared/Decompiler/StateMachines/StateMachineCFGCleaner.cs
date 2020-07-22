using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
    class StateMachineCFGCleaner
    {
        private readonly ControlFlowGraph theCFG;
        private readonly SwitchData controllerSwitchData;
        private readonly InstructionBlock newEntryBlock;

        private HashSet<InstructionBlock> toBeRemoved;

        public StateMachineCFGCleaner(ControlFlowGraph theCFG, SwitchData controllerSwitchData, InstructionBlock newEntryBlock)
        {
            this.theCFG = theCFG;
            this.controllerSwitchData = controllerSwitchData;
            this.newEntryBlock = newEntryBlock;
        }

        /// <summary>
        /// Marks additional blocks for removal and then removes all of the marked blocks from the CFG.
        /// </summary>
        public bool CleanUpTheCFG(HashSet<InstructionBlock> blocksToRemove)
        {
            this.toBeRemoved = blocksToRemove;

            //Mark for removal all of the state entries that are left without a predecessor.
            for (int i = 1; i < controllerSwitchData.OrderedCasesArray.Length; i++)
            {
                if (controllerSwitchData.OrderedCasesArray[i] != null &&
                    controllerSwitchData.OrderedCasesArray[i] != newEntryBlock &&
                    controllerSwitchData.OrderedCasesArray[i].Predecessors.Count == 0)
                {
                    toBeRemoved.Add(controllerSwitchData.OrderedCasesArray[i]);
                }
            }

            if (controllerSwitchData.DefaultCase != null && controllerSwitchData.DefaultCase.Predecessors.Count == 0 &&
                controllerSwitchData.DefaultCase != newEntryBlock)
            {
                toBeRemoved.Add(controllerSwitchData.DefaultCase);
            }

            if (!BFSRemoveBlocks())
            {
                return false;
            }

            FixTheNewFirstBlock();
            return true;
        }

        /// <summary>
        /// Traverses the CFG and removes the marked blocks.
        /// </summary>
        /// <remarks>
        /// In order to remove a block it has to be marked for removal and it should not have any predecessors.
        /// </remarks>
        private bool BFSRemoveBlocks()
        {
            Queue<InstructionBlock> traverseQueue = new Queue<InstructionBlock>();

            foreach (InstructionBlock block in toBeRemoved)
            {
                if (block.Predecessors.Count == 0)
                {
                    traverseQueue.Enqueue(block);
                }
            }

            while (traverseQueue.Count > 0)
            {
                InstructionBlock currentBlock = traverseQueue.Dequeue();

                if (currentBlock.Index == -1)
                {
                    continue;
                }
                InstructionBlock[] currentBlockSuccessors = currentBlock.Successors;
                theCFG.RemoveBlockAt(currentBlock.Index);

                foreach (InstructionBlock successor in currentBlockSuccessors)
                {
                    if (successor.Predecessors.Count == 0 && successor != newEntryBlock)
                    {
                        traverseQueue.Enqueue(successor);
                    }
                }

                toBeRemoved.Remove(currentBlock);
            }

            return toBeRemoved.Count == 0;
        }

        /// <summary>
        /// Fix the index of the blocks, so that the block with index 0 is the entry of the first state.
        /// </summary>
        private void FixTheNewFirstBlock()
        {
            InstructionBlock firstBlock = newEntryBlock;

            for (int i = firstBlock.Index; i > 0; i--)
            {
                theCFG.Blocks[i] = theCFG.Blocks[i - 1];
                theCFG.Blocks[i].Index = i;
            }

            theCFG.Blocks[0] = firstBlock;
            theCFG.Blocks[0].Index = 0;
        }
    }
}
