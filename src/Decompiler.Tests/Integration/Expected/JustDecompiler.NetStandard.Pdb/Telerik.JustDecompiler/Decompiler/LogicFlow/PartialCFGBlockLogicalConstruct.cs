using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	public class PartialCFGBlockLogicalConstruct : CFGBlockLogicalConstruct
	{
		public CFGBlockLogicalConstruct OriginalCFGConstruct
		{
			get;
			private set;
		}

		public PartialCFGBlockLogicalConstruct(CFGBlockLogicalConstruct originalCFGConstruct, IEnumerable<Expression> expressions) : base(originalCFGConstruct.TheBlock, expressions)
		{
			this.OriginalCFGConstruct = originalCFGConstruct;
			this.RedirectParent();
		}

		public override int CompareTo(ISingleEntrySubGraph other)
		{
			object enumerator = null;
			PartialCFGBlockLogicalConstruct firstBlock = (other as ILogicalConstruct).FirstBlock as PartialCFGBlockLogicalConstruct;
			if (firstBlock == null || this.Index != firstBlock.Index)
			{
				return base.CompareTo(other);
			}
			if (this == firstBlock)
			{
				return 0;
			}
			for (PartialCFGBlockLogicalConstruct i = this; i != null && i.Index == this.Index; i = enumerator.Current as PartialCFGBlockLogicalConstruct)
			{
				if (i == firstBlock)
				{
					return -1;
				}
				if (i.CFGSuccessors.Count != 1)
				{
					break;
				}
				enumerator = i.CFGSuccessors.GetEnumerator();
				enumerator.MoveNext();
			}
			return 1;
		}

		private void RedirectParent()
		{
			base.Parent = this.OriginalCFGConstruct.Parent;
			base.Parent.Children.Remove(this.OriginalCFGConstruct);
			base.Parent.Children.Add(this);
		}

		public void RedirectPredecessors()
		{
			if (base.Parent.Entry == this.OriginalCFGConstruct)
			{
				base.Parent.Entry = this;
			}
			foreach (CFGBlockLogicalConstruct cFGPredecessor in this.OriginalCFGConstruct.CFGPredecessors)
			{
				cFGPredecessor.RemoveFromSuccessors(this.OriginalCFGConstruct);
				cFGPredecessor.AddToSuccessors(this);
				base.AddToPredecessors(cFGPredecessor);
				for (LogicalConstructBase i = cFGPredecessor.Parent as LogicalConstructBase; i != null; i = i.Parent as LogicalConstructBase)
				{
					if (i.RemoveFromSuccessors(this.OriginalCFGConstruct))
					{
						i.AddToSuccessors(this);
					}
				}
			}
		}

		public void RedirectSuccessors()
		{
			foreach (LogicalConstructBase cFGSuccessor in this.OriginalCFGConstruct.CFGSuccessors)
			{
				cFGSuccessor.RemoveFromPredecessors(this.OriginalCFGConstruct);
				cFGSuccessor.AddToPredecessors(this);
				base.AddToSuccessors(cFGSuccessor);
				for (LogicalConstructBase i = cFGSuccessor.Parent as LogicalConstructBase; i != null; i = i.Parent as LogicalConstructBase)
				{
					if (i.RemoveFromPredecessors(this.OriginalCFGConstruct))
					{
						i.AddToPredecessors(this);
					}
				}
			}
		}

		protected override string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedBlocks, LogicalFlowBuilderContext context)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("PartialCFGConstruct");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine(String.Format("\t{0}:", base.NodeILOffset(context, this)));
			foreach (Expression logicalConstructExpression in base.LogicalConstructExpressions)
			{
				stringBuilder.Append("\t");
				stringBuilder.AppendLine(logicalConstructExpression.ToCodeString());
			}
			string str = String.Format("\tFollowNode: {0}", base.NodeILOffset(context, base.CFGFollowNode));
			stringBuilder.AppendLine(str);
			stringBuilder.AppendLine("}");
			printedBlocks.Add(this);
			return stringBuilder.ToString();
		}
	}
}