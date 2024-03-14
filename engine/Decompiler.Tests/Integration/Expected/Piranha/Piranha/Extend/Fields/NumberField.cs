using Piranha.Extend;
using System;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Number", Shorthand="Number", Component="number-field")]
	public class NumberField : SimpleField<int?>
	{
		public NumberField()
		{
		}

		public static implicit operator NumberField(int number)
		{
			return new NumberField()
			{
				Value = new int?(number)
			};
		}

		public static implicit operator Nullable<Int32>(NumberField field)
		{
			return field.Value;
		}
	}
}