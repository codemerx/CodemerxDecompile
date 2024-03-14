using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	public class BlockLogicalConstruct : LogicalConstructBase
	{
		protected BlockLogicalConstruct()
		{
		}

		public BlockLogicalConstruct(ILogicalConstruct Entry, IEnumerable<ILogicalConstruct> body)
		{
			this.Entry = Entry;
			base.RedirectChildrenToNewParent(body);
		}
	}
}