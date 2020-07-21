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
					V_0.set_Id(str);
				}
				catch
				{
					dummyVar0 = exception_0;
					V_0.set_Id(null);
				}
			}
			return V_0;
		}

		public string Serialize(object obj)
		{
			V_0 = obj as DataSelectFieldBase;
			if (V_0 == null)
			{
				throw new ArgumentException("The given object doesn't match the serialization type");
			}
			return V_0.get_Id();
		}
	}
}