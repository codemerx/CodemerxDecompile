using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Piranha;
using System;
using System.Collections.Generic;

namespace Piranha.Cache
{
	public class DistributedCache : ICache
	{
		private readonly IDistributedCache _cache;

		private readonly Dictionary<Type, bool> _types;

		private readonly JsonSerializerSettings _jsonSettings;

		public DistributedCache(IDistributedCache cache)
		{
			this._types = new Dictionary<Type, bool>();
			base();
			this._cache = cache;
			stackVariable6 = new JsonSerializerSettings();
			stackVariable6.set_TypeNameHandling(3);
			this._jsonSettings = stackVariable6;
			return;
		}

		public T Get<T>(string key)
		{
			V_0 = DistributedCacheExtensions.GetString(this._cache, key);
			if (String.IsNullOrEmpty(V_0))
			{
				V_1 = default(T);
				return V_1;
			}
			return JsonConvert.DeserializeObject<T>(V_0, this._jsonSettings);
		}

		public void Remove(string key)
		{
			this._cache.Remove(key);
			return;
		}

		public void Set<T>(string key, T value)
		{
			DistributedCacheExtensions.SetString(this._cache, key, JsonConvert.SerializeObject(value, this._jsonSettings));
			return;
		}
	}
}