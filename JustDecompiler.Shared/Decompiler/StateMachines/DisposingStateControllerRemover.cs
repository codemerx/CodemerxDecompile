using System;
using Telerik.JustDecompiler.Cil;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
    /// <summary>
    /// Removes the disposing check blocks at the begining of each state.
    /// </summary>
    /// <remarks>
    /// In the new version of the yield state machine logic for disposing (i.e. invocations of finally methods) is no longer moved in separate methods.
    /// Instead, a new field is used to determine if the state machine is in a disposing state. This field is set to true in the Dispose method and then
    /// the MoveNext method is called. The state controller in the move next method jumps to the begining of the next state. At the begining of each state
    /// there are two compiler generated blocks that check if the state machine is in a disposing state. If so the returnFlag variable is set to false and
    /// a leave instruction leads to the end of the method. Since the doFinallyBlocks variable is not set, the finally handlers of the guarded blocks
    /// that are left are executed. 
    /// </remarks>
    class DisposingStateControllerRemover : StateControllerRemover
    {
        private readonly FieldDefinition disposingField;
        private VariableReference returnFlagVariable;
        
        public DisposingStateControllerRemover(MethodSpecificContext methodContext, FieldDefinition stateField, FieldDefinition disposingField)
            :base(methodContext, stateField)
        {
            this.disposingField = disposingField;
        }

        protected override ControllerTraversalSearchResult TryGetStateEntry(Cil.InstructionBlock theBlock, out Cil.InstructionBlock actualSuccessor)
        {
            actualSuccessor = SkipBranchChain(theBlock);

            if (!IsDisposingBlock(actualSuccessor))
            {
                return ControllerTraversalSearchResult.FoundControllerCandidate;
            }

            toBeRemoved.Add(actualSuccessor);
            if (IsFalseReturnBlock(actualSuccessor.Successors[1])) //more likely to be the near successor
            {
                toBeRemoved.Add(actualSuccessor.Successors[1]);
                actualSuccessor = actualSuccessor.Successors[0];
            }
            else if (IsFalseReturnBlock(actualSuccessor.Successors[0]))
            {
                toBeRemoved.Add(actualSuccessor.Successors[0]);
                actualSuccessor = actualSuccessor.Successors[1];
            }
            else
            {
                return ControllerTraversalSearchResult.PatternFailed;
            }

            return ControllerTraversalSearchResult.FoundStateEntry;
        }

        /// <summary>
        /// Checks whether the specified block is a disposing-check block.
        /// </summary>
        /// <remarks>
        /// Pattern:
        /// ldarg.0
        /// ldfld disposingField
        /// ..........
        /// conditional branch
        /// </remarks>
        /// <param name="theBlock"></param>
        /// <returns></returns>
        private bool IsDisposingBlock(InstructionBlock theBlock)
        {
            Instruction currentInstruction = theBlock.First;
            if (currentInstruction.OpCode.Code != Code.Ldarg_0 || currentInstruction == theBlock.Last)
            {
                return false;
            }

            currentInstruction = currentInstruction.Next;
            if (currentInstruction.OpCode.Code != Code.Ldfld || ((FieldReference)currentInstruction.Operand).Resolve() != disposingField ||
                currentInstruction == theBlock.Last)
            {
                return false;
            }

            Instruction lastInstruction = theBlock.Last;
            return lastInstruction.OpCode.Code == Code.Brfalse || lastInstruction.OpCode.Code == Code.Brfalse_S ||
                lastInstruction.OpCode.Code == Code.Brtrue || lastInstruction.OpCode.Code == Code.Brtrue_S;
        }

        /// <summary>
        /// Checks whether the block sets the returnFlag variable to false.
        /// </summary>
        /// <param name="theBlock"></param>
        /// <returns></returns>
        private bool IsFalseReturnBlock(InstructionBlock theBlock)
        {
            Instruction currentInstruction = theBlock.First;
            if (currentInstruction.OpCode.Code != Code.Ldc_I4_0 || currentInstruction == theBlock.Last)
            {
                return false;
            }

            currentInstruction = currentInstruction.Next;
            VariableReference retFlagVariable;
            if (!TryGetVariableFromInstruction(currentInstruction, out retFlagVariable) || currentInstruction == theBlock.Last ||
                !CheckAndSaveReturnFlagVariable(retFlagVariable))
            {
                return false;
            }
            
            currentInstruction = currentInstruction.Next;
            return StateMachineUtilities.IsUnconditionalBranch(currentInstruction);
        }

        private bool CheckAndSaveReturnFlagVariable(VariableReference foundFlagVariable)
        {
            if (returnFlagVariable == null)
            {
                returnFlagVariable = foundFlagVariable;
            }
            else if(returnFlagVariable != foundFlagVariable)
            {
                return false;
            }
            return true;
        }
    }
}
