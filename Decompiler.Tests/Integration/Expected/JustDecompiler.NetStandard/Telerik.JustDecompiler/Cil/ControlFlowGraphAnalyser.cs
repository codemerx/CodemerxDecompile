using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Cil
{
	internal class ControlFlowGraphAnalyser
	{
		private ControlFlowGraph cfg;

		private List<InstructionBlock> verticesThatPartitionGraph;

		private uint mostDistantInstructionBlockIndexReached;

		internal ControlFlowGraphAnalyser(ControlFlowGraph TheGraph)
		{
			base();
			this.cfg = TheGraph;
			this.verticesThatPartitionGraph = new List<InstructionBlock>();
			this.mostDistantInstructionBlockIndexReached = 0;
			this.MarkPartitioningVertices(0);
			return;
		}

		private InstructionBlock FindMostDistantSuccessor(InstructionBlock currentBlock)
		{
			V_0 = null;
			V_1 = currentBlock.get_Successors();
			V_2 = 0;
			while (V_2 < (int)V_1.Length)
			{
				V_3 = V_1[V_2];
				if (!InstructionBlock.op_Equality(V_0, null))
				{
					if (V_3.get_Index() > V_0.get_Index())
					{
						V_0 = V_3;
					}
				}
				else
				{
					if (V_3.get_Index() > currentBlock.get_Index())
					{
						V_0 = V_3;
					}
				}
				V_2 = V_2 + 1;
			}
			return V_0;
		}

		private void MarkPartitioningVertices(int startInstructionBlockIndex)
		{
			V_0 = this.cfg.get_Blocks()[startInstructionBlockIndex];
			V_1 = this.FindMostDistantSuccessor(V_0);
			if (InstructionBlock.op_Equality(V_1, null))
			{
				return;
			}
			if (V_1.get_Index() == this.cfg.get_Blocks().Count<InstructionBlock>() - 1)
			{
				return;
			}
			V_2 = null;
			V_3 = V_0.get_Index() + 1;
			while (V_3 <= V_1.get_Index())
			{
				if (!InstructionBlock.op_Equality(V_2, null))
				{
					V_4 = this.FindMostDistantSuccessor(this.cfg.get_Blocks()[V_3]);
					if (V_4.get_Index() > V_2.get_Index())
					{
						V_2 = V_4;
					}
				}
				else
				{
					V_2 = this.FindMostDistantSuccessor(this.cfg.get_Blocks()[V_3]);
				}
				V_3 = V_3 + 1;
			}
			if ((long)V_1.get_Index() < (ulong)this.mostDistantInstructionBlockIndexReached)
			{
				if (InstructionBlock.op_Inequality(V_2, null) && (long)V_2.get_Index() > (ulong)this.mostDistantInstructionBlockIndexReached)
				{
					this.mostDistantInstructionBlockIndexReached = V_2.get_Index();
				}
			}
			else
			{
				if (!InstructionBlock.op_Inequality(V_2, null))
				{
					this.verticesThatPartitionGraph.Add(V_1);
				}
				else
				{
					if (V_2.get_Index() <= V_1.get_Index())
					{
						this.verticesThatPartitionGraph.Add(V_1);
					}
				}
			}
			this.MarkPartitioningVertices(V_1.get_Index());
			return;
		}
	}
}