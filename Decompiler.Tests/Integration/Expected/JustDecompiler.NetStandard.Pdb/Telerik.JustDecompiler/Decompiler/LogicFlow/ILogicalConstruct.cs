using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	public interface ILogicalConstruct : ISingleEntrySubGraph, IComparable<ISingleEntrySubGraph>
	{
		HashSet<CFGBlockLogicalConstruct> CFGBlocks
		{
			get;
		}

		CFGBlockLogicalConstruct CFGFollowNode
		{
			get;
			set;
		}

		HashSet<CFGBlockLogicalConstruct> CFGPredecessors
		{
			get;
		}

		HashSet<CFGBlockLogicalConstruct> CFGSuccessors
		{
			get;
		}

		CFGBlockLogicalConstruct FirstBlock
		{
			get;
		}

		ILogicalConstruct FollowNode
		{
			get;
		}

		string ToString(LogicalFlowBuilderContext context);
	}
}