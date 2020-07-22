using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Common
{
    class UnionFinder<T>
    {
        private readonly Dictionary<T, UnionNode> elementToNodeMap = new Dictionary<T, UnionNode>();

        private UnionNode GetNode(T element)
        {
            UnionNode node;
            if (!elementToNodeMap.TryGetValue(element, out node))
            {
                node = new UnionNode(element);
                elementToNodeMap.Add(element, node);
            }
            return node;
        }

        public void Union(T firstElement, T secondElement)
        {
            UnionNode firstNodeRep = FindNode(GetNode(firstElement));
            UnionNode secondNodeRep = FindNode(GetNode(secondElement));

            if (firstNodeRep == secondNodeRep)
            {
                return;
            }

            if (firstNodeRep.rank < secondNodeRep.rank)
            {
                firstNodeRep.parent = secondNodeRep;
            }
            else if (firstNodeRep.rank > secondNodeRep.rank)
            {
                secondNodeRep.parent = firstNodeRep;
            }
            else
            {
                secondNodeRep.parent = firstNodeRep;
                firstNodeRep.rank++;
            }
        }

        public T Find(T element)
        {
            UnionNode node;
            if (!elementToNodeMap.TryGetValue(element, out node))
            {
                return element;
            }

            return FindNode(node).element;
        }

        private UnionNode FindNode(UnionNode initialNode)
        {
            if (initialNode.parent == null)
            {
                return initialNode;
            }

            UnionNode currentNode = initialNode.parent;
            while (currentNode.parent != null)
            {
                currentNode = currentNode.parent;
            }

            UnionNode representative = currentNode;

            currentNode = initialNode;
            while (currentNode.parent != representative)
            {
                UnionNode temp = currentNode;
                currentNode = currentNode.parent;
                temp.parent = representative;
            }

            return representative;
        }

        private class UnionNode
        {
            public readonly T element;
            public UnionNode parent;
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
