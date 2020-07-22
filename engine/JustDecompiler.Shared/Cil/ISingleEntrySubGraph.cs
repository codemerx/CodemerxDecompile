using System.Collections.Generic;
using System;

namespace Telerik.JustDecompiler.Cil
{
	public interface ISingleEntrySubGraph : IComparable<ISingleEntrySubGraph>
	{
        /// <summary>
        /// Gets or sets the entry child of the subgraph.
        /// </summary>
        ISingleEntrySubGraph Entry { get; set; }

        /// <summary>
        /// Gets the subgraph predecessors that have the same parent as this subgraph.
        /// </summary>
        HashSet<ISingleEntrySubGraph> SameParentPredecessors { get; }

        /// <summary>
        /// Gets the collection of the children of the subgraph.
        /// </summary>
        HashSet<ISingleEntrySubGraph> Children { get; }

        /// <summary>
        /// Gets the subgraph successors that have the same parent as this subgraph.
        /// </summary>
        HashSet<ISingleEntrySubGraph> SameParentSuccessors { get; }

        /// <summary>
        /// Gets or sets the parent of the current subgraph.
        /// </summary>
        ISingleEntrySubGraph Parent { get; set; }

        /// <summary>
        /// Gets the index of the subgraph. Used for total ordering when traversing the subgraphs.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Gets the outermost subgraphs that are predecessors of the subgraph.
        /// </summary>
        HashSet<ISingleEntrySubGraph> AllPredecessors { get; }

        /// <summary>
        /// Gets the outermost subgraphs that are successors of the subgraph.
        /// </summary>
        HashSet<ISingleEntrySubGraph> AllSuccessors { get; }
    }
}