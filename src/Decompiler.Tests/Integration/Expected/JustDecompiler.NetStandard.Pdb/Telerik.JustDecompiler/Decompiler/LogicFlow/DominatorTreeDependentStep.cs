using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DTree;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	internal abstract class DominatorTreeDependentStep
	{
		protected readonly LogicalFlowBuilderContext logicalContext;

		protected DominatorTreeDependentStep(LogicalFlowBuilderContext context)
		{
			this.logicalContext = context;
		}

		protected DominatorTree GetDominatorTreeFromContext(ILogicalConstruct construct)
		{
			DominatorTree dominatorTree;
			if (!this.logicalContext.LogicalConstructToDominatorTreeMap.TryGetValue(construct, out dominatorTree))
			{
				dominatorTree = FastDominatorTreeBuilder.BuildTree(construct);
				this.logicalContext.LogicalConstructToDominatorTreeMap.Add(construct, dominatorTree);
			}
			return dominatorTree;
		}
	}
}