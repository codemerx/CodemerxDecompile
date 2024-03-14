using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Cil
{
	public interface ISingleEntrySubGraph : IComparable<ISingleEntrySubGraph>
	{
		HashSet<ISingleEntrySubGraph> AllPredecessors
		{
			get;
		}

		HashSet<ISingleEntrySubGraph> AllSuccessors
		{
			get;
		}

		HashSet<ISingleEntrySubGraph> Children
		{
			get;
		}

		ISingleEntrySubGraph Entry
		{
			get;
			set;
		}

		int Index
		{
			get;
		}

		ISingleEntrySubGraph Parent
		{
			get;
			set;
		}

		HashSet<ISingleEntrySubGraph> SameParentPredecessors
		{
			get;
		}

		HashSet<ISingleEntrySubGraph> SameParentSuccessors
		{
			get;
		}
	}
}