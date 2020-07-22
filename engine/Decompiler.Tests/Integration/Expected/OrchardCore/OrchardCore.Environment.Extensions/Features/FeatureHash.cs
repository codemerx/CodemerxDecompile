using Microsoft.Extensions.Caching.Memory;
using OrchardCore.Environment.Cache;
using OrchardCore.Environment.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			base();
			this._memoryCache = memoryCache;
			this._featureManager = featureManager;
			this._signal = signal;
			return;
		}

		public async Task<int> GetFeatureHashAsync()
		{
			V_0.u003cu003e4__this = this;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<int>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<FeatureHash.u003cGetFeatureHashAsyncu003ed__5>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<int> GetFeatureHashAsync(string featureId)
		{
			V_0.u003cu003e4__this = this;
			V_0.featureId = featureId;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<int>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<FeatureHash.u003cGetFeatureHashAsyncu003ed__6>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}