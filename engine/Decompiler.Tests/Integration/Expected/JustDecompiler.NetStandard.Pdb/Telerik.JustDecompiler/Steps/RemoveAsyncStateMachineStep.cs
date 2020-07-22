using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.StateMachines;

namespace Telerik.JustDecompiler.Steps
{
	internal class RemoveAsyncStateMachineStep : BaseStateMachineRemoverStep
	{
		private readonly HashSet<VariableReference> awaiterVariables;

		private FieldDefinition stateField;

		public RemoveAsyncStateMachineStep()
		{
			this.awaiterVariables = new HashSet<VariableReference>();
			base();
			return;
		}

		private bool CheckForIsCompletedCall(InstructionBlock theBlock)
		{
			V_0 = theBlock.get_First();
			while ((object)V_0 != (object)theBlock.get_Last())
			{
				if (V_0.get_OpCode().get_Code() == 39 || V_0.get_OpCode().get_Code() == 110 && String.op_Equality(((MethodReference)V_0.get_Operand()).get_Name(), "get_IsCompleted"))
				{
					if (!StateMachineUtilities.TryGetVariableFromInstruction(V_0.get_Previous(), this.moveNextMethodContext.get_Body().get_Variables(), out V_2))
					{
						return false;
					}
					dummyVar0 = this.awaiterVariables.Add(V_2);
					return true;
				}
				V_0 = V_0.get_Next();
			}
			return false;
		}

		private bool CheckForStateFieldSet(InstructionBlock theBlock)
		{
			V_0 = theBlock.get_First();
			while ((object)V_0 != (object)theBlock.get_Last())
			{
				if (V_0.get_OpCode().get_Code() == 122 && (object)((FieldReference)V_0.get_Operand()).Resolve() == (object)this.stateField)
				{
					return true;
				}
				V_0 = V_0.get_Next();
			}
			return false;
		}

		private bool CreateFakeSwitchData(out SwitchData switchData)
		{
			if (!this.GetMethodEntry(out V_0))
			{
				switchData = null;
				return false;
			}
			switchData = new SwitchData(null, V_0, new InstructionBlock[0]);
			return true;
		}

		private T GetFirst<T>(IEnumerable<T> collection)
		{
			V_0 = collection.GetEnumerator();
			try
			{
				if (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
				}
				else
				{
					goto Label0;
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			return V_1;
		Label0:
			throw new Exception("Collection empty");
		}

		private bool GetMethodEntry(out InstructionBlock methodEntry)
		{
			V_0 = this.theCFG.get_Blocks()[0];
			while (this.toBeRemoved.Contains(V_0))
			{
				if ((int)V_0.get_Successors().Length != 1)
				{
					methodEntry = null;
					return false;
				}
				V_0 = V_0.get_Successors()[0];
			}
			methodEntry = V_0;
			return true;
		}

		private bool GetStateField()
		{
			if ((int)this.theCFG.get_Blocks().Length < 2)
			{
				return false;
			}
			V_0 = this.theCFG.get_Blocks()[(int)this.theCFG.get_Blocks().Length - 2];
			V_1 = V_0.get_First();
			while ((object)V_1 != (object)V_0.get_Last())
			{
				if (V_1.get_OpCode().get_Code() == 122)
				{
					this.stateField = ((FieldReference)V_1.get_Operand()).Resolve();
					return (object)this.stateField.get_DeclaringType() == (object)this.moveNextMethodContext.get_Method().get_DeclaringType();
				}
				V_1 = V_1.get_Next();
			}
			return false;
		}

		protected override bool ProcessCFG()
		{
			if (!this.GetStateField() || !this.RemoveStateSavingBlocks())
			{
				return false;
			}
			V_0 = new AsyncMoveNextMethodAnalyzer(this.moveNextMethodContext, this.stateField);
			if (V_0.get_StateMachineVersion() != AsyncStateMachineVersion.V1)
			{
				V_1 = new StateMachineFinallyStateCheckRemover(this.moveNextMethodContext);
				V_1.MarkFinallyConditionsForRemoval(V_0.get_StateVariable());
			}
			else
			{
				V_1 = new StateMachineDoFinallyCheckRemover(this.moveNextMethodContext);
				V_1.MarkFinallyConditionsForRemoval(V_0.get_DoFinallyVariable());
			}
			this.toBeRemoved.UnionWith(V_1.get_BlocksMarkedForRemoval());
			V_2 = new AsyncStateControllerRemover(this.moveNextMethodContext, this.stateField, V_0.get_DoFinallyVariable(), V_0.get_StateMachineVersion());
			if (!V_2.RemoveStateMachineController() && V_2.get_FoundControllerBlocks())
			{
				return false;
			}
			this.toBeRemoved.UnionWith(V_2.get_BlocksMarkedForRemoval());
			if (V_2.get_SwitchData() == null)
			{
				if (!this.CreateFakeSwitchData(out V_3))
				{
					return false;
				}
			}
			else
			{
				V_3 = V_2.get_SwitchData();
			}
			if (!(new StateMachineCFGCleaner(this.theCFG, V_3, V_3.get_DefaultCase())).CleanUpTheCFG(this.toBeRemoved))
			{
				return false;
			}
			this.moveNextMethodContext.set_AsyncData(new AsyncData(this.stateField, this.awaiterVariables, V_0.variableToFieldMap));
			return true;
		}

		private bool RemoveStateSavingBlocks()
		{
			V_0 = this.theCFG.get_Blocks()[(int)this.theCFG.get_Blocks().Length - 1];
			V_1 = V_0.get_First();
			while (V_1.get_OpCode().get_Code() == null && (object)V_1 != (object)V_0.get_Last())
			{
				V_1 = V_1.get_Next();
			}
			if (V_1.get_OpCode().get_Code() != 41)
			{
				return false;
			}
			V_2 = 0;
			V_4 = (new HashSet<InstructionBlock>(V_0.get_Predecessors())).GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_5 = V_4.get_Current();
					if ((int)V_5.get_Successors().Length <= 1)
					{
						if (V_5.get_Predecessors().get_Count() != 1 || !InstructionBlock.op_Inequality(this.theCFG.get_Blocks()[(int)this.theCFG.get_Blocks().Length - 2], V_5))
						{
							V_5.set_Successors(new InstructionBlock[0]);
							V_2 = V_2 + 1;
						}
						else
						{
							if (this.CheckForStateFieldSet(V_5) && this.TryRemoveStateSavingBlock(V_5))
							{
								continue;
							}
							V_6 = false;
							goto Label1;
						}
					}
					else
					{
						V_6 = false;
						goto Label1;
					}
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_4).Dispose();
			}
		Label1:
			return V_6;
		Label0:
			dummyVar0 = this.toBeRemoved.Add(V_0);
			return V_2 == 2;
		}

		private bool TryRemoveStateSavingBlock(InstructionBlock theBlock)
		{
			V_0 = this.GetFirst<InstructionBlock>(theBlock.get_Predecessors());
			if ((int)V_0.get_Successors().Length != 2 || !this.CheckForIsCompletedCall(V_0))
			{
				return false;
			}
			if (InstructionBlock.op_Equality(V_0.get_Successors()[0], theBlock))
			{
				stackVariable22 = V_0.get_Successors()[1];
			}
			else
			{
				stackVariable22 = V_0.get_Successors()[0];
			}
			V_1 = stackVariable22;
			theBlock.set_Successors(new InstructionBlock[0]);
			stackVariable28 = new InstructionBlock[1];
			stackVariable28[0] = V_1;
			V_0.set_Successors(stackVariable28);
			dummyVar0 = this.toBeRemoved.Add(theBlock);
			return true;
		}
	}
}