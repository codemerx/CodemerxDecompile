using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
    class IntervalConstruct : ISingleEntrySubGraph
    {
        private readonly HashSet<ISingleEntrySubGraph> predecesors;
        private readonly HashSet<ISingleEntrySubGraph> successors;
        private readonly HashSet<ISingleEntrySubGraph> children;

        public IntervalConstruct(ISingleEntrySubGraph entry)
        {
            this.Entry = entry;
            predecesors = new HashSet<ISingleEntrySubGraph>();
            successors = new HashSet<ISingleEntrySubGraph>();
            children = new HashSet<ISingleEntrySubGraph>();
            children.Add(entry);
        }

        public bool ContainsBlock(InstructionBlock block)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the index of the interval. Used for total ordering when traversing the intervals.
        /// </summary>
        public int Index
        {
            get
            {
                return Entry.Index;
            }
        }

        /// <summary>
        /// The header of the interval.
        /// </summary>
        public ISingleEntrySubGraph Entry { get; set; }

        /*The intervals that preceed this one in the new graph*/
        public HashSet<ISingleEntrySubGraph> SameParentPredecessors
        {
            get { return predecesors; }
        }

        /*The intervals that are preceded by this one in the new graph*/
        public HashSet<ISingleEntrySubGraph> SameParentSuccessors
        {
            get { return successors; }
        }

        /*The logical constructs that fall within the interval*/
        public HashSet<ISingleEntrySubGraph> Children
        {
            get { return children; }
        }

        public ISingleEntrySubGraph Parent
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public HashSet<ISingleEntrySubGraph> AllPredecessors
        {

            get
            {
                throw new NotSupportedException();
            }
        }

        public HashSet<ISingleEntrySubGraph> AllSuccessors
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public int CompareTo(ISingleEntrySubGraph other)
        {
            return this.Index.CompareTo(other.Index);
        }
    }
}
