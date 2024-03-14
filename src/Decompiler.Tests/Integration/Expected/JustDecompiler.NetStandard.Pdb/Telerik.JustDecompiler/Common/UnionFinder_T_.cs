using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Common
{
	internal class UnionFinder<T>
	{
		private readonly Dictionary<T, UnionFinder<T>.UnionNode> elementToNodeMap;

		public UnionFinder()
		{
		}

		public T Find(T element)
		{
			UnionFinder<T>.UnionNode unionNode;
			if (!this.elementToNodeMap.TryGetValue(element, out unionNode))
			{
				return element;
			}
			return this.FindNode(unionNode).element;
		}

		private UnionFinder<T>.UnionNode FindNode(UnionFinder<T>.UnionNode initialNode)
		{
			if (initialNode.parent == null)
			{
				return initialNode;
			}
			UnionFinder<T>.UnionNode unionNode = initialNode.parent;
			while (unionNode.parent != null)
			{
				unionNode = unionNode.parent;
			}
			UnionFinder<T>.UnionNode unionNode1 = unionNode;
			unionNode = initialNode;
			while (unionNode.parent != unionNode1)
			{
				UnionFinder<T>.UnionNode unionNode2 = unionNode;
				unionNode = unionNode.parent;
				unionNode2.parent = unionNode1;
			}
			return unionNode1;
		}

		private UnionFinder<T>.UnionNode GetNode(T element)
		{
			UnionFinder<T>.UnionNode unionNode;
			if (!this.elementToNodeMap.TryGetValue(element, out unionNode))
			{
				unionNode = new UnionFinder<T>.UnionNode(element);
				this.elementToNodeMap.Add(element, unionNode);
			}
			return unionNode;
		}

		public void Union(T firstElement, T secondElement)
		{
			UnionFinder<T>.UnionNode unionNode = this.FindNode(this.GetNode(firstElement));
			UnionFinder<T>.UnionNode unionNode1 = this.FindNode(this.GetNode(secondElement));
			if (unionNode == unionNode1)
			{
				return;
			}
			if (unionNode.rank < unionNode1.rank)
			{
				unionNode.parent = unionNode1;
				return;
			}
			if (unionNode.rank > unionNode1.rank)
			{
				unionNode1.parent = unionNode;
				return;
			}
			unionNode1.parent = unionNode;
			unionNode.rank++;
		}

		private class UnionNode
		{
			public readonly T element;

			public UnionFinder<T>.UnionNode parent;

			public int rank;

			public UnionNode(T element)
			{
				this.element = element;
				this.parent = null;
				this.rank = 0;
			}
		}
	}
}