using Piranha.Extend;
using System;

namespace Piranha.Extend.Blocks
{
	[BlockType(Name="Separator", Category="Content", Icon="fas fa-divide", Component="separator-block")]
	public class SeparatorBlock : Block
	{
		public SeparatorBlock()
		{
		}

		public override string GetTitle()
		{
			return "----";
		}
	}
}