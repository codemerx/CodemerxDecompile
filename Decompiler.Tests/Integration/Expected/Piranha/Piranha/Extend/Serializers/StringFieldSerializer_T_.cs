using Piranha.Extend;
using Piranha.Extend.Fields;
using System;

namespace Piranha.Extend.Serializers
{
	public class StringFieldSerializer<T> : ISerializer
	where T : SimpleField<string>
	{
		public StringFieldSerializer()
		{
			base();
			return;
		}

		public object Deserialize(string str)
		{
			stackVariable0 = Activator.CreateInstance<T>();
			stackVariable0.set_Value(str);
			return stackVariable0;
		}

		public string Serialize(object obj)
		{
			V_0 = (T)(obj as T);
			if (V_0 == null)
			{
				throw new ArgumentException("The given object doesn't match the serialization type");
			}
			return V_0.get_Value();
		}
	}
}