using System;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions
{
	class TryFaultLogicalConstruct : ExceptionHandlingLogicalConstruct
	{
		public TryFaultLogicalConstruct(BlockLogicalConstruct @try, BlockLogicalConstruct fault) 
		{
			InitiExceptionHandlingLogicalConstruct(@try);

			RedirectChildrenToNewParent(new BlockLogicalConstruct[] { fault });
			Fault = fault;
		}

		public BlockLogicalConstruct Fault { get; private set; }
	}
}
