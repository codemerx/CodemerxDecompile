using System.Collections.Generic;
using System.Text;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Switches
{
    class CaseLogicalConstruct : BlockLogicalConstruct
    {
        private HashSet<ILogicalConstruct> body;

        /// <summary>
        /// Creates a new case logical construct that is not yet attached to the logical tree.
        /// </summary>
        /// <param name="entry"></param>
        public CaseLogicalConstruct(ILogicalConstruct entry)
        {
            this.Entry = entry;
            this.body = new HashSet<ILogicalConstruct>();
            body.Add(entry);
            CaseNumbers = new List<int>();
        }

        /// <summary>
        /// Gets the list of all integer labels of this case.
        /// </summary>
        public List<int> CaseNumbers { get; private set; }

        /// <summary>
        /// Attaches the case construct to the logical tree.
        /// </summary>
        internal void AttachCaseConstructToGraph()
        {
            RedirectChildrenToNewParent(body);
            body = null;
        }

        /// <summary>
        /// Gets the body of the unattached case construct.
        /// </summary>
        internal HashSet<ILogicalConstruct> Body
        {
            get
            {
                return body;
            }
        }

        protected override string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedBlocks, LogicalFlowBuilderContext context)
        {
            StringBuilder nameBuilder = new StringBuilder(this.GetType().Name);
            if((this.Parent as SwitchLogicalConstruct).DefaultCase != this)
            {
                foreach (int caseIndex in CaseNumbers)
                {
                    nameBuilder.Append(' ');
                    nameBuilder.Append(caseIndex);
                }
            }
            else
            {
                nameBuilder.Append(" Default");
            }

            return base.ToString(nameBuilder.ToString(), printedBlocks, context);
        }
    }
}
