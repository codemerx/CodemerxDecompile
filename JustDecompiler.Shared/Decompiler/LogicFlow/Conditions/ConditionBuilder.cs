using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Cil;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions
{
    internal class ConditionBuilder
    {
        private readonly LogicalFlowBuilderContext logicalBuilderContext;
        private readonly TypeReference booleanTypeReference;
        private readonly TypeSystem typeSystem;

        public ConditionBuilder(LogicalFlowBuilderContext logicalBuilderContext, TypeSystem typeSystem)
        {
            this.typeSystem = typeSystem;
            this.logicalBuilderContext = logicalBuilderContext;
            this.booleanTypeReference = logicalBuilderContext.CFG.MethodBody.Method.Module.TypeSystem.Boolean;
        }

        /// <summary>
        /// Creates condition logical constructs and merges them (where possible) into complex condition constructs.
        /// </summary>
        /// <param name="theConstruct"></param>
        public void BuildConstructs(ILogicalConstruct theConstruct)
        {
            CreateSimpleConditions();
            CreateComplexConditions(theConstruct);
        }

        /// <summary>
        /// Creates simple conditions containing only one CFG construct (may be partial).
        /// </summary>
        private void CreateSimpleConditions()
        {
            //Since we've split the CFG constructs in the CFGBlockSplitter, we now know that if an instruction block is a condition block,
            //the last of the (partial) CFG constructs holds the condition expression.
            //The only thing left to check is if the block is not a switch block, since switch blocks are a special case and are not proccessed as
            //normal conditions.
            foreach (CFGBlockLogicalConstruct[] cfgConstructsArray in logicalBuilderContext.CFGBlockToLogicalConstructMap.Values)
            {
                CFGBlockLogicalConstruct cfgConstruct = cfgConstructsArray[cfgConstructsArray.Length - 1];
                InstructionBlock theInstructionBlock = cfgConstruct.TheBlock;
                if(theInstructionBlock.Successors.Length == 2 && theInstructionBlock.Successors[0] != theInstructionBlock.Successors[1]
                    && theInstructionBlock.Last.OpCode.Code != Code.Switch)
                {
                    ConditionLogicalConstruct.GroupInSimpleConditionConstruct(cfgConstruct);
                }
            }
        }

        /// <summary>
        /// Merges the simple condition constructs into complex condition constructs representing a short-circuit boolean expression.
        /// </summary>
        /// <param name="theConstruct"></param>
        private void CreateComplexConditions(ILogicalConstruct theConstruct)
        {
            //If the construct is ConditionLC then there is no need to traverse it.
            if(theConstruct is ConditionLogicalConstruct || theConstruct is CFGBlockLogicalConstruct)
            {
                return;
            }

            foreach (ILogicalConstruct child in theConstruct.Children)
            {
                CreateComplexConditions(child);
            }

            //Each iteration tries to find and merge new condition constructs together.
            bool mergedCondition;
            do
            {
                mergedCondition = TryTraverseAndMerge(theConstruct);
            }
            while (mergedCondition);
        }

        /// <summary>
        /// Traverses the graph and tries to merge conditions into complex condition constructs.
        /// </summary>
        /// <param name="theConstruct"></param>
        /// <returns>Returns true if there was a successful merge.</returns>
        private bool TryTraverseAndMerge(ILogicalConstruct theConstruct)
        {
            //The algorithm is split into iterations, because once it merges a complex condition this condition can be used with one of
            //its predecessors to create another complex condition. This means that special modifications are needed to the traversal,
            //which may lead to bugs and is overall more hard to maintain.

            //We use BFS to traverse the subgraph
            HashSet<ILogicalConstruct> traversedNodes = new HashSet<ILogicalConstruct>();
            Queue<ILogicalConstruct> traverseQueue = new Queue<ILogicalConstruct>();
            traverseQueue.Enqueue(theConstruct.Entry as ILogicalConstruct);
            bool changed = false;

            while (traverseQueue.Count > 0)
            {
                ILogicalConstruct currentNode = traverseQueue.Dequeue();
                
                //For each node that is a condition construct we try to create a complex condition starting from it
                ConditionLogicalConstruct currentConditionNode = currentNode as ConditionLogicalConstruct;
                if(currentConditionNode != null)
                {
                    ConditionLogicalConstruct newNode = CreateComplexCondition(currentConditionNode);

                    //If we succeed we mark that there was a change in the subgraph;
                    changed |= newNode != currentNode;

                    //We change the currentNode to the newNode since if there was a merge then the currentNode will be a successor of the newNode
                    currentNode = newNode;
                }

                //We mark the current node as traversed
                traversedNodes.Add(currentNode);

                //Normal bfs continues
                foreach (ILogicalConstruct successor in currentNode.SameParentSuccessors)
                {
                    if(!traversedNodes.Contains(successor))
                    {
                        traverseQueue.Enqueue(successor);
                    }
                }

                while(traverseQueue.Count > 0 && traversedNodes.Contains(traverseQueue.Peek()))
                {
                    traverseQueue.Dequeue();
                }
            }

            return changed;
        }

        /// <summary>
        /// Tries to create a new complex condition starting from the specified.
        /// </summary>
        /// <param name="conditionNode"></param>
        /// <returns></returns>
        private ConditionLogicalConstruct CreateComplexCondition(ConditionLogicalConstruct conditionNode)
        {
            //Explanation:
            //
            //       A
            //      / \
            //     B-> C   (A -> B, C; B -> C, ...; C -> ...)
            //    /
            //A and B - condition nodes, C - common successor
            //
            //This is the representation of every short-circuit boolean expression. Depending on the relation between the conditions, all possible complex
            //boolean expressions can be made. After we've made the new condition from A and B we can continue the check for this type of construction from the
            //new node.

            //So, the purpose of the implemented algorithm is to find the "chain" (directed path containing all the nodes) of all nodes that
            //can be made into a complex condition. The reason for this is to reduce the creating of new logical constructs.

            //Holds the complex condition that we have found so far.
            Expression complexExpression = conditionNode.ConditionExpression;
            //Holds all the nodes that form the complex condition.
            HashSet<ILogicalConstruct> conditionNodes = new HashSet<ILogicalConstruct>();
            //Holds the last node in the "chain", since it's successors are the successors of the complex condition.
            ConditionLogicalConstruct lastNode = conditionNode;
            conditionNodes.Add(lastNode);
            //The true successor of the complex condition.
            ILogicalConstruct trueSuccessor = lastNode.TrueSuccessor;
            //The false successor of the complex condition.
            ILogicalConstruct falseSuccessor = lastNode.FalseSuccessor;
            while(true)
            {
                ConditionLogicalConstruct newConditionNode;
                ILogicalConstruct commonSuccessor;
                BinaryOperator @operator;

                if (CanBePartOfComplexCondition(trueSuccessor, conditionNodes, lastNode.FalseCFGSuccessor))
                {
                    //If the true successor can be added, then the common successor is the false node.
                    newConditionNode = trueSuccessor as ConditionLogicalConstruct;
                    commonSuccessor = falseSuccessor;
                    
                    //This check is to ensure that the common successor is the false successor to both the nodes.
                    if (newConditionNode.FalseSuccessor != commonSuccessor)
                    {
                        newConditionNode.Negate(typeSystem);
                    }

                    //Since both of the conditions have the common successor as false successor, then the binary operation is &&.
                    @operator = BinaryOperator.LogicalAnd;
                }
                else if (CanBePartOfComplexCondition(falseSuccessor, conditionNodes, lastNode.TrueCFGSuccessor))
                {
                    //If the false successor can be added, then the common successor is the true node.
                    newConditionNode = falseSuccessor as ConditionLogicalConstruct;
                    commonSuccessor = trueSuccessor;

                    //This check is to ensure that the common successor is the true successor to both the nodes.
                    if (newConditionNode.TrueSuccessor != commonSuccessor)
                    {
                        newConditionNode.Negate(typeSystem);
                    }

                    //Since both of the conditions have the common successor as the true successor, then the binary operation is ||.
                    @operator = BinaryOperator.LogicalOr;
                }
                else
                {
                    //If we cannot add any of the successors to the condition, we finish the search.
                    break;
                }

                //Update the variables.
                complexExpression = new BinaryExpression(@operator, complexExpression, newConditionNode.ConditionExpression, typeSystem, null);
                complexExpression.ExpressionType = booleanTypeReference;

                lastNode = newConditionNode;
                trueSuccessor = lastNode.TrueSuccessor;
                falseSuccessor = lastNode.FalseSuccessor;
                conditionNodes.Add(lastNode);
            }

            //If we haven't found any other nodes, we return the original condition construct.
            if(conditionNodes.Count == 1)
            {
                return conditionNode;
            }

            //Otherwise we make the new construct and return it.
            HashSet<ConditionLogicalConstruct> complexConditionNodes = new HashSet<ConditionLogicalConstruct>();
            foreach (ConditionLogicalConstruct conditionChild in conditionNodes)
            {
                complexConditionNodes.Add(conditionChild);
            }

            return new ConditionLogicalConstruct(conditionNode, lastNode, complexConditionNodes, complexExpression);
        }

        /// <summary>
        /// Determines whether the specified node can be added in a complex condition.
        /// </summary>
        /// <param name="node">The node that we want to add to the condition.</param>
        /// <param name="nodesInCondition">The nodes that are in the complex condition.</param>
        /// <param name="commonSuccessor">The supposed common successor.</param>
        /// <returns></returns>
        private bool CanBePartOfComplexCondition(ILogicalConstruct node, HashSet<ILogicalConstruct> nodesInCondition, CFGBlockLogicalConstruct commonSuccessor)
        {
            //In order to add the node to the condition it has to be a condition node. All of it's predecessors should be in the complex condition.
            //The node should have for successor the supposed common successor (the check is optimized). And the node should not be part of the complex
            //condition.
            return node != null && node is ConditionLogicalConstruct && ArePredecessorsLegal(node, nodesInCondition) 
                && node.CFGSuccessors.Contains(commonSuccessor) && !nodesInCondition.Contains(node);
        }

        /// <summary>
        /// Determines whether all of the predecessors of the specified node are from the given set of allowed predecessors.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="allowedPredecessors"></param>
        /// <returns></returns>
        private bool ArePredecessorsLegal(ILogicalConstruct node, HashSet<ILogicalConstruct> allowedPredecessors)
        {
            foreach (ILogicalConstruct predecessor in node.SameParentPredecessors)
            {
                if(!allowedPredecessors.Contains(predecessor))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
