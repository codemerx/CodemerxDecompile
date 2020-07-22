using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Extensions.Loaders;
using OrchardCore.Environment.Extensions.Manifests;
using OrchardCore.Modules;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OrchardCore.Environment.Extensions
{
	public class ExtensionManager : IExtensionManager
	{
		private readonly IApplicationContext _applicationContext;

		private readonly IEnumerable<IExtensionDependencyStrategy> _extensionDependencyStrategies;

		private readonly IEnumerable<IExtensionPriorityStrategy> _extensionPriorityStrategies;

		private readonly ITypeFeatureProvider _typeFeatureProvider;

		private readonly IFeaturesProvider _featuresProvider;

		private IDictionary<string, ExtensionEntry> _extensions;

		private IEnumerable<IExtensionInfo> _extensionsInfos;

		private IDictionary<string, FeatureEntry> _features;

		private IFeatureInfo[] _featureInfos;

		private readonly ConcurrentDictionary<string, Lazy<IEnumerable<IFeatureInfo>>> _featureDependencies;

		private readonly ConcurrentDictionary<string, Lazy<IEnumerable<IFeatureInfo>>> _dependentFeatures;

		private readonly static Func<IFeatureInfo, IFeatureInfo[], IFeatureInfo[]> GetDependentFeaturesFunc;

		private readonly static Func<IFeatureInfo, IFeatureInfo[], IFeatureInfo[]> GetFeatureDependenciesFunc;

		private bool _isInitialized;

		private readonly SemaphoreSlim _semaphore;

		public ILogger L
		{
			get;
			set;
		}

		static ExtensionManager()
		{
			ExtensionManager.GetDependentFeaturesFunc = new Func<IFeatureInfo, IFeatureInfo[], IFeatureInfo[]>(ExtensionManager.u003cu003ec.u003cu003e9, ExtensionManager.u003cu003ec.u003cu002ecctoru003eb__38_0);
			ExtensionManager.GetFeatureDependenciesFunc = new Func<IFeatureInfo, IFeatureInfo[], IFeatureInfo[]>(ExtensionManager.u003cu003ec.u003cu003e9, ExtensionManager.u003cu003ec.u003cu002ecctoru003eb__38_1);
			return;
		}

		public ExtensionManager(IApplicationContext applicationContext, IEnumerable<IExtensionDependencyStrategy> extensionDependencyStrategies, IEnumerable<IExtensionPriorityStrategy> extensionPriorityStrategies, ITypeFeatureProvider typeFeatureProvider, IFeaturesProvider featuresProvider, ILogger<ExtensionManager> logger)
		{
			this._featureDependencies = new ConcurrentDictionary<string, Lazy<IEnumerable<IFeatureInfo>>>();
			this._dependentFeatures = new ConcurrentDictionary<string, Lazy<IEnumerable<IFeatureInfo>>>();
			this._semaphore = new SemaphoreSlim(1);
			base();
			this._applicationContext = applicationContext;
			this._extensionDependencyStrategies = extensionDependencyStrategies;
			this._extensionPriorityStrategies = extensionPriorityStrategies;
			this._typeFeatureProvider = typeFeatureProvider;
			this._featuresProvider = featuresProvider;
			this.set_L(logger);
			return;
		}

		private void EnsureInitialized()
		{
			if (this._isInitialized)
			{
				return;
			}
			this.EnsureInitializedAsync().GetAwaiter().GetResult();
			return;
		}

		private async Task EnsureInitializedAsync()
		{
			V_0.u003cu003e4__this = this;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ExtensionManager.u003cEnsureInitializedAsyncu003ed__33>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public IEnumerable<IFeatureInfo> GetDependentFeatures(string featureId)
		{
			this.EnsureInitialized();
			return this._dependentFeatures.GetOrAdd(featureId, new Func<string, Lazy<IEnumerable<IFeatureInfo>>>(this, ExtensionManager.u003cGetDependentFeaturesu003eb__27_0)).get_Value();
		}

		private IEnumerable<IFeatureInfo> GetDependentFeatures(IFeatureInfo feature, IFeatureInfo[] features)
		{
			V_0 = new ExtensionManager.u003cu003ec__DisplayClass29_0();
			stackVariable2 = new HashSet<IFeatureInfo>();
			dummyVar0 = stackVariable2.Add(feature);
			V_0.dependencies = stackVariable2;
			V_1 = new Stack<IFeatureInfo[]>();
			V_1.Push(ExtensionManager.GetDependentFeaturesFunc.Invoke(feature, features));
			while (V_1.get_Count() > 0)
			{
				stackVariable15 = V_1.Pop();
				stackVariable17 = V_0.u003cu003e9__1;
				if (stackVariable17 == null)
				{
					dummyVar1 = stackVariable17;
					stackVariable36 = new Func<IFeatureInfo, bool>(V_0, ExtensionManager.u003cu003ec__DisplayClass29_0.u003cGetDependentFeaturesu003eb__1);
					V_3 = stackVariable36;
					V_0.u003cu003e9__1 = stackVariable36;
					stackVariable17 = V_3;
				}
				V_2 = Enumerable.Where<IFeatureInfo>(stackVariable15, stackVariable17).GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_4 = V_2.get_Current();
						dummyVar2 = V_0.dependencies.Add(V_4);
						V_1.Push(ExtensionManager.GetDependentFeaturesFunc.Invoke(V_4, features));
					}
				}
				finally
				{
					if (V_2 != null)
					{
						V_2.Dispose();
					}
				}
			}
			return Enumerable.Where<IFeatureInfo>(this._featureInfos, new Func<IFeatureInfo, bool>(V_0, ExtensionManager.u003cu003ec__DisplayClass29_0.u003cGetDependentFeaturesu003eb__0));
		}

		public IExtensionInfo GetExtension(string extensionId)
		{
			this.EnsureInitialized();
			if (!string.IsNullOrEmpty(extensionId) && this._extensions.TryGetValue(extensionId, ref V_0))
			{
				return V_0.get_ExtensionInfo();
			}
			return new NotFoundExtensionInfo(extensionId);
		}

		public IEnumerable<IExtensionInfo> GetExtensions()
		{
			this.EnsureInitialized();
			return this._extensionsInfos;
		}

		public IEnumerable<IFeatureInfo> GetFeatureDependencies(string featureId)
		{
			this.EnsureInitialized();
			return this._featureDependencies.GetOrAdd(featureId, new Func<string, Lazy<IEnumerable<IFeatureInfo>>>(this, ExtensionManager.u003cGetFeatureDependenciesu003eb__26_0)).get_Value();
		}

		private IEnumerable<IFeatureInfo> GetFeatureDependencies(IFeatureInfo feature, IFeatureInfo[] features)
		{
			V_0 = new ExtensionManager.u003cu003ec__DisplayClass28_0();
			stackVariable2 = new HashSet<IFeatureInfo>();
			dummyVar0 = stackVariable2.Add(feature);
			V_0.dependencies = stackVariable2;
			V_1 = new Stack<IFeatureInfo[]>();
			V_1.Push(ExtensionManager.GetFeatureDependenciesFunc.Invoke(feature, features));
			while (V_1.get_Count() > 0)
			{
				stackVariable15 = V_1.Pop();
				stackVariable17 = V_0.u003cu003e9__1;
				if (stackVariable17 == null)
				{
					dummyVar1 = stackVariable17;
					stackVariable36 = new Func<IFeatureInfo, bool>(V_0, ExtensionManager.u003cu003ec__DisplayClass28_0.u003cGetFeatureDependenciesu003eb__1);
					V_3 = stackVariable36;
					V_0.u003cu003e9__1 = stackVariable36;
					stackVariable17 = V_3;
				}
				V_2 = Enumerable.Where<IFeatureInfo>(stackVariable15, stackVariable17).GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_4 = V_2.get_Current();
						dummyVar2 = V_0.dependencies.Add(V_4);
						V_1.Push(ExtensionManager.GetFeatureDependenciesFunc.Invoke(V_4, features));
					}
				}
				finally
				{
					if (V_2 != null)
					{
						V_2.Dispose();
					}
				}
			}
			return Enumerable.Where<IFeatureInfo>(this._featureInfos, new Func<IFeatureInfo, bool>(V_0, ExtensionManager.u003cu003ec__DisplayClass28_0.u003cGetFeatureDependenciesu003eb__0));
		}

		public IEnumerable<IFeatureInfo> GetFeatures(string[] featureIdsToLoad)
		{
			V_0 = new ExtensionManager.u003cu003ec__DisplayClass22_0();
			V_0.u003cu003e4__this = this;
			this.EnsureInitialized();
			V_0.allDependencies = Enumerable.Distinct<IFeatureInfo>(Enumerable.SelectMany<string, IFeatureInfo>(featureIdsToLoad, new Func<string, IEnumerable<IFeatureInfo>>(V_0, ExtensionManager.u003cu003ec__DisplayClass22_0.u003cGetFeaturesu003eb__0)));
			return Enumerable.Where<IFeatureInfo>(this._featureInfos, new Func<IFeatureInfo, bool>(V_0, ExtensionManager.u003cu003ec__DisplayClass22_0.u003cGetFeaturesu003eb__1));
		}

		public IEnumerable<IFeatureInfo> GetFeatures()
		{
			this.EnsureInitialized();
			return this._featureInfos;
		}

		private int GetPriority(IFeatureInfo feature)
		{
			V_0 = new ExtensionManager.u003cu003ec__DisplayClass37_0();
			V_0.feature = feature;
			return Enumerable.Sum<IExtensionPriorityStrategy>(this._extensionPriorityStrategies, new Func<IExtensionPriorityStrategy, int>(V_0, ExtensionManager.u003cu003ec__DisplayClass37_0.u003cGetPriorityu003eb__0));
		}

		private static string GetSourceFeatureNameForType(Type type, string extensionId)
		{
			stackVariable3 = Enumerable.FirstOrDefault<FeatureAttribute>(CustomAttributeExtensions.GetCustomAttributes<FeatureAttribute>(type, false));
			if (stackVariable3 != null)
			{
				stackVariable4 = stackVariable3.get_FeatureName();
			}
			else
			{
				dummyVar0 = stackVariable3;
				stackVariable4 = null;
			}
			if (stackVariable4 == null)
			{
				dummyVar1 = stackVariable4;
				stackVariable4 = extensionId;
			}
			return stackVariable4;
		}

		private bool HasDependency(IFeatureInfo f1, IFeatureInfo f2)
		{
			V_0 = new ExtensionManager.u003cu003ec__DisplayClass36_0();
			V_0.f1 = f1;
			V_0.f2 = f2;
			return Enumerable.Any<IExtensionDependencyStrategy>(this._extensionDependencyStrategies, new Func<IExtensionDependencyStrategy, bool>(V_0, ExtensionManager.u003cu003ec__DisplayClass36_0.u003cHasDependencyu003eb__0));
		}

		private bool IsComponentType(Type type)
		{
			if (!type.get_IsClass() || type.get_IsAbstract())
			{
				return false;
			}
			return type.get_IsPublic();
		}

		public Task<ExtensionEntry> LoadExtensionAsync(IExtensionInfo extensionInfo)
		{
			this.EnsureInitialized();
			if (this._extensions.TryGetValue(extensionInfo.get_Id(), ref V_0))
			{
				return Task.FromResult<ExtensionEntry>(V_0);
			}
			return Task.FromResult<ExtensionEntry>(null);
		}

		public async Task<IEnumerable<FeatureEntry>> LoadFeaturesAsync()
		{
			V_0.u003cu003e4__this = this;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<IEnumerable<FeatureEntry>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ExtensionManager.u003cLoadFeaturesAsyncu003ed__24>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<IEnumerable<FeatureEntry>> LoadFeaturesAsync(string[] featureIdsToLoad)
		{
			V_0.u003cu003e4__this = this;
			V_0.featureIdsToLoad = featureIdsToLoad;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<IEnumerable<FeatureEntry>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ExtensionManager.u003cLoadFeaturesAsyncu003ed__25>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private IFeatureInfo[] Order(IEnumerable<IFeatureInfo> featuresToOrder)
		{
			stackVariable0 = featuresToOrder;
			stackVariable1 = ExtensionManager.u003cu003ec.u003cu003e9__35_0;
			if (stackVariable1 == null)
			{
				dummyVar0 = stackVariable1;
				stackVariable1 = new Func<IFeatureInfo, string>(ExtensionManager.u003cu003ec.u003cu003e9, ExtensionManager.u003cu003ec.u003cOrderu003eb__35_0);
				ExtensionManager.u003cu003ec.u003cu003e9__35_0 = stackVariable1;
			}
			return Enumerable.ToArray<IFeatureInfo>(DependencyOrdering.OrderByDependenciesAndPriorities<IFeatureInfo>(Enumerable.OrderBy<IFeatureInfo, string>(stackVariable0, stackVariable1), new Func<IFeatureInfo, IFeatureInfo, bool>(this, ExtensionManager.HasDependency), new Func<IFeatureInfo, int>(this, ExtensionManager.GetPriority)));
		}
	}
}