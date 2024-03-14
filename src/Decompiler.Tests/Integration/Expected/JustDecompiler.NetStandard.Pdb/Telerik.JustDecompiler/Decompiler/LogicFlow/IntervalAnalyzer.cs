using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Common;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DFST;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	internal class IntervalAnalyzer
	{
		private readonly HashSet<ISingleEntrySubGraph> availableNodes;

		private readonly ILogicalConstruct entryPoint;

		private readonly Queue<ILogicalConstruct> headers;

		private readonly List<IntervalConstruct> intervals;

		private readonly Dictionary<ISingleEntrySubGraph, IntervalConstruct> nodeToInterval;

		private readonly Dictionary<ILogicalConstruct, HashSet<ILogicalConstruct>> removedEdges;

		public IntervalAnalyzer(ISingleEntrySubGraph graph, Dictionary<ILogicalConstruct, HashSet<ILogicalConstruct>> removedEdges)
		{
			this.availableNodes = graph.Children;
			this.entryPoint = graph.Entry as ILogicalConstruct;
			this.headers = new Queue<ILogicalConstruct>();
			this.intervals = new List<IntervalConstruct>();
			this.nodeToInterval = new Dictionary<ISingleEntrySubGraph, IntervalConstruct>();
			this.removedEdges = removedEdges;
		}

		private void AddNewHeaders(ILogicalConstruct currentHeader, IntervalConstruct currentInterval)
		{
			Stack<ILogicalConstruct> logicalConstructs = new Stack<ILogicalConstruct>();
			HashSet<ILogicalConstruct> logicalConstructs1 = new HashSet<ILogicalConstruct>();
			logicalConstructs.Push(currentHeader);
			while (logicalConstructs.Count > 0)
			{
				ILogicalConstruct logicalConstruct = logicalConstructs.Pop();
				if (logicalConstructs1.Contains(logicalConstruct))
				{
					continue;
				}
				logicalConstructs1.Add(logicalConstruct);
				foreach (ILogicalConstruct nodeSuccessor in this.GetNodeSuccessors(logicalConstruct))
				{
					this.CheckAndAddPossibleHeader(nodeSuccessor, currentInterval, logicalConstructs);
				}
			}
		}

		private void CheckAndAddPossibleHeader(ILogicalConstruct node, IntervalConstruct currentInterval, Stack<ILogicalConstruct> st)
		{
			if (!this.nodeToInterval.ContainsKey(node))
			{
				if (!this.headers.Contains(node))
				{
					this.headers.Enqueue(node);
				}
				return;
			}
			if (this.nodeToInterval[node] == currentInterval)
			{
				st.Push(node);
			}
		}

		private void CreateGraph(IEnumerable<IntervalConstruct> intervals)
		{
			foreach (IntervalConstruct interval in intervals)
			{
				foreach (ILogicalConstruct child in interval.Children)
				{
					foreach (ILogicalConstruct nodeSuccessor in this.GetNodeSuccessors(child))
					{
						if (!this.nodeToInterval.ContainsKey(nodeSuccessor) || this.nodeToInterval[nodeSuccessor] == interval)
						{
							continue;
						}
						IntervalConstruct item = this.nodeToInterval[nodeSuccessor];
						if (interval.SameParentSuccessors.Contains(item))
						{
							continue;
						}
						interval.SameParentSuccessors.Add(item);
						item.SameParentPredecessors.Add(interval);
					}
				}
			}
		}

		private void FillInterval(ILogicalConstruct intervalHeader, IntervalConstruct interval)
		{
			this.nodeToInterval.Add(intervalHeader, interval);
			Queue<ILogicalConstruct> logicalConstructs = new Queue<ILogicalConstruct>();
			foreach (ILogicalConstruct nodeSuccessor in this.GetNodeSuccessors(intervalHeader))
			{
				if (!this.availableNodes.Contains(nodeSuccessor))
				{
					continue;
				}
				logicalConstructs.Enqueue(nodeSuccessor);
			}
			while (logicalConstructs.Count > 0)
			{
				ILogicalConstruct logicalConstruct = logicalConstructs.Dequeue();
				if (this.nodeToInterval.ContainsKey(logicalConstruct))
				{
					continue;
				}
				bool flag = true;
				foreach (ILogicalConstruct nodePredecessor in this.GetNodePredecessors(logicalConstruct))
				{
					if (this.nodeToInterval.ContainsKey(nodePredecessor) && this.nodeToInterval[nodePredecessor] == interval)
					{
						continue;
					}
					flag = false;
					goto Label0;
				}
			Label0:
				if (!flag)
				{
					continue;
				}
				interval.Children.Add(logicalConstruct);
				this.nodeToInterval.Add(logicalConstruct, interval);
				foreach (ILogicalConstruct nodeSuccessor1 in this.GetNodeSuccessors(logicalConstruct))
				{
					if (!this.availableNodes.Contains(nodeSuccessor1))
					{
						continue;
					}
					logicalConstructs.Enqueue(nodeSuccessor1);
				}
			}
		}

		private IEnumerable<ILogicalConstruct> GetNodePredecessors(ILogicalConstruct node)
		{
			ICollection<ILogicalConstruct> logicalConstructs = new HashSet<ILogicalConstruct>();
			foreach (ILogicalConstruct sameParentPredecessor in node.SameParentPredecessors)
			{
				if (this.removedEdges.ContainsKey(sameParentPredecessor) && this.removedEdges[sameParentPredecessor].Contains(node))
				{
					continue;
				}
				logicalConstructs.Add(sameParentPredecessor);
			}
			return logicalConstructs;
		}

		private IEnumerable<ILogicalConstruct> GetNodeSuccessors(ILogicalConstruct node)
		{
			ICollection<ILogicalConstruct> logicalConstructs = new List<ILogicalConstruct>();
			foreach (ILogicalConstruct sameParentSuccessor in node.SameParentSuccessors)
			{
				if (this.removedEdges.ContainsKey(node) && this.removedEdges[node].Contains(sameParentSuccessor))
				{
					continue;
				}
				logicalConstructs.Add(sameParentSuccessor);
			}
			return logicalConstructs;
		}

		public List<IntervalConstruct> ReduceCfg()
		{
			this.headers.Enqueue(this.entryPoint);
			while (this.headers.Count > 0)
			{
				ILogicalConstruct logicalConstruct = this.headers.Dequeue();
				IntervalConstruct intervalConstruct = new IntervalConstruct(logicalConstruct);
				this.intervals.Add(intervalConstruct);
				this.FillInterval(logicalConstruct, intervalConstruct);
				this.AddNewHeaders(logicalConstruct, intervalConstruct);
			}
			this.CreateGraph(this.intervals);
			return this.SortIntervalList(this.intervals);
		}

		private List<IntervalConstruct> SortIntervalList(List<IntervalConstruct> intervals)
		{
			IntervalConstruct intervalConstruct = new IntervalConstruct(intervals[0]);
			foreach (ISingleEntrySubGraph interval in intervals)
			{
				intervalConstruct.Children.Add(interval);
			}
			DFSTree dFSTree = DFSTBuilder.BuildTree(intervalConstruct);
			List<IntervalConstruct> intervalConstructs = new List<IntervalConstruct>();
			foreach (DFSTNode reversePostOrder in dFSTree.ReversePostOrder)
			{
				intervalConstructs.Add(reversePostOrder.Construct as IntervalConstruct);
			}
			return intervalConstructs;
		}
	}
}