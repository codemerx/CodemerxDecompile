using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Shell.Descriptor.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OrchardCore.Environment.Shell
{
	public class ShellFeaturesManager : IShellFeaturesManager
	{
		private readonly IExtensionManager _extensionManager;

		private readonly ShellDescriptor _shellDescriptor;

		private readonly IShellDescriptorFeaturesManager _shellDescriptorFeaturesManager;

		public ShellFeaturesManager(IExtensionManager extensionManager, ShellDescriptor shellDescriptor, IShellDescriptorFeaturesManager shellDescriptorFeaturesManager)
		{
			base();
			this._extensionManager = extensionManager;
			this._shellDescriptor = shellDescriptor;
			this._shellDescriptorFeaturesManager = shellDescriptorFeaturesManager;
			return;
		}

		public Task<IEnumerable<IFeatureInfo>> GetAlwaysEnabledFeaturesAsync()
		{
			return Task.FromResult<IEnumerable<IFeatureInfo>>(Enumerable.Where<IFeatureInfo>(this._extensionManager.GetFeatures(), new Func<IFeatureInfo, bool>(this, ShellFeaturesManager.u003cGetAlwaysEnabledFeaturesAsyncu003eb__5_0)));
		}

		public Task<IEnumerable<IFeatureInfo>> GetDisabledFeaturesAsync()
		{
			return Task.FromResult<IEnumerable<IFeatureInfo>>(Enumerable.Where<IFeatureInfo>(this._extensionManager.GetFeatures(), new Func<IFeatureInfo, bool>(this, ShellFeaturesManager.u003cGetDisabledFeaturesAsyncu003eb__6_0)));
		}

		public Task<IEnumerable<IExtensionInfo>> GetEnabledExtensionsAsync()
		{
			V_0 = new ShellFeaturesManager.u003cu003ec__DisplayClass8_0();
			V_0.u003cu003e4__this = this;
			stackVariable3 = V_0;
			stackVariable10 = Enumerable.Where<IFeatureInfo>(this._extensionManager.GetFeatures(), new Func<IFeatureInfo, bool>(V_0, ShellFeaturesManager.u003cu003ec__DisplayClass8_0.u003cGetEnabledExtensionsAsyncu003eb__0));
			stackVariable11 = ShellFeaturesManager.u003cu003ec.u003cu003e9__8_1;
			if (stackVariable11 == null)
			{
				dummyVar0 = stackVariable11;
				stackVariable11 = new Func<IFeatureInfo, string>(ShellFeaturesManager.u003cu003ec.u003cu003e9, ShellFeaturesManager.u003cu003ec.u003cGetEnabledExtensionsAsyncu003eb__8_1);
				ShellFeaturesManager.u003cu003ec.u003cu003e9__8_1 = stackVariable11;
			}
			stackVariable3.enabledIds = Enumerable.ToArray<string>(Enumerable.Distinct<string>(Enumerable.Select<IFeatureInfo, string>(stackVariable10, stackVariable11)));
			return Task.FromResult<IEnumerable<IExtensionInfo>>(Enumerable.Where<IExtensionInfo>(this._extensionManager.GetExtensions(), new Func<IExtensionInfo, bool>(V_0, ShellFeaturesManager.u003cu003ec__DisplayClass8_0.u003cGetEnabledExtensionsAsyncu003eb__2)));
		}

		public Task<IEnumerable<IFeatureInfo>> GetEnabledFeaturesAsync()
		{
			return Task.FromResult<IEnumerable<IFeatureInfo>>(Enumerable.Where<IFeatureInfo>(this._extensionManager.GetFeatures(), new Func<IFeatureInfo, bool>(this, ShellFeaturesManager.u003cGetEnabledFeaturesAsyncu003eb__4_0)));
		}

		public Task<ValueTuple<IEnumerable<IFeatureInfo>, IEnumerable<IFeatureInfo>>> UpdateFeaturesAsync(IEnumerable<IFeatureInfo> featuresToDisable, IEnumerable<IFeatureInfo> featuresToEnable, bool force)
		{
			return this._shellDescriptorFeaturesManager.UpdateFeaturesAsync(this._shellDescriptor, featuresToDisable, featuresToEnable, force);
		}
	}
}