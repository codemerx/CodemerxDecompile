using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Common;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.DTree
{
	public class DominatorTree
	{
		private readonly Dictionary<ISingleEntrySubGraph, DTNode> constructToNodeMap;

		public ISingleEntrySubGraph RootConstruct
		{
			get;
			private set;
		}

		internal DominatorTree(Dictionary<ISingleEntrySubGraph, DTNode> constructToNodeMap, ISingleEntrySubGraph rootConstruct)
		{
			this.constructToNodeMap = constructToNodeMap;
			this.RootConstruct = rootConstruct;
		}

		public HashSet<ISingleEntrySubGraph> GetDominanceFrontier(ISingleEntrySubGraph construct)
		{
			DTNode dTNode;
			if (!this.constructToNodeMap.TryGetValue(construct, out dTNode))
			{
				return null;
			}
			HashSet<ISingleEntrySubGraph> singleEntrySubGraphs = new HashSet<ISingleEntrySubGraph>();
			foreach (DTNode dominanceFrontier in dTNode.DominanceFrontier)
			{
				singleEntrySubGraphs.Add(dominanceFrontier.Construct);
			}
			return singleEntrySubGraphs;
		}

		public HashSet<ISingleEntrySubGraph> GetDominatedNodes(ISingleEntrySubGraph construct)
		{
			DTNode dTNode;
			if (!this.constructToNodeMap.TryGetValue(construct, out dTNode))
			{
				return null;
			}
			HashSet<ISingleEntrySubGraph> singleEntrySubGraphs = new HashSet<ISingleEntrySubGraph>();
			Queue<DTNode> dTNodes = new Queue<DTNode>();
			dTNodes.Enqueue(dTNode);
			while (dTNodes.Count > 0)
			{
				DTNode dTNode1 = dTNodes.Dequeue();
				singleEntrySubGraphs.Add(dTNode1.Construct);
				foreach (DTNode treeEdgeSuccessor in dTNode1.TreeEdgeSuccessors)
				{
					dTNodes.Enqueue(treeEdgeSuccessor);
				}
			}
			return singleEntrySubGraphs;
		}

		public HashSet<ISingleEntrySubGraph> GetDominators(ISingleEntrySubGraph construct)
		{
			DTNode dTNode;
			if (!this.constructToNodeMap.TryGetValue(construct, out dTNode))
			{
				return null;
			}
			HashSet<ISingleEntrySubGraph> singleEntrySubGraphs = new HashSet<ISingleEntrySubGraph>();
			foreach (DTNode dominator in dTNode.Dominators)
			{
				singleEntrySubGraphs.Add(dominator.Construct);
			}
			return singleEntrySubGraphs;
		}

		public ISingleEntrySubGraph GetImmediateDominator(ISingleEntrySubGraph construct)
		{
			DTNode dTNode;
			if (!this.constructToNodeMap.TryGetValue(construct, out dTNode) || dTNode.Predecessor == null)
			{
				return null;
			}
			return dTNode.Predecessor.Construct;
		}

		public void MergeNodes(HashSet<ISingleEntrySubGraph> constructs, ISingleEntrySubGraph originalEntry, ISingleEntrySubGraph newConstruct)
		{
			DTNode item = this.constructToNodeMap[originalEntry];
			DTNode dTNode = new DTNode(newConstruct)
			{
				Predecessor = item.Predecessor
			};
			dTNode.DominanceFrontier.UnionWith(item.DominanceFrontier);
			dTNode.DominanceFrontier.Remove(item);
			if (dTNode.Predecessor != null)
			{
				dTNode.Predecessor.TreeEdgeSuccessors.Remove(item);
				dTNode.Predecessor.TreeEdgeSuccessors.Add(dTNode);
			}
			foreach (ISingleEntrySubGraph construct in constructs)
			{
				this.constructToNodeMap.Remove(construct);
			}
			foreach (KeyValuePair<ISingleEntrySubGraph, DTNode> keyValuePair in this.constructToNodeMap)
			{
				if (keyValuePair.Value.Predecessor != null && constructs.Contains(keyValuePair.Value.Predecessor.Construct))
				{
					keyValuePair.Value.Predecessor = dTNode;
					dTNode.TreeEdgeSuccessors.Add(keyValuePair.Value);
				}
				if (!keyValuePair.Value.DominanceFrontier.Remove(item))
				{
					continue;
				}
				keyValuePair.Value.DominanceFrontier.Add(dTNode);
			}
			if (this.RootConstruct == originalEntry)
			{
				this.RootConstruct = newConstruct;
			}
			this.constructToNodeMap.Add(newConstruct, dTNode);
		}
	}
}