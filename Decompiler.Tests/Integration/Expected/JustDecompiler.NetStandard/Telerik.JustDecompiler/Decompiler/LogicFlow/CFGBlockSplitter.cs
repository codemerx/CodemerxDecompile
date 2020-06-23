using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	internal class CFGBlockSplitter
	{
		private readonly LogicalFlowBuilderContext logicalContext;

		public CFGBlockSplitter(LogicalFlowBuilderContext logicalContext)
		{
			this.logicalContext = logicalContext;
		}

		public void SplitConditionalCFGBlocks()
		{
			foreach (KeyValuePair<InstructionBlock, CFGBlockLogicalConstruct[]> keyValuePair in new List<KeyValuePair<InstructionBlock, CFGBlockLogicalConstruct[]>>(this.logicalContext.CFGBlockToLogicalConstructMap))
			{
				if ((int)keyValuePair.Key.Successors.Length <= 1)
				{
					continue;
				}
				CFGBlockLogicalConstruct value = keyValuePair.Value[(int)keyValuePair.Value.Length - 1];
				if (value.LogicalConstructExpressions.Count <= 1)
				{
					continue;
				}
				LogicalFlowUtilities.SplitCFGBlockAt(this.logicalContext, value, value.LogicalConstructExpressions.Count - 1);
			}
		}
	}
}