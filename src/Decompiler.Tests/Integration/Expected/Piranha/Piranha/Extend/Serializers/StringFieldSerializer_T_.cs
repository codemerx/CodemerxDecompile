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
		}

		public object Deserialize(string str)
		{
			T t = Activator.CreateInstance<T>();
			t.Value = str;
			return t;
		}

		public string Serialize(object obj)
		{
			T t = (T)(obj as T);
			if (t == null)
			{
				throw new ArgumentException("The given object doesn't match the serialization type");
			}
			return t.Value;
		}
	}
}