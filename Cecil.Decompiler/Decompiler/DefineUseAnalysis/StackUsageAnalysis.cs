using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Steps;
using Telerik.JustDecompiler.Common;

namespace Telerik.JustDecompiler.Decompiler.DefineUseAnalysis
{
    /// <summary>
    /// Performs the stack analysis. This class is responsible for the generation of PhiVariables.
    /// </summary>
    class StackUsageAnalysis : IDecompilationStep
    {
        private ICollection<ExceptionHandler> exceptionHandlers;
        private const string StackVariablePrefix = "stackVariable";
        private const string ExceptionVariablePrefix = "exception_";
        private int stackVariablesCount;
        private int exceptionVariableCount;
        private ControlFlowGraph controlFlowGraph;
        private int[][] blockToInitialStackMap; //[0] is bottom
        private int[][] blockToExitStackMap;    //[0] is bottom
        private readonly Dictionary<int, VariableDefinition> instructionOffsetToVariableDefinitionMap = new Dictionary<int, VariableDefinition>();
        private readonly Dictionary<int, Stack<int>> instructionOffsetToUsedInstructionsMap = new Dictionary<int, Stack<int>>(); //top is bottom
        private readonly UnionFinder<int> unionFinder = new UnionFinder<int>();
        private readonly HashSet<int> stackVariableInstructionsSet = new HashSet<int>();
        private readonly HashSet<int> exceptionVariableInstructionsSet = new HashSet<int>();
        private bool[] traversed;
        private MethodSpecificContext methodContext;

        public StackUsageAnalysis()
        {
            this.stackVariablesCount = 0;
            this.exceptionVariableCount = 0;
        }

        /// <summary>
        /// The entry point for the class.
        /// </summary>
        /// <param name="context">The decompilation context.</param>
        /// <param name="body">The body of the method.</param>
        /// <returns>Returns the updated body of the method.</returns>
        public BlockStatement Process(DecompilationContext context, BlockStatement body)
        {
            this.methodContext = context.MethodContext;
            this.controlFlowGraph = this.methodContext.ControlFlowGraph;
            this.exceptionHandlers = controlFlowGraph.RawExceptionHandlers;
            this.blockToInitialStackMap = new int[controlFlowGraph.Blocks.Length][];
            this.blockToExitStackMap = new int[controlFlowGraph.Blocks.Length][];

            /// Perform the analysis.
            AnalyzeStackUsage();

            GenerateStackVariables();

            GenerateStackData();

            return body;
        }

        /// <summary>
        /// The method that does the actual analysis.
        /// </summary>
        /// <param name="graphEntry">The entry point of the decompiled method.</param>
        private void AnalyzeStackUsage()
        {
            this.traversed = new bool[controlFlowGraph.Blocks.Length];

            /// This should pass through all the blocks of the method, missing only the exception handlers.
            RecursiveDfs(controlFlowGraph.Blocks[0], new int[0]);

            /// Pass through all the exception handlers.
            foreach (ExceptionHandler handler in exceptionHandlers)
            {
                InstructionBlock handlerBlock = controlFlowGraph.InstructionToBlockMapping[handler.HandlerStart.Offset];

                if (handler.HandlerType == ExceptionHandlerType.Fault || handler.HandlerType == ExceptionHandlerType.Finally)
                {
                    RecursiveDfs(handlerBlock, new int[0]);
                }
                else
                {
                    if (handler.HandlerType == ExceptionHandlerType.Filter)
                    {
                        InstructionBlock filterBlock = controlFlowGraph.InstructionToBlockMapping[handler.FilterStart.Offset];
                        RecursiveDfs(filterBlock, new int[] { -handler.FilterStart.Offset });
                        instructionOffsetToVariableDefinitionMap[-handler.FilterStart.Offset] =
                            new VariableDefinition(ExceptionVariablePrefix + exceptionVariableCount++, Utilities.GetCorlibTypeReference(typeof(Exception), methodContext.Method.Module), this.methodContext.Method);
                        exceptionVariableInstructionsSet.Add(-handler.FilterStart.Offset);
                    }
                    RecursiveDfs(handlerBlock, new int[] { -handler.HandlerStart.Offset });
                    instructionOffsetToVariableDefinitionMap[-handler.HandlerStart.Offset] =
                        new VariableDefinition(ExceptionVariablePrefix + exceptionVariableCount++,
                            handler.CatchType ?? Utilities.GetCorlibTypeReference(typeof(Exception), methodContext.Method.Module), this.methodContext.Method);
                    exceptionVariableInstructionsSet.Add(-handler.HandlerStart.Offset);
                }
            }

            for (int i = 0; i < controlFlowGraph.Blocks.Length; i++)
            {
                if (!traversed[i])
                {
                    throw new Exception("Unreachable block found");
                }
            }
        }

        private void RecursiveDfs(InstructionBlock currentBlock, int[] initialStack)
        {
            traversed[currentBlock.Index] = true;
            blockToInitialStackMap[currentBlock.Index] = initialStack;

            /// Perform the stack analysis on the current block.
            ComputeStackUsage(currentBlock);

            int[] exitStack = blockToExitStackMap[currentBlock.Index];

            /// Traverse successing blocks
            foreach (InstructionBlock successor in currentBlock.Successors)
            {
                if (!traversed[successor.Index])
                {
                    /// Visit unvisited successor.
                    RecursiveDfs(successor, exitStack);
                }
                else
                {
                    UpdateCurrentStackVariables(currentBlock, successor);
                }
            }
        }

        private void UpdateCurrentStackVariables(InstructionBlock parent, InstructionBlock successor)
        {
            int[] parentExitStack = blockToExitStackMap[parent.Index];
            int[] successorEntryStack = blockToInitialStackMap[successor.Index];

            if (parentExitStack.Length != successorEntryStack.Length)
            {
                /// This coveres the case, when single block can be entered with stacks of different size.
                /// As this is not allowed by standarts, an exception is thrown.
                /// No real code has been seen, that has this behavior. Only test that reproduces it is written by hand.
                throw new ArgumentException("Two paths with different stack states encountered.");
            }

            for (int i = 0; i < parentExitStack.Length; i++)
            {
                unionFinder.Union(successorEntryStack[i], parentExitStack[i]);
            }
        }

        /// <summary>
        /// Performs stack analysis on single block.
        /// </summary>
        /// <param name="block">The block to be analysed.</param>
        private void ComputeStackUsage(InstructionBlock block)
        {
            List<int> currentStack = new List<int>(blockToInitialStackMap[block.Index]);

            /// Analyse the stack usage for each instruction.
            foreach (Instruction instruction in block)
            {
                if (instruction.OpCode.Code == Code.Dup)
                {
                    if (currentStack.Count == 0)
                    {
                        throw new Exception("Invalid evaluation stack");
                    }
                    currentStack.Add(currentStack[currentStack.Count - 1]);
                    continue;
                }

                uint popDelta = GetPopDelta((uint)currentStack.Count, instruction);
                uint pushDelta = GetPushDelta(instruction);

                List<int> instructionPopStack = new List<int>();
                for (int i = 0; i < popDelta; i++)
                {
                    int last = currentStack.Count - 1;
                    instructionPopStack.Add(currentStack[last]);
                    currentStack.RemoveAt(last);
                }

                if (pushDelta > 0)
                {
                    stackVariableInstructionsSet.Add(instruction.Offset);
                }

                /// Push entries on the evaluation stack as they will be pushed by the instruction.
                for (int i = 0; i < pushDelta; i++)
                {
                    currentStack.Add(instruction.Offset);
                }

                /// Update the records for the instruction.
                instructionOffsetToUsedInstructionsMap[instruction.Offset] = new Stack<int>(instructionPopStack);
            }

            blockToExitStackMap[block.Index] = currentStack.ToArray();
        }

        /// <summary>
        /// Determines what ammount of entries does <paramref name="instruction"/> push on the evaluation stack.
        /// </summary>
        /// <param name="instruction">The instruction to be analyzed.</param>
        /// <returns>Returns the number of pushed items.</returns>
        private uint GetPushDelta(Instruction instruction)
        {
            OpCode code = instruction.OpCode;
            switch (code.StackBehaviourPush)
            {
                case StackBehaviour.Push0:
                    return 0;

                case StackBehaviour.Push1:
                case StackBehaviour.Pushi:
                case StackBehaviour.Pushi8:
                case StackBehaviour.Pushr4:
                case StackBehaviour.Pushr8:
                case StackBehaviour.Pushref:
                    return 1;

                case StackBehaviour.Push1_push1:
                    return 2;

                case StackBehaviour.Varpush:
                    if (code.FlowControl == FlowControl.Call)
                    {
                        IMethodSignature method = (IMethodSignature)instruction.Operand;
                        TypeReference @return = method.ReturnType;

                        return IsVoid(@return) ? (uint)0 : 1;
                    }

                    break;
            }
            throw new ArgumentException(Formatter.FormatInstruction(instruction));
        }

        /// <summary>
        /// Checks if <paramref name="type"/> is void or not.
        /// </summary>
        /// <param name="type">The type to be checked.</param>
        /// <returns>Returns true, if the type is void.</returns>
        private bool IsVoid(TypeReference type)
        {
            if (type.IsPointer)
            {
                /// void * is not considered void as void* represents pointer.
                return false;
            }

            if (type is IModifierType)
            {
                IModifierType optional = type as IModifierType;
                return IsVoid(optional.ElementType);
            }

            if (type.FullName == Constants.Void)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines what amount of stack entries does <paramref name="instruction"/> pop.
        /// </summary>
        /// <param name="stackHeight">The height of the stack, prior to the instruction.</param>
        /// <param name="instruction">The instruction.</param>
        /// <returns>Returns the number of popped elements.</returns>
        private uint GetPopDelta(uint stackHeight, Instruction instruction)
        {
            OpCode code = instruction.OpCode;
            switch (code.StackBehaviourPop)
            {
                case StackBehaviour.Pop0:
                    return 0;
                case StackBehaviour.Popi:
                case StackBehaviour.Popref:
                case StackBehaviour.Pop1:
                    return 1;

                case StackBehaviour.Pop1_pop1:
                case StackBehaviour.Popi_pop1:
                case StackBehaviour.Popi_popi:
                case StackBehaviour.Popi_popi8:
                case StackBehaviour.Popi_popr4:
                case StackBehaviour.Popi_popr8:
                case StackBehaviour.Popref_pop1:
                case StackBehaviour.Popref_popi:
                    return 2;

                case StackBehaviour.Popi_popi_popi:
                case StackBehaviour.Popref_popi_popi:
                case StackBehaviour.Popref_popi_popi8:
                case StackBehaviour.Popref_popi_popr4:
                case StackBehaviour.Popref_popi_popr8:
                case StackBehaviour.Popref_popi_popref:
                    return 3;

                case StackBehaviour.PopAll:
                    return stackHeight;

                case StackBehaviour.Varpop:
                    if (code.FlowControl == FlowControl.Call)
                    {
                        IMethodSignature method = (IMethodSignature)instruction.Operand;
                        uint count = (uint)method.Parameters.Count; /// All method's arguments are already loaded on the stack

                        if (OpCodes.Newobj.Value != code.Value)
                        {
                            if (method.HasThis) /// If the method has target, then it's also loaded on the stack
                            {
                                ++count;
                            }
                        }
                        if (code.Code == Code.Calli)
                        {
                            /// The reference to the method should be on the top of the stack.
                            count++;
                        }

                        return count;
                    }

                    /// After return the stack is empty.
                    if (code.Code == Code.Ret)
                        return stackHeight;

                    break;
            }
            throw new ArgumentException(Formatter.FormatInstruction(instruction));
        }

        private void GenerateStackVariables()
        {
            foreach (int instructionOffset in stackVariableInstructionsSet)
            {
                VariableDefinition varDef;
                int representative = unionFinder.Find(instructionOffset);
                if (!instructionOffsetToVariableDefinitionMap.TryGetValue(representative, out varDef))
                {
                    varDef = new VariableDefinition(StackVariablePrefix + stackVariablesCount++, null, this.methodContext.Method);
                    instructionOffsetToVariableDefinitionMap.Add(representative, varDef);
                }

                if (representative == instructionOffset)
                {
                    continue;
                }

                instructionOffsetToVariableDefinitionMap.Add(instructionOffset, varDef);
            }

            foreach (int instructionOffset in exceptionVariableInstructionsSet)
            {
                if (unionFinder.Find(instructionOffset) != instructionOffset)
                {
                    throw new Exception("Invalid stack usage");
                }
            }
        }

        private void GenerateStackData()
        {
            StackUsageData stackData = new StackUsageData();
            foreach (KeyValuePair<int, Stack<int>> pair in instructionOffsetToUsedInstructionsMap)
            {
                List<VariableDefinition> usedVariables = new List<VariableDefinition>(pair.Value.Select(offset => instructionOffsetToVariableDefinitionMap[offset]));
                stackData.InstructionOffsetToUsedStackVariablesMap[pair.Key] = usedVariables;

                foreach (VariableDefinition varDef in usedVariables)
                {
                    GetDefineUseInfo(stackData, varDef).UsedAt.Add(pair.Key);
                }
            }

            foreach (KeyValuePair<int, VariableDefinition> pair in instructionOffsetToVariableDefinitionMap)
            {
                if (pair.Key < 0)
                {
                    stackData.ExceptionHandlerStartToExceptionVariableMap.Add(-pair.Key, pair.Value);
                }
                else
                {
                    stackData.InstructionOffsetToAssignedVariableMap.Add(pair.Key, pair.Value);
                    GetDefineUseInfo(stackData, pair.Value).DefinedAt.Add(pair.Key);
                }
            }

            methodContext.StackData = stackData;
        }

        private StackVariableDefineUseInfo GetDefineUseInfo(StackUsageData stackData, VariableDefinition varDef)
        {
            StackVariableDefineUseInfo defineUseInfo;
            if (!stackData.VariableToDefineUseInfo.TryGetValue(varDef, out defineUseInfo))
            {
                defineUseInfo = new StackVariableDefineUseInfo();
                stackData.VariableToDefineUseInfo.Add(varDef, defineUseInfo);
            }

            return defineUseInfo;
        }
    }
}
