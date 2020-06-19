using System.Collections.Generic;
using System.Text;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
    public class PartialCFGBlockLogicalConstruct : CFGBlockLogicalConstruct
    {
        public CFGBlockLogicalConstruct OriginalCFGConstruct { get; private set; }

        public PartialCFGBlockLogicalConstruct(CFGBlockLogicalConstruct originalCFGConstruct, IEnumerable<Expression> expressions)
        :base(originalCFGConstruct.TheBlock, expressions)
        {
            OriginalCFGConstruct = originalCFGConstruct;
            RedirectParent();
        }
        
        private void RedirectParent()
        {
            this.Parent = OriginalCFGConstruct.Parent;
            this.Parent.Children.Remove(OriginalCFGConstruct);
            this.Parent.Children.Add(this);
        }

        public void RedirectPredecessors()
        {
            if (this.Parent.Entry == OriginalCFGConstruct)
            {
                this.Parent.Entry = this;
            }

			foreach (CFGBlockLogicalConstruct predecessor in OriginalCFGConstruct.CFGPredecessors)
            {
                predecessor.RemoveFromSuccessors(OriginalCFGConstruct);
                predecessor.AddToSuccessors(this);
                this.AddToPredecessors(predecessor);

                LogicalConstructBase currentPredecessorParent = predecessor.Parent as LogicalConstructBase;
                while(currentPredecessorParent != null)
                {
                    if(currentPredecessorParent.RemoveFromSuccessors(OriginalCFGConstruct))
                    {
                        currentPredecessorParent.AddToSuccessors(this);
                    }

                    currentPredecessorParent = currentPredecessorParent.Parent as LogicalConstructBase;
                }

            }
        }

        public void RedirectSuccessors()
        {
            foreach (LogicalConstructBase successor in OriginalCFGConstruct.CFGSuccessors)
            {
                successor.RemoveFromPredecessors(OriginalCFGConstruct);
                successor.AddToPredecessors(this);
                this.AddToSuccessors(successor);

                LogicalConstructBase currentSuccessorParent = successor.Parent as LogicalConstructBase;
                while(currentSuccessorParent != null)
                {
                    if(currentSuccessorParent.RemoveFromPredecessors(OriginalCFGConstruct))
                    {
                        currentSuccessorParent.AddToPredecessors(this);
                    }

                    currentSuccessorParent = currentSuccessorParent.Parent as LogicalConstructBase;
                }
            }
        }

        public override int CompareTo(Cil.ISingleEntrySubGraph other)
        {
            PartialCFGBlockLogicalConstruct otherPartial = (other as ILogicalConstruct).FirstBlock as PartialCFGBlockLogicalConstruct;
            if(otherPartial != null)
            {
                if(this.Index == otherPartial.Index)
                {
                    if(this == otherPartial)
                    {
                        return 0;
                    }

                    PartialCFGBlockLogicalConstruct currentPartial = this;
                    while (currentPartial != null && currentPartial.Index == this.Index)
                    {
                        if(currentPartial == otherPartial)
                        {
                            return -1;
                        }

                        if(currentPartial.CFGSuccessors.Count != 1)
                        {
                            break;
                        }

                        IEnumerator<CFGBlockLogicalConstruct> cfgSuccessorsEnumerator = currentPartial.CFGSuccessors.GetEnumerator();
                        cfgSuccessorsEnumerator.MoveNext();
                        currentPartial = cfgSuccessorsEnumerator.Current as PartialCFGBlockLogicalConstruct;
                    }

                    return 1;
                }
            }
            return base.CompareTo(other);
        }

        protected override string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedBlocks, LogicalFlowBuilderContext context)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("PartialCFGConstruct");
            sb.AppendLine("{");
            sb.AppendLine(string.Format("\t{0}:", NodeILOffset(context, this)));

            foreach (Telerik.JustDecompiler.Ast.Expressions.Expression expression in LogicalConstructExpressions)
            {
                sb.Append("\t");
                sb.AppendLine(expression.ToCodeString());
            }

            string followNodeString = string.Format("\tFollowNode: {0}", NodeILOffset(context, CFGFollowNode));
            sb.AppendLine(followNodeString);
            sb.AppendLine("}");

            printedBlocks.Add(this);

            return sb.ToString();
        }
    }
}
