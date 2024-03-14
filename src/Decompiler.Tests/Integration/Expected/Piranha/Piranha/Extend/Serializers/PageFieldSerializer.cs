using Piranha.Extend;
using Piranha.Extend.Fields;
using System;

namespace Piranha.Extend.Serializers
{
	public class PageFieldSerializer : ISerializer
	{
		public PageFieldSerializer()
		{
		}

		public object Deserialize(string str)
		{
			Guid? nullable;
			PageField pageField = new PageField();
			if (!String.IsNullOrEmpty(str))
			{
				nullable = new Guid?(new Guid(str));
			}
			else
			{
				nullable = null;
			}
			pageField.Id = nullable;
			return pageField;
		}

		public string Serialize(object obj)
		{
			PageField pageField = obj as PageField;
			if (pageField == null)
			{
				throw new ArgumentException("The given object doesn't match the serialization type");
			}
			return pageField.Id.ToString();
		}
	}
}