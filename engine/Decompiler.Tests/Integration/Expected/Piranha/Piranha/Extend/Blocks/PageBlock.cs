using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
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
		}

		public override string GetTitle()
		{
			if (!(this.Body != null) || this.Body.Page == null)
			{
				return "No page selected";
			}
			return this.Body.Page.Title;
		}
	}
}