using Piranha.Extend;
using Piranha.Extend.Fields;
using System;

namespace Piranha.Extend.Serializers
{
	public class VideoFieldSerializer : ISerializer
	{
		public VideoFieldSerializer()
		{
		}

		public object Deserialize(string str)
		{
			Guid? nullable;
			VideoField videoField = new VideoField();
			if (!String.IsNullOrEmpty(str))
			{
				nullable = new Guid?(new Guid(str));
			}
			else
			{
				nullable = null;
			}
			videoField.Id = nullable;
			return videoField;
		}

		public string Serialize(object obj)
		{
			VideoField videoField = obj as VideoField;
			if (videoField == null)
			{
				throw new ArgumentException("The given object doesn't match the serialization type");
			}
			return videoField.Id.ToString();
		}
	}
}