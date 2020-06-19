using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustDecompiler.Cil;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
    class YieldStateMachineControlFlowRebuilder
    {
        private readonly HashSet<InstructionBlock> yieldBreaks = new HashSet<InstructionBlock>();
        private readonly HashSet<InstructionBlock> yieldReturns = new HashSet<InstructionBlock>();
        private readonly HashSet<InstructionBlock> toBeRemoved = new HashSet<InstructionBlock>();

        private readonly SwitchData switchData;
        private readonly FieldDefinition stateField;
        private readonly MethodSpecificContext moveNextMethodContext;

        private FieldDefinition currentItemField;
        private VariableReference returnFlagVariable;

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

        public HashSet<InstructionBlock> BlocksMarkedForRemoval
        {
            get
            {
                return this.toBeRemoved;
            }
        }

        public YieldStateMachineControlFlowRebuilder(MethodSpecificContext moveNextMethodContext, SwitchData controllerSwitchData, FieldDefinition stateField)
        {
            this.moveNextMethodContext = moveNextMethodContext;
            this.switchData = controllerSwitchData;
            this.stateField = stateField;
        }

        /// <summary>
        /// Processes the blocks that end with returns.
        /// </summary>
        /// <remarks>
        /// Since each yield return and yield break exits the yield state machine, then we need to analyze all of the blocks that exit the method,
        /// to determine which of them should be marked as yield returns and which of them - yield breaks.
        /// </remarks>
        public bool ProcessEndBlocks()
        {
            InstructionBlock[] blocks = moveNextMethodContext.ControlFlowGraph.Blocks;
            for (int i = 0; i < blocks.Length; i++)
            {
                InstructionBlock theBlock = blocks[i];
                if (theBlock.Successors.Length == 0 && theBlock.Last.OpCode.Code == Code.Ret)
                {
                    if (!TryProcessEndBlock(theBlock))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Process the specified block, which ends with ret instruction.
        /// </summary>
        /// <remarks>
        /// The block should end with: return true, return false or return flagVariable.
        /// If the block ends with "return flagVariable", then we mark the block as yield break and we search for all of the predecessors of
        /// the block that set the flagVariable to true to mark them as yield returns.
        /// </remarks>
        /// <param name="theBlock"></param>
        private bool TryProcessEndBlock(InstructionBlock theBlock)
        {
            VariableReference flagVarReference = null;

            Instruction beforeRet = theBlock.Last.Previous;
            int returnValue;
            if (StateMachineUtilities.TryGetOperandOfLdc(beforeRet, out returnValue))
            {
                if (returnValue == 0) // == return false;
                {
                    yieldBreaks.Add(theBlock);
                }
                else if (returnValue == 1) // == return true;
                {
                    int stateNumber;
                    switch (TryGetNextStateNumber(theBlock, out stateNumber))
                    {
                        case NextStateNumberSearchResult.StateNumberFound:
                            yieldReturns.Add(theBlock);

                            InstructionBlock nextState = GetStateFistBlock(stateNumber);
                            theBlock.Successors = new InstructionBlock[] { nextState };
                            break;
                        case NextStateNumberSearchResult.StateWasNotSet:
                            //If we cannot find the next state number, we assume that this is a common exit for a bunch of yield return blocks.
                            //That's why we mark all the predecessors of this block as yield returns.
                            return MarkPredecessorsAsYieldReturns(theBlock);
                        case NextStateNumberSearchResult.PatternFailed:
                            return false;
                    }
                }
                else
                {
                    //throw new Exception("Illegal return value of MoveNext method.");
                    return false;
                }
            }
            else if (TryGetVariableFromInstruction(beforeRet, out flagVarReference) && CheckAndSaveReturnFlagVariable(flagVarReference))
            {
                yieldBreaks.Add(theBlock);
                return MarkTrueReturningPredecessorsAsYieldReturn(theBlock);
            }
            else
            {
                //throw new Exception("Illegal end block of yield iterator.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to get the state to which the current block leads.
        /// </summary>
        /// <remarks>
        /// We are searching for this pattern at the end of the block:
        /// .....
        /// load currentItemValue
        /// stfld currentItemField
        /// ldarg.0   --this
        /// ldc.i4. nextStateNumber
        /// stfld stateField
        /// .....
        /// </remarks>
        /// <param name="theBlock"></param>
        /// <param name="newStateNumber"></param>
        /// <returns></returns>
        private NextStateNumberSearchResult TryGetNextStateNumber(InstructionBlock theBlock, out int newStateNumber)
        {
            Instruction currentInstruction = theBlock.Last;

            while (currentInstruction != theBlock.First)
            {
                if (currentInstruction.OpCode.Code == Code.Stfld && stateField == ((FieldReference)currentInstruction.Operand).Resolve())
                {
                    currentInstruction = currentInstruction.Previous;
                    if (!StateMachineUtilities.TryGetOperandOfLdc(currentInstruction, out newStateNumber))
                    {
                        return NextStateNumberSearchResult.PatternFailed;
                    }

                    //currentInstruction == ldc.i4. ; currentInstruction.Previous == ldarg.0 ; currentInstruction.Previous.Previous == stfld currentItemField
                    currentInstruction = currentInstruction.Previous.Previous;
                    if (currentInstruction.OpCode.Code != Code.Stfld || !CheckAndSaveCurrentItemField((FieldReference)currentInstruction.Operand))
                    {
                        return NextStateNumberSearchResult.PatternFailed;
                    }

                    return NextStateNumberSearchResult.StateNumberFound;
                }

                currentInstruction = currentInstruction.Previous;
            }

            newStateNumber = 0;
            return NextStateNumberSearchResult.StateWasNotSet;
        }

        /// <summary>
        /// Gets the instruction block at which this state begins.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private InstructionBlock GetStateFistBlock(int state)
        {
            if (state >= 0 && state < switchData.OrderedCasesArray.Length)
            {
                return switchData.OrderedCasesArray[state];
            }

            return switchData.DefaultCase;
        }

        /// <summary>
        /// Marks the predecessors, of the specifed instruction block, that set the return flag variable to true, as yield returns.
        /// </summary>
        /// <remarks>
        /// If a predecessor is a block that contains a single unconditional branch instruction then we recursively call the method for that
        /// predecessor and we mark it for removal.
        /// This means that the block, that we initially call this method for, will not be removed and will be left to be marked as an yield break.
        /// 
        /// Update:
        /// Since C#6.0 not all of the predecessors should contain instructions for settings the flag variable. There could be a predecessor
        /// which contains invocation of finally method. In this case we recursively call this method for it's predecessors.
        /// </remarks>
        /// <param name="theBlock"></param>
        private bool MarkTrueReturningPredecessorsAsYieldReturn(InstructionBlock theBlock)
        {
            HashSet<InstructionBlock> predecessors = new HashSet<InstructionBlock>(theBlock.Predecessors);

            foreach (InstructionBlock predecessor in predecessors)
            {
                if (predecessor.Last == predecessor.First)
                {
                    if (StateMachineUtilities.IsUnconditionalBranch(predecessor.Last))
                    {
                        toBeRemoved.Add(predecessor);
                        if (!MarkTrueReturningPredecessorsAsYieldReturn(predecessor))
                        {
                            return false;
                        }
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }

                VariableReference flagVarReference = null;

                Instruction lastInstruction = predecessor.Last;
                if (StateMachineUtilities.IsUnconditionalBranch(lastInstruction))
                {
                    lastInstruction = lastInstruction.Previous;
                }

                int returnValue;
                if (IsFinallyMethodInvocationBlock(predecessor))
                {
                    if (!MarkTrueReturningPredecessorsAsYieldReturn(predecessor))
                    {
                        return false;
                    }
                }
                else if (TryGetVariableFromInstruction(lastInstruction, out flagVarReference) && CheckAndSaveReturnFlagVariable(flagVarReference) &&
                    StateMachineUtilities.TryGetOperandOfLdc(lastInstruction.Previous, out returnValue))
                {
                    if (returnValue == 1)
                    {
                        if (!TryAddToYieldReturningBlocks(predecessor))
                        {
                            return false;
                        }
                    }
                    else if (returnValue != 0)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the given block is block that contains only Finally method invocation. This pattern is brought by the new C#6.0 compiler.
        /// </summary>
        private bool IsFinallyMethodInvocationBlock(InstructionBlock block)
        {
            // The pattern is like so:
            // ldarg.0
            // call instance void ... (the finally method)
            // nop <- could be missing
            // leave.s ...

            Instruction current = block.First;
            if (current.OpCode.Code != Code.Ldarg_0)
            {
                return false;
            }

            current = current.Next;
            if (current.OpCode.Code != Code.Call)
            {
                return false;
            }

            Instruction callInstruction = current;

            current = current.Next;
            if (current.OpCode.Code == Code.Nop)
            {
                current = current.Next;
            }

            if (!StateMachineUtilities.IsUnconditionalBranch(current))
            {
                return false;
            }

            if (block.Last != current)
            {
                return false;
            }

            MethodReference finallyMethod = callInstruction.Operand as MethodReference;
            if (finallyMethod == null)
            {
                return false;
            }

            if (!finallyMethod.Name.StartsWith("<>m__Finally"))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Marks the predecessors of this block as yield return. Also adds this block to the set of nodes that are to be removed.
        /// </summary>
        /// <remarks>
        /// If a predecessor of the specified block is a block with a single unconditional branch instruction, then this method is
        /// recursively called for that predecessor.
        /// </remarks>
        /// <param name="theBlock"></param>
        private bool MarkPredecessorsAsYieldReturns(InstructionBlock theBlock)
        {
            toBeRemoved.Add(theBlock);

            HashSet<InstructionBlock> predecessors = new HashSet<InstructionBlock>(theBlock.Predecessors);

            foreach (InstructionBlock predecessor in predecessors)
            {
                if (predecessor.Last == predecessor.First)
                {
                    if (StateMachineUtilities.IsUnconditionalBranch(predecessor.Last) && MarkPredecessorsAsYieldReturns(predecessor))
                    {
                        continue;
                    }
                    else
                    {
                        //throw new Exception("Illegal block");
                        return false;
                    }
                }

                if (!TryAddToYieldReturningBlocks(predecessor))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Marks the instruction block as a block that contains yield return. Also finds the state to which this block leads and fixes the relation
        /// between the blocks.
        /// </summary>
        /// <param name="theBlock"></param>
        private bool TryAddToYieldReturningBlocks(InstructionBlock theBlock)
        {
            int nextStateNumber;
            if (TryGetNextStateNumber(theBlock, out nextStateNumber) == NextStateNumberSearchResult.StateNumberFound)
            {
                yieldReturns.Add(theBlock);

                InstructionBlock nextStateFirstBlock = GetStateFistBlock(nextStateNumber);
                theBlock.Successors = new InstructionBlock[] { nextStateFirstBlock };
                return true;
            }

            return false;
        }

        /// <summary>
        /// Saves the variable that holds the return value of the MoveNext method.
        /// </summary>
        /// <param name="foundReturnFlagVariable"></param>
        private bool CheckAndSaveReturnFlagVariable(VariableReference foundReturnFlagVariable)
        {
            if (returnFlagVariable == null)
            {
                returnFlagVariable = foundReturnFlagVariable;
            }
            else if (returnFlagVariable != foundReturnFlagVariable)
            {
                return false;
            }
            return true;
        }

        private bool CheckAndSaveCurrentItemField(FieldReference foundCurrentItemFieldRef)
        {
            FieldDefinition fieldDef = foundCurrentItemFieldRef.Resolve();
            if(currentItemField == null)
            {
                currentItemField = fieldDef;
            }
            else if(currentItemField != fieldDef)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to get the variable that is used by the specified instruction.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="varReference"></param>
        /// <returns>Flase if the instruction does not handle variables.</returns>
        private bool TryGetVariableFromInstruction(Instruction instruction, out VariableReference varReference)
        {
            return StateMachineUtilities.TryGetVariableFromInstruction(instruction, moveNextMethodContext.Variables, out varReference);
        }

        private enum NextStateNumberSearchResult
        {
            PatternFailed,
            StateNumberFound,
            StateWasNotSet
        }
    }
}
