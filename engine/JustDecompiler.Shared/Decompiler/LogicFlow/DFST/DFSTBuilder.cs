using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.DFST
{
    internal class DFSTBuilder
    {
        private readonly HashSet<DFSTNode> traversedNodes = new HashSet<DFSTNode>();
        private readonly HashSet<DFSTNode> currentPath = new HashSet<DFSTNode>();
        private readonly Dictionary<ISingleEntrySubGraph, DFSTNode> constructToNodeMap = new Dictionary<ISingleEntrySubGraph,DFSTNode>();
        private readonly ISingleEntrySubGraph theGraph;
        private readonly ISingleEntrySubGraph entry;
        private DFSTree theTree;

        private DFSTBuilder(ISingleEntrySubGraph theGraph, ISingleEntrySubGraph entry)
        {
            this.theGraph = theGraph;
            this.entry = entry;
        }

        /// <summary>
        /// Creats a DFS traversal tree for the specified graph.
        /// </summary>
        /// <param name="theGraph"></param>
        /// <returns></returns>
        public static DFSTree BuildTree(ISingleEntrySubGraph theGraph)
        {
            return BuildTree(theGraph, theGraph.Entry);
        }

        public static DFSTree BuildTree(ISingleEntrySubGraph theGraph, ISingleEntrySubGraph entry)
        {
            return BuildTreeInternal(new DFSTBuilder(theGraph, entry));
        }

        private static DFSTree BuildTreeInternal(DFSTBuilder theBuilder)
        {
            theBuilder.MapChilds();
            theBuilder.TraverseAndBuildTree();
            return theBuilder.theTree;
        }

        /// <summary>
        /// Maps each child of the given graph to a new node.
        /// </summary>
        private void MapChilds()
        {
            foreach (ISingleEntrySubGraph child in theGraph.Children)
            {
                constructToNodeMap.Add(child, new DFSTNode(child));
            }
        }

        /// <summary>
        /// Builds the tree starting from the graph's entry.
        /// </summary>
        private void TraverseAndBuildTree()
        {
            theTree = new DFSTree(constructToNodeMap);

            DFSTNode root = constructToNodeMap[this.entry];

            DFSBuild(root);

            //Since the nodes were added in postorder we must reverse them to get ... the reverse postorder.
            theTree.ReversePostOrder.Reverse();
            theTree.ReversePostOrder.TrimExcess();

            AssignOrderIndices();
        }

        /// <summary>
        /// Recursively traverses the nodes and adds them to the tree. Also determines the type of each edge.
        /// </summary>
        /// <param name="currentNode"></param>
        private void DFSBuild(DFSTNode currentNode)
        {
            traversedNodes.Add(currentNode);

            //The current path is the set of all nodes on the DFS tree path from the root to the current node.
            currentPath.Add(currentNode);

            foreach (ISingleEntrySubGraph successorConstruct in LogicalFlowUtilities.GetTraversableSuccessors(currentNode.Construct))
            {
                DFSTNode successor;
                if(!constructToNodeMap.TryGetValue(successorConstruct, out successor))
                {
                    //Special case for interval constructs
                    continue;
                }

                if (!traversedNodes.Contains(successor))
                {
                    //If the successor is not traversed then the edge between the two nodes is a tree edge.
                    successor.Predecessor = currentNode;
                    currentNode.TreeEdgeSuccessors.Add(successor);
                    theTree.TreeEdges.Add(new DFSTEdge(currentNode, successor));

                    //We continue the build from this successor.
                    DFSBuild(successor);
                }
                else if(currentPath.Contains(successor))
                {
                    //If the successor is traversed and is on the current path then the edge between the nodes is a back edge.
                    successor.BackEdgePredecessors.Add(currentNode);
                    currentNode.BackEdgeSuccessors.Add(successor);
                    theTree.BackEdges.Add(new DFSTEdge(currentNode, successor));
                }
                else if (IsAncestor(successor, currentNode))
                {
                    //If the successor is traversed and the current node is its ancestor, then the edge is a forward edge.
                    successor.ForwardEdgePredecessors.Add(currentNode);
                    currentNode.ForwardEdgeSucessors.Add(successor);
                    theTree.ForwardEdges.Add(new DFSTEdge(currentNode, successor));
                }
                else
                {
                    //Otherwise the edge between the nodes is a cross edge.
                    successor.CrossEdgePredecessors.Add(currentNode);
                    currentNode.CrossEdgeSuccessors.Add(successor);
                    theTree.CrossEdges.Add(new DFSTEdge(currentNode, successor));
                }
            }

            currentPath.Remove(currentNode);

            //Adding the nodes in post order.
            theTree.ReversePostOrder.Add(currentNode);
        }

        /// <summary>
        /// Determines whether <paramref name="supposedAncestor"/> is on the tree path from the root to <paramref name="node"/>
        /// </summary>
        /// <param name="node"></param>
        /// <param name="supposedAncestor"></param>
        /// <returns></returns>
        private bool IsAncestor(DFSTNode node, DFSTNode supposedAncestor)
        {
            DFSTNode currentAncestor = (DFSTNode)node.Predecessor;

            while (currentAncestor != null)
            {
                if (currentAncestor == supposedAncestor)
                {
                    return true;
                }

                currentAncestor = (DFSTNode)currentAncestor.Predecessor;
            }
            return false;
        }

        /// <summary>
        /// Sets the ReversePostOrderIndex property for each node, based on the results of the DFS.
        /// </summary>
        private void AssignOrderIndices()
        {
            foreach (DFSTNode node in constructToNodeMap.Values)
            {
                node.ReversePostOrderIndex = -1;
            }

            //In some cases there are nodes not reachable from the entry of the graph (i.e. dead code).
            //These nodes will remain with index -1.
            for (int i = 0; i < theTree.ReversePostOrder.Count; i++)
            {
                theTree.ReversePostOrder[i].ReversePostOrderIndex = i;
            }
        }
    }
}
