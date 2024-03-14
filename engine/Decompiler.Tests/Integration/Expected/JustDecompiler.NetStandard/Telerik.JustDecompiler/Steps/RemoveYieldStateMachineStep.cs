using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.StateMachines;

namespace Telerik.JustDecompiler.Steps
{
	internal class RemoveYieldStateMachineStep : BaseStateMachineRemoverStep
	{
		public RemoveYieldStateMachineStep()
		{
		}

		protected override bool ProcessCFG()
		{
			StateControllerRemover disposingStateControllerRemover;
			StateMachineDisposeAnalyzer stateMachineDisposeAnalyzer = new StateMachineDisposeAnalyzer(this.moveNextMethodContext.Method);
			YieldStateMachineVersion yieldStateMachineVersion = stateMachineDisposeAnalyzer.ProcessDisposeMethod();
			if (yieldStateMachineVersion != YieldStateMachineVersion.V1)
			{
				if (yieldStateMachineVersion != YieldStateMachineVersion.V2)
				{
					return false;
				}
				disposingStateControllerRemover = new DisposingStateControllerRemover(this.moveNextMethodContext, stateMachineDisposeAnalyzer.StateField, stateMachineDisposeAnalyzer.DisposingField);
				StateMachineDoFinallyCheckRemover stateMachineDoFinallyCheckRemover = new StateMachineDoFinallyCheckRemover(this.moveNextMethodContext);
				if (!stateMachineDoFinallyCheckRemover.MarkFinallyConditionsForRemoval())
				{
					return false;
				}
				this.toBeRemoved.UnionWith(stateMachineDoFinallyCheckRemover.BlocksMarkedForRemoval);
			}
			else
			{
				disposingStateControllerRemover = new StateControllerRemover(this.moveNextMethodContext, null);
			}
			if (!disposingStateControllerRemover.RemoveStateMachineController())
			{
				return false;
			}
			this.toBeRemoved.UnionWith(disposingStateControllerRemover.BlocksMarkedForRemoval);
			SwitchData switchData = disposingStateControllerRemover.SwitchData;
			YieldStateMachineControlFlowRebuilder yieldStateMachineControlFlowRebuilder = new YieldStateMachineControlFlowRebuilder(this.moveNextMethodContext, switchData, disposingStateControllerRemover.StateField);
			if (!yieldStateMachineControlFlowRebuilder.ProcessEndBlocks())
			{
				return false;
			}
			this.toBeRemoved.UnionWith(yieldStateMachineControlFlowRebuilder.BlocksMarkedForRemoval);
			if (!(new StateMachineCFGCleaner(this.theCFG, switchData, switchData.OrderedCasesArray[0])).CleanUpTheCFG(this.toBeRemoved))
			{
				return false;
			}
			YieldFieldsInformation yieldFieldsInformation = new YieldFieldsInformation(disposingStateControllerRemover.StateField, yieldStateMachineControlFlowRebuilder.CurrentItemField, yieldStateMachineControlFlowRebuilder.ReturnFlagVariable);
			this.moveNextMethodContext.YieldData = new YieldData(yieldStateMachineVersion, yieldStateMachineControlFlowRebuilder.YieldReturnBlocks, yieldStateMachineControlFlowRebuilder.YieldBreakBlocks, yieldFieldsInformation, stateMachineDisposeAnalyzer.YieldsExceptionData);
			return true;
		}
	}
}