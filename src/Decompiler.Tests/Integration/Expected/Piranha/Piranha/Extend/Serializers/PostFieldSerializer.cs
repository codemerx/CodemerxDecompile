using Piranha.Extend;
using Piranha.Extend.Fields;
using System;

namespace Piranha.Extend.Serializers
{
	public class PostFieldSerializer : ISerializer
	{
		public PostFieldSerializer()
		{
		}

		public object Deserialize(string str)
		{
			Guid? nullable;
			PostField postField = new PostField();
			if (!String.IsNullOrEmpty(str))
			{
				nullable = new Guid?(new Guid(str));
			}
			else
			{
				nullable = null;
			}
			postField.Id = nullable;
			return postField;
		}

		public string Serialize(object obj)
		{
			PostField postField = obj as PostField;
			if (postField == null)
			{
				throw new ArgumentException("The given object doesn't match the serialization type");
			}
			return postField.Id.ToString();
		}
	}
}