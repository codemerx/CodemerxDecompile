using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions
{
	internal abstract class ExceptionHandlingLogicalConstruct : LogicalConstructBase
	{
		public override ISingleEntrySubGraph Entry
		{
			get
			{
				return this.Try;
			}
		}

		public BlockLogicalConstruct Try
		{
			get;
			private set;
		}

		protected ExceptionHandlingLogicalConstruct()
		{
		}

		protected ExceptionHandlingLogicalConstruct(BlockLogicalConstruct @try)
		{
			this.InitiExceptionHandlingLogicalConstruct(@try);
		}

		protected void InitiExceptionHandlingLogicalConstruct(BlockLogicalConstruct @try)
		{
			this.Try = @try;
			base.RedirectChildrenToNewParent((IEnumerable<ILogicalConstruct>)(new BlockLogicalConstruct[] { @try }));
		}
	}
}