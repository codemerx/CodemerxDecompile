using Piranha;
using Piranha.Extend;
using Piranha.Models;
using System;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Image", Shorthand="Image", Component="image-field")]
	public class ImageField : MediaFieldBase<ImageField>
	{
		public ImageField()
		{
			base();
			return;
		}

		public static implicit operator ImageField(Guid guid)
		{
			stackVariable0 = new ImageField();
			stackVariable0.set_Id(new Guid?(guid));
			return stackVariable0;
		}

		public static implicit operator ImageField(Piranha.Models.Media media)
		{
			stackVariable0 = new ImageField();
			stackVariable0.set_Id(new Guid?(media.get_Id()));
			return stackVariable0;
		}

		public static implicit operator String(ImageField image)
		{
			if (image.get_Media() == null)
			{
				return "";
			}
			return image.get_Media().get_PublicUrl();
		}

		public string Resize(IApi api, int width, int? height = null)
		{
			if (!this.get_Id().get_HasValue())
			{
				return null;
			}
			stackVariable6 = api.get_Media();
			V_0 = this.get_Id();
			return stackVariable6.EnsureVersion(V_0.get_Value(), width, height);
		}
	}
}