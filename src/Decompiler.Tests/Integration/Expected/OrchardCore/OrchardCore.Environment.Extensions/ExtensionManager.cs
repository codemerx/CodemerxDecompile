using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Extensions.Loaders;
using OrchardCore.Environment.Extensions.Manifests;
using OrchardCore.Environment.Extensions.Utility;
using OrchardCore.Modules;
using OrchardCore.Modules.Manifest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

		private readonly ConcurrentDictionary<string, Lazy<IEnumerable<IFeatureInfo>>> _featureDependencies = new ConcurrentDictionary<string, Lazy<IEnumerable<IFeatureInfo>>>();

		private readonly ConcurrentDictionary<string, Lazy<IEnumerable<IFeatureInfo>>> _dependentFeatures = new ConcurrentDictionary<string, Lazy<IEnumerable<IFeatureInfo>>>();

		private readonly static Func<IFeatureInfo, IFeatureInfo[], IFeatureInfo[]> GetDependentFeaturesFunc;

		private readonly static Func<IFeatureInfo, IFeatureInfo[], IFeatureInfo[]> GetFeatureDependenciesFunc;

		private bool _isInitialized;

		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

		public ILogger L
		{
			get;
			set;
		}

		static ExtensionManager()
		{
			ExtensionManager.GetDependentFeaturesFunc = (IFeatureInfo currentFeature, IFeatureInfo[] fs) => {
				Func<string, bool> func2 = null;
				return fs.Where<IFeatureInfo>((IFeatureInfo f) => {
					string[] dependencies = f.get_Dependencies();
					Func<string, bool> u003cu003e9_3 = func2;
					if (u003cu003e9_3 == null)
					{
						Func<string, bool> func = new Func<string, bool>(this.u003cu002ecctoru003eb__3);
						Func<string, bool> func1 = func;
						func2 = func;
						u003cu003e9_3 = func1;
					}
					return dependencies.Any<string>(u003cu003e9_3);
				}).ToArray<IFeatureInfo>();
			};
			ExtensionManager.GetFeatureDependenciesFunc = (IFeatureInfo currentFeature, IFeatureInfo[] fs) => (
				from f in fs
				where currentFeature.get_Dependencies().Any<string>((string dep) => dep == f.get_Id())
				select f).ToArray<IFeatureInfo>();
		}

		public ExtensionManager(IApplicationContext applicationContext, IEnumerable<IExtensionDependencyStrategy> extensionDependencyStrategies, IEnumerable<IExtensionPriorityStrategy> extensionPriorityStrategies, ITypeFeatureProvider typeFeatureProvider, IFeaturesProvider featuresProvider, ILogger<ExtensionManager> logger)
		{
			this._applicationContext = applicationContext;
			this._extensionDependencyStrategies = extensionDependencyStrategies;
			this._extensionPriorityStrategies = extensionPriorityStrategies;
			this._typeFeatureProvider = typeFeatureProvider;
			this._featuresProvider = featuresProvider;
			this.L = logger;
		}

		private void EnsureInitialized()
		{
			if (this._isInitialized)
			{
				return;
			}
			this.EnsureInitializedAsync().GetAwaiter().GetResult();
		}

		private async Task EnsureInitializedAsync()
		{
			Type[] typeArray;
			if (!this._isInitialized)
			{
				await this._semaphore.WaitAsync();
				try
				{
					if (!this._isInitialized)
					{
						IEnumerable<OrchardCore.Modules.Module> modules = this._applicationContext.get_Application().get_Modules();
						ConcurrentDictionary<string, ExtensionEntry> strs = new ConcurrentDictionary<string, ExtensionEntry>();
						await EnumerableExtensions.ForEachAsync<OrchardCore.Modules.Module>(modules, (OrchardCore.Modules.Module module) => {
							if (!module.get_ModuleInfo().get_Exists())
							{
								return Task.CompletedTask;
							}
							ManifestInfo manifestInfo = new ManifestInfo(module.get_ModuleInfo());
							ExtensionInfo extensionInfo = new ExtensionInfo(module.get_SubPath(), manifestInfo, (IManifestInfo mi, IExtensionInfo ei) => this._featuresProvider.GetFeatures(ei, mi));
							ExtensionEntry extensionEntry = new ExtensionEntry();
							extensionEntry.set_ExtensionInfo(extensionInfo);
							extensionEntry.set_Assembly(module.get_Assembly());
							extensionEntry.set_ExportedTypes(module.get_Assembly().ExportedTypes);
							strs.TryAdd(module.get_Name(), extensionEntry);
							return Task.CompletedTask;
						});
						Dictionary<string, FeatureEntry> strs1 = new Dictionary<string, FeatureEntry>();
						var array = strs.SelectMany((KeyValuePair<string, ExtensionEntry> extension) => extension.Value.get_ExportedTypes().Where<Type>(new Func<Type, bool>(this.IsComponentType)).Select((Type type) => new { ExtensionEntry = extension.Value, Type = type })).ToArray();
						var sourceFeatureNameForType = 
							from typeByExtension in (IEnumerable<u003cu003ef__AnonymousType0<ExtensionEntry, Type>>)array
							group typeByExtension by ExtensionManager.GetSourceFeatureNameForType(typeByExtension.Type, typeByExtension.ExtensionEntry.get_ExtensionInfo().get_Id());
						Dictionary<string, Type[]> dictionary = sourceFeatureNameForType.ToDictionary((group) => group.Key, (group) => (
							from typesByExtension in group
							select typesByExtension.Type).ToArray<Type>());
						foreach (KeyValuePair<string, ExtensionEntry> str in strs)
						{
							foreach (IFeatureInfo feature in str.Value.get_ExtensionInfo().get_Features())
							{
								if (!dictionary.TryGetValue(feature.get_Id(), out typeArray))
								{
									typeArray = Array.Empty<Type>();
								}
								else
								{
									Type[] typeArray1 = typeArray;
									for (int i = 0; i < (int)typeArray1.Length; i++)
									{
										Type type1 = typeArray1[i];
										this._typeFeatureProvider.TryAdd(type1, feature);
									}
								}
								strs1.Add(feature.get_Id(), new CompiledFeatureEntry(feature, typeArray));
							}
						}
						ExtensionManager extensionManager = this;
						ExtensionManager extensionManager1 = this;
						Dictionary<!0, !1>.ValueCollection values = strs1.Values;
						extensionManager._featureInfos = extensionManager1.Order(
							from f in values
							select f.get_FeatureInfo());
						ExtensionManager dictionary1 = this;
						IFeatureInfo[] featureInfoArray = this._featureInfos;
						dictionary1._features = featureInfoArray.ToDictionary<IFeatureInfo, string, FeatureEntry>((IFeatureInfo f) => f.get_Id(), (IFeatureInfo f) => strs1[f.get_Id()]);
						ExtensionManager extensionManager2 = this;
						IFeatureInfo[] featureInfoArray1 = this._featureInfos;
						IEnumerable<IFeatureInfo> id = 
							from f in featureInfoArray1
							where f.get_Id() == f.get_Extension().get_Features().First<IFeatureInfo>().get_Id()
							select f;
						extensionManager2._extensionsInfos = 
							from f in id
							select f.get_Extension();
						ExtensionManager dictionary2 = this;
						IEnumerable<IExtensionInfo> extensionInfos = this._extensionsInfos;
						dictionary2._extensions = extensionInfos.ToDictionary<IExtensionInfo, string, ExtensionEntry>((IExtensionInfo e) => e.get_Id(), (IExtensionInfo e) => strs[e.get_Id()]);
						this._isInitialized = true;
					}
					else
					{
						return;
					}
				}
				finally
				{
					this._semaphore.Release();
				}
			}
		}

		public IEnumerable<IFeatureInfo> GetDependentFeatures(string featureId)
		{
			this.EnsureInitialized();
			return this._dependentFeatures.GetOrAdd(featureId, (string key) => new Lazy<IEnumerable<IFeatureInfo>>(() => {
				if (!this._features.ContainsKey(key))
				{
					return Enumerable.Empty<IFeatureInfo>();
				}
				IFeatureInfo featureInfo = this._features[key].get_FeatureInfo();
				return this.GetDependentFeatures(featureInfo, this._featureInfos);
			})).Value;
		}

		private IEnumerable<IFeatureInfo> GetDependentFeatures(IFeatureInfo feature, IFeatureInfo[] features)
		{
			Func<IFeatureInfo, bool> func = null;
			HashSet<IFeatureInfo> featureInfos = new HashSet<IFeatureInfo>();
			featureInfos.Add(feature);
			HashSet<IFeatureInfo> featureInfos1 = featureInfos;
			Stack<IFeatureInfo[]> featureInfoArrays = new Stack<IFeatureInfo[]>();
			featureInfoArrays.Push(ExtensionManager.GetDependentFeaturesFunc(feature, features));
			while (featureInfoArrays.Count > 0)
			{
				IFeatureInfo[] featureInfoArray = featureInfoArrays.Pop();
				Func<IFeatureInfo, bool> func1 = func;
				if (func1 == null)
				{
					Func<IFeatureInfo, bool> func2 = (IFeatureInfo dependency) => !featureInfos1.Contains(dependency);
					Func<IFeatureInfo, bool> func3 = func2;
					func = func2;
					func1 = func3;
				}
				foreach (IFeatureInfo featureInfo in featureInfoArray.Where<IFeatureInfo>(func1))
				{
					featureInfos1.Add(featureInfo);
					featureInfoArrays.Push(ExtensionManager.GetDependentFeaturesFunc(featureInfo, features));
				}
			}
			return 
				from f in this._featureInfos
				where featureInfos1.Any<IFeatureInfo>((IFeatureInfo d) => d.get_Id() == f.get_Id())
				select f;
		}

		public IExtensionInfo GetExtension(string extensionId)
		{
			ExtensionEntry extensionEntry;
			this.EnsureInitialized();
			if (!string.IsNullOrEmpty(extensionId) && this._extensions.TryGetValue(extensionId, out extensionEntry))
			{
				return extensionEntry.get_ExtensionInfo();
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
			return this._featureDependencies.GetOrAdd(featureId, (string key) => new Lazy<IEnumerable<IFeatureInfo>>(() => {
				if (!this._features.ContainsKey(key))
				{
					return Enumerable.Empty<IFeatureInfo>();
				}
				IFeatureInfo featureInfo = this._features[key].get_FeatureInfo();
				return this.GetFeatureDependencies(featureInfo, this._featureInfos);
			})).Value;
		}

		private IEnumerable<IFeatureInfo> GetFeatureDependencies(IFeatureInfo feature, IFeatureInfo[] features)
		{
			Func<IFeatureInfo, bool> func = null;
			HashSet<IFeatureInfo> featureInfos = new HashSet<IFeatureInfo>();
			featureInfos.Add(feature);
			HashSet<IFeatureInfo> featureInfos1 = featureInfos;
			Stack<IFeatureInfo[]> featureInfoArrays = new Stack<IFeatureInfo[]>();
			featureInfoArrays.Push(ExtensionManager.GetFeatureDependenciesFunc(feature, features));
			while (featureInfoArrays.Count > 0)
			{
				IFeatureInfo[] featureInfoArray = featureInfoArrays.Pop();
				Func<IFeatureInfo, bool> func1 = func;
				if (func1 == null)
				{
					Func<IFeatureInfo, bool> func2 = (IFeatureInfo dependency) => !featureInfos1.Contains(dependency);
					Func<IFeatureInfo, bool> func3 = func2;
					func = func2;
					func1 = func3;
				}
				foreach (IFeatureInfo featureInfo in featureInfoArray.Where<IFeatureInfo>(func1))
				{
					featureInfos1.Add(featureInfo);
					featureInfoArrays.Push(ExtensionManager.GetFeatureDependenciesFunc(featureInfo, features));
				}
			}
			return 
				from f in this._featureInfos
				where featureInfos1.Any<IFeatureInfo>((IFeatureInfo d) => d.get_Id() == f.get_Id())
				select f;
		}

		public IEnumerable<IFeatureInfo> GetFeatures(string[] featureIdsToLoad)
		{
			this.EnsureInitialized();
			IEnumerable<IFeatureInfo> featureInfos = featureIdsToLoad.SelectMany<string, IFeatureInfo>((string featureId) => this.GetFeatureDependencies(featureId)).Distinct<IFeatureInfo>();
			return 
				from f in this._featureInfos
				where featureInfos.Any<IFeatureInfo>((IFeatureInfo d) => d.get_Id() == f.get_Id())
				select f;
		}

		public IEnumerable<IFeatureInfo> GetFeatures()
		{
			this.EnsureInitialized();
			return this._featureInfos;
		}

		private int GetPriority(IFeatureInfo feature)
		{
			return this._extensionPriorityStrategies.Sum<IExtensionPriorityStrategy>((IExtensionPriorityStrategy s) => s.GetPriority(feature));
		}

		private static string GetSourceFeatureNameForType(Type type, string extensionId)
		{
			object featureName;
			OrchardCore.Modules.FeatureAttribute featureAttribute = type.GetCustomAttributes<OrchardCore.Modules.FeatureAttribute>(false).FirstOrDefault<OrchardCore.Modules.FeatureAttribute>();
			if (featureAttribute != null)
			{
				featureName = featureAttribute.get_FeatureName();
			}
			else
			{
				featureName = null;
			}
			if (featureName == null)
			{
				featureName = extensionId;
			}
			return featureName;
		}

		private bool HasDependency(IFeatureInfo f1, IFeatureInfo f2)
		{
			return this._extensionDependencyStrategies.Any<IExtensionDependencyStrategy>((IExtensionDependencyStrategy s) => s.HasDependency(f1, f2));
		}

		private bool IsComponentType(Type type)
		{
			if (!type.IsClass || type.IsAbstract)
			{
				return false;
			}
			return type.IsPublic;
		}

		public Task<ExtensionEntry> LoadExtensionAsync(IExtensionInfo extensionInfo)
		{
			ExtensionEntry extensionEntry;
			this.EnsureInitialized();
			if (this._extensions.TryGetValue(extensionInfo.get_Id(), out extensionEntry))
			{
				return Task.FromResult<ExtensionEntry>(extensionEntry);
			}
			return Task.FromResult<ExtensionEntry>(null);
		}

		public async Task<IEnumerable<FeatureEntry>> LoadFeaturesAsync()
		{
			await this.EnsureInitializedAsync();
			return this._features.Values;
		}

		public async Task<IEnumerable<FeatureEntry>> LoadFeaturesAsync(string[] featureIdsToLoad)
		{
			await this.EnsureInitializedAsync();
			IEnumerable<IFeatureInfo> features = this.GetFeatures(featureIdsToLoad);
			List<string> list = (
				from f in features
				select f.get_Id()).ToList<string>();
			IEnumerable<FeatureEntry> values = 
				from f in this._features.Values
				where list.Contains(f.get_FeatureInfo().get_Id())
				select f;
			return values;
		}

		private IFeatureInfo[] Order(IEnumerable<IFeatureInfo> featuresToOrder)
		{
			return DependencyOrdering.OrderByDependenciesAndPriorities<IFeatureInfo>(
				from x in featuresToOrder
				orderby x.get_Id()
				select x, new Func<IFeatureInfo, IFeatureInfo, bool>(this.HasDependency), new Func<IFeatureInfo, int>(this.GetPriority)).ToArray<IFeatureInfo>();
		}
	}
}