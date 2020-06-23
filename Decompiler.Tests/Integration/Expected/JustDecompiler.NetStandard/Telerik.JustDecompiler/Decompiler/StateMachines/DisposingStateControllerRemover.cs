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
			else if (this.returnFlagVariable != foundFlagVariable)
			{
				return false;
			}
			return true;
		}

		private bool IsDisposingBlock(InstructionBlock theBlock)
		{
			Instruction first = theBlock.First;
			if (first.OpCode.Code != Code.Ldarg_0 || first == theBlock.Last)
			{
				return false;
			}
			first = first.Next;
			if (first.OpCode.Code != Code.Ldfld || ((FieldReference)first.Operand).Resolve() != this.disposingField || first == theBlock.Last)
			{
				return false;
			}
			Instruction last = theBlock.Last;
			if (last.OpCode.Code == Code.Brfalse || last.OpCode.Code == Code.Brfalse_S || last.OpCode.Code == Code.Brtrue)
			{
				return true;
			}
			return last.OpCode.Code == Code.Brtrue_S;
		}

		private bool IsFalseReturnBlock(InstructionBlock theBlock)
		{
			VariableReference variableReference;
			Instruction first = theBlock.First;
			if (first.OpCode.Code != Code.Ldc_I4_0 || first == theBlock.Last)
			{
				return false;
			}
			first = first.Next;
			if (!base.TryGetVariableFromInstruction(first, out variableReference) || first == theBlock.Last || !this.CheckAndSaveReturnFlagVariable(variableReference))
			{
				return false;
			}
			first = first.Next;
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