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
				if (InstructionBlock.op_Inequality(this.debugStateCheckBlock, null))
				{
					return true;
				}
				return this.get_BlocksMarkedForRemoval().get_Count() > this.firstControllerBlock;
			}
		}

		public AsyncStateControllerRemover(MethodSpecificContext methodContext, FieldDefinition stateField, VariableReference doFinallyVariable, AsyncStateMachineVersion version)
		{
			base(methodContext, stateField);
			this.doFinallyVariable = doFinallyVariable;
			this.version = version;
			return;
		}

		private bool BeginsWithDoFinallySet(InstructionBlock theBlock)
		{
			V_0 = theBlock.get_First();
			if ((object)V_0 == (object)theBlock.get_Last() || V_0.get_OpCode().get_Code() != 23)
			{
				return false;
			}
			if (!StateMachineUtilities.TryGetVariableFromInstruction(V_0.get_Next(), this.methodContext.get_Body().get_Variables(), out V_1))
			{
				return false;
			}
			return (object)V_1 == (object)this.doFinallyVariable;
		}

		protected override Queue<InstructionBlock> InitializeTheTraversalQueue()
		{
			V_0 = this.InitializeTheTraversalQueue();
			if (InstructionBlock.op_Equality(this.debugStateCheckBlock, null) || !this.theCFG.get_SwitchBlocksInformation().TryGetValue(this.debugStateCheckBlock, out V_1))
			{
				return V_0;
			}
			V_2 = 0;
			while (V_2 + this.stateCheckOffset < (int)V_1.get_OrderedCasesArray().Length)
			{
				this.stateToStartBlock[V_2] = this.SkipBranchChain(V_1.get_OrderedCasesArray()[V_2 + this.stateCheckOffset]);
				V_0.Enqueue(this.stateToStartBlock[V_2]);
				V_2 = V_2 + 1;
			}
			return V_0;
		}

		private bool IsDebugCheckStateBlock(InstructionBlock theBlock)
		{
			if (!this.BeginsWithDoFinallySet(theBlock) || !this.ContainsStateFieldLoad(theBlock))
			{
				return false;
			}
			V_0 = theBlock.get_Last();
			if (!this.IsBeqInstruction(V_0))
			{
				if (V_0.get_OpCode().get_Code() != 68)
				{
					return false;
				}
				V_0 = V_0.get_Previous();
				if (V_0.get_OpCode().get_Code() != 88)
				{
					return false;
				}
			}
			V_0 = V_0.get_Previous();
			if (!StateMachineUtilities.TryGetOperandOfLdc(V_0, out V_1) || V_1 > -3)
			{
				return false;
			}
			this.debugStateCheckBlock = theBlock;
			this.stateCheckOffset = -V_1;
			return true;
		}

		private bool IsDoFinallySetBlock(InstructionBlock theBlock)
		{
			if (!this.BeginsWithDoFinallySet(theBlock))
			{
				return false;
			}
			V_0 = theBlock.get_First().get_Next();
			if ((object)V_0 == (object)theBlock.get_Last())
			{
				return true;
			}
			return this.IsNopTillEnd(theBlock, V_0.get_Next());
		}

		private bool IsDummyStateControllerBlock(InstructionBlock theBlock)
		{
			if (this.version == AsyncStateMachineVersion.V1)
			{
				if (!this.ContainsStateFieldLoad(theBlock))
				{
					return false;
				}
				V_0 = theBlock.get_Last();
				V_1 = 0;
				while (V_1 < 2)
				{
					if ((object)V_0 == (object)theBlock.get_First() || V_0.get_OpCode().get_Code() != 37)
					{
						return false;
					}
					V_0 = V_0.get_Previous();
					V_1 = V_1 + 1;
				}
				dummyVar0 = this.toBeRemoved.Add(theBlock);
				return true;
			}
			V_3 = theBlock.get_First();
			if (!this.TryGetVariableFromInstruction(V_3, out V_4) || (object)V_4 != (object)this.stateVariable)
			{
				return false;
			}
			if (V_3.get_Next().get_OpCode().get_Code() != 37 || V_3.get_Next().get_Next().get_OpCode().get_Code() != null)
			{
				return false;
			}
			if ((object)V_3.get_Next().get_Next() != (object)theBlock.get_Last())
			{
				return false;
			}
			return true;
		}

		private bool IsNopBlock(InstructionBlock theBlock)
		{
			return this.IsNopTillEnd(theBlock, theBlock.get_First());
		}

		private bool IsNopTillEnd(InstructionBlock theBlock, Instruction currentInstruction)
		{
			while ((object)currentInstruction != (object)theBlock.get_Last())
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
			if (this.IsUnconditionalBranchBlock(theBlock) || this.IsDummyStateControllerBlock(theBlock))
			{
				return true;
			}
			return this.IsNopBlock(theBlock);
		}

		public override bool RemoveStateMachineController()
		{
			if (this.version != AsyncStateMachineVersion.V1)
			{
				if (this.ContainsStateFieldLoad(this.theCFG.get_Blocks()[0]))
				{
					if (!this.GetStateFieldAndVariable())
					{
						return false;
					}
					this.SkipFirstBlock();
				}
				return this.RemoveControllerChain();
			}
			if (this.IsDebugCheckStateBlock(this.theCFG.get_Blocks()[0]))
			{
				if (!this.GetStateFieldAndVariable())
				{
					return false;
				}
				this.SkipFirstBlock();
				return this.RemoveControllerChain();
			}
			if (this.IsDoFinallySetBlock(this.theCFG.get_Blocks()[0]))
			{
				this.SkipFirstBlock();
			}
			return this.RemoveStateMachineController();
		}

		private void SkipFirstBlock()
		{
			dummyVar0 = this.get_BlocksMarkedForRemoval().Add(this.theCFG.get_Blocks()[0]);
			this.firstControllerBlock = 1;
			return;
		}
	}
}