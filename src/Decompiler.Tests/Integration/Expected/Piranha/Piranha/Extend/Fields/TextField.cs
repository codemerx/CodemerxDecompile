using Piranha.Extend;
using System;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Text", Shorthand="Text", Component="text-field")]
	public class TextField : SimpleField<string>, ISearchable
	{
		public TextField()
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

		public static implicit operator TextField(string str)
		{
			return new TextField()
			{
				Value = str
			};
		}

		public static implicit operator String(TextField field)
		{
			return field.Value;
		}
	}
}