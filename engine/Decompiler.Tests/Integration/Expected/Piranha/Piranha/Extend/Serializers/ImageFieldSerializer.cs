using Piranha.Extend;
using Piranha.Extend.Fields;
using System;

namespace Piranha.Extend.Serializers
{
	public class ImageFieldSerializer : ISerializer
	{
		public ImageFieldSerializer()
		{
		}

		public object Deserialize(string str)
		{
			Guid? nullable;
			ImageField imageField = new ImageField();
			if (!String.IsNullOrEmpty(str))
			{
				nullable = new Guid?(new Guid(str));
			}
			else
			{
				nullable = null;
			}
			imageField.Id = nullable;
			return imageField;
		}

		public string Serialize(object obj)
		{
			ImageField imageField = obj as ImageField;
			if (imageField == null)
			{
				throw new ArgumentException("The given object doesn't match the serialization type");
			}
			return imageField.Id.ToString();
		}
	}
}