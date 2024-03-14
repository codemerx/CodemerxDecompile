using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Cil;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
    class StateControllerRemover
    {
        protected readonly MethodSpecificContext methodContext;
        protected readonly ControlFlowGraph theCFG;
        protected readonly InstructionBlock[] stateToStartBlock;

        private FieldDefinition stateField;

        private SwitchData switchData;
        /* AGPL */
        private List<SwitchData> switchDataList;
        /* End AGPL */
        private InstructionBlock defaultStateEntry;
        protected VariableReference stateVariable;

        protected int firstControllerBlock = 0;

        protected readonly HashSet<InstructionBlock> toBeRemoved = new HashSet<InstructionBlock>();

        /// <summary>
        /// Gets the switch data representing the state machine controller.
        /// </summary>
        public SwitchData SwitchData
        {
            get
            {
                return this.switchData;
            }
        }

        /* AGPL */
        public List<SwitchData> SwitchDataList
        {
            get
            {
                return this.switchDataList;
            }
        }

        public InstructionBlock DefaultStateEntry
        {
            get
            {
                return this.defaultStateEntry;
            }
        }
        /* End AGPL */

        /// <summary>
        /// Gets the field that holds the number of the current state of the state machine.
        /// </summary>
        public FieldDefinition StateField
        {
            get
            {
                return this.stateField;
            }
        }

        /// <summary>
        /// Gets the blocks that were marked for removal during the cleaning of the state controller.
        /// </summary>
        public HashSet<InstructionBlock> BlocksMarkedForRemoval
        {
            get
            {
                return this.toBeRemoved;
            }
        }

        public StateControllerRemover(MethodSpecificContext methodContext, FieldDefinition stateField = null)
        {
            this.methodContext = methodContext;
            this.theCFG = methodContext.ControlFlowGraph;
            this.stateField = stateField;
            this.stateToStartBlock = new InstructionBlock[this.theCFG.Blocks.Length];
        }

        /// <summary>
        /// Gets the state field and state variable and removes the state machine controller.
        /// </summary>
        /// <returns>
        /// True if the state controller was successfully removed, false otherwise.
        /// </returns>
        public virtual bool RemoveStateMachineController()
        {
            return GetStateFieldAndVariable() && RemoveControllerChain();
        }

        /// <summary>
        /// Gets the state field and the state variable.
        /// </summary>
        /// <remarks>
        /// We presume that that the state variable is set in the first controller block of the CFG and that the first loaded field is the state field.
        /// We search for this pattern:
        /// ......
        /// ldfld stateField
        /// (stloc.* stateVariable) - in some cases there is no state variable
        /// ......
        /// </remarks>
        protected bool GetStateFieldAndVariable()
        {
            Instruction currentInstruction = theCFG.Blocks[firstControllerBlock].First;
            while (currentInstruction != theCFG.Blocks[firstControllerBlock].Last)
            {
                if (currentInstruction.OpCode.Code == Code.Ldfld)
                {
                    if (!CheckAndSaveStateField((FieldReference)currentInstruction.Operand))
                    {
                        return false;
                    }
                    TryGetVariableFromInstruction(currentInstruction.Next, out stateVariable);
                    return true;
                }

                currentInstruction = currentInstruction.Next;
            }

            return false;
        }

        /// <summary>
        /// Removes the chain of blocks that represents the state machine controller.
        /// </summary>
        /// <remarks>
        /// The idea is to remove the chain and to create a fake switch data that represents the state machine controller.
        /// </remarks>
        protected bool RemoveControllerChain()
        {
            Queue<InstructionBlock> controllerTraversalQueue = InitializeTheTraversalQueue();
            
            while (controllerTraversalQueue.Count > 0)
            {
                InstructionBlock initialBlock = controllerTraversalQueue.Dequeue();
                InstructionBlock currentBlock = initialBlock;
                int stateNumber;
                StateMachineControllerType controllerType;
                while (IsStateMachineControllerBlock(ref currentBlock, out controllerType, out stateNumber))
                {
                    switch (controllerType)
                    {
                        case StateMachineControllerType.Switch:
                            {
                                InstructionBlock actualSuccessor;
                                SwitchData switchData = theCFG.SwitchBlocksInformation[currentBlock];
                                InstructionBlock[] switchCasesArray = switchData.OrderedCasesArray;
                                for (int i = 0; i < switchCasesArray.Length; i++)
                                {
                                    if (toBeRemoved.Contains(switchCasesArray[i]))
                                    {
                                        continue;
                                    }


                                    switch (TryGetStateEntry(switchCasesArray[i], out actualSuccessor))
                                    {
                                        case ControllerTraversalSearchResult.FoundStateEntry:
                                            stateToStartBlock[i + stateNumber] = actualSuccessor;
                                            break;
                                        case ControllerTraversalSearchResult.FoundControllerCandidate:
                                            stateToStartBlock[i + stateNumber] = actualSuccessor;
                                            controllerTraversalQueue.Enqueue(actualSuccessor);
                                            break;
                                        case ControllerTraversalSearchResult.PatternFailed:
                                            return false;
                                    }
                                }

                                controllerTraversalQueue.Enqueue(SkipBranchChain(switchData.DefaultCase));
                                break;
                            }
                        case StateMachineControllerType.Condition:
                            {
                                InstructionBlock actualSuccessor;
                                switch (TryGetStateEntry(currentBlock.Successors[0], out actualSuccessor))
                                {
                                    case ControllerTraversalSearchResult.FoundStateEntry:
                                        stateToStartBlock[stateNumber] = actualSuccessor;
                                        break;
                                    case ControllerTraversalSearchResult.FoundControllerCandidate:
                                        stateToStartBlock[stateNumber] = actualSuccessor;
                                        controllerTraversalQueue.Enqueue(actualSuccessor);
                                        break;
                                    case ControllerTraversalSearchResult.PatternFailed:
                                        return false;
                                }
                                break;
                            }
                        case StateMachineControllerType.NegativeCondition:
                            {
                                InstructionBlock actualSuccessor;
                                int successorIndex = 1;
                                if (stateNumber == -1)
                                {
                                    stateNumber = 0;
                                    successorIndex = 0;
                                }

                                switch (TryGetStateEntry(currentBlock.Successors[successorIndex], out actualSuccessor))
                                {
                                    case ControllerTraversalSearchResult.FoundStateEntry:
                                    case ControllerTraversalSearchResult.FoundControllerCandidate:
                                        stateToStartBlock[stateNumber] = actualSuccessor;
                                        break;
                                    case ControllerTraversalSearchResult.PatternFailed:
                                        return false;
                                }
                                break;
                            }
                    }

                    toBeRemoved.Add(currentBlock);
                    if (controllerType == StateMachineControllerType.NegativeCondition)
                    {
                        currentBlock = currentBlock.Successors[0];
                    }
                    else
                    {
                        currentBlock = currentBlock.Successors[currentBlock.Successors.Length - 1];
                    }

                    currentBlock = SkipBranchChain(currentBlock);
                }

                if (defaultStateEntry == null)
                {
                    defaultStateEntry = currentBlock;
                }

                ReattachDefaultSuccessor(initialBlock, currentBlock); //Redirect the predecessors of the first controller block to it's default successor

                while (controllerTraversalQueue.Count > 0 && toBeRemoved.Contains(controllerTraversalQueue.Peek()))
                {
                    controllerTraversalQueue.Dequeue();
                }
            }

            if (toBeRemoved.Count == 0)
            {
                return false;
            }

            CreateControllerSwitchData();
            return true;
        }

        /* AGPL */
        /// <summary>
        /// Removes all chains of blocks that represents the state machine controller.
        /// </summary>
        /// <remarks>
        /// The idea is to remove each chain and to create a fake switch data (for each of them) that represents the state machine controller.
        /// </remarks>
        protected bool RemoveControllerChainV2()
        {
            Queue<InstructionBlock> controllerTraversalQueue = new Queue<InstructionBlock>();
            this.switchDataList = new List<SwitchData>();
            HashSet<string> vis = new HashSet<string>();

            foreach (var block in theCFG.Blocks)
            {
                string blockId = block.First.ToString() + block.Last.ToString();
                if (vis.Contains(blockId) || toBeRemoved.Contains(block))
                {
                    continue;
                }
                vis.Add(blockId);

                InstructionBlock _currentBlock = block;
                int _stateNumber;
                StateMachineControllerType _controllerType;
                if (IsStateMachineControllerBlock(ref _currentBlock, out _controllerType, out _stateNumber))
                {
                    InstructionBlock currentStateEntry = null;
                    controllerTraversalQueue.Enqueue(block);
                    while (controllerTraversalQueue.Count > 0)
                    {
                        InstructionBlock initialBlock = controllerTraversalQueue.Dequeue();
                        vis.Add(initialBlock.First.ToString() + initialBlock.Last.ToString());
                        InstructionBlock currentBlock = initialBlock;
                        int stateNumber;
                        StateMachineControllerType controllerType;
                        while (IsStateMachineControllerBlock(ref currentBlock, out controllerType, out stateNumber))
                        {
                            switch (controllerType)
                            {
                                case StateMachineControllerType.Switch:
                                    {
                                        InstructionBlock actualSuccessor;
                                        SwitchData switchData = theCFG.SwitchBlocksInformation[currentBlock];
                                        InstructionBlock[] switchCasesArray = switchData.OrderedCasesArray;
                                        for (int i = 0; i < switchCasesArray.Length; i++)
                                        {
                                            if (toBeRemoved.Contains(switchCasesArray[i]))
                                            {
                                                continue;
                                            }


                                            switch (TryGetStateEntry(switchCasesArray[i], out actualSuccessor))
                                            {
                                                case ControllerTraversalSearchResult.FoundStateEntry:
                                                    stateToStartBlock[i + stateNumber] = actualSuccessor;
                                                    break;
                                                case ControllerTraversalSearchResult.FoundControllerCandidate:
                                                    stateToStartBlock[i + stateNumber] = actualSuccessor;
                                                    controllerTraversalQueue.Enqueue(actualSuccessor);
                                                    break;
                                                case ControllerTraversalSearchResult.PatternFailed:
                                                    return false;
                                            }
                                        }

                                        controllerTraversalQueue.Enqueue(SkipBranchChain(switchData.DefaultCase));
                                        break;
                                    }
                                case StateMachineControllerType.Condition:
                                    {
                                        InstructionBlock actualSuccessor;
                                        switch (TryGetStateEntry(currentBlock.Successors[0], out actualSuccessor))
                                        {
                                            case ControllerTraversalSearchResult.FoundStateEntry:
                                                stateToStartBlock[stateNumber] = actualSuccessor;
                                                break;
                                            case ControllerTraversalSearchResult.FoundControllerCandidate:
                                                stateToStartBlock[stateNumber] = actualSuccessor;
                                                controllerTraversalQueue.Enqueue(actualSuccessor);
                                                break;
                                            case ControllerTraversalSearchResult.PatternFailed:
                                                return false;
                                        }
                                        break;
                                    }
                                case StateMachineControllerType.ConditionV2:
                                    {
                                        // both successors should be preserved. we don't need to skip branch chain since it will be skipped during cleanup.
                                        stateToStartBlock[stateNumber] = SkipBranchChain(currentBlock.Successors[0]);

                                        //InstructionBlock actualSuccessor;
                                        //switch (TryGetStateEntry(currentBlock.Successors[0], out actualSuccessor))
                                        //{
                                        //    case ControllerTraversalSearchResult.FoundStateEntry:
                                        //        stateToStartBlock[stateNumber] = actualSuccessor;
                                        //        break;
                                        //    case ControllerTraversalSearchResult.FoundControllerCandidate:
                                        //        stateToStartBlock[stateNumber] = actualSuccessor;
                                        //        controllerTraversalQueue.Enqueue(actualSuccessor);
                                        //        break;
                                        //    case ControllerTraversalSearchResult.PatternFailed:
                                        //        return false;
                                        //}
                                        break;
                                    }
                                case StateMachineControllerType.NegativeCondition:
                                    {
                                        InstructionBlock actualSuccessor;
                                        int successorIndex = 1;
                                        if (stateNumber == -1)
                                        {
                                            stateNumber = 0;
                                            successorIndex = 0;
                                        }

                                        switch (TryGetStateEntry(currentBlock.Successors[successorIndex], out actualSuccessor))
                                        {
                                            case ControllerTraversalSearchResult.FoundStateEntry:
                                            case ControllerTraversalSearchResult.FoundControllerCandidate:
                                                stateToStartBlock[stateNumber] = actualSuccessor;
                                                break;
                                            case ControllerTraversalSearchResult.PatternFailed:
                                                return false;
                                        }
                                        break;
                                    }
                            }

                            toBeRemoved.Add(currentBlock);
                            if (controllerType == StateMachineControllerType.NegativeCondition)
                            {
                                currentBlock = currentBlock.Successors[0];
                            }
                            else
                            {
                                currentBlock = currentBlock.Successors[currentBlock.Successors.Length - 1];
                            }
                            foreach(var nxtBlock in GetBranchChain(currentBlock))
                            {
                                vis.Add(nxtBlock.First.ToString() + nxtBlock.Last.ToString());
                            }
                            currentBlock = SkipBranchChain(currentBlock);
                        }

                        if(currentStateEntry == null)
                        {
                            currentStateEntry = currentBlock;
                        }

                        ReattachDefaultSuccessor(initialBlock, currentBlock); //Redirect the predecessors of the first controller block to it's default successor

                        while (controllerTraversalQueue.Count > 0 && toBeRemoved.Contains(controllerTraversalQueue.Peek()))
                        {
                            controllerTraversalQueue.Dequeue();
                        }
                    }

                    if (defaultStateEntry == null)
                    {
                        defaultStateEntry = currentStateEntry;
                    }

                    if (toBeRemoved.Count == 0)
                    {
                        return false;
                    }

                    CreateControllerSwitchDataV2(currentStateEntry);
                    this.switchDataList.Add(this.switchData);
                }
            }
            return true;
        }
        /* End AGPL */

        /// <summary>
        /// Initializes the queue that is used for traversing the state controller blocks.
        /// </summary>
        /// <returns></returns>
        protected virtual Queue<InstructionBlock> InitializeTheTraversalQueue()
        {
            Queue<InstructionBlock> theQueue = new Queue<InstructionBlock>();
            theQueue.Enqueue(theCFG.Blocks[firstControllerBlock]);
            return theQueue;
        }

        /// <summary>
        /// Redirects the predecessors of the <paramref name="initialBlock"/> to the <paramref name="currentBlock"/>.
        /// </summary>
        /// <param name="initialBlock"></param>
        /// <param name="currentBlock"></param>
        private void ReattachDefaultSuccessor(InstructionBlock initialBlock, InstructionBlock currentBlock)
        {
            if (initialBlock != currentBlock)
            {
                RedirectNonControllerPredecessors(initialBlock, currentBlock);
                SwapCFGBlocks(initialBlock.Index, currentBlock.Index);
                for (int i = 0; i < stateToStartBlock.Length; i++)
                {
                    if (stateToStartBlock[i] == initialBlock)
                    {
                        stateToStartBlock[i] = currentBlock;
                    }
                }
            }
        }

        /// <summary>
        /// Creates the controller switch data using the information gathered during the traversal of the state controller blocks.
        /// </summary>
        private void CreateControllerSwitchData()
        {
            int index = GetIndexOfLastNonNullElement(stateToStartBlock);
            InstructionBlock[] finalCasesArray = new InstructionBlock[++index];
            //Trim the excess elements of the cases array.
            for (int i = 0; i < index; i++)
            {
                if (stateToStartBlock[i] == null)
                {
                    finalCasesArray[i] = defaultStateEntry;
                }
                else
                {
                    finalCasesArray[i] = stateToStartBlock[i];
                }
            }

            this.switchData = new SwitchData(null, defaultStateEntry, finalCasesArray);
        }

        /* AGPL */
        /// <summary>
        /// Creates a controller switch data using the information gathered during the traversal of the state controller blocks.
        /// </summary>
        private void CreateControllerSwitchDataV2(InstructionBlock stateEntry)
        {
            int index = GetIndexOfLastNonNullElement(stateToStartBlock);
            InstructionBlock[] finalCasesArray = new InstructionBlock[++index];
            //Trim the excess elements of the cases array.
            for (int i = 0; i < index; i++)
            {
                if (stateToStartBlock[i] == null)
                {
                    finalCasesArray[i] = stateEntry;
                }
                else
                {
                    finalCasesArray[i] = stateToStartBlock[i];
                }
            }

            this.switchData = new SwitchData(null, stateEntry, finalCasesArray);
        }
        /* End AGPL */

        protected virtual ControllerTraversalSearchResult TryGetStateEntry(InstructionBlock theBlock, out InstructionBlock actualSuccessor)
        {
            actualSuccessor = SkipBranchChain(theBlock);
            if (IsControllerCondition(actualSuccessor.Last))
            {
                return ControllerTraversalSearchResult.FoundControllerCandidate;
            }
            else
            {
                return ControllerTraversalSearchResult.FoundStateEntry;
            }
        }

        /// <summary>
        /// Checks whether the specified <paramref name="instruction"/> is an instruction at which a state controller block might end.
        /// </summary>
        /// <param name="instruction"></param>
        /// <returns></returns>
        private bool IsControllerCondition(Instruction instruction)
        {
            return IsBeqInstruction(instruction) || IsBneInstruction(instruction) || instruction.OpCode.Code == Code.Switch;
        }

        /// <summary>
        /// Gets the block that is pointed by the unconditional branch chain starting at the specified <paramref name="initialBlock"/>.
        /// </summary>
        /// <param name="initialBlock"></param>
        /// <returns></returns>
        protected InstructionBlock SkipBranchChain(InstructionBlock initialBlock)
        {
            InstructionBlock currentBlock = initialBlock;
            while (IsUnconditionalBranchBlock(currentBlock))
            {
                //toBeRemoved.Add(currentBlock);
                currentBlock = currentBlock.Successors[0];
            }

            return currentBlock;
        }

        /* AGPL */
        private IEnumerable<InstructionBlock> GetBranchChain(InstructionBlock initialBlock)
        {
            InstructionBlock currentBlock = initialBlock;
            while (IsUnconditionalBranchBlock(currentBlock))
            {
                yield return currentBlock;
                currentBlock = currentBlock.Successors[0];
            }
        }
        /* End AGPL */

        /// <summary>
        /// Checks whether the specified block contains usage of the state field.
        /// </summary>
        /// <param name="theBlock"></param>
        /// <returns></returns>
        protected bool ContainsStateFieldLoad(InstructionBlock theBlock)
        {
            Instruction currentInstruction = theBlock.First;
            while (currentInstruction != theBlock.Last)
            {
                if (currentInstruction.OpCode.Code == Code.Ldfld && ((FieldReference)currentInstruction.Operand).Resolve() == this.stateField)
                {
                    return true;
                }

                currentInstruction = currentInstruction.Next;
            }

            return false;
        }

        /// <summary>
        /// Checks whether the specified block contains assignment of the state variable.
        /// </summary>
        /// <param name="theBlock"></param>
        /// <returns></returns>
        private bool ContainsStateVariableSet(InstructionBlock theBlock)
        {
            VariableReference varReference;
            Instruction currentInstruction = theBlock.First;
            while (currentInstruction != theBlock.Last)
            {
                if (IsStloc(currentInstruction) && TryGetVariableFromInstruction(currentInstruction, out varReference) && varReference == this.stateVariable)
                {
                    return true;
                }

                currentInstruction = currentInstruction.Next;
            }

            return false;
        }

        /// <summary>
        /// Checks whether the specified instruction is stloc* instruction.
        /// </summary>
        /// <param name="instruction"></param>
        /// <returns></returns>
        private bool IsStloc(Instruction instruction)
        {
            return instruction.OpCode.Code == Code.Stloc || instruction.OpCode.Code == Code.Stloc_0 || instruction.OpCode.Code == Code.Stloc_1 ||
                instruction.OpCode.Code == Code.Stloc_2 || instruction.OpCode.Code == Code.Stloc_3 || instruction.OpCode.Code == Code.Stloc_S;
        }

        /// <summary>
        /// Detaches the <paramref name="controllerBlock"/> from the CFG by redirecting it's predecessors to it's successors. Skips the predecessors
        /// that are marked for removal.
        /// </summary>
        /// <param name="controllerBlock"></param>
        /// <param name="successor"></param>
        private void RedirectNonControllerPredecessors(InstructionBlock controllerBlock, InstructionBlock successor)
        {
            List<InstructionBlock> predecessors = new List<InstructionBlock>(controllerBlock.Predecessors);

            foreach (InstructionBlock predecessor in predecessors)
            {
                if (!toBeRemoved.Contains(predecessor))
                {
                    successor.Predecessors.Remove(controllerBlock);
                    RedirectSuccessor(predecessor, controllerBlock, successor);
                }
            }
        }

        /// <summary>
        /// Swaps the positions of the two blocks in the CFG.
        /// </summary>
        /// <param name="firstIndex"></param>
        /// <param name="secondIndex"></param>
        private void SwapCFGBlocks(int firstIndex, int secondIndex)
        {
            InstructionBlock firstBlock = theCFG.Blocks[firstIndex];
            InstructionBlock secondBlock = theCFG.Blocks[secondIndex];
            theCFG.Blocks[firstIndex] = secondBlock;
            theCFG.Blocks[secondIndex] = firstBlock;
            firstBlock.Index = secondIndex;
            secondBlock.Index = firstIndex;
        }

        /// <summary>
        /// Gets the index of the last non null element of the specified array.
        /// </summary>
        /// <param name="blocks"></param>
        /// <returns></returns>
        private int GetIndexOfLastNonNullElement(InstructionBlock[] blocks)
        {
            for (int index = blocks.Length - 1; index >= 0; index--)
            {
                if (blocks[index] != null)
                {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Determines wheter the specified block is part of the state machine controller.
        /// </summary>
        /// <remarks>
        /// We look for this pattern:
        /// ...
        /// ldloc.* stateVariable
        /// ldc.i4.* stateNumber
        /// beq* stateEntry
        /// 
        /// or
        /// 
        /// ....
        /// ldloc.* stateVariable
        /// (ldc.i4.* stateNumber) - can be missing
        /// (sub)                  - can be missing
        /// switch ....
        /// 
        /// or
        /// 
        /// ldloc.* stateVariable
        /// ldc.i4.* stateNumber
        /// bnq* default
        /// stateEntry:
        /// .........
        /// 
        /// or
        /// 
        /// ldfld stateField
        /// ldc.i4.* stateNumber
        /// ceq
        /// ldc.i4.0
        /// .....
        /// brtrue* default
        /// stateEntry:
        /// ............
        /// 
        /// In some cases there is no state variable. Instead the state field is loaded.
        /// 
        /// Update:
        /// Since C#6.0 this case is added:
        /// 
        /// ...
        /// ldloc.1
        /// brfalse.s IL_0012
        /// ...
        /// </remarks>
        /// <param name="theBlock"></param>
        /// <param name="stateNumber"></param>
        /// <returns>False if the specified block is not part of the state machine controler, true - otherwise.</returns>
        private bool IsStateMachineControllerBlock(ref InstructionBlock theBlock, out StateMachineControllerType controllerType, out int stateNumber)
        {
            theBlock = SkipBranchChain(theBlock);

            Instruction currentInstruction = theBlock.Last;

            if (theCFG.SwitchBlocksInformation.ContainsKey(theBlock))
            {
                controllerType = StateMachineControllerType.Switch;

                if (currentInstruction.Previous.OpCode.Code == Code.Sub || currentInstruction.Previous.OpCode.Code == Code.Sub_Ovf)
                {
                    currentInstruction = currentInstruction.Previous.Previous;
                    if (!StateMachineUtilities.TryGetOperandOfLdc(currentInstruction, out stateNumber))
                    {
                        return false;
                    }
                }
                else
                {
                    stateNumber = 0;
                }
            }
            else if (IsBeqInstruction(theBlock.Last)) //controllerType == StateMachineControllerType.Conditions
            {
                controllerType = StateMachineControllerType.Condition;

                currentInstruction = theBlock.Last.Previous;
                if (!StateMachineUtilities.TryGetOperandOfLdc(currentInstruction, out stateNumber))
                {
                    return false;
                }
            }
            else if (IsBrFalseInstruction(theBlock.Last))
            {
                controllerType = StateMachineControllerType.Condition;

                currentInstruction = theBlock.Last;
                stateNumber = 0;
            }
            /* AGPL */
            else if (IsBleUnInstruction(theBlock.Last))
            {
                controllerType = StateMachineControllerType.ConditionV2;

                currentInstruction = theBlock.First.Next; // we can also skip setting currentInstruction and just return true here
                stateNumber = 0;
            }
            /* End AGPL */
            else if (IsBneInstruction(theBlock.Last))
            {
                controllerType = StateMachineControllerType.NegativeCondition;

                currentInstruction = theBlock.Last.Previous;
                if (!StateMachineUtilities.TryGetOperandOfLdc(currentInstruction, out stateNumber))
                {
                    return false;
                }
            }
            else if (IsDebugControllerBlock(theBlock, out stateNumber))
            {
                controllerType = StateMachineControllerType.NegativeCondition;
                return true;
            }
            else
            {
                controllerType = StateMachineControllerType.None;
                stateNumber = 0;
                return false;
            }

            VariableReference varReference;
            if (ContainsStateFieldLoad(theBlock) ||
                TryGetVariableFromInstruction(currentInstruction.Previous, out varReference) && varReference == stateVariable &&
                !ContainsStateVariableSet(theBlock))
                //In rare cases the state variable is used in a regular switch block. That's why we check whether there is an assignment of the variable
                //in theBlock. If so then the block is not a controller block.
            {
                /* AGPL */
                return true; // this check may fail for ble if currentInstruction is not set properly
                /* End AGPL */
            }

            stateNumber = 0;
            return false;
        }

        /// <summary>
        /// Determines whether the specified instruction is beq* instruction.
        /// </summary>
        /// <param name="theInstruction"></param>
        /// <returns></returns>
        protected bool IsBeqInstruction(Instruction theInstruction)
        {
            return theInstruction.OpCode.Code == Code.Beq || theInstruction.OpCode.Code == Code.Beq_S;
        }

        /// <summary>
        /// Determine whether the specified instruction is bne* instruction.
        /// </summary>
        /// <param name="theInstruction"></param>
        /// <returns></returns>
        private bool IsBneInstruction(Instruction theInstruction)
        {
            return theInstruction.OpCode.Code == Code.Bne_Un || theInstruction.OpCode.Code == Code.Bne_Un_S;
        }

        /// <summary>
        /// Determines whether the specified instruction is brfalse* instruction.
        /// </summary>
        /// <param name="theInstruction"></param>
        /// <returns></returns>
        private bool IsBrFalseInstruction(Instruction theInstruction)
        {
            return theInstruction.OpCode.Code == Code.Brfalse || theInstruction.OpCode.Code == Code.Brfalse_S;
        }

        /* AGPL */
        private bool IsBleUnInstruction(Instruction theInstruction)
        {
            return theInstruction.OpCode.Code == Code.Ble_Un || theInstruction.OpCode.Code == Code.Ble_Un_S;
        }
        /* End AGPL */

        /// <summary>
        /// Checks whether the specified block is a controller block generated by the compiler in debug mode.
        /// </summary>
        /// <remarks>
        /// Pattern:
        /// 
        /// ldfld stateField
        /// ldc.i4.* stateNumber
        /// ceq
        /// ldc.i4.0
        /// .....
        /// brtrue* .....
        /// </remarks>
        /// <param name="theBlock"></param>
        /// <param name="stateNumber"></param>
        /// <returns></returns>
        private bool IsDebugControllerBlock(InstructionBlock theBlock, out int stateNumber)
        {
            if (theBlock.First == theBlock.Last)
            {
                stateNumber = -1;
                return false;
            }

            Instruction currentInstruction = theBlock.First.Next;
            if (currentInstruction == theBlock.Last || currentInstruction.OpCode.Code != Code.Ldfld ||
                ((FieldReference)currentInstruction.Operand).Resolve() != this.stateField)
            {
                stateNumber = -1;
                return false;
            }

            currentInstruction = currentInstruction.Next;
            if (currentInstruction == theBlock.Last || !StateMachineUtilities.TryGetOperandOfLdc(currentInstruction, out stateNumber))
            {
                stateNumber = -1;
                return false;
            }

            currentInstruction = currentInstruction.Next;
            if (currentInstruction == theBlock.Last || currentInstruction.OpCode.Code != Code.Ceq)
            {
                return false;
            }

            currentInstruction = currentInstruction.Next;
            if (currentInstruction == theBlock.Last || currentInstruction.OpCode.Code != Code.Ldc_I4_0)
            {
                return false;
            }

            return theBlock.Last.OpCode.Code == Code.Brtrue || theBlock.Last.OpCode.Code == Code.Brtrue_S;
        }

        /// <summary>
        /// Removes the control flow edge between <paramref name="theBlock"/> and the <paramref name="oldSuccessor"/> and adds an edge between
        /// <paramref name="theBlock"/> and the <paramref name="newSuccessor"/>.
        /// </summary>
        /// <param name="theBlock"></param>
        /// <param name="oldSuccessor"></param>
        /// <param name="newSuccessor"></param>
        private void RedirectSuccessor(InstructionBlock theBlock, InstructionBlock oldSuccessor, InstructionBlock newSuccessor)
        {
            oldSuccessor.Predecessors.Remove(theBlock);
            for (int i = 0; i < theBlock.Successors.Length; i++)
            {
                if (theBlock.Successors[i] == oldSuccessor)
                {
                    theBlock.Successors[i] = newSuccessor;
                }
            }

            newSuccessor.Predecessors.Add(theBlock);

            SwitchData switchData;
            if (!theCFG.SwitchBlocksInformation.TryGetValue(theBlock, out switchData))
            {
                return;
            }

            for (int i = 0; i < switchData.OrderedCasesArray.Length; i++)
            {
                if (switchData.OrderedCasesArray[i] == oldSuccessor)
                {
                    switchData.OrderedCasesArray[i] = newSuccessor;
                }
            }

            if (switchData.DefaultCase == oldSuccessor)
            {
                switchData.DefaultCase = newSuccessor;
            }
        }

        /// <summary>
        /// Tries to get the variable that is used by the specified instruction.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="varReference"></param>
        /// <returns></returns>
        protected bool TryGetVariableFromInstruction(Instruction instruction, out VariableReference varReference)
        {
            return StateMachineUtilities.TryGetVariableFromInstruction(instruction, methodContext.Body.Variables, out varReference);
        }

        private bool CheckAndSaveStateField(FieldReference foundStateField)
        {
            FieldDefinition foundField = foundStateField.Resolve();
            if (stateField == null)
            {
                stateField = foundField;
            }
            else if (stateField != foundField)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks whether the given block contains only an unconditional branch instruction.
        /// </summary>
        /// <param name="theBlock"></param>
        /// <returns></returns>
        protected virtual bool IsUnconditionalBranchBlock(InstructionBlock theBlock)
        {
            return StateMachineUtilities.IsUnconditionalBranch(theBlock.First);
        }

        protected enum ControllerTraversalSearchResult
        {
            PatternFailed,
            FoundStateEntry,
            FoundControllerCandidate
        }

        enum StateMachineControllerType
        {
            None,
            Switch,
            Condition,
            NegativeCondition,
            /* AGPL */
            ConditionV2
            /* End AGPL */
        }
    }
}
