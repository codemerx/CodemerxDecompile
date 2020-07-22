using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.DFST
{
	internal class DFSTBuilder
	{
		private readonly HashSet<DFSTNode> traversedNodes;

		private readonly HashSet<DFSTNode> currentPath;

		private readonly Dictionary<ISingleEntrySubGraph, DFSTNode> constructToNodeMap;

		private readonly ISingleEntrySubGraph theGraph;

		private readonly ISingleEntrySubGraph entry;

		private DFSTree theTree;

		private DFSTBuilder(ISingleEntrySubGraph theGraph, ISingleEntrySubGraph entry)
		{
			this.traversedNodes = new HashSet<DFSTNode>();
			this.currentPath = new HashSet<DFSTNode>();
			this.constructToNodeMap = new Dictionary<ISingleEntrySubGraph, DFSTNode>();
			base();
			this.theGraph = theGraph;
			this.entry = entry;
			return;
		}

		private void AssignOrderIndices()
		{
			V_0 = this.constructToNodeMap.get_Values().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_0.get_Current().set_ReversePostOrderIndex(-1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			V_1 = 0;
			while (V_1 < this.theTree.get_ReversePostOrder().get_Count())
			{
				this.theTree.get_ReversePostOrder().get_Item(V_1).set_ReversePostOrderIndex(V_1);
				V_1 = V_1 + 1;
			}
			return;
		}

		public static DFSTree BuildTree(ISingleEntrySubGraph theGraph)
		{
			return DFSTBuilder.BuildTree(theGraph, theGraph.get_Entry());
		}

		public static DFSTree BuildTree(ISingleEntrySubGraph theGraph, ISingleEntrySubGraph entry)
		{
			return DFSTBuilder.BuildTreeInternal(new DFSTBuilder(theGraph, entry));
		}

		private static DFSTree BuildTreeInternal(DFSTBuilder theBuilder)
		{
			theBuilder.MapChilds();
			theBuilder.TraverseAndBuildTree();
			return theBuilder.theTree;
		}

		private void DFSBuild(DFSTNode currentNode)
		{
			dummyVar0 = this.traversedNodes.Add(currentNode);
			dummyVar1 = this.currentPath.Add(currentNode);
			V_0 = LogicalFlowUtilities.GetTraversableSuccessors(currentNode.get_Construct()).GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!this.constructToNodeMap.TryGetValue(V_1, out V_2))
					{
						continue;
					}
					if (this.traversedNodes.Contains(V_2))
					{
						if (!this.currentPath.Contains(V_2))
						{
							if (!this.IsAncestor(V_2, currentNode))
							{
								dummyVar10 = V_2.get_CrossEdgePredecessors().Add(currentNode);
								dummyVar11 = currentNode.get_CrossEdgeSuccessors().Add(V_2);
								dummyVar12 = this.theTree.get_CrossEdges().Add(new DFSTEdge(currentNode, V_2));
							}
							else
							{
								dummyVar7 = V_2.get_ForwardEdgePredecessors().Add(currentNode);
								dummyVar8 = currentNode.get_ForwardEdgeSucessors().Add(V_2);
								dummyVar9 = this.theTree.get_ForwardEdges().Add(new DFSTEdge(currentNode, V_2));
							}
						}
						else
						{
							dummyVar4 = V_2.get_BackEdgePredecessors().Add(currentNode);
							dummyVar5 = currentNode.get_BackEdgeSuccessors().Add(V_2);
							dummyVar6 = this.theTree.get_BackEdges().Add(new DFSTEdge(currentNode, V_2));
						}
					}
					else
					{
						V_2.set_Predecessor(currentNode);
						dummyVar2 = currentNode.get_TreeEdgeSuccessors().Add(V_2);
						dummyVar3 = this.theTree.get_TreeEdges().Add(new DFSTEdge(currentNode, V_2));
						this.DFSBuild(V_2);
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
			dummyVar13 = this.currentPath.Remove(currentNode);
			this.theTree.get_ReversePostOrder().Add(currentNode);
			return;
		}

		private bool IsAncestor(DFSTNode node, DFSTNode supposedAncestor)
		{
			V_0 = (DFSTNode)node.get_Predecessor();
			while (V_0 != null)
			{
				if (V_0 == supposedAncestor)
				{
					return true;
				}
				V_0 = (DFSTNode)V_0.get_Predecessor();
			}
			return false;
		}

		private void MapChilds()
		{
			V_0 = this.theGraph.get_Children().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.constructToNodeMap.Add(V_1, new DFSTNode(V_1));
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private void TraverseAndBuildTree()
		{
			this.theTree = new DFSTree(this.constructToNodeMap);
			this.DFSBuild(this.constructToNodeMap.get_Item(this.entry));
			this.theTree.get_ReversePostOrder().Reverse();
			this.theTree.get_ReversePostOrder().TrimExcess();
			this.AssignOrderIndices();
			return;
		}
	}
}