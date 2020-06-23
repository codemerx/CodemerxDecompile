using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions
{
	internal class GuardedBlocksBuilder
	{
		private LogicalFlowBuilderContext context;

		private List<int> sortedBlockStarts;

		private Dictionary<BlockRange, ExceptionHandlingLogicalConstruct> tryBlocksFound;

		public GuardedBlocksBuilder(LogicalFlowBuilderContext Context)
		{
			this.context = Context;
			this.sortedBlockStarts = this.context.InstructionToCFGBlockMapping.Keys.ToList<int>();
			this.sortedBlockStarts.Sort();
			this.tryBlocksFound = new Dictionary<BlockRange, ExceptionHandlingLogicalConstruct>();
		}

		private void AddHandlerToTryBlock(ExceptionHandlingLogicalConstruct tryBlockExistingHandler, ExceptionHandler handler)
		{
			if (tryBlockExistingHandler is TryCatchFilterLogicalConstruct)
			{
				if (handler.HandlerType == ExceptionHandlerType.Fault || handler.HandlerType == ExceptionHandlerType.Finally)
				{
					throw new Exception("Illegal IL: Non-exclusive Fault/Finally handler found");
				}
				if (handler.HandlerType == ExceptionHandlerType.Filter)
				{
					(tryBlockExistingHandler as TryCatchFilterLogicalConstruct).AddHandler(this.CreateFilterHandler(handler));
					return;
				}
				if (handler.HandlerType == ExceptionHandlerType.Catch)
				{
					(tryBlockExistingHandler as TryCatchFilterLogicalConstruct).AddHandler(this.CreateCatchHandler(handler));
					return;
				}
			}
			else if (tryBlockExistingHandler is TryFaultLogicalConstruct || tryBlockExistingHandler is TryFinallyLogicalConstruct)
			{
				throw new Exception("Illegal IL: Non-exclusive Fault/Finally handler found");
			}
		}

		private ExceptionHandlingBlockCatch CreateCatchHandler(ExceptionHandler handler)
		{
			ILogicalConstruct logicalConstruct;
			BlockRange blockRangeFromInstructions = this.GetBlockRangeFromInstructions(handler.HandlerStart, handler.HandlerEnd);
			return new ExceptionHandlingBlockCatch(this.GetLogicalConstructsInRange(blockRangeFromInstructions, out logicalConstruct), this.context.CFGBlockToLogicalConstructMap[blockRangeFromInstructions.Start][0], handler.CatchType);
		}

		private BlockLogicalConstruct CreateExceptionhandlingBlock(BlockRange cfgBlocks)
		{
			ILogicalConstruct logicalConstruct;
			HashSet<ILogicalConstruct> logicalConstructsInRange = this.GetLogicalConstructsInRange(cfgBlocks, out logicalConstruct);
			return new BlockLogicalConstruct(this.FindChildBlockBelongsTo(logicalConstruct, this.context.CFGBlockToLogicalConstructMap[this.GetEntryBlockInRange(cfgBlocks)][0]), logicalConstructsInRange);
		}

		private ExceptionHandlingBlockFilter CreateFilterHandler(ExceptionHandler handler)
		{
			ILogicalConstruct logicalConstruct;
			BlockRange blockRangeFromInstructions = this.GetBlockRangeFromInstructions(handler.FilterStart, handler.FilterEnd);
			HashSet<ILogicalConstruct> logicalConstructsInRange = this.GetLogicalConstructsInRange(blockRangeFromInstructions, out logicalConstruct);
			if (this.FindFollowNodes(logicalConstructsInRange) != null)
			{
				throw new Exception("The filter block must not have a follow node");
			}
			BlockLogicalConstruct blockLogicalConstruct = new BlockLogicalConstruct(this.context.CFGBlockToLogicalConstructMap[blockRangeFromInstructions.Start][0], logicalConstructsInRange);
			BlockRange blockRange = this.GetBlockRangeFromInstructions(handler.HandlerStart, handler.HandlerEnd);
			HashSet<ILogicalConstruct> logicalConstructs = this.GetLogicalConstructsInRange(blockRange, out logicalConstruct);
			BlockLogicalConstruct blockLogicalConstruct1 = new BlockLogicalConstruct(this.context.CFGBlockToLogicalConstructMap[blockRange.Start][0], logicalConstructs);
			return new ExceptionHandlingBlockFilter(blockLogicalConstruct, blockLogicalConstruct1);
		}

		private int FindBlockStartOffset(int blockLastInstructionOffset)
		{
			int num = -1;
			foreach (int sortedBlockStart in this.sortedBlockStarts)
			{
				if (sortedBlockStart > blockLastInstructionOffset)
				{
					break;
				}
				num = sortedBlockStart;
			}
			return num;
		}

		private ILogicalConstruct FindChildBlockBelongsTo(ILogicalConstruct parent, CFGBlockLogicalConstruct block)
		{
			ILogicalConstruct logicalConstruct = null;
			ILogicalConstruct logicalConstruct1 = block;
			do
			{
				if (logicalConstruct1.Parent != parent)
				{
					logicalConstruct1 = (ILogicalConstruct)logicalConstruct1.Parent;
				}
				else
				{
					logicalConstruct = logicalConstruct1;
					break;
				}
			}
			while (logicalConstruct1.Parent != null);
			return logicalConstruct;
		}

		public void FindExceptionHandlingConstructs()
		{
			foreach (ExceptionHandler rawExceptionHandler in this.context.CFG.RawExceptionHandlers)
			{
				BlockRange blockRangeFromInstructions = this.GetBlockRangeFromInstructions(rawExceptionHandler.TryStart, rawExceptionHandler.TryEnd);
				ExceptionHandlingLogicalConstruct exceptionHandlingLogicalConstruct = null;
				if (!this.tryBlocksFound.TryGetValue(blockRangeFromInstructions, out exceptionHandlingLogicalConstruct))
				{
					BlockLogicalConstruct blockLogicalConstruct = this.CreateExceptionhandlingBlock(blockRangeFromInstructions);
					ExceptionHandlingLogicalConstruct tryCatchFilterLogicalConstruct = null;
					switch (rawExceptionHandler.HandlerType)
					{
						case ExceptionHandlerType.Catch:
						{
							tryCatchFilterLogicalConstruct = new TryCatchFilterLogicalConstruct(blockLogicalConstruct, this.CreateCatchHandler(rawExceptionHandler));
							goto case ExceptionHandlerType.Filter | ExceptionHandlerType.Finally;
						}
						case ExceptionHandlerType.Filter:
						{
							tryCatchFilterLogicalConstruct = new TryCatchFilterLogicalConstruct(blockLogicalConstruct, this.CreateFilterHandler(rawExceptionHandler));
							goto case ExceptionHandlerType.Filter | ExceptionHandlerType.Finally;
						}
						case ExceptionHandlerType.Finally:
						{
							BlockRange blockRange = this.GetBlockRangeFromInstructions(rawExceptionHandler.HandlerStart, rawExceptionHandler.HandlerEnd);
							tryCatchFilterLogicalConstruct = new TryFinallyLogicalConstruct(blockLogicalConstruct, this.CreateExceptionhandlingBlock(blockRange));
							goto case ExceptionHandlerType.Filter | ExceptionHandlerType.Finally;
						}
						case ExceptionHandlerType.Filter | ExceptionHandlerType.Finally:
						{
							this.tryBlocksFound.Add(blockRangeFromInstructions, tryCatchFilterLogicalConstruct);
							continue;
						}
						case ExceptionHandlerType.Fault:
						{
							BlockRange blockRangeFromInstructions1 = this.GetBlockRangeFromInstructions(rawExceptionHandler.HandlerStart, rawExceptionHandler.HandlerEnd);
							tryCatchFilterLogicalConstruct = new TryFaultLogicalConstruct(blockLogicalConstruct, this.CreateExceptionhandlingBlock(blockRangeFromInstructions1));
							goto case ExceptionHandlerType.Filter | ExceptionHandlerType.Finally;
						}
						default:
						{
							goto case ExceptionHandlerType.Filter | ExceptionHandlerType.Finally;
						}
					}
				}
				else
				{
					this.AddHandlerToTryBlock(exceptionHandlingLogicalConstruct, rawExceptionHandler);
				}
			}
		}

		private ILogicalConstruct FindFollowNodes(HashSet<ILogicalConstruct> children)
		{
			HashSet<ILogicalConstruct> logicalConstructs = new HashSet<ILogicalConstruct>();
			foreach (ILogicalConstruct child in children)
			{
				foreach (ILogicalConstruct allSuccessor in child.AllSuccessors)
				{
					if (children.Contains(allSuccessor))
					{
						continue;
					}
					logicalConstructs.Add(allSuccessor);
				}
			}
			return logicalConstructs.FirstOrDefault<ILogicalConstruct>();
		}

		private BlockRange GetBlockRangeFromInstructions(Instruction start, Instruction end)
		{
			InstructionBlock item = this.context.CFG.InstructionToBlockMapping[start.Offset];
			int num = (end != null ? end.Previous.Offset : this.context.CFG.MethodBody.CodeSize);
			return new BlockRange(item, this.context.CFG.InstructionToBlockMapping[this.FindBlockStartOffset(num)]);
		}

		private InstructionBlock GetEntryBlockInRange(BlockRange blockRange)
		{
			int offset = blockRange.Start.First.Offset;
			int num = blockRange.End.First.Offset;
			for (int i = 0; i < (int)this.context.CFG.Blocks.Length; i++)
			{
				InstructionBlock blocks = this.context.CFG.Blocks[i];
				if (blocks.First.Offset >= offset && blocks.First.Offset <= num)
				{
					return blocks;
				}
			}
			throw new Exception("Invalid range");
		}

		private HashSet<ILogicalConstruct> GetLogicalConstructsInRange(BlockRange blockRange, out ILogicalConstruct theCommonParent)
		{
			ILogicalConstruct logicalConstruct;
			HashSet<ILogicalConstruct> logicalConstructs = new HashSet<ILogicalConstruct>();
			HashSet<ISingleEntrySubGraph> singleEntrySubGraphs = new HashSet<ISingleEntrySubGraph>();
			int offset = blockRange.Start.First.Offset;
			int num = blockRange.End.First.Offset;
			for (int i = 0; i < (int)this.context.CFG.Blocks.Length; i++)
			{
				InstructionBlock blocks = this.context.CFG.Blocks[i];
				if (blocks.First.Offset >= offset && blocks.First.Offset <= num)
				{
					CFGBlockLogicalConstruct[] item = this.context.CFGBlockToLogicalConstructMap[blocks];
					for (int j = 0; j < (int)item.Length; j++)
					{
						singleEntrySubGraphs.Add((ILogicalConstruct)item[j].Parent);
						logicalConstructs.Add(item[j]);
					}
				}
			}
			if (singleEntrySubGraphs.Count == 1)
			{
				theCommonParent = (ILogicalConstruct)singleEntrySubGraphs.ToArray<ISingleEntrySubGraph>()[0];
				return logicalConstructs;
			}
			theCommonParent = (ILogicalConstruct)LogicalFlowUtilities.FindFirstCommonParent(singleEntrySubGraphs);
			HashSet<ILogicalConstruct> logicalConstructs1 = new HashSet<ILogicalConstruct>();
			foreach (ILogicalConstruct logicalConstruct1 in logicalConstructs)
			{
				LogicalFlowUtilities.TryGetParentConstructWithGivenParent(logicalConstruct1, theCommonParent, out logicalConstruct);
				logicalConstructs1.Add(logicalConstruct);
			}
			if (theCommonParent is ExceptionHandlingLogicalConstruct)
			{
				logicalConstructs1.Clear();
				logicalConstructs1.Add(theCommonParent);
				theCommonParent = theCommonParent.Parent as ILogicalConstruct;
			}
			return logicalConstructs1;
		}
	}
}