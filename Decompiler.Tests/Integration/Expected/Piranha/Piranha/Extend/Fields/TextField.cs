using Piranha.Extend;
using System;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Text", Shorthand="Text", Component="text-field")]
	public class TextField : SimpleField<string>, ISearchable
	{
		public TextField()
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

		public static implicit operator TextField(string str)
		{
			stackVariable0 = new TextField();
			stackVariable0.set_Value(str);
			return stackVariable0;
		}

		public static implicit operator String(TextField field)
		{
			return field.get_Value();
		}
	}
}