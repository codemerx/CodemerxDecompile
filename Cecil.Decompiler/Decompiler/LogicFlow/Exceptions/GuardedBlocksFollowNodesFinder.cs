using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions
{
	public static class GuardedBlocksFollowNodesFinder
	{

		internal static CFGBlockLogicalConstruct FindGuardedBlockCFGFollowNode(ExceptionHandlingLogicalConstruct construct, HashSet<CFGBlockLogicalConstruct> outOfconsideration)
		{
			//TODO: Don't consider leaves to blocks that end with return. The return can simpy be copied to the place where the leave originates by
			// the statement decompielr or the goto elimination later.


			//find all the targets the try/catch/filter/fault/finally blocks in the construct jump/leave to
			//record the number of leaves to each target
			Dictionary<CFGBlockLogicalConstruct, uint> numberOfHandlersLeavingToBlock = new Dictionary<CFGBlockLogicalConstruct, uint>();
			AddSuccessorsToCount(construct.Try, numberOfHandlersLeavingToBlock);
			uint leavableConstructsCount = 1;

			if (construct is TryCatchFilterLogicalConstruct)
			{
				TryCatchFilterLogicalConstruct tcf = construct as TryCatchFilterLogicalConstruct;
				foreach (IFilteringExceptionHandler handler in tcf.Handlers)
				{
					AddSuccessorsToCount(handler, numberOfHandlersLeavingToBlock);
					leavableConstructsCount++;
				}
			}
			else if (construct is TryFaultLogicalConstruct)
			{
				AddSuccessorsToCount((construct as TryFaultLogicalConstruct).Fault, numberOfHandlersLeavingToBlock);
				leavableConstructsCount++;
			}
			else if (construct is TryFinallyLogicalConstruct)
			{
				AddSuccessorsToCount((construct as TryFinallyLogicalConstruct).Finally, numberOfHandlersLeavingToBlock);
				leavableConstructsCount++;
			}

			//remove the leave targets taht should nto be considered
			foreach (CFGBlockLogicalConstruct ooc in outOfconsideration)
			{
				if (numberOfHandlersLeavingToBlock.ContainsKey(ooc))
				{
					numberOfHandlersLeavingToBlock.Remove(ooc);
				}
			}

			if(numberOfHandlersLeavingToBlock.Count == 0)
			{
				return null;
			}

			//find the leave targets that greatest number of exception handling blocks (try/catch/filter/fault/finally) jump to
			HashSet<CFGBlockLogicalConstruct> mostBlocksExitTo = new HashSet<CFGBlockLogicalConstruct>();
			CFGBlockLogicalConstruct randomLeaveTarget = numberOfHandlersLeavingToBlock.Keys.FirstOrDefault<CFGBlockLogicalConstruct>();
			mostBlocksExitTo.Add(randomLeaveTarget);
			uint maxNumberOfLeaveTargetPredecessors = numberOfHandlersLeavingToBlock[randomLeaveTarget];
			foreach (CFGBlockLogicalConstruct leaveTarget in numberOfHandlersLeavingToBlock.Keys)
			{
				if (numberOfHandlersLeavingToBlock[leaveTarget] > maxNumberOfLeaveTargetPredecessors)
				{
					maxNumberOfLeaveTargetPredecessors = numberOfHandlersLeavingToBlock[leaveTarget];
					mostBlocksExitTo.Clear();
					mostBlocksExitTo.Add(leaveTarget);
				}
				else if (numberOfHandlersLeavingToBlock[leaveTarget] == maxNumberOfLeaveTargetPredecessors)
				{
					mostBlocksExitTo.Add(leaveTarget);
				}
			}

			//use various heuristics to determine the best follow node
			//the follow node that will result in the smallest number of gotos is considered superior
			//the follow node could be changed once loops are created if it turns out loop condition was chosen as a follow node (i.e. we chose a continue edge)
			if (mostBlocksExitTo.Count == 1)
			{
				return mostBlocksExitTo.FirstOrDefault<CFGBlockLogicalConstruct>();
			}
			else
			{
				HashSet<CFGBlockLogicalConstruct> mostLeavesPointTo = new HashSet<CFGBlockLogicalConstruct>();
				uint maxLeavesToSingleTarget = 0;
				foreach (CFGBlockLogicalConstruct leaveTarget in mostBlocksExitTo)
				{
					uint leavesToThisTarget = 0;
                    HashSet<CFGBlockLogicalConstruct> constructCFGBlocks = construct.CFGBlocks;
					foreach (CFGBlockLogicalConstruct leaveTargetpredecessor in leaveTarget.CFGPredecessors)
					{
						if (constructCFGBlocks.Contains(leaveTargetpredecessor))
						{
							leavesToThisTarget++;
						}
					}

					if (leavesToThisTarget >= maxLeavesToSingleTarget)
					{
						if (leavesToThisTarget > maxLeavesToSingleTarget)
						{
							maxLeavesToSingleTarget = leavesToThisTarget;
							mostLeavesPointTo.Clear();
						}
						mostLeavesPointTo.Add(leaveTarget);
					}
				}

				//TODO: Try anoher heuristics here to chose the follow node that will minimize the number of gotos

				CFGBlockLogicalConstruct result = mostLeavesPointTo.FirstOrDefault<CFGBlockLogicalConstruct>();
				//HEURISTICS: Of all possible follow nodes we try to chose the one which start index is greater than the index of the exception construct start
				//but less than the start indexes of all other possible candidates. We rely on the assumption that the control flow closely resembles the ordering of
 				// the IL instructions, i.e. if Instr. A  comes before Instr. B the chances are that Instr. A will have to be executed before Instr. B in any workflow.
				// That might not be the case but it's a good assumption since the compilers will try to express teh control flow in the least amount of jumps possible
				// for performance reasons.
				if(result.Index < construct.Entry.Index)
				{
					CFGBlockLogicalConstruct closestAfterConstruct = null;
					foreach (CFGBlockLogicalConstruct candidate in mostLeavesPointTo)
					{
						if (candidate.Index > construct.Entry.Index)
						{
							if (closestAfterConstruct != null && closestAfterConstruct.Index < candidate.Index)
							{
								continue;
							}
							closestAfterConstruct = candidate;
						}
					}

					if (closestAfterConstruct != null)
					{
						result = closestAfterConstruct;
					}
				}
				return result;
			}
		}

		private static void AddSuccessorsToCount(ILogicalConstruct construct, Dictionary<CFGBlockLogicalConstruct, uint> numberOfHandlersLeavingToBlock)
		{
			foreach (CFGBlockLogicalConstruct successor in construct.CFGSuccessors)
			{
				if (!numberOfHandlersLeavingToBlock.ContainsKey(successor))
				{
					numberOfHandlersLeavingToBlock.Add(successor, 1);
					continue;
				}

				numberOfHandlersLeavingToBlock[successor]++;
			}
		}
	}
}
