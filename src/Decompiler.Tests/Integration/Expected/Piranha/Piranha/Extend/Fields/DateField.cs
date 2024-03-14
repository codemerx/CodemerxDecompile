using Piranha.Extend;
using System;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Date", Shorthand="Date", Component="date-field")]
	public class DateField : SimpleField<DateTime?>
	{
		public DateField()
		{
		}

		public static implicit operator DateField(DateTime date)
		{
			return new DateField()
			{
				Value = new DateTime?(date)
			};
		}
	}
}