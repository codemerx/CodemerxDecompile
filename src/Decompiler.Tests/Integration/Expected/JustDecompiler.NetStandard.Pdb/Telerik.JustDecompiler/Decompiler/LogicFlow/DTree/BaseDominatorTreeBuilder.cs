using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.DTree
{
	internal abstract class BaseDominatorTreeBuilder
	{
		protected readonly ISingleEntrySubGraph originalGraph;

		protected readonly Dictionary<ISingleEntrySubGraph, DTNode> constructToNodeMap = new Dictionary<ISingleEntrySubGraph, DTNode>();

		protected readonly ISingleEntrySubGraph rootConstruct;

		protected readonly Dictionary<DTNode, HashSet<DTNode>> predecessorMap = new Dictionary<DTNode, HashSet<DTNode>>();

		protected BaseDominatorTreeBuilder(ISingleEntrySubGraph graph)
		{
			this.originalGraph = graph;
			this.rootConstruct = graph.Entry;
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
			foreach (DTNode value in this.constructToNodeMap.Values)
			{
				foreach (DTNode item in this.predecessorMap[value])
				{
					while (item != value.ImmediateDominator)
					{
						item.DominanceFrontier.Add(value);
						item = item.ImmediateDominator;
					}
				}
			}
		}

		protected abstract void FindImmediateDominators();

		private void MapNodes()
		{
			foreach (ISingleEntrySubGraph child in this.originalGraph.Children)
			{
				this.constructToNodeMap.Add(child, new DTNode(child));
			}
			if (!this.constructToNodeMap.ContainsKey(this.rootConstruct))
			{
				throw new ArgumentException("The Graph does not contain the given start node");
			}
		}

		private void MapPredecessors()
		{
			DTNode dTNode;
			foreach (ISingleEntrySubGraph child in this.originalGraph.Children)
			{
				if (child != this.rootConstruct)
				{
					HashSet<ISingleEntrySubGraph> singleEntrySubGraphs = new HashSet<ISingleEntrySubGraph>();
					singleEntrySubGraphs.UnionWith(child.SameParentPredecessors);
					HashSet<DTNode> dTNodes = new HashSet<DTNode>();
					foreach (ISingleEntrySubGraph singleEntrySubGraph in singleEntrySubGraphs)
					{
						if (!this.constructToNodeMap.TryGetValue(singleEntrySubGraph, out dTNode))
						{
							throw new ArgumentException("The desired predecessor is not child of the same subgraph");
						}
						dTNodes.Add(dTNode);
					}
					this.predecessorMap[this.constructToNodeMap[child]] = dTNodes;
				}
				else
				{
					this.predecessorMap[this.constructToNodeMap[child]] = new HashSet<DTNode>();
				}
			}
		}
	}
}