using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions
{
	internal class IfLogicalConstruct : LogicalConstructBase
	{
		public ConditionLogicalConstruct Condition
		{
			get;
			private set;
		}

		public BlockLogicalConstruct Else
		{
			get;
			private set;
		}

		public override ISingleEntrySubGraph Entry
		{
			get
			{
				return this.get_Condition();
			}
		}

		public BlockLogicalConstruct Then
		{
			get;
			private set;
		}

		private IfLogicalConstruct(ConditionLogicalConstruct condition, BlockLogicalConstruct thenBlock, BlockLogicalConstruct elseBlock)
		{
			base();
			condition.set_LogicalContainer(this);
			this.set_Condition(condition);
			this.set_Then(thenBlock);
			this.set_Else(elseBlock);
			this.RedirectChildrenToNewParent(this.GetIfBody());
			return;
		}

		private ICollection<ILogicalConstruct> GetIfBody()
		{
			V_0 = new List<ILogicalConstruct>();
			V_0.Add(this.get_Condition());
			V_0.Add(this.get_Then());
			if (this.get_Else() != null)
			{
				V_0.Add(this.get_Else());
			}
			return V_0;
		}

		public static IfLogicalConstruct GroupInIfConstruct(ConditionLogicalConstruct condition, BlockLogicalConstruct theThenBlock, BlockLogicalConstruct theElseBlock)
		{
			return new IfLogicalConstruct(condition, theThenBlock, theElseBlock);
		}

		public void Negate(TypeSystem typeSystem)
		{
			this.get_Condition().Negate(typeSystem);
			V_0 = this.get_Then();
			this.set_Then(this.get_Else());
			this.set_Else(V_0);
			return;
		}

		protected override string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedBlocks, LogicalFlowBuilderContext context)
		{
			V_0 = new StringBuilder();
			dummyVar0 = V_0.AppendLine("IfLogicalConstruct");
			dummyVar1 = V_0.AppendLine("{");
			V_1 = new StringBuilder();
			dummyVar2 = V_1.Append((this.get_Entry() as ConditionLogicalConstruct).ToString(context));
			dummyVar3 = V_1.Append(this.get_Then().ToString(context));
			if (this.get_Else() != null)
			{
				dummyVar4 = V_1.Append(this.get_Else().ToString(context));
			}
			stackVariable24 = V_1.ToString();
			stackVariable26 = new String[1];
			stackVariable26[0] = Environment.get_NewLine();
			V_3 = stackVariable24.Split(stackVariable26, 1);
			V_4 = 0;
			while (V_4 < (int)V_3.Length)
			{
				V_5 = V_3[V_4];
				dummyVar5 = V_0.AppendLine(String.Format("\t{0}", V_5));
				V_4 = V_4 + 1;
			}
			V_2 = String.Format("\tFollowNode: {0}", this.NodeILOffset(context, this.get_CFGFollowNode()));
			dummyVar6 = V_0.AppendLine(V_2);
			dummyVar7 = V_0.AppendLine("}");
			printedBlocks.UnionWith(this.get_CFGBlocks());
			return V_0.ToString();
		}
	}
}