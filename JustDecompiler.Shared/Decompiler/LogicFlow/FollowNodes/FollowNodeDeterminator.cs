using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DFST;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.FollowNodes
{
    internal class FollowNodeDeterminator
    {
        private readonly TypeSystem typeSystem;

        public FollowNodeDeterminator(TypeSystem typeSystem)
        {
            this.typeSystem = typeSystem;
        }

        /// <summary>
        /// Recursively determines the follow nodes of all logical constructs in the given construct.
        /// </summary>
        /// <remarks>
        /// We need to determine only the follow nodes of the constructs that are children of block logical constructs. In every other construct
        /// the order is determined by the type of the construct.
        /// </remarks>
        /// <param name="theConstruct"></param>
        public void ProcessConstruct(ILogicalConstruct theConstruct)
        {
            if(theConstruct is CFGBlockLogicalConstruct || theConstruct is ConditionLogicalConstruct)
            {
                return;
            }

            if (theConstruct is BlockLogicalConstruct)
            {
                DetermineFollowNodesInSubGraph(theConstruct);
            }

            foreach (ILogicalConstruct child in theConstruct.Children)
            {
                ProcessConstruct(child);
            }
        }

        //adjacencyMatrix[i, j] holds the weight of the edge (orderedVertexArray[i], orderedVertexArray[j])
        private ILogicalConstruct[] orderedVertexArray;
        private int[,] adjacencyMatrix;

        /// <summary>
        /// Determines the follow nodes in the given subgraph.
        /// </summary>
        /// <remarks>
        /// Every node can follow only one node.
        /// Everey node can be followed by only one node.
        /// The follow node relation cannot form a back edge. (i.e. B can be the follow node of A <=> (A, B) is not a backedge)
        /// Let G = (V, E) be the original graph.
        /// Let G' = (V, E') be the subgraph of G, where E' = E \ { e | e is from E && e is backedge in the DFS traversal }.
        /// 
        /// The solution is to find a cover of vertex-disjoint paths of G' that will yield the minimum number of gotos.
        /// The gotos will be the edges that are not in the cover. So we want the edges that will yeild the greatest number of gotos to
        /// be in the cover. If we find a good weight function for the edges in the graph (i.e. correctly determines how much gotos there will be between
        /// two nodes), then all we have to do is find a cover of vertex-disjoint paths of G' with maximum total weight.
        /// </remarks>
        /// <param name="theGraph"></param>
        private void DetermineFollowNodesInSubGraph(ILogicalConstruct theGraph)
        {
            //Creates the adjacency matrix for the mentioned subgraph - G'.
            GetVerticesAndAdjacencyMatrix(theGraph);

            //Since no back edge is left in G', then G' is a DAG.
            List<KeyValuePair<int, int>> followEdges = MaxWeightDisjointPathsFinder.GetOptimalEdgesInDAG(adjacencyMatrix);

            foreach (KeyValuePair<int, int> followEdge in followEdges)
            {
                orderedVertexArray[followEdge.Key].CFGFollowNode = orderedVertexArray[followEdge.Value].FirstBlock;
                if(orderedVertexArray[followEdge.Key] is ConditionLogicalConstruct)
                {
                    ConditionLogicalConstruct theCondition = orderedVertexArray[followEdge.Key] as ConditionLogicalConstruct;
                    if(theCondition.CFGFollowNode == theCondition.TrueCFGSuccessor)
                    {
                        //If the follow node of a condition construct is the true successor, then we need to negate the condition construct.
                        theCondition.Negate(typeSystem);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a subgraph of the specified <paramref name="graph"/>.
        /// </summary>
        /// <remarks>
        /// The subgraph has the same vertices as the original. The difference is that the subgraph does not contain the back edges
        /// found by the dfs traversal of the <paramref name="graph"/>.
        /// </remarks>
        /// <param name="graph"></param>
        private void GetVerticesAndAdjacencyMatrix(ILogicalConstruct graph)
        {
            DFSTree dfsTree = DFSTBuilder.BuildTree(graph);

            orderedVertexArray = new ILogicalConstruct[dfsTree.ReversePostOrder.Count];
            for (int i = 0; i < orderedVertexArray.Length; i++)
            {
                orderedVertexArray[i] = dfsTree.ReversePostOrder[i].Construct as ILogicalConstruct;
            }

            BuildAdjacencyMatrix(dfsTree);
        }

        /// <summary>
        /// Builds the adjacency matrix of the subgraph by using the given <paramref name="dfsTree"/>.
        /// </summary>
        /// <remarks>
        /// The back edges are not included in the built subgraph.
        /// </remarks>
        /// <param name="dfsTree"></param>
        private void BuildAdjacencyMatrix(DFSTree dfsTree)
        {
            DFSTEdge[] edgeArray = GetForwardFlowEdges(dfsTree);
            adjacencyMatrix = new int[orderedVertexArray.Length, orderedVertexArray.Length];
            
            foreach (DFSTEdge edge in edgeArray)
            {
                int startIndex = edge.Start.ReversePostOrderIndex;
                int endIndex = edge.End.ReversePostOrderIndex;

                if(startIndex == endIndex)
                {
                    continue;
                }

                int weight = GetWeight(edge.Start.Construct as ILogicalConstruct, edge.End.Construct as ILogicalConstruct);
                adjacencyMatrix[startIndex, endIndex] = weight;
            }
        }

        /// <summary>
        /// Gets an array containing the tree, forward and cross edges of the given DFS traversal tree.
        /// </summary>
        /// <param name="dfsTree">The DFS traversal tree.</param>
        /// <returns></returns>
        private DFSTEdge[] GetForwardFlowEdges(DFSTree dfsTree)
        {
            DFSTEdge[] regularEdges = new DFSTEdge[dfsTree.TreeEdges.Count + dfsTree.ForwardEdges.Count + dfsTree.CrossEdges.Count];

            int index = 0;
            FillEdgesArray(regularEdges, dfsTree.TreeEdges, ref index);
            FillEdgesArray(regularEdges, dfsTree.ForwardEdges, ref index);
            FillEdgesArray(regularEdges, dfsTree.CrossEdges, ref index);
            return regularEdges;
        }

        /// <summary>
        /// Fills the <paramref name="edgeArray"/>, strating from the given <paramref name="index"/>, with all of the elements in the
        /// specified <paramref name="edgeSet"/>.
        /// </summary>
        /// <param name="edgeArray"></param>
        /// <param name="currentSet"></param>
        /// <param name="index"></param>
        private void FillEdgesArray(DFSTEdge[] edgeArray, HashSet<DFSTEdge> edgeSet, ref int index)
        {
            foreach (DFSTEdge edge in edgeSet)
            {
                edgeArray[index++] = edge;
            }
        }

        /// <summary>
        /// Determines the weight of the given edge.
        /// </summary>
        /// <remarks>
        /// This is the heuristic part of the algorithm.
        /// The weight of an edge is determined by the number of CFG block constructs in the given <paramref name="start"/> construct,
        /// that have for successor the specified <paramref name="end"/> construct.
        /// </remarks>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private int GetWeight(ILogicalConstruct start, ILogicalConstruct end)
        {
            int weight = 0;
            CFGBlockLogicalConstruct endCFGEntry = end.FirstBlock;

            foreach (CFGBlockLogicalConstruct cfgChild in start.CFGBlocks)
            {
                if (cfgChild.CFGSuccessors.Contains(endCFGEntry))
                {
                    weight++;
                }
            }

            return weight;
        }
    }
}
