using Piranha.Extend;
using Piranha.Extend.Fields;
using System;

namespace Piranha.Extend.Serializers
{
	public class DateFieldSerializer : ISerializer
	{
		public DateFieldSerializer()
		{
		}

		public object Deserialize(string str)
		{
			DateField dateField = new DateField();
			if (!String.IsNullOrWhiteSpace(str))
			{
				try
				{
					dateField.Value = new DateTime?(DateTime.Parse(str));
				}
				catch
				{
					dateField.Value = null;
				}
			}
			return dateField;
		}

		public string Serialize(object obj)
		{
			DateField dateField = obj as DateField;
			if (dateField == null)
			{
				throw new ArgumentException("The given object doesn't match the serialization type");
			}
			if (!dateField.Value.HasValue)
			{
				return null;
			}
			return dateField.Value.Value.ToString("yyyy-MM-dd");
		}
	}
}