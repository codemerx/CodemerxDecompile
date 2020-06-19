using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Common;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.DFST
{
    internal class DFSTNode : TreeNode, IComparable<DFSTNode>
    {
        /// <summary>
        /// Gets a set of the end points of all back edges going out of this node.
        /// </summary>
        public HashSet<DFSTNode> BackEdgeSuccessors { get; private set; }

        /// <summary>
        /// Gets a set of the end points of all forward edges going out of this node.
        /// </summary>
        public HashSet<DFSTNode> ForwardEdgeSucessors { get; private set; }

        /// <summary>
        /// Gets a set of the end points of all cross edges going out of this node.
        /// </summary>
        public HashSet<DFSTNode> CrossEdgeSuccessors { get; private set; }

        /// <summary>
        /// Gets a set of the start points of all back edges going in this node.
        /// </summary>
        public HashSet<DFSTNode> BackEdgePredecessors { get; private set; }

        /// <summary>
        /// Gets a set of the start points of all forward edges going in this node.
        /// </summary>
        public HashSet<DFSTNode> ForwardEdgePredecessors { get; private set; }

        /// <summary>
        /// Gets a set of the start points of all cross edges going in this node.
        /// </summary>
        public HashSet<DFSTNode> CrossEdgePredecessors { get; private set; }

        /// <summary>
        /// Gets the index of this node in the reverse post ordering.
        /// </summary>
        public int ReversePostOrderIndex { get; set; }

        public DFSTNode(ISingleEntrySubGraph construct)
            : base(construct)
        {
            BackEdgeSuccessors = new HashSet<DFSTNode>();
            ForwardEdgeSucessors = new HashSet<DFSTNode>();
            CrossEdgeSuccessors = new HashSet<DFSTNode>();
            BackEdgePredecessors = new HashSet<DFSTNode>();
            ForwardEdgePredecessors = new HashSet<DFSTNode>();
            CrossEdgePredecessors = new HashSet<DFSTNode>();
        }

        public int CompareTo(DFSTNode other)
        {
            return this.ReversePostOrderIndex.CompareTo(other.ReversePostOrderIndex);
        }

    }
}
