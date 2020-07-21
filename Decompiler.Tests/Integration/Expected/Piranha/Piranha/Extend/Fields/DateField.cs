using Piranha.Extend;
using System;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Date", Shorthand="Date", Component="date-field")]
	public class DateField : SimpleField<DateTime?>
	{
		public DateField()
		{
			base();
			return;
		}

		public static implicit operator DateField(DateTime date)
		{
			stackVariable0 = new DateField();
			stackVariable0.set_Value(new DateTime?(date));
			return stackVariable0;
		}
	}
}