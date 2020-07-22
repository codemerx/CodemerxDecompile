using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DFST;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.DTree
{
	internal class FastDominatorTreeBuilder : BaseDominatorTreeBuilder
	{
		private int[] dominators;

		private FastDominatorTreeBuilder(ISingleEntrySubGraph graph)
		{
			base(graph);
			return;
		}

		public static DominatorTree BuildTree(ISingleEntrySubGraph graph)
		{
			return BaseDominatorTreeBuilder.BuildTreeInternal(new FastDominatorTreeBuilder(graph));
		}

		protected override void FindImmediateDominators()
		{
			V_0 = DFSTBuilder.BuildTree(this.originalGraph).get_ReversePostOrder();
			V_1 = this.InitializePredecessors(V_0);
			V_2 = V_0.get_Count();
			this.InitializeDominators(V_2);
			do
			{
				V_3 = false;
				V_4 = new FastDominatorTreeBuilder.u003cu003ec__DisplayClass3_0();
				V_4.u003cu003e4__this = this;
				V_4.i = 1;
				while (V_4.i < V_2)
				{
					V_5 = V_1[V_4.i];
					V_6 = this.GetPredecessor(V_5, new Predicate<DFSTNode>(V_4.u003cFindImmediateDominatorsu003eb__0));
					V_7 = V_5.GetEnumerator();
					try
					{
						while (V_7.MoveNext())
						{
							V_8 = V_7.get_Current().get_ReversePostOrderIndex();
							if (V_8 == V_4.i || this.dominators[V_8] == -1)
							{
								continue;
							}
							V_6 = this.Intersect(V_8, V_6);
						}
					}
					finally
					{
						((IDisposable)V_7).Dispose();
					}
					if (this.dominators[V_4.i] != V_6)
					{
						this.dominators[V_4.i] = V_6;
						V_3 = true;
					}
					V_4.i = V_4.i + 1;
				}
			}
			while (V_3);
			V_10 = 1;
			while (V_10 < V_2)
			{
				V_11 = this.constructToNodeMap.get_Item(V_0.get_Item(V_10).get_Construct());
				V_12 = this.constructToNodeMap.get_Item(V_0.get_Item(this.dominators[V_10]).get_Construct());
				V_11.set_Predecessor(V_12);
				dummyVar0 = V_12.get_TreeEdgeSuccessors().Add(V_11);
				V_10 = V_10 + 1;
			}
			return;
		}

		private int GetPredecessor(List<DFSTNode> predecessors, Predicate<DFSTNode> predicate)
		{
			V_0 = predecessors.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!predicate.Invoke(V_1))
					{
						continue;
					}
					V_2 = V_1.get_ReversePostOrderIndex();
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
		Label1:
			return V_2;
		Label0:
			throw new Exception("No such element");
		}

		private List<DFSTNode> GetPredecessors(DFSTNode node)
		{
			V_0 = new List<DFSTNode>();
			if (node.get_Predecessor() != null)
			{
				V_0.Add(node.get_Predecessor() as DFSTNode);
			}
			V_0.AddRange(node.get_ForwardEdgePredecessors());
			V_0.AddRange(node.get_CrossEdgePredecessors());
			V_0.AddRange(node.get_BackEdgePredecessors());
			return V_0;
		}

		private void InitializeDominators(int count)
		{
			this.dominators = new Int32[count];
			this.dominators[0] = 0;
			V_0 = 1;
			while (V_0 < count)
			{
				this.dominators[V_0] = -1;
				V_0 = V_0 + 1;
			}
			return;
		}

		private List<DFSTNode>[] InitializePredecessors(List<DFSTNode> nodes)
		{
			V_0 = new List<DFSTNode>[nodes.get_Count()];
			V_1 = 0;
			while (V_1 < nodes.get_Count())
			{
				V_0[V_1] = this.GetPredecessors(nodes.get_Item(V_1));
				V_1 = V_1 + 1;
			}
			return V_0;
		}

		private int Intersect(int first, int second)
		{
			while (first != second)
			{
				while (first > second)
				{
					first = this.dominators[first];
				}
				while (second > first)
				{
					second = this.dominators[second];
				}
			}
			return first;
		}
	}
}