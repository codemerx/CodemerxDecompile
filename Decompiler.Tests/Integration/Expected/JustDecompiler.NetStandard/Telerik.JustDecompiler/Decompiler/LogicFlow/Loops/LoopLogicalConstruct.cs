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
				if (this.LoopType == Telerik.JustDecompiler.Decompiler.LogicFlow.Loops.LoopType.PreTestedLoop)
				{
					return this.LoopCondition;
				}
				return this.LoopBodyBlock;
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
				if (this.LoopType == Telerik.JustDecompiler.Decompiler.LogicFlow.Loops.LoopType.InfiniteLoop)
				{
					return this.LoopBodyBlock.FirstBlock;
				}
				return this.LoopCondition.FirstBlock;
			}
		}

		public Telerik.JustDecompiler.Decompiler.LogicFlow.Loops.LoopType LoopType
		{
			get;
			private set;
		}

		public LoopLogicalConstruct(ILogicalConstruct entry, HashSet<ILogicalConstruct> loopBody, Telerik.JustDecompiler.Decompiler.LogicFlow.Loops.LoopType loopType, ConditionLogicalConstruct loopCondition, TypeSystem typeSystem)
		{
			if (loopCondition != null)
			{
				loopCondition.LogicalContainer = this;
			}
			this.LoopType = loopType;
			this.LoopCondition = loopCondition;
			if (this.LoopType != Telerik.JustDecompiler.Decompiler.LogicFlow.Loops.LoopType.InfiniteLoop)
			{
				loopBody.Remove(this.LoopCondition);
			}
			this.DetermineLoopBodyBlock(entry, loopBody);
			base.RedirectChildrenToNewParent(this.GetLoopChildrenCollection());
			this.FixLoopCondition(typeSystem);
		}

		private void DetermineLoopBodyBlock(ILogicalConstruct entry, HashSet<ILogicalConstruct> loopBodyNodes)
		{
			this.LoopBodyBlock = null;
			if (loopBodyNodes.Count > 0)
			{
				if (this.LoopType != Telerik.JustDecompiler.Decompiler.LogicFlow.Loops.LoopType.PreTestedLoop)
				{
					if (!loopBodyNodes.Contains(entry))
					{
						throw new Exception("Invalid entry of loop body.");
					}
					this.LoopBodyBlock = new BlockLogicalConstruct(entry, loopBodyNodes);
					return;
				}
				ILogicalConstruct trueSuccessor = this.LoopCondition.TrueSuccessor;
				if (trueSuccessor == null || !loopBodyNodes.Contains(trueSuccessor))
				{
					trueSuccessor = this.LoopCondition.FalseSuccessor;
				}
				if (trueSuccessor == null || !loopBodyNodes.Contains(trueSuccessor))
				{
					throw new Exception("Invalid entry of loop body.");
				}
				this.LoopBodyBlock = new BlockLogicalConstruct(trueSuccessor, loopBodyNodes);
			}
		}

		private void FixLoopCondition(TypeSystem typeSystem)
		{
			if (this.LoopType == Telerik.JustDecompiler.Decompiler.LogicFlow.Loops.LoopType.InfiniteLoop)
			{
				return;
			}
			if (this.LoopCondition.TrueSuccessor == null)
			{
				this.LoopCondition.Negate(typeSystem);
			}
			if (this.LoopCondition.TrueSuccessor == null)
			{
				throw new ArgumentException("The loop condition must have a true successor inside of the loop");
			}
		}

		private ICollection<ILogicalConstruct> GetLoopChildrenCollection()
		{
			List<ILogicalConstruct> logicalConstructs = new List<ILogicalConstruct>();
			if (this.LoopBodyBlock != null)
			{
				logicalConstructs.Add(this.LoopBodyBlock);
			}
			if (this.LoopCondition != null)
			{
				logicalConstructs.Add(this.LoopCondition);
			}
			return logicalConstructs;
		}

		protected override string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedBlocks, LogicalFlowBuilderContext context)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.LoopType);
			stringBuilder.AppendLine("LogicalConstruct");
			stringBuilder.AppendLine("{");
			ILogicalConstruct[] sortedArrayFromCollection = base.GetSortedArrayFromCollection<ISingleEntrySubGraph>(this.Children);
			for (int i = 0; i < (int)sortedArrayFromCollection.Length; i++)
			{
				LogicalConstructBase logicalConstructBase = (LogicalConstructBase)sortedArrayFromCollection[i];
				string[] strArray = logicalConstructBase.ToString(context).Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				for (int j = 0; j < (int)strArray.Length; j++)
				{
					string str = strArray[j];
					stringBuilder.Append('\t');
					stringBuilder.AppendLine(str);
				}
				printedBlocks.UnionWith(logicalConstructBase.CFGBlocks);
			}
			string str1 = String.Format("\tFollowNode: {0}", base.NodeILOffset(context, base.CFGFollowNode));
			stringBuilder.AppendLine(str1);
			stringBuilder.AppendLine("}");
			return stringBuilder.ToString();
		}
	}
}