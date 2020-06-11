using System;
using System.Collections.Generic;
using System.Linq;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions
{
	internal class TryCatchFilterLogicalConstruct : ExceptionHandlingLogicalConstruct
	{
        private readonly List<IFilteringExceptionHandler> handlers;

		public TryCatchFilterLogicalConstruct(BlockLogicalConstruct @try, IFilteringExceptionHandler handler)
		{
            InitiExceptionHandlingLogicalConstruct(@try);
			handlers = new List<IFilteringExceptionHandler>();
			AddHandler(handler);
		}

		public IFilteringExceptionHandler[] Handlers 
		{ 
			get
			{
				return handlers.ToArray<IFilteringExceptionHandler>();
			}

		}

		public void AddHandler(IFilteringExceptionHandler handler)
		{
			//sanity check
			switch (handler.HandlerType)
			{
				case FilteringExceptionHandlerType.Catch :
					ExceptionHandlingBlockCatch catchHandler = (ExceptionHandlingBlockCatch)handler;
					if(catchHandler.Parent != this.Parent)
					{
						throw new Exception("Catch handler belongs to different logical construct than the try block");
					}
					break;
				case FilteringExceptionHandlerType.Filter :
					ExceptionHandlingBlockFilter filterHandler = (ExceptionHandlingBlockFilter)handler;
					if (filterHandler.Parent != this.Parent)
					{
						throw new Exception("Filter handler belongs to different logical construct than the try block");
					}
					break;
			}

			RedirectChildrenToNewParent(new IFilteringExceptionHandler[] { handler });
			handlers.Add(handler);
            FixSuccessors(handler);
		}

        /// <summary>
        /// Covers the case when the handler has a jump to the guarded block.
        /// </summary>
        /// <param name="handler">The exception handling block.</param>
        private void FixSuccessors(IFilteringExceptionHandler handler)
        {
            /// The handler forms a loop with the try.
            if(handler.CFGSuccessors.Contains(this.Try.FirstBlock))
            {
                this.AddToSuccessors(this.Try.FirstBlock);
            }

            /// Find which CFG block forms the loop and add it to the predecessors collection.
            foreach (CFGBlockLogicalConstruct cfgChild in handler.CFGBlocks)
            {
                if (cfgChild.CFGSuccessors.Contains(this.Try.FirstBlock))
                {
                    this.AddToPredecessors(cfgChild);
                }
            }
        }
	}
}
