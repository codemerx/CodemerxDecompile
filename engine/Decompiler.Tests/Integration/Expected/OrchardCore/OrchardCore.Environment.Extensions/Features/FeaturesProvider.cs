using OrchardCore.Environment.Extensions;
using OrchardCore.Modules.Manifest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OrchardCore.Environment.Extensions.Features
{
	public class FeaturesProvider : IFeaturesProvider
	{
		public const string FeatureProviderCacheKey = "FeatureProvider:Features";

		private readonly IEnumerable<IFeatureBuilderEvents> _featureBuilderEvents;

		public FeaturesProvider(IEnumerable<IFeatureBuilderEvents> featureBuilderEvents)
		{
			this._featureBuilderEvents = featureBuilderEvents;
		}

		public IEnumerable<IFeatureInfo> GetFeatures(IExtensionInfo extensionInfo, IManifestInfo manifestInfo)
		{
			int num;
			int num1;
			List<IFeatureInfo> featureInfos = new List<IFeatureInfo>();
			List<FeatureAttribute> list = manifestInfo.get_ModuleInfo().get_Features().ToList<FeatureAttribute>();
			if (list.Count <= 0)
			{
				string id = extensionInfo.get_Id();
				string name = manifestInfo.get_Name();
				string[] array = (
					from e in manifestInfo.get_ModuleInfo().get_Dependencies()
					select e.Trim()).ToArray<string>();
				if (!int.TryParse(manifestInfo.get_ModuleInfo().get_Priority(), out num1))
				{
					num1 = 0;
				}
				string category = manifestInfo.get_ModuleInfo().get_Category();
				string description = manifestInfo.get_ModuleInfo().get_Description();
				bool defaultTenantOnly = manifestInfo.get_ModuleInfo().get_DefaultTenantOnly();
				bool isAlwaysEnabled = manifestInfo.get_ModuleInfo().get_IsAlwaysEnabled();
				FeatureBuildingContext featureBuildingContext = new FeatureBuildingContext();
				featureBuildingContext.set_FeatureId(id);
				featureBuildingContext.set_FeatureName(name);
				featureBuildingContext.set_Category(category);
				featureBuildingContext.set_Description(description);
				featureBuildingContext.set_ExtensionInfo(extensionInfo);
				featureBuildingContext.set_ManifestInfo(manifestInfo);
				featureBuildingContext.set_Priority(num1);
				featureBuildingContext.set_FeatureDependencyIds(array);
				featureBuildingContext.set_DefaultTenantOnly(defaultTenantOnly);
				featureBuildingContext.set_IsAlwaysEnabled(isAlwaysEnabled);
				FeatureBuildingContext featureBuildingContext1 = featureBuildingContext;
				foreach (IFeatureBuilderEvents _featureBuilderEvent in this._featureBuilderEvents)
				{
					_featureBuilderEvent.Building(featureBuildingContext1);
				}
				FeatureInfo featureInfo = new FeatureInfo(featureBuildingContext1.get_FeatureId(), featureBuildingContext1.get_FeatureName(), featureBuildingContext1.get_Priority(), featureBuildingContext1.get_Category(), featureBuildingContext1.get_Description(), featureBuildingContext1.get_ExtensionInfo(), featureBuildingContext1.get_FeatureDependencyIds(), featureBuildingContext1.get_DefaultTenantOnly(), featureBuildingContext1.get_IsAlwaysEnabled());
				foreach (IFeatureBuilderEvents featureBuilderEvent in this._featureBuilderEvents)
				{
					featureBuilderEvent.Built(featureInfo);
				}
				featureInfos.Add(featureInfo);
			}
			else
			{
				foreach (FeatureAttribute featureAttribute in list)
				{
					if (string.IsNullOrWhiteSpace(featureAttribute.get_Id()))
					{
						throw new ArgumentException(string.Concat("A feature is missing a mandatory 'Id' property in the Module '", extensionInfo.get_Id(), "'"));
					}
					string str = featureAttribute.get_Id();
					string name1 = featureAttribute.get_Name() ?? featureAttribute.get_Id();
					string[] strArrays = (
						from e in featureAttribute.get_Dependencies()
						select e.Trim()).ToArray<string>();
					if (!int.TryParse(featureAttribute.get_Priority() ?? manifestInfo.get_ModuleInfo().get_Priority(), out num))
					{
						num = 0;
					}
					string category1 = featureAttribute.get_Category() ?? manifestInfo.get_ModuleInfo().get_Category();
					string description1 = featureAttribute.get_Description() ?? manifestInfo.get_ModuleInfo().get_Description();
					bool flag = featureAttribute.get_DefaultTenantOnly();
					bool isAlwaysEnabled1 = featureAttribute.get_IsAlwaysEnabled();
					FeatureBuildingContext featureBuildingContext2 = new FeatureBuildingContext();
					featureBuildingContext2.set_FeatureId(str);
					featureBuildingContext2.set_FeatureName(name1);
					featureBuildingContext2.set_Category(category1);
					featureBuildingContext2.set_Description(description1);
					featureBuildingContext2.set_ExtensionInfo(extensionInfo);
					featureBuildingContext2.set_ManifestInfo(manifestInfo);
					featureBuildingContext2.set_Priority(num);
					featureBuildingContext2.set_FeatureDependencyIds(strArrays);
					featureBuildingContext2.set_DefaultTenantOnly(flag);
					featureBuildingContext2.set_IsAlwaysEnabled(isAlwaysEnabled1);
					FeatureBuildingContext featureBuildingContext3 = featureBuildingContext2;
					foreach (IFeatureBuilderEvents _featureBuilderEvent1 in this._featureBuilderEvents)
					{
						_featureBuilderEvent1.Building(featureBuildingContext3);
					}
					FeatureInfo featureInfo1 = new FeatureInfo(str, name1, num, category1, description1, extensionInfo, strArrays, flag, isAlwaysEnabled1);
					foreach (IFeatureBuilderEvents featureBuilderEvent1 in this._featureBuilderEvents)
					{
						featureBuilderEvent1.Built(featureInfo1);
					}
					featureInfos.Add(featureInfo1);
				}
			}
			return featureInfos;
		}
	}
}