using Piranha.Extend;
using Piranha.Extend.Fields;
using System;
using System.Runtime.CompilerServices;

namespace Piranha.Extend.Blocks
{
	[BlockType(Name="Page link", Category="Content", Icon="fas fa-link", Component="page-block")]
	public class PageBlock : Block
	{
		public PageField Body
		{
			get;
			set;
		}

		public PageBlock()
		{
			base();
			return;
		}

		public override string GetTitle()
		{
			if (!PageField.op_Inequality(this.get_Body(), null) || this.get_Body().get_Page() == null)
			{
				return "No page selected";
			}
			return this.get_Body().get_Page().get_Title();
		}
	}
}