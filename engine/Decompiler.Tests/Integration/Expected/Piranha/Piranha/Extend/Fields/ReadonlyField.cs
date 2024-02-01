using Piranha.Extend;
using System;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Readonly", Shorthand="Readonly", Component="readonly-field")]
	public class ReadonlyField : SimpleField<string>
	{
		public ReadonlyField()
		{
		}

		public static implicit operator ReadonlyField(string str)
		{
			return new ReadonlyField()
			{
				Value = str
			};
		}

		public static implicit operator String(ReadonlyField field)
		{
			return field.Value;
		}
	}
}