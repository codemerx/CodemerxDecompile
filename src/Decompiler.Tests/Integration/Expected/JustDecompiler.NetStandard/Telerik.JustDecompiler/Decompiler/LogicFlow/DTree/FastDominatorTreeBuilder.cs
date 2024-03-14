using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Common;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DFST;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.DTree
{
	internal class FastDominatorTreeBuilder : BaseDominatorTreeBuilder
	{
		private int[] dominators;

		private FastDominatorTreeBuilder(ISingleEntrySubGraph graph) : base(graph)
		{
		}

		public static DominatorTree BuildTree(ISingleEntrySubGraph graph)
		{
			return BaseDominatorTreeBuilder.BuildTreeInternal(new FastDominatorTreeBuilder(graph));
		}

		protected override void FindImmediateDominators()
		{
			bool flag;
			List<DFSTNode> reversePostOrder = DFSTBuilder.BuildTree(this.originalGraph).ReversePostOrder;
			List<DFSTNode>[] listArrays = this.InitializePredecessors(reversePostOrder);
			int count = reversePostOrder.Count;
			this.InitializeDominators(count);
			do
			{
				flag = false;
				for (int i = 1; i < count; i++)
				{
					List<DFSTNode> dFSTNodes = listArrays[i];
					int predecessor = this.GetPredecessor(dFSTNodes, (DFSTNode node) => {
						if (node.ReversePostOrderIndex == i)
						{
							return false;
						}
						return this.dominators[node.ReversePostOrderIndex] != -1;
					});
					foreach (DFSTNode dFSTNode in dFSTNodes)
					{
						int reversePostOrderIndex = dFSTNode.ReversePostOrderIndex;
						if (reversePostOrderIndex == i || this.dominators[reversePostOrderIndex] == -1)
						{
							continue;
						}
						predecessor = this.Intersect(reversePostOrderIndex, predecessor);
					}
					if (this.dominators[i] != predecessor)
					{
						this.dominators[i] = predecessor;
						flag = true;
					}
				}
			}
			while (flag);
			for (int j = 1; j < count; j++)
			{
				DTNode item = this.constructToNodeMap[reversePostOrder[j].Construct];
				DTNode dTNode = this.constructToNodeMap[reversePostOrder[this.dominators[j]].Construct];
				item.Predecessor = dTNode;
				dTNode.TreeEdgeSuccessors.Add(item);
			}
		}

		private int GetPredecessor(List<DFSTNode> predecessors, Predicate<DFSTNode> predicate)
		{
			int reversePostOrderIndex;
			List<DFSTNode>.Enumerator enumerator = predecessors.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DFSTNode current = enumerator.Current;
					if (!predicate(current))
					{
						continue;
					}
					reversePostOrderIndex = current.ReversePostOrderIndex;
					return reversePostOrderIndex;
				}
				throw new Exception("No such element");
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return reversePostOrderIndex;
		}

		private List<DFSTNode> GetPredecessors(DFSTNode node)
		{
			List<DFSTNode> dFSTNodes = new List<DFSTNode>();
			if (node.Predecessor != null)
			{
				dFSTNodes.Add(node.Predecessor as DFSTNode);
			}
			dFSTNodes.AddRange(node.ForwardEdgePredecessors);
			dFSTNodes.AddRange(node.CrossEdgePredecessors);
			dFSTNodes.AddRange(node.BackEdgePredecessors);
			return dFSTNodes;
		}

		private void InitializeDominators(int count)
		{
			this.dominators = new Int32[count];
			this.dominators[0] = 0;
			for (int i = 1; i < count; i++)
			{
				this.dominators[i] = -1;
			}
		}

		private List<DFSTNode>[] InitializePredecessors(List<DFSTNode> nodes)
		{
			List<DFSTNode>[] predecessors = new List<DFSTNode>[nodes.Count];
			for (int i = 0; i < nodes.Count; i++)
			{
				predecessors[i] = this.GetPredecessors(nodes[i]);
			}
			return predecessors;
		}

		private int Intersect(int first, int second)
		{
			while (first != second)
			{
				while (first > second)
				{
					first = this.dominators[first];
				}
				while (second > first)
				{
					second = this.dominators[second];
				}
			}
			return first;
		}
	}
}