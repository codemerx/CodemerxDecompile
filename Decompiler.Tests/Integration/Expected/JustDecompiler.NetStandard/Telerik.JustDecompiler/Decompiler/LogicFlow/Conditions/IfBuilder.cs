using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Common;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DFST;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DTree;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions
{
	internal class IfBuilder : DominatorTreeDependentStep
	{
		private readonly TypeSystem typeSystem;

		private readonly Dictionary<int, int> blockToInstructionsCount = new Dictionary<int, int>();

		public IfBuilder(LogicalFlowBuilderContext logicalContext, TypeSystem typeSystem) : base(logicalContext)
		{
			this.typeSystem = typeSystem;
		}

		private bool AllEndsAreThrow(ILogicalConstruct entry)
		{
			if (!this.SubtreeEndsInInstructionCode(entry.FirstBlock.TheBlock, (IEnumerable<Code>)(new Code[] { Code.Throw })))
			{
				return false;
			}
			return true;
		}

		public void BuildConstructs(ILogicalConstruct construct)
		{
			if (construct is CFGBlockLogicalConstruct || construct is ConditionLogicalConstruct)
			{
				return;
			}
			foreach (ILogicalConstruct child in construct.Children)
			{
				this.BuildConstructs(child);
			}
			this.BuildIfConstructs(construct);
		}

		private void BuildIfConstructs(ILogicalConstruct construct)
		{
			DominatorTree dominatorTreeFromContext = base.GetDominatorTreeFromContext(construct);
			DFSTree dFSTree = DFSTBuilder.BuildTree(construct);
			foreach (ConditionLogicalConstruct postOrderedIfConditionCandidate in this.GetPostOrderedIfConditionCandidates(dFSTree))
			{
				this.TryBuildIfConstruct(postOrderedIfConditionCandidate, dominatorTreeFromContext, dFSTree);
			}
		}

		private ILogicalConstruct CheckSuccessor(ILogicalConstruct condition, ILogicalConstruct conditionSuccessor, HashSet<ISingleEntrySubGraph> otherSuccessorFrontier, DFSTree dfsTree)
		{
			DFSTNode dFSTNode;
			if (otherSuccessorFrontier.Contains(conditionSuccessor) && (!dfsTree.ConstructToNodeMap.TryGetValue(conditionSuccessor, out dFSTNode) || dfsTree.ConstructToNodeMap[condition].CompareTo(dFSTNode) < 0))
			{
				return conditionSuccessor;
			}
			return null;
		}

		private bool CheckSuccessors(HashSet<ILogicalConstruct> theBody, ILogicalConstruct successor)
		{
			bool flag;
			bool flag1 = false;
			HashSet<ILogicalConstruct>.Enumerator enumerator = theBody.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					HashSet<ISingleEntrySubGraph>.Enumerator enumerator1 = enumerator.Current.SameParentSuccessors.GetEnumerator();
					try
					{
						while (enumerator1.MoveNext())
						{
							ILogicalConstruct current = (ILogicalConstruct)enumerator1.Current;
							if (theBody.Contains(current))
							{
								continue;
							}
							flag1 = true;
							if (current != successor)
							{
								continue;
							}
							flag = true;
							return flag;
						}
					}
					finally
					{
						((IDisposable)enumerator1).Dispose();
					}
				}
				return !flag1;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private int CountInstructions(IEnumerable<ILogicalConstruct> trueSuccessor)
		{
			int num = 0;
			foreach (ILogicalConstruct logicalConstruct in trueSuccessor)
			{
				foreach (CFGBlockLogicalConstruct cFGBlock in logicalConstruct.CFGBlocks)
				{
					int item = 0;
					if (!this.blockToInstructionsCount.ContainsKey(cFGBlock.TheBlock.First.Offset))
					{
						foreach (Instruction theBlock in cFGBlock.TheBlock)
						{
							item++;
						}
						this.blockToInstructionsCount.Add(cFGBlock.TheBlock.First.Offset, item);
					}
					else
					{
						item = this.blockToInstructionsCount[cFGBlock.TheBlock.First.Offset];
					}
					num += item;
				}
			}
			return num;
		}

		private HashSet<ILogicalConstruct> GetBlockBody(DominatorTree dominatorTree, ILogicalConstruct conditionSuccessor, ConditionLogicalConstruct theCondition)
		{
			HashSet<ILogicalConstruct> logicalConstructs;
			if (conditionSuccessor == dominatorTree.RootConstruct)
			{
				return null;
			}
			HashSet<ILogicalConstruct> logicalConstructs1 = null;
			if (conditionSuccessor.AllPredecessors.Count == 1)
			{
				logicalConstructs1 = new HashSet<ILogicalConstruct>();
				HashSet<ISingleEntrySubGraph>.Enumerator enumerator = dominatorTree.GetDominatedNodes(conditionSuccessor).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ILogicalConstruct current = (ILogicalConstruct)enumerator.Current;
						if (current != theCondition)
						{
							logicalConstructs1.Add(current);
						}
						else
						{
							logicalConstructs = null;
							return logicalConstructs;
						}
					}
					return logicalConstructs1;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return logicalConstructs;
			}
			return logicalConstructs1;
		}

		private IEnumerable<ConditionLogicalConstruct> GetPostOrderedIfConditionCandidates(DFSTree dfsTree)
		{
			for (int i = dfsTree.ReversePostOrder.Count - 1; i >= 0; i--)
			{
				ConditionLogicalConstruct construct = dfsTree.ReversePostOrder[i].Construct as ConditionLogicalConstruct;
				if (construct != null && construct.SameParentSuccessors.Count == 2)
				{
					yield return construct;
				}
			}
		}

		private bool HasLessOrEqualInstructions(IEnumerable<ILogicalConstruct> trueSuccessor, IEnumerable<ILogicalConstruct> falseSuccessor)
		{
			int num = this.CountInstructions(trueSuccessor);
			return this.CountInstructions(falseSuccessor) <= num;
		}

		private bool HasSuccessors(ICollection<ILogicalConstruct> block)
		{
			bool flag;
			using (IEnumerator<ILogicalConstruct> enumerator = block.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					HashSet<ISingleEntrySubGraph>.Enumerator enumerator1 = enumerator.Current.AllSuccessors.GetEnumerator();
					try
					{
						while (enumerator1.MoveNext())
						{
							if (block.Contains((ILogicalConstruct)enumerator1.Current))
							{
								continue;
							}
							flag = true;
							return flag;
						}
					}
					finally
					{
						((IDisposable)enumerator1).Dispose();
					}
				}
				return false;
			}
			return flag;
		}

		private bool ShouldInvertIfAndRemoveElse(ICollection<ILogicalConstruct> trueSuccessor, ILogicalConstruct trueBlockEntry, ICollection<ILogicalConstruct> falseSuccessor, ILogicalConstruct falseBlockEntry)
		{
			if (falseSuccessor == null)
			{
				return false;
			}
			if (!this.HasSuccessors(falseSuccessor))
			{
				if (this.HasSuccessors(trueSuccessor))
				{
					return true;
				}
				if (!this.AllEndsAreThrow(trueBlockEntry))
				{
					if (this.HasLessOrEqualInstructions(trueSuccessor, falseSuccessor))
					{
						return true;
					}
					if (this.AllEndsAreThrow(falseBlockEntry))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool SubtreeEndsInInstructionCode(InstructionBlock entryBlock, IEnumerable<Code> operationCodes)
		{
			bool flag = true;
			Queue<InstructionBlock> instructionBlocks = new Queue<InstructionBlock>();
			HashSet<int> nums = new HashSet<int>();
			instructionBlocks.Enqueue(entryBlock);
			while (instructionBlocks.Count > 0 & flag)
			{
				InstructionBlock instructionBlocks1 = instructionBlocks.Dequeue();
				if (nums.Contains(instructionBlocks1.First.Offset))
				{
					continue;
				}
				nums.Add(instructionBlocks1.First.Offset);
				if (instructionBlocks1.Successors.Length != 0)
				{
					InstructionBlock[] successors = instructionBlocks1.Successors;
					for (int i = 0; i < (int)successors.Length; i++)
					{
						instructionBlocks.Enqueue(successors[i]);
					}
				}
				else
				{
					bool code = false;
					foreach (Code operationCode in operationCodes)
					{
						OpCode opCode = instructionBlocks1.Last.OpCode;
						code = code | opCode.Code == operationCode;
					}
					flag &= code;
				}
			}
			return flag;
		}

		private bool TryBuildIfConstruct(ConditionLogicalConstruct condition, DominatorTree dominatorTree, DFSTree dfsTree)
		{
			BlockLogicalConstruct blockLogicalConstruct;
			ILogicalConstruct falseSuccessor = condition.FalseSuccessor;
			ILogicalConstruct trueSuccessor = condition.TrueSuccessor;
			HashSet<ISingleEntrySubGraph> dominanceFrontier = dominatorTree.GetDominanceFrontier(falseSuccessor);
			HashSet<ISingleEntrySubGraph> singleEntrySubGraphs = dominatorTree.GetDominanceFrontier(trueSuccessor);
			ILogicalConstruct logicalConstruct = this.CheckSuccessor(condition, trueSuccessor, dominanceFrontier, dfsTree) ?? this.CheckSuccessor(condition, falseSuccessor, singleEntrySubGraphs, dfsTree);
			HashSet<ISingleEntrySubGraph> singleEntrySubGraphs1 = new HashSet<ISingleEntrySubGraph>(singleEntrySubGraphs);
			singleEntrySubGraphs1.IntersectWith(dominanceFrontier);
			if (logicalConstruct == null && dominanceFrontier.Count > 0 && singleEntrySubGraphs.Count > 0 && singleEntrySubGraphs1.Count == 0)
			{
				return false;
			}
			HashSet<ILogicalConstruct> blockBody = this.GetBlockBody(dominatorTree, trueSuccessor, condition);
			HashSet<ILogicalConstruct> logicalConstructs = this.GetBlockBody(dominatorTree, falseSuccessor, condition);
			if (blockBody == null && logicalConstructs == null)
			{
				return false;
			}
			if (blockBody == null)
			{
				condition.Negate(this.typeSystem);
				ILogicalConstruct logicalConstruct1 = trueSuccessor;
				trueSuccessor = falseSuccessor;
				falseSuccessor = logicalConstruct1;
				blockBody = logicalConstructs;
				logicalConstructs = null;
			}
			if (logicalConstructs == null && !this.CheckSuccessors(blockBody, falseSuccessor))
			{
				return false;
			}
			if (this.ShouldInvertIfAndRemoveElse(blockBody, trueSuccessor, logicalConstructs, falseSuccessor))
			{
				condition.Negate(this.typeSystem);
				ILogicalConstruct logicalConstruct2 = trueSuccessor;
				trueSuccessor = falseSuccessor;
				falseSuccessor = logicalConstruct2;
				HashSet<ILogicalConstruct> logicalConstructs1 = blockBody;
				blockBody = logicalConstructs;
				logicalConstructs = logicalConstructs1;
				logicalConstructs = null;
			}
			if (logicalConstructs != null && !this.HasSuccessors(blockBody))
			{
				if (this.SubtreeEndsInInstructionCode(trueSuccessor.FirstBlock.TheBlock, (IEnumerable<Code>)(new Code[] { Code.Ret, Code.Throw })))
				{
					logicalConstructs = null;
				}
			}
			BlockLogicalConstruct blockLogicalConstruct1 = new BlockLogicalConstruct(trueSuccessor, blockBody);
			if (logicalConstructs != null)
			{
				blockLogicalConstruct = new BlockLogicalConstruct(falseSuccessor, logicalConstructs);
			}
			else
			{
				blockLogicalConstruct = null;
			}
			this.UpdateDominatorTree(dominatorTree, IfLogicalConstruct.GroupInIfConstruct(condition, blockLogicalConstruct1, blockLogicalConstruct));
			return true;
		}

		private void UpdateDominatorTree(DominatorTree dominatorTree, IfLogicalConstruct theIfConstruct)
		{
			HashSet<ISingleEntrySubGraph> singleEntrySubGraphs = new HashSet<ISingleEntrySubGraph>();
			singleEntrySubGraphs.Add(theIfConstruct.Condition);
			singleEntrySubGraphs.UnionWith(theIfConstruct.Then.Children);
			if (theIfConstruct.Else != null)
			{
				singleEntrySubGraphs.UnionWith(theIfConstruct.Else.Children);
			}
			dominatorTree.MergeNodes(singleEntrySubGraphs, theIfConstruct.Condition, theIfConstruct);
		}
	}
}