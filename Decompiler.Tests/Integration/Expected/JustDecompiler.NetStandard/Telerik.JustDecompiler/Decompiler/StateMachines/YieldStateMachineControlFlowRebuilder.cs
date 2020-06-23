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
		private readonly HashSet<InstructionBlock> yieldBreaks = new HashSet<InstructionBlock>();

		private readonly HashSet<InstructionBlock> yieldReturns = new HashSet<InstructionBlock>();

		private readonly HashSet<InstructionBlock> toBeRemoved = new HashSet<InstructionBlock>();

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
			this.moveNextMethodContext = moveNextMethodContext;
			this.switchData = controllerSwitchData;
			this.stateField = stateField;
		}

		private bool CheckAndSaveCurrentItemField(FieldReference foundCurrentItemFieldRef)
		{
			FieldDefinition fieldDefinition = foundCurrentItemFieldRef.Resolve();
			if (this.currentItemField == null)
			{
				this.currentItemField = fieldDefinition;
			}
			else if (this.currentItemField != fieldDefinition)
			{
				return false;
			}
			return true;
		}

		private bool CheckAndSaveReturnFlagVariable(VariableReference foundReturnFlagVariable)
		{
			if (this.returnFlagVariable == null)
			{
				this.returnFlagVariable = foundReturnFlagVariable;
			}
			else if (this.returnFlagVariable != foundReturnFlagVariable)
			{
				return false;
			}
			return true;
		}

		private InstructionBlock GetStateFistBlock(int state)
		{
			if (state < 0 || state >= (int)this.switchData.OrderedCasesArray.Length)
			{
				return this.switchData.DefaultCase;
			}
			return this.switchData.OrderedCasesArray[state];
		}

		private bool IsFinallyMethodInvocationBlock(InstructionBlock block)
		{
			Instruction first = block.First;
			if (first.OpCode.Code != Code.Ldarg_0)
			{
				return false;
			}
			first = first.Next;
			if (first.OpCode.Code != Code.Call)
			{
				return false;
			}
			Instruction instruction = first;
			first = first.Next;
			if (first.OpCode.Code == Code.Nop)
			{
				first = first.Next;
			}
			if (!StateMachineUtilities.IsUnconditionalBranch(first))
			{
				return false;
			}
			if (block.Last != first)
			{
				return false;
			}
			MethodReference operand = instruction.Operand as MethodReference;
			if (operand == null)
			{
				return false;
			}
			if (!operand.Name.StartsWith("<>m__Finally"))
			{
				return false;
			}
			return true;
		}

		private bool MarkPredecessorsAsYieldReturns(InstructionBlock theBlock)
		{
			bool flag;
			this.toBeRemoved.Add(theBlock);
			HashSet<InstructionBlock>.Enumerator enumerator = (new HashSet<InstructionBlock>(theBlock.Predecessors)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					InstructionBlock current = enumerator.Current;
					if (current.Last != current.First)
					{
						if (this.TryAddToYieldReturningBlocks(current))
						{
							continue;
						}
						flag = false;
						return flag;
					}
					else
					{
						if (StateMachineUtilities.IsUnconditionalBranch(current.Last) && this.MarkPredecessorsAsYieldReturns(current))
						{
							continue;
						}
						flag = false;
						return flag;
					}
				}
				return true;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private bool MarkTrueReturningPredecessorsAsYieldReturn(InstructionBlock theBlock)
		{
			int num;
			bool flag;
			HashSet<InstructionBlock>.Enumerator enumerator = (new HashSet<InstructionBlock>(theBlock.Predecessors)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					InstructionBlock current = enumerator.Current;
					if (current.Last != current.First)
					{
						VariableReference variableReference = null;
						Instruction last = current.Last;
						if (StateMachineUtilities.IsUnconditionalBranch(last))
						{
							last = last.Previous;
						}
						if (this.IsFinallyMethodInvocationBlock(current))
						{
							if (this.MarkTrueReturningPredecessorsAsYieldReturn(current))
							{
								continue;
							}
							flag = false;
							return flag;
						}
						else if (!this.TryGetVariableFromInstruction(last, out variableReference) || !this.CheckAndSaveReturnFlagVariable(variableReference) || !StateMachineUtilities.TryGetOperandOfLdc(last.Previous, out num))
						{
							flag = false;
							return flag;
						}
						else if (num != 1)
						{
							if (num == 0)
							{
								continue;
							}
							flag = false;
							return flag;
						}
						else
						{
							if (this.TryAddToYieldReturningBlocks(current))
							{
								continue;
							}
							flag = false;
							return flag;
						}
					}
					else if (!StateMachineUtilities.IsUnconditionalBranch(current.Last))
					{
						flag = false;
						return flag;
					}
					else
					{
						this.toBeRemoved.Add(current);
						if (this.MarkTrueReturningPredecessorsAsYieldReturn(current))
						{
							continue;
						}
						flag = false;
						return flag;
					}
				}
				return true;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		public bool ProcessEndBlocks()
		{
			InstructionBlock[] blocks = this.moveNextMethodContext.ControlFlowGraph.Blocks;
			for (int i = 0; i < (int)blocks.Length; i++)
			{
				InstructionBlock instructionBlocks = blocks[i];
				if (instructionBlocks.Successors.Length == 0 && instructionBlocks.Last.OpCode.Code == Code.Ret && !this.TryProcessEndBlock(instructionBlocks))
				{
					return false;
				}
			}
			return true;
		}

		private bool TryAddToYieldReturningBlocks(InstructionBlock theBlock)
		{
			int num;
			if (this.TryGetNextStateNumber(theBlock, out num) != YieldStateMachineControlFlowRebuilder.NextStateNumberSearchResult.StateNumberFound)
			{
				return false;
			}
			this.yieldReturns.Add(theBlock);
			InstructionBlock stateFistBlock = this.GetStateFistBlock(num);
			theBlock.Successors = new InstructionBlock[] { stateFistBlock };
			return true;
		}

		private YieldStateMachineControlFlowRebuilder.NextStateNumberSearchResult TryGetNextStateNumber(InstructionBlock theBlock, out int newStateNumber)
		{
			for (Instruction i = theBlock.Last; i != theBlock.First; i = i.Previous)
			{
				if (i.OpCode.Code == Code.Stfld && this.stateField == ((FieldReference)i.Operand).Resolve())
				{
					i = i.Previous;
					if (!StateMachineUtilities.TryGetOperandOfLdc(i, out newStateNumber))
					{
						return YieldStateMachineControlFlowRebuilder.NextStateNumberSearchResult.PatternFailed;
					}
					i = i.Previous.Previous;
					if (i.OpCode.Code == Code.Stfld && this.CheckAndSaveCurrentItemField((FieldReference)i.Operand))
					{
						return YieldStateMachineControlFlowRebuilder.NextStateNumberSearchResult.StateNumberFound;
					}
					return YieldStateMachineControlFlowRebuilder.NextStateNumberSearchResult.PatternFailed;
				}
			}
			newStateNumber = 0;
			return YieldStateMachineControlFlowRebuilder.NextStateNumberSearchResult.StateWasNotSet;
		}

		private bool TryGetVariableFromInstruction(Instruction instruction, out VariableReference varReference)
		{
			return StateMachineUtilities.TryGetVariableFromInstruction(instruction, this.moveNextMethodContext.Variables, out varReference);
		}

		private bool TryProcessEndBlock(InstructionBlock theBlock)
		{
			int num;
			int num1;
			VariableReference variableReference = null;
			Instruction previous = theBlock.Last.Previous;
			if (!StateMachineUtilities.TryGetOperandOfLdc(previous, out num))
			{
				if (!this.TryGetVariableFromInstruction(previous, out variableReference) || !this.CheckAndSaveReturnFlagVariable(variableReference))
				{
					return false;
				}
				this.yieldBreaks.Add(theBlock);
				return this.MarkTrueReturningPredecessorsAsYieldReturn(theBlock);
			}
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				switch (this.TryGetNextStateNumber(theBlock, out num1))
				{
					case YieldStateMachineControlFlowRebuilder.NextStateNumberSearchResult.PatternFailed:
					{
						return false;
					}
					case YieldStateMachineControlFlowRebuilder.NextStateNumberSearchResult.StateNumberFound:
					{
						this.yieldReturns.Add(theBlock);
						InstructionBlock stateFistBlock = this.GetStateFistBlock(num1);
						theBlock.Successors = new InstructionBlock[] { stateFistBlock };
						break;
					}
					case YieldStateMachineControlFlowRebuilder.NextStateNumberSearchResult.StateWasNotSet:
					{
						return this.MarkPredecessorsAsYieldReturns(theBlock);
					}
				}
			}
			else
			{
				this.yieldBreaks.Add(theBlock);
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