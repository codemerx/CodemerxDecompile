using Piranha.Extend;
using Piranha.Extend.Fields;
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
			base();
			return;
		}

		public override string GetTitle()
		{
			if (!PostField.op_Inequality(this.get_Body(), null) || this.get_Body().get_Post() == null)
			{
				return "No post selected";
			}
			return this.get_Body().get_Post().get_Title();
		}
	}
}