using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions
{
	internal abstract class ExceptionHandlingLogicalConstruct : LogicalConstructBase
	{
		protected ExceptionHandlingLogicalConstruct()
		{}

		protected ExceptionHandlingLogicalConstruct(BlockLogicalConstruct @try)
		{
			InitiExceptionHandlingLogicalConstruct(@try);
		}

		protected void InitiExceptionHandlingLogicalConstruct(BlockLogicalConstruct @try)
		{
			Try = @try;
			RedirectChildrenToNewParent(new BlockLogicalConstruct[] { @try });
		}

		public BlockLogicalConstruct Try { get; private set; }

        /// <summary>
        /// The entry child of the construct.
        /// </summary>
		public override ISingleEntrySubGraph Entry
		{
			get
			{
				return Try;
			}
		}
	}
}
