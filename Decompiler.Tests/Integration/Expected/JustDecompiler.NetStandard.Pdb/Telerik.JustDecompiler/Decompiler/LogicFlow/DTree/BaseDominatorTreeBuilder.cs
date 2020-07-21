using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.DTree
{
	internal abstract class BaseDominatorTreeBuilder
	{
		protected readonly ISingleEntrySubGraph originalGraph;

		protected readonly Dictionary<ISingleEntrySubGraph, DTNode> constructToNodeMap;

		protected readonly ISingleEntrySubGraph rootConstruct;

		protected readonly Dictionary<DTNode, HashSet<DTNode>> predecessorMap;

		protected BaseDominatorTreeBuilder(ISingleEntrySubGraph graph)
		{
			this.constructToNodeMap = new Dictionary<ISingleEntrySubGraph, DTNode>();
			this.predecessorMap = new Dictionary<DTNode, HashSet<DTNode>>();
			base();
			this.originalGraph = graph;
			this.rootConstruct = graph.get_Entry();
			return;
		}

		protected static DominatorTree BuildTreeInternal(BaseDominatorTreeBuilder theBuilder)
		{
			theBuilder.MapNodes();
			theBuilder.MapPredecessors();
			theBuilder.FindImmediateDominators();
			theBuilder.ComputeDominanceFrontiers();
			return new DominatorTree(theBuilder.constructToNodeMap, theBuilder.rootConstruct);
		}

		private void ComputeDominanceFrontiers()
		{
			V_0 = this.constructToNodeMap.get_Values().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = this.predecessorMap.get_Item(V_1).GetEnumerator();
					try
					{
						while (V_2.MoveNext())
						{
							V_3 = V_2.get_Current();
							while (V_3 != V_1.get_ImmediateDominator())
							{
								dummyVar0 = V_3.get_DominanceFrontier().Add(V_1);
								V_3 = V_3.get_ImmediateDominator();
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
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		protected abstract void FindImmediateDominators();

		private void MapNodes()
		{
			V_0 = this.originalGraph.get_Children().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.constructToNodeMap.Add(V_1, new DTNode(V_1));
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			if (!this.constructToNodeMap.ContainsKey(this.rootConstruct))
			{
				throw new ArgumentException("The Graph does not contain the given start node");
			}
			return;
		}

		private void MapPredecessors()
		{
			V_0 = this.originalGraph.get_Children().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1 != this.rootConstruct)
					{
						stackVariable11 = new HashSet<ISingleEntrySubGraph>();
						stackVariable11.UnionWith(V_1.get_SameParentPredecessors());
						V_2 = new HashSet<DTNode>();
						V_3 = stackVariable11.GetEnumerator();
						try
						{
							while (V_3.MoveNext())
							{
								V_4 = V_3.get_Current();
								if (!this.constructToNodeMap.TryGetValue(V_4, out V_5))
								{
									throw new ArgumentException("The desired predecessor is not child of the same subgraph");
								}
								dummyVar0 = V_2.Add(V_5);
							}
						}
						finally
						{
							((IDisposable)V_3).Dispose();
						}
						this.predecessorMap.set_Item(this.constructToNodeMap.get_Item(V_1), V_2);
					}
					else
					{
						this.predecessorMap.set_Item(this.constructToNodeMap.get_Item(V_1), new HashSet<DTNode>());
					}
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}
	}
}