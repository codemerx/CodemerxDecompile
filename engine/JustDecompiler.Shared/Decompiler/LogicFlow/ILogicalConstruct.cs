using Telerik.JustDecompiler.Cil;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	public interface ILogicalConstruct : ISingleEntrySubGraph
    {
        string ToString(LogicalFlowBuilderContext context);

        /// <summary>
        /// Gets the CFG logical construct that is control flow entry of this construct.
        /// </summary>
		CFGBlockLogicalConstruct FirstBlock { get; }

        /// <summary>
        /// Gets all CFG logical construct predecessors of this construct.
        /// </summary>
        HashSet<CFGBlockLogicalConstruct> CFGPredecessors { get; }

        /// <summary>
        /// Gets all CFG logical construct successors of this construct.
        /// </summary>
        HashSet<CFGBlockLogicalConstruct> CFGSuccessors { get; }

        /// <summary>
        /// Gets all CFG block logical constructs that are contained in this logical construct.
        /// </summary>
        HashSet<CFGBlockLogicalConstruct> CFGBlocks { get; }

        /// <summary>
        /// Gets the follow construct that has the same parent as this construct. The follow node will be the next sibling of this construct in the AST generated
        /// by the StatementDecompilerStep.
        /// </summary>
        ILogicalConstruct FollowNode { get; }

        /// <summary>
        /// Gets or sets the CFG block logical construct that is entry of the follow node.
        /// </summary>
        CFGBlockLogicalConstruct CFGFollowNode { get; set; }
    }
}
