using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Common;

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
				return this.ReversePostOrder[0];
			}
		}

		public HashSet<DFSTEdge> TreeEdges
		{
			get;
			private set;
		}

		public DFSTree(Dictionary<ISingleEntrySubGraph, DFSTNode> constructToNodeMap)
		{
			this.ReversePostOrder = new List<DFSTNode>();
			this.TreeEdges = new HashSet<DFSTEdge>();
			this.ForwardEdges = new HashSet<DFSTEdge>();
			this.BackEdges = new HashSet<DFSTEdge>();
			this.CrossEdges = new HashSet<DFSTEdge>();
			this.ConstructToNodeMap = constructToNodeMap;
		}

		public List<DFSTNode> GetPath(DFSTNode ancestorNode, DFSTNode descenderNode)
		{
			DFSTNode i;
			List<DFSTNode> dFSTNodes = new List<DFSTNode>();
			for (i = descenderNode; i != null && i != ancestorNode; i = (DFSTNode)i.Predecessor)
			{
				dFSTNodes.Add(i);
			}
			if (i == null)
			{
				throw new Exception("No path between the two nodes.");
			}
			dFSTNodes.Add(i);
			dFSTNodes.Reverse();
			return dFSTNodes;
		}
	}
}