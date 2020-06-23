using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
	internal class AssignmentAnalyzer
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
			AssignmentType assignmentType;
			int num = 0;
			Queue<AssignmentFlowNode> assignmentFlowNodes = new Queue<AssignmentFlowNode>();
			this.CheckForAssignment(entryNode);
			if (entryNode.NodeState == AssignmentNodeState.ContainsAssignment)
			{
				return AssignmentType.SingleAssignment;
			}
			if (entryNode.NodeState == AssignmentNodeState.ContainsUsage)
			{
				return AssignmentType.NotAssigned;
			}
			assignmentFlowNodes.Enqueue(entryNode);
			while (assignmentFlowNodes.Count > 0)
			{
				using (IEnumerator<AssignmentFlowNode> enumerator = assignmentFlowNodes.Dequeue().Successors.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						AssignmentFlowNode current = enumerator.Current;
						if (current.NodeState != AssignmentNodeState.Unknown)
						{
							continue;
						}
						this.CheckForAssignment(current);
						if (current.NodeState == AssignmentNodeState.NotAssigned)
						{
							assignmentFlowNodes.Enqueue(current);
						}
						else if (current.NodeState != AssignmentNodeState.ContainsAssignment)
						{
							if (current.NodeState != AssignmentNodeState.ContainsUsage)
							{
								continue;
							}
							assignmentType = AssignmentType.NotAssigned;
							return assignmentType;
						}
						else
						{
							num++;
						}
					}
					continue;
				}
				return assignmentType;
			}
			if (num == 0)
			{
				return AssignmentType.NotUsed;
			}
			if (num != 1)
			{
				return AssignmentType.MultipleAssignments;
			}
			return AssignmentType.SingleAssignment;
		}

		private void CheckForAssignment(AssignmentFlowNode node)
		{
			if (node.NodeState != AssignmentNodeState.Unknown)
			{
				return;
			}
			foreach (Expression item in this.expressionsData.BlockExpressions[node.CFGBlock.First.Offset])
			{
				UsageFinderSearchResult usageFinderSearchResult = this.visitor.SearchForUsage(item);
				if (usageFinderSearchResult != UsageFinderSearchResult.Assigned)
				{
					if (usageFinderSearchResult != UsageFinderSearchResult.Used)
					{
						continue;
					}
					node.NodeState = AssignmentNodeState.ContainsUsage;
					return;
				}
				else
				{
					node.NodeState = AssignmentNodeState.ContainsAssignment;
					return;
				}
			}
			node.NodeState = AssignmentNodeState.NotAssigned;
		}
	}
}