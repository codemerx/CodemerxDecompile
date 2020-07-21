using Piranha.Extend;
using Piranha.Extend.Fields;
using System;

namespace Piranha.Extend.Serializers
{
	public class IntegerFieldSerializer<T> : ISerializer
	where T : SimpleField<int?>
	{
		public IntegerFieldSerializer()
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
					V_0.set_Value(new int?(Int32.Parse(str)));
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
			V_0 = (T)(obj as T);
			if (V_0 == null)
			{
				throw new ArgumentException("The given object doesn't match the serialization type");
			}
			if (!V_0.get_Value().get_HasValue())
			{
				return null;
			}
			V_1 = V_0.get_Value();
			return V_1.get_Value().ToString();
		}
	}
}