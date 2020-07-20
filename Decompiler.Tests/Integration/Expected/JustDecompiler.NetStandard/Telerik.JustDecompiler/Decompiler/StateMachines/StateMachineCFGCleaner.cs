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
			base();
			this.theCFG = theCFG;
			this.controllerSwitchData = controllerSwitchData;
			this.newEntryBlock = newEntryBlock;
			return;
		}

		private bool BFSRemoveBlocks()
		{
			V_0 = new Queue<InstructionBlock>();
			V_1 = this.toBeRemoved.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2.get_Predecessors().get_Count() != 0)
					{
						continue;
					}
					V_0.Enqueue(V_2);
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			while (V_0.get_Count() > 0)
			{
				V_3 = V_0.Dequeue();
				if (V_3.get_Index() == -1)
				{
					continue;
				}
				stackVariable22 = V_3.get_Successors();
				this.theCFG.RemoveBlockAt(V_3.get_Index());
				V_4 = stackVariable22;
				V_5 = 0;
				while (V_5 < (int)V_4.Length)
				{
					V_6 = V_4[V_5];
					if (V_6.get_Predecessors().get_Count() == 0 && InstructionBlock.op_Inequality(V_6, this.newEntryBlock))
					{
						V_0.Enqueue(V_6);
					}
					V_5 = V_5 + 1;
				}
				dummyVar0 = this.toBeRemoved.Remove(V_3);
			}
			return this.toBeRemoved.get_Count() == 0;
		}

		public bool CleanUpTheCFG(HashSet<InstructionBlock> blocksToRemove)
		{
			this.toBeRemoved = blocksToRemove;
			V_0 = 1;
			while (V_0 < (int)this.controllerSwitchData.get_OrderedCasesArray().Length)
			{
				if (InstructionBlock.op_Inequality(this.controllerSwitchData.get_OrderedCasesArray()[V_0], null) && InstructionBlock.op_Inequality(this.controllerSwitchData.get_OrderedCasesArray()[V_0], this.newEntryBlock) && this.controllerSwitchData.get_OrderedCasesArray()[V_0].get_Predecessors().get_Count() == 0)
				{
					dummyVar0 = this.toBeRemoved.Add(this.controllerSwitchData.get_OrderedCasesArray()[V_0]);
				}
				V_0 = V_0 + 1;
			}
			if (InstructionBlock.op_Inequality(this.controllerSwitchData.get_DefaultCase(), null) && this.controllerSwitchData.get_DefaultCase().get_Predecessors().get_Count() == 0 && InstructionBlock.op_Inequality(this.controllerSwitchData.get_DefaultCase(), this.newEntryBlock))
			{
				dummyVar1 = this.toBeRemoved.Add(this.controllerSwitchData.get_DefaultCase());
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
			V_0 = this.newEntryBlock;
			V_1 = V_0.get_Index();
			while (V_1 > 0)
			{
				this.theCFG.get_Blocks()[V_1] = this.theCFG.get_Blocks()[V_1 - 1];
				this.theCFG.get_Blocks()[V_1].set_Index(V_1);
				V_1 = V_1 - 1;
			}
			this.theCFG.get_Blocks()[0] = V_0;
			this.theCFG.get_Blocks()[0].set_Index(0);
			return;
		}
	}
}