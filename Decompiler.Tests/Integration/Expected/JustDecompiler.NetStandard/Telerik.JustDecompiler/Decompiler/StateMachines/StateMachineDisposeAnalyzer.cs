using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
	internal class StateMachineDisposeAnalyzer
	{
		private readonly List<YieldExceptionHandlerInfo> yieldsExceptionData;

		private readonly Dictionary<ExceptionHandler, HashSet<int>> handlerToStatesMap;

		private readonly MethodDefinition moveNextMethodDefinition;

		private ControlFlowGraph theDisposeCFG;

		private FieldDefinition stateField;

		private FieldDefinition disposingField;

		public FieldDefinition DisposingField
		{
			get
			{
				return this.disposingField;
			}
		}

		public FieldDefinition StateField
		{
			get
			{
				return this.stateField;
			}
		}

		public List<YieldExceptionHandlerInfo> YieldsExceptionData
		{
			get
			{
				return this.yieldsExceptionData;
			}
		}

		public StateMachineDisposeAnalyzer(MethodDefinition moveNextMethodDefinition)
		{
			this.yieldsExceptionData = new List<YieldExceptionHandlerInfo>();
			this.handlerToStatesMap = new Dictionary<ExceptionHandler, HashSet<int>>();
			base();
			this.moveNextMethodDefinition = moveNextMethodDefinition;
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

		private void DetermineExceptionHandlingStatesFromCFGBlocks()
		{
			V_0 = this.theDisposeCFG.get_Blocks();
			V_1 = 0;
			while (V_1 < (int)V_0.Length)
			{
				V_2 = V_0[V_1];
				if (this.IsBeqInstruction(V_2.get_Last()) || this.IsBneUnInstruction(V_2.get_Last()) && StateMachineUtilities.TryGetOperandOfLdc(V_2.get_Last().get_Previous(), out V_3))
				{
					V_4 = null;
					if (!this.IsBeqInstruction(V_2.get_Last()))
					{
						V_4 = V_2.get_Last().get_Next();
					}
					else
					{
						V_4 = V_2.get_Last().get_Operand() as Instruction;
					}
					if (V_4 == null)
					{
						throw new Exception("branchTargetInstruction cannot be null.");
					}
					if (this.TryGetExceptionHandler(StateMachineDisposeAnalyzer.SkipSingleNopInstructionBlock(this.theDisposeCFG.get_InstructionToBlockMapping().get_Item(V_4.get_Offset())), out V_6))
					{
						if (!this.handlerToStatesMap.ContainsKey(V_6))
						{
							this.handlerToStatesMap.Add(V_6, new HashSet<int>());
						}
						dummyVar0 = this.handlerToStatesMap.get_Item(V_6).Add(V_3);
					}
				}
				V_1 = V_1 + 1;
			}
			return;
		}

		private bool DetermineExceptionHandlingStatesFromSwitchData(SwitchData switchBlockInfo)
		{
			V_0 = 0;
			V_1 = switchBlockInfo.get_SwitchBlock().get_Last().get_Previous();
			if (V_1.get_OpCode().get_Code() == 88)
			{
				V_1 = V_1.get_Previous();
				if (!StateMachineUtilities.TryGetOperandOfLdc(V_1, out V_0))
				{
					return false;
				}
				V_1 = V_1.get_Previous();
			}
			V_2 = switchBlockInfo.get_OrderedCasesArray();
			V_4 = 0;
			while (V_4 < (int)V_2.Length)
			{
				if (this.TryGetExceptionHandler(this.GetActualCase(V_2[V_4]), out V_6))
				{
					if (!this.handlerToStatesMap.ContainsKey(V_6))
					{
						this.handlerToStatesMap.Add(V_6, new HashSet<int>());
					}
					dummyVar0 = this.handlerToStatesMap.get_Item(V_6).Add(V_4 + V_0);
				}
				V_4 = V_4 + 1;
			}
			return true;
		}

		private InstructionBlock GetActualCase(InstructionBlock theBlock)
		{
			while ((object)theBlock.get_First() == (object)theBlock.get_Last() && theBlock.get_First().get_OpCode().get_Code() == 55 || theBlock.get_First().get_OpCode().get_Code() == 42)
			{
				V_0 = theBlock.get_First().get_Operand() as Instruction;
				theBlock = this.theDisposeCFG.get_InstructionToBlockMapping().get_Item(V_0.get_Offset());
			}
			theBlock = StateMachineDisposeAnalyzer.SkipSingleNopInstructionBlock(theBlock);
			return theBlock;
		}

		private bool GetDisposeMethodCFG()
		{
			V_0 = "System.Void System.IDisposable.Dispose()";
			V_1 = null;
			V_2 = this.moveNextMethodDefinition.get_DeclaringType().get_Methods().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					if (!String.op_Equality(V_3.GetFullMemberName(null), V_0))
					{
						continue;
					}
					V_1 = V_3;
					goto Label0;
				}
			}
			finally
			{
				V_2.Dispose();
			}
		Label0:
			if (V_1 == null)
			{
				return false;
			}
			this.theDisposeCFG = (new ControlFlowGraphBuilder(V_1)).CreateGraph();
			return true;
		}

		private bool GetYieldExceptionData()
		{
			if ((int)this.theDisposeCFG.get_Blocks().Length == 1)
			{
				return true;
			}
			V_0 = this.theDisposeCFG.get_OffsetToInstruction().get_Item(0);
			if (V_0.get_OpCode().get_Code() != 2)
			{
				return false;
			}
			V_0 = this.theDisposeCFG.get_OffsetToInstruction().get_Item(1);
			if (V_0.get_OpCode().get_Code() != 120 || V_0.get_Operand() as FieldReference == null || !this.CheckAndSaveStateField(V_0.get_Operand() as FieldReference))
			{
				return false;
			}
			V_2 = this.theDisposeCFG.get_SwitchBlocksInformation().get_Values().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					if (this.DetermineExceptionHandlingStatesFromSwitchData(V_3))
					{
						continue;
					}
					V_4 = false;
					goto Label0;
				}
			}
			finally
			{
				((IDisposable)V_2).Dispose();
			}
			this.DetermineExceptionHandlingStatesFromCFGBlocks();
			V_5 = this.handlerToStatesMap.GetEnumerator();
			try
			{
				while (V_5.MoveNext())
				{
					V_6 = V_5.get_Current();
					if (this.TryCreateYieldExceptionHandler(V_6.get_Value(), V_6.get_Key()))
					{
						continue;
					}
					V_4 = false;
					goto Label0;
				}
				goto Label1;
			}
			finally
			{
				((IDisposable)V_5).Dispose();
			}
		Label0:
			return V_4;
		Label1:
			return true;
		}

		protected bool IsBeqInstruction(Instruction theInstruction)
		{
			if (theInstruction.get_OpCode().get_Code() == 58)
			{
				return true;
			}
			return theInstruction.get_OpCode().get_Code() == 45;
		}

		protected bool IsBneUnInstruction(Instruction theInstruction)
		{
			if (theInstruction.get_OpCode().get_Code() == 63)
			{
				return true;
			}
			return theInstruction.get_OpCode().get_Code() == 50;
		}

		private bool IsVersion2Disposer(InstructionBlock theBlock)
		{
			V_0 = theBlock.get_First();
			if (V_0.get_OpCode().get_Code() != 2)
			{
				return false;
			}
			V_0 = V_0.get_Next();
			if (V_0.get_OpCode().get_Code() != 23)
			{
				return false;
			}
			V_0 = V_0.get_Next();
			if (V_0.get_OpCode().get_Code() != 122 || V_0.get_Operand() as FieldReference == null)
			{
				return false;
			}
			this.disposingField = ((FieldReference)V_0.get_Operand()).Resolve();
			V_0 = V_0.get_Next();
			if (V_0.get_OpCode().get_Code() != 2)
			{
				return false;
			}
			V_0 = V_0.get_Next();
			if (V_0.get_OpCode().get_Code() != 39 || V_0.get_Operand() as MethodReference == null || (object)(V_0.get_Operand() as MethodReference).Resolve() != (object)this.moveNextMethodDefinition)
			{
				return false;
			}
			V_0 = V_0.get_Next();
			if (V_0.get_OpCode().get_Code() != 37)
			{
				return false;
			}
			V_0 = V_0.get_Next();
			if (V_0.get_OpCode().get_Code() != 2)
			{
				return false;
			}
			V_0 = V_0.get_Next();
			if (V_0.get_OpCode().get_Code() != 21)
			{
				return false;
			}
			V_0 = V_0.get_Next();
			if (V_0.get_OpCode().get_Code() != 122 || V_0.get_Operand() as FieldReference == null)
			{
				return false;
			}
			this.stateField = ((FieldReference)V_0.get_Operand()).Resolve();
			V_1 = V_0.get_Next().get_OpCode();
			return V_1.get_Code() == 41;
		}

		public YieldStateMachineVersion ProcessDisposeMethod()
		{
			if (this.GetDisposeMethodCFG())
			{
				if (this.theDisposeCFG.get_SwitchBlocksInformation().get_Count() == 0 && (int)this.theDisposeCFG.get_Blocks().Length == 1 && this.IsVersion2Disposer(this.theDisposeCFG.get_Blocks()[0]))
				{
					return 2;
				}
				if (this.GetYieldExceptionData())
				{
					return 1;
				}
			}
			return 0;
		}

		private static InstructionBlock SkipSingleNopInstructionBlock(InstructionBlock theBlock)
		{
			V_0 = theBlock;
			if ((object)theBlock.get_First() == (object)theBlock.get_Last() && theBlock.get_First().get_OpCode().get_Code() == null && (int)theBlock.get_Successors().Length == 1)
			{
				V_0 = theBlock.get_Successors()[0];
			}
			return V_0;
		}

		private bool TryCreateYieldExceptionHandler(HashSet<int> tryStates, ExceptionHandler handler)
		{
			if (handler.get_HandlerType() != 2)
			{
				return false;
			}
			if (!this.TryGetFinallyMethodDefinition(handler, out V_0))
			{
				if (!this.TryGetDisposableConditionData(handler, out V_1, out V_2, out V_3))
				{
					return false;
				}
				this.yieldsExceptionData.Add(new YieldExceptionHandlerInfo(tryStates, V_1, V_2, V_3));
			}
			else
			{
				this.yieldsExceptionData.Add(new YieldExceptionHandlerInfo(tryStates, V_0));
			}
			return true;
		}

		private bool TryGetDisposableConditionData(ExceptionHandler handler, out int nextState, out FieldReference enumeratorField, out FieldReference disposableField)
		{
			nextState = -1;
			enumeratorField = null;
			disposableField = null;
			V_0 = handler.get_HandlerStart();
			V_0 = V_0.get_Next();
			if (V_0 == null || !StateMachineUtilities.TryGetOperandOfLdc(V_0, out nextState))
			{
				return false;
			}
			V_0 = V_0.get_Next();
			if (V_0.get_OpCode().get_Code() != 122 || (object)((FieldReference)V_0.get_Operand()).Resolve() != (object)this.stateField)
			{
				return false;
			}
			V_0 = V_0.get_Next();
			V_0 = V_0.get_Next();
			if (V_0.get_OpCode().get_Code() != 120)
			{
				V_0 = V_0.get_Next();
				if (V_0.get_OpCode().get_Code() != 120)
				{
					return false;
				}
			}
			else
			{
				if (V_0.get_Next().get_OpCode().get_Code() != 43)
				{
					return false;
				}
			}
			enumeratorField = (FieldReference)V_0.get_Operand();
			if (V_0.get_Next().get_OpCode().get_Code() != 43)
			{
				V_0 = V_0.get_Next();
				if (V_0.get_OpCode().get_Code() != 116 || String.op_Inequality(((TypeReference)V_0.get_Operand()).get_Name(), "IDisposable"))
				{
					return false;
				}
				V_0 = V_0.get_Next();
				if (V_0.get_OpCode().get_Code() != 122)
				{
					return false;
				}
				disposableField = (FieldReference)V_0.get_Operand();
				V_0 = V_0.get_Next();
				if (V_0 == null)
				{
					return false;
				}
				V_0 = V_0.get_Next();
				if (V_0 == null || V_0.get_OpCode().get_Code() != 120 || V_0.get_Operand() != disposableField)
				{
					return false;
				}
			}
			else
			{
				disposableField = enumeratorField;
				enumeratorField = null;
			}
			V_0 = V_0.get_Next();
			if (V_0.get_OpCode().get_Code() != 43 || ((Instruction)V_0.get_Operand()).get_OpCode().get_Code() != 186)
			{
				return false;
			}
			V_0 = V_0.get_Next();
			V_0 = V_0.get_Next();
			if (V_0.get_OpCode().get_Code() != 120 || V_0.get_Operand() != disposableField)
			{
				return false;
			}
			V_0 = V_0.get_Next();
			if (V_0.get_OpCode().get_Code() != 110 || String.op_Inequality(((MethodReference)V_0.get_Operand()).get_Name(), "Dispose"))
			{
				return false;
			}
			if (V_0.get_Next().get_OpCode().get_Code() != 186)
			{
				return false;
			}
			return true;
		}

		private bool TryGetExceptionHandler(InstructionBlock theBlock, out ExceptionHandler theHandler)
		{
			V_0 = this.theDisposeCFG.get_RawExceptionHandlers().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if ((object)V_1.get_TryStart() != (object)theBlock.get_First())
					{
						continue;
					}
					theHandler = V_1;
					V_2 = true;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				V_0.Dispose();
			}
		Label1:
			return V_2;
		Label0:
			theHandler = null;
			return false;
		}

		private bool TryGetFinallyMethodDefinition(ExceptionHandler theHandler, out MethodDefinition methodDef)
		{
			V_0 = theHandler.get_HandlerStart().get_Next();
			if (V_0.get_OpCode().get_Code() == 39)
			{
				methodDef = ((MethodReference)V_0.get_Operand()).Resolve();
				while (V_0.get_Next().get_OpCode().get_Code() == null)
				{
					V_0 = V_0.get_Next();
				}
				if (V_0.get_Next().get_OpCode().get_Code() == 186 && (object)V_0.get_Next().get_Next() == (object)theHandler.get_HandlerEnd())
				{
					return true;
				}
			}
			methodDef = null;
			return false;
		}
	}
}