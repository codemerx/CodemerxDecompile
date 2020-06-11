using System;
using Mono.Cecil.Cil;
using Mono.Cecil;
using Telerik.JustDecompiler.Cil;
using Mono.Cecil.Extensions;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
    class StateMachineDisposeAnalyzer
    {
        private readonly List<YieldExceptionHandlerInfo> yieldsExceptionData = new List<YieldExceptionHandlerInfo>();
        private readonly Dictionary<ExceptionHandler, HashSet<int>> handlerToStatesMap = new Dictionary<ExceptionHandler, HashSet<int>>();

        private readonly MethodDefinition moveNextMethodDefinition;

        private ControlFlowGraph theDisposeCFG;
        private FieldDefinition stateField;
        private FieldDefinition disposingField;

        public FieldDefinition StateField
        {
            get
            {
                return stateField;
            }
        }

        public FieldDefinition DisposingField
        {
            get
            {
                return disposingField;
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

        /// <summary>
        /// Analyzes the Dispose method and gathers the yield exception handler data.
        /// </summary>
        public YieldStateMachineVersion ProcessDisposeMethod()
        {
            if (GetDisposeMethodCFG())
            {
                if (this.theDisposeCFG.SwitchBlocksInformation.Count == 0 &&
                    this.theDisposeCFG.Blocks.Length == 1 &&
                    IsVersion2Disposer(this.theDisposeCFG.Blocks[0]))
                {
                    return YieldStateMachineVersion.V2;
                }
                else if (GetYieldExceptionData())
                {
                    return YieldStateMachineVersion.V1;
                }
            }

            return YieldStateMachineVersion.None;
        }

        /// <summary>
        /// Gets the control flow graph of the Dispose method.
        /// </summary>
        private bool GetDisposeMethodCFG()
        {
            string disposeMethodFullMemberName = "System.Void System.IDisposable.Dispose()";
            TypeDefinition currentTypeDefinition = moveNextMethodDefinition.DeclaringType;
            MethodDefinition disposeMethodDefinition = null;
            foreach (MethodDefinition methodDefinition in currentTypeDefinition.Methods)
            {
                if (methodDefinition.GetFullMemberName(null) == disposeMethodFullMemberName)
                {
                    disposeMethodDefinition = methodDefinition;
                    break;
                }
            }

            if (disposeMethodDefinition == null)
            {
                return false;
            }

            theDisposeCFG = new ControlFlowGraphBuilder(disposeMethodDefinition).CreateGraph();
            return true;
        }

        /// <summary>
        /// Analyzes the CFG of the Dispose method and retrieves the information about the removed try/finally constructs.
        /// </summary>
        /// <remarks>
        /// We use the switch blocks information and the exception handler data of the dispose method to determine which states
        /// are covered by each try/finally construct. We also get the information for the finally method.
        /// 
        /// Update:
        /// Since C#6.0 in some simpler cases the compiler doesn't use switch but conditional branch instructions, so
        /// we now look for conditional branches who jump to try/finally constructs too.
        /// </remarks>
        private bool GetYieldExceptionData()
        {
            if (theDisposeCFG.Blocks.Length == 1)
            {
                // The dispose method contains only return statement.
                return true;
            }

            // The Dispose method should looks like this:
            // IL_0000: ldarg.0
            // IL_0001: ldfld int32 YieldBreakInDoWhile.TestCase/'<Main1>d__1'::'<>1__state'
            // IL_0006: stloc.0
            // ...

            Instruction currentInstruction = theDisposeCFG.OffsetToInstruction[0];
            if (currentInstruction.OpCode.Code != Code.Ldarg_0)
            {
                return false;
            }

            currentInstruction = theDisposeCFG.OffsetToInstruction[1];
            if (currentInstruction.OpCode.Code != Code.Ldfld || !(currentInstruction.Operand is FieldReference) ||
                !CheckAndSaveStateField(currentInstruction.Operand as FieldReference))
            {
                //sanity check - the state field used by the Dispose method should be the same as the field used by the MoveNext method
                return false;
            }

            foreach (SwitchData switchBlockInfo in theDisposeCFG.SwitchBlocksInformation.Values)
            {
                if (!DetermineExceptionHandlingStatesFromSwitchData(switchBlockInfo))
                {
                    return false;
                }
            }

            DetermineExceptionHandlingStatesFromCFGBlocks();

            foreach (KeyValuePair<ExceptionHandler, HashSet<int>> pair in this.handlerToStatesMap)
            {
                if (!TryCreateYieldExceptionHandler(pair.Value, pair.Key))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Process each switch block to determine which cases lead to try/finally constructs.
        /// </summary>
        /// <param name="switchBlockInfo"></param>
        private bool DetermineExceptionHandlingStatesFromSwitchData(SwitchData switchBlockInfo)
        {
            //Since the first try/finally that this switch covers can start at state 20, the complier will optimize this by subtracting 20 from the value of
            //the state field.
            int stateOffset = 0;
            Instruction currentInstruction = switchBlockInfo.SwitchBlock.Last.Previous;
            if (currentInstruction.OpCode.Code == Code.Sub)
            {
                currentInstruction = currentInstruction.Previous;
                if (!StateMachineUtilities.TryGetOperandOfLdc(currentInstruction, out stateOffset))
                {
                    return false;
                }

                currentInstruction = currentInstruction.Previous;
            }

            InstructionBlock[] orderedCases = switchBlockInfo.OrderedCasesArray;
            for (int i = 0; i < orderedCases.Length; i++)
            {
                InstructionBlock currentCase = GetActualCase(orderedCases[i]);

                ExceptionHandler theHandler;
                if (!TryGetExceptionHandler(currentCase, out theHandler))
                {
                    continue;
                }

                //We've found an exception handler.

                if (!this.handlerToStatesMap.ContainsKey(theHandler))
                {
                    this.handlerToStatesMap.Add(theHandler, new HashSet<int>());
                }

                this.handlerToStatesMap[theHandler].Add(i + stateOffset);
            }

            return true;
        }

        /// <summary>
        /// Process each CFG block to determine which cases lead to try/finally construct.
        /// </summary>
        private void DetermineExceptionHandlingStatesFromCFGBlocks()
        {
            foreach (InstructionBlock block in theDisposeCFG.Blocks)
            {
                int state;
                if ((!IsBeqInstruction(block.Last) && !IsBneUnInstruction(block.Last)) ||
                    !StateMachineUtilities.TryGetOperandOfLdc(block.Last.Previous, out state))
                {
                    continue;
                }

                Instruction branchTargetInstruction = null;
                if (IsBeqInstruction(block.Last))
                {
                    branchTargetInstruction = block.Last.Operand as Instruction;
                }
                else // bne.un*
                {
                    branchTargetInstruction = block.Last.Next as Instruction;
                }

                if (branchTargetInstruction == null)
                {
                    throw new Exception("branchTargetInstruction cannot be null.");
                }

                InstructionBlock targetBlock = SkipSingleNopInstructionBlock(theDisposeCFG.InstructionToBlockMapping[branchTargetInstruction.Offset]);
                ExceptionHandler theHandler;
                if (!TryGetExceptionHandler(targetBlock, out theHandler))
                {
                    continue;
                }

                //We've found an exception handler.

                if (!this.handlerToStatesMap.ContainsKey(theHandler))
                {
                    this.handlerToStatesMap.Add(theHandler, new HashSet<int>());
                }

                this.handlerToStatesMap[theHandler].Add(state);
            }
        }

        /// <summary>
        /// Determines whether the specified instruction is beq* instruction.
        /// </summary>
        protected bool IsBeqInstruction(Instruction theInstruction)
        {
            return theInstruction.OpCode.Code == Code.Beq || theInstruction.OpCode.Code == Code.Beq_S;
        }

        /// <summary>
        /// Determines whether the specified instruction is bne.un* instruction.
        /// </summary>
        protected bool IsBneUnInstruction(Instruction theInstruction)
        {
            return theInstruction.OpCode.Code == Code.Bne_Un || theInstruction.OpCode.Code == Code.Bne_Un_S;
        }

        /// <summary>
        /// Gets the actual target of the switch.
        /// </summary>
        /// <remarks>
        /// If the target of the switch is an instruction block containing only an unconditional branch instruction then we follow the branch target.
        /// This process is repeated until we reach a block that contains more than a single unconditional branch instruction.
        /// </remarks>
        /// <param name="theBlock"></param>
        /// <returns></returns>
        private InstructionBlock GetActualCase(InstructionBlock theBlock)
        {
            while (theBlock.First == theBlock.Last && (theBlock.First.OpCode.Code == Code.Br || theBlock.First.OpCode.Code == Code.Br_S))
            {
                Instruction branchTarget = theBlock.First.Operand as Instruction;
                theBlock = theDisposeCFG.InstructionToBlockMapping[branchTarget.Offset];
            }

            theBlock = SkipSingleNopInstructionBlock(theBlock);

            return theBlock;
        }

        /// <summary>
        /// If the given instruction block contains only a single nop instruction, this method returns it's successor, if such exists.
        /// </summary>
        private static InstructionBlock SkipSingleNopInstructionBlock(InstructionBlock theBlock)
        {
            InstructionBlock result = theBlock;
            if (theBlock.First == theBlock.Last && theBlock.First.OpCode.Code == Code.Nop && theBlock.Successors.Length == 1)
            {
                result = theBlock.Successors[0];
            }

            return result;
        }

        /// <summary>
        /// Tries to get the exception handler with guarded block that begins at the specified instruction block.
        /// </summary>
        /// <param name="theBlock"></param>
        /// <param name="theHandler"></param>
        /// <returns></returns>
        private bool TryGetExceptionHandler(InstructionBlock theBlock, out ExceptionHandler theHandler)
        {
            //TODO: Make the search faster
            foreach (ExceptionHandler handler in theDisposeCFG.RawExceptionHandlers)
            {
                if (handler.TryStart == theBlock.First)
                {
                    theHandler = handler;
                    return true;
                }
            }

            theHandler = null;
            return false;
        }

        /// <summary>
        /// Creates the yield exception handler info using the given information.
        /// </summary>
        /// <param name="tryStates"></param>
        /// <param name="handler"></param>
        private bool TryCreateYieldExceptionHandler(HashSet<int> tryStates, ExceptionHandler handler)
        {
            if (handler.HandlerType != ExceptionHandlerType.Finally)
            {
                return false;
            }

            MethodDefinition methodDef;
            int nextState;
            FieldReference enumeratorField;
            FieldReference disposableField;

            if (TryGetFinallyMethodDefinition(handler, out methodDef))
            {
                yieldsExceptionData.Add(new YieldExceptionHandlerInfo(tryStates, methodDef));
            }
            else if (TryGetDisposableConditionData(handler, out nextState, out enumeratorField, out disposableField))
            {
                yieldsExceptionData.Add(new YieldExceptionHandlerInfo(tryStates, nextState, enumeratorField, disposableField));
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether or not the finally block contains only a method invocation of a finally method.
        /// </summary>
        /// <remarks>
        /// This method works with the presumption that the finally block consist of only one instruction block, which contains
        /// the method invokation (, some nop instructions when compiled in Debug) and the endfinally instruction.
        /// </remarks>
        /// <param name="theHandler"></param>
        /// <param name="methodDef"></param>
        /// <returns></returns>
        private bool TryGetFinallyMethodDefinition(ExceptionHandler theHandler, out MethodDefinition methodDef)
        {
            Instruction theInstruction = theHandler.HandlerStart.Next;
            if (theInstruction.OpCode.Code == Code.Call)
            {
                MethodReference methodRef = (MethodReference)theInstruction.Operand;
                methodDef = methodRef.Resolve();

                while (theInstruction.Next.OpCode.Code == Code.Nop)
                {
                    theInstruction = theInstruction.Next;
                }

                if (theInstruction.Next.OpCode.Code == Code.Endfinally && theInstruction.Next.Next == theHandler.HandlerEnd) //sanity check
                {
                    return true;
                }
            }

            methodDef = null;
            return false;
        }

        /// <summary>
        /// Determines wheter the finally block contains a conditional disposal.
        /// </summary>
        /// <remarks>
        /// Matches two patterns:
        /// <code>
        /// this.stateField = nextState;
        /// (this.disposableField = this.enumeratorField as IDisposable;)   -- this might be missing
        /// if(this.disposableField != null)
        /// {
        ///     this.disposableField.Dispose();
        /// }
        /// </code>
        /// </remarks>
        /// <param name="handler"></param>
        /// <param name="nextState"></param>
        /// <param name="enumeratorField"></param>
        /// <param name="disposableField"></param>
        /// <returns></returns>
        private bool TryGetDisposableConditionData(ExceptionHandler handler, out int nextState,
            out FieldReference enumeratorField, out FieldReference disposableField)
        {
            nextState = -1;
            enumeratorField = null;
            disposableField = null;

            Instruction currentInstruction = handler.HandlerStart; //ldarg.0
            currentInstruction = currentInstruction.Next;          //ldc.i4. nextState
            if (currentInstruction == null || !StateMachineUtilities.TryGetOperandOfLdc(currentInstruction, out nextState))
            {
                return false;
            }

            currentInstruction = currentInstruction.Next;          //stfld stateField
            if (currentInstruction.OpCode.Code != Code.Stfld || ((FieldReference)currentInstruction.Operand).Resolve() != stateField)
            {
                return false;
            }

            currentInstruction = currentInstruction.Next;          //ldarg.0
            currentInstruction = currentInstruction.Next;          //ldarg.0 ?

            if (currentInstruction.OpCode.Code == Code.Ldfld)
            {
                if (currentInstruction.Next.OpCode.Code != Code.Brfalse_S)
                {
                    return false;
                }
            }
            else
            {
                currentInstruction = currentInstruction.Next;     //ldfld enumeratorField
                if (currentInstruction.OpCode.Code != Code.Ldfld)
                {
                    return false;
                }
            }
            enumeratorField = (FieldReference)currentInstruction.Operand;

            if (currentInstruction.Next.OpCode.Code == Code.Brfalse_S)
            {
                disposableField = enumeratorField;
                enumeratorField = null;
            }
            else
            {
                currentInstruction = currentInstruction.Next;          //isinst [mscorlib]System.IDisposable
                if (currentInstruction.OpCode.Code != Code.Isinst || ((TypeReference)currentInstruction.Operand).Name != "IDisposable")
                {
                    return false;
                }

                currentInstruction = currentInstruction.Next;          //stfld disposableField
                if (currentInstruction.OpCode.Code != Code.Stfld)
                {
                    return false;
                }
                disposableField = (FieldReference)currentInstruction.Operand;

                currentInstruction = currentInstruction.Next;          //ldarg.0
                if (currentInstruction == null)
                {
                    return false;
                }

                currentInstruction = currentInstruction.Next;          //ldfld disposableField
                if (currentInstruction == null || currentInstruction.OpCode.Code != Code.Ldfld || currentInstruction.Operand != disposableField)
                {
                    return false;
                }
            }

            currentInstruction = currentInstruction.Next;          //brfalse.s to endfinally
            if (currentInstruction.OpCode.Code != Code.Brfalse_S || ((Instruction)currentInstruction.Operand).OpCode.Code != Code.Endfinally)
            {
                return false;
            }

            currentInstruction = currentInstruction.Next;          //ldarg.0
            currentInstruction = currentInstruction.Next;          //ldfld disposableField
            if (currentInstruction.OpCode.Code != Code.Ldfld || currentInstruction.Operand != disposableField)
            {
                return false;
            }

            currentInstruction = currentInstruction.Next;          //callvirt Dispose()
            if (currentInstruction.OpCode.Code != Code.Callvirt || ((MethodReference)currentInstruction.Operand).Name != "Dispose")
            {
                return false;
            }

            if (currentInstruction.Next.OpCode.Code != Code.Endfinally)
            {
                return false;
            }

            return true;
        }

        private bool CheckAndSaveStateField(FieldReference foundStateField)
        {
            FieldDefinition foundFieldDef = foundStateField.Resolve();
            if (stateField == null)
            {
                stateField = foundFieldDef;
            }
            else if(stateField != foundFieldDef)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks whether the Dispose method is generated by the new C# compiler (Async CTP v3 and later)
        /// </summary>
        /// <remarks>
        /// Pattern:
        /// ldarg.0
        /// ldc.i4.1 (true)
        /// stfld disposingField
        /// ldarg.0
        /// call MoveNext()
        /// pop
        /// ldarg.0
        /// ldc.i4.m1
        /// stfld stateField
        /// ret
        /// </remarks>
        /// <param name="theBlock"></param>
        /// <returns></returns>
        private bool IsVersion2Disposer(InstructionBlock theBlock)
        {
            Instruction currentInstruction = theBlock.First;
            if (currentInstruction.OpCode.Code != Code.Ldarg_0)
            {
                return false;
            }

            currentInstruction = currentInstruction.Next;
            if (currentInstruction.OpCode.Code != Code.Ldc_I4_1)
            {
                return false;
            }

            currentInstruction = currentInstruction.Next;
            if (currentInstruction.OpCode.Code != Code.Stfld || !(currentInstruction.Operand is FieldReference))
            {
                return false;
            }
            disposingField = ((FieldReference)currentInstruction.Operand).Resolve();

            currentInstruction = currentInstruction.Next;
            if (currentInstruction.OpCode.Code != Code.Ldarg_0)
            {
                return false;
            }

            currentInstruction = currentInstruction.Next;
            if (currentInstruction.OpCode.Code != Code.Call || !(currentInstruction.Operand is MethodReference) ||
                (currentInstruction.Operand as MethodReference).Resolve() != this.moveNextMethodDefinition)
            {
                return false;
            }

            currentInstruction = currentInstruction.Next;
            if (currentInstruction.OpCode.Code != Code.Pop)
            {
                return false;
            }

            currentInstruction = currentInstruction.Next;
            if (currentInstruction.OpCode.Code != Code.Ldarg_0)
            {
                return false;
            }

            currentInstruction = currentInstruction.Next;
            if (currentInstruction.OpCode.Code != Code.Ldc_I4_M1)
            {
                return false;
            }

            currentInstruction = currentInstruction.Next;
            if (currentInstruction.OpCode.Code != Code.Stfld || !(currentInstruction.Operand is FieldReference))
            {
                return false;
            }
            stateField = ((FieldReference)currentInstruction.Operand).Resolve();

            return currentInstruction.Next.OpCode.Code == Code.Ret;
        }
    }
}
