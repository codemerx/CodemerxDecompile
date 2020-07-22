using Mono.Cecil.Cil;
using System;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
	internal class StateMachineDoFinallyCheckRemover : StateMachineFinallyCheckRemoverBase
	{
		private VariableReference doFinallyVariable;

		public StateMachineDoFinallyCheckRemover(MethodSpecificContext methodContext)
		{
			base(methodContext);
			return;
		}

		private bool GetDoFinallyVariable()
		{
			V_0 = this.theCFG.get_Blocks()[0].get_First();
			if (V_0.get_OpCode().get_Code() != 23)
			{
				return false;
			}
			return StateMachineUtilities.TryGetVariableFromInstruction(V_0.get_Next(), this.methodVariables, out this.doFinallyVariable);
		}

		private bool IsCSharpDebugCheck(InstructionBlock theBlock, Instruction currentInstruction)
		{
			if (currentInstruction.get_OpCode().get_Code() != 22)
			{
				return false;
			}
			if (theBlock.get_Last().get_OpCode().get_Code() == 57)
			{
				return true;
			}
			V_0 = theBlock.get_Last().get_OpCode();
			return V_0.get_Code() == 44;
		}

		protected override bool IsFinallyCheckBlock(InstructionBlock theBlock)
		{
			V_0 = theBlock.get_First();
			while (V_0.get_OpCode().get_Code() == null)
			{
				if ((object)V_0 == (object)theBlock.get_Last())
				{
					return false;
				}
				V_0 = V_0.get_Next();
			}
			if ((object)V_0 == (object)theBlock.get_Last() || !StateMachineUtilities.TryGetVariableFromInstruction(V_0, this.methodVariables, out V_1) || (object)V_1 != (object)this.doFinallyVariable)
			{
				return false;
			}
			V_0 = V_0.get_Next();
			if (V_0.get_OpCode().get_Code() == 56 || V_0.get_OpCode().get_Code() == 43)
			{
				return true;
			}
			if (this.IsCSharpDebugCheck(theBlock, V_0))
			{
				return true;
			}
			return this.IsVisualBasicDebugCheck(theBlock, V_0);
		}

		private bool IsVisualBasicDebugCheck(InstructionBlock theBlock, Instruction currentInstruction)
		{
			if ((object)currentInstruction == (object)theBlock.get_Last() || !StateMachineUtilities.TryGetVariableFromInstruction(currentInstruction, this.methodVariables, out V_0))
			{
				return false;
			}
			currentInstruction = currentInstruction.get_Next();
			if ((object)currentInstruction == (object)theBlock.get_Last() || !StateMachineUtilities.TryGetVariableFromInstruction(currentInstruction, this.methodVariables, out V_1) || (object)V_0 != (object)V_1)
			{
				return false;
			}
			currentInstruction = currentInstruction.get_Next();
			if ((object)currentInstruction != (object)theBlock.get_Last())
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
			this.MarkFinallyConditionsForRemovalInternal();
			return true;
		}

		public override void MarkFinallyConditionsForRemoval(VariableReference doFinallyVariable)
		{
			this.doFinallyVariable = doFinallyVariable;
			this.MarkFinallyConditionsForRemovalInternal();
			return;
		}
	}
}