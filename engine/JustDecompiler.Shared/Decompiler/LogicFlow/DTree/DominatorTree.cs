using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using System;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.DTree
{
    public class DominatorTree
    {
        private readonly Dictionary<ISingleEntrySubGraph, DTNode> constructToNodeMap;

        /// <summary>
        /// Gets the root subgraph construct of the dominator tree.
        /// </summary>
        public ISingleEntrySubGraph RootConstruct { get; private set; }

        internal DominatorTree(Dictionary<ISingleEntrySubGraph, DTNode> constructToNodeMap, ISingleEntrySubGraph rootConstruct)
        {
            this.constructToNodeMap = constructToNodeMap;
            RootConstruct = rootConstruct;
        }

        /// <summary>
        /// Gets the dominators of the specified subgraph construct.
        /// </summary>
        /// <param name="construct"></param>
        /// <returns></returns>
        public HashSet<ISingleEntrySubGraph> GetDominators(ISingleEntrySubGraph construct)
        {
            DTNode node;
            if (!constructToNodeMap.TryGetValue(construct, out node))
            {
                return null;
            }

            HashSet<ISingleEntrySubGraph> dominatorConstructs = new HashSet<ISingleEntrySubGraph>();

            foreach (DTNode dominator in node.Dominators)
            {
                dominatorConstructs.Add(dominator.Construct);
            }

            return dominatorConstructs;
        }

        /// <summary>
        /// Gets the immediate dominator of the specified subgraph construct.
        /// </summary>
        /// <param name="construct"></param>
        /// <returns></returns>
        public ISingleEntrySubGraph GetImmediateDominator(ISingleEntrySubGraph construct)
        {
            DTNode node;
            if (constructToNodeMap.TryGetValue(construct, out node) && node.Predecessor != null)
            {
                return node.Predecessor.Construct;
            }
            return null;
        }

        /// <summary>
        /// Gets the dominance frontier of the specified subgraph construct.
        /// </summary>
        /// <param name="construct"></param>
        /// <returns></returns>
        public HashSet<ISingleEntrySubGraph> GetDominanceFrontier(ISingleEntrySubGraph construct)
        {
            DTNode node;
            if (!constructToNodeMap.TryGetValue(construct, out node))
            {
                return null;
            }

            HashSet<ISingleEntrySubGraph> dominanceFrontier = new HashSet<ISingleEntrySubGraph>();
            foreach (DTNode frontierNode in node.DominanceFrontier)
            {
                dominanceFrontier.Add(frontierNode.Construct);
            }

            return dominanceFrontier;
        }

        /// <summary>
        /// Gets the nodes that are dominated by this subgraph construct.
        /// </summary>
        /// <param name="construct"></param>
        /// <returns></returns>
        public HashSet<ISingleEntrySubGraph> GetDominatedNodes(ISingleEntrySubGraph construct)
        {
            DTNode node;
            if(!constructToNodeMap.TryGetValue(construct, out node))
            {
                return null;
            }

            //A node dominates all of the constructs that are in the subtree (of the dominator tree) rooted at this node.
            //So we get them via simplified bfs (no need to check if a node is traversed, since it's a tree).
            HashSet<ISingleEntrySubGraph> dominatedNodes = new HashSet<ISingleEntrySubGraph>();
            Queue<DTNode> traversalQueue = new Queue<DTNode>();
            traversalQueue.Enqueue(node);

            while(traversalQueue.Count > 0)
            {
                DTNode currnetNode = traversalQueue.Dequeue();
                dominatedNodes.Add(currnetNode.Construct);

                foreach (DTNode successor in currnetNode.TreeEdgeSuccessors)
                {
                    traversalQueue.Enqueue(successor);
                }
            }

            return dominatedNodes;
        }

        public void MergeNodes(HashSet<ISingleEntrySubGraph> constructs, ISingleEntrySubGraph originalEntry, ISingleEntrySubGraph newConstruct)
        {
            DTNode oldNode = constructToNodeMap[originalEntry];
            DTNode newNode = new DTNode(newConstruct) { Predecessor = oldNode.Predecessor };

            newNode.DominanceFrontier.UnionWith(oldNode.DominanceFrontier);
            newNode.DominanceFrontier.Remove(oldNode);

            if (newNode.Predecessor != null)
            {
                newNode.Predecessor.TreeEdgeSuccessors.Remove(oldNode);
                newNode.Predecessor.TreeEdgeSuccessors.Add(newNode);
            }

            foreach (ISingleEntrySubGraph constructToRemove in constructs)
            {
                constructToNodeMap.Remove(constructToRemove);
            }

            foreach (KeyValuePair<ISingleEntrySubGraph, DTNode> pair in constructToNodeMap)
            {
                if (pair.Value.Predecessor != null && constructs.Contains(pair.Value.Predecessor.Construct))
                {
                    pair.Value.Predecessor = newNode;
                    newNode.TreeEdgeSuccessors.Add(pair.Value);
                }

                if (pair.Value.DominanceFrontier.Remove(oldNode))
                {
                    pair.Value.DominanceFrontier.Add(newNode);
                }
            }

            if (RootConstruct == originalEntry)
            {
                RootConstruct = newConstruct;
            }

            constructToNodeMap.Add(newConstruct, newNode);
        }
    }
}
