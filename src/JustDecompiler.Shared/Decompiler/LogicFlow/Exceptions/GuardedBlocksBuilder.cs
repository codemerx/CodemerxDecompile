using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions
{
	class GuardedBlocksBuilder
	{

		LogicalFlowBuilderContext context;
		List<int> sortedBlockStarts;
		Dictionary<BlockRange, ExceptionHandlingLogicalConstruct> tryBlocksFound;

		public GuardedBlocksBuilder(LogicalFlowBuilderContext Context)
		{
			context = Context;
			sortedBlockStarts = context.InstructionToCFGBlockMapping.Keys.ToList<int>();
			sortedBlockStarts.Sort();
			tryBlocksFound = new Dictionary<BlockRange, ExceptionHandlingLogicalConstruct>();
		}

		public void FindExceptionHandlingConstructs()
		{
			//The CLR searchese top to bottom through the EH table at the end of the method declaration looking for a CATCH or FILTER handlers
			//which guarded(try) block encompasses the spot where the exception occurred (FAULT and FINALLY handlers are compeltely ignored at this pass,
			//no matter whether the exception falls in their guarded(try) blocks). Assuming that CATCH/FILTER handler was found at row N of the EH table
			// the CLR marks it (but it does NOT execute it at this point) and does a second pass through the EH table searching ONLY rows 1 to N this time.
			//On hte second pass only FAULT/FINALLY handlers are considered. Any FAULT/FILTER handler found which guarded block encompasses the instruction causing
			//the exception gets executed at this pass. Only then the CATCH/FILTER handler at row N gets executed.
			//That means that the order in which handlers appear in the EH table has precedence over their hierarchical structure in the IL (i.e. which try block is inside another try block) when determineing
			//their order of execution. Hence given a FAULT/FINALLY handler that completely contains a CATCH/FILTER handler in the IL but whose EH table entry occurs before 
			// the CATCH/FILTER  handler EH table entry in the LogicalConstruct tree the CATCH/FILTER handler will be parent of the FAULT/FINALLY handler (complete reverse of theri relationship in the IL)
			//since the FAULT/FINALLY handler will be executed before the CATCH/FILTER one.


			foreach (ExceptionHandler handler in context.CFG.RawExceptionHandlers)
			{
                BlockRange tryBlockRange = GetBlockRangeFromInstructions(handler.TryStart, handler.TryEnd);

				ExceptionHandlingLogicalConstruct tryBlockExistingHandler = null;
				if (tryBlocksFound.TryGetValue(tryBlockRange, out tryBlockExistingHandler))
				{
					AddHandlerToTryBlock(tryBlockExistingHandler, handler);
				}
				else
				{
					BlockLogicalConstruct theTryBlock = CreateExceptionhandlingBlock(tryBlockRange);
					ExceptionHandlingLogicalConstruct guardedBlock = null;

					switch (handler.HandlerType)
					{
						case ExceptionHandlerType.Catch:	
							guardedBlock = new TryCatchFilterLogicalConstruct(theTryBlock, CreateCatchHandler(handler));
						break;
						case ExceptionHandlerType.Filter:		
							guardedBlock = new TryCatchFilterLogicalConstruct(theTryBlock, CreateFilterHandler(handler));
						break;
						case ExceptionHandlerType.Fault:
                            BlockRange faultBlockRange = GetBlockRangeFromInstructions(handler.HandlerStart, handler.HandlerEnd);
						    guardedBlock = new TryFaultLogicalConstruct(theTryBlock, CreateExceptionhandlingBlock(faultBlockRange));							
						break;
						case ExceptionHandlerType.Finally:
                            BlockRange finallyBlockRange = GetBlockRangeFromInstructions(handler.HandlerStart, handler.HandlerEnd);
						    guardedBlock = new TryFinallyLogicalConstruct(theTryBlock, CreateExceptionhandlingBlock(finallyBlockRange));	
						break;
					}

					tryBlocksFound.Add(tryBlockRange, guardedBlock);
				}
			}
		}

  
		private int FindBlockStartOffset(int blockLastInstructionOffset)
		{
			int endBlockFirstInstructionOffset = -1;
			foreach (int blockStartOffset in sortedBlockStarts)
			{
				if (blockStartOffset <= blockLastInstructionOffset)
				{
					endBlockFirstInstructionOffset = blockStartOffset;
				}
				else
				{
					break;
				}
			}
			return endBlockFirstInstructionOffset;
		}

		private ExceptionHandlingBlockCatch CreateCatchHandler(ExceptionHandler handler)
		{
			ILogicalConstruct theCommonParent;
            BlockRange catchBlockRange = GetBlockRangeFromInstructions(handler.HandlerStart, handler.HandlerEnd);
            HashSet<ILogicalConstruct> catchChildren = GetLogicalConstructsInRange(catchBlockRange, out theCommonParent);
			return new ExceptionHandlingBlockCatch(catchChildren, context.CFGBlockToLogicalConstructMap[catchBlockRange.Start][0], handler.CatchType);
		}

		private ExceptionHandlingBlockFilter CreateFilterHandler(ExceptionHandler handler)
		{
			ILogicalConstruct theCommonParent;
            BlockRange filterBlockRange = GetBlockRangeFromInstructions(handler.FilterStart, handler.FilterEnd);
            HashSet<ILogicalConstruct> filterChildren = GetLogicalConstructsInRange(filterBlockRange, out theCommonParent);
            if(FindFollowNodes(filterChildren) != null)
            {
                throw new Exception("The filter block must not have a follow node");
            }
			BlockLogicalConstruct filterConstruct = new BlockLogicalConstruct(context.CFGBlockToLogicalConstructMap[filterBlockRange.Start][0], filterChildren);

            BlockRange handlerBlockRange = GetBlockRangeFromInstructions(handler.HandlerStart, handler.HandlerEnd);
            HashSet<ILogicalConstruct> handlerChildren = GetLogicalConstructsInRange(handlerBlockRange, out theCommonParent);
			BlockLogicalConstruct handlerConstruct = new BlockLogicalConstruct(context.CFGBlockToLogicalConstructMap[handlerBlockRange.Start][0],
																			   handlerChildren);

			return new ExceptionHandlingBlockFilter(filterConstruct, handlerConstruct);
		}

		ILogicalConstruct FindFollowNodes(HashSet<ILogicalConstruct> children)
		{
			HashSet<ILogicalConstruct> possibleFollowNodes = new HashSet<ILogicalConstruct>();
			foreach (ILogicalConstruct child in children)
			{
				//CFGBlockLogicalConstruct cfgChild = child as CFGBlockLogicalConstruct;
				HashSet<ISingleEntrySubGraph> allSuccessors = child.AllSuccessors;
				//if (cfgChild == null && allSuccessors.Count > 0)
				//{
				//    //IEnumerator<ISingleEntrySubGraph> successorEnumerator = allSuccessors.GetEnumerator();
				//    //successorEnumerator.MoveNext();
				//    ISingleEntrySubGraph firstSuccessor = allSuccessors.FirstOrDefault<ISingleEntrySubGraph>();
				//    if (!children.Contains(firstSuccessor as ILogicalConstruct))
				//    {
				//        possibleFollowNodes.Add(firstSuccessor as ILogicalConstruct);
				//    }
				//}
				//else if(cfgChild != null && allSuccessors.Count > 0)
				//{
				foreach (ILogicalConstruct successor in allSuccessors)
				{
					if (!children.Contains(successor))
					{
						//if(cfgChild.TheBlock.Last.OpCode.Code == Code.Leave || cfgChild.TheBlock.Last.OpCode.Code == Code.Leave_S)
						//{
						possibleFollowNodes.Add(successor);
						//}
						//else
						//{
						//    throw new Exception("Illegal exit of Try/Handler block");
						//}
					}
				}
				// }
			}
			return possibleFollowNodes.FirstOrDefault<ILogicalConstruct>();
		}

		private void AddHandlerToTryBlock(ExceptionHandlingLogicalConstruct tryBlockExistingHandler, ExceptionHandler handler)
		{

			if (tryBlockExistingHandler is TryCatchFilterLogicalConstruct)
			{
				if (handler.HandlerType == ExceptionHandlerType.Fault || handler.HandlerType == ExceptionHandlerType.Finally)
				{
					//Fault/finally handlers cannot coexist together with any other type of handler on the same try block. New
					// try block should be defined for each of these.
					throw new Exception("Illegal IL: Non-exclusive Fault/Finally handler found");
				}
				else if (handler.HandlerType == ExceptionHandlerType.Filter)
				{
					(tryBlockExistingHandler as TryCatchFilterLogicalConstruct).AddHandler(CreateFilterHandler(handler));
				}
				else if (handler.HandlerType == ExceptionHandlerType.Catch)
				{
					(tryBlockExistingHandler as TryCatchFilterLogicalConstruct).AddHandler(CreateCatchHandler(handler));
				}
			}
			else if (tryBlockExistingHandler is TryFaultLogicalConstruct || tryBlockExistingHandler is TryFinallyLogicalConstruct)
			{
				//Fault/finally handlers cannot coexist together with any other type of handler on the same try block. New
				// try block should be defined for each of these.
				throw new Exception("Illegal IL: Non-exclusive Fault/Finally handler found");
			}
		}

		private HashSet<ILogicalConstruct> GetLogicalConstructsInRange(BlockRange blockRange, out ILogicalConstruct theCommonParent)
		{
			HashSet<ILogicalConstruct> children = new HashSet<ILogicalConstruct>();
			HashSet<ISingleEntrySubGraph> blocksParents = new HashSet<ISingleEntrySubGraph>();

            int rangeBegin = blockRange.Start.First.Offset;
            int rangeEnd = blockRange.End.First.Offset;
            for (int i = 0; i < context.CFG.Blocks.Length; i++)
            {
                InstructionBlock currentBlock = context.CFG.Blocks[i];
                if(currentBlock.First.Offset >= rangeBegin && currentBlock.First.Offset <= rangeEnd)
                {
                    CFGBlockLogicalConstruct[] cfgConstructs = context.CFGBlockToLogicalConstructMap[currentBlock];

                    for (int j = 0; j < cfgConstructs.Length; j++)
                    {
                        blocksParents.Add((ILogicalConstruct)cfgConstructs[j].Parent);
                        children.Add(cfgConstructs[j]);
                    }
                }
            }

			if (blocksParents.Count == 1)
			{
				theCommonParent = (ILogicalConstruct)blocksParents.ToArray<ISingleEntrySubGraph>()[0];

				return children;
			}
				//TODO: CCheck whether the parent logical construct of of each CFGBlockLogicalConstruct that belongs to the exception handling block,
				//(i.e. each member of blocksParents) contains as children ONLY blocks that belong to the exception handling block (i.e. blocks that are in blockRange).

			theCommonParent = (ILogicalConstruct)LogicalFlowUtilities.FindFirstCommonParent(blocksParents);

			HashSet<ILogicalConstruct> result = new HashSet<ILogicalConstruct>();
            foreach (ILogicalConstruct child in children)
            {
                ILogicalConstruct desiredNode;
                LogicalFlowUtilities.TryGetParentConstructWithGivenParent(child, theCommonParent, out desiredNode);
                result.Add(desiredNode);
            }

            if (theCommonParent is ExceptionHandlingLogicalConstruct)
            {
                result.Clear();
                result.Add(theCommonParent);
                theCommonParent = theCommonParent.Parent as ILogicalConstruct;
            }

			return result;
		}

		ILogicalConstruct FindChildBlockBelongsTo(ILogicalConstruct parent, CFGBlockLogicalConstruct block)
		{
			ILogicalConstruct result = null, current = block;

			do
			{
				if (current.Parent == parent)
				{
					result = current;
					break;
				}

				current = (ILogicalConstruct)current.Parent;

			} while (current.Parent != null);

			return result;
		}

		BlockLogicalConstruct CreateExceptionhandlingBlock(BlockRange cfgBlocks)
		{
			HashSet<ILogicalConstruct> children;
			ILogicalConstruct theCommonParent;

			children = GetLogicalConstructsInRange(cfgBlocks, out theCommonParent);
			ILogicalConstruct entry = FindChildBlockBelongsTo(theCommonParent, context.CFGBlockToLogicalConstructMap[GetEntryBlockInRange(cfgBlocks)][0]);
			return new BlockLogicalConstruct(entry, children);
		}

        private InstructionBlock GetEntryBlockInRange(BlockRange blockRange)
        {
            int rangeBegin = blockRange.Start.First.Offset;
            int rangeEnd = blockRange.End.First.Offset;

            for (int i = 0; i < context.CFG.Blocks.Length; i++)
            {
                InstructionBlock currentBlock = context.CFG.Blocks[i];
                if (currentBlock.First.Offset >= rangeBegin && currentBlock.First.Offset <= rangeEnd)
                {
                    return currentBlock;
                }
            }

            throw new Exception("Invalid range");
        }

        private BlockRange GetBlockRangeFromInstructions(Instruction start, Instruction end)
        {
            InstructionBlock startBlock = context.CFG.InstructionToBlockMapping[start.Offset];
            //In the EH table the block end is marked by the offset of the FIRST instruction AFTER the actual end of the block.
            //Mono.Cecil preserves that notation so we need to move one istruction back to get the actual last instruction offset.
            int endOffset = end != null ? end.Previous.Offset : context.CFG.MethodBody.CodeSize;
            return new BlockRange(startBlock, context.CFG.InstructionToBlockMapping[FindBlockStartOffset(endOffset)]);
        }
	}
}
