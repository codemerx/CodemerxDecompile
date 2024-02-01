using Piranha.Extend;
using System;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Checkbox", Shorthand="Checkbox", Component="checkbox-field")]
	public class CheckBoxField : SimpleField<bool>
	{
		public CheckBoxField()
		{
		}

		public static implicit operator CheckBoxField(bool str)
		{
			return new CheckBoxField()
			{
				Value = str
			};
		}

		public static implicit operator Boolean(CheckBoxField field)
		{
			return field.Value;
		}
	}
}