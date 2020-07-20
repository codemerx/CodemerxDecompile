using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
	internal class AssignmentAnalyzer
	{
		private readonly ExpressionDecompilerData expressionsData;

		private readonly BaseUsageFinder visitor;

		public AssignmentAnalyzer(BaseUsageFinder visitor, ExpressionDecompilerData expressionsData)
		{
			base();
			this.visitor = visitor;
			this.expressionsData = expressionsData;
			return;
		}

		public AssignmentType CheckAssignmentType(AssignmentFlowNode entryNode)
		{
			V_0 = 0;
			V_1 = new Queue<AssignmentFlowNode>();
			this.CheckForAssignment(entryNode);
			if (entryNode.get_NodeState() == 2)
			{
				return 2;
			}
			if (entryNode.get_NodeState() == 3)
			{
				return 1;
			}
			V_1.Enqueue(entryNode);
			while (V_1.get_Count() > 0)
			{
				V_2 = V_1.Dequeue().get_Successors().GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = V_2.get_Current();
						if (V_3.get_NodeState() != AssignmentNodeState.Unknown)
						{
							continue;
						}
						this.CheckForAssignment(V_3);
						if (V_3.get_NodeState() != 1)
						{
							if (V_3.get_NodeState() != 2)
							{
								if (V_3.get_NodeState() != 3)
								{
									continue;
								}
								V_4 = 1;
								goto Label0;
							}
							else
							{
								V_0 = V_0 + 1;
							}
						}
						else
						{
							V_1.Enqueue(V_3);
						}
					}
					continue;
				}
				finally
				{
					if (V_2 != null)
					{
						V_2.Dispose();
					}
				}
			Label0:
				return V_4;
			}
			if (V_0 == 0)
			{
				return 0;
			}
			if (V_0 != 1)
			{
				return 3;
			}
			return 2;
		}

		private void CheckForAssignment(AssignmentFlowNode node)
		{
			if (node.get_NodeState() != AssignmentNodeState.Unknown)
			{
				return;
			}
			V_0 = this.expressionsData.get_BlockExpressions().get_Item(node.get_CFGBlock().get_First().get_Offset()).GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = this.visitor.SearchForUsage(V_1);
					if (V_2 != 1)
					{
						if (V_2 != 2)
						{
							continue;
						}
						node.set_NodeState(3);
						goto Label0;
					}
					else
					{
						node.set_NodeState(2);
						goto Label0;
					}
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			node.set_NodeState(1);
		Label0:
			return;
		}
	}
}