/*
    Copyright CodeMerx 2024
    This file is part of CodemerxDecompile.

    CodemerxDecompile is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    CodemerxDecompile is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.
*/

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

        public StateMachineCFGCleanerV2(ControlFlowGraph theCFG, SwitchData[] controllerSwitchData, InstructionBlock newEntryBlock)
        {
            if (controllerSwitchData.Length < 1) // there should be at least one item in controllerSwitchData or finding newEntryBlock fails
            {
                throw new ArgumentException("Controller switch data cannot be empty", nameof(controllerSwitchData));
            }

            this.theCFG = theCFG;
            this.controllerSwitchData = controllerSwitchData;
            this.newEntryBlock = newEntryBlock;
        }

        private void MarkAdditionalSwitchDataBlocksForRemoval()
        {
            foreach(var switchData in this.controllerSwitchData)
            {
                // Mark for removal all of the state entries that are left without a predecessor.
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
