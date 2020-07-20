using System;
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
				return this.get_Try();
			}
		}

		public BlockLogicalConstruct Try
		{
			get;
			private set;
		}

		protected ExceptionHandlingLogicalConstruct()
		{
			base();
			return;
		}

		protected ExceptionHandlingLogicalConstruct(BlockLogicalConstruct try)
		{
			base();
			this.InitiExceptionHandlingLogicalConstruct(try);
			return;
		}

		protected void InitiExceptionHandlingLogicalConstruct(BlockLogicalConstruct try)
		{
			this.set_Try(try);
			stackVariable4 = new BlockLogicalConstruct[1];
			stackVariable4[0] = try;
			this.RedirectChildrenToNewParent((IEnumerable<ILogicalConstruct>)stackVariable4);
			return;
		}
	}
}