using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Loops
{
	internal class LoopLogicalConstruct : LogicalConstructBase, IBreaksContainer, ILogicalConstruct, ISingleEntrySubGraph, IComparable<ISingleEntrySubGraph>
	{
		public override ISingleEntrySubGraph Entry
		{
			get
			{
				if (this.get_LoopType() == 1)
				{
					return this.get_LoopCondition();
				}
				return this.get_LoopBodyBlock();
			}
		}

		public BlockLogicalConstruct LoopBodyBlock
		{
			get;
			private set;
		}

		public ConditionLogicalConstruct LoopCondition
		{
			get;
			private set;
		}

		public CFGBlockLogicalConstruct LoopContinueEndPoint
		{
			get
			{
				if (this.get_LoopType() == Telerik.JustDecompiler.Decompiler.LogicFlow.Loops.LoopType.InfiniteLoop)
				{
					return this.get_LoopBodyBlock().get_FirstBlock();
				}
				return this.get_LoopCondition().get_FirstBlock();
			}
		}

		public Telerik.JustDecompiler.Decompiler.LogicFlow.Loops.LoopType LoopType
		{
			get;
			private set;
		}

		public LoopLogicalConstruct(ILogicalConstruct entry, HashSet<ILogicalConstruct> loopBody, Telerik.JustDecompiler.Decompiler.LogicFlow.Loops.LoopType loopType, ConditionLogicalConstruct loopCondition, TypeSystem typeSystem)
		{
			base();
			if (loopCondition != null)
			{
				loopCondition.set_LogicalContainer(this);
			}
			this.set_LoopType(loopType);
			this.set_LoopCondition(loopCondition);
			if (this.get_LoopType() != Telerik.JustDecompiler.Decompiler.LogicFlow.Loops.LoopType.InfiniteLoop)
			{
				dummyVar0 = loopBody.Remove(this.get_LoopCondition());
			}
			this.DetermineLoopBodyBlock(entry, loopBody);
			this.RedirectChildrenToNewParent(this.GetLoopChildrenCollection());
			this.FixLoopCondition(typeSystem);
			return;
		}

		private void DetermineLoopBodyBlock(ILogicalConstruct entry, HashSet<ILogicalConstruct> loopBodyNodes)
		{
			this.set_LoopBodyBlock(null);
			if (loopBodyNodes.get_Count() > 0)
			{
				if (this.get_LoopType() != 1)
				{
					if (!loopBodyNodes.Contains(entry))
					{
						throw new Exception("Invalid entry of loop body.");
					}
					this.set_LoopBodyBlock(new BlockLogicalConstruct(entry, loopBodyNodes));
					return;
				}
				V_0 = this.get_LoopCondition().get_TrueSuccessor();
				if (V_0 == null || !loopBodyNodes.Contains(V_0))
				{
					V_0 = this.get_LoopCondition().get_FalseSuccessor();
				}
				if (V_0 == null || !loopBodyNodes.Contains(V_0))
				{
					throw new Exception("Invalid entry of loop body.");
				}
				this.set_LoopBodyBlock(new BlockLogicalConstruct(V_0, loopBodyNodes));
			}
			return;
		}

		private void FixLoopCondition(TypeSystem typeSystem)
		{
			if (this.get_LoopType() == Telerik.JustDecompiler.Decompiler.LogicFlow.Loops.LoopType.InfiniteLoop)
			{
				return;
			}
			if (this.get_LoopCondition().get_TrueSuccessor() == null)
			{
				this.get_LoopCondition().Negate(typeSystem);
			}
			if (this.get_LoopCondition().get_TrueSuccessor() == null)
			{
				throw new ArgumentException("The loop condition must have a true successor inside of the loop");
			}
			return;
		}

		private ICollection<ILogicalConstruct> GetLoopChildrenCollection()
		{
			V_0 = new List<ILogicalConstruct>();
			if (this.get_LoopBodyBlock() != null)
			{
				V_0.Add(this.get_LoopBodyBlock());
			}
			if (this.get_LoopCondition() != null)
			{
				V_0.Add(this.get_LoopCondition());
			}
			return V_0;
		}

		protected override string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedBlocks, LogicalFlowBuilderContext context)
		{
			V_0 = new StringBuilder();
			dummyVar0 = V_0.Append(this.get_LoopType());
			dummyVar1 = V_0.AppendLine("LogicalConstruct");
			dummyVar2 = V_0.AppendLine("{");
			V_2 = this.GetSortedArrayFromCollection<ISingleEntrySubGraph>(this.get_Children());
			V_3 = 0;
			while (V_3 < (int)V_2.Length)
			{
				V_4 = (LogicalConstructBase)V_2[V_3];
				stackVariable27 = V_4.ToString(context);
				stackVariable29 = new String[1];
				stackVariable29[0] = Environment.get_NewLine();
				V_5 = stackVariable27.Split(stackVariable29, 1);
				V_6 = 0;
				while (V_6 < (int)V_5.Length)
				{
					V_7 = V_5[V_6];
					dummyVar3 = V_0.Append('\t');
					dummyVar4 = V_0.AppendLine(V_7);
					V_6 = V_6 + 1;
				}
				printedBlocks.UnionWith(V_4.get_CFGBlocks());
				V_3 = V_3 + 1;
			}
			V_1 = String.Format("\tFollowNode: {0}", this.NodeILOffset(context, this.get_CFGFollowNode()));
			dummyVar5 = V_0.AppendLine(V_1);
			dummyVar6 = V_0.AppendLine("}");
			return V_0.ToString();
		}
	}
}