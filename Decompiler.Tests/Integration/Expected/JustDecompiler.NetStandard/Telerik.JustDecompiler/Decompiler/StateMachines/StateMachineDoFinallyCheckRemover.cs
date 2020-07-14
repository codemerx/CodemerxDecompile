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
			if (first.get_OpCode().get_Code() != 23)
			{
				return false;
			}
			return StateMachineUtilities.TryGetVariableFromInstruction(first.get_Next(), this.methodVariables, out this.doFinallyVariable);
		}

		private bool IsCSharpDebugCheck(InstructionBlock theBlock, Instruction currentInstruction)
		{
			if (currentInstruction.get_OpCode().get_Code() != 22)
			{
				return false;
			}
			if (theBlock.Last.get_OpCode().get_Code() == 57)
			{
				return true;
			}
			return theBlock.Last.get_OpCode().get_Code() == 44;
		}

		protected override bool IsFinallyCheckBlock(InstructionBlock theBlock)
		{
			Instruction i;
			VariableReference variableReference;
			for (i = theBlock.First; i.get_OpCode().get_Code() == null; i = i.get_Next())
			{
				if ((object)i == (object)theBlock.Last)
				{
					return false;
				}
			}
			if ((object)i == (object)theBlock.Last || !StateMachineUtilities.TryGetVariableFromInstruction(i, this.methodVariables, out variableReference) || (object)variableReference != (object)this.doFinallyVariable)
			{
				return false;
			}
			i = i.get_Next();
			if (i.get_OpCode().get_Code() == 56 || i.get_OpCode().get_Code() == 43)
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
			if ((object)currentInstruction == (object)theBlock.Last || !StateMachineUtilities.TryGetVariableFromInstruction(currentInstruction, this.methodVariables, out variableReference))
			{
				return false;
			}
			currentInstruction = currentInstruction.get_Next();
			if ((object)currentInstruction == (object)theBlock.Last || !StateMachineUtilities.TryGetVariableFromInstruction(currentInstruction, this.methodVariables, out variableReference1) || (object)variableReference != (object)variableReference1)
			{
				return false;
			}
			currentInstruction = currentInstruction.get_Next();
			if ((object)currentInstruction != (object)theBlock.Last)
			{
				return false;
			}
			if (currentInstruction.get_OpCode().get_Code() == 56)
			{
				return true;
			}
			return currentInstruction.get_OpCode().get_Code() == 43;
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