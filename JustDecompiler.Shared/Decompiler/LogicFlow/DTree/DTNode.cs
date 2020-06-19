using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Common;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.DTree
{
    class DTNode : TreeNode
    {
        /// <summary>
        /// Gets a set of all the nodes that dominate this node.
        /// </summary>
        public HashSet<DTNode> Dominators
        {
            get
            {
                HashSet<DTNode> dominators = new HashSet<DTNode>();
                DTNode currentNode = this;
                while(currentNode != null)
                {
                    dominators.Add(currentNode);
                    currentNode = currentNode.Predecessor as DTNode;
                }
                return dominators;
            }
        }

        /// <summary>
        /// Gets the set of all the nodes that are in the domination frontier of this node.
        /// </summary>
        public HashSet<DTNode> DominanceFrontier { get; private set; }

        /// <summary>
        /// Gets the immediate dominator of this node.
        /// </summary>
        public DTNode ImmediateDominator
        {
            get
            {
                return Predecessor as DTNode;
            }
        }

        public DTNode(ISingleEntrySubGraph construct)
            : base(construct)
        {
            DominanceFrontier = new HashSet<DTNode>();
        }
    }
}
