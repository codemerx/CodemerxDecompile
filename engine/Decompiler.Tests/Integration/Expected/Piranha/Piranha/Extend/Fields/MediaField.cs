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
			base();
			return;
		}

		public static implicit operator MediaField(Guid guid)
		{
			stackVariable0 = new MediaField();
			stackVariable0.set_Id(new Guid?(guid));
			return stackVariable0;
		}

		public static implicit operator MediaField(Piranha.Models.Media media)
		{
			stackVariable0 = new MediaField();
			stackVariable0.set_Id(new Guid?(media.get_Id()));
			return stackVariable0;
		}

		public static implicit operator String(MediaField image)
		{
			if (image.get_Media() == null)
			{
				return "";
			}
			return image.get_Media().get_PublicUrl();
		}
	}
}