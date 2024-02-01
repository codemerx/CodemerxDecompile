using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Common;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.DFST
{
	internal class DFSTNode : TreeNode, IComparable<DFSTNode>
	{
		public HashSet<DFSTNode> BackEdgePredecessors
		{
			get;
			private set;
		}

		public HashSet<DFSTNode> BackEdgeSuccessors
		{
			get;
			private set;
		}

		public HashSet<DFSTNode> CrossEdgePredecessors
		{
			get;
			private set;
		}

		public HashSet<DFSTNode> CrossEdgeSuccessors
		{
			get;
			private set;
		}

		public HashSet<DFSTNode> ForwardEdgePredecessors
		{
			get;
			private set;
		}

		public HashSet<DFSTNode> ForwardEdgeSucessors
		{
			get;
			private set;
		}

		public int ReversePostOrderIndex
		{
			get;
			set;
		}

		public DFSTNode(ISingleEntrySubGraph construct) : base(construct)
		{
			this.BackEdgeSuccessors = new HashSet<DFSTNode>();
			this.ForwardEdgeSucessors = new HashSet<DFSTNode>();
			this.CrossEdgeSuccessors = new HashSet<DFSTNode>();
			this.BackEdgePredecessors = new HashSet<DFSTNode>();
			this.ForwardEdgePredecessors = new HashSet<DFSTNode>();
			this.CrossEdgePredecessors = new HashSet<DFSTNode>();
		}

		public int CompareTo(DFSTNode other)
		{
			return this.ReversePostOrderIndex.CompareTo(other.ReversePostOrderIndex);
		}
	}
}