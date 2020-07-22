using Piranha.Extend;
using System;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Number", Shorthand="Number", Component="number-field")]
	public class NumberField : SimpleField<int?>
	{
		public NumberField()
		{
			base();
			return;
		}

		public static implicit operator NumberField(int number)
		{
			stackVariable0 = new NumberField();
			stackVariable0.set_Value(new int?(number));
			return stackVariable0;
		}

		public static implicit operator Nullable<Int32>(NumberField field)
		{
			return field.get_Value();
		}
	}
}