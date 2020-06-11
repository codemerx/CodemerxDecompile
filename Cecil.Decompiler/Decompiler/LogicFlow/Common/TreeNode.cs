using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Common
{
    class TreeNode
    {
        /// <summary>
        /// Gets the subgraph construct that this node represents.
        /// </summary>
        public ISingleEntrySubGraph Construct { get; private set; }

        /// <summary>
        /// Gets or sets the tree predecessor of this node.
        /// </summary>
        public TreeNode Predecessor { get; set; }

        /// <summary>
        /// Gets a set of all tree successors of this node.
        /// </summary>
        public HashSet<TreeNode> TreeEdgeSuccessors { get; private set; }

        protected TreeNode(ISingleEntrySubGraph construct)
        {
            Construct = construct;
            Predecessor = null;
            TreeEdgeSuccessors = new HashSet<TreeNode>();
        }
    }
}
