using Mono.Cecil.Cil;
using System;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
	internal class StateMachineFinallyStateCheckRemover : StateMachineFinallyCheckRemoverBase
	{
		private VariableReference stateVariable;

		public StateMachineFinallyStateCheckRemover(MethodSpecificContext moveNextMethodContext)
		{
			base(moveNextMethodContext);
			return;
		}

		protected override bool IsFinallyCheckBlock(InstructionBlock finallyEntry)
		{
			V_0 = finallyEntry.get_First();
			if (!StateMachineUtilities.TryGetVariableFromInstruction(V_0, this.methodVariables, out V_1) || (object)V_1 != (object)this.stateVariable)
			{
				return false;
			}
			V_0 = V_0.get_Next();
			if (V_0.get_OpCode().get_Code() != 22)
			{
				return false;
			}
			V_0 = V_0.get_Next();
			if (V_0.get_OpCode().get_Code() != 59 && V_0.get_OpCode().get_Code() != 46)
			{
				return false;
			}
			return true;
		}

		public override void MarkFinallyConditionsForRemoval(VariableReference stateVariable)
		{
			this.stateVariable = stateVariable;
			this.MarkFinallyConditionsForRemovalInternal();
			return;
		}
	}
}