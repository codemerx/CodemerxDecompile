using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
using System;
using System.Runtime.CompilerServices;

namespace Piranha.Extend.Blocks
{
	[BlockType(Name="Image", Category="Media", Icon="fas fa-image", Component="image-block")]
	public class ImageBlock : Block
	{
		public SelectField<ImageAspect> Aspect { get; set; } = new SelectField<ImageAspect>();

		public ImageField Body
		{
			get;
			set;
		}

		public ImageBlock()
		{
		}

		public override string GetTitle()
		{
			if (!(this.Body != null) || this.Body.Media == null)
			{
				return "No image selected";
			}
			return this.Body.Media.Filename;
		}
	}
}