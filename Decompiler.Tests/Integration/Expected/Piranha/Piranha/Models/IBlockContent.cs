using Piranha.Extend;
using System;
using System.Collections.Generic;

namespace Piranha.Models
{
	public interface IBlockContent
	{
		IList<Block> Blocks
		{
			get;
			set;
		}
	}
}