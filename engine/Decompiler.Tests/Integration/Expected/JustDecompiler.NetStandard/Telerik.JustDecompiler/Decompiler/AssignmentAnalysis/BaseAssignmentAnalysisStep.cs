using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
	internal abstract class BaseAssignmentAnalysisStep : IDecompilationStep
	{
		protected DecompilationContext context;

		private AssignmentFlowNode[] mappedNodes;

		protected BaseAssignmentAnalysisStep()
		{
			base();
			return;
		}

		protected abstract void AnalyzeAssignments();

		protected AssignmentType AnalyzeAssignmentType(BaseUsageFinder usageFinder)
		{
			this.PrepareNodes();
			V_0 = new AssignmentAnalyzer(usageFinder, this.context.get_MethodContext().get_Expressions());
			V_1 = V_0.CheckAssignmentType(this.mappedNodes[0]);
			if (V_1 == 1)
			{
				return 1;
			}
			V_2 = V_1;
			V_3 = new List<ExceptionHandler>(this.context.get_MethodContext().get_ControlFlowGraph().get_RawExceptionHandlers());
			do
			{
				V_4 = V_3.get_Count();
				V_5 = 0;
				while (V_5 < V_3.get_Count())
				{
					V_6 = V_3.get_Item(V_5);
					if (this.GetNodeFromBlockOffset(V_6.get_TryStart().get_Offset()).get_NodeState() != AssignmentNodeState.Unknown)
					{
						stackVariable41 = V_5;
						V_5 = stackVariable41 - 1;
						V_3.RemoveAt(stackVariable41);
						this.CheckHandler(this.GetNodeFromBlockOffset(V_6.get_HandlerStart().get_Offset()), V_0, ref V_2);
						if (V_2 == 1)
						{
							return 1;
						}
						if (V_6.get_HandlerType() == 1)
						{
							this.CheckHandler(this.GetNodeFromBlockOffset(V_6.get_FilterStart().get_Offset()), V_0, ref V_2);
							if (V_2 == 1)
							{
								return 1;
							}
						}
					}
					V_5 = V_5 + 1;
				}
			}
			while (V_4 != V_3.get_Count());
			return V_2;
		}

		private void CheckHandler(AssignmentFlowNode handlerEntry, AssignmentAnalyzer analyzer, ref AssignmentType result)
		{
			V_0 = analyzer.CheckAssignmentType(handlerEntry);
			switch (V_0 - 1)
			{
				case 0:
				case 2:
				{
					result = V_0;
					break;
				}
				case 1:
				{
					if ((int)result == 3)
					{
						break;
					}
					result = V_0;
					return;
				}
				default:
				{
					return;
				}
			}
			return;
		}

		private AssignmentFlowNode GetNodeFromBlockOffset(int offset)
		{
			return this.mappedNodes[this.context.get_MethodContext().get_ControlFlowGraph().get_InstructionToBlockMapping().get_Item(offset).get_Index()];
		}

		private void PrepareNodes()
		{
			V_0 = this.mappedNodes;
			V_1 = 0;
			while (V_1 < (int)V_0.Length)
			{
				V_0[V_1].set_NodeState(0);
				V_1 = V_1 + 1;
			}
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			if (this.ShouldExecuteStep())
			{
				this.ProcessTheCFG();
				this.AnalyzeAssignments();
			}
			return body;
		}

		private void ProcessTheCFG()
		{
			this.mappedNodes = new AssignmentFlowNode[(int)this.context.get_MethodContext().get_ControlFlowGraph().get_Blocks().Length];
			V_0 = 0;
			while (V_0 < (int)this.mappedNodes.Length)
			{
				this.mappedNodes[V_0] = new AssignmentFlowNode(this.context.get_MethodContext().get_ControlFlowGraph().get_Blocks()[V_0]);
				V_0 = V_0 + 1;
			}
			V_1 = this.mappedNodes;
			V_2 = 0;
			while (V_2 < (int)V_1.Length)
			{
				V_3 = V_1[V_2];
				V_4 = 0;
				while (V_4 < V_3.get_Successors().get_Count())
				{
					V_3.get_Successors().set_Item(V_4, this.mappedNodes[V_3.get_CFGBlock().get_Successors()[V_4].get_Index()]);
					V_4 = V_4 + 1;
				}
				V_2 = V_2 + 1;
			}
			return;
		}

		protected virtual bool ShouldExecuteStep()
		{
			return true;
		}
	}
}