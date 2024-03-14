using Piranha.Extend;
using Piranha.Extend.Fields;
using System;

namespace Piranha.Extend.Serializers
{
	public class CheckBoxFieldSerializer<T> : ISerializer
	where T : SimpleField<bool>
	{
		public CheckBoxFieldSerializer()
		{
		}

		public object Deserialize(string str)
		{
			T t = Activator.CreateInstance<T>();
			t.Value = Boolean.Parse(str);
			return t;
		}

		public string Serialize(object obj)
		{
			T t = (T)(obj as T);
			if (t == null)
			{
				throw new ArgumentException("The given object doesn't match the serialization type");
			}
			return t.Value.ToString();
		}
	}
}