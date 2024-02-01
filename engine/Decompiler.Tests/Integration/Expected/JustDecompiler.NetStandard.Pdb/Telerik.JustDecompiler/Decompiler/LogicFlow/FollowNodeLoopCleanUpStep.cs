using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Loops;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Switches;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	internal class FollowNodeLoopCleanUpStep : IDecompilationStep
	{
		private readonly HashSet<ILogicalConstruct> visitedConstructs;

		public FollowNodeLoopCleanUpStep()
		{
			this.visitedConstructs = new HashSet<ILogicalConstruct>();
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.ProcessLogicalConstruct(context.MethodContext.LogicalConstructsTree);
			return body;
		}

		private void ProcessGotoFlowConstructs(BlockLogicalConstruct theConstruct)
		{
			ILogicalConstruct[] logicalConstructArray = new ILogicalConstruct[theConstruct.Children.Count];
			int num = 0;
			foreach (ILogicalConstruct child in theConstruct.Children)
			{
				int num1 = num;
				num = num1 + 1;
				logicalConstructArray[num1] = child;
			}
			Array.Sort<ISingleEntrySubGraph>(logicalConstructArray);
			HashSet<ILogicalConstruct> logicalConstructs = new HashSet<ILogicalConstruct>();
			ILogicalConstruct[] logicalConstructArray1 = logicalConstructArray;
			for (int i = 0; i < (int)logicalConstructArray1.Length; i++)
			{
				ILogicalConstruct logicalConstruct = logicalConstructArray1[i];
				if (this.visitedConstructs.Add(logicalConstruct))
				{
					if (this.visitedConstructs.Contains(logicalConstruct.FollowNode) || !logicalConstructs.Add(logicalConstruct.FollowNode))
					{
						logicalConstruct.CFGFollowNode = null;
					}
					this.ProcessLogicalConstruct(logicalConstruct);
				}
			}
		}

		private void ProcessLogicalConstruct(ILogicalConstruct theConstruct)
		{
			int j;
			if (theConstruct is BlockLogicalConstruct)
			{
				for (ILogicalConstruct i = (ILogicalConstruct)theConstruct.Entry; i != null; i = i.FollowNode)
				{
					this.ProcessLogicalConstruct(i);
					if (this.visitedConstructs.Contains(i.FollowNode))
					{
						i.CFGFollowNode = null;
					}
				}
				this.ProcessGotoFlowConstructs(theConstruct as BlockLogicalConstruct);
			}
			else if (theConstruct is ExceptionHandlingLogicalConstruct)
			{
				this.ProcessLogicalConstruct((theConstruct as ExceptionHandlingLogicalConstruct).Try);
				if (theConstruct is TryCatchFilterLogicalConstruct)
				{
					IFilteringExceptionHandler[] handlers = (theConstruct as TryCatchFilterLogicalConstruct).Handlers;
					for (j = 0; j < (int)handlers.Length; j++)
					{
						IFilteringExceptionHandler filteringExceptionHandler = handlers[j];
						if (filteringExceptionHandler.HandlerType == FilteringExceptionHandlerType.Filter)
						{
							this.ProcessLogicalConstruct((filteringExceptionHandler as ExceptionHandlingBlockFilter).Filter);
							this.ProcessLogicalConstruct((filteringExceptionHandler as ExceptionHandlingBlockFilter).Handler);
						}
						else if (filteringExceptionHandler.HandlerType == FilteringExceptionHandlerType.Catch)
						{
							this.ProcessLogicalConstruct(filteringExceptionHandler as ExceptionHandlingBlockCatch);
						}
					}
				}
				else if (theConstruct is TryFaultLogicalConstruct)
				{
					this.ProcessLogicalConstruct((theConstruct as TryFaultLogicalConstruct).Fault);
				}
				else if (theConstruct is TryFinallyLogicalConstruct)
				{
					this.ProcessLogicalConstruct((theConstruct as TryFinallyLogicalConstruct).Finally);
				}
			}
			else if (theConstruct is IfLogicalConstruct)
			{
				IfLogicalConstruct ifLogicalConstruct = theConstruct as IfLogicalConstruct;
				this.ProcessLogicalConstruct(ifLogicalConstruct.Then);
				if (ifLogicalConstruct.Else != null)
				{
					this.ProcessLogicalConstruct(ifLogicalConstruct.Else);
				}
			}
			else if (theConstruct is LoopLogicalConstruct)
			{
				LoopLogicalConstruct loopLogicalConstruct = theConstruct as LoopLogicalConstruct;
				this.ProcessLogicalConstruct(loopLogicalConstruct.LoopBodyBlock);
				this.ProcessLogicalConstruct(loopLogicalConstruct.LoopCondition);
			}
			else if (theConstruct is SwitchLogicalConstruct)
			{
				SwitchLogicalConstruct switchLogicalConstruct = theConstruct as SwitchLogicalConstruct;
				CaseLogicalConstruct[] conditionCases = switchLogicalConstruct.ConditionCases;
				for (j = 0; j < (int)conditionCases.Length; j++)
				{
					this.ProcessLogicalConstruct(conditionCases[j]);
				}
				this.ProcessLogicalConstruct(switchLogicalConstruct.DefaultCase);
			}
			else if (theConstruct is ConditionLogicalConstruct)
			{
				this.ProcessLogicalConstruct((theConstruct as ConditionLogicalConstruct).FirstBlock);
			}
			this.visitedConstructs.Add(theConstruct);
		}
	}
}