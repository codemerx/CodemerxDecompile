using System;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions
{
	class TryFinallyLogicalConstruct : ExceptionHandlingLogicalConstruct
	{
		public TryFinallyLogicalConstruct(BlockLogicalConstruct @try, BlockLogicalConstruct @finally)
		{
            InitiExceptionHandlingLogicalConstruct(@try);
			Finally = @finally;
			RedirectChildrenToNewParent(new BlockLogicalConstruct[] { Finally });
		}

		public BlockLogicalConstruct Finally { get; private set; }
	}
}
