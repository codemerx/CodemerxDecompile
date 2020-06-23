using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Common;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.DFST
{
	internal class DFSTBuilder
	{
		private readonly HashSet<DFSTNode> traversedNodes = new HashSet<DFSTNode>();

		private readonly HashSet<DFSTNode> currentPath = new HashSet<DFSTNode>();

		private readonly Dictionary<ISingleEntrySubGraph, DFSTNode> constructToNodeMap = new Dictionary<ISingleEntrySubGraph, DFSTNode>();

		private readonly ISingleEntrySubGraph theGraph;

		private readonly ISingleEntrySubGraph entry;

		private DFSTree theTree;

		private DFSTBuilder(ISingleEntrySubGraph theGraph, ISingleEntrySubGraph entry)
		{
			this.theGraph = theGraph;
			this.entry = entry;
		}

		private void AssignOrderIndices()
		{
			foreach (DFSTNode value in this.constructToNodeMap.Values)
			{
				value.ReversePostOrderIndex = -1;
			}
			for (int i = 0; i < this.theTree.ReversePostOrder.Count; i++)
			{
				this.theTree.ReversePostOrder[i].ReversePostOrderIndex = i;
			}
		}

		public static DFSTree BuildTree(ISingleEntrySubGraph theGraph)
		{
			return DFSTBuilder.BuildTree(theGraph, theGraph.Entry);
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
			DFSTNode dFSTNode;
			this.traversedNodes.Add(currentNode);
			this.currentPath.Add(currentNode);
			foreach (ISingleEntrySubGraph traversableSuccessor in LogicalFlowUtilities.GetTraversableSuccessors(currentNode.Construct))
			{
				if (!this.constructToNodeMap.TryGetValue(traversableSuccessor, out dFSTNode))
				{
					continue;
				}
				if (!this.traversedNodes.Contains(dFSTNode))
				{
					dFSTNode.Predecessor = currentNode;
					currentNode.TreeEdgeSuccessors.Add(dFSTNode);
					this.theTree.TreeEdges.Add(new DFSTEdge(currentNode, dFSTNode));
					this.DFSBuild(dFSTNode);
				}
				else if (this.currentPath.Contains(dFSTNode))
				{
					dFSTNode.BackEdgePredecessors.Add(currentNode);
					currentNode.BackEdgeSuccessors.Add(dFSTNode);
					this.theTree.BackEdges.Add(new DFSTEdge(currentNode, dFSTNode));
				}
				else if (!this.IsAncestor(dFSTNode, currentNode))
				{
					dFSTNode.CrossEdgePredecessors.Add(currentNode);
					currentNode.CrossEdgeSuccessors.Add(dFSTNode);
					this.theTree.CrossEdges.Add(new DFSTEdge(currentNode, dFSTNode));
				}
				else
				{
					dFSTNode.ForwardEdgePredecessors.Add(currentNode);
					currentNode.ForwardEdgeSucessors.Add(dFSTNode);
					this.theTree.ForwardEdges.Add(new DFSTEdge(currentNode, dFSTNode));
				}
			}
			this.currentPath.Remove(currentNode);
			this.theTree.ReversePostOrder.Add(currentNode);
		}

		private bool IsAncestor(DFSTNode node, DFSTNode supposedAncestor)
		{
			for (DFSTNode i = (DFSTNode)node.Predecessor; i != null; i = (DFSTNode)i.Predecessor)
			{
				if (i == supposedAncestor)
				{
					return true;
				}
			}
			return false;
		}

		private void MapChilds()
		{
			foreach (ISingleEntrySubGraph child in this.theGraph.Children)
			{
				this.constructToNodeMap.Add(child, new DFSTNode(child));
			}
		}

		private void TraverseAndBuildTree()
		{
			this.theTree = new DFSTree(this.constructToNodeMap);
			this.DFSBuild(this.constructToNodeMap[this.entry]);
			this.theTree.ReversePostOrder.Reverse();
			this.theTree.ReversePostOrder.TrimExcess();
			this.AssignOrderIndices();
		}
	}
}