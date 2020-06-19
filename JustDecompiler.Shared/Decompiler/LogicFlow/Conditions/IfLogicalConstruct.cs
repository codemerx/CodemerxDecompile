using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions
{
    class IfLogicalConstruct : LogicalConstructBase
    {
        /// <summary>
        /// Gets the then block of the if construct.
        /// </summary>
        public BlockLogicalConstruct Then { get; private set; }

        /// <summary>
        /// Gets the else block of the if construct.
        /// </summary>
        public BlockLogicalConstruct Else { get; private set; }

        /// <summary>
        /// Gets the condition ot the if construct.
        /// </summary>
		public ConditionLogicalConstruct Condition { get; private set; }

        public override Cil.ISingleEntrySubGraph Entry
        {
            get
            {
                return this.Condition;
            }
        }

        private IfLogicalConstruct(ConditionLogicalConstruct condition, BlockLogicalConstruct thenBlock, BlockLogicalConstruct elseBlock)
        {
			condition.LogicalContainer = this;

            this.Condition = condition;
            this.Then = thenBlock;
            this.Else = elseBlock;

            RedirectChildrenToNewParent(GetIfBody());
        }

        private ICollection<ILogicalConstruct> GetIfBody()
        {
            List<ILogicalConstruct> body = new List<ILogicalConstruct>();
            body.Add(this.Condition);
            body.Add(this.Then);
            if(this.Else != null)
            {
                body.Add(this.Else);
            }

            return body;
        }

        /// <summary>
        /// Creates a new IfLogicalConstruct and adds it to the logical tree.
        /// </summary>
        /// <param name="condition">The condition of the if.</param>
        /// <param name="theThenBlock">The then block of the if.</param>
        /// <param name="theElseBlock">The else block of the if.</param>
        /// <returns>The created if construct.</returns>
		public static IfLogicalConstruct GroupInIfConstruct(ConditionLogicalConstruct condition,
            BlockLogicalConstruct theThenBlock, BlockLogicalConstruct theElseBlock)
		{
			return new IfLogicalConstruct(condition, theThenBlock, theElseBlock);
		}

        /// <summary>
        /// Swaps the then and else and negates the condition
        /// </summary>
        public void Negate(TypeSystem typeSystem)
        {
            this.Condition.Negate(typeSystem);
            BlockLogicalConstruct temp = this.Then;
            this.Then = this.Else;
            this.Else = temp;
        }

        protected override string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedBlocks, LogicalFlowBuilderContext context)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("IfLogicalConstruct");
            sb.AppendLine("{");

            StringBuilder childStrings = new StringBuilder();
            childStrings.Append((Entry as ConditionLogicalConstruct).ToString(context));
            childStrings.Append(Then.ToString(context));
            if(Else != null)
            {
                childStrings.Append(Else.ToString(context));
            }

            foreach (string line in childStrings.ToString().Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                sb.AppendLine(string.Format("\t{0}", line));
            }

            string followNodeString = string.Format("\tFollowNode: {0}", NodeILOffset(context, CFGFollowNode));
            sb.AppendLine(followNodeString);
            sb.AppendLine("}");

            printedBlocks.UnionWith(this.CFGBlocks);

            return sb.ToString();
        }
    }
}
