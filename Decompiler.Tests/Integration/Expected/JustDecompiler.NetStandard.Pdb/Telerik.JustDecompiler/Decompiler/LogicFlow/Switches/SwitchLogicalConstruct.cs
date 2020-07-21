using System;
using System.Collections.Generic;
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
			base();
			this.set_SwitchConditionExpression(entry.get_LogicalConstructExpressions().get_Item(0));
			this.set_DefaultCase(defaultCase);
			this.set_DefaultCFGSuccessor(defaultCFGSuccessor);
			this.set_NonDominatedCFGSuccessors(nonDominatedCFGSuccessors);
			this.FillCasesArray(body);
			this.set_Entry(entry);
			this.RedirectChildrenToNewParent(this.GetBodyCollection());
			if (entry.get_CFGSuccessors().Contains(entry))
			{
				this.AddToPredecessors(entry);
				this.AddToSuccessors(entry);
			}
			return;
		}

		private void FillCasesArray(ICollection<CaseLogicalConstruct> cases)
		{
			this.set_ConditionCases(new CaseLogicalConstruct[cases.get_Count()]);
			V_0 = 0;
			V_1 = cases.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					stackVariable13 = V_0;
					V_0 = stackVariable13 + 1;
					this.get_ConditionCases()[stackVariable13] = V_2;
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return;
		}

		private ICollection<ILogicalConstruct> GetBodyCollection()
		{
			if (this.get_DefaultCase() != null)
			{
				stackVariable2 = 2;
			}
			else
			{
				stackVariable2 = 1;
			}
			V_0 = stackVariable2;
			V_1 = new ILogicalConstruct[(int)this.get_ConditionCases().Length + V_0];
			Array.Copy(this.get_ConditionCases(), V_1, (int)this.get_ConditionCases().Length);
			V_1[(int)V_1.Length - V_0] = this.get_Entry() as ILogicalConstruct;
			if (V_0 == 2)
			{
				V_1[(int)V_1.Length - 1] = this.get_DefaultCase();
			}
			return V_1;
		}

		public static SwitchLogicalConstruct GroupInSwitchConstruct(CFGBlockLogicalConstruct entry, ICollection<CaseLogicalConstruct> body, PairList<List<int>, CFGBlockLogicalConstruct> nonDominatedCFGSuccessors, CaseLogicalConstruct defaultCase, CFGBlockLogicalConstruct defaultCFGSuccessor)
		{
			return new SwitchLogicalConstruct(entry, body, nonDominatedCFGSuccessors, defaultCase, defaultCFGSuccessor);
		}

		private void IndentAndAppendString(StringBuilder stringBuilder, string str)
		{
			stackVariable2 = new String[1];
			stackVariable2[0] = Environment.get_NewLine();
			V_0 = str.Split(stackVariable2, 1);
			V_1 = 0;
			while (V_1 < (int)V_0.Length)
			{
				V_2 = V_0[V_1];
				dummyVar0 = stringBuilder.AppendLine(String.Concat("\t", V_2));
				V_1 = V_1 + 1;
			}
			return;
		}

		protected override string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedCFGBlocks, LogicalFlowBuilderContext context)
		{
			V_0 = new StringBuilder(this.GetType().get_Name());
			dummyVar0 = V_0.AppendLine();
			dummyVar1 = V_0.AppendLine("{");
			this.IndentAndAppendString(V_0, (this.get_Entry() as LogicalConstructBase).ToString(context));
			dummyVar2 = V_0.AppendLine();
			if (this.get_DefaultCase() != null)
			{
				this.IndentAndAppendString(V_0, this.get_DefaultCase().ToString(context));
			}
			V_1 = this.get_ConditionCases();
			V_2 = 0;
			while (V_2 < (int)V_1.Length)
			{
				this.IndentAndAppendString(V_0, V_1[V_2].ToString(context));
				V_2 = V_2 + 1;
			}
			V_4 = this.get_NonDominatedCFGSuccessors().GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_5 = V_4.get_Current();
					dummyVar3 = V_0.Append("\tCase");
					V_6 = V_5.get_Key().GetEnumerator();
					try
					{
						while (V_6.MoveNext())
						{
							V_7 = V_6.get_Current();
							dummyVar4 = V_0.Append(" ").Append(V_7);
						}
					}
					finally
					{
						((IDisposable)V_6).Dispose();
					}
					dummyVar5 = V_0.Append(": ").AppendLine(this.NodeILOffset(context, V_5.get_Value()));
				}
			}
			finally
			{
				((IDisposable)V_4).Dispose();
			}
			if (this.get_DefaultCase() == null)
			{
				dummyVar6 = V_0.Append("\tDefault: ").AppendLine(this.NodeILOffset(context, this.get_DefaultCFGSuccessor()));
			}
			dummyVar7 = V_0.Append("\tFollowNode: ").AppendLine(this.NodeILOffset(context, this.get_CFGFollowNode()));
			dummyVar8 = V_0.AppendLine("}");
			printedCFGBlocks.UnionWith(this.get_CFGBlocks());
			return V_0.ToString();
		}
	}
}