using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
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
		}

		public override string GetTitle()
		{
			if (!(this.Body != null) || this.Body.Media == null)
			{
				return "No video selected";
			}
			return this.Body.Media.Filename;
		}
	}
}