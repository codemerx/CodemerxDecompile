using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DFST;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DTree;
using Telerik.JustDecompiler.Cil;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions
{
    internal class IfBuilder : DominatorTreeDependentStep
    {
        private readonly TypeSystem typeSystem;
		private readonly Dictionary<int, int> blockToInstructionsCount = new Dictionary<int, int>();

        public IfBuilder(LogicalFlowBuilderContext logicalContext, TypeSystem typeSystem)
            :base(logicalContext)
        {
            this.typeSystem = typeSystem;
        }

        /// <summary>
        /// Builds if constructs in the specified construct and all of it's children.
        /// </summary>
        /// <param name="construct"></param>
        public void BuildConstructs(ILogicalConstruct construct)
        {
            if(construct is CFGBlockLogicalConstruct || construct is ConditionLogicalConstruct)
            {
                return;
            }

            foreach (ILogicalConstruct child in construct.Children)
            {
                BuildConstructs(child);
            }

            BuildIfConstructs(construct);
        }

        /// <summary>
        /// Builds if constructs in the specified construct.
        /// </summary>
        /// <param name="construct"></param>
        private void BuildIfConstructs(ILogicalConstruct construct)
        {
            DominatorTree dominatorTree = GetDominatorTreeFromContext(construct);
            DFSTree dfsTree = DFSTBuilder.BuildTree(construct);
            foreach (ConditionLogicalConstruct condition in GetPostOrderedIfConditionCandidates(dfsTree))
            {
                TryBuildIfConstruct(condition, dominatorTree, dfsTree);
            }
        }

        /// <summary>
        /// Gets the candidates to become conditions of if constructs in postorder.
        /// </summary>
        /// <param name="construct"></param>
        /// <returns></returns>
        private IEnumerable<ConditionLogicalConstruct> GetPostOrderedIfConditionCandidates(DFSTree dfsTree)
        {
            //The post order is so that we start making the if constructs from the innermost nested first (if there is nesting).

            for (int i = dfsTree.ReversePostOrder.Count - 1; i >= 0; i--)
            {
                ConditionLogicalConstruct currentConstruct = dfsTree.ReversePostOrder[i].Construct as ConditionLogicalConstruct;

                //For candidates we take these conditions that have 2 same parent successors.
                //TODO: consider taking the conditions with 1 same parent successor.
                if (currentConstruct != null && currentConstruct.SameParentSuccessors.Count == 2)
                {
                    yield return currentConstruct;
                }
            }
        }

        /// <summary>
        /// Tries to build an if construct with condition - the specified condition.
        /// </summary>
        /// <remarks>
        /// The idea is to get the dominated nodes of the true successor to create the then block and the dominated nodes of the false successor
        /// to create the else block.
        /// If both the then and else blocks have successors, then they must have a common successor to create the if construct.
        /// </remarks>
        /// <param name="condition"></param>
        /// <returns>True on success.</returns>
		private bool TryBuildIfConstruct(ConditionLogicalConstruct condition, DominatorTree dominatorTree, DFSTree dfsTree)
		{
            //Store the true and false successors for optimization.
            ILogicalConstruct falseSuccessor = condition.FalseSuccessor;
            ILogicalConstruct trueSuccessor = condition.TrueSuccessor;

            HashSet<ISingleEntrySubGraph> falseSuccessorFrontier = dominatorTree.GetDominanceFrontier(falseSuccessor);
            HashSet<ISingleEntrySubGraph> trueSuccessorFrontier = dominatorTree.GetDominanceFrontier(trueSuccessor);

            ILogicalConstruct exitSuccessor = CheckSuccessor(condition, trueSuccessor, falseSuccessorFrontier, dfsTree) ??
                CheckSuccessor(condition, falseSuccessor, trueSuccessorFrontier, dfsTree);

            HashSet<ISingleEntrySubGraph> frontierIntersection = new HashSet<ISingleEntrySubGraph>(trueSuccessorFrontier);
            frontierIntersection.IntersectWith(falseSuccessorFrontier);

            if (exitSuccessor == null && falseSuccessorFrontier.Count > 0 && trueSuccessorFrontier.Count > 0 && frontierIntersection.Count == 0)
            {
                //If none of the successors can be a proper exit and the false and true successor frontiers are not empty but have no common node,
                //then we do not make the if since it will not have a common exit.
                return false;
            }

            HashSet<ILogicalConstruct> thenBody = GetBlockBody(dominatorTree, trueSuccessor, condition);
            HashSet<ILogicalConstruct> elseBody = GetBlockBody(dominatorTree, falseSuccessor, condition);

			if (thenBody == null && elseBody == null)
            {
                return false;
            }
            else if(thenBody == null)
            {
                condition.Negate(typeSystem);

                ILogicalConstruct swapHelper = trueSuccessor;
                trueSuccessor = falseSuccessor;
                falseSuccessor = swapHelper;

                thenBody = elseBody;
                elseBody = null;
            }

            //If the else body is null but the false successor is not a successor of the then body then we do not make the if.
            if(elseBody == null && !CheckSuccessors(thenBody, falseSuccessor))
            {
                return false;
            }

			if (ShouldInvertIfAndRemoveElse(thenBody, trueSuccessor, elseBody, falseSuccessor))
			{
				///This is performed for cosmetic reasons.
				condition.Negate(typeSystem);

				ILogicalConstruct successorSwapHelper = trueSuccessor;
				trueSuccessor = falseSuccessor;
				falseSuccessor = successorSwapHelper;

				HashSet<ILogicalConstruct> swapHelper = thenBody;
				thenBody = elseBody;
				elseBody = swapHelper;
				elseBody = null;
			}
			if (elseBody != null && !HasSuccessors(thenBody) && 
				SubtreeEndsInInstructionCode(trueSuccessor.FirstBlock.TheBlock,new Code[]{Code.Ret, Code.Throw})) // check if all ends are throw and/or return -> allow mixed ends as well
			{
				// we don't need the else
				elseBody = null;
			}

            BlockLogicalConstruct theThenBlock = new BlockLogicalConstruct(trueSuccessor, thenBody);
            BlockLogicalConstruct theElseBlock = elseBody != null ? new BlockLogicalConstruct(falseSuccessor, elseBody) : null;

			IfLogicalConstruct theIfConstruct = IfLogicalConstruct.GroupInIfConstruct(condition, theThenBlock, theElseBlock);
            UpdateDominatorTree(dominatorTree, theIfConstruct);
            return true;
		}

		/// <summary>
		/// Checks if the true and false succsessor should be swapped, so that the end code will be more readable.
		/// </summary>
		/// <param name="trueSuccessor">Collection of logical constructs, that form the body of the "then" block.</param>
		/// <param name="trueBlockEntry">The logical construct that is the entry point of the "then" block. This is the successor of the condition jump that will be entered when the condition is evaluated as "true".</param>
		/// <param name="falseSuccessor">Collection of logical constructs, that form the body of the "else" block.</param>
		/// <param name="falseBlockEntry">The logical construct that is the entry point of the "else" block. This is the successor of the condition jump that will be entered when the condition is evaluated as "false".</param>
		/// <returns></returns>
		private bool ShouldInvertIfAndRemoveElse(ICollection<ILogicalConstruct> trueSuccessor, ILogicalConstruct trueBlockEntry, ICollection<ILogicalConstruct> falseSuccessor, ILogicalConstruct falseBlockEntry)
		{
			/// Whole this method should be done before creating the BlockLogicalConstructs for then and else, because this will change the parents for most of the 
			/// nodes.

			if (falseSuccessor == null)
			{
				return false;
			}
			bool falseHasSuccessors = HasSuccessors(falseSuccessor);

			if (!falseHasSuccessors)
			{
				// All paths in the false successor exit from the program
				bool trueHasSuccessors = HasSuccessors(trueSuccessor);
				if (trueHasSuccessors)
				{
					// The "then" block doesn't exit at all from the method.
					return true;
				}
				if (!AllEndsAreThrow(trueBlockEntry))
				{
					if (HasLessOrEqualInstructions(trueSuccessor, falseSuccessor))
					{
						/// If the "else" block is shorter, or has the same number of instructions
						/// and if the "then" block doesn't exit only via "throw" statements, then we invert
						return true;
					}
					else if (AllEndsAreThrow(falseBlockEntry))
					{
						/// If the "else" block ends only in throws, and the "then" block exits via returns or returns and throws,
						/// we should invert.
						return true;
					}
				}
			}
			return false;
		}
  
		private bool HasSuccessors(ICollection<ILogicalConstruct> block)
		{
			foreach (ILogicalConstruct construct in block)
			{
				foreach (ILogicalConstruct possibleSuccessor in construct.AllSuccessors)
				{
					if (!block.Contains(possibleSuccessor))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool AllEndsAreThrow(ILogicalConstruct entry)
		{
			CFGBlockLogicalConstruct entryBlock = entry.FirstBlock;
			if (!SubtreeEndsInInstructionCode(entryBlock.TheBlock, new Code[] { Code.Throw }))
			{
				return false;
			}
			return true;
		}

		private bool SubtreeEndsInInstructionCode(InstructionBlock entryBlock, IEnumerable<Code> operationCodes)
		{
			bool result = true;
			Queue<InstructionBlock> blocksQueue = new Queue<InstructionBlock>();
			HashSet<int> visited = new HashSet<int>();
			blocksQueue.Enqueue(entryBlock);
			while (blocksQueue.Count > 0 && result)
			{
				InstructionBlock currentBlock = blocksQueue.Dequeue();
				if (visited.Contains(currentBlock.First.Offset))
				{
					continue;
				}
				visited.Add(currentBlock.First.Offset);
				if (currentBlock.Successors.Length == 0) // the entry block is the only block
				{
					bool blockResult = false;
					foreach (Code operationCode in operationCodes)
					{
						blockResult |= currentBlock.Last.OpCode.Code == operationCode;
					}
					result &= blockResult;
					continue;
				}
				foreach (var successor in currentBlock.Successors) //INFINITE CYCLE FFS
				{
					blocksQueue.Enqueue(successor);
				}
			}
			return result;
		}

		private bool HasLessOrEqualInstructions(IEnumerable<ILogicalConstruct> trueSuccessor, IEnumerable<ILogicalConstruct> falseSuccessor)
		{
			int trueSuccessorInstructionCount = CountInstructions(trueSuccessor);
			int falseSuccessorInstructionCount = CountInstructions(falseSuccessor);
			return falseSuccessorInstructionCount <= trueSuccessorInstructionCount;
		}
  
		private int CountInstructions(IEnumerable<ILogicalConstruct> trueSuccessor)
		{
			int result = 0;
			foreach (ILogicalConstruct construct in trueSuccessor)
			{
				foreach (CFGBlockLogicalConstruct block in construct.CFGBlocks)
				{
					int blockResult = 0;
					if (blockToInstructionsCount.ContainsKey(block.TheBlock.First.Offset))
					{
						blockResult = blockToInstructionsCount[block.TheBlock.First.Offset];
					}
					else
					{
						foreach (Instruction instruction in block.TheBlock) // this is basicaly the LINQ's Count on IEnumerable
						{
							blockResult++;
						}
						blockToInstructionsCount.Add(block.TheBlock.First.Offset, blockResult);
					}
					result += blockResult;
				}
			}
			return result;
		}

        /// <summary>
        /// Checks if the specified condition successor can be successor of the if construct.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="conditionSuccessor">The condition successor.</param>
        /// <param name="otherSuccessorFrontier">The dominance frontier of the other successor.</param>
        /// <returns>On success - the successor. Otherwise null.</returns>
        private ILogicalConstruct CheckSuccessor(ILogicalConstruct condition, ILogicalConstruct conditionSuccessor,
            HashSet<ISingleEntrySubGraph> otherSuccessorFrontier, DFSTree dfsTree)
        {
            //In order the condition successor to be successor of the if, it has to be in the dominance frontier of the other successor.
            //Also the edge between the condition and the successor should not be backedge or crossedge.
            DFSTNode successorNode;
            if(otherSuccessorFrontier.Contains(conditionSuccessor) &&
                (!dfsTree.ConstructToNodeMap.TryGetValue(conditionSuccessor, out successorNode) || dfsTree.ConstructToNodeMap[condition].CompareTo(successorNode) < 0))
            {
                return conditionSuccessor;
            }

            return null;
        }

        /// <summary>
        /// Gets the dominated nodes of the condition successor, only if they can form a legal block for the if construct.
        /// </summary>
        /// <param name="dominatorTree"></param>
        /// <param name="conditionSuccessor"></param>
        /// <returns>On success - a set of the nodes fo the block. Otherwise it returns null.</returns>
        private HashSet<ILogicalConstruct> GetBlockBody(DominatorTree dominatorTree, ILogicalConstruct conditionSuccessor, ConditionLogicalConstruct theCondition)
        {
            if(conditionSuccessor == dominatorTree.RootConstruct) //Corner case - the successor cannot be the entry of the construct.
            {
                return null;
            }

            HashSet<ILogicalConstruct> body = null;
            if(conditionSuccessor.AllPredecessors.Count == 1) //The condition successor must have only one predecessor - the condition.
            {
                body = new HashSet<ILogicalConstruct>();
                foreach (ILogicalConstruct node in dominatorTree.GetDominatedNodes(conditionSuccessor))
                {
                    if (node == theCondition)
                    {
                        return null;
                    }
                    body.Add(node);
                }
            }

            return body;
        }

        /// <summary>
        /// Determines whether or not the specified body has the specified successor as same parent successor or has no same parent successors.
        /// </summary>
        /// <param name="theBody"></param>
        /// <param name="successor"></param>
        /// <returns></returns>
        private bool CheckSuccessors(HashSet<ILogicalConstruct> theBody, ILogicalConstruct successor)
        {
            bool hasSuccessors = false;
            foreach (ILogicalConstruct node in theBody)
            {
                foreach (ILogicalConstruct nodeSuccessor in node.SameParentSuccessors)
                {
                    if(theBody.Contains(nodeSuccessor)) //Out going successors only.
                    {
                        continue;
                    }

                    hasSuccessors = true;

                    if(nodeSuccessor == successor)
                    {
                        return true;
                    }
                }
            }

            return !hasSuccessors;
        }

        private void UpdateDominatorTree(DominatorTree dominatorTree, IfLogicalConstruct theIfConstruct)
        {
            HashSet<ISingleEntrySubGraph> ifNodes = new HashSet<ISingleEntrySubGraph>();
            ifNodes.Add(theIfConstruct.Condition);
            ifNodes.UnionWith(theIfConstruct.Then.Children);
            if (theIfConstruct.Else != null)
            {
                ifNodes.UnionWith(theIfConstruct.Else.Children);
            }

            dominatorTree.MergeNodes(ifNodes, theIfConstruct.Condition, theIfConstruct);
        }
    }
}
