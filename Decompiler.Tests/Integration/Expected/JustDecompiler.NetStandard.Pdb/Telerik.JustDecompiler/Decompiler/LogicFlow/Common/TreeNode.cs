using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Common
{
	internal class TreeNode
	{
		public ISingleEntrySubGraph Construct
		{
			get;
			private set;
		}

		public TreeNode Predecessor
		{
			get;
			set;
		}

		public HashSet<TreeNode> TreeEdgeSuccessors
		{
			get;
			private set;
		}

		protected TreeNode(ISingleEntrySubGraph construct)
		{
			base();
			this.set_Construct(construct);
			this.set_Predecessor(null);
			this.set_TreeEdgeSuccessors(new HashSet<TreeNode>());
			return;
		}
	}
}