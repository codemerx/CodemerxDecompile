using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
	internal class DisposingStateControllerRemover : StateControllerRemover
	{
		private readonly FieldDefinition disposingField;

		private VariableReference returnFlagVariable;

		public DisposingStateControllerRemover(MethodSpecificContext methodContext, FieldDefinition stateField, FieldDefinition disposingField)
		{
			base(methodContext, stateField);
			this.disposingField = disposingField;
			return;
		}

		private bool CheckAndSaveReturnFlagVariable(VariableReference foundFlagVariable)
		{
			if (this.returnFlagVariable != null)
			{
				if ((object)this.returnFlagVariable != (object)foundFlagVariable)
				{
					return false;
				}
			}
			else
			{
				this.returnFlagVariable = foundFlagVariable;
			}
			return true;
		}

		private bool IsDisposingBlock(InstructionBlock theBlock)
		{
			V_0 = theBlock.get_First();
			if (V_0.get_OpCode().get_Code() != 2 || (object)V_0 == (object)theBlock.get_Last())
			{
				return false;
			}
			V_0 = V_0.get_Next();
			if (V_0.get_OpCode().get_Code() != 120 || (object)((FieldReference)V_0.get_Operand()).Resolve() != (object)this.disposingField || (object)V_0 == (object)theBlock.get_Last())
			{
				return false;
			}
			V_1 = theBlock.get_Last();
			if (V_1.get_OpCode().get_Code() == 56 || V_1.get_OpCode().get_Code() == 43 || V_1.get_OpCode().get_Code() == 57)
			{
				return true;
			}
			return V_1.get_OpCode().get_Code() == 44;
		}

		private bool IsFalseReturnBlock(InstructionBlock theBlock)
		{
			V_0 = theBlock.get_First();
			if (V_0.get_OpCode().get_Code() != 22 || (object)V_0 == (object)theBlock.get_Last())
			{
				return false;
			}
			V_0 = V_0.get_Next();
			if (!this.TryGetVariableFromInstruction(V_0, out V_1) || (object)V_0 == (object)theBlock.get_Last() || !this.CheckAndSaveReturnFlagVariable(V_1))
			{
				return false;
			}
			V_0 = V_0.get_Next();
			return StateMachineUtilities.IsUnconditionalBranch(V_0);
		}

		protected override StateControllerRemover.ControllerTraversalSearchResult TryGetStateEntry(InstructionBlock theBlock, out InstructionBlock actualSuccessor)
		{
			actualSuccessor = this.SkipBranchChain(theBlock);
			if (!this.IsDisposingBlock(actualSuccessor))
			{
				return 2;
			}
			dummyVar0 = this.toBeRemoved.Add(actualSuccessor);
			if (!this.IsFalseReturnBlock(actualSuccessor.get_Successors()[1]))
			{
				if (!this.IsFalseReturnBlock(actualSuccessor.get_Successors()[0]))
				{
					return 0;
				}
				dummyVar2 = this.toBeRemoved.Add(actualSuccessor.get_Successors()[0]);
				actualSuccessor = actualSuccessor.get_Successors()[1];
			}
			else
			{
				dummyVar1 = this.toBeRemoved.Add(actualSuccessor.get_Successors()[1]);
				actualSuccessor = actualSuccessor.get_Successors()[0];
			}
			return 1;
		}
	}
}