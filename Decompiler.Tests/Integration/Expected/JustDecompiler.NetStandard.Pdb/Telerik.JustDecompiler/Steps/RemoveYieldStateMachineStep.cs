using System;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.StateMachines;

namespace Telerik.JustDecompiler.Steps
{
	internal class RemoveYieldStateMachineStep : BaseStateMachineRemoverStep
	{
		public RemoveYieldStateMachineStep()
		{
			base();
			return;
		}

		protected override bool ProcessCFG()
		{
			V_0 = new StateMachineDisposeAnalyzer(this.moveNextMethodContext.get_Method());
			V_2 = V_0.ProcessDisposeMethod();
			if (V_2 != 1)
			{
				if (V_2 != 2)
				{
					return false;
				}
				V_1 = new DisposingStateControllerRemover(this.moveNextMethodContext, V_0.get_StateField(), V_0.get_DisposingField());
				V_6 = new StateMachineDoFinallyCheckRemover(this.moveNextMethodContext);
				if (!V_6.MarkFinallyConditionsForRemoval())
				{
					return false;
				}
				this.toBeRemoved.UnionWith(V_6.get_BlocksMarkedForRemoval());
			}
			else
			{
				V_1 = new StateControllerRemover(this.moveNextMethodContext, null);
			}
			if (!V_1.RemoveStateMachineController())
			{
				return false;
			}
			this.toBeRemoved.UnionWith(V_1.get_BlocksMarkedForRemoval());
			V_3 = V_1.get_SwitchData();
			V_4 = new YieldStateMachineControlFlowRebuilder(this.moveNextMethodContext, V_3, V_1.get_StateField());
			if (!V_4.ProcessEndBlocks())
			{
				return false;
			}
			this.toBeRemoved.UnionWith(V_4.get_BlocksMarkedForRemoval());
			if (!(new StateMachineCFGCleaner(this.theCFG, V_3, V_3.get_OrderedCasesArray()[0])).CleanUpTheCFG(this.toBeRemoved))
			{
				return false;
			}
			V_5 = new YieldFieldsInformation(V_1.get_StateField(), V_4.get_CurrentItemField(), V_4.get_ReturnFlagVariable());
			this.moveNextMethodContext.set_YieldData(new YieldData(V_2, V_4.get_YieldReturnBlocks(), V_4.get_YieldBreakBlocks(), V_5, V_0.get_YieldsExceptionData()));
			return true;
		}
	}
}