using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.DTree
{
	public class DominatorTree
	{
		private readonly Dictionary<ISingleEntrySubGraph, DTNode> constructToNodeMap;

		public ISingleEntrySubGraph RootConstruct
		{
			get;
			private set;
		}

		internal DominatorTree(Dictionary<ISingleEntrySubGraph, DTNode> constructToNodeMap, ISingleEntrySubGraph rootConstruct)
		{
			base();
			this.constructToNodeMap = constructToNodeMap;
			this.set_RootConstruct(rootConstruct);
			return;
		}

		public HashSet<ISingleEntrySubGraph> GetDominanceFrontier(ISingleEntrySubGraph construct)
		{
			if (!this.constructToNodeMap.TryGetValue(construct, out V_0))
			{
				return null;
			}
			V_1 = new HashSet<ISingleEntrySubGraph>();
			V_2 = V_0.get_DominanceFrontier().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					dummyVar0 = V_1.Add(V_3.get_Construct());
				}
			}
			finally
			{
				((IDisposable)V_2).Dispose();
			}
			return V_1;
		}

		public HashSet<ISingleEntrySubGraph> GetDominatedNodes(ISingleEntrySubGraph construct)
		{
			if (!this.constructToNodeMap.TryGetValue(construct, out V_0))
			{
				return null;
			}
			V_1 = new HashSet<ISingleEntrySubGraph>();
			V_2 = new Queue<DTNode>();
			V_2.Enqueue(V_0);
			while (V_2.get_Count() > 0)
			{
				V_3 = V_2.Dequeue();
				dummyVar0 = V_1.Add(V_3.get_Construct());
				V_4 = V_3.get_TreeEdgeSuccessors().GetEnumerator();
				try
				{
					while (V_4.MoveNext())
					{
						V_5 = (DTNode)V_4.get_Current();
						V_2.Enqueue(V_5);
					}
				}
				finally
				{
					((IDisposable)V_4).Dispose();
				}
			}
			return V_1;
		}

		public HashSet<ISingleEntrySubGraph> GetDominators(ISingleEntrySubGraph construct)
		{
			if (!this.constructToNodeMap.TryGetValue(construct, out V_0))
			{
				return null;
			}
			V_1 = new HashSet<ISingleEntrySubGraph>();
			V_2 = V_0.get_Dominators().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					dummyVar0 = V_1.Add(V_3.get_Construct());
				}
			}
			finally
			{
				((IDisposable)V_2).Dispose();
			}
			return V_1;
		}

		public ISingleEntrySubGraph GetImmediateDominator(ISingleEntrySubGraph construct)
		{
			if (!this.constructToNodeMap.TryGetValue(construct, out V_0) || V_0.get_Predecessor() == null)
			{
				return null;
			}
			return V_0.get_Predecessor().get_Construct();
		}

		public void MergeNodes(HashSet<ISingleEntrySubGraph> constructs, ISingleEntrySubGraph originalEntry, ISingleEntrySubGraph newConstruct)
		{
			V_0 = this.constructToNodeMap.get_Item(originalEntry);
			stackVariable5 = new DTNode(newConstruct);
			stackVariable5.set_Predecessor(V_0.get_Predecessor());
			V_1 = stackVariable5;
			V_1.get_DominanceFrontier().UnionWith(V_0.get_DominanceFrontier());
			dummyVar0 = V_1.get_DominanceFrontier().Remove(V_0);
			if (V_1.get_Predecessor() != null)
			{
				dummyVar1 = V_1.get_Predecessor().get_TreeEdgeSuccessors().Remove(V_0);
				dummyVar2 = V_1.get_Predecessor().get_TreeEdgeSuccessors().Add(V_1);
			}
			V_2 = constructs.GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					dummyVar3 = this.constructToNodeMap.Remove(V_3);
				}
			}
			finally
			{
				((IDisposable)V_2).Dispose();
			}
			V_4 = this.constructToNodeMap.GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_5 = V_4.get_Current();
					if (V_5.get_Value().get_Predecessor() != null && constructs.Contains(V_5.get_Value().get_Predecessor().get_Construct()))
					{
						V_5.get_Value().set_Predecessor(V_1);
						dummyVar4 = V_1.get_TreeEdgeSuccessors().Add(V_5.get_Value());
					}
					if (!V_5.get_Value().get_DominanceFrontier().Remove(V_0))
					{
						continue;
					}
					dummyVar5 = V_5.get_Value().get_DominanceFrontier().Add(V_1);
				}
			}
			finally
			{
				((IDisposable)V_4).Dispose();
			}
			if (this.get_RootConstruct() == originalEntry)
			{
				this.set_RootConstruct(newConstruct);
			}
			this.constructToNodeMap.Add(newConstruct, V_1);
			return;
		}
	}
}