using System.Collections.Generic;
using System.Linq;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DTree;
using Mono.Cecil;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DFST;
using Telerik.JustDecompiler.Common;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Switches
{
    class SwitchBuilder : DominatorTreeDependentStep
    {
        private readonly Dictionary<ILogicalConstruct, List<CFGBlockLogicalConstruct>> logicalConstructToSwitchBlocksMap;

        public SwitchBuilder(LogicalFlowBuilderContext logicalContext)
            :base(logicalContext)
        {
            this.logicalConstructToSwitchBlocksMap = new Dictionary<ILogicalConstruct, List<CFGBlockLogicalConstruct>>();
        }

        public void BuildConstructs()
        {
            MapSwitchBlocksToParents();

            foreach (KeyValuePair<ILogicalConstruct, List<CFGBlockLogicalConstruct>> pair in logicalConstructToSwitchBlocksMap)
            {
                ProcessSwitchConstructs(pair.Key, pair.Value);
            }
        }

        private void MapSwitchBlocksToParents()
        {
            foreach (InstructionBlock instructionBlock in logicalContext.CFG.SwitchBlocksInformation.Keys)
            {
                CFGBlockLogicalConstruct[] constructs = logicalContext.CFGBlockToLogicalConstructMap[instructionBlock];
                //We get the last CFG construct since it will hold the condition of the switch and its successors.
                CFGBlockLogicalConstruct instructionBlockConstruct = constructs[constructs.Length - 1];
                AddSwitchCFGBlockToMap(instructionBlockConstruct);
            }
        }

        private void AddSwitchCFGBlockToMap(CFGBlockLogicalConstruct cfgConstruct)
        {
            ILogicalConstruct parentConstruct = cfgConstruct.Parent as ILogicalConstruct;
            List<CFGBlockLogicalConstruct> switchCFGConstructs;
            if (!logicalConstructToSwitchBlocksMap.TryGetValue(parentConstruct, out switchCFGConstructs))
            {
                switchCFGConstructs = new List<CFGBlockLogicalConstruct>();
                logicalConstructToSwitchBlocksMap.Add(parentConstruct, switchCFGConstructs);
            }

            switchCFGConstructs.Add(cfgConstruct);
        }

        private void ProcessSwitchConstructs(ILogicalConstruct parent, List<CFGBlockLogicalConstruct> switchBlocks)
        {
            DominatorTree dominatorTree = GetDominatorTreeFromContext(parent);
            DFSTree dfsTree = DFSTBuilder.BuildTree(parent);
            switchBlocks.Sort((x, y) => dfsTree.ConstructToNodeMap[y].ReversePostOrderIndex.CompareTo(dfsTree.ConstructToNodeMap[x].ReversePostOrderIndex));
            foreach (CFGBlockLogicalConstruct switchBlock in switchBlocks)
            {
                CreateSwitchConstruct(switchBlock, parent, this.logicalContext.CFG.SwitchBlocksInformation[switchBlock.TheBlock], dominatorTree);
            }
        }

        private void CreateSwitchConstruct(CFGBlockLogicalConstruct switchBlock, ILogicalConstruct parentConstruct,
            SwitchData switchData, DominatorTree dominatorTree)
        {
            List<KeyValuePair<CFGBlockLogicalConstruct, List<int>>> cfgSuccessorToLabelsMap = GetOrderedCFGSuccessorToLabelsMap(switchData);
            Dictionary<ILogicalConstruct, HashSet<ISingleEntrySubGraph>> validCaseEntryToDominatedNodesMap = GetValidCases(dominatorTree, switchBlock);

            List<CaseLogicalConstruct> orderedCaseConstructs = new List<CaseLogicalConstruct>();
            PairList<List<int>, CFGBlockLogicalConstruct> labelsToCFGSuccessorsList = new PairList<List<int>, CFGBlockLogicalConstruct>();
            foreach (KeyValuePair<CFGBlockLogicalConstruct, List<int>> cfgSuccessorToLabelsPair in cfgSuccessorToLabelsMap)
            {
                ILogicalConstruct successor;
                HashSet<ISingleEntrySubGraph> dominatedNodes;
                if (LogicalFlowUtilities.TryGetParentConstructWithGivenParent(cfgSuccessorToLabelsPair.Key, parentConstruct, out successor) &&
                    validCaseEntryToDominatedNodesMap.TryGetValue(successor, out dominatedNodes))
                {
                    CaseLogicalConstruct newCaseConstruct = new CaseLogicalConstruct(successor);
                    newCaseConstruct.CaseNumbers.AddRange(cfgSuccessorToLabelsPair.Value);
                    newCaseConstruct.Body.UnionWith(dominatedNodes.Cast<ILogicalConstruct>());
                    newCaseConstruct.AttachCaseConstructToGraph();
                    orderedCaseConstructs.Add(newCaseConstruct);
                }
                else
                {
                    labelsToCFGSuccessorsList.Add(cfgSuccessorToLabelsPair.Value, cfgSuccessorToLabelsPair.Key);
                }
            }

            CaseLogicalConstruct defaultCase = null;
            CFGBlockLogicalConstruct defaultCFGSuccessor = GetCFGLogicalConstructFromBlock(switchData.DefaultCase);
            ILogicalConstruct defaultSuccessor;
            HashSet<ISingleEntrySubGraph> defaultCaseNodes;
            if (LogicalFlowUtilities.TryGetParentConstructWithGivenParent(defaultCFGSuccessor, parentConstruct, out defaultSuccessor) &&
                    validCaseEntryToDominatedNodesMap.TryGetValue(defaultSuccessor, out defaultCaseNodes))
            {
                defaultCase = new CaseLogicalConstruct(defaultSuccessor);
                if (HasSuccessors(defaultCaseNodes))
                {
                    defaultCase.Body.UnionWith(defaultCaseNodes.Cast<ILogicalConstruct>());
                }
                defaultCase.AttachCaseConstructToGraph();
            }

            SwitchLogicalConstruct theSwitch = SwitchLogicalConstruct.GroupInSwitchConstruct(switchBlock, orderedCaseConstructs, labelsToCFGSuccessorsList, defaultCase, defaultCFGSuccessor);
            UpdateDominatorTree(dominatorTree, theSwitch);
        }

        private PairList<CFGBlockLogicalConstruct, List<int>> GetOrderedCFGSuccessorToLabelsMap(SwitchData switchData)
        {
            //Ugly but effective. Sorry
            PairList<CFGBlockLogicalConstruct, List<int>> result = new PairList<CFGBlockLogicalConstruct, List<int>>();
            Dictionary<InstructionBlock, KeyValuePair<int, List<int>>> blockSuccessorToResultPositionMap =
                new Dictionary<InstructionBlock, KeyValuePair<int, List<int>>>();

            for (int i = 0; i < switchData.OrderedCasesArray.Length; i++)
            {
                InstructionBlock instructionBlock = switchData.OrderedCasesArray[i];
                if (instructionBlock != switchData.DefaultCase)
                {
                    KeyValuePair<int, List<int>> positionToLabelListPair;
                    if (!blockSuccessorToResultPositionMap.TryGetValue(instructionBlock, out positionToLabelListPair))
                    {
                        positionToLabelListPair = new KeyValuePair<int, List<int>>(result.Count, new List<int>());

                        result.Add(GetCFGLogicalConstructFromBlock(instructionBlock), positionToLabelListPair.Value);

                        blockSuccessorToResultPositionMap.Add(instructionBlock, positionToLabelListPair);
                    }
                    positionToLabelListPair.Value.Add(i);
                }
            }

            return result;
        }

        private bool HasSuccessors(IEnumerable<ISingleEntrySubGraph> body)
        {
            foreach (ILogicalConstruct construct in body)
            {
                foreach (ILogicalConstruct successor in construct.SameParentSuccessors)
                {
                    if (!body.Contains(successor))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private Dictionary<ILogicalConstruct, HashSet<ISingleEntrySubGraph>> GetValidCases(DominatorTree dominatorTree, ILogicalConstruct switchCFGBlock)
        {
            Dictionary<ILogicalConstruct, HashSet<ISingleEntrySubGraph>> caseEntriesToDominatedNodesMap = new Dictionary<ILogicalConstruct, HashSet<ISingleEntrySubGraph>>();
            HashSet<ISingleEntrySubGraph> legalPredecessors = new HashSet<ISingleEntrySubGraph>();
            legalPredecessors.Add(switchCFGBlock);

            foreach (ILogicalConstruct successor in switchCFGBlock.SameParentSuccessors)
            {
                if (successor != switchCFGBlock && dominatorTree.GetImmediateDominator(successor) == switchCFGBlock)
                {
                    HashSet<ISingleEntrySubGraph> dominatedNodes = dominatorTree.GetDominatedNodes(successor);
                    caseEntriesToDominatedNodesMap.Add(successor, dominatedNodes);
                    legalPredecessors.UnionWith(dominatedNodes);
                }
            }

            DFSTree dfsTree = DFSTBuilder.BuildTree(switchCFGBlock.Parent, switchCFGBlock);
            List<ILogicalConstruct> orderedCaseEntries =
                new List<ILogicalConstruct>(dfsTree.ReversePostOrder.Select(node => node.Construct as ILogicalConstruct).Where(construct => caseEntriesToDominatedNodesMap.ContainsKey(construct)));

            bool changed;
            do
            {
                changed = false;
                foreach (ILogicalConstruct caseEntry in orderedCaseEntries)
                {
                    HashSet<ISingleEntrySubGraph> dominatedNodes;
                    if (caseEntriesToDominatedNodesMap.TryGetValue(caseEntry, out dominatedNodes) && !IsCaseValid(caseEntry, legalPredecessors))
                    {
                        legalPredecessors.ExceptWith(dominatedNodes);
                        caseEntriesToDominatedNodesMap.Remove(caseEntry);
                        changed = true;
                    }
                }
            } while (changed);

            return caseEntriesToDominatedNodesMap;
        }

        private bool IsCaseValid(ISingleEntrySubGraph caseEntry, HashSet<ISingleEntrySubGraph> legalPredecessors)
        {
            foreach (ILogicalConstruct predecessor in caseEntry.SameParentPredecessors)
            {
                if (!legalPredecessors.Contains(predecessor))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the entry CFG construct for the specified instruction block.
        /// </summary>
        /// <param name="successor"></param>
        /// <returns></returns>
        private CFGBlockLogicalConstruct GetCFGLogicalConstructFromBlock(InstructionBlock theBlock)
        {
            CFGBlockLogicalConstruct[] constructs = logicalContext.CFGBlockToLogicalConstructMap[theBlock];
            return constructs[0];
        }

        private void UpdateDominatorTree(DominatorTree dominatorTree, SwitchLogicalConstruct theSwitchConstruct)
        {
            HashSet<ISingleEntrySubGraph> switchNodes = new HashSet<ISingleEntrySubGraph>();
            switchNodes.Add(theSwitchConstruct.Entry);
            foreach (CaseLogicalConstruct @case in theSwitchConstruct.ConditionCases)
            {
                switchNodes.UnionWith(@case.Children);
            }

            if (theSwitchConstruct.DefaultCase != null)
            {
                switchNodes.UnionWith(theSwitchConstruct.DefaultCase.Children);
            }

            dominatorTree.MergeNodes(switchNodes, theSwitchConstruct.Entry, theSwitchConstruct);
        }
    }
}
