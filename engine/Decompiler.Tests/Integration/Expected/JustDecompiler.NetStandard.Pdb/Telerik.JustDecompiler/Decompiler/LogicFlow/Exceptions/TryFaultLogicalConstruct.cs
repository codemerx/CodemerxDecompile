using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Decompiler.LogicFlow;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions
{
	internal class TryFaultLogicalConstruct : ExceptionHandlingLogicalConstruct
	{
		public BlockLogicalConstruct Fault
		{
			get;
			private set;
		}

		public TryFaultLogicalConstruct(BlockLogicalConstruct @try, BlockLogicalConstruct fault)
		{
			base.InitiExceptionHandlingLogicalConstruct(@try);
			base.RedirectChildrenToNewParent((IEnumerable<ILogicalConstruct>)(new BlockLogicalConstruct[] { fault }));
			this.Fault = fault;
		}
	}
}