using Piranha.Extend;
using System;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="String", Shorthand="String", Component="string-field")]
	public class StringField : SimpleField<string>, ISearchable
	{
		public StringField()
		{
			base();
			return;
		}

		public string GetIndexedContent()
		{
			if (String.IsNullOrEmpty(this.get_Value()))
			{
				return "";
			}
			return this.get_Value();
		}

		public static implicit operator StringField(string str)
		{
			stackVariable0 = new StringField();
			stackVariable0.set_Value(str);
			return stackVariable0;
		}

		public static implicit operator String(StringField field)
		{
			return field.get_Value();
		}
	}
}