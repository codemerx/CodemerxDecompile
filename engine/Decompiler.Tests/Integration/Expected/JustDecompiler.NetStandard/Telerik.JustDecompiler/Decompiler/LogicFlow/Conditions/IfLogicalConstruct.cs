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
				return this.Condition;
			}
		}

		public BlockLogicalConstruct Then
		{
			get;
			private set;
		}

		private IfLogicalConstruct(ConditionLogicalConstruct condition, BlockLogicalConstruct thenBlock, BlockLogicalConstruct elseBlock)
		{
			condition.LogicalContainer = this;
			this.Condition = condition;
			this.Then = thenBlock;
			this.Else = elseBlock;
			base.RedirectChildrenToNewParent(this.GetIfBody());
		}

		private ICollection<ILogicalConstruct> GetIfBody()
		{
			List<ILogicalConstruct> logicalConstructs = new List<ILogicalConstruct>()
			{
				this.Condition,
				this.Then
			};
			if (this.Else != null)
			{
				logicalConstructs.Add(this.Else);
			}
			return logicalConstructs;
		}

		public static IfLogicalConstruct GroupInIfConstruct(ConditionLogicalConstruct condition, BlockLogicalConstruct theThenBlock, BlockLogicalConstruct theElseBlock)
		{
			return new IfLogicalConstruct(condition, theThenBlock, theElseBlock);
		}

		public void Negate(TypeSystem typeSystem)
		{
			this.Condition.Negate(typeSystem);
			BlockLogicalConstruct then = this.Then;
			this.Then = this.Else;
			this.Else = then;
		}

		protected override string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedBlocks, LogicalFlowBuilderContext context)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("IfLogicalConstruct");
			stringBuilder.AppendLine("{");
			StringBuilder stringBuilder1 = new StringBuilder();
			stringBuilder1.Append((this.Entry as ConditionLogicalConstruct).ToString(context));
			stringBuilder1.Append(this.Then.ToString(context));
			if (this.Else != null)
			{
				stringBuilder1.Append(this.Else.ToString(context));
			}
			string[] strArray = stringBuilder1.ToString().Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < (int)strArray.Length; i++)
			{
				string str = strArray[i];
				stringBuilder.AppendLine(String.Format("\t{0}", str));
			}
			string str1 = String.Format("\tFollowNode: {0}", base.NodeILOffset(context, base.CFGFollowNode));
			stringBuilder.AppendLine(str1);
			stringBuilder.AppendLine("}");
			printedBlocks.UnionWith(this.CFGBlocks);
			return stringBuilder.ToString();
		}
	}
}