using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DFST;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.DTree
{
    abstract class BaseDominatorTreeBuilder
    {
        protected readonly ISingleEntrySubGraph originalGraph;
        protected readonly Dictionary<ISingleEntrySubGraph, DTNode> constructToNodeMap = new Dictionary<ISingleEntrySubGraph, DTNode>();
        protected readonly ISingleEntrySubGraph rootConstruct;
        protected readonly Dictionary<DTNode, HashSet<DTNode>> predecessorMap = new Dictionary<DTNode, HashSet<DTNode>>();

        protected BaseDominatorTreeBuilder(ISingleEntrySubGraph graph)
        {
            this.originalGraph = graph;
            this.rootConstruct = graph.Entry;
        }

        protected static DominatorTree BuildTreeInternal(BaseDominatorTreeBuilder theBuilder)
        {
            theBuilder.MapNodes();
            theBuilder.MapPredecessors();
            theBuilder.FindImmediateDominators();
            theBuilder.ComputeDominanceFrontiers();
            return new DominatorTree(theBuilder.constructToNodeMap, theBuilder.rootConstruct);
        }

        /// <summary>
        /// Map each node of the subgraph to a new DTNode construct
        /// </summary>
        private void MapNodes()
        {
            foreach (ISingleEntrySubGraph child in originalGraph.Children)
            {
                constructToNodeMap.Add(child, new DTNode(child));
            }

            if (!constructToNodeMap.ContainsKey(rootConstruct))
            {
                throw new ArgumentException("The Graph does not contain the given start node");
            }
        }

        /// <summary>
        /// Since the algorithm works with the predecessors we make the relations between the DTNodes in this method.
        /// </summary>
        private void MapPredecessors()
        {
            foreach (ISingleEntrySubGraph child in originalGraph.Children)
            {
                //If the current child is the rootConstruct we leave it without predecessors, otherwise the algorithm will break
                if (child == rootConstruct)
                {
                    predecessorMap[constructToNodeMap[child]] = new HashSet<DTNode>();
                    continue;
                }

                HashSet<ISingleEntrySubGraph> desiredPredecessorSet = new HashSet<ISingleEntrySubGraph>();
                desiredPredecessorSet.UnionWith(child.SameParentPredecessors);

                //We make the relations between the DTNodes
                HashSet<DTNode> desiredDTPredecessors = new HashSet<DTNode>();
                foreach (ISingleEntrySubGraph desiredPredecessor in desiredPredecessorSet)
                {
                    DTNode dtPredecessor;
                    if (constructToNodeMap.TryGetValue(desiredPredecessor, out dtPredecessor))
                    {
                        desiredDTPredecessors.Add(dtPredecessor);
                    }
                    else
                    {
                        //sanity check
                        throw new ArgumentException("The desired predecessor is not child of the same subgraph");
                    }
                }

                predecessorMap[constructToNodeMap[child]] = desiredDTPredecessors;
            }

            //From this point on we work only with the created mappings
        }

        protected abstract void FindImmediateDominators();

        /// <summary>
        /// Computes the dominance frontier of each node.
        /// </summary>
        private void ComputeDominanceFrontiers()
        {
            //A dominance frontier of a specified node is the set of all nodes that are not dominated by the specified node,
            //but have a predecessor dominated by it. (If A dominates B which is a predecessor of C and A does not dominate C, then C is in the dominance
            //frontier of A.)

            //For each real predecessor of a specified node, we find the path from this predecessor to the immediate dominator of the specified node, and
            //we add the specified node to the dominance frontiers of all these nodes except to the dominance frontier of the immediate dominator.

            //e.g. If A is immediate dominator of B and D, B is immediate dominator of C and C is predecessor of D, then D is in the dominance frontier of C,
            //because C dominates C and D is not dominated by C (because it's not in its subtree in the dominance tree). D is also in the dominance frontier of B,
            //because B dominates C which is a predecessor of D.

            foreach (DTNode node in constructToNodeMap.Values)
            {
                HashSet<DTNode> realPredecessors = predecessorMap[node];

                foreach (DTNode predecessor in realPredecessors)
                {
                    DTNode currentDominator = predecessor;
                    while (currentDominator != node.ImmediateDominator)
                    {
                        currentDominator.DominanceFrontier.Add(node);
                        currentDominator = currentDominator.ImmediateDominator;
                    }
                }
            }
        }
    }
}
