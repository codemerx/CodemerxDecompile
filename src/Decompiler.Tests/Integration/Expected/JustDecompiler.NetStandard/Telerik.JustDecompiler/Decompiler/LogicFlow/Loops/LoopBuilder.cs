using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Common;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DFST;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DTree;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Loops
{
	internal class LoopBuilder : DominatorTreeDependentStep
	{
		private readonly Dictionary<ILogicalConstruct, HashSet<ILogicalConstruct>> removedEdges = new Dictionary<ILogicalConstruct, HashSet<ILogicalConstruct>>();

		private readonly TypeSystem typeSystem;

		public LoopBuilder(LogicalFlowBuilderContext logicalContext, TypeSystem typeSystem) : base(logicalContext)
		{
			this.typeSystem = typeSystem;
		}

		private HashSet<ILogicalConstruct> BuildLoop(DFSTree tree, out HashSet<ILogicalConstruct> loopBody)
		{
			loopBody = new HashSet<ILogicalConstruct>();
			HashSet<ILogicalConstruct> logicalConstructs = new HashSet<ILogicalConstruct>();
			foreach (DFSTEdge backEdge in tree.BackEdges)
			{
				ILogicalConstruct construct = backEdge.End.Construct as ILogicalConstruct;
				ILogicalConstruct logicalConstruct = backEdge.Start.Construct as ILogicalConstruct;
				if (this.removedEdges.ContainsKey(logicalConstruct) && this.removedEdges[logicalConstruct].Contains(construct))
				{
					continue;
				}
				ICollection<DFSTNode> path = tree.GetPath(backEdge.End, backEdge.Start);
				ICollection<DFSTNode> dFSTNodes = this.ExpandLoopBodyWithCrossEdges(path);
				ICollection<ILogicalConstruct> constructsCollection = this.GetConstructsCollection(dFSTNodes);
				if (!this.CanBeLoop(construct, logicalConstruct, dFSTNodes))
				{
					continue;
				}
				logicalConstructs.Add(logicalConstruct);
				foreach (ILogicalConstruct logicalConstruct1 in constructsCollection)
				{
					loopBody.Add(logicalConstruct1);
				}
			}
			return logicalConstructs;
		}

		public void BuildLoops(ILogicalConstruct block)
		{
			if (block.Children.Count == 0)
			{
				return;
			}
			foreach (ISingleEntrySubGraph child in block.Children)
			{
				ILogicalConstruct logicalConstruct = child as ILogicalConstruct;
				if (logicalConstruct == null)
				{
					throw new ArgumentException("Child is not a logical construct.");
				}
				if (logicalConstruct is ConditionLogicalConstruct)
				{
					continue;
				}
				this.BuildLoops(logicalConstruct);
			}
			this.ProcessLogicalConstruct(block);
		}

		private bool CanBeInLoop(ILogicalConstruct node, ICollection<ILogicalConstruct> nodesInLoop, ILogicalConstruct loopHeader)
		{
			HashSet<ILogicalConstruct> logicalConstructs;
			bool flag;
			if (node == null)
			{
				return false;
			}
			if (node == loopHeader)
			{
				return true;
			}
			HashSet<ISingleEntrySubGraph>.Enumerator enumerator = node.SameParentPredecessors.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ILogicalConstruct current = enumerator.Current as ILogicalConstruct;
					if (current != null)
					{
						if (nodesInLoop.Contains(current) && (!this.removedEdges.TryGetValue(current, out logicalConstructs) || !logicalConstructs.Contains(node)))
						{
							continue;
						}
						flag = false;
						return flag;
					}
					else
					{
						flag = false;
						return flag;
					}
				}
				return true;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private bool CanBeLoop(ILogicalConstruct header, ILogicalConstruct latchingNode, ICollection<DFSTNode> nodesInLoop)
		{
			bool flag;
			if (header == null || latchingNode == null)
			{
				return false;
			}
			ICollection<ILogicalConstruct> constructsCollection = this.GetConstructsCollection(nodesInLoop);
			using (IEnumerator<ILogicalConstruct> enumerator = constructsCollection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (this.CanBeInLoop(enumerator.Current, constructsCollection, header))
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
			}
			return flag;
		}

		private bool CanBeLoopCondition(ILogicalConstruct node, HashSet<ILogicalConstruct> loopBody)
		{
			if (!loopBody.Contains(node))
			{
				return false;
			}
			if (!(node is ConditionLogicalConstruct))
			{
				return false;
			}
			int num = 0;
			foreach (ILogicalConstruct sameParentSuccessor in node.SameParentSuccessors)
			{
				if (!loopBody.Contains(sameParentSuccessor))
				{
					continue;
				}
				num++;
			}
			return num == 1;
		}

		private void CleanUpEdges(LoopLogicalConstruct loopConstruct)
		{
			DFSTNode item = DFSTBuilder.BuildTree(loopConstruct.Parent).ConstructToNodeMap[loopConstruct];
			if (item.BackEdgeSuccessors.Count == 0)
			{
				return;
			}
			foreach (DFSTNode backEdgeSuccessor in item.BackEdgeSuccessors)
			{
				if (backEdgeSuccessor.Construct as ILogicalConstruct is ConditionLogicalConstruct)
				{
					continue;
				}
				this.MarkAsGotoEdge(loopConstruct, backEdgeSuccessor.Construct as ILogicalConstruct);
			}
		}

		private LoopType DetermineLoopType(HashSet<ILogicalConstruct> loopBody, HashSet<ILogicalConstruct> latchingNodes, IntervalConstruct interval, DominatorTree dominatorTree, out ConditionLogicalConstruct loopCondition)
		{
			LoopType loopType;
			ILogicalConstruct entry = interval.Entry as ILogicalConstruct;
			HashSet<ILogicalConstruct> logicalConstructs = new HashSet<ILogicalConstruct>(latchingNodes);
			logicalConstructs.Add(entry);
			DFSTree dFSTree = DFSTBuilder.BuildTree(entry.Parent as ILogicalConstruct);
			HashSet<ILogicalConstruct> logicalConstructs1 = new HashSet<ILogicalConstruct>();
			foreach (ILogicalConstruct logicalConstruct in loopBody)
			{
				foreach (ILogicalConstruct dominanceFrontier in dominatorTree.GetDominanceFrontier(logicalConstruct))
				{
					if (!interval.Children.Contains(dominanceFrontier) || loopBody.Contains(dominanceFrontier))
					{
						continue;
					}
					logicalConstructs1.Add(dominanceFrontier);
				}
			}
			if (logicalConstructs1.Count == 0)
			{
				List<DFSTNode>.Enumerator enumerator = dFSTree.ReversePostOrder.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ILogicalConstruct construct = enumerator.Current.Construct as ILogicalConstruct;
						if (loopBody.Contains(construct))
						{
							continue;
						}
						loopCondition = this.GetLoopConditionWithMaxIndex(dFSTree, loopBody, logicalConstructs, construct);
						if (loopCondition == null)
						{
							continue;
						}
						this.ExpandLoopBody(interval, loopBody, construct);
						if (loopCondition != entry)
						{
							loopType = LoopType.PostTestedLoop;
							return loopType;
						}
						else
						{
							loopType = LoopType.PreTestedLoop;
							return loopType;
						}
					}
					if (!this.CanBeLoopCondition(entry, loopBody))
					{
						loopCondition = null;
						return LoopType.InfiniteLoop;
					}
					loopCondition = entry as ConditionLogicalConstruct;
					return LoopType.PreTestedLoop;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return loopType;
			}
			int count = dFSTree.ReversePostOrder.Count;
			foreach (ILogicalConstruct logicalConstruct1 in logicalConstructs1)
			{
				int reversePostOrderIndex = dFSTree.ConstructToNodeMap[logicalConstruct1].ReversePostOrderIndex;
				if (reversePostOrderIndex >= count)
				{
					continue;
				}
				count = reversePostOrderIndex;
			}
			ILogicalConstruct construct1 = dFSTree.ReversePostOrder[count].Construct as ILogicalConstruct;
			loopCondition = this.GetLoopConditionWithMaxIndex(dFSTree, loopBody, logicalConstructs, construct1);
			this.ExpandLoopBody(interval, loopBody, construct1);
			if (loopCondition == null)
			{
				return LoopType.InfiniteLoop;
			}
			if (loopCondition == entry)
			{
				return LoopType.PreTestedLoop;
			}
			return LoopType.PostTestedLoop;
		}

		private void ExpandLoopBody(IntervalConstruct interval, HashSet<ILogicalConstruct> loopBody, ILogicalConstruct loopSuccessor)
		{
			HashSet<ILogicalConstruct> intervalSuccessors = this.GetIntervalSuccessors(interval, loopSuccessor);
			intervalSuccessors.Add(loopSuccessor);
			foreach (LogicalConstructBase child in interval.Children)
			{
				if (intervalSuccessors.Contains(child))
				{
					continue;
				}
				loopBody.Add(child);
			}
		}

		private ICollection<DFSTNode> ExpandLoopBodyWithCrossEdges(ICollection<DFSTNode> nodesInLoop)
		{
			HashSet<DFSTNode> dFSTNodes = new HashSet<DFSTNode>(nodesInLoop);
			Queue<DFSTNode> dFSTNodes1 = new Queue<DFSTNode>(nodesInLoop);
			while (dFSTNodes1.Count > 0)
			{
				DFSTNode dFSTNode = dFSTNodes1.Dequeue();
				foreach (DFSTNode crossEdgePredecessor in dFSTNode.CrossEdgePredecessors)
				{
					if (dFSTNodes.Contains(crossEdgePredecessor))
					{
						continue;
					}
					dFSTNodes.Add(crossEdgePredecessor);
					dFSTNodes1.Enqueue(crossEdgePredecessor);
				}
				DFSTNode predecessor = dFSTNode.Predecessor as DFSTNode;
				if (predecessor == null || dFSTNodes.Contains(predecessor))
				{
					continue;
				}
				dFSTNodes.Add(predecessor);
				dFSTNodes1.Enqueue(predecessor);
			}
			return dFSTNodes;
		}

		private ICollection<ILogicalConstruct> GetConstructsCollection(ICollection<DFSTNode> nodeCollection)
		{
			HashSet<ILogicalConstruct> logicalConstructs = new HashSet<ILogicalConstruct>();
			foreach (DFSTNode dFSTNode in nodeCollection)
			{
				logicalConstructs.Add(dFSTNode.Construct as ILogicalConstruct);
			}
			return logicalConstructs;
		}

		private HashSet<ILogicalConstruct> GetIntervalSuccessors(IntervalConstruct interval, ILogicalConstruct startNode)
		{
			HashSet<ILogicalConstruct> logicalConstructs = new HashSet<ILogicalConstruct>();
			if (!interval.Children.Contains(startNode))
			{
				return logicalConstructs;
			}
			Queue<ILogicalConstruct> logicalConstructs1 = new Queue<ILogicalConstruct>();
			logicalConstructs1.Enqueue(startNode);
			HashSet<ILogicalConstruct> logicalConstructs2 = new HashSet<ILogicalConstruct>();
			logicalConstructs2.Add(startNode);
			while (logicalConstructs1.Count > 0)
			{
				foreach (ILogicalConstruct sameParentSuccessor in logicalConstructs1.Dequeue().SameParentSuccessors)
				{
					if (logicalConstructs2.Contains(sameParentSuccessor) || !interval.Children.Contains(sameParentSuccessor) || !logicalConstructs.Add(sameParentSuccessor))
					{
						continue;
					}
					logicalConstructs2.Add(sameParentSuccessor);
					logicalConstructs1.Enqueue(sameParentSuccessor);
				}
			}
			return logicalConstructs;
		}

		private ConditionLogicalConstruct GetLoopConditionWithMaxIndex(DFSTree dfsTree, HashSet<ILogicalConstruct> loopBody, HashSet<ILogicalConstruct> loopExits, ILogicalConstruct loopSuccessor)
		{
			int num = -1;
			foreach (ILogicalConstruct loopExit in loopExits)
			{
				int reversePostOrderIndex = dfsTree.ConstructToNodeMap[loopExit].ReversePostOrderIndex;
				if (!loopExit.SameParentSuccessors.Contains(loopSuccessor) || reversePostOrderIndex <= num || !this.CanBeLoopCondition(loopExit, loopBody))
				{
					continue;
				}
				num = reversePostOrderIndex;
			}
			if (num <= -1)
			{
				return null;
			}
			return dfsTree.ReversePostOrder[num].Construct as ConditionLogicalConstruct;
		}

		private void MarkAsGotoEdge(ILogicalConstruct start, ILogicalConstruct end)
		{
			HashSet<ILogicalConstruct> logicalConstructs;
			if (start == null || end == null)
			{
				throw new ArgumentOutOfRangeException("GoTo edge's ends must implement ILogicalConstruct.");
			}
			if (!this.removedEdges.TryGetValue(start, out logicalConstructs))
			{
				logicalConstructs = new HashSet<ILogicalConstruct>();
				this.removedEdges[start] = logicalConstructs;
			}
			logicalConstructs.Add(end);
		}

		private void ProcessLogicalConstruct(ILogicalConstruct construct)
		{
			DominatorTree dominatorTreeFromContext = base.GetDominatorTreeFromContext(construct);
			int count = 0x7fffffff;
			this.RemoveBackEdgesFromSwitchConstructs(construct);
			while (count > 1)
			{
				List<IntervalConstruct> intervalConstructs = (new IntervalAnalyzer(construct, this.removedEdges)).ReduceCfg();
				List<IntervalConstruct> intervalConstructs1 = new List<IntervalConstruct>(intervalConstructs);
				intervalConstructs1.Reverse();
				bool flag = false;
				foreach (IntervalConstruct intervalConstruct in intervalConstructs1)
				{
					if (!this.TryMakeLoop(intervalConstruct, dominatorTreeFromContext))
					{
						continue;
					}
					flag = true;
					goto Label0;
				}
			Label0:
				if (!flag)
				{
					if (intervalConstructs.Count == count)
					{
						this.RemoveBlockingEdges(intervalConstructs);
					}
					if (intervalConstructs.Count > count)
					{
						throw new Exception("Intervails are more than in the last iteration.");
					}
					count = intervalConstructs.Count;
				}
				else
				{
					count = intervalConstructs.Count;
				}
			}
		}

		private void RemoveBackEdgesFromSwitchConstructs(ILogicalConstruct theConstruct)
		{
			foreach (DFSTEdge backEdge in DFSTBuilder.BuildTree(theConstruct).BackEdges)
			{
				ILogicalConstruct construct = backEdge.Start.Construct as ILogicalConstruct;
				if (construct is ConditionLogicalConstruct)
				{
					continue;
				}
				CFGBlockLogicalConstruct cFGBlockLogicalConstruct = construct as CFGBlockLogicalConstruct;
				if (cFGBlockLogicalConstruct == null || cFGBlockLogicalConstruct.TheBlock.Last.get_OpCode().get_Code() != 68)
				{
					continue;
				}
				this.MarkAsGotoEdge(construct, backEdge.End.Construct as ILogicalConstruct);
			}
		}

		private void RemoveBlockingEdges(List<IntervalConstruct> intervals)
		{
			HashSet<ILogicalConstruct> logicalConstructs;
			IntervalConstruct intervalConstruct = new IntervalConstruct(intervals[0]);
			for (int i = 1; i < intervals.Count; i++)
			{
				intervalConstruct.Children.Add(intervals[i]);
			}
			DFSTree dFSTree = DFSTBuilder.BuildTree(intervalConstruct);
			DFSTEdge dFSTEdge = dFSTree.BackEdges.FirstOrDefault<DFSTEdge>() ?? dFSTree.CrossEdges.FirstOrDefault<DFSTEdge>();
			IntervalConstruct construct = dFSTEdge.Start.Construct as IntervalConstruct;
			IntervalConstruct construct1 = dFSTEdge.End.Construct as IntervalConstruct;
			foreach (ILogicalConstruct sameParentPredecessor in construct1.Entry.SameParentPredecessors)
			{
				if (!construct.Children.Contains(sameParentPredecessor))
				{
					continue;
				}
				ILogicalConstruct logicalConstruct = sameParentPredecessor;
				ILogicalConstruct entry = construct1.Entry as ILogicalConstruct;
				if (this.removedEdges.TryGetValue(logicalConstruct, out logicalConstructs) && logicalConstructs.Contains(entry))
				{
					continue;
				}
				this.MarkAsGotoEdge(logicalConstruct, entry);
				return;
			}
		}

		private bool TryMakeLoop(IntervalConstruct interval, DominatorTree dominatorTree)
		{
			HashSet<ILogicalConstruct> logicalConstructs;
			ConditionLogicalConstruct conditionLogicalConstruct;
			DFSTree dFSTree = DFSTBuilder.BuildTree(interval);
			if (dFSTree.BackEdges.Count == 0)
			{
				return false;
			}
			HashSet<ILogicalConstruct> logicalConstructs1 = this.BuildLoop(dFSTree, out logicalConstructs);
			LoopType loopType = this.DetermineLoopType(logicalConstructs, logicalConstructs1, interval, dominatorTree, out conditionLogicalConstruct);
			if (logicalConstructs.Count > 0)
			{
				LoopLogicalConstruct loopLogicalConstruct = new LoopLogicalConstruct(interval.Entry as ILogicalConstruct, logicalConstructs, loopType, conditionLogicalConstruct, this.typeSystem);
				this.CleanUpEdges(loopLogicalConstruct);
				this.UpdateDominatorTree(dominatorTree, loopLogicalConstruct);
				return true;
			}
			foreach (DFSTEdge backEdge in dFSTree.BackEdges)
			{
				this.MarkAsGotoEdge(backEdge.Start.Construct as ILogicalConstruct, backEdge.End.Construct as ILogicalConstruct);
			}
			return false;
		}

		private void UpdateDominatorTree(DominatorTree dominatorTree, LoopLogicalConstruct theLoopConstruct)
		{
			ISingleEntrySubGraph loopCondition;
			HashSet<ISingleEntrySubGraph> singleEntrySubGraphs = new HashSet<ISingleEntrySubGraph>();
			if (theLoopConstruct.LoopCondition != null)
			{
				singleEntrySubGraphs.Add(theLoopConstruct.LoopCondition);
			}
			if (theLoopConstruct.LoopBodyBlock != null)
			{
				singleEntrySubGraphs.UnionWith(theLoopConstruct.LoopBodyBlock.Children);
			}
			if (theLoopConstruct.LoopType == LoopType.PreTestedLoop)
			{
				loopCondition = theLoopConstruct.LoopCondition;
			}
			else
			{
				loopCondition = theLoopConstruct.LoopBodyBlock.Entry;
			}
			dominatorTree.MergeNodes(singleEntrySubGraphs, loopCondition, theLoopConstruct);
		}
	}
}