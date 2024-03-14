using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Shell.Descriptor;
using OrchardCore.Environment.Shell.Descriptor.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OrchardCore.Environment.Shell
{
	public class ShellDescriptorFeaturesManager : IShellDescriptorFeaturesManager
	{
		private readonly IExtensionManager _extensionManager;

		private readonly IEnumerable<ShellFeature> _alwaysEnabledFeatures;

		private readonly IShellDescriptorManager _shellDescriptorManager;

		private readonly ILogger _logger;

		public FeatureDependencyNotificationHandler FeatureDependencyNotification
		{
			get;
			set;
		}

		public ShellDescriptorFeaturesManager(IExtensionManager extensionManager, IEnumerable<ShellFeature> shellFeatures, IShellDescriptorManager shellDescriptorManager, ILogger<ShellFeaturesManager> logger)
		{
			this._extensionManager = extensionManager;
			this._alwaysEnabledFeatures = (
				from f in shellFeatures
				where f.get_AlwaysEnabled()
				select f).ToArray<ShellFeature>();
			this._shellDescriptorManager = shellDescriptorManager;
			this._logger = logger;
		}

		private IEnumerable<IFeatureInfo> GetFeaturesToDisable(IFeatureInfo featureInfo, IEnumerable<string> enabledFeatureIds, bool force)
		{
			List<IFeatureInfo> list = (
				from f in this._extensionManager.GetDependentFeatures(featureInfo.get_Id())
				where enabledFeatureIds.Contains<string>(f.get_Id())
				select f).ToList<IFeatureInfo>();
			if (list.Count <= 1 || force)
			{
				return list;
			}
			if (this._logger.IsEnabled(3))
			{
				LoggerExtensions.LogWarning(this._logger, " To disable '{FeatureId}', additional features need to be disabled.", new object[] { featureInfo.get_Id() });
			}
			FeatureDependencyNotificationHandler featureDependencyNotification = this.FeatureDependencyNotification;
			if (featureDependencyNotification != null)
			{
				featureDependencyNotification.Invoke("If {0} is disabled, then you'll also need to disable {1}.", featureInfo, 
					from f in list
					where f.get_Id() != featureInfo.get_Id()
					select f);
			}
			else
			{
			}
			return Enumerable.Empty<IFeatureInfo>();
		}

		private IEnumerable<IFeatureInfo> GetFeaturesToEnable(IFeatureInfo featureInfo, IEnumerable<string> enabledFeatureIds, bool force)
		{
			List<IFeatureInfo> list = (
				from f in this._extensionManager.GetFeatureDependencies(featureInfo.get_Id())
				where !enabledFeatureIds.Contains<string>(f.get_Id())
				select f).ToList<IFeatureInfo>();
			if (list.Count <= 1 || force)
			{
				return list;
			}
			if (this._logger.IsEnabled(3))
			{
				LoggerExtensions.LogWarning(this._logger, " To enable '{FeatureId}', additional features need to be enabled.", new object[] { featureInfo.get_Id() });
			}
			FeatureDependencyNotificationHandler featureDependencyNotification = this.FeatureDependencyNotification;
			if (featureDependencyNotification != null)
			{
				featureDependencyNotification.Invoke("If {0} is enabled, then you'll also need to enable {1}.", featureInfo, 
					from f in list
					where f.get_Id() != featureInfo.get_Id()
					select f);
			}
			else
			{
			}
			return Enumerable.Empty<IFeatureInfo>();
		}

		public async Task<ValueTuple<IEnumerable<IFeatureInfo>, IEnumerable<IFeatureInfo>>> UpdateFeaturesAsync(ShellDescriptor shellDescriptor, IEnumerable<IFeatureInfo> featuresToDisable, IEnumerable<IFeatureInfo> featuresToEnable, bool force)
		{
			IEnumerable<ShellFeature> shellFeatures = this._alwaysEnabledFeatures;
			string[] array = (
				from sf in shellFeatures
				select sf.get_Id()).ToArray<string>();
			List<IFeatureInfo> list = (
				from f in this._extensionManager.GetFeatures()
				where shellDescriptor.get_Features().Any<ShellFeature>((ShellFeature sf) => sf.get_Id() == f.get_Id())
				select f).ToList<IFeatureInfo>();
			List<IFeatureInfo> featureInfos = list;
			string[] strArrays = (
				from f in featureInfos
				select f.get_Id()).ToArray<string>();
			List<IFeatureInfo> list1 = (
				from f in featuresToDisable
				where !array.Contains<string>(f.get_Id())
				select f).SelectMany<IFeatureInfo, IFeatureInfo>((IFeatureInfo feature) => this.GetFeaturesToDisable(feature, strArrays, force)).Distinct<IFeatureInfo>().ToList<IFeatureInfo>();
			if (list1.Count > 0)
			{
				foreach (IFeatureInfo featureInfo in list1)
				{
					list.Remove(featureInfo);
					if (!this._logger.IsEnabled(2))
					{
						continue;
					}
					ILogger logger = this._logger;
					object[] id = new object[] { featureInfo.get_Id() };
					LoggerExtensions.LogInformation(logger, "Feature '{FeatureName}' was disabled", id);
				}
			}
			List<IFeatureInfo> featureInfos1 = list;
			strArrays = (
				from f in featureInfos1
				select f.get_Id()).ToArray<string>();
			List<IFeatureInfo> list2 = featuresToEnable.SelectMany<IFeatureInfo, IFeatureInfo>((IFeatureInfo feature) => this.GetFeaturesToEnable(feature, strArrays, force)).Distinct<IFeatureInfo>().ToList<IFeatureInfo>();
			if (list2.Count > 0)
			{
				if (this._logger.IsEnabled(2))
				{
					foreach (IFeatureInfo featureInfo1 in list2)
					{
						ILogger logger1 = this._logger;
						object[] objArray = new object[] { featureInfo1.get_Id() };
						LoggerExtensions.LogInformation(logger1, "Enabling feature '{FeatureName}'", objArray);
					}
				}
				list = list.Concat<IFeatureInfo>(list2).Distinct<IFeatureInfo>().ToList<IFeatureInfo>();
			}
			if (list1.Count > 0 || list2.Count > 0)
			{
				IShellDescriptorManager shellDescriptorManager = this._shellDescriptorManager;
				int serialNumber = shellDescriptor.get_SerialNumber();
				List<IFeatureInfo> featureInfos2 = list;
				await shellDescriptorManager.UpdateShellDescriptorAsync(serialNumber, (
					from x in featureInfos2
					select new ShellFeature(x.get_Id(), false)).ToList<ShellFeature>(), shellDescriptor.get_Parameters());
			}
			return new ValueTuple<IEnumerable<IFeatureInfo>, IEnumerable<IFeatureInfo>>(list1, list2);
		}
	}
}