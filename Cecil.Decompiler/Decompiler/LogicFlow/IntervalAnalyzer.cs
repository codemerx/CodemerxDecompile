using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DFST;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
    /// <summary>
    /// Realises the algorithm described in Chapter "6.3.3 Interval Theory" of <see cref="Reverse Compilation Techniques.pdf"/>.
    /// </summary>
    class IntervalAnalyzer
    {
        private readonly HashSet<ISingleEntrySubGraph> availableNodes;
        private readonly ILogicalConstruct entryPoint;
        private readonly Queue<ILogicalConstruct> headers;
        private readonly List<IntervalConstruct> intervals;
        private readonly Dictionary<ISingleEntrySubGraph, IntervalConstruct> nodeToInterval;
        private readonly Dictionary<ILogicalConstruct, HashSet<ILogicalConstruct>> removedEdges;

        /// <param name="graph">The graph to be analyzed by the interval builder.</param>
        /// <param name="removedEdges">The edges in the Control flow graph, that have been marked as goto-edges.</param>
        public IntervalAnalyzer(ISingleEntrySubGraph graph, Dictionary<ILogicalConstruct, HashSet<ILogicalConstruct>> removedEdges)
        {
            this.availableNodes = graph.Children;
            this.entryPoint = graph.Entry as ILogicalConstruct;
            this.headers = new Queue<ILogicalConstruct>();
            this.intervals = new List<IntervalConstruct>();
            this.nodeToInterval = new Dictionary<ISingleEntrySubGraph, IntervalConstruct>();
            this.removedEdges = removedEdges;
        }

        /// <summary>
        /// The entry point of the class. Performs one reducing step on the graph.
        /// </summary>
        /// <returns>Returns list of all the intervals.</returns>
        public List<IntervalConstruct> ReduceCfg()
        {
            headers.Enqueue(entryPoint);

            while (headers.Count > 0)
            {
                /// Construct new interval from the header.
                ILogicalConstruct currentHeader = headers.Dequeue();
                IntervalConstruct currentInterval = new IntervalConstruct(currentHeader);
                intervals.Add(currentInterval);

                /// Fill the newly constructed interval with the nodes that belong to it.
                FillInterval(currentHeader, currentInterval);

                /// Add the headers to the remaining intervals in the graph to 'headers' collection
                AddNewHeaders(currentHeader, currentInterval);
            }

            /// Update the relations between the intervals, based on the nodes they contain.
            /// Doesn't destroy the relations between the nested nodes themselves.
            CreateGraph(intervals);

            return SortIntervalList(intervals);
        }

        /// <summary>
        /// Fills <paramref name="interval"/> with the nodes, that belong to it.
        /// </summary>
        /// <param name="intervalHeader">The header node of <paramref name="interval"/>.</param>
        /// <param name="interval">The interval to be filled with nodes.</param>
        private void FillInterval(ILogicalConstruct intervalHeader, IntervalConstruct interval)
        {
            /// For more clarifications on the algorithm, see "6.3.3 Interval Theory" of <see cref="Reverse Compilation Techniques.pdf"/>.

            nodeToInterval.Add(intervalHeader, interval);
            Queue<ILogicalConstruct> possibleNodes = new Queue<ILogicalConstruct>();
            IEnumerable<ILogicalConstruct> currentNodeSuccessors = GetNodeSuccessors(intervalHeader);
            foreach (ILogicalConstruct successor in currentNodeSuccessors)
            {
                /// Check if the successor is from the graph that is being analysed
                /// This check is needed, because GetNodeSuccessors returns all successors, even the one that have been marked only as goto reachable.
                if (availableNodes.Contains(successor))
                {
                    possibleNodes.Enqueue(successor);
                }
            }

            while (possibleNodes.Count > 0)
            {
                ILogicalConstruct currentNode = possibleNodes.Dequeue();

                //check if the node is in any interval
                if (nodeToInterval.ContainsKey(currentNode))
                {
                    continue;
                }

                bool addInInterval = true;
                IEnumerable<ILogicalConstruct> currentNodePredecessorsCollection = GetNodePredecessors(currentNode);
                foreach (ILogicalConstruct predecessor in currentNodePredecessorsCollection)
                {
                    if (!nodeToInterval.ContainsKey(predecessor) || nodeToInterval[predecessor] != interval)
                    {
                        /// The construct has a predecessor, that is not from the current interval.
                        /// Thus, the construct is not dominated by the interval header, and isnt part of the interval
                        addInInterval = false;
                        break;
                    }
                }
                if (addInInterval)
                {
                    /// Add the node to the interval and update the corresponding collections.
                    interval.Children.Add(currentNode);
                    nodeToInterval.Add(currentNode, interval);
                    currentNodeSuccessors = GetNodeSuccessors(currentNode);

                    /// Update the possible nodes collection with all the successors of the current node.
                    foreach (ILogicalConstruct successor in currentNodeSuccessors)
                    {
                        if (availableNodes.Contains(successor))
                        {
                            possibleNodes.Enqueue(successor);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds entries to headers collection after the completion of an interval.
        /// </summary>
        /// <param name="currentHeader">The header of the completed interval.</param>
        /// <param name="currentInterval">The completed interval.</param>
        private void AddNewHeaders(ILogicalConstruct currentHeader, IntervalConstruct currentInterval)
        {
            /// Perform DFS on the headers.
            Stack<ILogicalConstruct> st = new Stack<ILogicalConstruct>();
            HashSet<ILogicalConstruct> checkedHeaders = new HashSet<ILogicalConstruct>();
            st.Push(currentHeader);
            while (st.Count > 0)
            {
                ILogicalConstruct currentNode = st.Pop();
                if (checkedHeaders.Contains(currentNode))
                {
                    continue;
                }
                checkedHeaders.Add(currentNode);

                IEnumerable<ILogicalConstruct> currentNodeSuccessorsCollection = GetNodeSuccessors(currentNode); 

                foreach (ILogicalConstruct node in currentNodeSuccessorsCollection)
                {
                    CheckAndAddPossibleHeader(node, currentInterval, st);
                }
            }
        }
  
        /// <summary>
        /// Checks if <paramref name="node"/> is new header. If it is, it's added to 'headers' collection. If it isn't,
        /// it's pushed in <paramref name="st"/>.
        /// </summary>
        /// <param name="node">The node in question.</param>
        /// <param name="currentInterval">The interval, that looks for successor headers.</param>
        /// <param name="st">The stack of the recursion.</param>
        private void CheckAndAddPossibleHeader(ILogicalConstruct node, IntervalConstruct currentInterval, Stack<ILogicalConstruct> st)
        {
            /// st should be the stack of AddNewHeaders method
            if (!nodeToInterval.ContainsKey(node))
            {
                if (!headers.Contains(node))
                {
                    /// The node is not in any interval, and not marked as header yet.
                    headers.Enqueue(node);
                }
                return;
            }
           
            if (nodeToInterval[node] == currentInterval)
            {
                /// The node is part of the interval being inspected.
                /// The node's children might be headers, so it's added.
                st.Push(node);
            }
        }

        /// <summary>
        /// Creates the graph between the interval constructs in <paramref name="intervals"/>.
        /// </summary>
        private void CreateGraph(IEnumerable<IntervalConstruct> intervals)
        {
            /// Traverse each interval
            foreach (IntervalConstruct interval in intervals)
            {
                /// Traverse each logical construct, part of the interval
                foreach (ILogicalConstruct node in interval.Children)
                {
                    IEnumerable<ILogicalConstruct> nodeSuccessorsCollection = GetNodeSuccessors(node);

                    /// Traverse each of the construct's successors
                    foreach (ILogicalConstruct nodeSuccessor in nodeSuccessorsCollection)
                    {
                        if (nodeToInterval.ContainsKey(nodeSuccessor) && nodeToInterval[nodeSuccessor] != interval)
                        {
                            /// The current successor is not part of the construct's interval
                            /// Thus, the interval that holds the successor must succeed the current construct's interval
                            IntervalConstruct successingInterval = nodeToInterval[nodeSuccessor];
                            if (interval.SameParentSuccessors.Contains(successingInterval))
                            {
                                /// The relation has already been added
                                continue;
                            }

                            interval.SameParentSuccessors.Add(successingInterval);
                            successingInterval.SameParentPredecessors.Add(interval);
                        }
                    }
                }
            }
        }
  
        /// <summary>
        /// Gets all the successors of <paramref name="node"/>. This doesn't include successors marked as goto successors.
        /// </summary>
        /// <param name="node">The parent node.</param>
        /// <returns>Enumeration of all the succeeding nodes.</returns>
        private IEnumerable<ILogicalConstruct> GetNodeSuccessors(ILogicalConstruct node)
        {
            ICollection<ILogicalConstruct> nodeSuccessorsCollection = new List<ILogicalConstruct>();
            foreach (ILogicalConstruct successor in node.SameParentSuccessors)
            {
                if(!removedEdges.ContainsKey(node) || !removedEdges[node].Contains(successor))
                {
                    nodeSuccessorsCollection.Add(successor);
                }
            }

            return nodeSuccessorsCollection;
        }

        /// <summary>
        /// Gets all predecessors of <paramref name="node"/>. This doesn't include goto predecessors.
        /// </summary>
        /// <param name="node">The succeeding node.</param>
        /// <returns>Enumeration of all the parent nodes.</returns>
        private IEnumerable<ILogicalConstruct> GetNodePredecessors(ILogicalConstruct node)
        {
            ICollection<ILogicalConstruct> nodePredecessorsCollection = new HashSet<ILogicalConstruct>();
            foreach (ILogicalConstruct predecessor in node.SameParentPredecessors)
            {
                if(!removedEdges.ContainsKey(predecessor) || !removedEdges[predecessor].Contains(node))
                {
                    nodePredecessorsCollection.Add(predecessor);
                }
            }

            return nodePredecessorsCollection;
        }

        /// <summary>
        /// Sorts the intervals in Reverse post order.
        /// </summary>
        /// <param name="intervals">The intervals to be sorted.</param>
        /// <returns>Returns sorted list of intervals.</returns>
        private List<IntervalConstruct> SortIntervalList(List<IntervalConstruct> intervals)
        {
            IntervalConstruct intervalGraph = new IntervalConstruct(intervals[0]);
            foreach (ISingleEntrySubGraph interval in intervals)
            {
                intervalGraph.Children.Add(interval);
            }
            DFSTree dfsTree = DFSTBuilder.BuildTree(intervalGraph);

            List<IntervalConstruct> sortedList = new List<IntervalConstruct>();

            foreach (DFSTNode node in dfsTree.ReversePostOrder)
            {
                sortedList.Add(node.Construct as IntervalConstruct);
            }

            return sortedList;
        }
    }
}
