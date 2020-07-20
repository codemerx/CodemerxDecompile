using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler.LogicFlow;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DFST;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DTree;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Switches
{
	internal class SwitchBuilder : DominatorTreeDependentStep
	{
		private readonly Dictionary<ILogicalConstruct, List<CFGBlockLogicalConstruct>> logicalConstructToSwitchBlocksMap;

		public SwitchBuilder(LogicalFlowBuilderContext logicalContext)
		{
			base(logicalContext);
			this.logicalConstructToSwitchBlocksMap = new Dictionary<ILogicalConstruct, List<CFGBlockLogicalConstruct>>();
			return;
		}

		private void AddSwitchCFGBlockToMap(CFGBlockLogicalConstruct cfgConstruct)
		{
			V_0 = cfgConstruct.get_Parent() as ILogicalConstruct;
			if (!this.logicalConstructToSwitchBlocksMap.TryGetValue(V_0, out V_1))
			{
				V_1 = new List<CFGBlockLogicalConstruct>();
				this.logicalConstructToSwitchBlocksMap.Add(V_0, V_1);
			}
			V_1.Add(cfgConstruct);
			return;
		}

		public void BuildConstructs()
		{
			this.MapSwitchBlocksToParents();
			V_0 = this.logicalConstructToSwitchBlocksMap.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.ProcessSwitchConstructs(V_1.get_Key(), V_1.get_Value());
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private void CreateSwitchConstruct(CFGBlockLogicalConstruct switchBlock, ILogicalConstruct parentConstruct, SwitchData switchData, DominatorTree dominatorTree)
		{
			stackVariable2 = this.GetOrderedCFGSuccessorToLabelsMap(switchData);
			V_0 = this.GetValidCases(dominatorTree, switchBlock);
			V_1 = new List<CaseLogicalConstruct>();
			V_2 = new PairList<List<int>, CFGBlockLogicalConstruct>();
			V_8 = stackVariable2.GetEnumerator();
			try
			{
				while (V_8.MoveNext())
				{
					V_9 = V_8.get_Current();
					if (!LogicalFlowUtilities.TryGetParentConstructWithGivenParent(V_9.get_Key(), parentConstruct, out V_10) || !V_0.TryGetValue(V_10, out V_11))
					{
						V_2.Add(V_9.get_Value(), V_9.get_Key());
					}
					else
					{
						V_12 = new CaseLogicalConstruct(V_10);
						V_12.get_CaseNumbers().AddRange(V_9.get_Value());
						V_12.get_Body().UnionWith(V_11.Cast<ILogicalConstruct>());
						V_12.AttachCaseConstructToGraph();
						V_1.Add(V_12);
					}
				}
			}
			finally
			{
				((IDisposable)V_8).Dispose();
			}
			V_3 = null;
			V_4 = this.GetCFGLogicalConstructFromBlock(switchData.get_DefaultCase());
			if (LogicalFlowUtilities.TryGetParentConstructWithGivenParent(V_4, parentConstruct, out V_5) && V_0.TryGetValue(V_5, out V_6))
			{
				V_3 = new CaseLogicalConstruct(V_5);
				if (this.HasSuccessors(V_6))
				{
					V_3.get_Body().UnionWith(V_6.Cast<ILogicalConstruct>());
				}
				V_3.AttachCaseConstructToGraph();
			}
			V_7 = SwitchLogicalConstruct.GroupInSwitchConstruct(switchBlock, V_1, V_2, V_3, V_4);
			this.UpdateDominatorTree(dominatorTree, V_7);
			return;
		}

		private CFGBlockLogicalConstruct GetCFGLogicalConstructFromBlock(InstructionBlock theBlock)
		{
			return this.logicalContext.get_CFGBlockToLogicalConstructMap().get_Item(theBlock)[0];
		}

		private PairList<CFGBlockLogicalConstruct, List<int>> GetOrderedCFGSuccessorToLabelsMap(SwitchData switchData)
		{
			V_0 = new PairList<CFGBlockLogicalConstruct, List<int>>();
			V_1 = new Dictionary<InstructionBlock, KeyValuePair<int, List<int>>>();
			V_2 = 0;
			while (V_2 < (int)switchData.get_OrderedCasesArray().Length)
			{
				V_3 = switchData.get_OrderedCasesArray()[V_2];
				if (InstructionBlock.op_Inequality(V_3, switchData.get_DefaultCase()))
				{
					if (!V_1.TryGetValue(V_3, out V_4))
					{
						V_4 = new KeyValuePair<int, List<int>>(V_0.get_Count(), new List<int>());
						V_0.Add(this.GetCFGLogicalConstructFromBlock(V_3), V_4.get_Value());
						V_1.Add(V_3, V_4);
					}
					V_4.get_Value().Add(V_2);
				}
				V_2 = V_2 + 1;
			}
			return V_0;
		}

		private Dictionary<ILogicalConstruct, HashSet<ISingleEntrySubGraph>> GetValidCases(DominatorTree dominatorTree, ILogicalConstruct switchCFGBlock)
		{
			V_0 = new SwitchBuilder.u003cu003ec__DisplayClass9_0();
			V_0.caseEntriesToDominatedNodesMap = new Dictionary<ILogicalConstruct, HashSet<ISingleEntrySubGraph>>();
			V_1 = new HashSet<ISingleEntrySubGraph>();
			dummyVar0 = V_1.Add(switchCFGBlock);
			V_4 = switchCFGBlock.get_SameParentSuccessors().GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_5 = (ILogicalConstruct)V_4.get_Current();
					if (V_5 == switchCFGBlock || dominatorTree.GetImmediateDominator(V_5) != switchCFGBlock)
					{
						continue;
					}
					V_6 = dominatorTree.GetDominatedNodes(V_5);
					V_0.caseEntriesToDominatedNodesMap.Add(V_5, V_6);
					V_1.UnionWith(V_6);
				}
			}
			finally
			{
				((IDisposable)V_4).Dispose();
			}
			stackVariable34 = DFSTBuilder.BuildTree(switchCFGBlock.get_Parent(), switchCFGBlock).get_ReversePostOrder();
			stackVariable35 = SwitchBuilder.u003cu003ec.u003cu003e9__9_0;
			if (stackVariable35 == null)
			{
				dummyVar1 = stackVariable35;
				stackVariable35 = new Func<DFSTNode, ILogicalConstruct>(SwitchBuilder.u003cu003ec.u003cu003e9.u003cGetValidCasesu003eb__9_0);
				SwitchBuilder.u003cu003ec.u003cu003e9__9_0 = stackVariable35;
			}
			V_2 = new List<ILogicalConstruct>(stackVariable34.Select<DFSTNode, ILogicalConstruct>(stackVariable35).Where<ILogicalConstruct>(new Func<ILogicalConstruct, bool>(V_0.u003cGetValidCasesu003eb__1)));
			do
			{
				V_3 = false;
				V_7 = V_2.GetEnumerator();
				try
				{
					while (V_7.MoveNext())
					{
						V_8 = V_7.get_Current();
						if (!V_0.caseEntriesToDominatedNodesMap.TryGetValue(V_8, out V_9) || this.IsCaseValid(V_8, V_1))
						{
							continue;
						}
						V_1.ExceptWith(V_9);
						dummyVar2 = V_0.caseEntriesToDominatedNodesMap.Remove(V_8);
						V_3 = true;
					}
				}
				finally
				{
					((IDisposable)V_7).Dispose();
				}
			}
			while (V_3);
			return V_0.caseEntriesToDominatedNodesMap;
		}

		private bool HasSuccessors(IEnumerable<ISingleEntrySubGraph> body)
		{
			V_0 = body.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = ((ILogicalConstruct)V_0.get_Current()).get_SameParentSuccessors().GetEnumerator();
					try
					{
						while (V_1.MoveNext())
						{
							V_2 = (ILogicalConstruct)V_1.get_Current();
							if (body.Contains<ISingleEntrySubGraph>(V_2))
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

		private bool IsCaseValid(ISingleEntrySubGraph caseEntry, HashSet<ISingleEntrySubGraph> legalPredecessors)
		{
			V_0 = caseEntry.get_SameParentPredecessors().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = (ILogicalConstruct)V_0.get_Current();
					if (legalPredecessors.Contains(V_1))
					{
						continue;
					}
					V_2 = false;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
		Label1:
			return V_2;
		Label0:
			return true;
		}

		private void MapSwitchBlocksToParents()
		{
			V_0 = this.logicalContext.get_CFG().get_SwitchBlocksInformation().get_Keys().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					stackVariable14 = this.logicalContext.get_CFGBlockToLogicalConstructMap().get_Item(V_1);
					this.AddSwitchCFGBlockToMap(stackVariable14[(int)stackVariable14.Length - 1]);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private void ProcessSwitchConstructs(ILogicalConstruct parent, List<CFGBlockLogicalConstruct> switchBlocks)
		{
			V_0 = new SwitchBuilder.u003cu003ec__DisplayClass5_0();
			V_1 = this.GetDominatorTreeFromContext(parent);
			V_0.dfsTree = DFSTBuilder.BuildTree(parent);
			switchBlocks.Sort(new Comparison<CFGBlockLogicalConstruct>(V_0.u003cProcessSwitchConstructsu003eb__0));
			V_2 = switchBlocks.GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					this.CreateSwitchConstruct(V_3, parent, this.logicalContext.get_CFG().get_SwitchBlocksInformation().get_Item(V_3.get_TheBlock()), V_1);
				}
			}
			finally
			{
				((IDisposable)V_2).Dispose();
			}
			return;
		}

		private void UpdateDominatorTree(DominatorTree dominatorTree, SwitchLogicalConstruct theSwitchConstruct)
		{
			V_0 = new HashSet<ISingleEntrySubGraph>();
			dummyVar0 = V_0.Add(theSwitchConstruct.get_Entry());
			V_1 = theSwitchConstruct.get_ConditionCases();
			V_2 = 0;
			while (V_2 < (int)V_1.Length)
			{
				V_0.UnionWith(V_1[V_2].get_Children());
				V_2 = V_2 + 1;
			}
			if (theSwitchConstruct.get_DefaultCase() != null)
			{
				V_0.UnionWith(theSwitchConstruct.get_DefaultCase().get_Children());
			}
			dominatorTree.MergeNodes(V_0, theSwitchConstruct.get_Entry(), theSwitchConstruct);
			return;
		}
	}
}