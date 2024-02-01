using Piranha.Extend;
using System;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="String", Shorthand="String", Component="string-field")]
	public class StringField : SimpleField<string>, ISearchable
	{
		public StringField()
		{
		}

		public string GetIndexedContent()
		{
			if (String.IsNullOrEmpty(base.Value))
			{
				return "";
			}
			return base.Value;
		}

		public static implicit operator StringField(string str)
		{
			return new StringField()
			{
				Value = str
			};
		}

		public static implicit operator String(StringField field)
		{
			return field.Value;
		}
	}
}