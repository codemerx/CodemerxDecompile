using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Common;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.DTree
{
	internal class DTNode : TreeNode
	{
		public HashSet<DTNode> DominanceFrontier
		{
			get;
			private set;
		}

		public HashSet<DTNode> Dominators
		{
			get
			{
				HashSet<DTNode> dTNodes = new HashSet<DTNode>();
				for (DTNode i = this; i != null; i = i.Predecessor as DTNode)
				{
					dTNodes.Add(i);
				}
				return dTNodes;
			}
		}

		public DTNode ImmediateDominator
		{
			get
			{
				return base.Predecessor as DTNode;
			}
		}

		public DTNode(ISingleEntrySubGraph construct) : base(construct)
		{
			this.DominanceFrontier = new HashSet<DTNode>();
		}
	}
}