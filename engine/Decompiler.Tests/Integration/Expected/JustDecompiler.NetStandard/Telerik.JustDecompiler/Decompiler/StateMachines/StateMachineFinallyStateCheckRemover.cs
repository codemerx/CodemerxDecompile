using Mono.Cecil.Cil;
using System;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
	internal class StateMachineFinallyStateCheckRemover : StateMachineFinallyCheckRemoverBase
	{
		private VariableReference stateVariable;

		public StateMachineFinallyStateCheckRemover(MethodSpecificContext moveNextMethodContext) : base(moveNextMethodContext)
		{
		}

		protected override bool IsFinallyCheckBlock(InstructionBlock finallyEntry)
		{
			VariableReference variableReference;
			Instruction first = finallyEntry.First;
			if (!StateMachineUtilities.TryGetVariableFromInstruction(first, this.methodVariables, out variableReference) || (object)variableReference != (object)this.stateVariable)
			{
				return false;
			}
			first = first.get_Next();
			if (first.get_OpCode().get_Code() != 22)
			{
				return false;
			}
			first = first.get_Next();
			if (first.get_OpCode().get_Code() != 59 && first.get_OpCode().get_Code() != 46)
			{
				return false;
			}
			return true;
		}

		public override void MarkFinallyConditionsForRemoval(VariableReference stateVariable)
		{
			this.stateVariable = stateVariable;
			base.MarkFinallyConditionsForRemovalInternal();
		}
	}
}