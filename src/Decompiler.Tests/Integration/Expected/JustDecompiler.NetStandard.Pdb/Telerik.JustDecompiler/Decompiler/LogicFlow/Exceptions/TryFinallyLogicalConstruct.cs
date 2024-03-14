using System;
using System.Collections.Generic;
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

		public TryFinallyLogicalConstruct(BlockLogicalConstruct @try, BlockLogicalConstruct @finally)
		{
			base.InitiExceptionHandlingLogicalConstruct(@try);
			this.Finally = @finally;
			base.RedirectChildrenToNewParent((IEnumerable<ILogicalConstruct>)(new BlockLogicalConstruct[] { this.Finally }));
		}
	}
}