using System;
using System.Collections.Generic;
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

		public TryCatchFilterLogicalConstruct(BlockLogicalConstruct try, IFilteringExceptionHandler handler)
		{
			base();
			this.InitiExceptionHandlingLogicalConstruct(try);
			this.handlers = new List<IFilteringExceptionHandler>();
			this.AddHandler(handler);
			return;
		}

		public void AddHandler(IFilteringExceptionHandler handler)
		{
			V_0 = handler.get_HandlerType();
			if (V_0 == FilteringExceptionHandlerType.Catch)
			{
				if (((ExceptionHandlingBlockCatch)handler).get_Parent() != this.get_Parent())
				{
					throw new Exception("Catch handler belongs to different logical construct than the try block");
				}
			}
			else
			{
				if (V_0 == 1)
				{
					if (((ExceptionHandlingBlockFilter)handler).get_Parent() != this.get_Parent())
					{
						throw new Exception("Filter handler belongs to different logical construct than the try block");
					}
				}
			}
			stackVariable10 = new IFilteringExceptionHandler[1];
			stackVariable10[0] = handler;
			this.RedirectChildrenToNewParent((IEnumerable<ILogicalConstruct>)stackVariable10);
			this.handlers.Add(handler);
			this.FixSuccessors(handler);
			return;
		}

		private void FixSuccessors(IFilteringExceptionHandler handler)
		{
			if (handler.get_CFGSuccessors().Contains(this.get_Try().get_FirstBlock()))
			{
				this.AddToSuccessors(this.get_Try().get_FirstBlock());
			}
			V_0 = handler.get_CFGBlocks().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!V_1.get_CFGSuccessors().Contains(this.get_Try().get_FirstBlock()))
					{
						continue;
					}
					this.AddToPredecessors(V_1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}
	}
}