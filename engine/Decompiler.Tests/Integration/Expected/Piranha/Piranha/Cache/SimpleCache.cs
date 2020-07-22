using Piranha;
using System;
using System.Collections.Generic;

namespace Piranha.Cache
{
	public class SimpleCache : ICache
	{
		private readonly IDictionary<string, object> _cache;

		private readonly bool _clone;

		public SimpleCache(bool clone = true)
		{
			this._cache = new ConcurrentDictionary<string, object>();
			base();
			this._clone = clone;
			return;
		}

		public T Get<T>(string key)
		{
			if (!this._cache.TryGetValue(key, out V_0))
			{
				V_1 = default(T);
				return V_1;
			}
			if (!this._clone)
			{
				return (T)V_0;
			}
			return Utils.DeepClone<T>((T)V_0);
		}

		public void Remove(string key)
		{
			dummyVar0 = this._cache.Remove(key);
			return;
		}

		public void Set<T>(string key, T value)
		{
			this._cache.set_Item(key, value);
			return;
		}
	}
}