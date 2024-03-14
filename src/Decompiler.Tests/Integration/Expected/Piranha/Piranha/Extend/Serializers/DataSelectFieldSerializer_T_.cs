using Piranha.Extend;
using Piranha.Extend.Fields;
using System;

namespace Piranha.Extend.Serializers
{
	public class DataSelectFieldSerializer<T> : ISerializer
	where T : DataSelectFieldBase
	{
		public DataSelectFieldSerializer()
		{
		}

		public object Deserialize(string str)
		{
			T t = Activator.CreateInstance<T>();
			if (!String.IsNullOrWhiteSpace(str))
			{
				try
				{
					t.Id = str;
				}
				catch
				{
					t.Id = null;
				}
			}
			return t;
		}

		public string Serialize(object obj)
		{
			DataSelectFieldBase dataSelectFieldBase = obj as DataSelectFieldBase;
			if (dataSelectFieldBase == null)
			{
				throw new ArgumentException("The given object doesn't match the serialization type");
			}
			return dataSelectFieldBase.Id;
		}
	}
}