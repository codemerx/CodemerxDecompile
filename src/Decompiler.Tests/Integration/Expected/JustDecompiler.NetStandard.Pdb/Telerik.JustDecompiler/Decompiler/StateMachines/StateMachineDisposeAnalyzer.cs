using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
	internal class StateMachineDisposeAnalyzer
	{
		private readonly List<YieldExceptionHandlerInfo> yieldsExceptionData = new List<YieldExceptionHandlerInfo>();

		private readonly Dictionary<ExceptionHandler, HashSet<int>> handlerToStatesMap = new Dictionary<ExceptionHandler, HashSet<int>>();

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
			this.moveNextMethodDefinition = moveNextMethodDefinition;
		}

		private bool CheckAndSaveStateField(FieldReference foundStateField)
		{
			FieldDefinition fieldDefinition = foundStateField.Resolve();
			if (this.stateField == null)
			{
				this.stateField = fieldDefinition;
			}
			else if ((object)this.stateField != (object)fieldDefinition)
			{
				return false;
			}
			return true;
		}

		private void DetermineExceptionHandlingStatesFromCFGBlocks()
		{
			int num;
			ExceptionHandler exceptionHandler;
			InstructionBlock[] blocks = this.theDisposeCFG.Blocks;
			for (int i = 0; i < (int)blocks.Length; i++)
			{
				InstructionBlock instructionBlocks = blocks[i];
				if ((this.IsBeqInstruction(instructionBlocks.Last) || this.IsBneUnInstruction(instructionBlocks.Last)) && StateMachineUtilities.TryGetOperandOfLdc(instructionBlocks.Last.get_Previous(), out num))
				{
					Instruction instruction = null;
					instruction = (!this.IsBeqInstruction(instructionBlocks.Last) ? instructionBlocks.Last.get_Next() : instructionBlocks.Last.get_Operand() as Instruction);
					if (instruction == null)
					{
						throw new Exception("branchTargetInstruction cannot be null.");
					}
					if (this.TryGetExceptionHandler(StateMachineDisposeAnalyzer.SkipSingleNopInstructionBlock(this.theDisposeCFG.InstructionToBlockMapping[instruction.get_Offset()]), out exceptionHandler))
					{
						if (!this.handlerToStatesMap.ContainsKey(exceptionHandler))
						{
							this.handlerToStatesMap.Add(exceptionHandler, new HashSet<int>());
						}
						this.handlerToStatesMap[exceptionHandler].Add(num);
					}
				}
			}
		}

		private bool DetermineExceptionHandlingStatesFromSwitchData(SwitchData switchBlockInfo)
		{
			ExceptionHandler exceptionHandler;
			int num = 0;
			Instruction previous = switchBlockInfo.SwitchBlock.Last.get_Previous();
			if (previous.get_OpCode().get_Code() == 88)
			{
				previous = previous.get_Previous();
				if (!StateMachineUtilities.TryGetOperandOfLdc(previous, out num))
				{
					return false;
				}
				previous = previous.get_Previous();
			}
			InstructionBlock[] orderedCasesArray = switchBlockInfo.OrderedCasesArray;
			for (int i = 0; i < (int)orderedCasesArray.Length; i++)
			{
				if (this.TryGetExceptionHandler(this.GetActualCase(orderedCasesArray[i]), out exceptionHandler))
				{
					if (!this.handlerToStatesMap.ContainsKey(exceptionHandler))
					{
						this.handlerToStatesMap.Add(exceptionHandler, new HashSet<int>());
					}
					this.handlerToStatesMap[exceptionHandler].Add(i + num);
				}
			}
			return true;
		}

		private InstructionBlock GetActualCase(InstructionBlock theBlock)
		{
			while ((object)theBlock.First == (object)theBlock.Last && (theBlock.First.get_OpCode().get_Code() == 55 || theBlock.First.get_OpCode().get_Code() == 42))
			{
				Instruction operand = theBlock.First.get_Operand() as Instruction;
				theBlock = this.theDisposeCFG.InstructionToBlockMapping[operand.get_Offset()];
			}
			theBlock = StateMachineDisposeAnalyzer.SkipSingleNopInstructionBlock(theBlock);
			return theBlock;
		}

		private bool GetDisposeMethodCFG()
		{
			string str = "System.Void System.IDisposable.Dispose()";
			MethodDefinition methodDefinition = null;
			foreach (MethodDefinition method in this.moveNextMethodDefinition.get_DeclaringType().get_Methods())
			{
				if (method.GetFullMemberName(null) != str)
				{
					continue;
				}
				methodDefinition = method;
				if (methodDefinition == null)
				{
					return false;
				}
				this.theDisposeCFG = (new ControlFlowGraphBuilder(methodDefinition)).CreateGraph();
				return true;
			}
			if (methodDefinition == null)
			{
				return false;
			}
			this.theDisposeCFG = (new ControlFlowGraphBuilder(methodDefinition)).CreateGraph();
			return true;
		}

		private bool GetYieldExceptionData()
		{
			bool flag;
			if ((int)this.theDisposeCFG.Blocks.Length == 1)
			{
				return true;
			}
			Instruction item = this.theDisposeCFG.OffsetToInstruction[0];
			if (item.get_OpCode().get_Code() != 2)
			{
				return false;
			}
			item = this.theDisposeCFG.OffsetToInstruction[1];
			if (item.get_OpCode().get_Code() != 120 || !(item.get_Operand() is FieldReference) || !this.CheckAndSaveStateField(item.get_Operand() as FieldReference))
			{
				return false;
			}
			foreach (SwitchData value in this.theDisposeCFG.SwitchBlocksInformation.Values)
			{
				if (this.DetermineExceptionHandlingStatesFromSwitchData(value))
				{
					continue;
				}
				flag = false;
				return flag;
			}
			this.DetermineExceptionHandlingStatesFromCFGBlocks();
			Dictionary<ExceptionHandler, HashSet<int>>.Enumerator enumerator = this.handlerToStatesMap.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<ExceptionHandler, HashSet<int>> current = enumerator.Current;
					if (this.TryCreateYieldExceptionHandler(current.Value, current.Key))
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
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
			Instruction first = theBlock.First;
			if (first.get_OpCode().get_Code() != 2)
			{
				return false;
			}
			first = first.get_Next();
			if (first.get_OpCode().get_Code() != 23)
			{
				return false;
			}
			first = first.get_Next();
			if (first.get_OpCode().get_Code() != 122 || !(first.get_Operand() is FieldReference))
			{
				return false;
			}
			this.disposingField = ((FieldReference)first.get_Operand()).Resolve();
			first = first.get_Next();
			if (first.get_OpCode().get_Code() != 2)
			{
				return false;
			}
			first = first.get_Next();
			if (first.get_OpCode().get_Code() != 39 || !(first.get_Operand() is MethodReference) || (object)(first.get_Operand() as MethodReference).Resolve() != (object)this.moveNextMethodDefinition)
			{
				return false;
			}
			first = first.get_Next();
			if (first.get_OpCode().get_Code() != 37)
			{
				return false;
			}
			first = first.get_Next();
			if (first.get_OpCode().get_Code() != 2)
			{
				return false;
			}
			first = first.get_Next();
			if (first.get_OpCode().get_Code() != 21)
			{
				return false;
			}
			first = first.get_Next();
			if (first.get_OpCode().get_Code() != 122 || !(first.get_Operand() is FieldReference))
			{
				return false;
			}
			this.stateField = ((FieldReference)first.get_Operand()).Resolve();
			OpCode opCode = first.get_Next().get_OpCode();
			return opCode.get_Code() == 41;
		}

		public YieldStateMachineVersion ProcessDisposeMethod()
		{
			if (this.GetDisposeMethodCFG())
			{
				if (this.theDisposeCFG.SwitchBlocksInformation.Count == 0 && (int)this.theDisposeCFG.Blocks.Length == 1 && this.IsVersion2Disposer(this.theDisposeCFG.Blocks[0]))
				{
					return YieldStateMachineVersion.V2;
				}
				if (this.GetYieldExceptionData())
				{
					return YieldStateMachineVersion.V1;
				}
			}
			return YieldStateMachineVersion.None;
		}

		private static InstructionBlock SkipSingleNopInstructionBlock(InstructionBlock theBlock)
		{
			InstructionBlock successors = theBlock;
			if ((object)theBlock.First == (object)theBlock.Last && theBlock.First.get_OpCode().get_Code() == null && (int)theBlock.Successors.Length == 1)
			{
				successors = theBlock.Successors[0];
			}
			return successors;
		}

		private bool TryCreateYieldExceptionHandler(HashSet<int> tryStates, ExceptionHandler handler)
		{
			MethodDefinition methodDefinition;
			int num;
			FieldReference fieldReference;
			FieldReference fieldReference1;
			if (handler.get_HandlerType() != 2)
			{
				return false;
			}
			if (!this.TryGetFinallyMethodDefinition(handler, out methodDefinition))
			{
				if (!this.TryGetDisposableConditionData(handler, out num, out fieldReference, out fieldReference1))
				{
					return false;
				}
				this.yieldsExceptionData.Add(new YieldExceptionHandlerInfo(tryStates, num, fieldReference, fieldReference1));
			}
			else
			{
				this.yieldsExceptionData.Add(new YieldExceptionHandlerInfo(tryStates, methodDefinition));
			}
			return true;
		}

		private bool TryGetDisposableConditionData(ExceptionHandler handler, out int nextState, out FieldReference enumeratorField, out FieldReference disposableField)
		{
			nextState = -1;
			enumeratorField = null;
			disposableField = null;
			Instruction handlerStart = handler.get_HandlerStart();
			handlerStart = handlerStart.get_Next();
			if (handlerStart == null || !StateMachineUtilities.TryGetOperandOfLdc(handlerStart, out nextState))
			{
				return false;
			}
			handlerStart = handlerStart.get_Next();
			if (handlerStart.get_OpCode().get_Code() != 122 || (object)((FieldReference)handlerStart.get_Operand()).Resolve() != (object)this.stateField)
			{
				return false;
			}
			handlerStart = handlerStart.get_Next();
			handlerStart = handlerStart.get_Next();
			if (handlerStart.get_OpCode().get_Code() != 120)
			{
				handlerStart = handlerStart.get_Next();
				if (handlerStart.get_OpCode().get_Code() != 120)
				{
					return false;
				}
			}
			else if (handlerStart.get_Next().get_OpCode().get_Code() != 43)
			{
				return false;
			}
			enumeratorField = (FieldReference)handlerStart.get_Operand();
			if (handlerStart.get_Next().get_OpCode().get_Code() != 43)
			{
				handlerStart = handlerStart.get_Next();
				if (handlerStart.get_OpCode().get_Code() != 116 || ((TypeReference)handlerStart.get_Operand()).get_Name() != "IDisposable")
				{
					return false;
				}
				handlerStart = handlerStart.get_Next();
				if (handlerStart.get_OpCode().get_Code() != 122)
				{
					return false;
				}
				disposableField = (FieldReference)handlerStart.get_Operand();
				handlerStart = handlerStart.get_Next();
				if (handlerStart == null)
				{
					return false;
				}
				handlerStart = handlerStart.get_Next();
				if (handlerStart == null || handlerStart.get_OpCode().get_Code() != 120 || handlerStart.get_Operand() != disposableField)
				{
					return false;
				}
			}
			else
			{
				disposableField = enumeratorField;
				enumeratorField = null;
			}
			handlerStart = handlerStart.get_Next();
			if (handlerStart.get_OpCode().get_Code() != 43 || ((Instruction)handlerStart.get_Operand()).get_OpCode().get_Code() != 186)
			{
				return false;
			}
			handlerStart = handlerStart.get_Next();
			handlerStart = handlerStart.get_Next();
			if (handlerStart.get_OpCode().get_Code() != 120 || handlerStart.get_Operand() != disposableField)
			{
				return false;
			}
			handlerStart = handlerStart.get_Next();
			if (handlerStart.get_OpCode().get_Code() != 110 || ((MethodReference)handlerStart.get_Operand()).get_Name() != "Dispose")
			{
				return false;
			}
			if (handlerStart.get_Next().get_OpCode().get_Code() != 186)
			{
				return false;
			}
			return true;
		}

		private bool TryGetExceptionHandler(InstructionBlock theBlock, out ExceptionHandler theHandler)
		{
			bool flag;
			Collection<ExceptionHandler>.Enumerator enumerator = this.theDisposeCFG.RawExceptionHandlers.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ExceptionHandler current = enumerator.get_Current();
					if ((object)current.get_TryStart() != (object)theBlock.First)
					{
						continue;
					}
					theHandler = current;
					flag = true;
					return flag;
				}
				theHandler = null;
				return false;
			}
			finally
			{
				enumerator.Dispose();
			}
			return flag;
		}

		private bool TryGetFinallyMethodDefinition(ExceptionHandler theHandler, out MethodDefinition methodDef)
		{
			Instruction next = theHandler.get_HandlerStart().get_Next();
			if (next.get_OpCode().get_Code() == 39)
			{
				methodDef = ((MethodReference)next.get_Operand()).Resolve();
				while (next.get_Next().get_OpCode().get_Code() == null)
				{
					next = next.get_Next();
				}
				if (next.get_Next().get_OpCode().get_Code() == 186 && (object)next.get_Next().get_Next() == (object)theHandler.get_HandlerEnd())
				{
					return true;
				}
			}
			methodDef = null;
			return false;
		}
	}
}