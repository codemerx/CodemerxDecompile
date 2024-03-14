using Piranha.Extend;
using System;

namespace Piranha.Extend.Blocks
{
	[BlockGroupType(Name="Gallery", Category="Media", Icon="fas fa-images")]
	[BlockItemType(Type=typeof(ImageBlock))]
	public class ImageGalleryBlock : BlockGroup
	{
		public ImageGalleryBlock()
		{
		}
	}
}