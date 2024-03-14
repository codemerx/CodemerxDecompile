using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustDecompiler.Decompiler.LogicFlow;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions
{
	internal class TryCatchFilterLogicalConstruct : ExceptionHandlingLogicalConstruct
	{
		private readonly List<IFilteringExceptionHandler> handlers;

		public IFilteringExceptionHandler[] Handlers
		{
			get
			{
				return this.handlers.ToArray<IFilteringExceptionHandler>();
			}
		}

		public TryCatchFilterLogicalConstruct(BlockLogicalConstruct @try, IFilteringExceptionHandler handler)
		{
			base.InitiExceptionHandlingLogicalConstruct(@try);
			this.handlers = new List<IFilteringExceptionHandler>();
			this.AddHandler(handler);
		}

		public void AddHandler(IFilteringExceptionHandler handler)
		{
			FilteringExceptionHandlerType handlerType = handler.HandlerType;
			if (handlerType != FilteringExceptionHandlerType.Catch)
			{
				if (handlerType == FilteringExceptionHandlerType.Filter)
				{
					if (((ExceptionHandlingBlockFilter)handler).Parent != base.Parent)
					{
						throw new Exception("Filter handler belongs to different logical construct than the try block");
					}
				}
			}
			else if (((ExceptionHandlingBlockCatch)handler).Parent != base.Parent)
			{
				throw new Exception("Catch handler belongs to different logical construct than the try block");
			}
			base.RedirectChildrenToNewParent((IEnumerable<ILogicalConstruct>)(new IFilteringExceptionHandler[] { handler }));
			this.handlers.Add(handler);
			this.FixSuccessors(handler);
		}

		private void FixSuccessors(IFilteringExceptionHandler handler)
		{
			if (handler.CFGSuccessors.Contains(base.Try.FirstBlock))
			{
				base.AddToSuccessors(base.Try.FirstBlock);
			}
			foreach (CFGBlockLogicalConstruct cFGBlock in handler.CFGBlocks)
			{
				if (!cFGBlock.CFGSuccessors.Contains(base.Try.FirstBlock))
				{
					continue;
				}
				base.AddToPredecessors(cFGBlock);
			}
		}
	}
}