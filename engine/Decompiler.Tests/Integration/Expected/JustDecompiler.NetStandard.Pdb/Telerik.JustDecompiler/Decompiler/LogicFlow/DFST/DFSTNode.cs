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

		public DFSTNode(ISingleEntrySubGraph construct)
		{
			base(construct);
			this.set_BackEdgeSuccessors(new HashSet<DFSTNode>());
			this.set_ForwardEdgeSucessors(new HashSet<DFSTNode>());
			this.set_CrossEdgeSuccessors(new HashSet<DFSTNode>());
			this.set_BackEdgePredecessors(new HashSet<DFSTNode>());
			this.set_ForwardEdgePredecessors(new HashSet<DFSTNode>());
			this.set_CrossEdgePredecessors(new HashSet<DFSTNode>());
			return;
		}

		public int CompareTo(DFSTNode other)
		{
			V_0 = this.get_ReversePostOrderIndex();
			return V_0.CompareTo(other.get_ReversePostOrderIndex());
		}
	}
}