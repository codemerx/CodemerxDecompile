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
			if (!StateMachineUtilities.TryGetVariableFromInstruction(first, this.methodVariables, out variableReference) || variableReference != this.stateVariable)
			{
				return false;
			}
			first = first.Next;
			if (first.OpCode.Code != Code.Ldc_I4_0)
			{
				return false;
			}
			first = first.Next;
			if (first.OpCode.Code != Code.Bge && first.OpCode.Code != Code.Bge_S)
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