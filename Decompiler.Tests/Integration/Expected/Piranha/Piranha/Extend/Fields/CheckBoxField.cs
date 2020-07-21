using Piranha.Extend;
using System;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Checkbox", Shorthand="Checkbox", Component="checkbox-field")]
	public class CheckBoxField : SimpleField<bool>
	{
		public CheckBoxField()
		{
			base();
			return;
		}

		public static implicit operator CheckBoxField(bool str)
		{
			stackVariable0 = new CheckBoxField();
			stackVariable0.set_Value(str);
			return stackVariable0;
		}

		public static implicit operator Boolean(CheckBoxField field)
		{
			return field.get_Value();
		}
	}
}