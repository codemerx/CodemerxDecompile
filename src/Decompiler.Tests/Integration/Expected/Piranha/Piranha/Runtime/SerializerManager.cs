using Piranha.Extend;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Piranha.Runtime
{
	public sealed class SerializerManager
	{
		private readonly Dictionary<Type, ISerializer> _serializers;

		public ISerializer this[Type type]
		{
			get
			{
				ISerializer serializer = null;
				if (this._serializers.TryGetValue(type, out serializer))
				{
					return serializer;
				}
				return null;
			}
		}

		public SerializerManager()
		{
			this._serializers = new Dictionary<Type, ISerializer>();
		}

		public void Register<T>(ISerializer serializer)
		{
			this._serializers[typeof(T)] = serializer;
		}

		public void UnRegister<T>()
		{
			this._serializers.Remove(typeof(T));
		}
	}
}