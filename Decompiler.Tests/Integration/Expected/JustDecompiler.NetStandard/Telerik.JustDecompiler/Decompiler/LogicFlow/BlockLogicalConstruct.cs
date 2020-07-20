using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	public class BlockLogicalConstruct : LogicalConstructBase
	{
		protected BlockLogicalConstruct()
		{
			base();
			return;
		}

		public BlockLogicalConstruct(ILogicalConstruct Entry, IEnumerable<ILogicalConstruct> body)
		{
			base();
			this.set_Entry(Entry);
			this.RedirectChildrenToNewParent(body);
			return;
		}
	}
}