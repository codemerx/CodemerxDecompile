using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions
{
	public static class GuardedBlocksFollowNodesFinder
	{
		private static void AddSuccessorsToCount(ILogicalConstruct construct, Dictionary<CFGBlockLogicalConstruct, uint> numberOfHandlersLeavingToBlock)
		{
			foreach (CFGBlockLogicalConstruct cFGSuccessor in construct.CFGSuccessors)
			{
				if (numberOfHandlersLeavingToBlock.ContainsKey(cFGSuccessor))
				{
					Dictionary<CFGBlockLogicalConstruct, uint> item = numberOfHandlersLeavingToBlock;
					CFGBlockLogicalConstruct cFGBlockLogicalConstruct = cFGSuccessor;
					item[cFGBlockLogicalConstruct] = item[cFGBlockLogicalConstruct] + 1;
				}
				else
				{
					numberOfHandlersLeavingToBlock.Add(cFGSuccessor, 1);
				}
			}
		}

		internal static CFGBlockLogicalConstruct FindGuardedBlockCFGFollowNode(ExceptionHandlingLogicalConstruct construct, HashSet<CFGBlockLogicalConstruct> outOfconsideration)
		{
			Dictionary<CFGBlockLogicalConstruct, uint> cFGBlockLogicalConstructs = new Dictionary<CFGBlockLogicalConstruct, uint>();
			GuardedBlocksFollowNodesFinder.AddSuccessorsToCount(construct.Try, cFGBlockLogicalConstructs);
			uint num = 1;
			if (construct is TryCatchFilterLogicalConstruct)
			{
				IFilteringExceptionHandler[] handlers = (construct as TryCatchFilterLogicalConstruct).Handlers;
				for (int i = 0; i < (int)handlers.Length; i++)
				{
					GuardedBlocksFollowNodesFinder.AddSuccessorsToCount(handlers[i], cFGBlockLogicalConstructs);
					num++;
				}
			}
			else if (construct is TryFaultLogicalConstruct)
			{
				GuardedBlocksFollowNodesFinder.AddSuccessorsToCount((construct as TryFaultLogicalConstruct).Fault, cFGBlockLogicalConstructs);
				num++;
			}
			else if (construct is TryFinallyLogicalConstruct)
			{
				GuardedBlocksFollowNodesFinder.AddSuccessorsToCount((construct as TryFinallyLogicalConstruct).Finally, cFGBlockLogicalConstructs);
				num++;
			}
			foreach (CFGBlockLogicalConstruct cFGBlockLogicalConstruct in outOfconsideration)
			{
				if (!cFGBlockLogicalConstructs.ContainsKey(cFGBlockLogicalConstruct))
				{
					continue;
				}
				cFGBlockLogicalConstructs.Remove(cFGBlockLogicalConstruct);
			}
			if (cFGBlockLogicalConstructs.Count == 0)
			{
				return null;
			}
			HashSet<CFGBlockLogicalConstruct> cFGBlockLogicalConstructs1 = new HashSet<CFGBlockLogicalConstruct>();
			CFGBlockLogicalConstruct cFGBlockLogicalConstruct1 = cFGBlockLogicalConstructs.Keys.FirstOrDefault<CFGBlockLogicalConstruct>();
			cFGBlockLogicalConstructs1.Add(cFGBlockLogicalConstruct1);
			uint item = cFGBlockLogicalConstructs[cFGBlockLogicalConstruct1];
			foreach (CFGBlockLogicalConstruct key in cFGBlockLogicalConstructs.Keys)
			{
				if (cFGBlockLogicalConstructs[key] <= item)
				{
					if (cFGBlockLogicalConstructs[key] != item)
					{
						continue;
					}
					cFGBlockLogicalConstructs1.Add(key);
				}
				else
				{
					item = cFGBlockLogicalConstructs[key];
					cFGBlockLogicalConstructs1.Clear();
					cFGBlockLogicalConstructs1.Add(key);
				}
			}
			if (cFGBlockLogicalConstructs1.Count == 1)
			{
				return cFGBlockLogicalConstructs1.FirstOrDefault<CFGBlockLogicalConstruct>();
			}
			HashSet<CFGBlockLogicalConstruct> cFGBlockLogicalConstructs2 = new HashSet<CFGBlockLogicalConstruct>();
			uint num1 = 0;
			foreach (CFGBlockLogicalConstruct cFGBlockLogicalConstruct2 in cFGBlockLogicalConstructs1)
			{
				uint num2 = 0;
				HashSet<CFGBlockLogicalConstruct> cFGBlocks = construct.CFGBlocks;
				foreach (CFGBlockLogicalConstruct cFGPredecessor in cFGBlockLogicalConstruct2.CFGPredecessors)
				{
					if (!cFGBlocks.Contains(cFGPredecessor))
					{
						continue;
					}
					num2++;
				}
				if (num2 < num1)
				{
					continue;
				}
				if (num2 > num1)
				{
					num1 = num2;
					cFGBlockLogicalConstructs2.Clear();
				}
				cFGBlockLogicalConstructs2.Add(cFGBlockLogicalConstruct2);
			}
			CFGBlockLogicalConstruct cFGBlockLogicalConstruct3 = cFGBlockLogicalConstructs2.FirstOrDefault<CFGBlockLogicalConstruct>();
			if (cFGBlockLogicalConstruct3.Index < construct.Entry.Index)
			{
				CFGBlockLogicalConstruct cFGBlockLogicalConstruct4 = null;
				foreach (CFGBlockLogicalConstruct cFGBlockLogicalConstruct5 in cFGBlockLogicalConstructs2)
				{
					if (cFGBlockLogicalConstruct5.Index <= construct.Entry.Index || cFGBlockLogicalConstruct4 != null && cFGBlockLogicalConstruct4.Index < cFGBlockLogicalConstruct5.Index)
					{
						continue;
					}
					cFGBlockLogicalConstruct4 = cFGBlockLogicalConstruct5;
				}
				if (cFGBlockLogicalConstruct4 != null)
				{
					cFGBlockLogicalConstruct3 = cFGBlockLogicalConstruct4;
				}
			}
			return cFGBlockLogicalConstruct3;
		}
	}
}