using OrchardCore.Environment.Extensions.Features;
using System;
using System.Collections.Concurrent;

namespace OrchardCore.Environment.Extensions
{
	public class TypeFeatureProvider : ITypeFeatureProvider
	{
		private readonly ConcurrentDictionary<Type, IFeatureInfo> _features;

		public TypeFeatureProvider()
		{
			this._features = new ConcurrentDictionary<Type, IFeatureInfo>();
			base();
			return;
		}

		public IFeatureInfo GetFeatureForDependency(Type dependency)
		{
			V_0 = null;
			if (!this._features.TryGetValue(dependency, ref V_0))
			{
				throw new InvalidOperationException(string.Concat("Could not resolve feature for type ", dependency.get_Name()));
			}
			return V_0;
		}

		public void TryAdd(Type type, IFeatureInfo feature)
		{
			dummyVar0 = this._features.TryAdd(type, feature);
			return;
		}
	}
}