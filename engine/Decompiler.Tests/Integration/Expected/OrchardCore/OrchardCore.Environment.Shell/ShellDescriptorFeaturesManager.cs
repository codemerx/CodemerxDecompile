using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Shell.Descriptor;
using OrchardCore.Environment.Shell.Descriptor.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			base();
			this._extensionManager = extensionManager;
			stackVariable4 = shellFeatures;
			stackVariable5 = ShellDescriptorFeaturesManager.u003cu003ec.u003cu003e9__8_0;
			if (stackVariable5 == null)
			{
				dummyVar0 = stackVariable5;
				stackVariable5 = new Func<ShellFeature, bool>(ShellDescriptorFeaturesManager.u003cu003ec.u003cu003e9, ShellDescriptorFeaturesManager.u003cu003ec.u003cu002ectoru003eb__8_0);
				ShellDescriptorFeaturesManager.u003cu003ec.u003cu003e9__8_0 = stackVariable5;
			}
			this._alwaysEnabledFeatures = Enumerable.ToArray<ShellFeature>(Enumerable.Where<ShellFeature>(stackVariable4, stackVariable5));
			this._shellDescriptorManager = shellDescriptorManager;
			this._logger = logger;
			return;
		}

		private IEnumerable<IFeatureInfo> GetFeaturesToDisable(IFeatureInfo featureInfo, IEnumerable<string> enabledFeatureIds, bool force)
		{
			V_0 = new ShellDescriptorFeaturesManager.u003cu003ec__DisplayClass11_0();
			V_0.enabledFeatureIds = enabledFeatureIds;
			V_0.featureInfo = featureInfo;
			V_1 = Enumerable.ToList<IFeatureInfo>(Enumerable.Where<IFeatureInfo>(this._extensionManager.GetDependentFeatures(V_0.featureInfo.get_Id()), new Func<IFeatureInfo, bool>(V_0, ShellDescriptorFeaturesManager.u003cu003ec__DisplayClass11_0.u003cGetFeaturesToDisableu003eb__0)));
			if (V_1.get_Count() <= 1 || force)
			{
				return V_1;
			}
			if (this._logger.IsEnabled(3))
			{
				stackVariable37 = this._logger;
				stackVariable40 = new object[1];
				stackVariable40[0] = V_0.featureInfo.get_Id();
				LoggerExtensions.LogWarning(stackVariable37, " To disable '{FeatureId}', additional features need to be disabled.", stackVariable40);
			}
			stackVariable26 = this.get_FeatureDependencyNotification();
			if (stackVariable26 != null)
			{
				stackVariable26.Invoke("If {0} is disabled, then you'll also need to disable {1}.", V_0.featureInfo, Enumerable.Where<IFeatureInfo>(V_1, new Func<IFeatureInfo, bool>(V_0, ShellDescriptorFeaturesManager.u003cu003ec__DisplayClass11_0.u003cGetFeaturesToDisableu003eb__1)));
			}
			else
			{
				dummyVar0 = stackVariable26;
			}
			return Enumerable.Empty<IFeatureInfo>();
		}

		private IEnumerable<IFeatureInfo> GetFeaturesToEnable(IFeatureInfo featureInfo, IEnumerable<string> enabledFeatureIds, bool force)
		{
			V_0 = new ShellDescriptorFeaturesManager.u003cu003ec__DisplayClass10_0();
			V_0.enabledFeatureIds = enabledFeatureIds;
			V_0.featureInfo = featureInfo;
			V_1 = Enumerable.ToList<IFeatureInfo>(Enumerable.Where<IFeatureInfo>(this._extensionManager.GetFeatureDependencies(V_0.featureInfo.get_Id()), new Func<IFeatureInfo, bool>(V_0, ShellDescriptorFeaturesManager.u003cu003ec__DisplayClass10_0.u003cGetFeaturesToEnableu003eb__0)));
			if (V_1.get_Count() <= 1 || force)
			{
				return V_1;
			}
			if (this._logger.IsEnabled(3))
			{
				stackVariable37 = this._logger;
				stackVariable40 = new object[1];
				stackVariable40[0] = V_0.featureInfo.get_Id();
				LoggerExtensions.LogWarning(stackVariable37, " To enable '{FeatureId}', additional features need to be enabled.", stackVariable40);
			}
			stackVariable26 = this.get_FeatureDependencyNotification();
			if (stackVariable26 != null)
			{
				stackVariable26.Invoke("If {0} is enabled, then you'll also need to enable {1}.", V_0.featureInfo, Enumerable.Where<IFeatureInfo>(V_1, new Func<IFeatureInfo, bool>(V_0, ShellDescriptorFeaturesManager.u003cu003ec__DisplayClass10_0.u003cGetFeaturesToEnableu003eb__1)));
			}
			else
			{
				dummyVar0 = stackVariable26;
			}
			return Enumerable.Empty<IFeatureInfo>();
		}

		public async Task<ValueTuple<IEnumerable<IFeatureInfo>, IEnumerable<IFeatureInfo>>> UpdateFeaturesAsync(ShellDescriptor shellDescriptor, IEnumerable<IFeatureInfo> featuresToDisable, IEnumerable<IFeatureInfo> featuresToEnable, bool force)
		{
			V_0.u003cu003e4__this = this;
			V_0.shellDescriptor = shellDescriptor;
			V_0.featuresToDisable = featuresToDisable;
			V_0.featuresToEnable = featuresToEnable;
			V_0.force = force;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<ValueTuple<IEnumerable<IFeatureInfo>, IEnumerable<IFeatureInfo>>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ShellDescriptorFeaturesManager.u003cUpdateFeaturesAsyncu003ed__9>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}