using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler.LogicFlow;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Common;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DFST;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DTree;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Switches
{
	internal class SwitchBuilder : DominatorTreeDependentStep
	{
		private readonly Dictionary<ILogicalConstruct, List<CFGBlockLogicalConstruct>> logicalConstructToSwitchBlocksMap;

		public SwitchBuilder(LogicalFlowBuilderContext logicalContext) : base(logicalContext)
		{
			this.logicalConstructToSwitchBlocksMap = new Dictionary<ILogicalConstruct, List<CFGBlockLogicalConstruct>>();
		}

		private void AddSwitchCFGBlockToMap(CFGBlockLogicalConstruct cfgConstruct)
		{
			List<CFGBlockLogicalConstruct> cFGBlockLogicalConstructs;
			ILogicalConstruct parent = cfgConstruct.Parent as ILogicalConstruct;
			if (!this.logicalConstructToSwitchBlocksMap.TryGetValue(parent, out cFGBlockLogicalConstructs))
			{
				cFGBlockLogicalConstructs = new List<CFGBlockLogicalConstruct>();
				this.logicalConstructToSwitchBlocksMap.Add(parent, cFGBlockLogicalConstructs);
			}
			cFGBlockLogicalConstructs.Add(cfgConstruct);
		}

		public void BuildConstructs()
		{
			this.MapSwitchBlocksToParents();
			foreach (KeyValuePair<ILogicalConstruct, List<CFGBlockLogicalConstruct>> keyValuePair in this.logicalConstructToSwitchBlocksMap)
			{
				this.ProcessSwitchConstructs(keyValuePair.Key, keyValuePair.Value);
			}
		}

		private void CreateSwitchConstruct(CFGBlockLogicalConstruct switchBlock, ILogicalConstruct parentConstruct, SwitchData switchData, DominatorTree dominatorTree)
		{
			ILogicalConstruct logicalConstruct;
			HashSet<ISingleEntrySubGraph> singleEntrySubGraphs;
			ILogicalConstruct logicalConstruct1;
			HashSet<ISingleEntrySubGraph> singleEntrySubGraphs1;
			PairList<CFGBlockLogicalConstruct, List<int>> orderedCFGSuccessorToLabelsMap = this.GetOrderedCFGSuccessorToLabelsMap(switchData);
			Dictionary<ILogicalConstruct, HashSet<ISingleEntrySubGraph>> validCases = this.GetValidCases(dominatorTree, switchBlock);
			List<CaseLogicalConstruct> caseLogicalConstructs = new List<CaseLogicalConstruct>();
			PairList<List<int>, CFGBlockLogicalConstruct> pairList = new PairList<List<int>, CFGBlockLogicalConstruct>();
			foreach (KeyValuePair<CFGBlockLogicalConstruct, List<int>> keyValuePair in orderedCFGSuccessorToLabelsMap)
			{
				if (!LogicalFlowUtilities.TryGetParentConstructWithGivenParent(keyValuePair.Key, parentConstruct, out logicalConstruct1) || !validCases.TryGetValue(logicalConstruct1, out singleEntrySubGraphs1))
				{
					pairList.Add(keyValuePair.Value, keyValuePair.Key);
				}
				else
				{
					CaseLogicalConstruct caseLogicalConstruct = new CaseLogicalConstruct(logicalConstruct1);
					caseLogicalConstruct.CaseNumbers.AddRange(keyValuePair.Value);
					caseLogicalConstruct.Body.UnionWith(singleEntrySubGraphs1.Cast<ILogicalConstruct>());
					caseLogicalConstruct.AttachCaseConstructToGraph();
					caseLogicalConstructs.Add(caseLogicalConstruct);
				}
			}
			CaseLogicalConstruct caseLogicalConstruct1 = null;
			CFGBlockLogicalConstruct cFGLogicalConstructFromBlock = this.GetCFGLogicalConstructFromBlock(switchData.DefaultCase);
			if (LogicalFlowUtilities.TryGetParentConstructWithGivenParent(cFGLogicalConstructFromBlock, parentConstruct, out logicalConstruct) && validCases.TryGetValue(logicalConstruct, out singleEntrySubGraphs))
			{
				caseLogicalConstruct1 = new CaseLogicalConstruct(logicalConstruct);
				if (this.HasSuccessors(singleEntrySubGraphs))
				{
					caseLogicalConstruct1.Body.UnionWith(singleEntrySubGraphs.Cast<ILogicalConstruct>());
				}
				caseLogicalConstruct1.AttachCaseConstructToGraph();
			}
			SwitchLogicalConstruct switchLogicalConstruct = SwitchLogicalConstruct.GroupInSwitchConstruct(switchBlock, caseLogicalConstructs, pairList, caseLogicalConstruct1, cFGLogicalConstructFromBlock);
			this.UpdateDominatorTree(dominatorTree, switchLogicalConstruct);
		}

		private CFGBlockLogicalConstruct GetCFGLogicalConstructFromBlock(InstructionBlock theBlock)
		{
			return this.logicalContext.CFGBlockToLogicalConstructMap[theBlock][0];
		}

		private PairList<CFGBlockLogicalConstruct, List<int>> GetOrderedCFGSuccessorToLabelsMap(SwitchData switchData)
		{
			KeyValuePair<int, List<int>> keyValuePair;
			PairList<CFGBlockLogicalConstruct, List<int>> pairList = new PairList<CFGBlockLogicalConstruct, List<int>>();
			Dictionary<InstructionBlock, KeyValuePair<int, List<int>>> instructionBlocks = new Dictionary<InstructionBlock, KeyValuePair<int, List<int>>>();
			for (int i = 0; i < (int)switchData.OrderedCasesArray.Length; i++)
			{
				InstructionBlock orderedCasesArray = switchData.OrderedCasesArray[i];
				if (orderedCasesArray != switchData.DefaultCase)
				{
					if (!instructionBlocks.TryGetValue(orderedCasesArray, out keyValuePair))
					{
						keyValuePair = new KeyValuePair<int, List<int>>(pairList.Count, new List<int>());
						pairList.Add(this.GetCFGLogicalConstructFromBlock(orderedCasesArray), keyValuePair.Value);
						instructionBlocks.Add(orderedCasesArray, keyValuePair);
					}
					keyValuePair.Value.Add(i);
				}
			}
			return pairList;
		}

		private Dictionary<ILogicalConstruct, HashSet<ISingleEntrySubGraph>> GetValidCases(DominatorTree dominatorTree, ILogicalConstruct switchCFGBlock)
		{
			bool flag;
			HashSet<ISingleEntrySubGraph> singleEntrySubGraphs;
			Dictionary<ILogicalConstruct, HashSet<ISingleEntrySubGraph>> logicalConstructs = new Dictionary<ILogicalConstruct, HashSet<ISingleEntrySubGraph>>();
			HashSet<ISingleEntrySubGraph> singleEntrySubGraphs1 = new HashSet<ISingleEntrySubGraph>();
			singleEntrySubGraphs1.Add(switchCFGBlock);
			foreach (ILogicalConstruct sameParentSuccessor in switchCFGBlock.SameParentSuccessors)
			{
				if (sameParentSuccessor == switchCFGBlock || dominatorTree.GetImmediateDominator(sameParentSuccessor) != switchCFGBlock)
				{
					continue;
				}
				HashSet<ISingleEntrySubGraph> dominatedNodes = dominatorTree.GetDominatedNodes(sameParentSuccessor);
				logicalConstructs.Add(sameParentSuccessor, dominatedNodes);
				singleEntrySubGraphs1.UnionWith(dominatedNodes);
			}
			List<ILogicalConstruct> logicalConstructs1 = new List<ILogicalConstruct>(
				from node in DFSTBuilder.BuildTree(switchCFGBlock.Parent, switchCFGBlock).ReversePostOrder
				select node.Construct as ILogicalConstruct into construct
				where logicalConstructs.ContainsKey(construct)
				select construct);
			do
			{
				flag = false;
				foreach (ILogicalConstruct logicalConstruct in logicalConstructs1)
				{
					if (!logicalConstructs.TryGetValue(logicalConstruct, out singleEntrySubGraphs) || this.IsCaseValid(logicalConstruct, singleEntrySubGraphs1))
					{
						continue;
					}
					singleEntrySubGraphs1.ExceptWith(singleEntrySubGraphs);
					logicalConstructs.Remove(logicalConstruct);
					flag = true;
				}
			}
			while (flag);
			return logicalConstructs;
		}

		private bool HasSuccessors(IEnumerable<ISingleEntrySubGraph> body)
		{
			bool flag;
			using (IEnumerator<ISingleEntrySubGraph> enumerator = body.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					HashSet<ISingleEntrySubGraph>.Enumerator enumerator1 = ((ILogicalConstruct)enumerator.Current).SameParentSuccessors.GetEnumerator();
					try
					{
						while (enumerator1.MoveNext())
						{
							if (body.Contains<ISingleEntrySubGraph>((ILogicalConstruct)enumerator1.Current))
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

		private bool IsCaseValid(ISingleEntrySubGraph caseEntry, HashSet<ISingleEntrySubGraph> legalPredecessors)
		{
			bool flag;
			HashSet<ISingleEntrySubGraph>.Enumerator enumerator = caseEntry.SameParentPredecessors.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (legalPredecessors.Contains((ILogicalConstruct)enumerator.Current))
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private void MapSwitchBlocksToParents()
		{
			foreach (InstructionBlock key in this.logicalContext.CFG.SwitchBlocksInformation.Keys)
			{
				CFGBlockLogicalConstruct[] item = this.logicalContext.CFGBlockToLogicalConstructMap[key];
				this.AddSwitchCFGBlockToMap(item[(int)item.Length - 1]);
			}
		}

		private void ProcessSwitchConstructs(ILogicalConstruct parent, List<CFGBlockLogicalConstruct> switchBlocks)
		{
			DominatorTree dominatorTreeFromContext = base.GetDominatorTreeFromContext(parent);
			DFSTree dFSTree = DFSTBuilder.BuildTree(parent);
			switchBlocks.Sort((CFGBlockLogicalConstruct x, CFGBlockLogicalConstruct y) => dFSTree.ConstructToNodeMap[y].ReversePostOrderIndex.CompareTo(dFSTree.ConstructToNodeMap[x].ReversePostOrderIndex));
			foreach (CFGBlockLogicalConstruct switchBlock in switchBlocks)
			{
				this.CreateSwitchConstruct(switchBlock, parent, this.logicalContext.CFG.SwitchBlocksInformation[switchBlock.TheBlock], dominatorTreeFromContext);
			}
		}

		private void UpdateDominatorTree(DominatorTree dominatorTree, SwitchLogicalConstruct theSwitchConstruct)
		{
			HashSet<ISingleEntrySubGraph> singleEntrySubGraphs = new HashSet<ISingleEntrySubGraph>();
			singleEntrySubGraphs.Add(theSwitchConstruct.Entry);
			CaseLogicalConstruct[] conditionCases = theSwitchConstruct.ConditionCases;
			for (int i = 0; i < (int)conditionCases.Length; i++)
			{
				singleEntrySubGraphs.UnionWith(conditionCases[i].Children);
			}
			if (theSwitchConstruct.DefaultCase != null)
			{
				singleEntrySubGraphs.UnionWith(theSwitchConstruct.DefaultCase.Children);
			}
			dominatorTree.MergeNodes(singleEntrySubGraphs, theSwitchConstruct.Entry, theSwitchConstruct);
		}
	}
}