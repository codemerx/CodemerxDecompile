using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Loops;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Switches;
using Telerik.JustDecompiler.Steps;
using Telerik.JustDecompiler.Cil;
using System;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	class FollowNodeLoopCleanUpStep : IDecompilationStep
	{
		private readonly HashSet<ILogicalConstruct> visitedConstructs;

		public FollowNodeLoopCleanUpStep()
		{
			visitedConstructs = new HashSet<ILogicalConstruct>();
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			ProcessLogicalConstruct(context.MethodContext.LogicalConstructsTree);
			return body;
		}

		private void ProcessLogicalConstruct(ILogicalConstruct theConstruct)
		{
			if (theConstruct is BlockLogicalConstruct)
			{
				ILogicalConstruct current = (ILogicalConstruct)theConstruct.Entry;
				while (current != null)
				{
					ProcessLogicalConstruct(current);
					if(visitedConstructs.Contains(current.FollowNode))
					{
						current.CFGFollowNode = null;
					}
					current = current.FollowNode;
				}

                ProcessGotoFlowConstructs(theConstruct as BlockLogicalConstruct);
			}
			else if (theConstruct is ExceptionHandlingLogicalConstruct)
			{
				ProcessLogicalConstruct((theConstruct as ExceptionHandlingLogicalConstruct).Try);
				if (theConstruct is TryCatchFilterLogicalConstruct)
				{
					foreach (IFilteringExceptionHandler handler in (theConstruct as TryCatchFilterLogicalConstruct).Handlers)
					{
						if (handler.HandlerType == FilteringExceptionHandlerType.Filter)
						{
							ProcessLogicalConstruct((handler as ExceptionHandlingBlockFilter).Filter);
							ProcessLogicalConstruct((handler as ExceptionHandlingBlockFilter).Handler);
						}
						else if (handler.HandlerType == FilteringExceptionHandlerType.Catch)
						{
							ProcessLogicalConstruct((handler as ExceptionHandlingBlockCatch));
						}
					}
				}
				else if (theConstruct is TryFaultLogicalConstruct)
				{
					ProcessLogicalConstruct((theConstruct as TryFaultLogicalConstruct).Fault);
				}
				else if (theConstruct is TryFinallyLogicalConstruct)
				{
					ProcessLogicalConstruct((theConstruct as TryFinallyLogicalConstruct).Finally);
				}
			}
			else if (theConstruct is IfLogicalConstruct)
			{
				IfLogicalConstruct theIf = theConstruct as IfLogicalConstruct;
				ProcessLogicalConstruct(theIf.Then);
				if (theIf.Else != null)
				{
					ProcessLogicalConstruct(theIf.Else);
				}
			}
			else if (theConstruct is LoopLogicalConstruct)
			{
				LoopLogicalConstruct theLogicalLoop = theConstruct as LoopLogicalConstruct;
				ProcessLogicalConstruct(theLogicalLoop.LoopBodyBlock);
				ProcessLogicalConstruct(theLogicalLoop.LoopCondition);
			}
			else if (theConstruct is SwitchLogicalConstruct)
			{
				SwitchLogicalConstruct theLogicalSwitch = theConstruct as SwitchLogicalConstruct;
				foreach(CaseLogicalConstruct @case in theLogicalSwitch.ConditionCases)
				{
					ProcessLogicalConstruct(@case);
				}
                ProcessLogicalConstruct(theLogicalSwitch.DefaultCase);
			}
			else if (theConstruct is ConditionLogicalConstruct)
			{
				ConditionLogicalConstruct clc = theConstruct as ConditionLogicalConstruct;
				ProcessLogicalConstruct(clc.FirstBlock);
			}

			visitedConstructs.Add(theConstruct);
		}

        private void ProcessGotoFlowConstructs(BlockLogicalConstruct theConstruct)
        {
            ILogicalConstruct[] sortedChildren = new ILogicalConstruct[theConstruct.Children.Count];
            int index = 0;
            foreach (ILogicalConstruct child in theConstruct.Children)
            {
                sortedChildren[index++] = child;
            }
            Array.Sort<ISingleEntrySubGraph>(sortedChildren);

            HashSet<ILogicalConstruct> usedAsFollow = new HashSet<ILogicalConstruct>();
            foreach (ILogicalConstruct currentChild in sortedChildren)
            {
                if(visitedConstructs.Add(currentChild))
                {
                    if (visitedConstructs.Contains(currentChild.FollowNode) || !usedAsFollow.Add(currentChild.FollowNode))
                    {
                        currentChild.CFGFollowNode = null;
                    }

                    ProcessLogicalConstruct(currentChild);
                }
            }
        }
	}
}