using OrchardCore.Environment.Extensions.Features;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace OrchardCore.Environment.Extensions
{
	public class TypeFeatureProvider : ITypeFeatureProvider
	{
		private readonly ConcurrentDictionary<Type, IFeatureInfo> _features = new ConcurrentDictionary<Type, IFeatureInfo>();

		public TypeFeatureProvider()
		{
		}

		public IFeatureInfo GetFeatureForDependency(Type dependency)
		{
			IFeatureInfo featureInfo = null;
			if (!this._features.TryGetValue(dependency, out featureInfo))
			{
				throw new InvalidOperationException(string.Concat("Could not resolve feature for type ", dependency.Name));
			}
			return featureInfo;
		}

		public void TryAdd(Type type, IFeatureInfo feature)
		{
			this._features.TryAdd(type, feature);
		}
	}
}