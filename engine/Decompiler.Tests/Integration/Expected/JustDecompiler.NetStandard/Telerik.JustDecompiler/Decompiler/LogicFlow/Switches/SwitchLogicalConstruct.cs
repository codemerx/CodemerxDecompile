using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler.LogicFlow;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Switches
{
	internal class SwitchLogicalConstruct : LogicalConstructBase, IBreaksContainer, ILogicalConstruct, ISingleEntrySubGraph, IComparable<ISingleEntrySubGraph>
	{
		public CaseLogicalConstruct[] ConditionCases
		{
			get;
			private set;
		}

		public CaseLogicalConstruct DefaultCase
		{
			get;
			private set;
		}

		public CFGBlockLogicalConstruct DefaultCFGSuccessor
		{
			get;
			private set;
		}

		public PairList<List<int>, CFGBlockLogicalConstruct> NonDominatedCFGSuccessors
		{
			get;
			private set;
		}

		public Expression SwitchConditionExpression
		{
			get;
			private set;
		}

		private SwitchLogicalConstruct(CFGBlockLogicalConstruct entry, ICollection<CaseLogicalConstruct> body, PairList<List<int>, CFGBlockLogicalConstruct> nonDominatedCFGSuccessors, CaseLogicalConstruct defaultCase, CFGBlockLogicalConstruct defaultCFGSuccessor)
		{
			this.SwitchConditionExpression = entry.LogicalConstructExpressions[0];
			this.DefaultCase = defaultCase;
			this.DefaultCFGSuccessor = defaultCFGSuccessor;
			this.NonDominatedCFGSuccessors = nonDominatedCFGSuccessors;
			this.FillCasesArray(body);
			this.Entry = entry;
			base.RedirectChildrenToNewParent(this.GetBodyCollection());
			if (entry.CFGSuccessors.Contains(entry))
			{
				base.AddToPredecessors(entry);
				base.AddToSuccessors(entry);
			}
		}

		private void FillCasesArray(ICollection<CaseLogicalConstruct> cases)
		{
			this.ConditionCases = new CaseLogicalConstruct[cases.Count];
			int num = 0;
			foreach (CaseLogicalConstruct @case in cases)
			{
				int num1 = num;
				num = num1 + 1;
				this.ConditionCases[num1] = @case;
			}
		}

		private ICollection<ILogicalConstruct> GetBodyCollection()
		{
			int num = (this.DefaultCase != null ? 2 : 1);
			ILogicalConstruct[] entry = new ILogicalConstruct[(int)this.ConditionCases.Length + num];
			Array.Copy(this.ConditionCases, entry, (int)this.ConditionCases.Length);
			entry[(int)entry.Length - num] = this.Entry as ILogicalConstruct;
			if (num == 2)
			{
				entry[(int)entry.Length - 1] = this.DefaultCase;
			}
			return entry;
		}

		public static SwitchLogicalConstruct GroupInSwitchConstruct(CFGBlockLogicalConstruct entry, ICollection<CaseLogicalConstruct> body, PairList<List<int>, CFGBlockLogicalConstruct> nonDominatedCFGSuccessors, CaseLogicalConstruct defaultCase, CFGBlockLogicalConstruct defaultCFGSuccessor)
		{
			return new SwitchLogicalConstruct(entry, body, nonDominatedCFGSuccessors, defaultCase, defaultCFGSuccessor);
		}

		private void IndentAndAppendString(StringBuilder stringBuilder, string str)
		{
			string[] strArray = str.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < (int)strArray.Length; i++)
			{
				string str1 = strArray[i];
				stringBuilder.AppendLine(String.Concat("\t", str1));
			}
		}

		protected override string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedCFGBlocks, LogicalFlowBuilderContext context)
		{
			StringBuilder stringBuilder = new StringBuilder(base.GetType().Name);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("{");
			this.IndentAndAppendString(stringBuilder, (this.Entry as LogicalConstructBase).ToString(context));
			stringBuilder.AppendLine();
			if (this.DefaultCase != null)
			{
				this.IndentAndAppendString(stringBuilder, this.DefaultCase.ToString(context));
			}
			CaseLogicalConstruct[] conditionCases = this.ConditionCases;
			for (int i = 0; i < (int)conditionCases.Length; i++)
			{
				this.IndentAndAppendString(stringBuilder, conditionCases[i].ToString(context));
			}
			foreach (KeyValuePair<List<int>, CFGBlockLogicalConstruct> nonDominatedCFGSuccessor in this.NonDominatedCFGSuccessors)
			{
				stringBuilder.Append("\tCase");
				foreach (int key in nonDominatedCFGSuccessor.Key)
				{
					stringBuilder.Append(" ").Append(key);
				}
				stringBuilder.Append(": ").AppendLine(base.NodeILOffset(context, nonDominatedCFGSuccessor.Value));
			}
			if (this.DefaultCase == null)
			{
				stringBuilder.Append("\tDefault: ").AppendLine(base.NodeILOffset(context, this.DefaultCFGSuccessor));
			}
			stringBuilder.Append("\tFollowNode: ").AppendLine(base.NodeILOffset(context, base.CFGFollowNode));
			stringBuilder.AppendLine("}");
			printedCFGBlocks.UnionWith(this.CFGBlocks);
			return stringBuilder.ToString();
		}
	}
}