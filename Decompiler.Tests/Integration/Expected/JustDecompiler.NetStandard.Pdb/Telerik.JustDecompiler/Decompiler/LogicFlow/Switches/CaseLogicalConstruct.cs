using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Telerik.JustDecompiler.Decompiler.LogicFlow;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Switches
{
	internal class CaseLogicalConstruct : BlockLogicalConstruct
	{
		private HashSet<ILogicalConstruct> body;

		internal HashSet<ILogicalConstruct> Body
		{
			get
			{
				return this.body;
			}
		}

		public List<int> CaseNumbers
		{
			get;
			private set;
		}

		public CaseLogicalConstruct(ILogicalConstruct entry)
		{
			base();
			this.set_Entry(entry);
			this.body = new HashSet<ILogicalConstruct>();
			dummyVar0 = this.body.Add(entry);
			this.set_CaseNumbers(new List<int>());
			return;
		}

		internal void AttachCaseConstructToGraph()
		{
			this.RedirectChildrenToNewParent(this.body);
			this.body = null;
			return;
		}

		protected override string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedBlocks, LogicalFlowBuilderContext context)
		{
			V_0 = new StringBuilder(this.GetType().get_Name());
			if ((this.get_Parent() as SwitchLogicalConstruct).get_DefaultCase() == this)
			{
				dummyVar2 = V_0.Append(" Default");
			}
			else
			{
				V_1 = this.get_CaseNumbers().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						dummyVar0 = V_0.Append(' ');
						dummyVar1 = V_0.Append(V_2);
					}
				}
				finally
				{
					((IDisposable)V_1).Dispose();
				}
			}
			return this.ToString(V_0.ToString(), printedBlocks, context);
		}
	}
}