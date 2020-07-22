using Piranha.Extend;
using Piranha.Extend.Fields;
using System;

namespace Piranha.Extend.Serializers
{
	public class SelectFieldSerializer<T> : ISerializer
	where T : SelectFieldBase
	{
		public SelectFieldSerializer()
		{
			base();
			return;
		}

		public object Deserialize(string str)
		{
			V_0 = Activator.CreateInstance<T>();
			if (!String.IsNullOrWhiteSpace(str))
			{
				try
				{
					V_0.set_EnumValue(str);
				}
				catch
				{
					dummyVar0 = exception_0;
					V_0.set_EnumValue(null);
				}
			}
			return V_0;
		}

		public string Serialize(object obj)
		{
			V_0 = obj as SelectFieldBase;
			if (V_0 == null)
			{
				throw new ArgumentException("The given object doesn't match the serialization type");
			}
			return V_0.get_EnumValue();
		}
	}
}