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

		private readonly Dictionary<Type, bool> _types = new Dictionary<Type, bool>();

		private readonly JsonSerializerSettings _jsonSettings;

		public DistributedCache(IDistributedCache cache)
		{
			this._cache = cache;
			JsonSerializerSettings jsonSerializerSetting = new JsonSerializerSettings();
			jsonSerializerSetting.set_TypeNameHandling(3);
			this._jsonSettings = jsonSerializerSetting;
		}

		public T Get<T>(string key)
		{
			string str = DistributedCacheExtensions.GetString(this._cache, key);
			if (String.IsNullOrEmpty(str))
			{
				return default(T);
			}
			return JsonConvert.DeserializeObject<T>(str, this._jsonSettings);
		}

		public void Remove(string key)
		{
			this._cache.Remove(key);
		}

		public void Set<T>(string key, T value)
		{
			DistributedCacheExtensions.SetString(this._cache, key, JsonConvert.SerializeObject(value, this._jsonSettings));
		}
	}
}