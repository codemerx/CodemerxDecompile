using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DFST;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.DTree
{
    //A Simple, Fast Dominance Algorithm (dom.pdf)
    class FastDominatorTreeBuilder : BaseDominatorTreeBuilder
    {
        private int[] dominators;

        private FastDominatorTreeBuilder(ISingleEntrySubGraph graph)
            :base(graph)
        {
        }

        /// <summary>
        /// Builds a dominator tree for the specified graph, with its entry as root.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
		public static DominatorTree BuildTree(ISingleEntrySubGraph graph)
		{
			return BuildTreeInternal(new FastDominatorTreeBuilder(graph));
		}

        protected override void FindImmediateDominators()
        {
            List<DFSTNode> reversePostOrderMap = DFSTBuilder.BuildTree(originalGraph).ReversePostOrder;
            List<DFSTNode>[] predecessorsCache = InitializePredecessors(reversePostOrderMap);
            int count = reversePostOrderMap.Count;
            InitializeDominators(count);

            bool changed;
            do
            {
                changed = false;
                for (int i = 1; i < count; i++)
                {
                    List<DFSTNode> predecessors = predecessorsCache[i];
                    int newImmDom = GetPredecessor(predecessors, node => node.ReversePostOrderIndex != i && dominators[node.ReversePostOrderIndex] != -1);
                    foreach (DFSTNode predecessor in predecessors)
                    {
                        int index = predecessor.ReversePostOrderIndex;
                        if (index != i && dominators[index] != -1)
                        {
                            newImmDom = Intersect(index, newImmDom);
                        }
                    }

                    if (dominators[i] != newImmDom)
                    {
                        dominators[i] = newImmDom;
                        changed = true;
                    }
                }
            }
            while (changed);

            for (int i = 1; i < count; i++)
            {
                DTNode node = constructToNodeMap[reversePostOrderMap[i].Construct];
                DTNode immDom = constructToNodeMap[reversePostOrderMap[dominators[i]].Construct];
                node.Predecessor = immDom;
                immDom.TreeEdgeSuccessors.Add(node);
            }
        }

        private List<DFSTNode>[] InitializePredecessors(List<DFSTNode> nodes)
        {
            List<DFSTNode>[] result = new List<DFSTNode>[nodes.Count];
            for(int i = 0; i < nodes.Count; i++)
            {
                result[i] = GetPredecessors(nodes[i]);
            }
            return result;
        }

        private void InitializeDominators(int count)
        {
            dominators = new int[count];

            dominators[0] = 0;
            for (int i = 1; i < count; i++)
            {
                dominators[i] = -1;
            }
        }

        private int GetPredecessor(List<DFSTNode> predecessors, Predicate<DFSTNode> predicate)
        {
            foreach (DFSTNode predecessor in predecessors)
            {
                if (predicate(predecessor))
                {
                    return predecessor.ReversePostOrderIndex;
                }
            }

            throw new Exception("No such element");
        }

        private List<DFSTNode> GetPredecessors(DFSTNode node)
        {
            List<DFSTNode> result = new List<DFSTNode>();
            if (node.Predecessor != null)
            {
                result.Add(node.Predecessor as DFSTNode);
            }
            result.AddRange(node.ForwardEdgePredecessors);
            result.AddRange(node.CrossEdgePredecessors);
            result.AddRange(node.BackEdgePredecessors);
            return result;
        }

        private int Intersect(int first, int second)
        {
            while (first != second)
            {
                while (first > second)
                {
                    first = dominators[first];
                }
                while (second > first)
                {
                    second = dominators[second];
                }
            }

            return first;
        }
    }
}
