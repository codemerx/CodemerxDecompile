using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RemoveUnreachableBlocksStep : IDecompilationStep
	{
		private DecompilationContext decompilationContext;

		private ControlFlowGraph theCFG;

		private readonly Dictionary<InstructionBlock, ICollection<InstructionBlock>> guardedBlockToExceptionHandler;

		private readonly HashSet<InstructionBlock> reachableBlocks;

		public RemoveUnreachableBlocksStep()
		{
			this.guardedBlockToExceptionHandler = new Dictionary<InstructionBlock, ICollection<InstructionBlock>>();
			this.reachableBlocks = new HashSet<InstructionBlock>();
			base();
			return;
		}

		private void FindReachableBlocks()
		{
			this.GetGuardedBlockToExceptionHandlersMap();
			stackVariable3 = new InstructionBlock[1];
			stackVariable3[0] = this.theCFG.get_Blocks()[0];
			this.GetReachableBlocks(stackVariable3);
			while (true)
			{
				V_0 = new List<InstructionBlock>();
				V_1 = new List<InstructionBlock>();
				V_2 = this.guardedBlockToExceptionHandler.GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = V_2.get_Current();
						if (!this.reachableBlocks.Contains(V_3.get_Key()))
						{
							continue;
						}
						V_0.Add(V_3.get_Key());
						V_1.AddRange(V_3.get_Value());
					}
				}
				finally
				{
					((IDisposable)V_2).Dispose();
				}
				if (V_0.get_Count() == 0)
				{
					break;
				}
				V_4 = V_0.GetEnumerator();
				try
				{
					while (V_4.MoveNext())
					{
						V_5 = V_4.get_Current();
						dummyVar0 = this.guardedBlockToExceptionHandler.Remove(V_5);
					}
				}
				finally
				{
					((IDisposable)V_4).Dispose();
				}
				this.GetReachableBlocks(V_1);
			}
			return;
		}

		private void GetGuardedBlockToExceptionHandlersMap()
		{
			V_0 = this.theCFG.get_RawExceptionHandlers().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = this.theCFG.get_InstructionToBlockMapping().get_Item(V_1.get_TryStart().get_Offset());
					if (!this.guardedBlockToExceptionHandler.TryGetValue(V_2, out V_3))
					{
						V_3 = new List<InstructionBlock>();
						this.guardedBlockToExceptionHandler.set_Item(V_2, V_3);
					}
					V_4 = this.theCFG.get_InstructionToBlockMapping().get_Item(V_1.get_HandlerStart().get_Offset());
					V_3.Add(V_4);
					if (V_1.get_HandlerType() != 1)
					{
						continue;
					}
					V_5 = this.theCFG.get_InstructionToBlockMapping().get_Item(V_1.get_FilterStart().get_Offset());
					V_3.Add(V_5);
				}
			}
			finally
			{
				V_0.Dispose();
			}
			return;
		}

		private void GetReachableBlocks(IEnumerable<InstructionBlock> startBlocks)
		{
			this.reachableBlocks.UnionWith(startBlocks);
			V_0 = new Queue<InstructionBlock>(startBlocks);
			while (V_0.get_Count() > 0)
			{
				V_1 = V_0.Dequeue().get_Successors();
				V_2 = 0;
				while (V_2 < (int)V_1.Length)
				{
					V_3 = V_1[V_2];
					if (this.reachableBlocks.Add(V_3))
					{
						V_0.Enqueue(V_3);
					}
					V_2 = V_2 + 1;
				}
			}
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.decompilationContext = context;
			this.theCFG = context.get_MethodContext().get_ControlFlowGraph();
			this.ProcessTheControlFlowGraph();
			return body;
		}

		private void ProcessTheControlFlowGraph()
		{
			this.FindReachableBlocks();
			this.RemoveUnreachableBlocks();
			return;
		}

		private void RemoveUnreachableBlocks()
		{
			V_0 = new HashSet<InstructionBlock>();
			V_1 = this.theCFG.get_Blocks();
			V_2 = 0;
			while (V_2 < (int)V_1.Length)
			{
				V_3 = V_1[V_2];
				if (!this.reachableBlocks.Contains(V_3))
				{
					V_3.set_Successors(new InstructionBlock[0]);
					dummyVar0 = V_0.Add(V_3);
				}
				V_2 = V_2 + 1;
			}
			if (V_0.get_Count() > 0)
			{
				this.decompilationContext.get_MethodContext().set_IsMethodBodyChanged(true);
			}
			V_4 = 0;
			while (V_4 < this.theCFG.get_RawExceptionHandlers().get_Count())
			{
				V_5 = this.theCFG.get_RawExceptionHandlers().get_Item(V_4);
				if (V_0.Contains(this.theCFG.get_InstructionToBlockMapping().get_Item(V_5.get_TryStart().get_Offset())))
				{
					stackVariable54 = V_4;
					V_4 = stackVariable54 - 1;
					this.theCFG.get_RawExceptionHandlers().RemoveAt(stackVariable54);
				}
				V_4 = V_4 + 1;
			}
			V_6 = V_0.GetEnumerator();
			try
			{
				while (V_6.MoveNext())
				{
					V_7 = V_6.get_Current();
					this.theCFG.RemoveBlockAt(V_7.get_Index());
				}
			}
			finally
			{
				((IDisposable)V_6).Dispose();
			}
			return;
		}
	}
}