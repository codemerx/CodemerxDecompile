using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DFST;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DTree;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Loops
{
    /// <summary>
    /// This class is responsible for the detection of loops. Ideas ahve been taken from <see cref="A Structuring Algorithm for Decompilation.pdf"/>. However,
    /// there are some changes in the way the latching node is decided and how the nodes in the loop are detected.
    /// Multientry loops are not allowed.
    /// </summary>
	internal class LoopBuilder : DominatorTreeDependentStep
	{
        private readonly Dictionary<ILogicalConstruct, HashSet<ILogicalConstruct>> removedEdges = new Dictionary<ILogicalConstruct, HashSet<ILogicalConstruct>>();
        private readonly TypeSystem typeSystem;

        public LoopBuilder(LogicalFlowBuilderContext logicalContext, TypeSystem typeSystem)
            :base(logicalContext)
        {
            this.typeSystem = typeSystem;
        }

        /// <summary>
        /// The entry point of the builder.
        /// </summary>
        /// <param name="block">The logical construct, that is searched for loops.</param>
		public void BuildLoops(ILogicalConstruct block)
		{
			if (block.Children.Count == 0)
			{
				return;
			}

			foreach (ISingleEntrySubGraph child in block.Children)
			{
				//all children should be logical constructs
				ILogicalConstruct childConstruct = child as ILogicalConstruct;
				if (childConstruct == null)
				{
					throw new ArgumentException("Child is not a logical construct.");
				}

                if (childConstruct is ConditionLogicalConstruct)
                {
                    //no loops should be found inside logical constructs
                    //this covers the rare case in which a single block is condition and body of the loop at the same time
                    continue;
                }

                /// Build the inner-most loops first.
				BuildLoops(childConstruct);
			}

			ProcessLogicalConstruct(block);
		}

        /// <summary>
        /// Generates the loops in the graph with entry point <paramref name="construct"/>.
        /// </summary>
        /// <param name="construct">The entry point of the graph.</param>
        private void ProcessLogicalConstruct(ILogicalConstruct construct)
		{
            DominatorTree dominatorTree = GetDominatorTreeFromContext(construct);
			int lastIterationIntervalsCount = int.MaxValue;

            RemoveBackEdgesFromSwitchConstructs(construct);

			while (lastIterationIntervalsCount > 1)
			{
				IntervalAnalyzer ia = new IntervalAnalyzer(construct, removedEdges);
				List<IntervalConstruct> intervals = ia.ReduceCfg();

                List<IntervalConstruct> reverseIntervals = new List<IntervalConstruct>(intervals);
                reverseIntervals.Reverse();

                /// Make only one loop at a time, since the newly made loop can be the header node of a bigger loop (if they are nested so).
				bool madeLoop = false;
				foreach (IntervalConstruct interval in reverseIntervals)
				{
                    if (TryMakeLoop(interval, dominatorTree))
                    {
                        madeLoop = true;
                        break;
                    }
				}

                if(madeLoop)
                {
                    lastIterationIntervalsCount = intervals.Count;
                    continue;
                }

                /// No loop was made in the last iteration and the interval count wasn't reduced. Then, an edge must be deleted from the graph, 
                /// to ensure the reducibility.
				if (intervals.Count == lastIterationIntervalsCount)
				{
					RemoveBlockingEdges(intervals);
				}

                /// This is sanity check. Intervals are supposed to decrease in count, or stay the same with each iteration
                if (intervals.Count > lastIterationIntervalsCount)
				{
					throw new Exception("Intervails are more than in the last iteration.");
				}

				lastIterationIntervalsCount = intervals.Count;
			}
		}

        /// <summary>
        /// Removes back edges from switch blocks. Switch cases can not form a loop. If the control flow forms a loop, it should be represented by goto-label contructs.
        /// </summary>
        /// <param name="theConstruct">The construct, that might contain switches.</param>
        private void RemoveBackEdgesFromSwitchConstructs(ILogicalConstruct theConstruct)
        {
            DFSTree dfsTree = DFSTBuilder.BuildTree(theConstruct);
            foreach (DFSTEdge edge in dfsTree.BackEdges)
            {
                ILogicalConstruct startConstruct = edge.Start.Construct as ILogicalConstruct;
                if (startConstruct is ConditionLogicalConstruct)
                {
                    continue;
                }

                CFGBlockLogicalConstruct startCfgConstruct = startConstruct as CFGBlockLogicalConstruct;
                if((startCfgConstruct != null && startCfgConstruct.TheBlock.Last.OpCode.Code == Mono.Cecil.Cil.Code.Switch))
                {
                    MarkAsGotoEdge(startConstruct, edge.End.Construct as ILogicalConstruct);
                }
            }
        }

        /// <summary>
        /// Removes and edge, that is preventing the reducibillity of the graph.
        /// </summary>
        /// <param name="intervals">The graph, that can't be reduced.</param>
		private void RemoveBlockingEdges(List<IntervalConstruct> intervals)
		{
			//Creating this interval, so that it holds the interval tree
			//This way we can use the DFSTree.
			IntervalConstruct allIntervals = new IntervalConstruct(intervals[0]);
			for (int i = 1; i < intervals.Count; i++)
			{
				allIntervals.Children.Add(intervals[i]);
			}

			DFSTree dfsTree = DFSTBuilder.BuildTree(allIntervals);

            /// Blocking edge can be either cross edge or back edge.
            /// If a backedge is detected, that means it wasn't converted in loop, so it must be marked as goto.
			DFSTEdge edgeToDelete = dfsTree.BackEdges.FirstOrDefault();
			if (edgeToDelete == null)
			{
				edgeToDelete = dfsTree.CrossEdges.FirstOrDefault();
			}

			//both should not be null, since the DFS was ran onto intervals tree
			IntervalConstruct edgeStart = edgeToDelete.Start.Construct as IntervalConstruct;
			IntervalConstruct edgeEnd = edgeToDelete.End.Construct as IntervalConstruct;

            //Find all logical constructs that make the intervals have this edge between them.
            foreach (ILogicalConstruct edgeEndPredecessor in edgeEnd.Entry.SameParentPredecessors)
            {
                if (edgeStart.Children.Contains(edgeEndPredecessor))
                {
                    ILogicalConstruct constructEdgeStart = edgeEndPredecessor;
                    ILogicalConstruct constructEdgeEnd = edgeEnd.Entry as ILogicalConstruct;

                    HashSet<ILogicalConstruct> removedEdgeInfo;
                    if(!removedEdges.TryGetValue(constructEdgeStart, out removedEdgeInfo) || !removedEdgeInfo.Contains(constructEdgeEnd))
                    {
                        MarkAsGotoEdge(constructEdgeStart, constructEdgeEnd);
                        return;
                    }
                }
            }
		}
 
        /// <summary>
        /// Marks the edge between <paramref name="start"/> and <paramref name="end"/> as goto.
        /// </summary>
        /// <param name="start">The starting construct of the edge.</param>
        /// <param name="end">The ending construct of the edge.</param>
		private void MarkAsGotoEdge(ILogicalConstruct start, ILogicalConstruct end)
		{
            /// We assume, that all edges that will be marked as Goto are between CFGBlockLogicalConstruct nodes.
            /// If at some later stage it's evident that this case may arrise between constructs, different from CFGBlocks, then the implementation
            /// must be changed in a way, that ensures that the goto-links are transfered down the logical tree to the CfgBlocks.
            if (start == null || end == null)
			{
				throw new System.ArgumentOutOfRangeException("GoTo edge's ends must implement ILogicalConstruct.");
			}

            HashSet<ILogicalConstruct> removedSuccessors;
            if(!removedEdges.TryGetValue(start, out removedSuccessors))
            {
                removedSuccessors = new HashSet<ILogicalConstruct>();
                removedEdges[start] = removedSuccessors;
            }

            removedSuccessors.Add(end);
		}

	    /// <summary>
	    /// Analyzes <paramref name="interval"/> and makes a loop from it, if possible.
	    /// </summary>
	    /// <param name="interval">The interval to be analyzed.</param>
	    /// <returns>Returns true if a loop was made.</returns>
        private bool TryMakeLoop(IntervalConstruct interval, DominatorTree dominatorTree)
		{
			DFSTree dfsTree = DFSTBuilder.BuildTree(interval);
			if (dfsTree.BackEdges.Count == 0)
			{
				/// No back edges in the interval, so no loop can be made.
				return false;
			}

			HashSet<ILogicalConstruct> loopBody;
			HashSet<ILogicalConstruct> possibleLatchingNodes = BuildLoop(dfsTree, out loopBody);
            ConditionLogicalConstruct loopCondition;
            LoopType typeOfLoop = DetermineLoopType(loopBody, possibleLatchingNodes, interval, dominatorTree, out loopCondition);
            if (loopBody.Count > 0)
            {
                LoopLogicalConstruct loop = new LoopLogicalConstruct(interval.Entry as ILogicalConstruct, loopBody, typeOfLoop, loopCondition, typeSystem);

                CleanUpEdges(loop); /// Covers the case in IrregularbackedgeExitLoop
                UpdateDominatorTree(dominatorTree, loop);
                return true;
            }
            else
            {
                /// Empty loops should not be created. Instead, backedges that form such loops will be marked as goto.
                foreach (DFSTEdge backedge in dfsTree.BackEdges)
                {
                    MarkAsGotoEdge(backedge.Start.Construct as ILogicalConstruct, backedge.End.Construct as ILogicalConstruct);
                }
            }
            return false;
		}

        /// <summary>
        /// Removes backedges exiting from <paramref name="loopConstruct"/>.
        /// </summary>
        /// <param name="loopConstruct">The loop construct.</param>
        private void CleanUpEdges(LoopLogicalConstruct loopConstruct)
        {
            DFSTree dfsTree = DFSTBuilder.BuildTree(loopConstruct.Parent);
            DFSTNode loopNode = dfsTree.ConstructToNodeMap[loopConstruct];
            if(loopNode.BackEdgeSuccessors.Count == 0)
            {
                return;
            }

            foreach (DFSTNode backedgeSuccessor in loopNode.BackEdgeSuccessors)
            {
                ILogicalConstruct edgeEndConstruct = backedgeSuccessor.Construct as ILogicalConstruct;
                if (!(edgeEndConstruct is ConditionLogicalConstruct)) /// if the target is ConditionLogicalConstruct, it can probably be a header of outer loop
                {
                    MarkAsGotoEdge(loopConstruct, backedgeSuccessor.Construct as ILogicalConstruct);
                }
            }
        }

        /// <summary>
        /// Gets a collection of all possible latching nodes for a loop. Builds the body of the loop in the process.
        /// </summary>
        /// <param name="tree">The tree in which the loop was found.</param>
        /// <param name="loopBody">On exit contains the resulted loop body.</param>
        /// <returns>Returns collection of all possible latching nodes.</returns>
		private HashSet<ILogicalConstruct> BuildLoop(DFSTree tree, out HashSet<ILogicalConstruct> loopBody)
		{
			loopBody = new HashSet<ILogicalConstruct>();
			HashSet<ILogicalConstruct> possibleLatchingNodes = new HashSet<ILogicalConstruct>();

            /// Loops are defined by their backedges.
			foreach (DFSTEdge edge in tree.BackEdges)
			{
				ILogicalConstruct header = edge.End.Construct as ILogicalConstruct;
				ILogicalConstruct latchingNode = edge.Start.Construct as ILogicalConstruct;

                /// The edge between the header and the latching node was marked as goto-edge on an earlier step.
                if(removedEdges.ContainsKey(latchingNode) && removedEdges[latchingNode].Contains(header))
                {
                    continue;
                }

				ICollection<DFSTNode> shortLoopBody = tree.GetPath(edge.End, edge.Start);
				ICollection<DFSTNode> fullLoopBody = ExpandLoopBodyWithCrossEdges(shortLoopBody);
				ICollection<ILogicalConstruct> fullLoopBodyConstructs = GetConstructsCollection(fullLoopBody);
				
                if (CanBeLoop(header, latchingNode, fullLoopBody))
				{
					possibleLatchingNodes.Add(latchingNode);
					foreach (ILogicalConstruct construct in fullLoopBodyConstructs)
					{
				        loopBody.Add(construct);
					}
				}
			}

			return possibleLatchingNodes;
		}

        /// <summary>
        /// Creates a collection of logical constructs, corresponding to the nodes in <paramref name="nodeCollection"/>.
        /// </summary>
        /// <param name="nodeCollection">Collection of tree nodes.</param>
        /// <returns>Returns the created collection.</returns>
		private ICollection<ILogicalConstruct> GetConstructsCollection(ICollection<DFSTNode> nodeCollection)
		{
			HashSet<ILogicalConstruct> result = new HashSet<ILogicalConstruct>();
			foreach (DFSTNode node in nodeCollection)
			{
				result.Add(node.Construct as ILogicalConstruct);
			}
			return result;
		}

        /// <summary>
        /// Expands <paramref name="nodesInLoop"/> with the cross edges exiting from it.
        /// </summary>
        /// <param name="nodesInLoop">The nodes, already part of the loop.</param>
        /// <returns>Returns the expanded collection of nodes.</returns>
		private ICollection<DFSTNode> ExpandLoopBodyWithCrossEdges(ICollection<DFSTNode> nodesInLoop)
		{
			HashSet<DFSTNode> hashetNodes = new HashSet<DFSTNode>(nodesInLoop);
			Queue<DFSTNode> queue = new Queue<DFSTNode>(nodesInLoop);
            /// Perform BFS on the nodes.
			while (queue.Count > 0)
			{
				DFSTNode currentNode = queue.Dequeue();
                /// Traversing the predecessors.
                /// Traverse predecessors instead of successors for two reasons
                /// 1) Predecessors are sure to be part of the loop, since the node it precedes is in the loop, thus the predecessor 
                ///     is dominated by the ehader node as well.
                /// 2) For CrossEdge successors, this statement is not valid. Both the node and the successor have common dominator, 
                ///     but this might not be the loop header node.
                foreach (DFSTNode predecessor in currentNode.CrossEdgePredecessors)
                {
                    if (!hashetNodes.Contains(predecessor))
                    {
                        hashetNodes.Add(predecessor);
                        queue.Enqueue(predecessor);
                    }
                }

                DFSTNode nodePredecessor = currentNode.Predecessor as DFSTNode;

                if (nodePredecessor != null && !hashetNodes.Contains(nodePredecessor))
                {
                    hashetNodes.Add(nodePredecessor);
                    queue.Enqueue(nodePredecessor);
                }
			}
			return hashetNodes;
		}

        /// <summary>
        /// Determines if <paramref name="header"/>, <paramref name="latchingNode"/> and <paramref name="nodesInLoop"/> can form a loop.
        /// </summary>
        /// <param name="header">The header of the loop.</param>
        /// <param name="latchingNode">The latching node of the loop.</param>
        /// <param name="nodesInLoop">The body of the loop.</param>
        /// <returns>Returns true, if correct loop can be formed.</returns>
		private bool CanBeLoop(ILogicalConstruct header, ILogicalConstruct latchingNode, ICollection<DFSTNode> nodesInLoop)
		{
			if (header == null || latchingNode == null)
			{
				return false;
			}

			ICollection<ILogicalConstruct> constructsInLoop = GetConstructsCollection(nodesInLoop);

			foreach (ILogicalConstruct node in constructsInLoop)
			{
				if (!CanBeInLoop(node, constructsInLoop, header))
				{
					return false;
				}
			}
			return true;
		}

        /// <summary>
        /// Checks if single node can be in a loop. The loop is determined by <paramref name="loopHeader"/> and has <paramref name="nodesInLoop"/> for its body.
        /// </summary>
        /// <param name="node">The node in question.</param>
        /// <param name="nodesInLoop">The other nodes in the body of the loop.</param>
        /// <param name="loopHeader">The header of the loop.</param>
        /// <returns>Returns true, if <paramref name="node"/> can be in the loop.</returns>
		private bool CanBeInLoop(ILogicalConstruct node, ICollection<ILogicalConstruct> nodesInLoop, ILogicalConstruct loopHeader)
		{
			if (node == null)
			{
				return false;
			}

			if (node == loopHeader)
			{
                /// The header is always part of the loop.
				return true;
			}

			foreach (ISingleEntrySubGraph predecessor in node.SameParentPredecessors)
			{
				ILogicalConstruct predecessorLogicalConstruct = predecessor as ILogicalConstruct;
				if (predecessorLogicalConstruct == null)
				{
					return false;
				}

                HashSet<ILogicalConstruct> removedEdgesEnds;
                if (!nodesInLoop.Contains(predecessorLogicalConstruct) ||
                    (removedEdges.TryGetValue(predecessorLogicalConstruct, out removedEdgesEnds) && removedEdgesEnds.Contains(node)))
				{
                    /// The predecessor is not part of the loop.
                    /// Or it was marked for goto on earlier stage.
					/// Either way this is multy-entry loop, which is not allowed
					return false;
				}
			}

			return true;
		}

        /// <summary>
        /// Determines the type of the loop and the condition of the loop. Adds additional nodes into the loop body.
        /// </summary>
        /// <param name="loopBody"></param>
        /// <param name="header"></param>
        /// <param name="latchingNodes"></param>
        /// <param name="interval"></param>
        /// <param name="loopCondition"></param>
        /// <returns></returns>
        private LoopType DetermineLoopType(HashSet<ILogicalConstruct> loopBody, HashSet<ILogicalConstruct> latchingNodes,
            IntervalConstruct interval, DominatorTree dominatorTree, out ConditionLogicalConstruct loopCondition)
        {
            ILogicalConstruct header = interval.Entry as ILogicalConstruct;
            HashSet<ILogicalConstruct> legalExits = new HashSet<ILogicalConstruct>(latchingNodes);
            legalExits.Add(header);

            ILogicalConstruct parentConstruct = header.Parent as ILogicalConstruct;
            DFSTree dfsTree = DFSTBuilder.BuildTree(parentConstruct);

            //B - nodes in the loop body (= loopBody)
            //I - nodes in the interval (= interval.Children)
            //U - union of all of the dominance frontiers of the nodes in B
            //exitDominanceFrontier = (U n I) \ B
            //If a node is in the exitDominanceFrontier, then it is dominated by the header and is a successor (not necessarily direct) of more than one
            //node in the loop body.
            HashSet<ILogicalConstruct> exitDominanceFrontier = new HashSet<ILogicalConstruct>();
            foreach (ILogicalConstruct loopNode in loopBody)
            {
                foreach (ILogicalConstruct frontierNode in dominatorTree.GetDominanceFrontier(loopNode))
                {
                    if (interval.Children.Contains(frontierNode) && !loopBody.Contains(frontierNode))
                    {
                        exitDominanceFrontier.Add(frontierNode);
                    }
                }
            }

            //This is leftover heuristic, that was used for determining a suitable successor that is going to be follow node of the loop.
            //Changing it now will break a good number of the tests. Since the produced output is acceptable, until a better heuristic is found
            //there is no need to change it.
            if (exitDominanceFrontier.Count == 0)
            {
                //If the exit dominance frontier is empty then we look for the node, with minimum post order index, that is a successor of a condition loop exit.
                //The desired exit should be a condition in order to reduce the number of infinite loops (heuristic).
                foreach (DFSTNode dfsNode in dfsTree.ReversePostOrder)
                {
                    ILogicalConstruct construct = dfsNode.Construct as ILogicalConstruct;
                    if (loopBody.Contains(construct))
                    {
                        continue;
                    }

                    loopCondition = GetLoopConditionWithMaxIndex(dfsTree, loopBody, legalExits, construct);
                    //By taking the successor with the minimum post order index and the loop exit with the maximum post order index, we ensure that
                    //the produced construct will always be the same, since the post order in our case is a total order.
                    //There are other various ways of finding the exit-successor pair that can bring consistent output, but none of them is found to yield
                    //better results than the rest.
                    if (loopCondition != null)
                    {
                        //We expand the loop body only when we've found a condition successor of the loop.
                        //This is done in order to avoid adding all of the dominated nodes of an infinite loop to the body. (Better readability of the final code.)
                        //E.g.: An infinite loop on the top level of the logical tree (i.e. child of the method block construct). If it dominates all of its
                        //succeeding nodes then they will be in its interval, which means that they will be added to the loop. As a result there will
                        //be an infinite loop at the end of the method, that encloses a cood part of the code, for no apparent reason.
                        ExpandLoopBody(interval, loopBody, construct);

                        if (loopCondition == header)
                        {
                            return LoopType.PreTestedLoop;
                        }
                        else
                        {
                            return LoopType.PostTestedLoop;
                        }
                    }
                }

                if (CanBeLoopCondition(header, loopBody))
                {
                    loopCondition = header as ConditionLogicalConstruct;
                    return LoopType.PreTestedLoop;
                }
                else
                {
                    loopCondition = null;
                    return LoopType.InfiniteLoop;
                }
            }
            else
            {
                //If there are nodes in the exitDominanceFrontier, then we choose the one with the minimum postorder index for successor of the loop.
                //Then we try to find a condition exit of the loop, with maximum post order index, that is predecessor of the successor node.
                int minOrderIndexOfSuccessor = dfsTree.ReversePostOrder.Count;
                foreach (ILogicalConstruct successor in exitDominanceFrontier)
                {
                    int currentOrderIndex = dfsTree.ConstructToNodeMap[successor].ReversePostOrderIndex;

                    if(currentOrderIndex < minOrderIndexOfSuccessor)
                    {
                        minOrderIndexOfSuccessor = currentOrderIndex;
                    }
                }

                ILogicalConstruct loopSuccessor = dfsTree.ReversePostOrder[minOrderIndexOfSuccessor].Construct as ILogicalConstruct;

                loopCondition = GetLoopConditionWithMaxIndex(dfsTree, loopBody, legalExits, loopSuccessor);

                ExpandLoopBody(interval, loopBody, loopSuccessor);

                if (loopCondition != null)
                {
                    if (loopCondition == header)
                    {
                        return LoopType.PreTestedLoop;
                    }
                    else
                    {
                        return LoopType.PostTestedLoop;
                    }
                }
                else
                {
                    return LoopType.InfiniteLoop;
                }
            }
        }

        /// <summary>
        /// Returns the condition loop exit with the greatest post order index, that has the <paramref name="loopSuccessor"/> as successor.
        /// </summary>
        /// <param name="dfsTree"></param>
        /// <param name="loopBody"></param>
        /// <param name="loopExits"></param>
        /// <param name="loopSuccessor"></param>
        /// <returns></returns>
        private ConditionLogicalConstruct GetLoopConditionWithMaxIndex(DFSTree dfsTree, HashSet<ILogicalConstruct> loopBody, HashSet<ILogicalConstruct> loopExits,
            ILogicalConstruct loopSuccessor)
        {
            int maxOrderIndexOfExit = -1;
            foreach (ILogicalConstruct loopExit in loopExits)
            {
                int currentOrderIndex = dfsTree.ConstructToNodeMap[loopExit].ReversePostOrderIndex;
                if (loopExit.SameParentSuccessors.Contains(loopSuccessor) && currentOrderIndex > maxOrderIndexOfExit && CanBeLoopCondition(loopExit, loopBody))
                {
                    maxOrderIndexOfExit = currentOrderIndex;
                }
            }

            if(maxOrderIndexOfExit > -1)
            {
                return dfsTree.ReversePostOrder[maxOrderIndexOfExit].Construct as ConditionLogicalConstruct;
            }

            return null;
        }

        /// <summary>
        /// Determines whether the specified <paramref name="node"/> can be a condition to the loop with body - <paramref name="loopBody"/>.
        /// </summary>
        /// <param name="node">The possible condition node.</param>
        /// <param name="loopBody">The body of the loop.</param>
        /// <returns>Returns true, if <paramref name="node"/> can be the condition of the loop.</returns>
        private bool CanBeLoopCondition(ILogicalConstruct node, HashSet<ILogicalConstruct> loopBody)
        {
            if(!loopBody.Contains(node))
            {
                //The node should be inside the loop.
                return false;
            }

            
            if(node is ConditionLogicalConstruct)
            {
                HashSet<ISingleEntrySubGraph> nodeSuccessors = node.SameParentSuccessors;
                int insideBody = 0;
                foreach (ILogicalConstruct successor in nodeSuccessors)
                {
                    if (loopBody.Contains(successor))
                    {
                        insideBody++;
                    }
                }

                //There should be exactly one successor that is inside the body of the loop.
                return insideBody == 1;
            }

            return false;
        }

        /// <summary>
        /// Adds all of the nodes from the <paramref name="interval"/>, that are not preceded by the <paramref name="loopSuccessor"/>,
        /// to the <paramref name="loopBody"/>.
        /// </summary>
        /// <remarks>
        /// Does not add the <paramref name="loopSuccessor"/>.
        /// </remarks>
        private void ExpandLoopBody(IntervalConstruct interval, HashSet<ILogicalConstruct> loopBody, ILogicalConstruct loopSuccessor)
        {
            HashSet<ILogicalConstruct> nodesToSkip = GetIntervalSuccessors(interval, loopSuccessor);
            nodesToSkip.Add(loopSuccessor);

            foreach (LogicalConstructBase node in interval.Children)
            {
                if (!nodesToSkip.Contains(node))
                {
                    loopBody.Add(node);
                }
            }
        }

        /// <summary>
        /// Gets the successors (direct or indirect) of the given <paramref name="startNode"/>,
        /// that are in the specified <paramref name="interval"/>.
        /// </summary>
        /// <remarks>
        /// If the start node is not in the interval, then it will not be traversed. This is a corner case for entwined loops.
        /// The start node will not be included in the result, even if it is its own successor.
        /// </remarks>
        /// <returns></returns>
        private HashSet<ILogicalConstruct> GetIntervalSuccessors(IntervalConstruct interval, ILogicalConstruct startNode)
        {
            HashSet<ILogicalConstruct> intervalSuccessors = new HashSet<ILogicalConstruct>();
            if (!interval.Children.Contains(startNode))
            {
                return intervalSuccessors;
            }

            Queue<ILogicalConstruct> traversalQueue = new Queue<ILogicalConstruct>();
            traversalQueue.Enqueue(startNode);
            HashSet<ILogicalConstruct> traversedNodes = new HashSet<ILogicalConstruct>();
            traversedNodes.Add(startNode);

            while(traversalQueue.Count > 0)
            {
                ILogicalConstruct currentNode = traversalQueue.Dequeue();
                foreach (ILogicalConstruct successor in currentNode.SameParentSuccessors)
                {
                    if(!traversedNodes.Contains(successor) && interval.Children.Contains(successor) && intervalSuccessors.Add(successor))
                    {
                        traversedNodes.Add(successor);
                        traversalQueue.Enqueue(successor);
                    }
                }
            }

            return intervalSuccessors;
        }

        private void UpdateDominatorTree(DominatorTree dominatorTree, LoopLogicalConstruct theLoopConstruct)
        {
            HashSet<ISingleEntrySubGraph> loopNodes = new HashSet<ISingleEntrySubGraph>();
            if (theLoopConstruct.LoopCondition != null)
            {
                loopNodes.Add(theLoopConstruct.LoopCondition);
            }
            if (theLoopConstruct.LoopBodyBlock != null)
            {
                loopNodes.UnionWith(theLoopConstruct.LoopBodyBlock.Children);
            }

            ISingleEntrySubGraph loopEntry = (theLoopConstruct.LoopType == LoopType.PreTestedLoop) ? theLoopConstruct.LoopCondition : theLoopConstruct.LoopBodyBlock.Entry;
            dominatorTree.MergeNodes(loopNodes, loopEntry, theLoopConstruct);
        }
	}
}
