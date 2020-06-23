using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
	internal class AsyncStateControllerRemover : StateControllerRemover
	{
		private readonly VariableReference doFinallyVariable;

		private readonly AsyncStateMachineVersion version;

		private InstructionBlock debugStateCheckBlock;

		private int stateCheckOffset;

		public bool FoundControllerBlocks
		{
			get
			{
				if (this.debugStateCheckBlock != null)
				{
					return true;
				}
				return base.BlocksMarkedForRemoval.Count > this.firstControllerBlock;
			}
		}

		public AsyncStateControllerRemover(MethodSpecificContext methodContext, FieldDefinition stateField, VariableReference doFinallyVariable, AsyncStateMachineVersion version) : base(methodContext, stateField)
		{
			this.doFinallyVariable = doFinallyVariable;
			this.version = version;
		}

		private bool BeginsWithDoFinallySet(InstructionBlock theBlock)
		{
			VariableReference variableReference;
			Instruction first = theBlock.First;
			if (first == theBlock.Last || first.OpCode.Code != Code.Ldc_I4_1)
			{
				return false;
			}
			if (!StateMachineUtilities.TryGetVariableFromInstruction(first.Next, this.methodContext.Body.Variables, out variableReference))
			{
				return false;
			}
			return variableReference == this.doFinallyVariable;
		}

		protected override Queue<InstructionBlock> InitializeTheTraversalQueue()
		{
			Telerik.JustDecompiler.Cil.SwitchData switchDatum;
			Queue<InstructionBlock> instructionBlocks = base.InitializeTheTraversalQueue();
			if (this.debugStateCheckBlock == null || !this.theCFG.SwitchBlocksInformation.TryGetValue(this.debugStateCheckBlock, out switchDatum))
			{
				return instructionBlocks;
			}
			for (int i = 0; i + this.stateCheckOffset < (int)switchDatum.OrderedCasesArray.Length; i++)
			{
				this.stateToStartBlock[i] = base.SkipBranchChain(switchDatum.OrderedCasesArray[i + this.stateCheckOffset]);
				instructionBlocks.Enqueue(this.stateToStartBlock[i]);
			}
			return instructionBlocks;
		}

		private bool IsDebugCheckStateBlock(InstructionBlock theBlock)
		{
			int num;
			if (!this.BeginsWithDoFinallySet(theBlock) || !base.ContainsStateFieldLoad(theBlock))
			{
				return false;
			}
			Instruction last = theBlock.Last;
			if (!base.IsBeqInstruction(last))
			{
				if (last.OpCode.Code != Code.Switch)
				{
					return false;
				}
				last = last.Previous;
				if (last.OpCode.Code != Code.Sub)
				{
					return false;
				}
			}
			last = last.Previous;
			if (!StateMachineUtilities.TryGetOperandOfLdc(last, out num) || num > -3)
			{
				return false;
			}
			this.debugStateCheckBlock = theBlock;
			this.stateCheckOffset = -num;
			return true;
		}

		private bool IsDoFinallySetBlock(InstructionBlock theBlock)
		{
			if (!this.BeginsWithDoFinallySet(theBlock))
			{
				return false;
			}
			Instruction next = theBlock.First.Next;
			if (next == theBlock.Last)
			{
				return true;
			}
			return this.IsNopTillEnd(theBlock, next.Next);
		}

		private bool IsDummyStateControllerBlock(InstructionBlock theBlock)
		{
			VariableReference variableReference;
			if (this.version == AsyncStateMachineVersion.V1)
			{
				if (!base.ContainsStateFieldLoad(theBlock))
				{
					return false;
				}
				Instruction last = theBlock.Last;
				for (int i = 0; i < 2; i++)
				{
					if (last == theBlock.First || last.OpCode.Code != Code.Pop)
					{
						return false;
					}
					last = last.Previous;
				}
				this.toBeRemoved.Add(theBlock);
				return true;
			}
			Instruction first = theBlock.First;
			if (!base.TryGetVariableFromInstruction(first, out variableReference) || variableReference != this.stateVariable)
			{
				return false;
			}
			if (first.Next.OpCode.Code != Code.Pop || first.Next.Next.OpCode.Code != Code.Nop)
			{
				return false;
			}
			if (first.Next.Next != theBlock.Last)
			{
				return false;
			}
			return true;
		}

		private bool IsNopBlock(InstructionBlock theBlock)
		{
			return this.IsNopTillEnd(theBlock, theBlock.First);
		}

		private bool IsNopTillEnd(InstructionBlock theBlock, Instruction currentInstruction)
		{
			while (currentInstruction != theBlock.Last)
			{
				if (currentInstruction.OpCode.Code != Code.Nop)
				{
					return false;
				}
				currentInstruction = currentInstruction.Next;
			}
			return currentInstruction.OpCode.Code == Code.Nop;
		}

		protected override bool IsUnconditionalBranchBlock(InstructionBlock theBlock)
		{
			if (base.IsUnconditionalBranchBlock(theBlock) || this.IsDummyStateControllerBlock(theBlock))
			{
				return true;
			}
			return this.IsNopBlock(theBlock);
		}

		public override bool RemoveStateMachineController()
		{
			if (this.version != AsyncStateMachineVersion.V1)
			{
				if (base.ContainsStateFieldLoad(this.theCFG.Blocks[0]))
				{
					if (!base.GetStateFieldAndVariable())
					{
						return false;
					}
					this.SkipFirstBlock();
				}
				return base.RemoveControllerChain();
			}
			if (this.IsDebugCheckStateBlock(this.theCFG.Blocks[0]))
			{
				if (!base.GetStateFieldAndVariable())
				{
					return false;
				}
				this.SkipFirstBlock();
				return base.RemoveControllerChain();
			}
			if (this.IsDoFinallySetBlock(this.theCFG.Blocks[0]))
			{
				this.SkipFirstBlock();
			}
			return base.RemoveStateMachineController();
		}

		private void SkipFirstBlock()
		{
			base.BlocksMarkedForRemoval.Add(this.theCFG.Blocks[0]);
			this.firstControllerBlock = 1;
		}
	}
}