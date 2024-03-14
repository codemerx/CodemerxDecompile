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
			this.inferenceGraph = inferenceGraph;
			this.used = new Dictionary<ClassHierarchyNode, int>();
			this.s = new Stack<ClassHierarchyNode>();
			this.p = new Stack<ClassHierarchyNode>();
			this.nodeToComponent = new Dictionary<ClassHierarchyNode, int>();
			this.componentCount = 0;
			this.preorderNumber = 0;
		}

		public IEnumerable<ICollection<ClassHierarchyNode>> GetConnectedComponents()
		{
			ClassHierarchyNode classHierarchyNode;
			do
			{
				classHierarchyNode = null;
				foreach (ClassHierarchyNode classHierarchyNode1 in this.inferenceGraph)
				{
					if (this.used.ContainsKey(classHierarchyNode1))
					{
						continue;
					}
					classHierarchyNode = classHierarchyNode1;
					goto Label0;
				}
			Label0:
				if (classHierarchyNode == null)
				{
					continue;
				}
				this.RecursiveDfs(classHierarchyNode);
			}
			while (classHierarchyNode != null);
			ICollection<ClassHierarchyNode>[] classHierarchyNodes = new ICollection<ClassHierarchyNode>[this.componentCount];
			foreach (ClassHierarchyNode key in this.nodeToComponent.Keys)
			{
				int item = this.nodeToComponent[key];
				if (classHierarchyNodes[item] == null)
				{
					classHierarchyNodes[item] = new List<ClassHierarchyNode>();
				}
				((List<ClassHierarchyNode>)classHierarchyNodes[item]).Add(key);
			}
			return classHierarchyNodes;
		}

		private void RecursiveDfs(ClassHierarchyNode node)
		{
			this.preorderNumber++;
			this.used.Add(node, this.preorderNumber);
			this.s.Push(node);
			this.p.Push(node);
			foreach (ClassHierarchyNode canAssignTo in node.CanAssignTo)
			{
				if (this.used.ContainsKey(canAssignTo))
				{
					if (this.nodeToComponent.ContainsKey(canAssignTo))
					{
						continue;
					}
					int item = this.used[canAssignTo];
					while (item < this.used[this.p.Peek()])
					{
						this.p.Pop();
					}
				}
				else
				{
					this.RecursiveDfs(canAssignTo);
				}
			}
			if (this.p.Peek() == node)
			{
				while (this.s.Peek() != node)
				{
					ClassHierarchyNode classHierarchyNode = this.s.Pop();
					this.nodeToComponent.Add(classHierarchyNode, this.componentCount);
				}
				this.nodeToComponent.Add(this.p.Pop(), this.componentCount);
				this.s.Pop();
				this.componentCount++;
			}
		}
	}
}