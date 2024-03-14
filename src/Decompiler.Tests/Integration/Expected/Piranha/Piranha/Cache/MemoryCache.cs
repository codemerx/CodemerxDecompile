using Microsoft.Extensions.Caching.Memory;
using Piranha;
using System;

namespace Piranha.Cache
{
	public class MemoryCache : ICache
	{
		private readonly IMemoryCache _cache;

		private readonly bool _clone;

		public MemoryCache(IMemoryCache cache)
		{
			this._cache = cache;
		}

		protected MemoryCache(IMemoryCache cache, bool clone)
		{
			this._cache = cache;
			this._clone = clone;
		}

		public T Get<T>(string key)
		{
			T t = default(T);
			if (!CacheExtensions.TryGetValue<T>(this._cache, key, ref t))
			{
				return default(T);
			}
			if (!this._clone)
			{
				return t;
			}
			return Utils.DeepClone<T>(t);
		}

		public void Remove(string key)
		{
			this._cache.Remove(key);
		}

		public void Set<T>(string key, T value)
		{
			CacheExtensions.Set<T>(this._cache, key, value);
		}
	}
}