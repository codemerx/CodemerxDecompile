using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.DFST
{
	internal class DFSTree
	{
		public HashSet<DFSTEdge> BackEdges
		{
			get;
			private set;
		}

		public Dictionary<ISingleEntrySubGraph, DFSTNode> ConstructToNodeMap
		{
			get;
			private set;
		}

		public HashSet<DFSTEdge> CrossEdges
		{
			get;
			private set;
		}

		public HashSet<DFSTEdge> ForwardEdges
		{
			get;
			private set;
		}

		public List<DFSTNode> ReversePostOrder
		{
			get;
			private set;
		}

		public DFSTNode Root
		{
			get
			{
				return this.get_ReversePostOrder().get_Item(0);
			}
		}

		public HashSet<DFSTEdge> TreeEdges
		{
			get;
			private set;
		}

		public DFSTree(Dictionary<ISingleEntrySubGraph, DFSTNode> constructToNodeMap)
		{
			base();
			this.set_ReversePostOrder(new List<DFSTNode>());
			this.set_TreeEdges(new HashSet<DFSTEdge>());
			this.set_ForwardEdges(new HashSet<DFSTEdge>());
			this.set_BackEdges(new HashSet<DFSTEdge>());
			this.set_CrossEdges(new HashSet<DFSTEdge>());
			this.set_ConstructToNodeMap(constructToNodeMap);
			return;
		}

		public List<DFSTNode> GetPath(DFSTNode ancestorNode, DFSTNode descenderNode)
		{
			V_0 = new List<DFSTNode>();
			V_1 = descenderNode;
			while (V_1 != null && V_1 != ancestorNode)
			{
				V_0.Add(V_1);
				V_1 = (DFSTNode)V_1.get_Predecessor();
			}
			if (V_1 == null)
			{
				throw new Exception("No path between the two nodes.");
			}
			V_0.Add(V_1);
			V_0.Reverse();
			return V_0;
		}
	}
}