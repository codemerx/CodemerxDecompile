using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
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
			base();
			this.availableNodes = graph.get_Children();
			this.entryPoint = graph.get_Entry() as ILogicalConstruct;
			this.headers = new Queue<ILogicalConstruct>();
			this.intervals = new List<IntervalConstruct>();
			this.nodeToInterval = new Dictionary<ISingleEntrySubGraph, IntervalConstruct>();
			this.removedEdges = removedEdges;
			return;
		}

		private void AddNewHeaders(ILogicalConstruct currentHeader, IntervalConstruct currentInterval)
		{
			V_0 = new Stack<ILogicalConstruct>();
			V_1 = new HashSet<ILogicalConstruct>();
			V_0.Push(currentHeader);
			while (V_0.get_Count() > 0)
			{
				V_2 = V_0.Pop();
				if (V_1.Contains(V_2))
				{
					continue;
				}
				dummyVar0 = V_1.Add(V_2);
				V_3 = this.GetNodeSuccessors(V_2).GetEnumerator();
				try
				{
					while (V_3.MoveNext())
					{
						V_4 = V_3.get_Current();
						this.CheckAndAddPossibleHeader(V_4, currentInterval, V_0);
					}
				}
				finally
				{
					if (V_3 != null)
					{
						V_3.Dispose();
					}
				}
			}
			return;
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
			if (this.nodeToInterval.get_Item(node) == currentInterval)
			{
				st.Push(node);
			}
			return;
		}

		private void CreateGraph(IEnumerable<IntervalConstruct> intervals)
		{
			V_0 = intervals.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = V_1.get_Children().GetEnumerator();
					try
					{
						while (V_2.MoveNext())
						{
							V_3 = (ILogicalConstruct)V_2.get_Current();
							V_4 = this.GetNodeSuccessors(V_3).GetEnumerator();
							try
							{
								while (V_4.MoveNext())
								{
									V_5 = V_4.get_Current();
									if (!this.nodeToInterval.ContainsKey(V_5) || this.nodeToInterval.get_Item(V_5) == V_1)
									{
										continue;
									}
									V_6 = this.nodeToInterval.get_Item(V_5);
									if (V_1.get_SameParentSuccessors().Contains(V_6))
									{
										continue;
									}
									dummyVar0 = V_1.get_SameParentSuccessors().Add(V_6);
									dummyVar1 = V_6.get_SameParentPredecessors().Add(V_1);
								}
							}
							finally
							{
								if (V_4 != null)
								{
									V_4.Dispose();
								}
							}
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
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			return;
		}

		private void FillInterval(ILogicalConstruct intervalHeader, IntervalConstruct interval)
		{
			this.nodeToInterval.Add(intervalHeader, interval);
			V_0 = new Queue<ILogicalConstruct>();
			V_1 = this.GetNodeSuccessors(intervalHeader).GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (!this.availableNodes.Contains(V_2))
					{
						continue;
					}
					V_0.Enqueue(V_2);
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			while (V_0.get_Count() > 0)
			{
				V_3 = V_0.Dequeue();
				if (this.nodeToInterval.ContainsKey(V_3))
				{
					continue;
				}
				V_4 = true;
				V_1 = this.GetNodePredecessors(V_3).GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_5 = V_1.get_Current();
						if (this.nodeToInterval.ContainsKey(V_5) && this.nodeToInterval.get_Item(V_5) == interval)
						{
							continue;
						}
						V_4 = false;
						goto Label0;
					}
				}
				finally
				{
					if (V_1 != null)
					{
						V_1.Dispose();
					}
				}
			Label0:
				if (!V_4)
				{
					continue;
				}
				dummyVar0 = interval.get_Children().Add(V_3);
				this.nodeToInterval.Add(V_3, interval);
				V_1 = this.GetNodeSuccessors(V_3).GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_6 = V_1.get_Current();
						if (!this.availableNodes.Contains(V_6))
						{
							continue;
						}
						V_0.Enqueue(V_6);
					}
				}
				finally
				{
					if (V_1 != null)
					{
						V_1.Dispose();
					}
				}
			}
			return;
		}

		private IEnumerable<ILogicalConstruct> GetNodePredecessors(ILogicalConstruct node)
		{
			V_0 = new HashSet<ILogicalConstruct>();
			V_1 = node.get_SameParentPredecessors().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = (ILogicalConstruct)V_1.get_Current();
					if (this.removedEdges.ContainsKey(V_2) && this.removedEdges.get_Item(V_2).Contains(node))
					{
						continue;
					}
					V_0.Add(V_2);
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0;
		}

		private IEnumerable<ILogicalConstruct> GetNodeSuccessors(ILogicalConstruct node)
		{
			V_0 = new List<ILogicalConstruct>();
			V_1 = node.get_SameParentSuccessors().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = (ILogicalConstruct)V_1.get_Current();
					if (this.removedEdges.ContainsKey(node) && this.removedEdges.get_Item(node).Contains(V_2))
					{
						continue;
					}
					V_0.Add(V_2);
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0;
		}

		public List<IntervalConstruct> ReduceCfg()
		{
			this.headers.Enqueue(this.entryPoint);
			while (this.headers.get_Count() > 0)
			{
				V_0 = this.headers.Dequeue();
				V_1 = new IntervalConstruct(V_0);
				this.intervals.Add(V_1);
				this.FillInterval(V_0, V_1);
				this.AddNewHeaders(V_0, V_1);
			}
			this.CreateGraph(this.intervals);
			return this.SortIntervalList(this.intervals);
		}

		private List<IntervalConstruct> SortIntervalList(List<IntervalConstruct> intervals)
		{
			V_0 = new IntervalConstruct(intervals.get_Item(0));
			V_2 = intervals.GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					dummyVar0 = V_0.get_Children().Add(V_3);
				}
			}
			finally
			{
				((IDisposable)V_2).Dispose();
			}
			stackVariable15 = DFSTBuilder.BuildTree(V_0);
			V_1 = new List<IntervalConstruct>();
			V_4 = stackVariable15.get_ReversePostOrder().GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_5 = V_4.get_Current();
					V_1.Add(V_5.get_Construct() as IntervalConstruct);
				}
			}
			finally
			{
				((IDisposable)V_4).Dispose();
			}
			return V_1;
		}
	}
}