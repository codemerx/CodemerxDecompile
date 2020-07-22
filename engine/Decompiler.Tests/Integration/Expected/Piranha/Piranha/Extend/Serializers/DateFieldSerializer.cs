using Piranha.Extend;
using Piranha.Extend.Fields;
using System;

namespace Piranha.Extend.Serializers
{
	public class DateFieldSerializer : ISerializer
	{
		public DateFieldSerializer()
		{
			base();
			return;
		}

		public object Deserialize(string str)
		{
			V_0 = new DateField();
			if (!String.IsNullOrWhiteSpace(str))
			{
				try
				{
					V_0.set_Value(new DateTime?(DateTime.Parse(str)));
				}
				catch
				{
					dummyVar0 = exception_0;
					V_1 = null;
					V_0.set_Value(V_1);
				}
			}
			return V_0;
		}

		public string Serialize(object obj)
		{
			V_0 = obj as DateField;
			if (V_0 == null)
			{
				throw new ArgumentException("The given object doesn't match the serialization type");
			}
			if (!V_0.get_Value().get_HasValue())
			{
				return null;
			}
			V_2 = V_0.get_Value().get_Value();
			return V_2.ToString("yyyy-MM-dd");
		}
	}
}