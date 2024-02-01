using Piranha.Extend;
using Piranha.Models;
using System;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Media", Shorthand="Media", Component="media-field")]
	public class MediaField : MediaFieldBase<MediaField>
	{
		public MediaField()
		{
		}

		public static implicit operator MediaField(Guid guid)
		{
			return new MediaField()
			{
				Id = new Guid?(guid)
			};
		}

		public static implicit operator MediaField(Piranha.Models.Media media)
		{
			return new MediaField()
			{
				Id = new Guid?(media.Id)
			};
		}

		public static implicit operator String(MediaField image)
		{
			if (image.Media == null)
			{
				return "";
			}
			return image.Media.PublicUrl;
		}
	}
}