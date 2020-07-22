using Piranha.Extend;
using Piranha.Extend.Fields;
using System;
using System.Runtime.CompilerServices;

namespace Piranha.Extend.Blocks
{
	[BlockType(Name="Image", Category="Media", Icon="fas fa-image", Component="image-block")]
	public class ImageBlock : Block
	{
		public SelectField<ImageAspect> Aspect
		{
			get;
			set;
		}

		public ImageField Body
		{
			get;
			set;
		}

		public ImageBlock()
		{
			this.u003cAspectu003ek__BackingField = new SelectField<ImageAspect>();
			base();
			return;
		}

		public override string GetTitle()
		{
			if (!MediaFieldBase<ImageField>.op_Inequality(this.get_Body(), null) || this.get_Body().get_Media() == null)
			{
				return "No image selected";
			}
			return this.get_Body().get_Media().get_Filename();
		}
	}
}