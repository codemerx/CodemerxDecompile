using System;
using System.Collections.Generic;
using System.Reflection;
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
			this.Entry = entry;
			this.body = new HashSet<ILogicalConstruct>();
			this.body.Add(entry);
			this.CaseNumbers = new List<int>();
		}

		internal void AttachCaseConstructToGraph()
		{
			base.RedirectChildrenToNewParent(this.body);
			this.body = null;
		}

		protected override string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedBlocks, LogicalFlowBuilderContext context)
		{
			StringBuilder stringBuilder = new StringBuilder(base.GetType().Name);
			if ((base.Parent as SwitchLogicalConstruct).DefaultCase == this)
			{
				stringBuilder.Append(" Default");
			}
			else
			{
				foreach (int caseNumber in this.CaseNumbers)
				{
					stringBuilder.Append(' ');
					stringBuilder.Append(caseNumber);
				}
			}
			return base.ToString(stringBuilder.ToString(), printedBlocks, context);
		}
	}
}