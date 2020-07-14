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

		private readonly Dictionary<InstructionBlock, ICollection<InstructionBlock>> guardedBlockToExceptionHandler = new Dictionary<InstructionBlock, ICollection<InstructionBlock>>();

		private readonly HashSet<InstructionBlock> reachableBlocks = new HashSet<InstructionBlock>();

		public RemoveUnreachableBlocksStep()
		{
		}

		private void FindReachableBlocks()
		{
			this.GetGuardedBlockToExceptionHandlersMap();
			this.GetReachableBlocks(new InstructionBlock[] { this.theCFG.Blocks[0] });
			while (true)
			{
				List<InstructionBlock> instructionBlocks = new List<InstructionBlock>();
				List<InstructionBlock> instructionBlocks1 = new List<InstructionBlock>();
				foreach (KeyValuePair<InstructionBlock, ICollection<InstructionBlock>> keyValuePair in this.guardedBlockToExceptionHandler)
				{
					if (!this.reachableBlocks.Contains(keyValuePair.Key))
					{
						continue;
					}
					instructionBlocks.Add(keyValuePair.Key);
					instructionBlocks1.AddRange(keyValuePair.Value);
				}
				if (instructionBlocks.Count == 0)
				{
					break;
				}
				foreach (InstructionBlock instructionBlock in instructionBlocks)
				{
					this.guardedBlockToExceptionHandler.Remove(instructionBlock);
				}
				this.GetReachableBlocks(instructionBlocks1);
			}
		}

		private void GetGuardedBlockToExceptionHandlersMap()
		{
			ICollection<InstructionBlock> instructionBlocks;
			foreach (ExceptionHandler rawExceptionHandler in this.theCFG.RawExceptionHandlers)
			{
				InstructionBlock item = this.theCFG.InstructionToBlockMapping[rawExceptionHandler.get_TryStart().get_Offset()];
				if (!this.guardedBlockToExceptionHandler.TryGetValue(item, out instructionBlocks))
				{
					instructionBlocks = new List<InstructionBlock>();
					this.guardedBlockToExceptionHandler[item] = instructionBlocks;
				}
				InstructionBlock item1 = this.theCFG.InstructionToBlockMapping[rawExceptionHandler.get_HandlerStart().get_Offset()];
				instructionBlocks.Add(item1);
				if (rawExceptionHandler.get_HandlerType() != 1)
				{
					continue;
				}
				InstructionBlock instructionBlocks1 = this.theCFG.InstructionToBlockMapping[rawExceptionHandler.get_FilterStart().get_Offset()];
				instructionBlocks.Add(instructionBlocks1);
			}
		}

		private void GetReachableBlocks(IEnumerable<InstructionBlock> startBlocks)
		{
			this.reachableBlocks.UnionWith(startBlocks);
			Queue<InstructionBlock> instructionBlocks = new Queue<InstructionBlock>(startBlocks);
			while (instructionBlocks.Count > 0)
			{
				InstructionBlock[] successors = instructionBlocks.Dequeue().Successors;
				for (int i = 0; i < (int)successors.Length; i++)
				{
					InstructionBlock instructionBlocks1 = successors[i];
					if (this.reachableBlocks.Add(instructionBlocks1))
					{
						instructionBlocks.Enqueue(instructionBlocks1);
					}
				}
			}
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.decompilationContext = context;
			this.theCFG = context.MethodContext.ControlFlowGraph;
			this.ProcessTheControlFlowGraph();
			return body;
		}

		private void ProcessTheControlFlowGraph()
		{
			this.FindReachableBlocks();
			this.RemoveUnreachableBlocks();
		}

		private void RemoveUnreachableBlocks()
		{
			HashSet<InstructionBlock> instructionBlocks = new HashSet<InstructionBlock>();
			InstructionBlock[] blocks = this.theCFG.Blocks;
			for (int i = 0; i < (int)blocks.Length; i++)
			{
				InstructionBlock instructionBlocks1 = blocks[i];
				if (!this.reachableBlocks.Contains(instructionBlocks1))
				{
					instructionBlocks1.Successors = new InstructionBlock[0];
					instructionBlocks.Add(instructionBlocks1);
				}
			}
			if (instructionBlocks.Count > 0)
			{
				this.decompilationContext.MethodContext.IsMethodBodyChanged = true;
			}
			for (int j = 0; j < this.theCFG.RawExceptionHandlers.get_Count(); j++)
			{
				ExceptionHandler item = this.theCFG.RawExceptionHandlers.get_Item(j);
				if (instructionBlocks.Contains(this.theCFG.InstructionToBlockMapping[item.get_TryStart().get_Offset()]))
				{
					int num = j;
					j = num - 1;
					this.theCFG.RawExceptionHandlers.RemoveAt(num);
				}
			}
			foreach (InstructionBlock instructionBlock in instructionBlocks)
			{
				this.theCFG.RemoveBlockAt(instructionBlock.Index);
			}
		}
	}
}