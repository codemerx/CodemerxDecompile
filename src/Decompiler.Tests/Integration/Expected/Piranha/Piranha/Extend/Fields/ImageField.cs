using Piranha;
using Piranha.Extend;
using Piranha.Models;
using Piranha.Services;
using System;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Image", Shorthand="Image", Component="image-field")]
	public class ImageField : MediaFieldBase<ImageField>
	{
		public ImageField()
		{
		}

		public static implicit operator ImageField(Guid guid)
		{
			return new ImageField()
			{
				Id = new Guid?(guid)
			};
		}

		public static implicit operator ImageField(Piranha.Models.Media media)
		{
			return new ImageField()
			{
				Id = new Guid?(media.Id)
			};
		}

		public static implicit operator String(ImageField image)
		{
			if (image.Media == null)
			{
				return "";
			}
			return image.Media.PublicUrl;
		}

		public string Resize(IApi api, int width, int? height = null)
		{
			if (!base.Id.HasValue)
			{
				return null;
			}
			return api.Media.EnsureVersion(base.Id.Value, width, height);
		}
	}
}