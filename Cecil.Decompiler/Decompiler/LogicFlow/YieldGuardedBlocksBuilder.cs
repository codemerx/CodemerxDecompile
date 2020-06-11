using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions;
using Telerik.JustDecompiler.Cil;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DFST;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
    /// <summary>
    /// Rebuilds the removed try/finally constructs, using the yield data from the decompilation context.
    /// </summary>
    /// <remarks>
    /// A very good article that explains the yield state machine: "C# in Depth  Iterator block implementation details.htm".
    /// All of the explanations below presume that the reader has read the mentioned article.
    /// 
    /// There can be yield returns/breaks only inside the try blocks of try/finally constructs in the original code.
    /// The try/finally constructs that contain yield breaks/returns are removed and a "record" for them is made in the Dispose method of the same class.
    /// Usually after making them the compiler generates a guarded block that contains all of the method's logic and a fault handler that will
    /// invoke the Dispose method if an unhandled exception occurs (i.e. try/fault construct).
    /// Since a removed try/finally construct cannot be nested in a normal exception handling contruct (a construct that was not removed by the compiler) and
    /// since this substep is invoked after the GuardedBlocksBuilder, then all of the algorith for the rebuilding can be executed in the topmost block that
    /// contains the logic of the method (i.e. the try block of the try/fault construct or the block of the method if the try/fault construct is not generated).
    /// 
    /// The idea is to have a list of all of the CFGBlockLC children of the block, sorted in reverse post order. This list will be used for
    /// finding the first CFGBlock in which a state begins. Also we ahve a sorted list of all the exception handlers.
    /// The list is sorted in a way that the inner most nested handlers are proccessed first.
    /// So for each exception handler we try to find the block at which the try begins.
    /// Once we have found the entry of the try, we travese the subgraph and add to the try all of the nodes
    /// that are reachable by the entry. The traversal stops when we find the invokation of the finally method
    /// or the conditional disposal that marks the finally block for this exception handler.
    /// After we have created the try/finally construct we update the list of the ordered CFGBlockLC - removing the constructs that are included in the
    /// try/finally.
    /// </remarks>
    internal class YieldGuardedBlocksBuilder
    {
        private readonly LogicalFlowBuilderContext logicalContext;
        private readonly MethodSpecificContext methodContext;

        private BlockLogicalConstruct theBlock;
        private FieldReference stateFieldRef;

        private readonly List<CFGBlockLogicalConstruct> orderedCFGNodes = new List<CFGBlockLogicalConstruct>();
        private readonly Dictionary<TryFinallyLogicalConstruct, YieldExceptionHandlerInfo> createdConstructsToIntervalMap =
            new Dictionary<TryFinallyLogicalConstruct, YieldExceptionHandlerInfo>();

        public YieldGuardedBlocksBuilder(LogicalFlowBuilderContext logicalContext, DecompilationContext decompilationContext)
        {
            this.logicalContext = logicalContext;
            this.methodContext = decompilationContext.MethodContext;
        }

        /// <summary>
        /// Rebuilds the removed try/finally constructs, using the yield data from the decompilation context.
        /// </summary>
        /// <param name="theBlock"></param>
        public void BuildGuardedBlocks(BlockLogicalConstruct theBlock)
        {
            if(methodContext.YieldData == null)
            {
                return;
            }

            this.theBlock = DetermineTheBlock(theBlock);
            this.stateFieldRef = methodContext.YieldData.FieldsInfo.StateHolderField;

            YieldExceptionHandlerInfo[] handlerInfoArray = methodContext.YieldData.ExceptionHandlers;
            Array.Sort(handlerInfoArray); //The order ensures that if we have 2 or more nested handlers, the most inner will be first, then the second most inner, etc.
            GetOrderedCFGNodes();

            foreach (YieldExceptionHandlerInfo handlerInfo in handlerInfoArray)
            {
                GenerateTryFinallyHandler(handlerInfo);
            }
        }

        /// <summary>
        /// Determines the block logical construct in which the algorithm, for recreating the removed try/finally constructs, will be executed.
        /// </summary>
        /// <param name="theBlock"></param>
        /// <returns></returns>
        private BlockLogicalConstruct DetermineTheBlock(BlockLogicalConstruct theBlock)
        {
            if(theBlock.Entry is TryFaultLogicalConstruct &&
                theBlock.Children.Count <= 2) //the try/fault construct and a return block
            {
                return (theBlock.Entry as TryFaultLogicalConstruct).Try;
            }
            else
            {
                return theBlock;
            }
        }

        /// <summary>
        /// Gets the CFGBlockLC children of theBlock sorted in reverse post order.
        /// </summary>
        private void GetOrderedCFGNodes()
        {
            DFSTree dfsTree = DFSTBuilder.BuildTree(theBlock);
            foreach (DFSTNode node in dfsTree.ReversePostOrder)
            {
                CFGBlockLogicalConstruct cfgConstruct = node.Construct as CFGBlockLogicalConstruct;
                if(cfgConstruct != null)
                {
                    orderedCFGNodes.Add(cfgConstruct);
                }
            }
        }

        HashSet<ILogicalConstruct> newTryBody;
        CFGBlockLogicalConstruct entryOfTry;
        CFGBlockLogicalConstruct newFinallyBody;
        HashSet<ILogicalConstruct> finallyBlocks;

        /// <summary>
        /// Generates the try/finally construct that is represented by the specifed exception handler info and attaches it to the logical tree.
        /// </summary>
        /// <param name="handlerInfo"></param>
        private void GenerateTryFinallyHandler(YieldExceptionHandlerInfo handlerInfo)
        {
            finallyBlocks = new HashSet<ILogicalConstruct>();

            entryOfTry = GetStateBeginBlockConstruct(handlerInfo.TryStates);

            BuildTryBody(handlerInfo);

            BlockLogicalConstruct newFinally;
            if (handlerInfo.HandlerType == YieldExceptionHandlerType.Method)
            {
                if (newFinallyBody == null)
                {
                    throw new Exception("Could not determine the end ot the try block");
                }

                RemoveExcessNodesFromTheTryBlock();

                ProcessFinallyNodes();
    
                newFinally = new BlockLogicalConstruct(newFinallyBody, new ILogicalConstruct[] { newFinallyBody });
            }
            else
            {
                newFinally = GenerateFinallyBlock();
            }

            BlockLogicalConstruct newTry = new BlockLogicalConstruct(entryOfTry, newTryBody);
            TryFinallyLogicalConstruct newTryFinallyConstruct = new TryFinallyLogicalConstruct(newTry, newFinally);

            createdConstructsToIntervalMap[newTryFinallyConstruct] = handlerInfo;

            CleanUpOrderedNodes(newTryFinallyConstruct);
        }

        /// <summary>
        /// Builds the try body of the specified exception handler starting from the given entry node.
        /// </summary>
        /// <remarks>
        /// We assume that all nodes before reaching the finally block are in the try construct.
        /// </remarks>
        /// <param name="handlerInfo"></param>
        /// <param name="entryOfTry"></param>
        private void BuildTryBody(YieldExceptionHandlerInfo handlerInfo)
        {
            newTryBody = new HashSet<ILogicalConstruct>();
            newTryBody.Add(entryOfTry);
            newFinallyBody = null;

            HashSet<ILogicalConstruct> traversedNodes = new HashSet<ILogicalConstruct>();
            Queue<ILogicalConstruct> bfsQueue = new Queue<ILogicalConstruct>();
            foreach (ILogicalConstruct successor in entryOfTry.SameParentSuccessors)
            {
                bfsQueue.Enqueue(successor);
            }

            while (bfsQueue.Count > 0)
            {
                ILogicalConstruct currentNode = bfsQueue.Dequeue();
                if (!traversedNodes.Add(currentNode) || finallyBlocks.Contains(currentNode))
                {
                    continue;
                }

                ProcessCurrentNode(handlerInfo, bfsQueue, currentNode);
            }
        }

        /// <summary>
        /// Checks each a node before adding its successors to the <paramref name="bfsQueue"/>.
        /// </summary>
        /// <remarks>
        /// If the node is a CFGBlockLC and it contains the invocation of the finally method, or the entry of the finally block,
        /// then we should not continue the traversal.
        /// </remarks>
        /// <param name="handlerInfo"></param>
        /// <param name="bfsQueue"></param>
        /// <param name="currentNode"></param>
        private void ProcessCurrentNode(YieldExceptionHandlerInfo handlerInfo, Queue<ILogicalConstruct> bfsQueue, ILogicalConstruct currentNode)
        {
            if(currentNode is CFGBlockLogicalConstruct)
            {
                CFGBlockLogicalConstruct currentCFGNode = currentNode as CFGBlockLogicalConstruct;
                for (int i = 0; i < currentCFGNode.LogicalConstructExpressions.Count; i++)
                {
                    Expression currentExpression = currentCFGNode.LogicalConstructExpressions[i];

                    int value;
                    if(TryGetStateAssignValue(currentExpression, out value))
                    {
                        if(!handlerInfo.TryStates.Contains(value)) //sanity check
                        {
                            if(handlerInfo.HandlerType != YieldExceptionHandlerType.Method &&
                                TryProcessConditionalDisposeHandler(handlerInfo, currentCFGNode))
                            {
                                return;
                            }
                            throw new Exception("Invalid state value");
                        }
                    }
                    else if(handlerInfo.HandlerType == YieldExceptionHandlerType.Method &&
                        currentExpression.CodeNodeType == CodeNodeType.MethodInvocationExpression &&
                        (currentExpression as MethodInvocationExpression).MethodExpression.MethodDefinition == handlerInfo.FinallyMethodDefinition)
                    {
                        //For the finally block we need the CFGBlockLC that contains only the invocation of the finally method.
                        //That's why we need to split the CFGBlockLC, if there are other expressions besides the invocation.
                        CFGBlockLogicalConstruct currentFinallyBlock;
                        if (currentCFGNode.LogicalConstructExpressions.Count == 1)
                        {
                            if(newFinallyBody == null)
                            {
                                newFinallyBody = currentCFGNode;
                            }
                            finallyBlocks.Add(newFinallyBody);
                            orderedCFGNodes.Remove(currentCFGNode);
                            return;
                        }

                        if (i == 0)
                        {
                            KeyValuePair<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct> newConstructsPair =
                                LogicalFlowUtilities.SplitCFGBlockAt(logicalContext, currentCFGNode, i + 1);

                            currentFinallyBlock = newConstructsPair.Key;
                            orderedCFGNodes[orderedCFGNodes.IndexOf(currentCFGNode)] = newConstructsPair.Value;
                        }
                        else if (i < currentCFGNode.LogicalConstructExpressions.Count - 1)
                        {
                            KeyValuePair<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct> endOfTryPair =
                                LogicalFlowUtilities.SplitCFGBlockAt(logicalContext, currentCFGNode, i);
                            newTryBody.Add(endOfTryPair.Key);
                            
                            KeyValuePair<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct> finallyRestOfBlockPair =
                                LogicalFlowUtilities.SplitCFGBlockAt(logicalContext, endOfTryPair.Value, 1);

                            currentFinallyBlock = finallyRestOfBlockPair.Key;
                            orderedCFGNodes[orderedCFGNodes.IndexOf(currentCFGNode)] = finallyRestOfBlockPair.Value;
                        }
                        else // i == count - 1
                        {
                            KeyValuePair<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct> tryFinallyPair =
                                LogicalFlowUtilities.SplitCFGBlockAt(logicalContext, currentCFGNode, i);

                            newTryBody.Add(tryFinallyPair.Key);
                            currentFinallyBlock = tryFinallyPair.Value;

                            orderedCFGNodes.Remove(currentCFGNode);
                        }

                        if(newFinallyBody == null)
                        {
                            newFinallyBody = currentFinallyBlock;
                        }
                        finallyBlocks.Add(currentFinallyBlock);

                        return;
                    }
                }
            }
            else if(currentNode is TryFinallyLogicalConstruct)
            {
                TryFinallyLogicalConstruct tryFinallyConstruct = currentNode as TryFinallyLogicalConstruct;
                YieldExceptionHandlerInfo oldHandlerInfo;
                if(createdConstructsToIntervalMap.TryGetValue(tryFinallyConstruct, out oldHandlerInfo) &&
                    oldHandlerInfo.TryStates.IsProperSupersetOf(handlerInfo.TryStates))
                {
                    throw new Exception("This try/finally construct cannot be nested in the current construct");
                }
            }

            newTryBody.Add(currentNode);
            foreach (ILogicalConstruct successor in currentNode.SameParentSuccessors)
            {
                bfsQueue.Enqueue(successor);
            }
        }

        /// <summary>
        /// Removes the nodes added to the new try construct from the orderedCFGNodes collection.
        /// </summary>
        /// <param name="theNewTryConstruct"></param>
        private void CleanUpOrderedNodes(TryFinallyLogicalConstruct theNewTryConstruct)
        {
            foreach (CFGBlockLogicalConstruct cfgChild in theNewTryConstruct.CFGBlocks)
            {
                orderedCFGNodes.Remove(cfgChild);
            }
        }

        /// <summary>
        /// Gets the first found entry CFGBlockLogicalConstruct for a state from the tryStates.
        /// </summary>
        /// <remarks>
        /// We try to get the first CFGBlockLC, that is going to be reached by the control flow, in which the state field is assigned to a state of the
        /// <paramref name="tryStates"/>. If the assignment is not at the begining of the CFGConstruct then we need to split it, since the try begins
        /// at the assignment.
        /// </remarks>
        /// <param name="tryStates"></param>
        /// <returns></returns>
        private CFGBlockLogicalConstruct GetStateBeginBlockConstruct(HashSet<int> tryStates)
        {
            for (int i = 0; i < this.orderedCFGNodes.Count; i++)
            {
                CFGBlockLogicalConstruct currentCFGConstruct = this.orderedCFGNodes[i];

                List<Expression> blockExpressions = currentCFGConstruct.LogicalConstructExpressions;
                for (int j = 0; j < blockExpressions.Count; j++)
                {
                    int value;
                    if (TryGetStateAssignValue(blockExpressions[j], out value) && tryStates.Contains(value))
                    {
                        if (j != 0)
                        {
                            KeyValuePair<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct> newCFGConstructsPair =
                                LogicalFlowUtilities.SplitCFGBlockAt(this.logicalContext, currentCFGConstruct, j);

                            this.orderedCFGNodes[i] = newCFGConstructsPair.Key;
                            this.orderedCFGNodes.Insert(i + 1, newCFGConstructsPair.Value);

                            return newCFGConstructsPair.Value;
                        }
                        else
                        {
                            return currentCFGConstruct;
                        }
                    }
                }
            }

            throw new Exception("Invalid state value");
        }

        /// <summary>
        /// Checks if the given expression is assignment of the state field. If this is so - returns the value via the ref parameter.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool TryGetStateAssignValue(Expression expression, out int value)
        {
            if(expression.CodeNodeType != CodeNodeType.BinaryExpression || !(expression as BinaryExpression).IsAssignmentExpression)
            {
                value = -1;
                return false;
            }

            BinaryExpression theAssignExpression = expression as BinaryExpression;
            if(theAssignExpression.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression ||
                (theAssignExpression.Left as FieldReferenceExpression).Field.Resolve() != this.stateFieldRef)
            {
                value = -1;
                return false;
            }

            if(theAssignExpression.Right.CodeNodeType != CodeNodeType.LiteralExpression)
            {
                throw new Exception("Incorrect value for state field");
            }

            value = Convert.ToInt32((theAssignExpression.Right as LiteralExpression).Value);
            return true;
        }

        /// <summary>
        /// Removes the inner nodes of the try that are reachable by the finally node.
        /// </summary>
        /// <remarks>
        /// Do not traverse the entry of the try since there can be an actual loop.
        /// </remarks>
        private void RemoveExcessNodesFromTheTryBlock()
        {
            HashSet<ILogicalConstruct> markedForTraversal = new HashSet<ILogicalConstruct>(finallyBlocks);
            Queue<ILogicalConstruct> bfsQueue = new Queue<ILogicalConstruct>(finallyBlocks);

            while(bfsQueue.Count > 0)
            {
                ILogicalConstruct current = bfsQueue.Dequeue();
                foreach (ILogicalConstruct successor in current.SameParentSuccessors)
                {
                    if(!markedForTraversal.Contains(successor) && successor != entryOfTry)
                    {
                        markedForTraversal.Add(successor);
                        bfsQueue.Enqueue(successor);
                    }
                }
            }

            foreach (ILogicalConstruct node in markedForTraversal)
            {
                if(node != entryOfTry)
                {
                    newTryBody.Remove(node);
                }
            }
        }

        private void ProcessFinallyNodes()
        {
            foreach (CFGBlockLogicalConstruct finallyBlock in finallyBlocks)
            {
                ProcessFinallyNode(finallyBlock);

                if(finallyBlock != newFinallyBody)
                {
                    DetachFromLogicalTree(finallyBlock);
                }
            }
        }

        /// <summary>
        /// Redirect the predecessors and successors of the finally node.
        /// </summary>
        /// <remarks>
        /// Check if the finally node is only reachable by nodes in the try block.
        /// Make the successor of the finally node successor to all of the predecessors of the finally node.
        /// This will detach the finally node from the subgraph, which is a desired result since every normal finally block is not reachable from any where
        /// (the CLR takes care of executing the code in the finally block).
        /// </remarks>
        /// <param name="finallyCFGBlock"></param>
        private void ProcessFinallyNode(CFGBlockLogicalConstruct finallyCFGBlock)
        {
            ProcessFinallyNode(finallyCFGBlock, finallyCFGBlock);
        }

        private void DetachFromLogicalTree(CFGBlockLogicalConstruct node)
        {
            if(node.CFGPredecessors.Count > 0 || node.CFGSuccessors.Count > 0)
            {
                throw new Exception("This node cannot be detached from the logical tree.");
            }

            node.Parent.Children.Remove(node);
            CFGBlockLogicalConstruct[] cfgConstructsArray = logicalContext.CFGBlockToLogicalConstructMap[node.TheBlock];

            if(cfgConstructsArray.Length == 1)
            {
                if(cfgConstructsArray[0] != node)
                {
                    throw new Exception("Logical tree is inconsistent.");
                }

                logicalContext.CFGBlockToLogicalConstructMap.Remove(node.TheBlock);
            }

            CFGBlockLogicalConstruct[] newCFGConstructsArray = new CFGBlockLogicalConstruct[cfgConstructsArray.Length - 1];
            for (int i = 0 , j = 0; i < cfgConstructsArray.Length; i++, j++)
            {
                if(cfgConstructsArray[i] == node)
                {
                    --j;
                    continue;
                }

                if(j == cfgConstructsArray.Length)
                {
                    throw new Exception("Logical tree is inconsistent.");
                }

                newCFGConstructsArray[j] = cfgConstructsArray[i];
            }

            logicalContext.CFGBlockToLogicalConstructMap[node.TheBlock] = newCFGConstructsArray;
        }

        /// <summary>
        /// Changes the successors of the given instruction block to reflect the changes in the logical tree.
        /// </summary>
        /// <remarks>
        /// Also modifies switch data.
        /// Done because later substeps of the LogicalFlowBuilderStep depend on the successor information of the CFG
        /// (e.g. Determining the true and false successors of a ConditionLogicalConstruct, determining the cases of a switch/NWayConditional construct).
        /// </remarks>
        /// <param name="theCFGConstruct"></param>
        /// <param name="theCFGSuccessor"></param>
        private void ProcessMultiWayCFGPredecessor(CFGBlockLogicalConstruct finallyBody, InstructionBlock theBlock, InstructionBlock theNewSuccessor)
        {
            InstructionBlock theFinallyBlock = finallyBody.TheBlock;

            for (int i = 0; i < theBlock.Successors.Length; i++)
            {
                if (theBlock.Successors[i] == theFinallyBlock)
                {
                    theBlock.Successors[i] = theNewSuccessor;
                }
            }

            SwitchData switchData;
            if(methodContext.ControlFlowGraph.SwitchBlocksInformation.TryGetValue(theBlock, out switchData))
            {
                InstructionBlock[] theConditionCases = switchData.OrderedCasesArray;
                for (int i = 0; i < theConditionCases.Length; i++)
                {
                    if (theConditionCases[i] == theFinallyBlock)
                    {
                        theConditionCases[i] = theNewSuccessor;
                    }
                }

                if (switchData.DefaultCase == theFinallyBlock)
                {
                    switchData.DefaultCase = theNewSuccessor;
                }
            }
        }

        CFGBlockLogicalConstruct finallyEntryBlock;
        CFGBlockLogicalConstruct conditionBlock;
        CFGBlockLogicalConstruct disposeCallBlock;
        /// <summary>
        /// Determines whether or not there is a conditional dispose starting from this block.
        /// </summary>
        /// <remarks>
        /// Since the conditional dispose is the same as in the Dispose method, we are trying to find the same pattern.
        /// <code>
        /// this.stateField = nextState;
        /// (this.disposableField = this.enumeratorField as IDisposable;)   -- this might be missing
        /// if(this.disposableField != null)
        /// {
        ///     this.disposableField.Dispose();
        /// }
        /// </code>
        /// </remarks>
        /// <param name="yieldExceptionHandler"></param>
        /// <param name="startBlock"></param>
        /// <returns></returns>
        private bool TryProcessConditionalDisposeHandler(YieldExceptionHandlerInfo yieldExceptionHandler, CFGBlockLogicalConstruct startBlock)
        {
            if(finallyBlocks.Count > 0)
            {
                return false;
            }

            if(!(startBlock is PartialCFGBlockLogicalConstruct) || startBlock.CFGSuccessors.Count != 1)
            {
                return false;
            }

            if(startBlock.LogicalConstructExpressions.Count == 0)
            {
                return false;
            }
            
            BinaryExpression stateAssignExpression = startBlock.LogicalConstructExpressions[0] as BinaryExpression;
            if(stateAssignExpression == null || !stateAssignExpression.IsAssignmentExpression ||
                stateAssignExpression.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression ||
                (stateAssignExpression.Left as FieldReferenceExpression).Field.Resolve() != stateFieldRef ||
                stateAssignExpression.Right.CodeNodeType != CodeNodeType.LiteralExpression ||
                (int)((stateAssignExpression.Right as LiteralExpression).Value) != yieldExceptionHandler.NextState)
            {
                return false;
            }

            if(startBlock.LogicalConstructExpressions.Count == 2 && yieldExceptionHandler.HandlerType == YieldExceptionHandlerType.ConditionalDispose)
            {
                BinaryExpression disposableAssignExpression = startBlock.LogicalConstructExpressions[1] as BinaryExpression;
                if (disposableAssignExpression == null || !disposableAssignExpression.IsAssignmentExpression ||
                    disposableAssignExpression.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression ||
                    (disposableAssignExpression.Left as FieldReferenceExpression).Field != yieldExceptionHandler.DisposableField ||
                    disposableAssignExpression.Right.CodeNodeType != CodeNodeType.SafeCastExpression ||
                    (disposableAssignExpression.Right as SafeCastExpression).Expression.CodeNodeType != CodeNodeType.FieldReferenceExpression ||
                    ((disposableAssignExpression.Right as SafeCastExpression).Expression as FieldReferenceExpression).Field != yieldExceptionHandler.EnumeratorField)
                {
                    return false;
                }
            }
            else if(startBlock.LogicalConstructExpressions.Count != 1 || yieldExceptionHandler.HandlerType != YieldExceptionHandlerType.SimpleConditionalDispose)
            {
                return false;
            }
            

            CFGBlockLogicalConstruct conditionBlock;
            IEnumerator<CFGBlockLogicalConstruct> enumerator = startBlock.CFGSuccessors.GetEnumerator();
            using(enumerator)
            {
                enumerator.MoveNext();
                conditionBlock = enumerator.Current;
            }

            if(conditionBlock.LogicalConstructExpressions.Count != 1)
            {
                return false;
            }

            BinaryExpression isNullCheckExpression = conditionBlock.LogicalConstructExpressions[0] as BinaryExpression;
            if(isNullCheckExpression == null || isNullCheckExpression.Operator != BinaryOperator.ValueEquality ||
                isNullCheckExpression.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression ||
                (isNullCheckExpression.Left as FieldReferenceExpression).Field != yieldExceptionHandler.DisposableField ||
                isNullCheckExpression.Right.CodeNodeType != CodeNodeType.LiteralExpression ||
                (isNullCheckExpression.Right as LiteralExpression).Value != null)
            {
                return false;
            }

            CFGBlockLogicalConstruct disposeCallBlock = null;
            foreach (ILogicalConstruct successor in conditionBlock.CFGSuccessors)
            {
                CFGBlockLogicalConstruct cfgSuccessor = successor as CFGBlockLogicalConstruct;
                if(cfgSuccessor != null && cfgSuccessor.CFGPredecessors.Count == 1)
                {
                    disposeCallBlock = cfgSuccessor;
                    break;
                }
            }

            if(disposeCallBlock == null || disposeCallBlock.LogicalConstructExpressions.Count != 1)
            {
                return false;
            }

            MethodInvocationExpression disposeMethodInvocation = disposeCallBlock.LogicalConstructExpressions[0] as MethodInvocationExpression;
            if(disposeMethodInvocation == null || !disposeMethodInvocation.VirtualCall ||
                disposeMethodInvocation.MethodExpression.Target.CodeNodeType != CodeNodeType.FieldReferenceExpression ||
                (disposeMethodInvocation.MethodExpression.Target as FieldReferenceExpression).Field != yieldExceptionHandler.DisposableField ||
                disposeMethodInvocation.MethodExpression.Method.Name != "Dispose")
            {
                return false;
            }

            this.finallyEntryBlock = startBlock;
            finallyBlocks.Add(startBlock);

            this.conditionBlock = conditionBlock;
            finallyBlocks.Add(conditionBlock);

            this.disposeCallBlock = disposeCallBlock;
            finallyBlocks.Add(disposeCallBlock);

            return true;
        }

        /// <summary>
        /// Generates the finally block.
        /// </summary>
        /// <remarks>
        /// Since the condition block and the dispose invocation block should have a common successor, a new EmptyBlockLogicalConstruct
        /// is added to the logical tree.
        /// </remarks>
        /// <returns></returns>
        private BlockLogicalConstruct GenerateFinallyBlock()
        {
            CFGBlockLogicalConstruct finallySuccessor = ProcessFinallyNode(finallyEntryBlock, disposeCallBlock);
            finallySuccessor.RemoveFromPredecessors(conditionBlock);
            conditionBlock.RemoveFromSuccessors(finallySuccessor);

            EmptyBlockLogicalConstruct emptyCommonNode = new EmptyBlockLogicalConstruct(++logicalContext.MaxBlockIndex);
            emptyCommonNode.AddToPredecessors(disposeCallBlock);
            emptyCommonNode.AddToPredecessors(conditionBlock);
            disposeCallBlock.AddToSuccessors(emptyCommonNode);
            conditionBlock.AddToSuccessors(emptyCommonNode);

            emptyCommonNode.Parent = finallyEntryBlock.Parent;
            emptyCommonNode.Parent.Children.Add(emptyCommonNode);

            for (int i = 0; i < conditionBlock.TheBlock.Successors.Length; i++)
            {
                if(conditionBlock.TheBlock.Successors[i] == finallySuccessor.TheBlock)
                {
                    conditionBlock.TheBlock.Successors[i] = null;
                }
            }

            finallyBlocks.Add(emptyCommonNode);
            return new BlockLogicalConstruct(finallyEntryBlock, finallyBlocks);
        }


        /// <summary>
        /// Redirect the predecessors and successors of the finally block.
        /// </summary>
        /// <remarks>
        /// Check if the finallyBlockEntry is only reachable by nodes in the try block.
        /// Make the successor of the finallyBlockEnd successor to all of the predecessors of the finallyBlockEntry.
        /// This will detach the finally block from the subgraph, which is a desired result since every normal finally block is not reachable from any where
        /// (the CLR takes care of executing the code in the finally block).
        /// </remarks>
        /// <param name="finallyBlockEntry"></param>
        /// <param name="finallyBlockEnd"></param>
        /// <returns>The successor of the finally block.</returns>
        private CFGBlockLogicalConstruct ProcessFinallyNode(CFGBlockLogicalConstruct finallyBlockEntry, CFGBlockLogicalConstruct finallyBlockEnd)
        {
            foreach (ILogicalConstruct predecessor in finallyBlockEntry.SameParentPredecessors)
            {
                if (!newTryBody.Contains(predecessor))
                {
                    throw new Exception("Invalid entry to the finally block");
                }
            }

            CFGBlockLogicalConstruct finallySuccessor;
            using (IEnumerator<CFGBlockLogicalConstruct> enumerator = finallyBlockEnd.CFGSuccessors.GetEnumerator())
            {
                enumerator.MoveNext();
                finallySuccessor = enumerator.Current;

                if (enumerator.MoveNext())
                {
                    throw new Exception("Invalid count of successors");
                }
            }

            HashSet<CFGBlockLogicalConstruct> finallyCFGPredecessors = new HashSet<CFGBlockLogicalConstruct>(finallyBlockEntry.CFGPredecessors);
            foreach (CFGBlockLogicalConstruct cfgPredecessor in finallyCFGPredecessors)
            {
                if (cfgPredecessor.TheBlock != finallyBlockEntry.TheBlock && cfgPredecessor.TheBlock.Successors.Length > 1)
                {
                    ProcessMultiWayCFGPredecessor(finallyBlockEntry, cfgPredecessor.TheBlock, finallySuccessor.TheBlock);
                }

                LogicalConstructBase currenConstruct = cfgPredecessor;
                while (currenConstruct != finallyBlockEntry.Parent)
                {
                    currenConstruct.RemoveFromSuccessors(finallyBlockEntry);
                    currenConstruct.AddToSuccessors(finallySuccessor);

                    currenConstruct = currenConstruct.Parent as LogicalConstructBase;
                }

                finallySuccessor.AddToPredecessors(cfgPredecessor);
                finallyBlockEntry.RemoveFromPredecessors(cfgPredecessor);
            }

            finallySuccessor.RemoveFromPredecessors(finallyBlockEnd);
            finallyBlockEnd.RemoveFromSuccessors(finallySuccessor);

            return finallySuccessor;
        }
    }
}
