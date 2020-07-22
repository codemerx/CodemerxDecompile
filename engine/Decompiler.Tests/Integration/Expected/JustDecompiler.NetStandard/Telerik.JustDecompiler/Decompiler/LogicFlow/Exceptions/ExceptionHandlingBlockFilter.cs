using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions
{
	internal class ExceptionHandlingBlockFilter : LogicalConstructBase, IFilteringExceptionHandler, ILogicalConstruct, ISingleEntrySubGraph, IComparable<ISingleEntrySubGraph>
	{
		public override ISingleEntrySubGraph Entry
		{
			get
			{
				return this.get_Filter();
			}
		}

		public BlockLogicalConstruct Filter
		{
			get;
			private set;
		}

		public BlockLogicalConstruct Handler
		{
			get;
			private set;
		}

		public FilteringExceptionHandlerType HandlerType
		{
			get
			{
				return 1;
			}
		}

		public ExceptionHandlingBlockFilter(BlockLogicalConstruct filter, BlockLogicalConstruct handler)
		{
			base();
			this.set_Filter(filter);
			this.set_Handler(handler);
			stackVariable7 = new BlockLogicalConstruct[2];
			stackVariable7[0] = filter;
			stackVariable7[1] = handler;
			this.RedirectChildrenToNewParent((IEnumerable<ILogicalConstruct>)stackVariable7);
			return;
		}
	}
}