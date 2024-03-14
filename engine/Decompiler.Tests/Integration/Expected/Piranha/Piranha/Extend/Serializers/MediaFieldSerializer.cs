using Piranha.Extend;
using Piranha.Extend.Fields;
using System;

namespace Piranha.Extend.Serializers
{
	public class MediaFieldSerializer : ISerializer
	{
		public MediaFieldSerializer()
		{
		}

		public object Deserialize(string str)
		{
			Guid? nullable;
			MediaField mediaField = new MediaField();
			if (!String.IsNullOrEmpty(str))
			{
				nullable = new Guid?(new Guid(str));
			}
			else
			{
				nullable = null;
			}
			mediaField.Id = nullable;
			return mediaField;
		}

		public string Serialize(object obj)
		{
			MediaField mediaField = obj as MediaField;
			if (mediaField == null)
			{
				throw new ArgumentException("The given object doesn't match the serialization type");
			}
			return mediaField.Id.ToString();
		}
	}
}