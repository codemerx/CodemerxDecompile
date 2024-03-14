using Piranha.Extend;
using Piranha.Extend.Fields;
using System;

namespace Piranha.Extend.Serializers
{
	public class DocumentFieldSerializer : ISerializer
	{
		public DocumentFieldSerializer()
		{
		}

		public object Deserialize(string str)
		{
			Guid? nullable;
			DocumentField documentField = new DocumentField();
			if (!String.IsNullOrEmpty(str))
			{
				nullable = new Guid?(new Guid(str));
			}
			else
			{
				nullable = null;
			}
			documentField.Id = nullable;
			return documentField;
		}

		public string Serialize(object obj)
		{
			DocumentField documentField = obj as DocumentField;
			if (documentField == null)
			{
				throw new ArgumentException("The given object doesn't match the serialization type");
			}
			return documentField.Id.ToString();
		}
	}
}