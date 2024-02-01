using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions
{
	internal class ExceptionHandlingBlockCatch : BlockLogicalConstruct, IFilteringExceptionHandler, ILogicalConstruct, ISingleEntrySubGraph, IComparable<ISingleEntrySubGraph>
	{
		public TypeReference CatchType
		{
			get;
			private set;
		}

		public FilteringExceptionHandlerType HandlerType
		{
			get
			{
				return FilteringExceptionHandlerType.Catch;
			}
		}

		public ExceptionHandlingBlockCatch(HashSet<ILogicalConstruct> Children, ILogicalConstruct Entry, TypeReference catchType) : base(Entry, Children)
		{
			this.CatchType = catchType;
		}
	}
}