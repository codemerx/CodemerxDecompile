using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DFST;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DTree;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions
{
	internal class IfBuilder : DominatorTreeDependentStep
	{
		private readonly TypeSystem typeSystem;

		private readonly Dictionary<int, int> blockToInstructionsCount;

		public IfBuilder(LogicalFlowBuilderContext logicalContext, TypeSystem typeSystem)
		{
			this.blockToInstructionsCount = new Dictionary<int, int>();
			base(logicalContext);
			this.typeSystem = typeSystem;
			return;
		}

		private bool AllEndsAreThrow(ILogicalConstruct entry)
		{
			stackVariable4 = entry.get_FirstBlock().get_TheBlock();
			stackVariable6 = new Code[1];
			stackVariable6[0] = 119;
			if (!this.SubtreeEndsInInstructionCode(stackVariable4, stackVariable6))
			{
				return false;
			}
			return true;
		}

		public void BuildConstructs(ILogicalConstruct construct)
		{
			if (construct as CFGBlockLogicalConstruct != null || construct as ConditionLogicalConstruct != null)
			{
				return;
			}
			V_0 = construct.get_Children().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = (ILogicalConstruct)V_0.get_Current();
					this.BuildConstructs(V_1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			this.BuildIfConstructs(construct);
			return;
		}

		private void BuildIfConstructs(ILogicalConstruct construct)
		{
			V_0 = this.GetDominatorTreeFromContext(construct);
			V_1 = DFSTBuilder.BuildTree(construct);
			V_2 = this.GetPostOrderedIfConditionCandidates(V_1).GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					dummyVar0 = this.TryBuildIfConstruct(V_3, V_0, V_1);
				}
			}
			finally
			{
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
			return;
		}

		private ILogicalConstruct CheckSuccessor(ILogicalConstruct condition, ILogicalConstruct conditionSuccessor, HashSet<ISingleEntrySubGraph> otherSuccessorFrontier, DFSTree dfsTree)
		{
			if (otherSuccessorFrontier.Contains(conditionSuccessor) && !dfsTree.get_ConstructToNodeMap().TryGetValue(conditionSuccessor, out V_0) || dfsTree.get_ConstructToNodeMap().get_Item(condition).CompareTo(V_0) < 0)
			{
				return conditionSuccessor;
			}
			return null;
		}

		private bool CheckSuccessors(HashSet<ILogicalConstruct> theBody, ILogicalConstruct successor)
		{
			V_0 = false;
			V_1 = theBody.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current().get_SameParentSuccessors().GetEnumerator();
					try
					{
						while (V_2.MoveNext())
						{
							V_3 = (ILogicalConstruct)V_2.get_Current();
							if (theBody.Contains(V_3))
							{
								continue;
							}
							V_0 = true;
							if (V_3 != successor)
							{
								continue;
							}
							V_4 = true;
							goto Label1;
						}
					}
					finally
					{
						((IDisposable)V_2).Dispose();
					}
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
		Label1:
			return V_4;
		Label0:
			return !V_0;
		}

		private int CountInstructions(IEnumerable<ILogicalConstruct> trueSuccessor)
		{
			V_0 = 0;
			V_1 = trueSuccessor.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current().get_CFGBlocks().GetEnumerator();
					try
					{
						while (V_2.MoveNext())
						{
							V_3 = V_2.get_Current();
							V_4 = 0;
							if (!this.blockToInstructionsCount.ContainsKey(V_3.get_TheBlock().get_First().get_Offset()))
							{
								V_5 = V_3.get_TheBlock().GetEnumerator();
								try
								{
									while (V_5.MoveNext())
									{
										dummyVar0 = V_5.get_Current();
										V_4 = V_4 + 1;
									}
								}
								finally
								{
									if (V_5 != null)
									{
										V_5.Dispose();
									}
								}
								this.blockToInstructionsCount.Add(V_3.get_TheBlock().get_First().get_Offset(), V_4);
							}
							else
							{
								V_4 = this.blockToInstructionsCount.get_Item(V_3.get_TheBlock().get_First().get_Offset());
							}
							V_0 = V_0 + V_4;
						}
					}
					finally
					{
						((IDisposable)V_2).Dispose();
					}
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}

		private HashSet<ILogicalConstruct> GetBlockBody(DominatorTree dominatorTree, ILogicalConstruct conditionSuccessor, ConditionLogicalConstruct theCondition)
		{
			if (conditionSuccessor == dominatorTree.get_RootConstruct())
			{
				return null;
			}
			V_0 = null;
			if (conditionSuccessor.get_AllPredecessors().get_Count() == 1)
			{
				V_0 = new HashSet<ILogicalConstruct>();
				V_1 = dominatorTree.GetDominatedNodes(conditionSuccessor).GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = (ILogicalConstruct)V_1.get_Current();
						if (V_2 != theCondition)
						{
							dummyVar0 = V_0.Add(V_2);
						}
						else
						{
							V_3 = null;
							goto Label1;
						}
					}
					goto Label0;
				}
				finally
				{
					((IDisposable)V_1).Dispose();
				}
			Label1:
				return V_3;
			}
		Label0:
			return V_0;
		}

		private IEnumerable<ConditionLogicalConstruct> GetPostOrderedIfConditionCandidates(DFSTree dfsTree)
		{
			stackVariable1 = new IfBuilder.u003cGetPostOrderedIfConditionCandidatesu003ed__5(-2);
			stackVariable1.u003cu003e3__dfsTree = dfsTree;
			return stackVariable1;
		}

		private bool HasLessOrEqualInstructions(IEnumerable<ILogicalConstruct> trueSuccessor, IEnumerable<ILogicalConstruct> falseSuccessor)
		{
			V_0 = this.CountInstructions(trueSuccessor);
			return this.CountInstructions(falseSuccessor) <= V_0;
		}

		private bool HasSuccessors(ICollection<ILogicalConstruct> block)
		{
			V_0 = block.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current().get_AllSuccessors().GetEnumerator();
					try
					{
						while (V_1.MoveNext())
						{
							V_2 = (ILogicalConstruct)V_1.get_Current();
							if (block.Contains(V_2))
							{
								continue;
							}
							V_3 = true;
							goto Label1;
						}
					}
					finally
					{
						((IDisposable)V_1).Dispose();
					}
				}
				goto Label0;
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
		Label1:
			return V_3;
		Label0:
			return false;
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
			V_0 = true;
			V_1 = new Queue<InstructionBlock>();
			V_2 = new HashSet<int>();
			V_1.Enqueue(entryBlock);
			while (V_1.get_Count() > 0 & V_0)
			{
				V_3 = V_1.Dequeue();
				if (V_2.Contains(V_3.get_First().get_Offset()))
				{
					continue;
				}
				dummyVar0 = V_2.Add(V_3.get_First().get_Offset());
				if (V_3.get_Successors().Length != 0)
				{
					V_8 = V_3.get_Successors();
					V_9 = 0;
					while (V_9 < (int)V_8.Length)
					{
						V_1.Enqueue(V_8[V_9]);
						V_9 = V_9 + 1;
					}
				}
				else
				{
					V_4 = false;
					V_5 = operationCodes.GetEnumerator();
					try
					{
						while (V_5.MoveNext())
						{
							V_6 = V_5.get_Current();
							V_7 = V_3.get_Last().get_OpCode();
							V_4 = V_4 | V_7.get_Code() == V_6;
						}
					}
					finally
					{
						if (V_5 != null)
						{
							V_5.Dispose();
						}
					}
					V_0 = V_0 & V_4;
				}
			}
			return V_0;
		}

		private bool TryBuildIfConstruct(ConditionLogicalConstruct condition, DominatorTree dominatorTree, DFSTree dfsTree)
		{
			V_0 = condition.get_FalseSuccessor();
			V_1 = condition.get_TrueSuccessor();
			V_2 = dominatorTree.GetDominanceFrontier(V_0);
			V_3 = dominatorTree.GetDominanceFrontier(V_1);
			stackVariable15 = this.CheckSuccessor(condition, V_1, V_2, dfsTree);
			if (stackVariable15 == null)
			{
				dummyVar0 = stackVariable15;
				stackVariable15 = this.CheckSuccessor(condition, V_0, V_3, dfsTree);
			}
			V_4 = new HashSet<ISingleEntrySubGraph>(V_3);
			V_4.IntersectWith(V_2);
			if (stackVariable15 == null && V_2.get_Count() > 0 && V_3.get_Count() > 0 && V_4.get_Count() == 0)
			{
				return false;
			}
			V_5 = this.GetBlockBody(dominatorTree, V_1, condition);
			V_6 = this.GetBlockBody(dominatorTree, V_0, condition);
			if (V_5 == null && V_6 == null)
			{
				return false;
			}
			if (V_5 == null)
			{
				condition.Negate(this.typeSystem);
				stackVariable86 = V_1;
				V_1 = V_0;
				V_0 = stackVariable86;
				V_5 = V_6;
				V_6 = null;
			}
			if (V_6 == null && !this.CheckSuccessors(V_5, V_0))
			{
				return false;
			}
			if (this.ShouldInvertIfAndRemoveElse(V_5, V_1, V_6, V_0))
			{
				condition.Negate(this.typeSystem);
				stackVariable73 = V_1;
				V_1 = V_0;
				V_0 = stackVariable73;
				stackVariable75 = V_5;
				V_5 = V_6;
				V_6 = stackVariable75;
				V_6 = null;
			}
			if (V_6 != null && !this.HasSuccessors(V_5))
			{
				stackVariable61 = V_1.get_FirstBlock().get_TheBlock();
				stackVariable63 = new Code[2];
				stackVariable63[0] = 41;
				stackVariable63[1] = 119;
				if (this.SubtreeEndsInInstructionCode(stackVariable61, stackVariable63))
				{
					V_6 = null;
				}
			}
			V_7 = new BlockLogicalConstruct(V_1, V_5);
			if (V_6 != null)
			{
				stackVariable46 = new BlockLogicalConstruct(V_0, V_6);
			}
			else
			{
				stackVariable46 = null;
			}
			this.UpdateDominatorTree(dominatorTree, IfLogicalConstruct.GroupInIfConstruct(condition, V_7, stackVariable46));
			return true;
		}

		private void UpdateDominatorTree(DominatorTree dominatorTree, IfLogicalConstruct theIfConstruct)
		{
			V_0 = new HashSet<ISingleEntrySubGraph>();
			dummyVar0 = V_0.Add(theIfConstruct.get_Condition());
			V_0.UnionWith(theIfConstruct.get_Then().get_Children());
			if (theIfConstruct.get_Else() != null)
			{
				V_0.UnionWith(theIfConstruct.get_Else().get_Children());
			}
			dominatorTree.MergeNodes(V_0, theIfConstruct.get_Condition(), theIfConstruct);
			return;
		}
	}
}