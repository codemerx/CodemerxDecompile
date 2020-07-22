using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
	internal class StateControllerRemover
	{
		protected readonly MethodSpecificContext methodContext;

		protected readonly ControlFlowGraph theCFG;

		protected readonly InstructionBlock[] stateToStartBlock;

		private FieldDefinition stateField;

		private Telerik.JustDecompiler.Cil.SwitchData switchData;

		private InstructionBlock defaultStateEntry;

		protected VariableReference stateVariable;

		protected int firstControllerBlock;

		protected readonly HashSet<InstructionBlock> toBeRemoved;

		public HashSet<InstructionBlock> BlocksMarkedForRemoval
		{
			get
			{
				return this.toBeRemoved;
			}
		}

		public FieldDefinition StateField
		{
			get
			{
				return this.stateField;
			}
		}

		public Telerik.JustDecompiler.Cil.SwitchData SwitchData
		{
			get
			{
				return this.switchData;
			}
		}

		public StateControllerRemover(MethodSpecificContext methodContext, FieldDefinition stateField = null)
		{
			this.toBeRemoved = new HashSet<InstructionBlock>();
			base();
			this.methodContext = methodContext;
			this.theCFG = methodContext.get_ControlFlowGraph();
			this.stateField = stateField;
			this.stateToStartBlock = new InstructionBlock[(int)this.theCFG.get_Blocks().Length];
			return;
		}

		private bool CheckAndSaveStateField(FieldReference foundStateField)
		{
			V_0 = foundStateField.Resolve();
			if (this.stateField != null)
			{
				if ((object)this.stateField != (object)V_0)
				{
					return false;
				}
			}
			else
			{
				this.stateField = V_0;
			}
			return true;
		}

		protected bool ContainsStateFieldLoad(InstructionBlock theBlock)
		{
			V_0 = theBlock.get_First();
			while ((object)V_0 != (object)theBlock.get_Last())
			{
				if (V_0.get_OpCode().get_Code() == 120 && (object)((FieldReference)V_0.get_Operand()).Resolve() == (object)this.stateField)
				{
					return true;
				}
				V_0 = V_0.get_Next();
			}
			return false;
		}

		private bool ContainsStateVariableSet(InstructionBlock theBlock)
		{
			V_1 = theBlock.get_First();
			while ((object)V_1 != (object)theBlock.get_Last())
			{
				if (this.IsStloc(V_1) && this.TryGetVariableFromInstruction(V_1, out V_0) && (object)V_0 == (object)this.stateVariable)
				{
					return true;
				}
				V_1 = V_1.get_Next();
			}
			return false;
		}

		private void CreateControllerSwitchData()
		{
			V_0 = this.GetIndexOfLastNonNullElement(this.stateToStartBlock);
			stackVariable6 = V_0 + 1;
			V_0 = stackVariable6;
			V_1 = new InstructionBlock[stackVariable6];
			V_2 = 0;
			while (V_2 < V_0)
			{
				if (!InstructionBlock.op_Equality(this.stateToStartBlock[V_2], null))
				{
					V_1[V_2] = this.stateToStartBlock[V_2];
				}
				else
				{
					V_1[V_2] = this.defaultStateEntry;
				}
				V_2 = V_2 + 1;
			}
			this.switchData = new Telerik.JustDecompiler.Cil.SwitchData(null, this.defaultStateEntry, V_1);
			return;
		}

		private int GetIndexOfLastNonNullElement(InstructionBlock[] blocks)
		{
			V_0 = (int)blocks.Length - 1;
			while (V_0 >= 0)
			{
				if (InstructionBlock.op_Inequality(blocks[V_0], null))
				{
					return V_0;
				}
				V_0 = V_0 - 1;
			}
			return -1;
		}

		protected bool GetStateFieldAndVariable()
		{
			V_0 = this.theCFG.get_Blocks()[this.firstControllerBlock].get_First();
			while ((object)V_0 != (object)this.theCFG.get_Blocks()[this.firstControllerBlock].get_Last())
			{
				if (V_0.get_OpCode().get_Code() == 120)
				{
					if (!this.CheckAndSaveStateField((FieldReference)V_0.get_Operand()))
					{
						return false;
					}
					dummyVar0 = this.TryGetVariableFromInstruction(V_0.get_Next(), out this.stateVariable);
					return true;
				}
				V_0 = V_0.get_Next();
			}
			return false;
		}

		protected virtual Queue<InstructionBlock> InitializeTheTraversalQueue()
		{
			stackVariable0 = new Queue<InstructionBlock>();
			stackVariable0.Enqueue(this.theCFG.get_Blocks()[this.firstControllerBlock]);
			return stackVariable0;
		}

		protected bool IsBeqInstruction(Instruction theInstruction)
		{
			if (theInstruction.get_OpCode().get_Code() == 58)
			{
				return true;
			}
			return theInstruction.get_OpCode().get_Code() == 45;
		}

		private bool IsBneInstruction(Instruction theInstruction)
		{
			if (theInstruction.get_OpCode().get_Code() == 63)
			{
				return true;
			}
			return theInstruction.get_OpCode().get_Code() == 50;
		}

		private bool IsBrFalseInstruction(Instruction theInstruction)
		{
			if (theInstruction.get_OpCode().get_Code() == 56)
			{
				return true;
			}
			return theInstruction.get_OpCode().get_Code() == 43;
		}

		private bool IsControllerCondition(Instruction instruction)
		{
			if (this.IsBeqInstruction(instruction) || this.IsBneInstruction(instruction))
			{
				return true;
			}
			return instruction.get_OpCode().get_Code() == 68;
		}

		private bool IsDebugControllerBlock(InstructionBlock theBlock, out int stateNumber)
		{
			if ((object)theBlock.get_First() == (object)theBlock.get_Last())
			{
				stateNumber = -1;
				return false;
			}
			V_0 = theBlock.get_First().get_Next();
			if ((object)V_0 == (object)theBlock.get_Last() || V_0.get_OpCode().get_Code() != 120 || (object)((FieldReference)V_0.get_Operand()).Resolve() != (object)this.stateField)
			{
				stateNumber = -1;
				return false;
			}
			V_0 = V_0.get_Next();
			if ((object)V_0 == (object)theBlock.get_Last() || !StateMachineUtilities.TryGetOperandOfLdc(V_0, out stateNumber))
			{
				stateNumber = -1;
				return false;
			}
			V_0 = V_0.get_Next();
			if ((object)V_0 == (object)theBlock.get_Last() || V_0.get_OpCode().get_Code() != 192)
			{
				return false;
			}
			V_0 = V_0.get_Next();
			if ((object)V_0 == (object)theBlock.get_Last() || V_0.get_OpCode().get_Code() != 22)
			{
				return false;
			}
			if (theBlock.get_Last().get_OpCode().get_Code() == 57)
			{
				return true;
			}
			V_1 = theBlock.get_Last().get_OpCode();
			return V_1.get_Code() == 44;
		}

		private bool IsStateMachineControllerBlock(ref InstructionBlock theBlock, out StateControllerRemover.StateMachineControllerType controllerType, out int stateNumber)
		{
			theBlock = this.SkipBranchChain(theBlock);
			V_0 = theBlock.get_Last();
			if (!this.theCFG.get_SwitchBlocksInformation().ContainsKey(theBlock))
			{
				if (!this.IsBeqInstruction(theBlock.get_Last()))
				{
					if (!this.IsBrFalseInstruction(theBlock.get_Last()))
					{
						if (!this.IsBneInstruction(theBlock.get_Last()))
						{
							if (this.IsDebugControllerBlock(theBlock, out stateNumber))
							{
								controllerType = 3;
								return true;
							}
							controllerType = 0;
							stateNumber = 0;
							return false;
						}
						controllerType = 3;
						V_0 = theBlock.get_Last().get_Previous();
						if (!StateMachineUtilities.TryGetOperandOfLdc(V_0, out stateNumber))
						{
							return false;
						}
					}
					else
					{
						controllerType = 2;
						V_0 = theBlock.get_Last();
						stateNumber = 0;
					}
				}
				else
				{
					controllerType = 2;
					V_0 = theBlock.get_Last().get_Previous();
					if (!StateMachineUtilities.TryGetOperandOfLdc(V_0, out stateNumber))
					{
						return false;
					}
				}
			}
			else
			{
				controllerType = 1;
				if (V_0.get_Previous().get_OpCode().get_Code() == 88 || V_0.get_Previous().get_OpCode().get_Code() == 184)
				{
					V_0 = V_0.get_Previous().get_Previous();
					if (!StateMachineUtilities.TryGetOperandOfLdc(V_0, out stateNumber))
					{
						return false;
					}
				}
				else
				{
					stateNumber = 0;
				}
			}
			if (this.ContainsStateFieldLoad(theBlock) || this.TryGetVariableFromInstruction(V_0.get_Previous(), out V_1) && (object)V_1 == (object)this.stateVariable && !this.ContainsStateVariableSet(theBlock))
			{
				return true;
			}
			stateNumber = 0;
			return false;
		}

		private bool IsStloc(Instruction instruction)
		{
			if (instruction.get_OpCode().get_Code() == 204 || instruction.get_OpCode().get_Code() == 10 || instruction.get_OpCode().get_Code() == 11 || instruction.get_OpCode().get_Code() == 12 || instruction.get_OpCode().get_Code() == 13)
			{
				return true;
			}
			return instruction.get_OpCode().get_Code() == 19;
		}

		protected virtual bool IsUnconditionalBranchBlock(InstructionBlock theBlock)
		{
			return StateMachineUtilities.IsUnconditionalBranch(theBlock.get_First());
		}

		private void ReattachDefaultSuccessor(InstructionBlock initialBlock, InstructionBlock currentBlock)
		{
			if (InstructionBlock.op_Inequality(initialBlock, currentBlock))
			{
				this.RedirectNonControllerPredecessors(initialBlock, currentBlock);
				this.SwapCFGBlocks(initialBlock.get_Index(), currentBlock.get_Index());
				V_0 = 0;
				while (V_0 < (int)this.stateToStartBlock.Length)
				{
					if (InstructionBlock.op_Equality(this.stateToStartBlock[V_0], initialBlock))
					{
						this.stateToStartBlock[V_0] = currentBlock;
					}
					V_0 = V_0 + 1;
				}
			}
			return;
		}

		private void RedirectNonControllerPredecessors(InstructionBlock controllerBlock, InstructionBlock successor)
		{
			V_0 = (new List<InstructionBlock>(controllerBlock.get_Predecessors())).GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (this.toBeRemoved.Contains(V_1))
					{
						continue;
					}
					dummyVar0 = successor.get_Predecessors().Remove(controllerBlock);
					this.RedirectSuccessor(V_1, controllerBlock, successor);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private void RedirectSuccessor(InstructionBlock theBlock, InstructionBlock oldSuccessor, InstructionBlock newSuccessor)
		{
			dummyVar0 = oldSuccessor.get_Predecessors().Remove(theBlock);
			V_1 = 0;
			while (V_1 < (int)theBlock.get_Successors().Length)
			{
				if (InstructionBlock.op_Equality(theBlock.get_Successors()[V_1], oldSuccessor))
				{
					theBlock.get_Successors()[V_1] = newSuccessor;
				}
				V_1 = V_1 + 1;
			}
			dummyVar1 = newSuccessor.get_Predecessors().Add(theBlock);
			if (!this.theCFG.get_SwitchBlocksInformation().TryGetValue(theBlock, out V_0))
			{
				return;
			}
			V_2 = 0;
			while (V_2 < (int)V_0.get_OrderedCasesArray().Length)
			{
				if (InstructionBlock.op_Equality(V_0.get_OrderedCasesArray()[V_2], oldSuccessor))
				{
					V_0.get_OrderedCasesArray()[V_2] = newSuccessor;
				}
				V_2 = V_2 + 1;
			}
			if (InstructionBlock.op_Equality(V_0.get_DefaultCase(), oldSuccessor))
			{
				V_0.set_DefaultCase(newSuccessor);
			}
			return;
		}

		protected bool RemoveControllerChain()
		{
			V_0 = this.InitializeTheTraversalQueue();
			while (V_0.get_Count() > 0)
			{
				V_1 = V_0.Dequeue();
				V_2 = V_1;
				while (this.IsStateMachineControllerBlock(ref V_2, out V_4, out V_3))
				{
					switch (V_4 - 1)
					{
						case 0:
						{
							V_6 = this.theCFG.get_SwitchBlocksInformation().get_Item(V_2);
							V_7 = V_6.get_OrderedCasesArray();
							V_8 = 0;
							while (V_8 < (int)V_7.Length)
							{
								if (!this.toBeRemoved.Contains(V_7[V_8]))
								{
									V_9 = this.TryGetStateEntry(V_7[V_8], out V_5);
									switch (V_9)
									{
										case 0:
										{
											return false;
										}
										case 1:
										{
											this.stateToStartBlock[V_8 + V_3] = V_5;
											break;
										}
										case 2:
										{
											this.stateToStartBlock[V_8 + V_3] = V_5;
											V_0.Enqueue(V_5);
											break;
										}
									}
								}
								V_8 = V_8 + 1;
							}
							V_0.Enqueue(this.SkipBranchChain(V_6.get_DefaultCase()));
							break;
						}
						case 1:
						{
							V_9 = this.TryGetStateEntry(V_2.get_Successors()[0], out V_10);
							switch (V_9)
							{
								case 0:
								{
									return false;
								}
								case 1:
								{
									this.stateToStartBlock[V_3] = V_10;
									break;
								}
								case 2:
								{
									this.stateToStartBlock[V_3] = V_10;
									V_0.Enqueue(V_10);
									break;
								}
							}
							break;
						}
						case 2:
						{
							V_12 = 1;
							if (V_3 == -1)
							{
								V_3 = 0;
								V_12 = 0;
							}
							V_9 = this.TryGetStateEntry(V_2.get_Successors()[V_12], out V_11);
							if (V_9 == StateControllerRemover.ControllerTraversalSearchResult.PatternFailed)
							{
								return false;
							}
							if (V_9 - 1 > 1)
							{
								break;
							}
							this.stateToStartBlock[V_3] = V_11;
							break;
						}
					}
					dummyVar0 = this.toBeRemoved.Add(V_2);
					if (V_4 != 3)
					{
						V_2 = V_2.get_Successors()[(int)V_2.get_Successors().Length - 1];
					}
					else
					{
						V_2 = V_2.get_Successors()[0];
					}
					V_2 = this.SkipBranchChain(V_2);
				}
				if (InstructionBlock.op_Equality(this.defaultStateEntry, null))
				{
					this.defaultStateEntry = V_2;
				}
				this.ReattachDefaultSuccessor(V_1, V_2);
				while (V_0.get_Count() > 0 && this.toBeRemoved.Contains(V_0.Peek()))
				{
					dummyVar1 = V_0.Dequeue();
				}
			}
			if (this.toBeRemoved.get_Count() == 0)
			{
				return false;
			}
			this.CreateControllerSwitchData();
			return true;
		}

		public virtual bool RemoveStateMachineController()
		{
			if (!this.GetStateFieldAndVariable())
			{
				return false;
			}
			return this.RemoveControllerChain();
		}

		protected InstructionBlock SkipBranchChain(InstructionBlock initialBlock)
		{
			V_0 = initialBlock;
			while (this.IsUnconditionalBranchBlock(V_0))
			{
				V_0 = V_0.get_Successors()[0];
			}
			return V_0;
		}

		private void SwapCFGBlocks(int firstIndex, int secondIndex)
		{
			V_0 = this.theCFG.get_Blocks()[firstIndex];
			V_1 = this.theCFG.get_Blocks()[secondIndex];
			this.theCFG.get_Blocks()[firstIndex] = V_1;
			this.theCFG.get_Blocks()[secondIndex] = V_0;
			V_0.set_Index(secondIndex);
			V_1.set_Index(firstIndex);
			return;
		}

		protected virtual StateControllerRemover.ControllerTraversalSearchResult TryGetStateEntry(InstructionBlock theBlock, out InstructionBlock actualSuccessor)
		{
			actualSuccessor = this.SkipBranchChain(theBlock);
			if (this.IsControllerCondition(actualSuccessor.get_Last()))
			{
				return 2;
			}
			return 1;
		}

		protected bool TryGetVariableFromInstruction(Instruction instruction, out VariableReference varReference)
		{
			return StateMachineUtilities.TryGetVariableFromInstruction(instruction, this.methodContext.get_Body().get_Variables(), out varReference);
		}

		protected enum ControllerTraversalSearchResult
		{
			PatternFailed,
			FoundStateEntry,
			FoundControllerCandidate
		}

		private enum StateMachineControllerType
		{
			None,
			Switch,
			Condition,
			NegativeCondition
		}
	}
}