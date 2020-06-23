using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
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
				HashSet<CFGBlockLogicalConstruct> cFGBlockLogicalConstructs = new HashSet<CFGBlockLogicalConstruct>();
				cFGBlockLogicalConstructs.Add(this);
				return cFGBlockLogicalConstructs;
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
				return this.TheBlock.Index;
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
			this.theBlock = theBlock;
			this.expressions = new List<Expression>(expressionsEnumeration);
		}

		public override int CompareTo(ISingleEntrySubGraph other)
		{
			return this.Index.CompareTo(other.Index);
		}

		protected override string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedBlocks, LogicalFlowBuilderContext context)
		{
			Instruction i;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(base.GetType().Name);
			stringBuilder.AppendLine("{");
			for (i = this.TheBlock.First; i != this.TheBlock.Last; i = i.Next)
			{
				stringBuilder.AppendLine(String.Format("\t{0}", i.ToString()));
			}
			stringBuilder.AppendLine(String.Format("\t{0}", i.ToString()));
			string str = String.Format("\tFollowNode: {0}", base.NodeILOffset(context, base.CFGFollowNode));
			stringBuilder.AppendLine(str);
			stringBuilder.AppendLine("}");
			printedBlocks.Add(this);
			return stringBuilder.ToString();
		}
	}
}