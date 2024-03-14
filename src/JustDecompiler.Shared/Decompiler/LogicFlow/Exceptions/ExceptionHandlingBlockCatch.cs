using System.Collections.Generic;
using Mono.Cecil;


namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions
{
	class ExceptionHandlingBlockCatch : BlockLogicalConstruct, IFilteringExceptionHandler
	{
		public ExceptionHandlingBlockCatch(HashSet<ILogicalConstruct> Children, ILogicalConstruct Entry, TypeReference catchType)
			: base(Entry, Children)
		{
			CatchType = catchType;
		}

		/// <summary>
		/// The type of exception that triggers the catch clause. Null for catch all.
		/// </summary>
		public TypeReference CatchType { get; private set; }

		public FilteringExceptionHandlerType HandlerType
		{
			get
			{
				return FilteringExceptionHandlerType.Catch;
			}
		}
	}
}
