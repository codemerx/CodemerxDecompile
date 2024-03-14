using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.DFST
{
    internal class DFSTree
    {
        /// <summary>
        /// Gets the root node of the tree.
        /// </summary>
        public DFSTNode Root
        {
            get
            {
                return ReversePostOrder[0];
            }
        }

        /// <summary>
        /// Gets a list of all tree nodes sorted in reverse post order.
        /// </summary>
        public List<DFSTNode> ReversePostOrder { get; private set; }

        /// <summary>
        /// Gets a set of all tree edges.
        /// </summary>
        public HashSet<DFSTEdge> TreeEdges { get; private set; }

        /// <summary>
        /// Gets a set of all forward edges found by the DFS.
        /// </summary>
        public HashSet<DFSTEdge> ForwardEdges { get; private set; }

        /// <summary>
        /// Gets a set of all back edges found by the DFS.
        /// </summary>
        public HashSet<DFSTEdge> BackEdges { get; private set; }

        /// <summary>
        /// Gets a set of all cross edges found by the DFS.
        /// </summary>
        public HashSet<DFSTEdge> CrossEdges { get; private set; }

        /// <summary>
        /// Gets a dictionary containing all constructs as keys and their corresponding nodes as values.
        /// </summary>
        public Dictionary<ISingleEntrySubGraph, DFSTNode> ConstructToNodeMap { get; private set; }

        public DFSTree(Dictionary<ISingleEntrySubGraph, DFSTNode> constructToNodeMap)
        {
            ReversePostOrder = new List<DFSTNode>();
            TreeEdges = new HashSet<DFSTEdge>();
            ForwardEdges = new HashSet<DFSTEdge>();
            BackEdges = new HashSet<DFSTEdge>();
            CrossEdges = new HashSet<DFSTEdge>();
            ConstructToNodeMap = constructToNodeMap;
        }

        /// <summary>
        /// Gets the path in the tree between two nodes. The <paramref name="ancestorNode"/>
        /// must be on the path from the root to the <paramref name="descenderNode"/>.
        /// </summary>
        /// <param name="ancestorNode"></param>
        /// <param name="descenderNode"></param>
        /// <returns></returns>
        public List<DFSTNode> GetPath(DFSTNode ancestorNode, DFSTNode descenderNode)
        {
            List<DFSTNode> path = new List<DFSTNode>();
            DFSTNode currentNodeOnPath = descenderNode;
            while (currentNodeOnPath != null && currentNodeOnPath != ancestorNode)
            {
                path.Add(currentNodeOnPath);
                currentNodeOnPath = (DFSTNode)currentNodeOnPath.Predecessor;
            }

            if (currentNodeOnPath != null)
            {
                path.Add(currentNodeOnPath);
            }
            else
            {
                //sanity check
                throw new Exception("No path between the two nodes.");
            }

            path.Reverse();
            return path;
        }
    }
}
