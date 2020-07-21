using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Text;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	public class CFGBlockLogicalConstruct : LogicalConstructBase, IInstructionBlockContainer
	{
		private readonly InstructionBlock theBlock;

		private readonly List<Expression> expressions;

		public override HashSet<CFGBlockLogicalConstruct> CFGBlocks
		{
			get
			{
				stackVariable0 = new HashSet<CFGBlockLogicalConstruct>();
				dummyVar0 = stackVariable0.Add(this);
				return stackVariable0;
			}
		}

		public override HashSet<ISingleEntrySubGraph> Children
		{
			get
			{
				return LogicalConstructBase.EmptyISingleEntrySubGraphSet;
			}
		}

		public override ISingleEntrySubGraph Entry
		{
			get
			{
				return this;
			}
		}

		public override CFGBlockLogicalConstruct FirstBlock
		{
			get
			{
				return this;
			}
		}

		public override int Index
		{
			get
			{
				return this.get_TheBlock().get_Index();
			}
		}

		public List<Expression> LogicalConstructExpressions
		{
			get
			{
				return this.expressions;
			}
		}

		public InstructionBlock TheBlock
		{
			get
			{
				return this.theBlock;
			}
		}

		public CFGBlockLogicalConstruct(InstructionBlock theBlock, IEnumerable<Expression> expressionsEnumeration)
		{
			base();
			this.theBlock = theBlock;
			this.expressions = new List<Expression>(expressionsEnumeration);
			return;
		}

		public override int CompareTo(ISingleEntrySubGraph other)
		{
			V_0 = this.get_Index();
			return V_0.CompareTo(other.get_Index());
		}

		protected override string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedBlocks, LogicalFlowBuilderContext context)
		{
			V_0 = new StringBuilder();
			dummyVar0 = V_0.AppendLine(this.GetType().get_Name());
			dummyVar1 = V_0.AppendLine("{");
			V_1 = this.get_TheBlock().get_First();
			while ((object)V_1 != (object)this.get_TheBlock().get_Last())
			{
				dummyVar2 = V_0.AppendLine(String.Format("\t{0}", V_1.ToString()));
				V_1 = V_1.get_Next();
			}
			dummyVar3 = V_0.AppendLine(String.Format("\t{0}", V_1.ToString()));
			V_2 = String.Format("\tFollowNode: {0}", this.NodeILOffset(context, this.get_CFGFollowNode()));
			dummyVar4 = V_0.AppendLine(V_2);
			dummyVar5 = V_0.AppendLine("}");
			dummyVar6 = printedBlocks.Add(this);
			return V_0.ToString();
		}
	}
}