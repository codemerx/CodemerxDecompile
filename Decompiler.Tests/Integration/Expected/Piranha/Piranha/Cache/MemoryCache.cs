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
			base();
			this._cache = cache;
			return;
		}

		protected MemoryCache(IMemoryCache cache, bool clone)
		{
			base();
			this._cache = cache;
			this._clone = clone;
			return;
		}

		public T Get<T>(string key)
		{
			if (!CacheExtensions.TryGetValue<T>(this._cache, key, ref V_0))
			{
				V_1 = default(T);
				return V_1;
			}
			if (!this._clone)
			{
				return V_0;
			}
			return Utils.DeepClone<T>(V_0);
		}

		public void Remove(string key)
		{
			this._cache.Remove(key);
			return;
		}

		public void Set<T>(string key, T value)
		{
			dummyVar0 = CacheExtensions.Set<T>(this._cache, key, value);
			return;
		}
	}
}