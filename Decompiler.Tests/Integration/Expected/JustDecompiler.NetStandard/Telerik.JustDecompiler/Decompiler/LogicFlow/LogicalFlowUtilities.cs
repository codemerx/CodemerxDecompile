using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	internal static class LogicalFlowUtilities
	{
		public static ISingleEntrySubGraph FindFirstCommonParent(IEnumerable<ISingleEntrySubGraph> blocks)
		{
			V_0 = new Queue<ISingleEntrySubGraph>();
			V_1 = new HashSet<ISingleEntrySubGraph>();
			V_2 = blocks.GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					if (V_0.get_Count() != 0)
					{
						V_5 = V_3;
						while (!V_1.Contains(V_5))
						{
							V_5 = V_5.get_Parent();
						}
						if (!V_1.Contains(V_5))
						{
							continue;
						}
						while (V_0.Peek() != V_5)
						{
							dummyVar1 = V_1.Remove(V_0.Dequeue());
						}
					}
					else
					{
						V_4 = V_3;
						while (V_4 != null)
						{
							V_0.Enqueue(V_4);
							dummyVar0 = V_1.Add(V_4);
							V_4 = V_4.get_Parent();
						}
					}
				}
			}
			finally
			{
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
			return V_0.Peek();
		}

		public static BlockLogicalConstruct GetNearestGuardedBlock(ILogicalConstruct start)
		{
			V_0 = start;
			while (V_0 != null && V_0 as ExceptionHandlingLogicalConstruct == null)
			{
				V_0 = (ILogicalConstruct)V_0.get_Parent();
			}
			if (V_0 == null)
			{
				return null;
			}
			if ((V_0 as ExceptionHandlingLogicalConstruct).get_Try().get_CFGBlocks().Contains(start.get_FirstBlock()))
			{
				return (V_0 as ExceptionHandlingLogicalConstruct).get_Try();
			}
			if (V_0 as TryCatchFilterLogicalConstruct == null)
			{
				if (V_0 as TryFinallyLogicalConstruct != null)
				{
					return (V_0 as TryFinallyLogicalConstruct).get_Finally();
				}
				if (V_0 as TryFaultLogicalConstruct == null)
				{
					throw new Exception("Unknown type of exception handling logical construct encountered.");
				}
				return (V_0 as TryFaultLogicalConstruct).get_Fault();
			}
			V_1 = (V_0 as TryCatchFilterLogicalConstruct).get_Handlers();
			V_2 = 0;
			while (V_2 < (int)V_1.Length)
			{
				V_3 = V_1[V_2];
				if (V_3.get_HandlerType() == 1)
				{
					if ((V_3 as ExceptionHandlingBlockFilter).get_Filter().get_CFGBlocks().Contains(start.get_FirstBlock()))
					{
						return (V_3 as ExceptionHandlingBlockFilter).get_Filter();
					}
					return (V_3 as ExceptionHandlingBlockFilter).get_Handler();
				}
				if (V_3.get_HandlerType() == FilteringExceptionHandlerType.Catch)
				{
					return V_3 as ExceptionHandlingBlockCatch;
				}
				V_2 = V_2 + 1;
			}
			return null;
		}

		public static ICollection<ISingleEntrySubGraph> GetTraversablePredecessors(ISingleEntrySubGraph construct)
		{
			stackVariable2 = new List<ISingleEntrySubGraph>(construct.get_SameParentPredecessors());
			stackVariable2.Sort();
			return stackVariable2;
		}

		public static ICollection<ISingleEntrySubGraph> GetTraversableSuccessors(ISingleEntrySubGraph construct)
		{
			stackVariable2 = new List<ISingleEntrySubGraph>(construct.get_SameParentSuccessors());
			stackVariable2.Sort();
			return stackVariable2;
		}

		public static KeyValuePair<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct> SplitCFGBlockAt(LogicalFlowBuilderContext logicalContext, CFGBlockLogicalConstruct cfgBlock, int expressionIndex)
		{
			V_0 = cfgBlock.get_LogicalConstructExpressions();
			if (V_0 == null)
			{
				throw new ArgumentNullException("blockExpressions");
			}
			if (expressionIndex <= 0 || expressionIndex >= V_0.get_Count())
			{
				throw new ArgumentOutOfRangeException("expressionIndex");
			}
			V_1 = logicalContext.get_CFGBlockToLogicalConstructMap().get_Item(cfgBlock.get_TheBlock());
			V_2 = (int)V_1.Length;
			V_3 = 0;
			while (V_3 < V_2 && cfgBlock != V_1[V_3])
			{
				V_3 = V_3 + 1;
			}
			if (V_3 == V_2)
			{
				throw new ArgumentException("cfgBlock");
			}
			V_4 = cfgBlock.get_LogicalConstructExpressions().GetRange(0, expressionIndex);
			V_5 = new PartialCFGBlockLogicalConstruct(cfgBlock, V_4);
			V_5.RedirectPredecessors();
			V_6 = cfgBlock.get_LogicalConstructExpressions().GetRange(expressionIndex, V_0.get_Count() - expressionIndex);
			V_7 = new PartialCFGBlockLogicalConstruct(cfgBlock, V_6);
			V_7.RedirectSuccessors();
			V_5.AddToSuccessors(V_7);
			V_7.AddToPredecessors(V_5);
			V_8 = new CFGBlockLogicalConstruct[V_2 + 1];
			V_9 = 0;
			V_10 = 0;
			while (V_9 < V_2)
			{
				if (V_9 == V_3)
				{
					V_8[V_10] = V_5;
					stackVariable68 = V_10 + 1;
					V_10 = stackVariable68;
					V_8[stackVariable68] = V_7;
				}
				else
				{
					V_8[V_10] = V_1[V_9];
				}
				V_9 = V_9 + 1;
				V_10 = V_10 + 1;
			}
			logicalContext.get_CFGBlockToLogicalConstructMap().set_Item(cfgBlock.get_TheBlock(), V_8);
			return new KeyValuePair<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct>(V_5, V_7);
		}

		public static bool TryGetParentConstructWithGivenParent(ILogicalConstruct initialNode, ILogicalConstruct parent, out ILogicalConstruct desiredNode)
		{
			if (initialNode == null || parent == null)
			{
				desiredNode = null;
				return false;
			}
			desiredNode = initialNode;
			while (desiredNode.get_Parent() != null)
			{
				if (desiredNode.get_Parent() == parent)
				{
					return true;
				}
				desiredNode = desiredNode.get_Parent() as ILogicalConstruct;
			}
			desiredNode = null;
			return false;
		}
	}
}