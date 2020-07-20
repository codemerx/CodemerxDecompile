using System;
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

		public TryFaultLogicalConstruct(BlockLogicalConstruct try, BlockLogicalConstruct fault)
		{
			base();
			this.InitiExceptionHandlingLogicalConstruct(try);
			stackVariable5 = new BlockLogicalConstruct[1];
			stackVariable5[0] = fault;
			this.RedirectChildrenToNewParent((IEnumerable<ILogicalConstruct>)stackVariable5);
			this.set_Fault(fault);
			return;
		}
	}
}