using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
	internal class StateMachineCFGCleaner
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

		private bool BFSRemoveBlocks()
		{
			Queue<InstructionBlock> instructionBlocks = new Queue<InstructionBlock>();
			foreach (InstructionBlock instructionBlocks1 in this.toBeRemoved)
			{
				if (instructionBlocks1.Predecessors.Count != 0)
				{
					continue;
				}
				instructionBlocks.Enqueue(instructionBlocks1);
			}
			while (instructionBlocks.Count > 0)
			{
				InstructionBlock instructionBlocks2 = instructionBlocks.Dequeue();
				if (instructionBlocks2.Index == -1)
				{
					continue;
				}
				InstructionBlock[] successors = instructionBlocks2.Successors;
				this.theCFG.RemoveBlockAt(instructionBlocks2.Index);
				InstructionBlock[] instructionBlockArrays = successors;
				for (int i = 0; i < (int)instructionBlockArrays.Length; i++)
				{
					InstructionBlock instructionBlocks3 = instructionBlockArrays[i];
					if (instructionBlocks3.Predecessors.Count == 0 && instructionBlocks3 != this.newEntryBlock)
					{
						instructionBlocks.Enqueue(instructionBlocks3);
					}
				}
				this.toBeRemoved.Remove(instructionBlocks2);
			}
			return this.toBeRemoved.Count == 0;
		}

		public bool CleanUpTheCFG(HashSet<InstructionBlock> blocksToRemove)
		{
			this.toBeRemoved = blocksToRemove;
			for (int i = 1; i < (int)this.controllerSwitchData.OrderedCasesArray.Length; i++)
			{
				if (this.controllerSwitchData.OrderedCasesArray[i] != null && this.controllerSwitchData.OrderedCasesArray[i] != this.newEntryBlock && this.controllerSwitchData.OrderedCasesArray[i].Predecessors.Count == 0)
				{
					this.toBeRemoved.Add(this.controllerSwitchData.OrderedCasesArray[i]);
				}
			}
			if (this.controllerSwitchData.DefaultCase != null && this.controllerSwitchData.DefaultCase.Predecessors.Count == 0 && this.controllerSwitchData.DefaultCase != this.newEntryBlock)
			{
				this.toBeRemoved.Add(this.controllerSwitchData.DefaultCase);
			}
			if (!this.BFSRemoveBlocks())
			{
				return false;
			}
			this.FixTheNewFirstBlock();
			return true;
		}

		private void FixTheNewFirstBlock()
		{
			InstructionBlock instructionBlocks = this.newEntryBlock;
			for (int i = instructionBlocks.Index; i > 0; i--)
			{
				this.theCFG.Blocks[i] = this.theCFG.Blocks[i - 1];
				this.theCFG.Blocks[i].Index = i;
			}
			this.theCFG.Blocks[0] = instructionBlocks;
			this.theCFG.Blocks[0].Index = 0;
		}
	}
}