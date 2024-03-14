using System;
using System.Collections.Generic;
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
				return this.Filter;
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
				return FilteringExceptionHandlerType.Filter;
			}
		}

		public ExceptionHandlingBlockFilter(BlockLogicalConstruct filter, BlockLogicalConstruct handler)
		{
			this.Filter = filter;
			this.Handler = handler;
			base.RedirectChildrenToNewParent((IEnumerable<ILogicalConstruct>)(new BlockLogicalConstruct[] { filter, handler }));
		}
	}
}