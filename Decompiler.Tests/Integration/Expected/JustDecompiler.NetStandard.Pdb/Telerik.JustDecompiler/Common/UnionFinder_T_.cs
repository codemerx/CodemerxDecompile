using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Common
{
	internal class UnionFinder<T>
	{
		private readonly Dictionary<T, UnionFinder<T>.UnionNode> elementToNodeMap;

		public UnionFinder()
		{
			this.elementToNodeMap = new Dictionary<T, UnionFinder<T>.UnionNode>();
			base();
			return;
		}

		public T Find(T element)
		{
			if (!this.elementToNodeMap.TryGetValue(element, out V_0))
			{
				return element;
			}
			return this.FindNode(V_0).element;
		}

		private UnionFinder<T>.UnionNode FindNode(UnionFinder<T>.UnionNode initialNode)
		{
			if (initialNode.parent == null)
			{
				return initialNode;
			}
			V_0 = initialNode.parent;
			while (V_0.parent != null)
			{
				V_0 = V_0.parent;
			}
			V_1 = V_0;
			V_0 = initialNode;
			while (V_0.parent != V_1)
			{
				V_2 = V_0;
				V_0 = V_0.parent;
				V_2.parent = V_1;
			}
			return V_1;
		}

		private UnionFinder<T>.UnionNode GetNode(T element)
		{
			if (!this.elementToNodeMap.TryGetValue(element, out V_0))
			{
				V_0 = new UnionFinder<T>.UnionNode(element);
				this.elementToNodeMap.Add(element, V_0);
			}
			return V_0;
		}

		public void Union(T firstElement, T secondElement)
		{
			V_0 = this.FindNode(this.GetNode(firstElement));
			V_1 = this.FindNode(this.GetNode(secondElement));
			if (V_0 == V_1)
			{
				return;
			}
			if (V_0.rank < V_1.rank)
			{
				V_0.parent = V_1;
				return;
			}
			if (V_0.rank > V_1.rank)
			{
				V_1.parent = V_0;
				return;
			}
			V_1.parent = V_0;
			stackVariable22 = V_0;
			stackVariable22.rank = stackVariable22.rank + 1;
			return;
		}

		private class UnionNode
		{
			public readonly T element;

			public UnionFinder<T>.UnionNode parent;

			public int rank;

			public UnionNode(T element)
			{
				base();
				this.element = element;
				this.parent = null;
				this.rank = 0;
				return;
			}
		}
	}
}