using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Piranha.Extend
{
	public abstract class BlockGroup : Block
	{
		public IList<Block> Items { get; set; } = new List<Block>();

		protected BlockGroup()
		{
		}
	}
}