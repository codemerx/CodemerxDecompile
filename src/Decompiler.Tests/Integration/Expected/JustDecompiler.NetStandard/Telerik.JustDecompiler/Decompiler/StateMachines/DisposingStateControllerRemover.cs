using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
	internal class DisposingStateControllerRemover : StateControllerRemover
	{
		private readonly FieldDefinition disposingField;

		private VariableReference returnFlagVariable;

		public DisposingStateControllerRemover(MethodSpecificContext methodContext, FieldDefinition stateField, FieldDefinition disposingField) : base(methodContext, stateField)
		{
			this.disposingField = disposingField;
		}

		private bool CheckAndSaveReturnFlagVariable(VariableReference foundFlagVariable)
		{
			if (this.returnFlagVariable == null)
			{
				this.returnFlagVariable = foundFlagVariable;
			}
			else if ((object)this.returnFlagVariable != (object)foundFlagVariable)
			{
				return false;
			}
			return true;
		}

		private bool IsDisposingBlock(InstructionBlock theBlock)
		{
			Instruction first = theBlock.First;
			if (first.get_OpCode().get_Code() != 2 || (object)first == (object)theBlock.Last)
			{
				return false;
			}
			first = first.get_Next();
			if (first.get_OpCode().get_Code() != 120 || (object)((FieldReference)first.get_Operand()).Resolve() != (object)this.disposingField || (object)first == (object)theBlock.Last)
			{
				return false;
			}
			Instruction last = theBlock.Last;
			if (last.get_OpCode().get_Code() == 56 || last.get_OpCode().get_Code() == 43 || last.get_OpCode().get_Code() == 57)
			{
				return true;
			}
			return last.get_OpCode().get_Code() == 44;
		}

		private bool IsFalseReturnBlock(InstructionBlock theBlock)
		{
			VariableReference variableReference;
			Instruction first = theBlock.First;
			if (first.get_OpCode().get_Code() != 22 || (object)first == (object)theBlock.Last)
			{
				return false;
			}
			first = first.get_Next();
			if (!base.TryGetVariableFromInstruction(first, out variableReference) || (object)first == (object)theBlock.Last || !this.CheckAndSaveReturnFlagVariable(variableReference))
			{
				return false;
			}
			first = first.get_Next();
			return StateMachineUtilities.IsUnconditionalBranch(first);
		}

		protected override StateControllerRemover.ControllerTraversalSearchResult TryGetStateEntry(InstructionBlock theBlock, out InstructionBlock actualSuccessor)
		{
			actualSuccessor = base.SkipBranchChain(theBlock);
			if (!this.IsDisposingBlock(actualSuccessor))
			{
				return StateControllerRemover.ControllerTraversalSearchResult.FoundControllerCandidate;
			}
			this.toBeRemoved.Add(actualSuccessor);
			if (!this.IsFalseReturnBlock(actualSuccessor.Successors[1]))
			{
				if (!this.IsFalseReturnBlock(actualSuccessor.Successors[0]))
				{
					return StateControllerRemover.ControllerTraversalSearchResult.PatternFailed;
				}
				this.toBeRemoved.Add(actualSuccessor.Successors[0]);
				actualSuccessor = actualSuccessor.Successors[1];
			}
			else
			{
				this.toBeRemoved.Add(actualSuccessor.Successors[1]);
				actualSuccessor = actualSuccessor.Successors[0];
			}
			return StateControllerRemover.ControllerTraversalSearchResult.FoundStateEntry;
		}
	}
}