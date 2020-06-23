using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
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
		}

		protected abstract void AnalyzeAssignments();

		protected AssignmentType AnalyzeAssignmentType(BaseUsageFinder usageFinder)
		{
			int count;
			this.PrepareNodes();
			AssignmentAnalyzer assignmentAnalyzer = new AssignmentAnalyzer(usageFinder, this.context.MethodContext.Expressions);
			AssignmentType assignmentType = assignmentAnalyzer.CheckAssignmentType(this.mappedNodes[0]);
			if (assignmentType == AssignmentType.NotAssigned)
			{
				return AssignmentType.NotAssigned;
			}
			AssignmentType assignmentType1 = assignmentType;
			List<ExceptionHandler> exceptionHandlers = new List<ExceptionHandler>(this.context.MethodContext.ControlFlowGraph.RawExceptionHandlers);
			do
			{
				count = exceptionHandlers.Count;
				for (int i = 0; i < exceptionHandlers.Count; i++)
				{
					ExceptionHandler item = exceptionHandlers[i];
					if (this.GetNodeFromBlockOffset(item.TryStart.Offset).NodeState != AssignmentNodeState.Unknown)
					{
						int num = i;
						i = num - 1;
						exceptionHandlers.RemoveAt(num);
						this.CheckHandler(this.GetNodeFromBlockOffset(item.HandlerStart.Offset), assignmentAnalyzer, ref assignmentType1);
						if (assignmentType1 == AssignmentType.NotAssigned)
						{
							return AssignmentType.NotAssigned;
						}
						if (item.HandlerType == ExceptionHandlerType.Filter)
						{
							this.CheckHandler(this.GetNodeFromBlockOffset(item.FilterStart.Offset), assignmentAnalyzer, ref assignmentType1);
							if (assignmentType1 == AssignmentType.NotAssigned)
							{
								return AssignmentType.NotAssigned;
							}
						}
					}
				}
			}
			while (count != exceptionHandlers.Count);
			return assignmentType1;
		}

		private void CheckHandler(AssignmentFlowNode handlerEntry, AssignmentAnalyzer analyzer, ref AssignmentType result)
		{
			AssignmentType assignmentType = analyzer.CheckAssignmentType(handlerEntry);
			switch (assignmentType)
			{
				case AssignmentType.NotAssigned:
				case AssignmentType.MultipleAssignments:
				{
					result = assignmentType;
					break;
				}
				case AssignmentType.SingleAssignment:
				{
					if ((int)result == 3)
					{
						break;
					}
					result = assignmentType;
					return;
				}
				default:
				{
					return;
				}
			}
		}

		private AssignmentFlowNode GetNodeFromBlockOffset(int offset)
		{
			return this.mappedNodes[this.context.MethodContext.ControlFlowGraph.InstructionToBlockMapping[offset].Index];
		}

		private void PrepareNodes()
		{
			AssignmentFlowNode[] assignmentFlowNodeArray = this.mappedNodes;
			for (int i = 0; i < (int)assignmentFlowNodeArray.Length; i++)
			{
				assignmentFlowNodeArray[i].NodeState = AssignmentNodeState.Unknown;
			}
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
			this.mappedNodes = new AssignmentFlowNode[(int)this.context.MethodContext.ControlFlowGraph.Blocks.Length];
			for (int i = 0; i < (int)this.mappedNodes.Length; i++)
			{
				this.mappedNodes[i] = new AssignmentFlowNode(this.context.MethodContext.ControlFlowGraph.Blocks[i]);
			}
			AssignmentFlowNode[] assignmentFlowNodeArray = this.mappedNodes;
			for (int j = 0; j < (int)assignmentFlowNodeArray.Length; j++)
			{
				AssignmentFlowNode assignmentFlowNode = assignmentFlowNodeArray[j];
				for (int k = 0; k < assignmentFlowNode.Successors.Count; k++)
				{
					assignmentFlowNode.Successors[k] = this.mappedNodes[assignmentFlowNode.CFGBlock.Successors[k].Index];
				}
			}
		}

		protected virtual bool ShouldExecuteStep()
		{
			return true;
		}
	}
}