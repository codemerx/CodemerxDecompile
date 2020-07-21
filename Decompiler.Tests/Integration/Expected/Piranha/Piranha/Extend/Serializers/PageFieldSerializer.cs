using Piranha.Extend;
using Piranha.Extend.Fields;
using System;

namespace Piranha.Extend.Serializers
{
	public class PageFieldSerializer : ISerializer
	{
		public PageFieldSerializer()
		{
			base();
			return;
		}

		public object Deserialize(string str)
		{
			stackVariable0 = new PageField();
			if (!String.IsNullOrEmpty(str))
			{
				stackVariable5 = new Guid?(new Guid(str));
			}
			else
			{
				V_0 = null;
				stackVariable5 = V_0;
			}
			stackVariable0.set_Id(stackVariable5);
			return stackVariable0;
		}

		public string Serialize(object obj)
		{
			V_0 = obj as PageField;
			if (V_0 == null)
			{
				throw new ArgumentException("The given object doesn't match the serialization type");
			}
			return V_0.get_Id().ToString();
		}
	}
}