using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler.LogicFlow;
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
			base();
			this.typeSystem = typeSystem;
			return;
		}

		private void BuildAdjacencyMatrix(DFSTree dfsTree)
		{
			stackVariable2 = this.GetForwardFlowEdges(dfsTree);
			this.adjacencyMatrix = new int[(int)this.orderedVertexArray.Length, (int)this.orderedVertexArray.Length];
			V_0 = stackVariable2;
			V_1 = 0;
			while (V_1 < (int)V_0.Length)
			{
				V_2 = V_0[V_1];
				V_3 = V_2.get_Start().get_ReversePostOrderIndex();
				V_4 = V_2.get_End().get_ReversePostOrderIndex();
				if (V_3 != V_4)
				{
					V_5 = this.GetWeight(V_2.get_Start().get_Construct() as ILogicalConstruct, V_2.get_End().get_Construct() as ILogicalConstruct);
					this.adjacencyMatrix[V_3, V_4] = V_5;
				}
				V_1 = V_1 + 1;
			}
			return;
		}

		private void DetermineFollowNodesInSubGraph(ILogicalConstruct theGraph)
		{
			this.GetVerticesAndAdjacencyMatrix(theGraph);
			V_0 = MaxWeightDisjointPathsFinder.GetOptimalEdgesInDAG(this.adjacencyMatrix).GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.orderedVertexArray[V_1.get_Key()].set_CFGFollowNode(this.orderedVertexArray[V_1.get_Value()].get_FirstBlock());
					if (this.orderedVertexArray[V_1.get_Key()] as ConditionLogicalConstruct == null)
					{
						continue;
					}
					V_2 = this.orderedVertexArray[V_1.get_Key()] as ConditionLogicalConstruct;
					if (V_2.get_CFGFollowNode() != V_2.get_TrueCFGSuccessor())
					{
						continue;
					}
					V_2.Negate(this.typeSystem);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private void FillEdgesArray(DFSTEdge[] edgeArray, HashSet<DFSTEdge> edgeSet, ref int index)
		{
			V_0 = edgeSet.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = index;
					index = V_2 + 1;
					edgeArray[V_2] = V_1;
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private DFSTEdge[] GetForwardFlowEdges(DFSTree dfsTree)
		{
			V_0 = new DFSTEdge[dfsTree.get_TreeEdges().get_Count() + dfsTree.get_ForwardEdges().get_Count() + dfsTree.get_CrossEdges().get_Count()];
			V_1 = 0;
			this.FillEdgesArray(V_0, dfsTree.get_TreeEdges(), ref V_1);
			this.FillEdgesArray(V_0, dfsTree.get_ForwardEdges(), ref V_1);
			this.FillEdgesArray(V_0, dfsTree.get_CrossEdges(), ref V_1);
			return V_0;
		}

		private void GetVerticesAndAdjacencyMatrix(ILogicalConstruct graph)
		{
			V_0 = DFSTBuilder.BuildTree(graph);
			this.orderedVertexArray = new ILogicalConstruct[V_0.get_ReversePostOrder().get_Count()];
			V_1 = 0;
			while (V_1 < (int)this.orderedVertexArray.Length)
			{
				this.orderedVertexArray[V_1] = V_0.get_ReversePostOrder().get_Item(V_1).get_Construct() as ILogicalConstruct;
				V_1 = V_1 + 1;
			}
			this.BuildAdjacencyMatrix(V_0);
			return;
		}

		private int GetWeight(ILogicalConstruct start, ILogicalConstruct end)
		{
			V_0 = 0;
			V_1 = end.get_FirstBlock();
			V_2 = start.get_CFGBlocks().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					if (!V_2.get_Current().get_CFGSuccessors().Contains(V_1))
					{
						continue;
					}
					V_0 = V_0 + 1;
				}
			}
			finally
			{
				((IDisposable)V_2).Dispose();
			}
			return V_0;
		}

		public void ProcessConstruct(ILogicalConstruct theConstruct)
		{
			if (theConstruct as CFGBlockLogicalConstruct != null || theConstruct as ConditionLogicalConstruct != null)
			{
				return;
			}
			if (theConstruct as BlockLogicalConstruct != null)
			{
				this.DetermineFollowNodesInSubGraph(theConstruct);
			}
			V_0 = theConstruct.get_Children().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = (ILogicalConstruct)V_0.get_Current();
					this.ProcessConstruct(V_1);
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