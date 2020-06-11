using System;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DTree;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
    abstract class DominatorTreeDependentStep
    {
        protected readonly LogicalFlowBuilderContext logicalContext;

        protected DominatorTreeDependentStep(LogicalFlowBuilderContext context)
        {
            this.logicalContext = context;
        }

        protected DominatorTree GetDominatorTreeFromContext(ILogicalConstruct construct)
        {
            DominatorTree result;
            if (!logicalContext.LogicalConstructToDominatorTreeMap.TryGetValue(construct, out result))
            {
                result = FastDominatorTreeBuilder.BuildTree(construct);
                logicalContext.LogicalConstructToDominatorTreeMap.Add(construct, result);
            }

            return result;
        }
    }
}
