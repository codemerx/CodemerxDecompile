using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
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
		}

		public override string GetTitle()
		{
			if (!(this.Body != null) || this.Body.Media == null)
			{
				return "No audio selected";
			}
			return this.Body.Media.Filename;
		}
	}
}