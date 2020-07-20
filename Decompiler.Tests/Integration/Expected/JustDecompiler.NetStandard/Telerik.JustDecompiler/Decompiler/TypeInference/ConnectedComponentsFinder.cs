using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
	internal class ConnectedComponentsFinder
	{
		private int preorderNumber;

		private int componentCount;

		private readonly Dictionary<ClassHierarchyNode, int> used;

		private readonly Stack<ClassHierarchyNode> s;

		private readonly Stack<ClassHierarchyNode> p;

		private readonly Dictionary<ClassHierarchyNode, int> nodeToComponent;

		private readonly ICollection<ClassHierarchyNode> inferenceGraph;

		public ConnectedComponentsFinder(ICollection<ClassHierarchyNode> inferenceGraph)
		{
			base();
			this.inferenceGraph = inferenceGraph;
			this.used = new Dictionary<ClassHierarchyNode, int>();
			this.s = new Stack<ClassHierarchyNode>();
			this.p = new Stack<ClassHierarchyNode>();
			this.nodeToComponent = new Dictionary<ClassHierarchyNode, int>();
			this.componentCount = 0;
			this.preorderNumber = 0;
			return;
		}

		public IEnumerable<ICollection<ClassHierarchyNode>> GetConnectedComponents()
		{
			do
			{
				V_0 = null;
				V_2 = this.inferenceGraph.GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = V_2.get_Current();
						if (this.used.ContainsKey(V_3))
						{
							continue;
						}
						V_0 = V_3;
						goto Label0;
					}
				}
				finally
				{
					if (V_2 != null)
					{
						V_2.Dispose();
					}
				}
			Label0:
				if (V_0 == null)
				{
					continue;
				}
				this.RecursiveDfs(V_0);
			}
			while (V_0 != null);
			V_1 = new ICollection<ClassHierarchyNode>[this.componentCount];
			V_4 = this.nodeToComponent.get_Keys().GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_5 = V_4.get_Current();
					V_6 = this.nodeToComponent.get_Item(V_5);
					if (V_1[V_6] == null)
					{
						V_1[V_6] = new List<ClassHierarchyNode>();
					}
					((List<ClassHierarchyNode>)V_1[V_6]).Add(V_5);
				}
			}
			finally
			{
				((IDisposable)V_4).Dispose();
			}
			return V_1;
		}

		private void RecursiveDfs(ClassHierarchyNode node)
		{
			this.preorderNumber = this.preorderNumber + 1;
			this.used.Add(node, this.preorderNumber);
			this.s.Push(node);
			this.p.Push(node);
			V_0 = node.get_CanAssignTo().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (this.used.ContainsKey(V_1))
					{
						if (this.nodeToComponent.ContainsKey(V_1))
						{
							continue;
						}
						V_2 = this.used.get_Item(V_1);
						while (V_2 < this.used.get_Item(this.p.Peek()))
						{
							dummyVar0 = this.p.Pop();
						}
					}
					else
					{
						this.RecursiveDfs(V_1);
					}
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			if (this.p.Peek() == node)
			{
				while (this.s.Peek() != node)
				{
					V_3 = this.s.Pop();
					this.nodeToComponent.Add(V_3, this.componentCount);
				}
				this.nodeToComponent.Add(this.p.Pop(), this.componentCount);
				dummyVar1 = this.s.Pop();
				this.componentCount = this.componentCount + 1;
			}
			return;
		}
	}
}