using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler.LogicFlow;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions
{
	public static class GuardedBlocksFollowNodesFinder
	{
		private static void AddSuccessorsToCount(ILogicalConstruct construct, Dictionary<CFGBlockLogicalConstruct, uint> numberOfHandlersLeavingToBlock)
		{
			V_0 = construct.get_CFGSuccessors().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (numberOfHandlersLeavingToBlock.ContainsKey(V_1))
					{
						stackVariable10 = numberOfHandlersLeavingToBlock;
						V_2 = V_1;
						stackVariable10.set_Item(V_2, stackVariable10.get_Item(V_2) + 1);
					}
					else
					{
						numberOfHandlersLeavingToBlock.Add(V_1, 1);
					}
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		internal static CFGBlockLogicalConstruct FindGuardedBlockCFGFollowNode(ExceptionHandlingLogicalConstruct construct, HashSet<CFGBlockLogicalConstruct> outOfconsideration)
		{
			V_0 = new Dictionary<CFGBlockLogicalConstruct, uint>();
			GuardedBlocksFollowNodesFinder.AddSuccessorsToCount(construct.get_Try(), V_0);
			V_1 = 1;
			if (construct as TryCatchFilterLogicalConstruct == null)
			{
				if (construct as TryFaultLogicalConstruct == null)
				{
					if (construct as TryFinallyLogicalConstruct != null)
					{
						GuardedBlocksFollowNodesFinder.AddSuccessorsToCount((construct as TryFinallyLogicalConstruct).get_Finally(), V_0);
						V_1 = V_1 + 1;
					}
				}
				else
				{
					GuardedBlocksFollowNodesFinder.AddSuccessorsToCount((construct as TryFaultLogicalConstruct).get_Fault(), V_0);
					V_1 = V_1 + 1;
				}
			}
			else
			{
				V_5 = (construct as TryCatchFilterLogicalConstruct).get_Handlers();
				V_6 = 0;
				while (V_6 < (int)V_5.Length)
				{
					GuardedBlocksFollowNodesFinder.AddSuccessorsToCount(V_5[V_6], V_0);
					V_1 = V_1 + 1;
					V_6 = V_6 + 1;
				}
			}
			V_7 = outOfconsideration.GetEnumerator();
			try
			{
				while (V_7.MoveNext())
				{
					V_8 = V_7.get_Current();
					if (!V_0.ContainsKey(V_8))
					{
						continue;
					}
					dummyVar0 = V_0.Remove(V_8);
				}
			}
			finally
			{
				((IDisposable)V_7).Dispose();
			}
			if (V_0.get_Count() == 0)
			{
				return null;
			}
			V_2 = new HashSet<CFGBlockLogicalConstruct>();
			V_3 = V_0.get_Keys().FirstOrDefault<CFGBlockLogicalConstruct>();
			dummyVar1 = V_2.Add(V_3);
			V_4 = V_0.get_Item(V_3);
			V_9 = V_0.get_Keys().GetEnumerator();
			try
			{
				while (V_9.MoveNext())
				{
					V_10 = V_9.get_Current();
					if (V_0.get_Item(V_10) <= V_4)
					{
						if (V_0.get_Item(V_10) != V_4)
						{
							continue;
						}
						dummyVar3 = V_2.Add(V_10);
					}
					else
					{
						V_4 = V_0.get_Item(V_10);
						V_2.Clear();
						dummyVar2 = V_2.Add(V_10);
					}
				}
			}
			finally
			{
				((IDisposable)V_9).Dispose();
			}
			if (V_2.get_Count() == 1)
			{
				return V_2.FirstOrDefault<CFGBlockLogicalConstruct>();
			}
			V_11 = new HashSet<CFGBlockLogicalConstruct>();
			V_12 = 0;
			V_7 = V_2.GetEnumerator();
			try
			{
				while (V_7.MoveNext())
				{
					V_14 = V_7.get_Current();
					V_15 = 0;
					V_16 = construct.get_CFGBlocks();
					V_17 = V_14.get_CFGPredecessors().GetEnumerator();
					try
					{
						while (V_17.MoveNext())
						{
							V_18 = V_17.get_Current();
							if (!V_16.Contains(V_18))
							{
								continue;
							}
							V_15 = V_15 + 1;
						}
					}
					finally
					{
						((IDisposable)V_17).Dispose();
					}
					if (V_15 < V_12)
					{
						continue;
					}
					if (V_15 > V_12)
					{
						V_12 = V_15;
						V_11.Clear();
					}
					dummyVar4 = V_11.Add(V_14);
				}
			}
			finally
			{
				((IDisposable)V_7).Dispose();
			}
			V_13 = V_11.FirstOrDefault<CFGBlockLogicalConstruct>();
			if (V_13.get_Index() < construct.get_Entry().get_Index())
			{
				V_19 = null;
				V_7 = V_11.GetEnumerator();
				try
				{
					while (V_7.MoveNext())
					{
						V_20 = V_7.get_Current();
						if (V_20.get_Index() <= construct.get_Entry().get_Index() || V_19 != null && V_19.get_Index() < V_20.get_Index())
						{
							continue;
						}
						V_19 = V_20;
					}
				}
				finally
				{
					((IDisposable)V_7).Dispose();
				}
				if (V_19 != null)
				{
					V_13 = V_19;
				}
			}
			return V_13;
		}
	}
}