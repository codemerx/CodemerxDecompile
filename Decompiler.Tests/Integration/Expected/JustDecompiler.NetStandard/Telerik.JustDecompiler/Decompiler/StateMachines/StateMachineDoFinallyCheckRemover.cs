using Mono.Cecil.Cil;
using System;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
	internal class StateMachineDoFinallyCheckRemover : StateMachineFinallyCheckRemoverBase
	{
		private VariableReference doFinallyVariable;

		public StateMachineDoFinallyCheckRemover(MethodSpecificContext methodContext) : base(methodContext)
		{
		}

		private bool GetDoFinallyVariable()
		{
			Instruction first = this.theCFG.Blocks[0].First;
			if (first.OpCode.Code != Code.Ldc_I4_1)
			{
				return false;
			}
			return StateMachineUtilities.TryGetVariableFromInstruction(first.Next, this.methodVariables, out this.doFinallyVariable);
		}

		private bool IsCSharpDebugCheck(InstructionBlock theBlock, Instruction currentInstruction)
		{
			if (currentInstruction.OpCode.Code != Code.Ldc_I4_0)
			{
				return false;
			}
			if (theBlock.Last.OpCode.Code == Code.Brtrue)
			{
				return true;
			}
			return theBlock.Last.OpCode.Code == Code.Brtrue_S;
		}

		protected override bool IsFinallyCheckBlock(InstructionBlock theBlock)
		{
			Instruction i;
			VariableReference variableReference;
			for (i = theBlock.First; i.OpCode.Code == Code.Nop; i = i.Next)
			{
				if (i == theBlock.Last)
				{
					return false;
				}
			}
			if (i == theBlock.Last || !StateMachineUtilities.TryGetVariableFromInstruction(i, this.methodVariables, out variableReference) || variableReference != this.doFinallyVariable)
			{
				return false;
			}
			i = i.Next;
			if (i.OpCode.Code == Code.Brfalse || i.OpCode.Code == Code.Brfalse_S)
			{
				return true;
			}
			if (this.IsCSharpDebugCheck(theBlock, i))
			{
				return true;
			}
			return this.IsVisualBasicDebugCheck(theBlock, i);
		}

		private bool IsVisualBasicDebugCheck(InstructionBlock theBlock, Instruction currentInstruction)
		{
			VariableReference variableReference;
			VariableReference variableReference1;
			if (currentInstruction == theBlock.Last || !StateMachineUtilities.TryGetVariableFromInstruction(currentInstruction, this.methodVariables, out variableReference))
			{
				return false;
			}
			currentInstruction = currentInstruction.Next;
			if (currentInstruction == theBlock.Last || !StateMachineUtilities.TryGetVariableFromInstruction(currentInstruction, this.methodVariables, out variableReference1) || variableReference != variableReference1)
			{
				return false;
			}
			currentInstruction = currentInstruction.Next;
			if (currentInstruction != theBlock.Last)
			{
				return false;
			}
			if (currentInstruction.OpCode.Code == Code.Brfalse)
			{
				return true;
			}
			return currentInstruction.OpCode.Code == Code.Brfalse_S;
		}

		public bool MarkFinallyConditionsForRemoval()
		{
			if (!this.GetDoFinallyVariable())
			{
				return false;
			}
			base.MarkFinallyConditionsForRemovalInternal();
			return true;
		}

		public override void MarkFinallyConditionsForRemoval(VariableReference doFinallyVariable)
		{
			this.doFinallyVariable = doFinallyVariable;
			base.MarkFinallyConditionsForRemovalInternal();
		}
	}
}