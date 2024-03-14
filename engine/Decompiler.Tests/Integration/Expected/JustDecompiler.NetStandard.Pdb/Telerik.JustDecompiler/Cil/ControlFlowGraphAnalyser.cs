using System;
using System.Collections.Generic;
using System.Linq;

namespace Telerik.JustDecompiler.Cil
{
	internal class ControlFlowGraphAnalyser
	{
		private ControlFlowGraph cfg;

		private List<InstructionBlock> verticesThatPartitionGraph;

		private uint mostDistantInstructionBlockIndexReached;

		internal ControlFlowGraphAnalyser(ControlFlowGraph TheGraph)
		{
			this.cfg = TheGraph;
			this.verticesThatPartitionGraph = new List<InstructionBlock>();
			this.mostDistantInstructionBlockIndexReached = 0;
			this.MarkPartitioningVertices(0);
		}

		private InstructionBlock FindMostDistantSuccessor(InstructionBlock currentBlock)
		{
			InstructionBlock instructionBlocks = null;
			InstructionBlock[] successors = currentBlock.Successors;
			for (int i = 0; i < (int)successors.Length; i++)
			{
				InstructionBlock instructionBlocks1 = successors[i];
				if (instructionBlocks == null)
				{
					if (instructionBlocks1.Index > currentBlock.Index)
					{
						instructionBlocks = instructionBlocks1;
					}
				}
				else if (instructionBlocks1.Index > instructionBlocks.Index)
				{
					instructionBlocks = instructionBlocks1;
				}
			}
			return instructionBlocks;
		}

		private void MarkPartitioningVertices(int startInstructionBlockIndex)
		{
			InstructionBlock blocks = this.cfg.Blocks[startInstructionBlockIndex];
			InstructionBlock instructionBlocks = this.FindMostDistantSuccessor(blocks);
			if (instructionBlocks == null)
			{
				return;
			}
			if (instructionBlocks.Index == this.cfg.Blocks.Count<InstructionBlock>() - 1)
			{
				return;
			}
			InstructionBlock instructionBlocks1 = null;
			for (int i = blocks.Index + 1; i <= instructionBlocks.Index; i++)
			{
				if (instructionBlocks1 != null)
				{
					InstructionBlock instructionBlocks2 = this.FindMostDistantSuccessor(this.cfg.Blocks[i]);
					if (instructionBlocks2.Index > instructionBlocks1.Index)
					{
						instructionBlocks1 = instructionBlocks2;
					}
				}
				else
				{
					instructionBlocks1 = this.FindMostDistantSuccessor(this.cfg.Blocks[i]);
				}
			}
			if ((long)instructionBlocks.Index < (ulong)this.mostDistantInstructionBlockIndexReached)
			{
				if (instructionBlocks1 != null && (long)instructionBlocks1.Index > (ulong)this.mostDistantInstructionBlockIndexReached)
				{
					this.mostDistantInstructionBlockIndexReached = (uint)instructionBlocks1.Index;
				}
			}
			else if (instructionBlocks1 == null)
			{
				this.verticesThatPartitionGraph.Add(instructionBlocks);
			}
			else if (instructionBlocks1.Index <= instructionBlocks.Index)
			{
				this.verticesThatPartitionGraph.Add(instructionBlocks);
			}
			this.MarkPartitioningVertices(instructionBlocks.Index);
		}
	}
}