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
				V_0 = new HashSet<DTNode>();
				V_1 = this;
				while (V_1 != null)
				{
					dummyVar0 = V_0.Add(V_1);
					V_1 = V_1.get_Predecessor() as DTNode;
				}
				return V_0;
			}
		}

		public DTNode ImmediateDominator
		{
			get
			{
				return this.get_Predecessor() as DTNode;
			}
		}

		public DTNode(ISingleEntrySubGraph construct)
		{
			base(construct);
			this.set_DominanceFrontier(new HashSet<DTNode>());
			return;
		}
	}
}