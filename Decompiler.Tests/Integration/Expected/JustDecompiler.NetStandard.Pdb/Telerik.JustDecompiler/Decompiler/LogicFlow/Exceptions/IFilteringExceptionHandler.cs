using System;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions
{
	internal interface IFilteringExceptionHandler : ILogicalConstruct, ISingleEntrySubGraph, IComparable<ISingleEntrySubGraph>
	{
		FilteringExceptionHandlerType HandlerType
		{
			get;
		}
	}
}