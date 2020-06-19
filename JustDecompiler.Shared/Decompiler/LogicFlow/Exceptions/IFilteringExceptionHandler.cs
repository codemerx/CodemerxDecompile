namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions
{
	interface IFilteringExceptionHandler : ILogicalConstruct
	{
		FilteringExceptionHandlerType HandlerType { get; }
	}

	enum FilteringExceptionHandlerType
	{
		Catch,
		Filter
	}
}
