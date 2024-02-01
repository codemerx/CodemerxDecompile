using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	internal class IntervalConstruct : ISingleEntrySubGraph, IComparable<ISingleEntrySubGraph>
	{
		private readonly HashSet<ISingleEntrySubGraph> predecesors;

		private readonly HashSet<ISingleEntrySubGraph> successors;

		private readonly HashSet<ISingleEntrySubGraph> children;

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

		public HashSet<ISingleEntrySubGraph> Children
		{
			get
			{
				return this.children;
			}
		}

		public ISingleEntrySubGraph Entry
		{
			get;
			set;
		}

		public int Index
		{
			get
			{
				return this.Entry.Index;
			}
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

		public HashSet<ISingleEntrySubGraph> SameParentPredecessors
		{
			get
			{
				return this.predecesors;
			}
		}

		public HashSet<ISingleEntrySubGraph> SameParentSuccessors
		{
			get
			{
				return this.successors;
			}
		}

		public IntervalConstruct(ISingleEntrySubGraph entry)
		{
			this.Entry = entry;
			this.predecesors = new HashSet<ISingleEntrySubGraph>();
			this.successors = new HashSet<ISingleEntrySubGraph>();
			this.children = new HashSet<ISingleEntrySubGraph>();
			this.children.Add(entry);
		}

		public int CompareTo(ISingleEntrySubGraph other)
		{
			return this.Index.CompareTo(other.Index);
		}

		public bool ContainsBlock(InstructionBlock block)
		{
			throw new NotSupportedException();
		}
	}
}