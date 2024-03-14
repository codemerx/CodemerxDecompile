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
		}

		public object Deserialize(string str)
		{
			T nullable = Activator.CreateInstance<T>();
			if (!String.IsNullOrWhiteSpace(str))
			{
				try
				{
					nullable.Value = new int?(Int32.Parse(str));
				}
				catch
				{
					nullable.Value = null;
				}
			}
			return nullable;
		}

		public string Serialize(object obj)
		{
			T t = (T)(obj as T);
			if (t == null)
			{
				throw new ArgumentException("The given object doesn't match the serialization type");
			}
			if (!t.Value.HasValue)
			{
				return null;
			}
			return t.Value.Value.ToString();
		}
	}
}