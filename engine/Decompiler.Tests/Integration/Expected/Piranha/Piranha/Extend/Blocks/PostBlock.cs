using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
using System;
using System.Runtime.CompilerServices;

namespace Piranha.Extend.Blocks
{
	[BlockType(Name="Post link", Category="Content", Icon="fas fa-link", Component="post-block")]
	public class PostBlock : Block
	{
		public PostField Body
		{
			get;
			set;
		}

		public PostBlock()
		{
		}

		public override string GetTitle()
		{
			if (!(this.Body != null) || this.Body.Post == null)
			{
				return "No post selected";
			}
			return this.Body.Post.Title;
		}
	}
}