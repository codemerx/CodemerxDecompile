using Piranha.Extend;
using Piranha.Extend.Fields;
using System;
using System.Runtime.CompilerServices;

namespace Piranha.Extend.Blocks
{
	[BlockType(Name="Audio", Category="Media", Icon="fas fa-headphones", Component="audio-block")]
	public class AudioBlock : Block
	{
		public AudioField Body
		{
			get;
			set;
		}

		public AudioBlock()
		{
			base();
			return;
		}

		public override string GetTitle()
		{
			if (!MediaFieldBase<AudioField>.op_Inequality(this.get_Body(), null) || this.get_Body().get_Media() == null)
			{
				return "No audio selected";
			}
			return this.get_Body().get_Media().get_Filename();
		}
	}
}