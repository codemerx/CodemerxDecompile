using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Shell.Descriptor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
			this._extensionManager = extensionManager;
			this._shellDescriptor = shellDescriptor;
			this._shellDescriptorFeaturesManager = shellDescriptorFeaturesManager;
		}

		public Task<IEnumerable<IFeatureInfo>> GetAlwaysEnabledFeaturesAsync()
		{
			return Task.FromResult<IEnumerable<IFeatureInfo>>(this._extensionManager.GetFeatures().Where<IFeatureInfo>((IFeatureInfo f) => {
				if (f.get_IsAlwaysEnabled())
				{
					return true;
				}
				return this._shellDescriptor.get_Features().Any<ShellFeature>((ShellFeature sf) => {
					if (sf.get_Id() != f.get_Id())
					{
						return false;
					}
					return sf.get_AlwaysEnabled();
				});
			}));
		}

		public Task<IEnumerable<IFeatureInfo>> GetDisabledFeaturesAsync()
		{
			return Task.FromResult<IEnumerable<IFeatureInfo>>(
				from f in this._extensionManager.GetFeatures()
				where this._shellDescriptor.get_Features().All<ShellFeature>((ShellFeature sf) => sf.get_Id() != f.get_Id())
				select f);
		}

		public Task<IEnumerable<IExtensionInfo>> GetEnabledExtensionsAsync()
		{
			string[] array = (
				from f in this._extensionManager.GetFeatures()
				where this._shellDescriptor.get_Features().Any<ShellFeature>((ShellFeature sf) => sf.get_Id() == f.get_Id())
				select f.get_Extension().get_Id()).Distinct<string>().ToArray<string>();
			return Task.FromResult<IEnumerable<IExtensionInfo>>(
				from e in this._extensionManager.GetExtensions()
				where array.Contains<string>(e.get_Id())
				select e);
		}

		public Task<IEnumerable<IFeatureInfo>> GetEnabledFeaturesAsync()
		{
			return Task.FromResult<IEnumerable<IFeatureInfo>>(
				from f in this._extensionManager.GetFeatures()
				where this._shellDescriptor.get_Features().Any<ShellFeature>((ShellFeature sf) => sf.get_Id() == f.get_Id())
				select f);
		}

		public Task<ValueTuple<IEnumerable<IFeatureInfo>, IEnumerable<IFeatureInfo>>> UpdateFeaturesAsync(IEnumerable<IFeatureInfo> featuresToDisable, IEnumerable<IFeatureInfo> featuresToEnable, bool force)
		{
			return this._shellDescriptorFeaturesManager.UpdateFeaturesAsync(this._shellDescriptor, featuresToDisable, featuresToEnable, force);
		}
	}
}