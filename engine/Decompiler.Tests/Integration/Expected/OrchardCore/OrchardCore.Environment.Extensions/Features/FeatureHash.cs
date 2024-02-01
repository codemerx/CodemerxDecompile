using Microsoft.Extensions.Caching.Memory;
using OrchardCore.Environment.Cache;
using OrchardCore.Environment.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OrchardCore.Environment.Extensions.Features
{
	public class FeatureHash : IFeatureHash
	{
		private const string FeatureHashCacheKey = "FeatureHash:Features";

		private readonly IShellFeaturesManager _featureManager;

		private readonly IMemoryCache _memoryCache;

		private readonly ISignal _signal;

		public FeatureHash(IShellFeaturesManager featureManager, IMemoryCache memoryCache, ISignal signal)
		{
			this._memoryCache = memoryCache;
			this._featureManager = featureManager;
			this._signal = signal;
		}

		public async Task<int> GetFeatureHashAsync()
		{
			int num;
			int num1 = 0;
			if (!CacheExtensions.TryGetValue<int>(this._memoryCache, "FeatureHash:Features", ref num1))
			{
				IEnumerable<IFeatureInfo> enabledFeaturesAsync = await this._featureManager.GetEnabledFeaturesAsync();
				IOrderedEnumerable<IFeatureInfo> id = 
					from x in enabledFeaturesAsync
					orderby x.get_Id()
					select x;
				num1 = id.Aggregate<IFeatureInfo, int>(0, (int a, IFeatureInfo f) => a * 7 + f.get_Id().GetHashCode());
				MemoryCacheEntryOptions memoryCacheEntryOption = MemoryCacheEntryExtensions.AddExpirationToken(new MemoryCacheEntryOptions(), this._signal.GetToken("FeatureProvider:Features"));
				CacheExtensions.Set<int>(this._memoryCache, "FeatureHash:Features", num1, memoryCacheEntryOption);
				num = num1;
			}
			else
			{
				num = num1;
			}
			return num;
		}

		public async Task<int> GetFeatureHashAsync(string featureId)
		{
			bool flag = false;
			string str = string.Format("{0}:{1}", "FeatureHash:Features", featureId);
			if (!CacheExtensions.TryGetValue<bool>(this._memoryCache, str, ref flag))
			{
				flag = await this._featureManager.GetEnabledFeaturesAsync().Any<IFeatureInfo>((IFeatureInfo x) => x.get_Id().Equals(featureId));
				MemoryCacheEntryOptions memoryCacheEntryOption = MemoryCacheEntryExtensions.AddExpirationToken(new MemoryCacheEntryOptions(), this._signal.GetToken("FeatureProvider:Features"));
				CacheExtensions.Set<bool>(this._memoryCache, str, flag, memoryCacheEntryOption);
			}
			return (flag ? 1 : 0);
		}
	}
}