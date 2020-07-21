using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	internal class CFGBlockSplitter
	{
		private readonly LogicalFlowBuilderContext logicalContext;

		public CFGBlockSplitter(LogicalFlowBuilderContext logicalContext)
		{
			base();
			this.logicalContext = logicalContext;
			return;
		}

		public void SplitConditionalCFGBlocks()
		{
			V_0 = (new List<KeyValuePair<InstructionBlock, CFGBlockLogicalConstruct[]>>(this.logicalContext.get_CFGBlockToLogicalConstructMap())).GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if ((int)V_1.get_Key().get_Successors().Length <= 1)
					{
						continue;
					}
					V_2 = V_1.get_Value()[(int)V_1.get_Value().Length - 1];
					if (V_2.get_LogicalConstructExpressions().get_Count() <= 1)
					{
						continue;
					}
					dummyVar0 = LogicalFlowUtilities.SplitCFGBlockAt(this.logicalContext, V_2, V_2.get_LogicalConstructExpressions().get_Count() - 1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}
	}
}