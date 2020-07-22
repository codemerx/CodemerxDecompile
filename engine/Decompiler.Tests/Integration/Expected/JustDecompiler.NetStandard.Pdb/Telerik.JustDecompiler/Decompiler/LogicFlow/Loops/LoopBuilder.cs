using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DFST;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DTree;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Loops
{
	internal class LoopBuilder : DominatorTreeDependentStep
	{
		private readonly Dictionary<ILogicalConstruct, HashSet<ILogicalConstruct>> removedEdges;

		private readonly TypeSystem typeSystem;

		public LoopBuilder(LogicalFlowBuilderContext logicalContext, TypeSystem typeSystem)
		{
			this.removedEdges = new Dictionary<ILogicalConstruct, HashSet<ILogicalConstruct>>();
			base(logicalContext);
			this.typeSystem = typeSystem;
			return;
		}

		private HashSet<ILogicalConstruct> BuildLoop(DFSTree tree, out HashSet<ILogicalConstruct> loopBody)
		{
			loopBody = new HashSet<ILogicalConstruct>();
			V_0 = new HashSet<ILogicalConstruct>();
			V_1 = tree.get_BackEdges().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_3 = V_2.get_End().get_Construct() as ILogicalConstruct;
					V_4 = V_2.get_Start().get_Construct() as ILogicalConstruct;
					if (this.removedEdges.ContainsKey(V_4) && this.removedEdges.get_Item(V_4).Contains(V_3))
					{
						continue;
					}
					V_5 = tree.GetPath(V_2.get_End(), V_2.get_Start());
					V_6 = this.ExpandLoopBodyWithCrossEdges(V_5);
					V_7 = this.GetConstructsCollection(V_6);
					if (!this.CanBeLoop(V_3, V_4, V_6))
					{
						continue;
					}
					dummyVar0 = V_0.Add(V_4);
					V_8 = V_7.GetEnumerator();
					try
					{
						while (V_8.MoveNext())
						{
							V_9 = V_8.get_Current();
							dummyVar1 = loopBody.Add(V_9);
						}
					}
					finally
					{
						if (V_8 != null)
						{
							V_8.Dispose();
						}
					}
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0;
		}

		public void BuildLoops(ILogicalConstruct block)
		{
			if (block.get_Children().get_Count() == 0)
			{
				return;
			}
			V_0 = block.get_Children().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current() as ILogicalConstruct;
					if (V_1 == null)
					{
						throw new ArgumentException("Child is not a logical construct.");
					}
					if (V_1 as ConditionLogicalConstruct != null)
					{
						continue;
					}
					this.BuildLoops(V_1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			this.ProcessLogicalConstruct(block);
			return;
		}

		private bool CanBeInLoop(ILogicalConstruct node, ICollection<ILogicalConstruct> nodesInLoop, ILogicalConstruct loopHeader)
		{
			if (node == null)
			{
				return false;
			}
			if (node == loopHeader)
			{
				return true;
			}
			V_0 = node.get_SameParentPredecessors().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current() as ILogicalConstruct;
					if (V_1 != null)
					{
						if (nodesInLoop.Contains(V_1) && !this.removedEdges.TryGetValue(V_1, out V_2) || !V_2.Contains(node))
						{
							continue;
						}
						V_3 = false;
						goto Label1;
					}
					else
					{
						V_3 = false;
						goto Label1;
					}
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
		Label1:
			return V_3;
		Label0:
			return true;
		}

		private bool CanBeLoop(ILogicalConstruct header, ILogicalConstruct latchingNode, ICollection<DFSTNode> nodesInLoop)
		{
			if (header == null || latchingNode == null)
			{
				return false;
			}
			V_0 = this.GetConstructsCollection(nodesInLoop);
			V_1 = V_0.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (this.CanBeInLoop(V_2, V_0, header))
					{
						continue;
					}
					V_3 = false;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
		Label1:
			return V_3;
		Label0:
			return true;
		}

		private bool CanBeLoopCondition(ILogicalConstruct node, HashSet<ILogicalConstruct> loopBody)
		{
			if (!loopBody.Contains(node))
			{
				return false;
			}
			if (node as ConditionLogicalConstruct == null)
			{
				return false;
			}
			V_0 = 0;
			V_1 = node.get_SameParentSuccessors().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = (ILogicalConstruct)V_1.get_Current();
					if (!loopBody.Contains(V_2))
					{
						continue;
					}
					V_0 = V_0 + 1;
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0 == 1;
		}

		private void CleanUpEdges(LoopLogicalConstruct loopConstruct)
		{
			V_0 = DFSTBuilder.BuildTree(loopConstruct.get_Parent()).get_ConstructToNodeMap().get_Item(loopConstruct);
			if (V_0.get_BackEdgeSuccessors().get_Count() == 0)
			{
				return;
			}
			V_1 = V_0.get_BackEdgeSuccessors().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2.get_Construct() as ILogicalConstruct as ConditionLogicalConstruct != null)
					{
						continue;
					}
					this.MarkAsGotoEdge(loopConstruct, V_2.get_Construct() as ILogicalConstruct);
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return;
		}

		private LoopType DetermineLoopType(HashSet<ILogicalConstruct> loopBody, HashSet<ILogicalConstruct> latchingNodes, IntervalConstruct interval, DominatorTree dominatorTree, out ConditionLogicalConstruct loopCondition)
		{
			V_0 = interval.get_Entry() as ILogicalConstruct;
			V_1 = new HashSet<ILogicalConstruct>(latchingNodes);
			dummyVar0 = V_1.Add(V_0);
			V_2 = DFSTBuilder.BuildTree(V_0.get_Parent() as ILogicalConstruct);
			V_3 = new HashSet<ILogicalConstruct>();
			V_4 = loopBody.GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_5 = V_4.get_Current();
					V_6 = dominatorTree.GetDominanceFrontier(V_5).GetEnumerator();
					try
					{
						while (V_6.MoveNext())
						{
							V_7 = (ILogicalConstruct)V_6.get_Current();
							if (!interval.get_Children().Contains(V_7) || loopBody.Contains(V_7))
							{
								continue;
							}
							dummyVar1 = V_3.Add(V_7);
						}
					}
					finally
					{
						((IDisposable)V_6).Dispose();
					}
				}
			}
			finally
			{
				((IDisposable)V_4).Dispose();
			}
			if (V_3.get_Count() == 0)
			{
				V_8 = V_2.get_ReversePostOrder().GetEnumerator();
				try
				{
					while (V_8.MoveNext())
					{
						V_9 = V_8.get_Current().get_Construct() as ILogicalConstruct;
						if (loopBody.Contains(V_9))
						{
							continue;
						}
						loopCondition = this.GetLoopConditionWithMaxIndex(V_2, loopBody, V_1, V_9);
						if (loopCondition == null)
						{
							continue;
						}
						this.ExpandLoopBody(interval, loopBody, V_9);
						if (loopCondition != V_0)
						{
							V_10 = 2;
							goto Label1;
						}
						else
						{
							V_10 = 1;
							goto Label1;
						}
					}
					goto Label0;
				}
				finally
				{
					((IDisposable)V_8).Dispose();
				}
			Label1:
				return V_10;
			}
			V_11 = V_2.get_ReversePostOrder().get_Count();
			V_4 = V_3.GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_13 = V_4.get_Current();
					V_14 = V_2.get_ConstructToNodeMap().get_Item(V_13).get_ReversePostOrderIndex();
					if (V_14 >= V_11)
					{
						continue;
					}
					V_11 = V_14;
				}
			}
			finally
			{
				((IDisposable)V_4).Dispose();
			}
			V_12 = V_2.get_ReversePostOrder().get_Item(V_11).get_Construct() as ILogicalConstruct;
			loopCondition = this.GetLoopConditionWithMaxIndex(V_2, loopBody, V_1, V_12);
			this.ExpandLoopBody(interval, loopBody, V_12);
			if (loopCondition == null)
			{
				return 0;
			}
			if (loopCondition == V_0)
			{
				return 1;
			}
			return 2;
		Label0:
			if (!this.CanBeLoopCondition(V_0, loopBody))
			{
				loopCondition = null;
				return 0;
			}
			loopCondition = V_0 as ConditionLogicalConstruct;
			return 1;
		}

		private void ExpandLoopBody(IntervalConstruct interval, HashSet<ILogicalConstruct> loopBody, ILogicalConstruct loopSuccessor)
		{
			V_0 = this.GetIntervalSuccessors(interval, loopSuccessor);
			dummyVar0 = V_0.Add(loopSuccessor);
			V_1 = interval.get_Children().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = (LogicalConstructBase)V_1.get_Current();
					if (V_0.Contains(V_2))
					{
						continue;
					}
					dummyVar1 = loopBody.Add(V_2);
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return;
		}

		private ICollection<DFSTNode> ExpandLoopBodyWithCrossEdges(ICollection<DFSTNode> nodesInLoop)
		{
			V_0 = new HashSet<DFSTNode>(nodesInLoop);
			V_1 = new Queue<DFSTNode>(nodesInLoop);
			while (V_1.get_Count() > 0)
			{
				V_2 = V_1.Dequeue();
				V_4 = V_2.get_CrossEdgePredecessors().GetEnumerator();
				try
				{
					while (V_4.MoveNext())
					{
						V_5 = V_4.get_Current();
						if (V_0.Contains(V_5))
						{
							continue;
						}
						dummyVar0 = V_0.Add(V_5);
						V_1.Enqueue(V_5);
					}
				}
				finally
				{
					((IDisposable)V_4).Dispose();
				}
				V_3 = V_2.get_Predecessor() as DFSTNode;
				if (V_3 == null || V_0.Contains(V_3))
				{
					continue;
				}
				dummyVar1 = V_0.Add(V_3);
				V_1.Enqueue(V_3);
			}
			return V_0;
		}

		private ICollection<ILogicalConstruct> GetConstructsCollection(ICollection<DFSTNode> nodeCollection)
		{
			V_0 = new HashSet<ILogicalConstruct>();
			V_1 = nodeCollection.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					dummyVar0 = V_0.Add(V_2.get_Construct() as ILogicalConstruct);
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}

		private HashSet<ILogicalConstruct> GetIntervalSuccessors(IntervalConstruct interval, ILogicalConstruct startNode)
		{
			V_0 = new HashSet<ILogicalConstruct>();
			if (!interval.get_Children().Contains(startNode))
			{
				return V_0;
			}
			V_1 = new Queue<ILogicalConstruct>();
			V_1.Enqueue(startNode);
			V_2 = new HashSet<ILogicalConstruct>();
			dummyVar0 = V_2.Add(startNode);
			while (V_1.get_Count() > 0)
			{
				V_3 = V_1.Dequeue().get_SameParentSuccessors().GetEnumerator();
				try
				{
					while (V_3.MoveNext())
					{
						V_4 = (ILogicalConstruct)V_3.get_Current();
						if (V_2.Contains(V_4) || !interval.get_Children().Contains(V_4) || !V_0.Add(V_4))
						{
							continue;
						}
						dummyVar1 = V_2.Add(V_4);
						V_1.Enqueue(V_4);
					}
				}
				finally
				{
					((IDisposable)V_3).Dispose();
				}
			}
			return V_0;
		}

		private ConditionLogicalConstruct GetLoopConditionWithMaxIndex(DFSTree dfsTree, HashSet<ILogicalConstruct> loopBody, HashSet<ILogicalConstruct> loopExits, ILogicalConstruct loopSuccessor)
		{
			V_0 = -1;
			V_1 = loopExits.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_3 = dfsTree.get_ConstructToNodeMap().get_Item(V_2).get_ReversePostOrderIndex();
					if (!V_2.get_SameParentSuccessors().Contains(loopSuccessor) || V_3 <= V_0 || !this.CanBeLoopCondition(V_2, loopBody))
					{
						continue;
					}
					V_0 = V_3;
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			if (V_0 <= -1)
			{
				return null;
			}
			return dfsTree.get_ReversePostOrder().get_Item(V_0).get_Construct() as ConditionLogicalConstruct;
		}

		private void MarkAsGotoEdge(ILogicalConstruct start, ILogicalConstruct end)
		{
			if (start == null || end == null)
			{
				throw new ArgumentOutOfRangeException("GoTo edge's ends must implement ILogicalConstruct.");
			}
			if (!this.removedEdges.TryGetValue(start, out V_0))
			{
				V_0 = new HashSet<ILogicalConstruct>();
				this.removedEdges.set_Item(start, V_0);
			}
			dummyVar0 = V_0.Add(end);
			return;
		}

		private void ProcessLogicalConstruct(ILogicalConstruct construct)
		{
			V_0 = this.GetDominatorTreeFromContext(construct);
			V_1 = 0x7fffffff;
			this.RemoveBackEdgesFromSwitchConstructs(construct);
			while (V_1 > 1)
			{
				V_2 = (new IntervalAnalyzer(construct, this.removedEdges)).ReduceCfg();
				stackVariable14 = new List<IntervalConstruct>(V_2);
				stackVariable14.Reverse();
				V_3 = false;
				V_4 = stackVariable14.GetEnumerator();
				try
				{
					while (V_4.MoveNext())
					{
						V_5 = V_4.get_Current();
						if (!this.TryMakeLoop(V_5, V_0))
						{
							continue;
						}
						V_3 = true;
						goto Label0;
					}
				}
				finally
				{
					((IDisposable)V_4).Dispose();
				}
			Label0:
				if (!V_3)
				{
					if (V_2.get_Count() == V_1)
					{
						this.RemoveBlockingEdges(V_2);
					}
					if (V_2.get_Count() > V_1)
					{
						throw new Exception("Intervails are more than in the last iteration.");
					}
					V_1 = V_2.get_Count();
				}
				else
				{
					V_1 = V_2.get_Count();
				}
			}
			return;
		}

		private void RemoveBackEdgesFromSwitchConstructs(ILogicalConstruct theConstruct)
		{
			V_0 = DFSTBuilder.BuildTree(theConstruct).get_BackEdges().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = V_1.get_Start().get_Construct() as ILogicalConstruct;
					if (V_2 as ConditionLogicalConstruct != null)
					{
						continue;
					}
					V_3 = V_2 as CFGBlockLogicalConstruct;
					if (V_3 == null || V_3.get_TheBlock().get_Last().get_OpCode().get_Code() != 68)
					{
						continue;
					}
					this.MarkAsGotoEdge(V_2, V_1.get_End().get_Construct() as ILogicalConstruct);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private void RemoveBlockingEdges(List<IntervalConstruct> intervals)
		{
			V_0 = new IntervalConstruct(intervals.get_Item(0));
			V_5 = 1;
			while (V_5 < intervals.get_Count())
			{
				dummyVar0 = V_0.get_Children().Add(intervals.get_Item(V_5));
				V_5 = V_5 + 1;
			}
			V_1 = DFSTBuilder.BuildTree(V_0);
			V_2 = V_1.get_BackEdges().FirstOrDefault<DFSTEdge>();
			if (V_2 == null)
			{
				V_2 = V_1.get_CrossEdges().FirstOrDefault<DFSTEdge>();
			}
			V_3 = V_2.get_Start().get_Construct() as IntervalConstruct;
			V_4 = V_2.get_End().get_Construct() as IntervalConstruct;
			V_6 = V_4.get_Entry().get_SameParentPredecessors().GetEnumerator();
			try
			{
				while (V_6.MoveNext())
				{
					V_7 = (ILogicalConstruct)V_6.get_Current();
					if (!V_3.get_Children().Contains(V_7))
					{
						continue;
					}
					V_8 = V_7;
					V_9 = V_4.get_Entry() as ILogicalConstruct;
					if (this.removedEdges.TryGetValue(V_8, out V_10) && V_10.Contains(V_9))
					{
						continue;
					}
					this.MarkAsGotoEdge(V_8, V_9);
					goto Label0;
				}
			}
			finally
			{
				((IDisposable)V_6).Dispose();
			}
		Label0:
			return;
		}

		private bool TryMakeLoop(IntervalConstruct interval, DominatorTree dominatorTree)
		{
			V_0 = DFSTBuilder.BuildTree(interval);
			if (V_0.get_BackEdges().get_Count() == 0)
			{
				return false;
			}
			V_2 = this.BuildLoop(V_0, out V_1);
			V_4 = this.DetermineLoopType(V_1, V_2, interval, dominatorTree, out V_3);
			if (V_1.get_Count() > 0)
			{
				V_5 = new LoopLogicalConstruct(interval.get_Entry() as ILogicalConstruct, V_1, V_4, V_3, this.typeSystem);
				this.CleanUpEdges(V_5);
				this.UpdateDominatorTree(dominatorTree, V_5);
				return true;
			}
			V_6 = V_0.get_BackEdges().GetEnumerator();
			try
			{
				while (V_6.MoveNext())
				{
					V_7 = V_6.get_Current();
					this.MarkAsGotoEdge(V_7.get_Start().get_Construct() as ILogicalConstruct, V_7.get_End().get_Construct() as ILogicalConstruct);
				}
			}
			finally
			{
				((IDisposable)V_6).Dispose();
			}
			return false;
		}

		private void UpdateDominatorTree(DominatorTree dominatorTree, LoopLogicalConstruct theLoopConstruct)
		{
			V_0 = new HashSet<ISingleEntrySubGraph>();
			if (theLoopConstruct.get_LoopCondition() != null)
			{
				dummyVar0 = V_0.Add(theLoopConstruct.get_LoopCondition());
			}
			if (theLoopConstruct.get_LoopBodyBlock() != null)
			{
				V_0.UnionWith(theLoopConstruct.get_LoopBodyBlock().get_Children());
			}
			if (theLoopConstruct.get_LoopType() == 1)
			{
				stackVariable10 = theLoopConstruct.get_LoopCondition();
			}
			else
			{
				stackVariable10 = theLoopConstruct.get_LoopBodyBlock().get_Entry();
			}
			dominatorTree.MergeNodes(V_0, stackVariable10, theLoopConstruct);
			return;
		}
	}
}