using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Common;
using System.Text;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Switches
{
    class SwitchLogicalConstruct : LogicalConstructBase, IBreaksContainer
    {

        /// <summary>
        /// Gets the switch condition.
        /// </summary>
        public Expression SwitchConditionExpression { get; private set; }

        /// <summary>
        /// Gets the default case of the switch construct.
        /// </summary>
        public CaseLogicalConstruct DefaultCase { get; private set; }

        /// <summary>
        /// Gets the labeled case constructs.
        /// </summary>
		public CaseLogicalConstruct[] ConditionCases { get; private set; }

        public CFGBlockLogicalConstruct DefaultCFGSuccessor { get; private set; }

        public PairList<List<int>, CFGBlockLogicalConstruct> NonDominatedCFGSuccessors { get; private set; }

        private SwitchLogicalConstruct(CFGBlockLogicalConstruct entry, ICollection<CaseLogicalConstruct> body,
            PairList<List<int>, CFGBlockLogicalConstruct> nonDominatedCFGSuccessors, CaseLogicalConstruct defaultCase,
            CFGBlockLogicalConstruct defaultCFGSuccessor)
	    {
            this.SwitchConditionExpression = entry.LogicalConstructExpressions[0];
            this.DefaultCase = defaultCase;
            this.DefaultCFGSuccessor = defaultCFGSuccessor;
            this.NonDominatedCFGSuccessors = nonDominatedCFGSuccessors;

            FillCasesArray(body);

            this.Entry = entry;
            RedirectChildrenToNewParent(GetBodyCollection());

            if (entry.CFGSuccessors.Contains(entry))
            {
                this.AddToPredecessors(entry);
                this.AddToSuccessors(entry);
            }
	    }

        /// <summary>
        /// Initializes and fills the ConditionCases array with the specified collection of condition cases.
        /// </summary>
        /// <param name="cases"></param>
        private void FillCasesArray(ICollection<CaseLogicalConstruct> cases)
        {
            this.ConditionCases = new CaseLogicalConstruct[cases.Count];
            int index = 0;
            foreach (CaseLogicalConstruct @case in cases)
            {
                this.ConditionCases[index++] = @case;
            }
        }

        /// <summary>
        /// Creates a collection holding all the children (to be) of the construct.
        /// </summary>
        /// <returns></returns>
        private ICollection<ILogicalConstruct> GetBodyCollection()
        {
            int additionalNodes = this.DefaultCase != null ? 2 : 1;
            ILogicalConstruct[] switchBody = new ILogicalConstruct[this.ConditionCases.Length + additionalNodes];
            Array.Copy(this.ConditionCases, switchBody, this.ConditionCases.Length);
            switchBody[switchBody.Length - additionalNodes] = this.Entry as ILogicalConstruct;
            if (additionalNodes == 2)
            {
                switchBody[switchBody.Length - 1] = this.DefaultCase;
            }

            return switchBody;
        }

        public static SwitchLogicalConstruct GroupInSwitchConstruct(CFGBlockLogicalConstruct entry, ICollection<CaseLogicalConstruct> body,
            PairList<List<int>, CFGBlockLogicalConstruct> nonDominatedCFGSuccessors, CaseLogicalConstruct defaultCase,
            CFGBlockLogicalConstruct defaultCFGSuccessor)
        {
            return new SwitchLogicalConstruct(entry, body, nonDominatedCFGSuccessors, defaultCase, defaultCFGSuccessor);
        }

        protected override string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedCFGBlocks, LogicalFlowBuilderContext context)
        {
            StringBuilder theBuilder = new StringBuilder(this.GetType().Name);
            theBuilder.AppendLine();
            theBuilder.AppendLine("{");
            IndentAndAppendString(theBuilder, (this.Entry as LogicalConstructBase).ToString(context));
            theBuilder.AppendLine();

            if (DefaultCase != null)
            {
                IndentAndAppendString(theBuilder, DefaultCase.ToString(context));
            }

            foreach (CaseLogicalConstruct @case in ConditionCases)
            {
                IndentAndAppendString(theBuilder, @case.ToString(context));
            }

            foreach (KeyValuePair<List<int>, CFGBlockLogicalConstruct> pair in NonDominatedCFGSuccessors)
            {
                theBuilder.Append("\tCase");
                foreach (int numLabel in pair.Key)
                {
                    theBuilder.Append(" ").Append(numLabel);
                }
                theBuilder.Append(": ").AppendLine(NodeILOffset(context, pair.Value));
            }

            if (DefaultCase == null)
            {
                theBuilder.Append("\tDefault: ").AppendLine(NodeILOffset(context, this.DefaultCFGSuccessor));
            }

            theBuilder.Append("\tFollowNode: ").AppendLine(NodeILOffset(context, this.CFGFollowNode));
            theBuilder.AppendLine("}");

            printedCFGBlocks.UnionWith(this.CFGBlocks);
            return theBuilder.ToString();
        }

        private void IndentAndAppendString(StringBuilder stringBuilder, string str)
        {
            string[] stringLines = str.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in stringLines)
            {
                stringBuilder.AppendLine("\t" + line);
            }
        }
    }
}
