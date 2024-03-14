using System;
using Telerik.JustDecompiler.Decompiler.StateMachines;
using Telerik.JustDecompiler.Cil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using Mono.Cecil;
/* AGPL */
using JustDecompiler.Shared.Decompiler.StateMachines;
/* End AGPL */

namespace Telerik.JustDecompiler.Steps
{
    /// <summary>
    /// Removes the async state machine from the CFG of the method.
    /// </summary>
    /// <remarks>
    /// The async state machine is similar to the new version of the yield state machine(see RemoveYieldStateMachineStep for details on the yield state machine).
    /// The difference between the two is as follows:
    /// 1) There is no disposing field and disposing checks at the begining of each state, since there is no dispose method.
    /// 2) The logic of the state machine is enclosed by try/catch instead of try finally.
    /// 3) Each state is connected to the next, which means that there is no need to reattach the states.
    /// 4) The first state is numbered -1.
    /// 5) The async state machine may not have a state controller.
    /// 
    /// Each state, except the first, begins with a block that loads the values of local variables from the previous state. These blocks have for predecessors
    /// only state controller blocks. Each of the state entry blocks has only one successor - the starting block of the actual logic for this state. Since the
    /// starting block of the state's logic has also for a predecessor the ending block of the previous state. This means that if we remove the state
    /// controllers and we find the first state, then the CFG cleaner will remove the entry blocks of the states(the ones that load the previous state), since
    /// they will have no predecessors.
    /// Each state ends with a condition block which is part of the original method logic. The block checks the value of the IsCompleted property of an awaiter
    /// variable. If the awaiting is completed then the control transfers to the start of the next state's logic. Otherwise the state is saved and the method
    /// is exited.
    /// 
    /// Here is how the state machine is removed:
    /// 1) Get the state field. Since there might not be any state controller blocks we get the state field from the second to last block in the method.
    /// This block always begins with assignment of the state field. The value that is assigned represents the "terminated" state of the state machine.
    /// 2) Mark for removal the blocks that save the state at the end of each state. They have for predecessors only the end of each state so there is no problem
    /// to mark them for removal at this point.
    /// 3) Mark for removal the do-finally-check blocks.
    /// 4) Mark the blocks of the state machine controller for removal. Since there are some minor differences with the yield state machine controller, a special
    /// AsyncStateMachineController was implemented. It handles the "strange" controller blocks that are generated from the C# and VB compiler in Debug and Release
    /// 5) Create fake controller switch data, if there were no state controller blocks.
    /// 6) Remove the blocks that were marked for removal.
    /// </remarks>
    class RemoveAsyncStateMachineStep : BaseStateMachineRemoverStep
    {
        private readonly HashSet<VariableReference> awaiterVariables = new HashSet<VariableReference>();

        private FieldDefinition stateField;
        
        protected override bool ProcessCFG()
        {
            if (!GetStateField() || !RemoveStateSavingBlocks())
            {
                return false;
            }

            AsyncMoveNextMethodAnalyzer analyzer = new AsyncMoveNextMethodAnalyzer(this.moveNextMethodContext, this.stateField);
            StateMachineFinallyCheckRemoverBase finallyCheckRemover;
            if (analyzer.StateMachineVersion == AsyncStateMachineVersion.V1)
            {
                finallyCheckRemover = new StateMachineDoFinallyCheckRemover(this.moveNextMethodContext);
                finallyCheckRemover.MarkFinallyConditionsForRemoval(analyzer.DoFinallyVariable);
            }
            else
            {
                finallyCheckRemover = new StateMachineFinallyStateCheckRemover(this.moveNextMethodContext);
                finallyCheckRemover.MarkFinallyConditionsForRemoval(analyzer.StateVariable);
            }

            toBeRemoved.UnionWith(finallyCheckRemover.BlocksMarkedForRemoval);

            AsyncStateControllerRemover asyncControllerRemover =
                new AsyncStateControllerRemover(this.moveNextMethodContext, this.stateField, analyzer.DoFinallyVariable, analyzer.StateMachineVersion);
            if (!asyncControllerRemover.RemoveStateMachineController() && asyncControllerRemover.FoundControllerBlocks)
            {
                return false;
            }
            toBeRemoved.UnionWith(asyncControllerRemover.BlocksMarkedForRemoval);

            /* AGPL */
            SwitchData[] controllerSwitchData;
            if (asyncControllerRemover.SwitchDataList != null && asyncControllerRemover.SwitchDataList.Count > 0)
            {
                controllerSwitchData = asyncControllerRemover.SwitchDataList.ToArray();
            }
            else if (asyncControllerRemover.SwitchData != null)
            {
                controllerSwitchData = new SwitchData[1] { asyncControllerRemover.SwitchData };
            }
            else
            {
                SwitchData s;
                if (!CreateFakeSwitchData(out s))
                {
                    return false;
                }
                controllerSwitchData = new SwitchData[1] { s };
            }

            StateMachineCFGCleanerV2 cfgCleaner = new StateMachineCFGCleanerV2(this.theCFG, controllerSwitchData, asyncControllerRemover.DefaultStateEntry ?? controllerSwitchData[0].DefaultCase);
            /* End AGPL */
            if (!cfgCleaner.CleanUpTheCFG(toBeRemoved))
            {
                return false;
            }

            this.moveNextMethodContext.AsyncData = new Decompiler.AsyncData(stateField, awaiterVariables, analyzer.variableToFieldMap);
            return true;
        }

        /// <summary>
        /// Creates a fake switch data for the CFG cleaner.
        /// </summary>
        /// <remarks>
        /// In async methods without await there is no state controller blocks. In these cases switch data cannot be created by the state controller remover.
        /// Thats why we create a fake switch data which contains only the default case, which will later become the entry of the method.
        /// </remarks>
        /// <returns></returns>
        private bool CreateFakeSwitchData(out SwitchData switchData)
        {
            InstructionBlock defaultCase;
            if (!GetMethodEntry(out defaultCase))
            {
                switchData = null;
                return false;
            }

            switchData = new SwitchData(null, defaultCase, new InstructionBlock[0]);
            return true;
        }

        /// <summary>
        /// Gets the first control flow block that is not marked for removal.
        /// </summary>
        /// <remarks>
        /// The search for the first block follows the control flow. This method is used only when creating fake switch data, which means
        /// that there are no controller blocks in the method. This means that only some simple blocks should have been marked for removal
        /// (e.g. nop blocks, unconditional branch blocks). That's why we presume that there will be no branches during the search, which means
        /// that a simple while will suffice for the algorithm.
        /// If we reach a block that is marked for removal but has more than one successor then the method fails (returns false).
        /// </remarks>
        /// <param name="methodEntry"></param>
        /// <returns></returns>
        private bool GetMethodEntry(out InstructionBlock methodEntry)
        {
            InstructionBlock current = theCFG.Blocks[0];
            while(toBeRemoved.Contains(current))
            {
                if(current.Successors.Length != 1)
                {
                    methodEntry = null;
                    return false;
                }

                current = current.Successors[0];
            }

            methodEntry = current;
            return true;
        }

        /// <summary>
        /// Removes the blocks that save the state and then exit the method.
        /// </summary>
        /// <remarks>
        /// We search for the state saving blocks by starting from the last block in the CFG, which should be a block that contains only the ret instruction
        /// (and some nops when compiled in debug). This block has two predecessors that are not state saving blocks: the second to last block that is used to set
        /// the result when the state machine terminates (i.e. return in the original code) and the body of the catch clause that is part of the try/catch construct
        /// that encloses the logic of the state machine. All other predecessors should be state saving blocks.
        /// We detach the final return block (and we mark it for removal) from these two block in order to avoid creating an unneeded goto from the catch block.
        /// </remarks>
        /// <returns></returns>
        private bool RemoveStateSavingBlocks()
        {
            InstructionBlock lastBlock = theCFG.Blocks[theCFG.Blocks.Length - 1];

            Instruction currentInstruction = lastBlock.First;
            while (currentInstruction.OpCode.Code == Code.Nop && currentInstruction != lastBlock.Last)
            {
                currentInstruction = currentInstruction.Next;
            }

            if (currentInstruction.OpCode.Code != Code.Ret)
            {
                return false;
            }

            int detachedPredecessorsCount = 0;
            HashSet<InstructionBlock> predecessors = new HashSet<InstructionBlock>(lastBlock.Predecessors);
            foreach (InstructionBlock predecessor in predecessors)
            {
                if (predecessor.Successors.Length > 1)
                {
                    return false;
                }

                if (predecessor.Predecessors.Count == 1 && theCFG.Blocks[theCFG.Blocks.Length - 2] != predecessor)
                {
                    if (!CheckForStateFieldSet(predecessor) || !TryRemoveStateSavingBlock(predecessor))
                    {
                        return false;
                    }
                }
                else
                {
                    predecessor.Successors = new InstructionBlock[0];
                    detachedPredecessorsCount++;
                }
            }

            toBeRemoved.Add(lastBlock);
            return detachedPredecessorsCount == 2;
        }

        /// <summary>
        /// Checks whether the state field is set in this block.
        /// </summary>
        /// <param name="theBlock"></param>
        /// <returns></returns>
        private bool CheckForStateFieldSet(InstructionBlock theBlock)
        {
            for (Instruction current = theBlock.First; current != theBlock.Last; current = current.Next)
            {
                if (current.OpCode.Code == Code.Stfld && ((FieldReference)current.Operand).Resolve() == stateField)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to detach the state changing block from its predecessor.
        /// </summary>
        /// <remarks>
        /// Marks the block for removal.
        /// Checks whether the predecessor of the block is IsCompleted check. If not then the removal fails.
        /// </remarks>
        /// <param name="theBlock"></param>
        /// <returns></returns>
        private bool TryRemoveStateSavingBlock(InstructionBlock theBlock)
        {
            InstructionBlock conditionBlock = GetFirst(theBlock.Predecessors);
            if (conditionBlock.Successors.Length != 2 || !CheckForIsCompletedCall(conditionBlock))
            {
                return false;
            }

            InstructionBlock onCompletedBlock = conditionBlock.Successors[0] == theBlock ? conditionBlock.Successors[1] : conditionBlock.Successors[0];

            theBlock.Successors = new InstructionBlock[0];
            conditionBlock.Successors = new InstructionBlock[] { onCompletedBlock };
            toBeRemoved.Add(theBlock);
            return true;
        }

        /// <summary>
        /// Checks whether the specified block contains a call of the get_IsCompleted method.
        /// </summary>
        /// <remarks>
        /// Saves the target variable of the call.
        /// </remarks>
        /// <param name="theBlock"></param>
        /// <returns></returns>
        private bool CheckForIsCompletedCall(InstructionBlock theBlock)
        {
            for (Instruction current = theBlock.First; current != theBlock.Last; current = current.Next)
            {
                if ((current.OpCode.Code == Code.Call || current.OpCode.Code == Code.Callvirt) &&
                    ((MethodReference)current.Operand).Name == "get_IsCompleted")
                {
                    VariableReference awaiterVariable;
                    if (StateMachineUtilities.TryGetVariableFromInstruction(current.Previous, moveNextMethodContext.Body.Variables, out awaiterVariable))
                    {
                        awaiterVariables.Add(awaiterVariable);
                        return true;
                    }
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the first element from the specified enumeration.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        private T GetFirst<T>(IEnumerable<T> collection)
        {
            foreach (T element in collection)
            {
                return element;
            }

            throw new Exception("Collection empty");
        }

        /// <summary>
        /// Gets the state field from the second to last block.
        /// </summary>
        /// <remarks>
        /// Since in some cases there might not be any controller blocks we need to take the state field from the second to last block.
        /// This block is always present and it always stores a value in the state field.
        /// </remarks>
        /// <returns></returns>
        private bool GetStateField()
        {
            if (this.theCFG.Blocks.Length < 2)
            {
                return false;
            }

            InstructionBlock theBlock = this.theCFG.Blocks[this.theCFG.Blocks.Length - 2];

            Instruction currentInstruction = theBlock.First;
            while (currentInstruction != theBlock.Last)
            {
                if (currentInstruction.OpCode.Code == Code.Stfld)
                {
                    stateField = ((FieldReference)currentInstruction.Operand).Resolve();
                    return stateField.DeclaringType == this.moveNextMethodContext.Method.DeclaringType;
                }
                currentInstruction = currentInstruction.Next;
            }

            return false;
        }
    }
}
