using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;

namespace JustDecompiler.Shared.Decompiler.StateMachines
{
    class StateMachineCFGCleanerV2
    {
        private readonly ControlFlowGraph theCFG;
        private readonly SwitchData[] controllerSwitchData;
        private readonly InstructionBlock newEntryBlock;

        private HashSet<InstructionBlock> toBeRemoved;

        //private StateMachineCFGCleanerV2(ControlFlowGraph theCFG, SwitchData[] controllerSwitchData, InstructionBlock newEntryBlock)
        //{
        //    this.theCFG = theCFG;
        //    this.controllerSwitchData = controllerSwitchData;
        //    this.newEntryBlock = newEntryBlock;
        //}

        public StateMachineCFGCleanerV2(ControlFlowGraph theCFG, SwitchData[] controllerSwitchData, InstructionBlock newEntryBlock)
        {
            if(controllerSwitchData.Length < 1) // there should be at least one item in controllerSwitchData or finding newEntryBlock fails
            {
                throw new ArgumentException(); // TODO improve the log
            }

            //var newEntryBlockCandidate = controllerSwitchData[0].DefaultCase;
            //foreach (SwitchData switchData in controllerSwitchData)
            //{
            //    if (switchData.DefaultCase.Index < newEntryBlockCandidate.Index) // TODO this is a weak condition, we should find the one that is ancestor of all
            //    {
            //        newEntryBlockCandidate = switchData.DefaultCase;
            //    }
            //}

            this.theCFG = theCFG;
            this.controllerSwitchData = controllerSwitchData;
            this.newEntryBlock = newEntryBlock;
        }

        private void MarkAdditionalSwitchDataBlocksForRemoval()
        {
            foreach(var switchData in this.controllerSwitchData)
            {
                //Mark for removal all of the state entries that are left without a predecessor.
                for (int i = 1; i < switchData.OrderedCasesArray.Length; i++)
                {
                    if (switchData.OrderedCasesArray[i] != null &&
                        switchData.OrderedCasesArray[i] != newEntryBlock &&
                        switchData.OrderedCasesArray[i].Predecessors.Count == 0)
                    {
                        toBeRemoved.Add(switchData.OrderedCasesArray[i]);
                    }
                }

                if (switchData.DefaultCase != null && switchData.DefaultCase.Predecessors.Count == 0 &&
                    switchData.DefaultCase != newEntryBlock)
                {
                    toBeRemoved.Add(switchData.DefaultCase);
                }
            }
        }

        /// <summary>
        /// Marks additional blocks for removal and then removes all of the marked blocks from the CFG.
        /// </summary>
        public bool CleanUpTheCFG(HashSet<InstructionBlock> blocksToRemove)
        {
            this.toBeRemoved = blocksToRemove;

            MarkAdditionalSwitchDataBlocksForRemoval();

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
