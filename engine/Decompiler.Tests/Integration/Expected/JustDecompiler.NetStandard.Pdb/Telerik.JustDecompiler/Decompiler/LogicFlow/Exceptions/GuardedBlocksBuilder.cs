using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
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
			base();
			this.context = Context;
			this.sortedBlockStarts = this.context.get_InstructionToCFGBlockMapping().get_Keys().ToList<int>();
			this.sortedBlockStarts.Sort();
			this.tryBlocksFound = new Dictionary<BlockRange, ExceptionHandlingLogicalConstruct>();
			return;
		}

		private void AddHandlerToTryBlock(ExceptionHandlingLogicalConstruct tryBlockExistingHandler, ExceptionHandler handler)
		{
			if (tryBlockExistingHandler as TryCatchFilterLogicalConstruct == null)
			{
				if (tryBlockExistingHandler as TryFaultLogicalConstruct != null || tryBlockExistingHandler as TryFinallyLogicalConstruct != null)
				{
					throw new Exception("Illegal IL: Non-exclusive Fault/Finally handler found");
				}
			}
			else
			{
				if (handler.get_HandlerType() == 4 || handler.get_HandlerType() == 2)
				{
					throw new Exception("Illegal IL: Non-exclusive Fault/Finally handler found");
				}
				if (handler.get_HandlerType() == 1)
				{
					(tryBlockExistingHandler as TryCatchFilterLogicalConstruct).AddHandler(this.CreateFilterHandler(handler));
					return;
				}
				if (handler.get_HandlerType() == null)
				{
					(tryBlockExistingHandler as TryCatchFilterLogicalConstruct).AddHandler(this.CreateCatchHandler(handler));
					return;
				}
			}
			return;
		}

		private ExceptionHandlingBlockCatch CreateCatchHandler(ExceptionHandler handler)
		{
			V_1 = this.GetBlockRangeFromInstructions(handler.get_HandlerStart(), handler.get_HandlerEnd());
			return new ExceptionHandlingBlockCatch(this.GetLogicalConstructsInRange(V_1, out V_0), this.context.get_CFGBlockToLogicalConstructMap().get_Item(V_1.Start)[0], handler.get_CatchType());
		}

		private BlockLogicalConstruct CreateExceptionhandlingBlock(BlockRange cfgBlocks)
		{
			V_0 = this.GetLogicalConstructsInRange(cfgBlocks, out V_1);
			return new BlockLogicalConstruct(this.FindChildBlockBelongsTo(V_1, this.context.get_CFGBlockToLogicalConstructMap().get_Item(this.GetEntryBlockInRange(cfgBlocks))[0]), V_0);
		}

		private ExceptionHandlingBlockFilter CreateFilterHandler(ExceptionHandler handler)
		{
			V_1 = this.GetBlockRangeFromInstructions(handler.get_FilterStart(), handler.get_FilterEnd());
			V_2 = this.GetLogicalConstructsInRange(V_1, out V_0);
			if (this.FindFollowNodes(V_2) != null)
			{
				throw new Exception("The filter block must not have a follow node");
			}
			stackVariable22 = new BlockLogicalConstruct(this.context.get_CFGBlockToLogicalConstructMap().get_Item(V_1.Start)[0], V_2);
			V_3 = this.GetBlockRangeFromInstructions(handler.get_HandlerStart(), handler.get_HandlerEnd());
			V_4 = this.GetLogicalConstructsInRange(V_3, out V_0);
			V_5 = new BlockLogicalConstruct(this.context.get_CFGBlockToLogicalConstructMap().get_Item(V_3.Start)[0], V_4);
			return new ExceptionHandlingBlockFilter(stackVariable22, V_5);
		}

		private int FindBlockStartOffset(int blockLastInstructionOffset)
		{
			V_0 = -1;
			V_1 = this.sortedBlockStarts.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2 > blockLastInstructionOffset)
					{
						break;
					}
					V_0 = V_2;
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0;
		}

		private ILogicalConstruct FindChildBlockBelongsTo(ILogicalConstruct parent, CFGBlockLogicalConstruct block)
		{
			V_0 = null;
			V_1 = block;
			do
			{
				if (V_1.get_Parent() != parent)
				{
					V_1 = (ILogicalConstruct)V_1.get_Parent();
				}
				else
				{
					V_0 = V_1;
					break;
				}
			}
			while (V_1.get_Parent() != null);
			return V_0;
		}

		public void FindExceptionHandlingConstructs()
		{
			V_0 = this.context.get_CFG().get_RawExceptionHandlers().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = this.GetBlockRangeFromInstructions(V_1.get_TryStart(), V_1.get_TryEnd());
					V_3 = null;
					if (!this.tryBlocksFound.TryGetValue(V_2, out V_3))
					{
						V_4 = this.CreateExceptionhandlingBlock(V_2);
						V_5 = null;
						switch (V_1.get_HandlerType())
						{
							case 0:
							{
								V_5 = new TryCatchFilterLogicalConstruct(V_4, this.CreateCatchHandler(V_1));
								goto Label0;
							}
							case 1:
							{
								V_5 = new TryCatchFilterLogicalConstruct(V_4, this.CreateFilterHandler(V_1));
								goto Label0;
							}
							case 2:
							{
								V_7 = this.GetBlockRangeFromInstructions(V_1.get_HandlerStart(), V_1.get_HandlerEnd());
								V_5 = new TryFinallyLogicalConstruct(V_4, this.CreateExceptionhandlingBlock(V_7));
								goto Label0;
							}
							case 3:
							{
							Label0:
								this.tryBlocksFound.Add(V_2, V_5);
								continue;
							}
							case 4:
							{
								V_6 = this.GetBlockRangeFromInstructions(V_1.get_HandlerStart(), V_1.get_HandlerEnd());
								V_5 = new TryFaultLogicalConstruct(V_4, this.CreateExceptionhandlingBlock(V_6));
								goto Label0;
							}
							default:
							{
								goto Label0;
							}
						}
					}
					else
					{
						this.AddHandlerToTryBlock(V_3, V_1);
					}
				}
			}
			finally
			{
				V_0.Dispose();
			}
			return;
		}

		private ILogicalConstruct FindFollowNodes(HashSet<ILogicalConstruct> children)
		{
			V_0 = new HashSet<ILogicalConstruct>();
			V_1 = children.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current().get_AllSuccessors().GetEnumerator();
					try
					{
						while (V_2.MoveNext())
						{
							V_3 = (ILogicalConstruct)V_2.get_Current();
							if (children.Contains(V_3))
							{
								continue;
							}
							dummyVar0 = V_0.Add(V_3);
						}
					}
					finally
					{
						((IDisposable)V_2).Dispose();
					}
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0.FirstOrDefault<ILogicalConstruct>();
		}

		private BlockRange GetBlockRangeFromInstructions(Instruction start, Instruction end)
		{
			stackVariable6 = this.context.get_CFG().get_InstructionToBlockMapping().get_Item(start.get_Offset());
			if (end != null)
			{
				stackVariable10 = end.get_Previous().get_Offset();
			}
			else
			{
				stackVariable10 = this.context.get_CFG().get_MethodBody().get_CodeSize();
			}
			V_0 = stackVariable10;
			return new BlockRange(stackVariable6, this.context.get_CFG().get_InstructionToBlockMapping().get_Item(this.FindBlockStartOffset(V_0)));
		}

		private InstructionBlock GetEntryBlockInRange(BlockRange blockRange)
		{
			V_0 = blockRange.Start.get_First().get_Offset();
			V_1 = blockRange.End.get_First().get_Offset();
			V_2 = 0;
			while (V_2 < (int)this.context.get_CFG().get_Blocks().Length)
			{
				V_3 = this.context.get_CFG().get_Blocks()[V_2];
				if (V_3.get_First().get_Offset() >= V_0 && V_3.get_First().get_Offset() <= V_1)
				{
					return V_3;
				}
				V_2 = V_2 + 1;
			}
			throw new Exception("Invalid range");
		}

		private HashSet<ILogicalConstruct> GetLogicalConstructsInRange(BlockRange blockRange, out ILogicalConstruct theCommonParent)
		{
			V_0 = new HashSet<ILogicalConstruct>();
			V_1 = new HashSet<ISingleEntrySubGraph>();
			V_2 = blockRange.Start.get_First().get_Offset();
			V_3 = blockRange.End.get_First().get_Offset();
			V_5 = 0;
			while (V_5 < (int)this.context.get_CFG().get_Blocks().Length)
			{
				V_6 = this.context.get_CFG().get_Blocks()[V_5];
				if (V_6.get_First().get_Offset() >= V_2 && V_6.get_First().get_Offset() <= V_3)
				{
					V_7 = this.context.get_CFGBlockToLogicalConstructMap().get_Item(V_6);
					V_8 = 0;
					while (V_8 < (int)V_7.Length)
					{
						dummyVar0 = V_1.Add((ILogicalConstruct)V_7[V_8].get_Parent());
						dummyVar1 = V_0.Add(V_7[V_8]);
						V_8 = V_8 + 1;
					}
				}
				V_5 = V_5 + 1;
			}
			if (V_1.get_Count() == 1)
			{
				theCommonParent = (ILogicalConstruct)V_1.ToArray<ISingleEntrySubGraph>()[0];
				return V_0;
			}
			theCommonParent = (ILogicalConstruct)LogicalFlowUtilities.FindFirstCommonParent(V_1);
			V_4 = new HashSet<ILogicalConstruct>();
			V_9 = V_0.GetEnumerator();
			try
			{
				while (V_9.MoveNext())
				{
					dummyVar2 = LogicalFlowUtilities.TryGetParentConstructWithGivenParent(V_9.get_Current(), theCommonParent, out V_10);
					dummyVar3 = V_4.Add(V_10);
				}
			}
			finally
			{
				((IDisposable)V_9).Dispose();
			}
			if (theCommonParent as ExceptionHandlingLogicalConstruct != null)
			{
				V_4.Clear();
				dummyVar4 = V_4.Add(theCommonParent);
				theCommonParent = theCommonParent.get_Parent() as ILogicalConstruct;
			}
			return V_4;
		}
	}
}