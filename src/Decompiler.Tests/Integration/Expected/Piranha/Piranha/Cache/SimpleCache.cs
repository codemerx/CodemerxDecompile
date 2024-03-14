using Piranha;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Piranha.Cache
{
	public class SimpleCache : ICache
	{
		private readonly IDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();

		private readonly bool _clone;

		public SimpleCache(bool clone = true)
		{
			this._clone = clone;
		}

		public T Get<T>(string key)
		{
			object obj;
			if (!this._cache.TryGetValue(key, out obj))
			{
				return default(T);
			}
			if (!this._clone)
			{
				return (T)obj;
			}
			return Utils.DeepClone<T>((T)obj);
		}

		public void Remove(string key)
		{
			this._cache.Remove(key);
		}

		public void Set<T>(string key, T value)
		{
			this._cache[key] = value;
		}
	}
}