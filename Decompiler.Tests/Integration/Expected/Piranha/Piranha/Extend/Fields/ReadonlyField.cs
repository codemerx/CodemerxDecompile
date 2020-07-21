using Piranha.Extend;
using System;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Readonly", Shorthand="Readonly", Component="readonly-field")]
	public class ReadonlyField : SimpleField<string>
	{
		public ReadonlyField()
		{
			base();
			return;
		}

		public static implicit operator ReadonlyField(string str)
		{
			stackVariable0 = new ReadonlyField();
			stackVariable0.set_Value(str);
			return stackVariable0;
		}

		public static implicit operator String(ReadonlyField field)
		{
			return field.get_Value();
		}
	}
}