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
			if ((object)first == (object)theBlock.Last || first.get_OpCode().get_Code() != 23)
			{
				return false;
			}
			if (!StateMachineUtilities.TryGetVariableFromInstruction(first.get_Next(), this.methodContext.Body.get_Variables(), out variableReference))
			{
				return false;
			}
			return (object)variableReference == (object)this.doFinallyVariable;
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
				if (last.get_OpCode().get_Code() != 68)
				{
					return false;
				}
				last = last.get_Previous();
				if (last.get_OpCode().get_Code() != 88)
				{
					return false;
				}
			}
			last = last.get_Previous();
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
			Instruction next = theBlock.First.get_Next();
			if ((object)next == (object)theBlock.Last)
			{
				return true;
			}
			return this.IsNopTillEnd(theBlock, next.get_Next());
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
					if ((object)last == (object)theBlock.First || last.get_OpCode().get_Code() != 37)
					{
						return false;
					}
					last = last.get_Previous();
				}
				this.toBeRemoved.Add(theBlock);
				return true;
			}
			Instruction first = theBlock.First;
			if (!base.TryGetVariableFromInstruction(first, out variableReference) || (object)variableReference != (object)this.stateVariable)
			{
				return false;
			}
			if (first.get_Next().get_OpCode().get_Code() != 37 || first.get_Next().get_Next().get_OpCode().get_Code() != null)
			{
				return false;
			}
			if ((object)first.get_Next().get_Next() != (object)theBlock.Last)
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
			while ((object)currentInstruction != (object)theBlock.Last)
			{
				if (currentInstruction.get_OpCode().get_Code() != null)
				{
					return false;
				}
				currentInstruction = currentInstruction.get_Next();
			}
			return currentInstruction.get_OpCode().get_Code() == 0;
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