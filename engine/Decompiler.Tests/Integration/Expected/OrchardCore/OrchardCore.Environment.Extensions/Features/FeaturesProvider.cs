using OrchardCore.Environment.Extensions;
using OrchardCore.Modules.Manifest;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OrchardCore.Environment.Extensions.Features
{
	public class FeaturesProvider : IFeaturesProvider
	{
		public const string FeatureProviderCacheKey = "FeatureProvider:Features";

		private readonly IEnumerable<IFeatureBuilderEvents> _featureBuilderEvents;

		public FeaturesProvider(IEnumerable<IFeatureBuilderEvents> featureBuilderEvents)
		{
			base();
			this._featureBuilderEvents = featureBuilderEvents;
			return;
		}

		public IEnumerable<IFeatureInfo> GetFeatures(IExtensionInfo extensionInfo, IManifestInfo manifestInfo)
		{
			V_0 = new List<IFeatureInfo>();
			V_1 = Enumerable.ToList<FeatureAttribute>(manifestInfo.get_ModuleInfo().get_Features());
			if (V_1.get_Count() <= 0)
			{
				V_15 = extensionInfo.get_Id();
				V_16 = manifestInfo.get_Name();
				stackVariable14 = manifestInfo.get_ModuleInfo().get_Dependencies();
				stackVariable15 = FeaturesProvider.u003cu003ec.u003cu003e9__3_1;
				if (stackVariable15 == null)
				{
					dummyVar5 = stackVariable15;
					stackVariable15 = new Func<string, string>(FeaturesProvider.u003cu003ec.u003cu003e9, FeaturesProvider.u003cu003ec.u003cGetFeaturesu003eb__3_1);
					FeaturesProvider.u003cu003ec.u003cu003e9__3_1 = stackVariable15;
				}
				V_17 = Enumerable.ToArray<string>(Enumerable.Select<string, string>(stackVariable14, stackVariable15));
				if (!int.TryParse(manifestInfo.get_ModuleInfo().get_Priority(), ref V_18))
				{
					V_18 = 0;
				}
				V_19 = manifestInfo.get_ModuleInfo().get_Category();
				V_20 = manifestInfo.get_ModuleInfo().get_Description();
				V_21 = manifestInfo.get_ModuleInfo().get_DefaultTenantOnly();
				V_22 = manifestInfo.get_ModuleInfo().get_IsAlwaysEnabled();
				stackVariable35 = new FeatureBuildingContext();
				stackVariable35.set_FeatureId(V_15);
				stackVariable35.set_FeatureName(V_16);
				stackVariable35.set_Category(V_19);
				stackVariable35.set_Description(V_20);
				stackVariable35.set_ExtensionInfo(extensionInfo);
				stackVariable35.set_ManifestInfo(manifestInfo);
				stackVariable35.set_Priority(V_18);
				stackVariable35.set_FeatureDependencyIds(V_17);
				stackVariable35.set_DefaultTenantOnly(V_21);
				stackVariable35.set_IsAlwaysEnabled(V_22);
				V_23 = stackVariable35;
				V_14 = this._featureBuilderEvents.GetEnumerator();
				try
				{
					while (V_14.MoveNext())
					{
						V_14.get_Current().Building(V_23);
					}
				}
				finally
				{
					if (V_14 != null)
					{
						V_14.Dispose();
					}
				}
				V_24 = new FeatureInfo(V_23.get_FeatureId(), V_23.get_FeatureName(), V_23.get_Priority(), V_23.get_Category(), V_23.get_Description(), V_23.get_ExtensionInfo(), V_23.get_FeatureDependencyIds(), V_23.get_DefaultTenantOnly(), V_23.get_IsAlwaysEnabled());
				V_14 = this._featureBuilderEvents.GetEnumerator();
				try
				{
					while (V_14.MoveNext())
					{
						V_14.get_Current().Built(V_24);
					}
				}
				finally
				{
					if (V_14 != null)
					{
						V_14.Dispose();
					}
				}
				V_0.Add(V_24);
			}
			else
			{
				V_2 = V_1.GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = V_2.get_Current();
						if (string.IsNullOrWhiteSpace(V_3.get_Id()))
						{
							throw new ArgumentException(string.Concat("A feature is missing a mandatory 'Id' property in the Module '", extensionInfo.get_Id(), "'"));
						}
						V_4 = V_3.get_Id();
						stackVariable99 = V_3.get_Name();
						if (stackVariable99 == null)
						{
							dummyVar0 = stackVariable99;
							stackVariable99 = V_3.get_Id();
						}
						V_5 = stackVariable99;
						stackVariable101 = V_3.get_Dependencies();
						stackVariable102 = FeaturesProvider.u003cu003ec.u003cu003e9__3_0;
						if (stackVariable102 == null)
						{
							dummyVar1 = stackVariable102;
							stackVariable102 = new Func<string, string>(FeaturesProvider.u003cu003ec.u003cu003e9, FeaturesProvider.u003cu003ec.u003cGetFeaturesu003eb__3_0);
							FeaturesProvider.u003cu003ec.u003cu003e9__3_0 = stackVariable102;
						}
						V_6 = Enumerable.ToArray<string>(Enumerable.Select<string, string>(stackVariable101, stackVariable102));
						stackVariable106 = V_3.get_Priority();
						if (stackVariable106 == null)
						{
							dummyVar2 = stackVariable106;
							stackVariable106 = manifestInfo.get_ModuleInfo().get_Priority();
						}
						if (!int.TryParse(stackVariable106, ref V_7))
						{
							V_7 = 0;
						}
						stackVariable110 = V_3.get_Category();
						if (stackVariable110 == null)
						{
							dummyVar3 = stackVariable110;
							stackVariable110 = manifestInfo.get_ModuleInfo().get_Category();
						}
						V_8 = stackVariable110;
						stackVariable112 = V_3.get_Description();
						if (stackVariable112 == null)
						{
							dummyVar4 = stackVariable112;
							stackVariable112 = manifestInfo.get_ModuleInfo().get_Description();
						}
						V_9 = stackVariable112;
						V_10 = V_3.get_DefaultTenantOnly();
						V_11 = V_3.get_IsAlwaysEnabled();
						stackVariable117 = new FeatureBuildingContext();
						stackVariable117.set_FeatureId(V_4);
						stackVariable117.set_FeatureName(V_5);
						stackVariable117.set_Category(V_8);
						stackVariable117.set_Description(V_9);
						stackVariable117.set_ExtensionInfo(extensionInfo);
						stackVariable117.set_ManifestInfo(manifestInfo);
						stackVariable117.set_Priority(V_7);
						stackVariable117.set_FeatureDependencyIds(V_6);
						stackVariable117.set_DefaultTenantOnly(V_10);
						stackVariable117.set_IsAlwaysEnabled(V_11);
						V_12 = stackVariable117;
						V_14 = this._featureBuilderEvents.GetEnumerator();
						try
						{
							while (V_14.MoveNext())
							{
								V_14.get_Current().Building(V_12);
							}
						}
						finally
						{
							if (V_14 != null)
							{
								V_14.Dispose();
							}
						}
						V_13 = new FeatureInfo(V_4, V_5, V_7, V_8, V_9, extensionInfo, V_6, V_10, V_11);
						V_14 = this._featureBuilderEvents.GetEnumerator();
						try
						{
							while (V_14.MoveNext())
							{
								V_14.get_Current().Built(V_13);
							}
						}
						finally
						{
							if (V_14 != null)
							{
								V_14.Dispose();
							}
						}
						V_0.Add(V_13);
					}
				}
				finally
				{
					V_2.Dispose();
				}
			}
			return V_0;
		}
	}
}