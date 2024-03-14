using Piranha.Extend;
using Piranha.Extend.Fields;
using System;

namespace Piranha.Extend.Serializers
{
	public class AudioFieldSerializer : ISerializer
	{
		public AudioFieldSerializer()
		{
		}

		public object Deserialize(string str)
		{
			Guid? nullable;
			AudioField audioField = new AudioField();
			if (!String.IsNullOrEmpty(str))
			{
				nullable = new Guid?(new Guid(str));
			}
			else
			{
				nullable = null;
			}
			audioField.Id = nullable;
			return audioField;
		}

		public string Serialize(object obj)
		{
			AudioField audioField = obj as AudioField;
			if (audioField == null)
			{
				throw new ArgumentException("The given object doesn't match the serialization type");
			}
			return audioField.Id.ToString();
		}
	}
}