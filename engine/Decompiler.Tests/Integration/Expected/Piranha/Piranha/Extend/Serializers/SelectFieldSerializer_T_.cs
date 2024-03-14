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
		}

		public object Deserialize(string str)
		{
			T t = Activator.CreateInstance<T>();
			if (!String.IsNullOrWhiteSpace(str))
			{
				try
				{
					t.EnumValue = str;
				}
				catch
				{
					t.EnumValue = null;
				}
			}
			return t;
		}

		public string Serialize(object obj)
		{
			SelectFieldBase selectFieldBase = obj as SelectFieldBase;
			if (selectFieldBase == null)
			{
				throw new ArgumentException("The given object doesn't match the serialization type");
			}
			return selectFieldBase.EnumValue;
		}
	}
}