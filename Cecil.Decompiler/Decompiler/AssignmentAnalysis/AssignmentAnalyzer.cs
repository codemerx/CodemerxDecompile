using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
    class AssignmentAnalyzer
    {
        private readonly ExpressionDecompilerData expressionsData;
        private readonly BaseUsageFinder visitor;

        public AssignmentAnalyzer(BaseUsageFinder visitor, ExpressionDecompilerData expressionsData)
        {
            this.visitor = visitor;
            this.expressionsData = expressionsData;
        }

        public AssignmentType CheckAssignmentType(AssignmentFlowNode entryNode)
        {
            int assignmentsCount = 0;
            Queue<AssignmentFlowNode> traversalQueue = new Queue<AssignmentFlowNode>();

            CheckForAssignment(entryNode);
            if (entryNode.NodeState == AssignmentNodeState.ContainsAssignment)
            {
                return AssignmentType.SingleAssignment;
            }
            else if (entryNode.NodeState == AssignmentNodeState.ContainsUsage)
            {
                return AssignmentType.NotAssigned;
            }

            traversalQueue.Enqueue(entryNode);

            while (traversalQueue.Count > 0)
            {
                AssignmentFlowNode currentNode = traversalQueue.Dequeue();

                foreach (AssignmentFlowNode successor in currentNode.Successors)
                {
                    if (successor.NodeState != AssignmentNodeState.Unknown)
                    {
                        continue;
                    }

                    CheckForAssignment(successor);

                    if (successor.NodeState == AssignmentNodeState.NotAssigned)
                    {
                        traversalQueue.Enqueue(successor);
                    }
                    else if (successor.NodeState == AssignmentNodeState.ContainsAssignment)
                    {
                        assignmentsCount++;
                    }
                    else if (successor.NodeState == AssignmentNodeState.ContainsUsage)
                    {
                        return AssignmentType.NotAssigned;
                    }
                }
            }

            return assignmentsCount == 0 ? AssignmentType.NotUsed :
                assignmentsCount == 1 ? AssignmentType.SingleAssignment : AssignmentType.MultipleAssignments;
        }

        private void CheckForAssignment(AssignmentFlowNode node)
        {
            if(node.NodeState != AssignmentNodeState.Unknown)
            {
                return;
            }

            IList<Expression> expressions = expressionsData.BlockExpressions[node.CFGBlock.First.Offset];

            foreach (Expression expression in expressions)
            {
                UsageFinderSearchResult searchResult = visitor.SearchForUsage(expression);
                if (searchResult == UsageFinderSearchResult.Assigned)
                {
                    node.NodeState = AssignmentNodeState.ContainsAssignment;
                    return;
                }
                else if (searchResult == UsageFinderSearchResult.Used)
                {
                    node.NodeState = AssignmentNodeState.ContainsUsage;
                    return;
                }
            }

            node.NodeState = AssignmentNodeState.NotAssigned;
        }
    }
}
