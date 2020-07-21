using Piranha.Extend;
using Piranha.Extend.Fields;
using System;
using System.Runtime.CompilerServices;

namespace Piranha.Extend.Blocks
{
	[BlockType(Name="Video", Category="Media", Icon="fas fa-video", Component="video-block")]
	public class VideoBlock : Block
	{
		public VideoField Body
		{
			get;
			set;
		}

		public VideoBlock()
		{
			base();
			return;
		}

		public override string GetTitle()
		{
			if (!MediaFieldBase<VideoField>.op_Inequality(this.get_Body(), null) || this.get_Body().get_Media() == null)
			{
				return "No video selected";
			}
			return this.get_Body().get_Media().get_Filename();
		}
	}
}