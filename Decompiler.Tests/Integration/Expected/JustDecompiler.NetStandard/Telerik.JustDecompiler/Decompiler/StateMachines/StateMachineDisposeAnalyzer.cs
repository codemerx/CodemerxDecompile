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
			else if (this.stateField != fieldDefinition)
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
				if ((this.IsBeqInstruction(instructionBlocks.Last) || this.IsBneUnInstruction(instructionBlocks.Last)) && StateMachineUtilities.TryGetOperandOfLdc(instructionBlocks.Last.Previous, out num))
				{
					Instruction instruction = null;
					instruction = (!this.IsBeqInstruction(instructionBlocks.Last) ? instructionBlocks.Last.Next : instructionBlocks.Last.Operand as Instruction);
					if (instruction == null)
					{
						throw new Exception("branchTargetInstruction cannot be null.");
					}
					if (this.TryGetExceptionHandler(StateMachineDisposeAnalyzer.SkipSingleNopInstructionBlock(this.theDisposeCFG.InstructionToBlockMapping[instruction.Offset]), out exceptionHandler))
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
			Instruction previous = switchBlockInfo.SwitchBlock.Last.Previous;
			if (previous.OpCode.Code == Code.Sub)
			{
				previous = previous.Previous;
				if (!StateMachineUtilities.TryGetOperandOfLdc(previous, out num))
				{
					return false;
				}
				previous = previous.Previous;
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
			while (theBlock.First == theBlock.Last && (theBlock.First.OpCode.Code == Code.Br || theBlock.First.OpCode.Code == Code.Br_S))
			{
				Instruction operand = theBlock.First.Operand as Instruction;
				theBlock = this.theDisposeCFG.InstructionToBlockMapping[operand.Offset];
			}
			theBlock = StateMachineDisposeAnalyzer.SkipSingleNopInstructionBlock(theBlock);
			return theBlock;
		}

		private bool GetDisposeMethodCFG()
		{
			string str = "System.Void System.IDisposable.Dispose()";
			MethodDefinition methodDefinition = null;
			foreach (MethodDefinition method in this.moveNextMethodDefinition.DeclaringType.Methods)
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
			if (item.OpCode.Code != Code.Ldarg_0)
			{
				return false;
			}
			item = this.theDisposeCFG.OffsetToInstruction[1];
			if (item.OpCode.Code != Code.Ldfld || !(item.Operand is FieldReference) || !this.CheckAndSaveStateField(item.Operand as FieldReference))
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
			if (theInstruction.OpCode.Code == Code.Beq)
			{
				return true;
			}
			return theInstruction.OpCode.Code == Code.Beq_S;
		}

		protected bool IsBneUnInstruction(Instruction theInstruction)
		{
			if (theInstruction.OpCode.Code == Code.Bne_Un)
			{
				return true;
			}
			return theInstruction.OpCode.Code == Code.Bne_Un_S;
		}

		private bool IsVersion2Disposer(InstructionBlock theBlock)
		{
			Instruction first = theBlock.First;
			if (first.OpCode.Code != Code.Ldarg_0)
			{
				return false;
			}
			first = first.Next;
			if (first.OpCode.Code != Code.Ldc_I4_1)
			{
				return false;
			}
			first = first.Next;
			if (first.OpCode.Code != Code.Stfld || !(first.Operand is FieldReference))
			{
				return false;
			}
			this.disposingField = ((FieldReference)first.Operand).Resolve();
			first = first.Next;
			if (first.OpCode.Code != Code.Ldarg_0)
			{
				return false;
			}
			first = first.Next;
			if (first.OpCode.Code != Code.Call || !(first.Operand is MethodReference) || (first.Operand as MethodReference).Resolve() != this.moveNextMethodDefinition)
			{
				return false;
			}
			first = first.Next;
			if (first.OpCode.Code != Code.Pop)
			{
				return false;
			}
			first = first.Next;
			if (first.OpCode.Code != Code.Ldarg_0)
			{
				return false;
			}
			first = first.Next;
			if (first.OpCode.Code != Code.Ldc_I4_M1)
			{
				return false;
			}
			first = first.Next;
			if (first.OpCode.Code != Code.Stfld || !(first.Operand is FieldReference))
			{
				return false;
			}
			this.stateField = ((FieldReference)first.Operand).Resolve();
			return first.Next.OpCode.Code == Code.Ret;
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
			if (theBlock.First == theBlock.Last && theBlock.First.OpCode.Code == Code.Nop && (int)theBlock.Successors.Length == 1)
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
			if (handler.HandlerType != ExceptionHandlerType.Finally)
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
			Instruction handlerStart = handler.HandlerStart;
			handlerStart = handlerStart.Next;
			if (handlerStart == null || !StateMachineUtilities.TryGetOperandOfLdc(handlerStart, out nextState))
			{
				return false;
			}
			handlerStart = handlerStart.Next;
			if (handlerStart.OpCode.Code != Code.Stfld || ((FieldReference)handlerStart.Operand).Resolve() != this.stateField)
			{
				return false;
			}
			handlerStart = handlerStart.Next;
			handlerStart = handlerStart.Next;
			if (handlerStart.OpCode.Code != Code.Ldfld)
			{
				handlerStart = handlerStart.Next;
				if (handlerStart.OpCode.Code != Code.Ldfld)
				{
					return false;
				}
			}
			else if (handlerStart.Next.OpCode.Code != Code.Brfalse_S)
			{
				return false;
			}
			enumeratorField = (FieldReference)handlerStart.Operand;
			if (handlerStart.Next.OpCode.Code != Code.Brfalse_S)
			{
				handlerStart = handlerStart.Next;
				if (handlerStart.OpCode.Code != Code.Isinst || ((TypeReference)handlerStart.Operand).Name != "IDisposable")
				{
					return false;
				}
				handlerStart = handlerStart.Next;
				if (handlerStart.OpCode.Code != Code.Stfld)
				{
					return false;
				}
				disposableField = (FieldReference)handlerStart.Operand;
				handlerStart = handlerStart.Next;
				if (handlerStart == null)
				{
					return false;
				}
				handlerStart = handlerStart.Next;
				if (handlerStart == null || handlerStart.OpCode.Code != Code.Ldfld || handlerStart.Operand != disposableField)
				{
					return false;
				}
			}
			else
			{
				disposableField = enumeratorField;
				enumeratorField = null;
			}
			handlerStart = handlerStart.Next;
			if (handlerStart.OpCode.Code != Code.Brfalse_S || ((Instruction)handlerStart.Operand).OpCode.Code != Code.Endfinally)
			{
				return false;
			}
			handlerStart = handlerStart.Next;
			handlerStart = handlerStart.Next;
			if (handlerStart.OpCode.Code != Code.Ldfld || handlerStart.Operand != disposableField)
			{
				return false;
			}
			handlerStart = handlerStart.Next;
			if (handlerStart.OpCode.Code != Code.Callvirt || ((MethodReference)handlerStart.Operand).Name != "Dispose")
			{
				return false;
			}
			if (handlerStart.Next.OpCode.Code != Code.Endfinally)
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
					ExceptionHandler current = enumerator.Current;
					if (current.TryStart != theBlock.First)
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
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private bool TryGetFinallyMethodDefinition(ExceptionHandler theHandler, out MethodDefinition methodDef)
		{
			Instruction next = theHandler.HandlerStart.Next;
			if (next.OpCode.Code == Code.Call)
			{
				methodDef = ((MethodReference)next.Operand).Resolve();
				while (next.Next.OpCode.Code == Code.Nop)
				{
					next = next.Next;
				}
				if (next.Next.OpCode.Code == Code.Endfinally && next.Next.Next == theHandler.HandlerEnd)
				{
					return true;
				}
			}
			methodDef = null;
			return false;
		}
	}
}