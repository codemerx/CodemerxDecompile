using System;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DTree;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	internal abstract class DominatorTreeDependentStep
	{
		protected readonly LogicalFlowBuilderContext logicalContext;

		protected DominatorTreeDependentStep(LogicalFlowBuilderContext context)
		{
			base();
			this.logicalContext = context;
			return;
		}

		protected DominatorTree GetDominatorTreeFromContext(ILogicalConstruct construct)
		{
			if (!this.logicalContext.get_LogicalConstructToDominatorTreeMap().TryGetValue(construct, out V_0))
			{
				V_0 = FastDominatorTreeBuilder.BuildTree(construct);
				this.logicalContext.get_LogicalConstructToDominatorTreeMap().Add(construct, V_0);
			}
			return V_0;
		}
	}
}