using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Decompiler.LogicFlow;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions
{
	internal class TryFinallyLogicalConstruct : ExceptionHandlingLogicalConstruct
	{
		public BlockLogicalConstruct Finally
		{
			get;
			private set;
		}

		public TryFinallyLogicalConstruct(BlockLogicalConstruct try, BlockLogicalConstruct finally)
		{
			base();
			this.InitiExceptionHandlingLogicalConstruct(try);
			this.set_Finally(finally);
			stackVariable7 = new BlockLogicalConstruct[1];
			stackVariable7[0] = this.get_Finally();
			this.RedirectChildrenToNewParent((IEnumerable<ILogicalConstruct>)stackVariable7);
			return;
		}
	}
}