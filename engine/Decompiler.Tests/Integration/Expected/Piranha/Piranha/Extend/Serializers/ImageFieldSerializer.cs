using Piranha.Extend;
using Piranha.Extend.Fields;
using System;

namespace Piranha.Extend.Serializers
{
	public class ImageFieldSerializer : ISerializer
	{
		public ImageFieldSerializer()
		{
			base();
			return;
		}

		public object Deserialize(string str)
		{
			stackVariable0 = new ImageField();
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
			V_0 = obj as ImageField;
			if (V_0 == null)
			{
				throw new ArgumentException("The given object doesn't match the serialization type");
			}
			return V_0.get_Id().ToString();
		}
	}
}