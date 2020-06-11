using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions
{
	class ExceptionHandlingBlockFilter : LogicalConstructBase, IFilteringExceptionHandler
	{
		public ExceptionHandlingBlockFilter(BlockLogicalConstruct filter, BlockLogicalConstruct handler)
		{
			Filter = filter;
			Handler = handler;

			RedirectChildrenToNewParent(new BlockLogicalConstruct[] { filter, handler });
		}

		public BlockLogicalConstruct Filter { get; private set; }
		public BlockLogicalConstruct Handler { get; private set; }

		public FilteringExceptionHandlerType HandlerType
		{
			get
			{
				return FilteringExceptionHandlerType.Filter;
			}
		}

        /// <summary>
        /// The entry child of the construct.
        /// </summary>
		public override ISingleEntrySubGraph Entry
		{
			get
			{
				return Filter;
			}
		}
	}
}
