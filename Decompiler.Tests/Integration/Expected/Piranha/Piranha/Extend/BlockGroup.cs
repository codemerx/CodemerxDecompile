using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Piranha.Extend
{
	public abstract class BlockGroup : Block
	{
		public IList<Block> Items
		{
			get;
			set;
		}

		protected BlockGroup()
		{
			this.u003cItemsu003ek__BackingField = new List<Block>();
			base();
			return;
		}
	}
}