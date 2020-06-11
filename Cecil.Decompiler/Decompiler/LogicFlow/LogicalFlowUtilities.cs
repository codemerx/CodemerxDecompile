using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Ast.Expressions;
using System;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
    static class LogicalFlowUtilities
    {
		/// <summary>
		/// Returns the closest guarded block (try/catch/filter/fault/finally) that encloses the given construct.
		/// </summary>
		/// <param name="start">The construct for which the guarded block parent should be found</param>
		/// <returns></returns>
		public static BlockLogicalConstruct GetNearestGuardedBlock(ILogicalConstruct start)
		{
			ILogicalConstruct nearestGuardedBlockCandidate = start;
			while (nearestGuardedBlockCandidate != null &&
				  !(nearestGuardedBlockCandidate is ExceptionHandlingLogicalConstruct))
			{
				nearestGuardedBlockCandidate = (ILogicalConstruct)nearestGuardedBlockCandidate.Parent;
			}

			if (nearestGuardedBlockCandidate == null)
			{
				return null;
			}

			if ((nearestGuardedBlockCandidate as ExceptionHandlingLogicalConstruct).Try.CFGBlocks.Contains(start.FirstBlock))
			{
				return (nearestGuardedBlockCandidate as ExceptionHandlingLogicalConstruct).Try;
			}

			if (nearestGuardedBlockCandidate is TryCatchFilterLogicalConstruct)
			{
				TryCatchFilterLogicalConstruct tcf = nearestGuardedBlockCandidate as TryCatchFilterLogicalConstruct;
				foreach(IFilteringExceptionHandler handler in tcf.Handlers)
				{
					if (handler.HandlerType == FilteringExceptionHandlerType.Filter)
					{
						if ((handler as ExceptionHandlingBlockFilter).Filter.CFGBlocks.Contains(start.FirstBlock))
						{
							return (handler as ExceptionHandlingBlockFilter).Filter;
						}
						else
						{
							return (handler as ExceptionHandlingBlockFilter).Handler;
						}
					}
					else if (handler.HandlerType == FilteringExceptionHandlerType.Catch)
					{
						return handler as ExceptionHandlingBlockCatch;
					}
				}
			}
			else if (nearestGuardedBlockCandidate is TryFinallyLogicalConstruct)
			{
				return (nearestGuardedBlockCandidate as TryFinallyLogicalConstruct).Finally;
			}
			else if (nearestGuardedBlockCandidate is TryFaultLogicalConstruct)
			{
				return (nearestGuardedBlockCandidate as TryFaultLogicalConstruct).Fault;
			}
			else
			{
				throw new Exception("Unknown type of exception handling logical construct encountered.");
			}

			return null;
		}

        /// <summary>
        /// Retruns the first common parent of the enumeration members. This is, the logical construct that is the first one to hold all of them.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
		public static ISingleEntrySubGraph FindFirstCommonParent(IEnumerable<ISingleEntrySubGraph> blocks)
        {

			Queue<ISingleEntrySubGraph> currentCommonParentsOrdered = new Queue<ISingleEntrySubGraph>();
			HashSet<ISingleEntrySubGraph> currentCommonParentsSearchable = new HashSet<ISingleEntrySubGraph>();

			foreach (ISingleEntrySubGraph construct in blocks)
			{
				if (currentCommonParentsOrdered.Count == 0) //first pass
				{
					ISingleEntrySubGraph currentParent = construct;
					while (currentParent != null)
					{
						currentCommonParentsOrdered.Enqueue(currentParent);
						currentCommonParentsSearchable.Add(currentParent);
						currentParent = currentParent.Parent;
					}
				}
				else
				{
                    ISingleEntrySubGraph firstConstructsParent = construct;
                    while (!currentCommonParentsSearchable.Contains(firstConstructsParent))
                    {
                        firstConstructsParent = firstConstructsParent.Parent;
                    }

                    if (currentCommonParentsSearchable.Contains(firstConstructsParent))
					{
                        while (currentCommonParentsOrdered.Peek() != firstConstructsParent)
						{
							ISingleEntrySubGraph removed = currentCommonParentsOrdered.Dequeue();
                            currentCommonParentsSearchable.Remove(removed);
						}
					}

				}
			}

			return currentCommonParentsOrdered.Peek();
        }

        /// <summary>
        /// Gets a sorted collection of all same parent successors.
        /// </summary>
        /// <param name="construct"></param>
        /// <returns></returns>
        public static ICollection<ISingleEntrySubGraph> GetTraversableSuccessors(ISingleEntrySubGraph construct)
        {
            List<ISingleEntrySubGraph> traversableSuccessors = new List<ISingleEntrySubGraph>(construct.SameParentSuccessors);
            traversableSuccessors.Sort();
            return traversableSuccessors;
        }

        public static ICollection<ISingleEntrySubGraph> GetTraversablePredecessors(ISingleEntrySubGraph construct)
        {
            List<ISingleEntrySubGraph> traversablePredecessors = new List<ISingleEntrySubGraph>(construct.SameParentPredecessors);
            traversablePredecessors.Sort();
            return traversablePredecessors;
        }

        /// <summary>
        /// Tries to get the construct that contains the initial node and has for parent the specified parent.
        /// </summary>
        /// <param name="initialNode"></param>
        /// <param name="parent"></param>
        /// <param name="desiredNode"></param>
        /// <returns></returns>
        public static bool TryGetParentConstructWithGivenParent(ILogicalConstruct initialNode, ILogicalConstruct parent, out ILogicalConstruct desiredNode)
        {
            if(initialNode == null || parent == null)
            {
                desiredNode = null;
                return false;
            }

            desiredNode = initialNode;
            while(desiredNode.Parent != null)
            {
                if(desiredNode.Parent == parent)
                {
                    return true;
                }
                desiredNode = desiredNode.Parent as ILogicalConstruct;
            }

            desiredNode = null;
            return false;
        }

        /// <summary>
        /// Splits the given CFG construct into two partial constructs. The first containing the expressions with indices 0 to expressionIndex - 1, the second -
        /// expressionIndex to end. This method modifies the logicalContext.CFGBlockToLogicalConstructMap
        /// </summary>
        /// <param name="logicalContext"></param>
        /// <param name="cfgBlock"></param>
        /// <param name="expressionIndex"></param>
        /// <returns>A key value pair containing the first partial construct as key and the second as value.</returns>
        public static KeyValuePair<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct> SplitCFGBlockAt(LogicalFlowBuilderContext logicalContext,
            CFGBlockLogicalConstruct cfgBlock, int expressionIndex)
        {
            List<Expression> blockExpressions = cfgBlock.LogicalConstructExpressions;

            if(blockExpressions == null)
            {
                throw new ArgumentNullException("blockExpressions");
            }

            if(expressionIndex <= 0 || expressionIndex >= blockExpressions.Count)
            {
                throw new ArgumentOutOfRangeException("expressionIndex");
            }

            CFGBlockLogicalConstruct[] instructionBlockConstructs = logicalContext.CFGBlockToLogicalConstructMap[cfgBlock.TheBlock];
            int cfgBlockArrayLength = instructionBlockConstructs.Length;

            int cfgBlockIndex;
            for (cfgBlockIndex = 0; cfgBlockIndex < cfgBlockArrayLength; cfgBlockIndex++)
            {
                if(cfgBlock == instructionBlockConstructs[cfgBlockIndex])
                {
                    break;
                }
            }

            if(cfgBlockIndex == cfgBlockArrayLength)
            {
                throw new ArgumentException("cfgBlock");
            }

            List<Expression> firstExpressionsList = cfgBlock.LogicalConstructExpressions.GetRange(0, expressionIndex);
            PartialCFGBlockLogicalConstruct firstPartial = new PartialCFGBlockLogicalConstruct(cfgBlock, firstExpressionsList);
            firstPartial.RedirectPredecessors();

            List<Expression> secondExpressionsList = cfgBlock.LogicalConstructExpressions.GetRange(expressionIndex, blockExpressions.Count - expressionIndex);
            PartialCFGBlockLogicalConstruct secondPartial = new PartialCFGBlockLogicalConstruct(cfgBlock, secondExpressionsList);
            secondPartial.RedirectSuccessors();

            firstPartial.AddToSuccessors(secondPartial);
            secondPartial.AddToPredecessors(firstPartial);

            CFGBlockLogicalConstruct[] updatedCollection = new CFGBlockLogicalConstruct[cfgBlockArrayLength + 1];
            for (int i = 0, j = 0; i < cfgBlockArrayLength; i++, j++)
            {
                if (i != cfgBlockIndex)
                {
                    updatedCollection[j] = instructionBlockConstructs[i];
                }
                else
                {
                    updatedCollection[j] = firstPartial;
                    updatedCollection[++j] = secondPartial;
                }
            }

            logicalContext.CFGBlockToLogicalConstructMap[cfgBlock.TheBlock] = updatedCollection;

            return new KeyValuePair<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct>(firstPartial, secondPartial);
        }
    }
}
