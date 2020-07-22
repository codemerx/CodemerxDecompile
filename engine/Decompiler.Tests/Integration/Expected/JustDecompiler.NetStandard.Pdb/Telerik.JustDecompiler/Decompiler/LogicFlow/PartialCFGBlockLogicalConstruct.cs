using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
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

		public PartialCFGBlockLogicalConstruct(CFGBlockLogicalConstruct originalCFGConstruct, IEnumerable<Expression> expressions)
		{
			base(originalCFGConstruct.get_TheBlock(), expressions);
			this.set_OriginalCFGConstruct(originalCFGConstruct);
			this.RedirectParent();
			return;
		}

		public override int CompareTo(ISingleEntrySubGraph other)
		{
			V_0 = (other as ILogicalConstruct).get_FirstBlock() as PartialCFGBlockLogicalConstruct;
			if (V_0 == null || this.get_Index() != V_0.get_Index())
			{
				return this.CompareTo(other);
			}
			if (this == V_0)
			{
				return 0;
			}
			V_1 = this;
			while (V_1 != null && V_1.get_Index() == this.get_Index())
			{
				if (V_1 == V_0)
				{
					return -1;
				}
				if (V_1.get_CFGSuccessors().get_Count() != 1)
				{
					break;
				}
				stackVariable30 = V_1.get_CFGSuccessors().GetEnumerator();
				dummyVar0 = stackVariable30.MoveNext();
				V_1 = stackVariable30.get_Current() as PartialCFGBlockLogicalConstruct;
			}
			return 1;
		}

		private void RedirectParent()
		{
			this.set_Parent(this.get_OriginalCFGConstruct().get_Parent());
			dummyVar0 = this.get_Parent().get_Children().Remove(this.get_OriginalCFGConstruct());
			dummyVar1 = this.get_Parent().get_Children().Add(this);
			return;
		}

		public void RedirectPredecessors()
		{
			if (this.get_Parent().get_Entry() == this.get_OriginalCFGConstruct())
			{
				this.get_Parent().set_Entry(this);
			}
			V_0 = this.get_OriginalCFGConstruct().get_CFGPredecessors().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					dummyVar0 = V_1.RemoveFromSuccessors(this.get_OriginalCFGConstruct());
					V_1.AddToSuccessors(this);
					this.AddToPredecessors(V_1);
					V_2 = V_1.get_Parent() as LogicalConstructBase;
					while (V_2 != null)
					{
						if (V_2.RemoveFromSuccessors(this.get_OriginalCFGConstruct()))
						{
							V_2.AddToSuccessors(this);
						}
						V_2 = V_2.get_Parent() as LogicalConstructBase;
					}
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		public void RedirectSuccessors()
		{
			V_0 = this.get_OriginalCFGConstruct().get_CFGSuccessors().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					dummyVar0 = V_1.RemoveFromPredecessors(this.get_OriginalCFGConstruct());
					V_1.AddToPredecessors(this);
					this.AddToSuccessors(V_1);
					V_2 = V_1.get_Parent() as LogicalConstructBase;
					while (V_2 != null)
					{
						if (V_2.RemoveFromPredecessors(this.get_OriginalCFGConstruct()))
						{
							V_2.AddToPredecessors(this);
						}
						V_2 = V_2.get_Parent() as LogicalConstructBase;
					}
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		protected override string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedBlocks, LogicalFlowBuilderContext context)
		{
			V_0 = new StringBuilder();
			dummyVar0 = V_0.AppendLine("PartialCFGConstruct");
			dummyVar1 = V_0.AppendLine("{");
			dummyVar2 = V_0.AppendLine(String.Format("\t{0}:", this.NodeILOffset(context, this)));
			V_2 = this.get_LogicalConstructExpressions().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					dummyVar3 = V_0.Append("\t");
					dummyVar4 = V_0.AppendLine(V_3.ToCodeString());
				}
			}
			finally
			{
				((IDisposable)V_2).Dispose();
			}
			V_1 = String.Format("\tFollowNode: {0}", this.NodeILOffset(context, this.get_CFGFollowNode()));
			dummyVar5 = V_0.AppendLine(V_1);
			dummyVar6 = V_0.AppendLine("}");
			dummyVar7 = printedBlocks.Add(this);
			return V_0.ToString();
		}
	}
}