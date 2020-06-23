using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.StateMachines;

namespace Telerik.JustDecompiler.Steps
{
	internal class RemoveAsyncStateMachineStep : BaseStateMachineRemoverStep
	{
		private readonly HashSet<VariableReference> awaiterVariables = new HashSet<VariableReference>();

		private FieldDefinition stateField;

		public RemoveAsyncStateMachineStep()
		{
		}

		private bool CheckForIsCompletedCall(InstructionBlock theBlock)
		{
			VariableReference variableReference;
			for (Instruction i = theBlock.First; i != theBlock.Last; i = i.Next)
			{
				if ((i.OpCode.Code == Code.Call || i.OpCode.Code == Code.Callvirt) && ((MethodReference)i.Operand).Name == "get_IsCompleted")
				{
					if (!StateMachineUtilities.TryGetVariableFromInstruction(i.Previous, this.moveNextMethodContext.Body.Variables, out variableReference))
					{
						return false;
					}
					this.awaiterVariables.Add(variableReference);
					return true;
				}
			}
			return false;
		}

		private bool CheckForStateFieldSet(InstructionBlock theBlock)
		{
			for (Instruction i = theBlock.First; i != theBlock.Last; i = i.Next)
			{
				if (i.OpCode.Code == Code.Stfld && ((FieldReference)i.Operand).Resolve() == this.stateField)
				{
					return true;
				}
			}
			return false;
		}

		private bool CreateFakeSwitchData(out SwitchData switchData)
		{
			InstructionBlock instructionBlocks;
			if (!this.GetMethodEntry(out instructionBlocks))
			{
				switchData = null;
				return false;
			}
			switchData = new SwitchData(null, instructionBlocks, new InstructionBlock[0]);
			return true;
		}

		private T GetFirst<T>(IEnumerable<T> collection)
		{
			T current;
			using (IEnumerator<T> enumerator = collection.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					current = enumerator.Current;
				}
				else
				{
					throw new Exception("Collection empty");
				}
			}
			return current;
		}

		private bool GetMethodEntry(out InstructionBlock methodEntry)
		{
			InstructionBlock i;
			for (i = this.theCFG.Blocks[0]; this.toBeRemoved.Contains(i); i = i.Successors[0])
			{
				if ((int)i.Successors.Length != 1)
				{
					methodEntry = null;
					return false;
				}
			}
			methodEntry = i;
			return true;
		}

		private bool GetStateField()
		{
			if ((int)this.theCFG.Blocks.Length < 2)
			{
				return false;
			}
			InstructionBlock blocks = this.theCFG.Blocks[(int)this.theCFG.Blocks.Length - 2];
			for (Instruction i = blocks.First; i != blocks.Last; i = i.Next)
			{
				if (i.OpCode.Code == Code.Stfld)
				{
					this.stateField = ((FieldReference)i.Operand).Resolve();
					return this.stateField.DeclaringType == this.moveNextMethodContext.Method.DeclaringType;
				}
			}
			return false;
		}

		protected override bool ProcessCFG()
		{
			StateMachineFinallyCheckRemoverBase stateMachineFinallyStateCheckRemover;
			SwitchData switchData;
			if (!this.GetStateField() || !this.RemoveStateSavingBlocks())
			{
				return false;
			}
			AsyncMoveNextMethodAnalyzer asyncMoveNextMethodAnalyzer = new AsyncMoveNextMethodAnalyzer(this.moveNextMethodContext, this.stateField);
			if (asyncMoveNextMethodAnalyzer.StateMachineVersion != AsyncStateMachineVersion.V1)
			{
				stateMachineFinallyStateCheckRemover = new StateMachineFinallyStateCheckRemover(this.moveNextMethodContext);
				stateMachineFinallyStateCheckRemover.MarkFinallyConditionsForRemoval(asyncMoveNextMethodAnalyzer.StateVariable);
			}
			else
			{
				stateMachineFinallyStateCheckRemover = new StateMachineDoFinallyCheckRemover(this.moveNextMethodContext);
				stateMachineFinallyStateCheckRemover.MarkFinallyConditionsForRemoval(asyncMoveNextMethodAnalyzer.DoFinallyVariable);
			}
			this.toBeRemoved.UnionWith(stateMachineFinallyStateCheckRemover.BlocksMarkedForRemoval);
			AsyncStateControllerRemover asyncStateControllerRemover = new AsyncStateControllerRemover(this.moveNextMethodContext, this.stateField, asyncMoveNextMethodAnalyzer.DoFinallyVariable, asyncMoveNextMethodAnalyzer.StateMachineVersion);
			if (!asyncStateControllerRemover.RemoveStateMachineController() && asyncStateControllerRemover.FoundControllerBlocks)
			{
				return false;
			}
			this.toBeRemoved.UnionWith(asyncStateControllerRemover.BlocksMarkedForRemoval);
			if (asyncStateControllerRemover.SwitchData != null)
			{
				switchData = asyncStateControllerRemover.SwitchData;
			}
			else if (!this.CreateFakeSwitchData(out switchData))
			{
				return false;
			}
			if (!(new StateMachineCFGCleaner(this.theCFG, switchData, switchData.DefaultCase)).CleanUpTheCFG(this.toBeRemoved))
			{
				return false;
			}
			this.moveNextMethodContext.AsyncData = new AsyncData(this.stateField, this.awaiterVariables, asyncMoveNextMethodAnalyzer.variableToFieldMap);
			return true;
		}

		private bool RemoveStateSavingBlocks()
		{
			bool flag;
			InstructionBlock blocks = this.theCFG.Blocks[(int)this.theCFG.Blocks.Length - 1];
			Instruction first = blocks.First;
			while (first.OpCode.Code == Code.Nop && first != blocks.Last)
			{
				first = first.Next;
			}
			if (first.OpCode.Code != Code.Ret)
			{
				return false;
			}
			int num = 0;
			HashSet<InstructionBlock>.Enumerator enumerator = (new HashSet<InstructionBlock>(blocks.Predecessors)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					InstructionBlock current = enumerator.Current;
					if ((int)current.Successors.Length > 1)
					{
						flag = false;
						return flag;
					}
					else if (current.Predecessors.Count != 1 || !(this.theCFG.Blocks[(int)this.theCFG.Blocks.Length - 2] != current))
					{
						current.Successors = new InstructionBlock[0];
						num++;
					}
					else
					{
						if (this.CheckForStateFieldSet(current) && this.TryRemoveStateSavingBlock(current))
						{
							continue;
						}
						flag = false;
						return flag;
					}
				}
				this.toBeRemoved.Add(blocks);
				return num == 2;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private bool TryRemoveStateSavingBlock(InstructionBlock theBlock)
		{
			InstructionBlock first = this.GetFirst<InstructionBlock>(theBlock.Predecessors);
			if ((int)first.Successors.Length != 2 || !this.CheckForIsCompletedCall(first))
			{
				return false;
			}
			InstructionBlock instructionBlocks = (first.Successors[0] == theBlock ? first.Successors[1] : first.Successors[0]);
			theBlock.Successors = new InstructionBlock[0];
			first.Successors = new InstructionBlock[] { instructionBlocks };
			this.toBeRemoved.Add(theBlock);
			return true;
		}
	}
}