using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Common;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DFST;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.FollowNodes
{
	internal class FollowNodeDeterminator
	{
		private readonly TypeSystem typeSystem;

		private ILogicalConstruct[] orderedVertexArray;

		private int[,] adjacencyMatrix;

		public FollowNodeDeterminator(TypeSystem typeSystem)
		{
			this.typeSystem = typeSystem;
		}

		private void BuildAdjacencyMatrix(DFSTree dfsTree)
		{
			DFSTEdge[] forwardFlowEdges = this.GetForwardFlowEdges(dfsTree);
			this.adjacencyMatrix = new int[(int)this.orderedVertexArray.Length, (int)this.orderedVertexArray.Length];
			DFSTEdge[] dFSTEdgeArray = forwardFlowEdges;
			for (int i = 0; i < (int)dFSTEdgeArray.Length; i++)
			{
				DFSTEdge dFSTEdge = dFSTEdgeArray[i];
				int reversePostOrderIndex = dFSTEdge.Start.ReversePostOrderIndex;
				int num = dFSTEdge.End.ReversePostOrderIndex;
				if (reversePostOrderIndex != num)
				{
					int weight = this.GetWeight(dFSTEdge.Start.Construct as ILogicalConstruct, dFSTEdge.End.Construct as ILogicalConstruct);
					this.adjacencyMatrix[reversePostOrderIndex, num] = weight;
				}
			}
		}

		private void DetermineFollowNodesInSubGraph(ILogicalConstruct theGraph)
		{
			this.GetVerticesAndAdjacencyMatrix(theGraph);
			foreach (KeyValuePair<int, int> optimalEdgesInDAG in MaxWeightDisjointPathsFinder.GetOptimalEdgesInDAG(this.adjacencyMatrix))
			{
				this.orderedVertexArray[optimalEdgesInDAG.Key].CFGFollowNode = this.orderedVertexArray[optimalEdgesInDAG.Value].FirstBlock;
				if (!(this.orderedVertexArray[optimalEdgesInDAG.Key] is ConditionLogicalConstruct))
				{
					continue;
				}
				ConditionLogicalConstruct conditionLogicalConstruct = this.orderedVertexArray[optimalEdgesInDAG.Key] as ConditionLogicalConstruct;
				if (conditionLogicalConstruct.CFGFollowNode != conditionLogicalConstruct.TrueCFGSuccessor)
				{
					continue;
				}
				conditionLogicalConstruct.Negate(this.typeSystem);
			}
		}

		private void FillEdgesArray(DFSTEdge[] edgeArray, HashSet<DFSTEdge> edgeSet, ref int index)
		{
			foreach (DFSTEdge dFSTEdge in edgeSet)
			{
				int num = index;
				index = num + 1;
				edgeArray[num] = dFSTEdge;
			}
		}

		private DFSTEdge[] GetForwardFlowEdges(DFSTree dfsTree)
		{
			DFSTEdge[] dFSTEdgeArray = new DFSTEdge[dfsTree.TreeEdges.Count + dfsTree.ForwardEdges.Count + dfsTree.CrossEdges.Count];
			int num = 0;
			this.FillEdgesArray(dFSTEdgeArray, dfsTree.TreeEdges, ref num);
			this.FillEdgesArray(dFSTEdgeArray, dfsTree.ForwardEdges, ref num);
			this.FillEdgesArray(dFSTEdgeArray, dfsTree.CrossEdges, ref num);
			return dFSTEdgeArray;
		}

		private void GetVerticesAndAdjacencyMatrix(ILogicalConstruct graph)
		{
			DFSTree dFSTree = DFSTBuilder.BuildTree(graph);
			this.orderedVertexArray = new ILogicalConstruct[dFSTree.ReversePostOrder.Count];
			for (int i = 0; i < (int)this.orderedVertexArray.Length; i++)
			{
				this.orderedVertexArray[i] = dFSTree.ReversePostOrder[i].Construct as ILogicalConstruct;
			}
			this.BuildAdjacencyMatrix(dFSTree);
		}

		private int GetWeight(ILogicalConstruct start, ILogicalConstruct end)
		{
			int num = 0;
			CFGBlockLogicalConstruct firstBlock = end.FirstBlock;
			foreach (CFGBlockLogicalConstruct cFGBlock in start.CFGBlocks)
			{
				if (!cFGBlock.CFGSuccessors.Contains(firstBlock))
				{
					continue;
				}
				num++;
			}
			return num;
		}

		public void ProcessConstruct(ILogicalConstruct theConstruct)
		{
			if (theConstruct is CFGBlockLogicalConstruct || theConstruct is ConditionLogicalConstruct)
			{
				return;
			}
			if (theConstruct is BlockLogicalConstruct)
			{
				this.DetermineFollowNodesInSubGraph(theConstruct);
			}
			foreach (ILogicalConstruct child in theConstruct.Children)
			{
				this.ProcessConstruct(child);
			}
		}
	}
}