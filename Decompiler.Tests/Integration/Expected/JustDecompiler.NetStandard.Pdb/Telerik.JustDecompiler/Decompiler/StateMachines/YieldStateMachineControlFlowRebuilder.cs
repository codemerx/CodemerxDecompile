using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
	internal class YieldStateMachineControlFlowRebuilder
	{
		private readonly HashSet<InstructionBlock> yieldBreaks;

		private readonly HashSet<InstructionBlock> yieldReturns;

		private readonly HashSet<InstructionBlock> toBeRemoved;

		private readonly SwitchData switchData;

		private readonly FieldDefinition stateField;

		private readonly MethodSpecificContext moveNextMethodContext;

		private FieldDefinition currentItemField;

		private VariableReference returnFlagVariable;

		public HashSet<InstructionBlock> BlocksMarkedForRemoval
		{
			get
			{
				return this.toBeRemoved;
			}
		}

		public FieldDefinition CurrentItemField
		{
			get
			{
				return this.currentItemField;
			}
		}

		public VariableReference ReturnFlagVariable
		{
			get
			{
				return this.returnFlagVariable;
			}
		}

		public HashSet<InstructionBlock> YieldBreakBlocks
		{
			get
			{
				return this.yieldBreaks;
			}
		}

		public HashSet<InstructionBlock> YieldReturnBlocks
		{
			get
			{
				return this.yieldReturns;
			}
		}

		public YieldStateMachineControlFlowRebuilder(MethodSpecificContext moveNextMethodContext, SwitchData controllerSwitchData, FieldDefinition stateField)
		{
			this.yieldBreaks = new HashSet<InstructionBlock>();
			this.yieldReturns = new HashSet<InstructionBlock>();
			this.toBeRemoved = new HashSet<InstructionBlock>();
			base();
			this.moveNextMethodContext = moveNextMethodContext;
			this.switchData = controllerSwitchData;
			this.stateField = stateField;
			return;
		}

		private bool CheckAndSaveCurrentItemField(FieldReference foundCurrentItemFieldRef)
		{
			V_0 = foundCurrentItemFieldRef.Resolve();
			if (this.currentItemField != null)
			{
				if ((object)this.currentItemField != (object)V_0)
				{
					return false;
				}
			}
			else
			{
				this.currentItemField = V_0;
			}
			return true;
		}

		private bool CheckAndSaveReturnFlagVariable(VariableReference foundReturnFlagVariable)
		{
			if (this.returnFlagVariable != null)
			{
				if ((object)this.returnFlagVariable != (object)foundReturnFlagVariable)
				{
					return false;
				}
			}
			else
			{
				this.returnFlagVariable = foundReturnFlagVariable;
			}
			return true;
		}

		private InstructionBlock GetStateFistBlock(int state)
		{
			if (state < 0 || state >= (int)this.switchData.get_OrderedCasesArray().Length)
			{
				return this.switchData.get_DefaultCase();
			}
			return this.switchData.get_OrderedCasesArray()[state];
		}

		private bool IsFinallyMethodInvocationBlock(InstructionBlock block)
		{
			V_0 = block.get_First();
			if (V_0.get_OpCode().get_Code() != 2)
			{
				return false;
			}
			V_0 = V_0.get_Next();
			if (V_0.get_OpCode().get_Code() != 39)
			{
				return false;
			}
			V_1 = V_0;
			V_0 = V_0.get_Next();
			if (V_0.get_OpCode().get_Code() == null)
			{
				V_0 = V_0.get_Next();
			}
			if (!StateMachineUtilities.IsUnconditionalBranch(V_0))
			{
				return false;
			}
			if ((object)block.get_Last() != (object)V_0)
			{
				return false;
			}
			V_2 = V_1.get_Operand() as MethodReference;
			if (V_2 == null)
			{
				return false;
			}
			if (!V_2.get_Name().StartsWith("<>m__Finally"))
			{
				return false;
			}
			return true;
		}

		private bool MarkPredecessorsAsYieldReturns(InstructionBlock theBlock)
		{
			dummyVar0 = this.toBeRemoved.Add(theBlock);
			V_0 = (new HashSet<InstructionBlock>(theBlock.get_Predecessors())).GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if ((object)V_1.get_Last() != (object)V_1.get_First())
					{
						if (this.TryAddToYieldReturningBlocks(V_1))
						{
							continue;
						}
						V_2 = false;
						goto Label1;
					}
					else
					{
						if (StateMachineUtilities.IsUnconditionalBranch(V_1.get_Last()) && this.MarkPredecessorsAsYieldReturns(V_1))
						{
							continue;
						}
						V_2 = false;
						goto Label1;
					}
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
		Label1:
			return V_2;
		Label0:
			return true;
		}

		private bool MarkTrueReturningPredecessorsAsYieldReturn(InstructionBlock theBlock)
		{
			V_0 = (new HashSet<InstructionBlock>(theBlock.get_Predecessors())).GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if ((object)V_1.get_Last() != (object)V_1.get_First())
					{
						V_2 = null;
						V_3 = V_1.get_Last();
						if (StateMachineUtilities.IsUnconditionalBranch(V_3))
						{
							V_3 = V_3.get_Previous();
						}
						if (!this.IsFinallyMethodInvocationBlock(V_1))
						{
							if (!this.TryGetVariableFromInstruction(V_3, out V_2) || !this.CheckAndSaveReturnFlagVariable(V_2) || !StateMachineUtilities.TryGetOperandOfLdc(V_3.get_Previous(), out V_4))
							{
								V_5 = false;
								goto Label1;
							}
							else
							{
								if (V_4 != 1)
								{
									if (V_4 == 0)
									{
										continue;
									}
									V_5 = false;
									goto Label1;
								}
								else
								{
									if (this.TryAddToYieldReturningBlocks(V_1))
									{
										continue;
									}
									V_5 = false;
									goto Label1;
								}
							}
						}
						else
						{
							if (this.MarkTrueReturningPredecessorsAsYieldReturn(V_1))
							{
								continue;
							}
							V_5 = false;
							goto Label1;
						}
					}
					else
					{
						if (!StateMachineUtilities.IsUnconditionalBranch(V_1.get_Last()))
						{
							V_5 = false;
							goto Label1;
						}
						else
						{
							dummyVar0 = this.toBeRemoved.Add(V_1);
							if (this.MarkTrueReturningPredecessorsAsYieldReturn(V_1))
							{
								continue;
							}
							V_5 = false;
							goto Label1;
						}
					}
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
		Label1:
			return V_5;
		Label0:
			return true;
		}

		public bool ProcessEndBlocks()
		{
			V_0 = this.moveNextMethodContext.get_ControlFlowGraph().get_Blocks();
			V_1 = 0;
			while (V_1 < (int)V_0.Length)
			{
				V_2 = V_0[V_1];
				if (V_2.get_Successors().Length == 0 && V_2.get_Last().get_OpCode().get_Code() == 41 && !this.TryProcessEndBlock(V_2))
				{
					return false;
				}
				V_1 = V_1 + 1;
			}
			return true;
		}

		private bool TryAddToYieldReturningBlocks(InstructionBlock theBlock)
		{
			if (this.TryGetNextStateNumber(theBlock, out V_0) != 1)
			{
				return false;
			}
			dummyVar0 = this.yieldReturns.Add(theBlock);
			V_1 = this.GetStateFistBlock(V_0);
			stackVariable15 = new InstructionBlock[1];
			stackVariable15[0] = V_1;
			theBlock.set_Successors(stackVariable15);
			return true;
		}

		private YieldStateMachineControlFlowRebuilder.NextStateNumberSearchResult TryGetNextStateNumber(InstructionBlock theBlock, out int newStateNumber)
		{
			V_0 = theBlock.get_Last();
			while ((object)V_0 != (object)theBlock.get_First())
			{
				if (V_0.get_OpCode().get_Code() == 122 && (object)this.stateField == (object)((FieldReference)V_0.get_Operand()).Resolve())
				{
					V_0 = V_0.get_Previous();
					if (!StateMachineUtilities.TryGetOperandOfLdc(V_0, out newStateNumber))
					{
						return 0;
					}
					V_0 = V_0.get_Previous().get_Previous();
					if (V_0.get_OpCode().get_Code() == 122 && this.CheckAndSaveCurrentItemField((FieldReference)V_0.get_Operand()))
					{
						return 1;
					}
					return 0;
				}
				V_0 = V_0.get_Previous();
			}
			newStateNumber = 0;
			return 2;
		}

		private bool TryGetVariableFromInstruction(Instruction instruction, out VariableReference varReference)
		{
			return StateMachineUtilities.TryGetVariableFromInstruction(instruction, this.moveNextMethodContext.get_Variables(), out varReference);
		}

		private bool TryProcessEndBlock(InstructionBlock theBlock)
		{
			V_0 = null;
			V_1 = theBlock.get_Last().get_Previous();
			if (!StateMachineUtilities.TryGetOperandOfLdc(V_1, out V_2))
			{
				if (!this.TryGetVariableFromInstruction(V_1, out V_0) || !this.CheckAndSaveReturnFlagVariable(V_0))
				{
					return false;
				}
				dummyVar2 = this.yieldBreaks.Add(theBlock);
				return this.MarkTrueReturningPredecessorsAsYieldReturn(theBlock);
			}
			if (V_2 != 0)
			{
				if (V_2 != 1)
				{
					return false;
				}
				switch (this.TryGetNextStateNumber(theBlock, out V_3))
				{
					case 0:
					{
						return false;
					}
					case 1:
					{
						dummyVar1 = this.yieldReturns.Add(theBlock);
						V_4 = this.GetStateFistBlock(V_3);
						stackVariable41 = new InstructionBlock[1];
						stackVariable41[0] = V_4;
						theBlock.set_Successors(stackVariable41);
						break;
					}
					case 2:
					{
						return this.MarkPredecessorsAsYieldReturns(theBlock);
					}
				}
			}
			else
			{
				dummyVar0 = this.yieldBreaks.Add(theBlock);
			}
			return true;
		}

		private enum NextStateNumberSearchResult
		{
			PatternFailed,
			StateNumberFound,
			StateWasNotSet
		}
	}
}