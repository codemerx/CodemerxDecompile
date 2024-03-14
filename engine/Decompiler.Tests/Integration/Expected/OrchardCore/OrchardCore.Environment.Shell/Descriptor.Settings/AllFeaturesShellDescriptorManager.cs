using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Shell.Descriptor;
using OrchardCore.Environment.Shell.Descriptor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OrchardCore.Environment.Shell.Descriptor.Settings
{
	public class AllFeaturesShellDescriptorManager : IShellDescriptorManager
	{
		private readonly IExtensionManager _extensionManager;

		private ShellDescriptor _shellDescriptor;

		public AllFeaturesShellDescriptorManager(IExtensionManager extensionManager)
		{
			this._extensionManager = extensionManager;
		}

		public Task<ShellDescriptor> GetShellDescriptorAsync()
		{
			if (this._shellDescriptor == null)
			{
				ShellDescriptor shellDescriptor = new ShellDescriptor();
				shellDescriptor.set_Features(this._extensionManager.GetFeatures().Select<IFeatureInfo, ShellFeature>((IFeatureInfo x) => {
					ShellFeature shellFeature = new ShellFeature();
					shellFeature.set_Id(x.get_Id());
					return shellFeature;
				}).ToList<ShellFeature>());
				this._shellDescriptor = shellDescriptor;
			}
			return Task.FromResult<ShellDescriptor>(this._shellDescriptor);
		}

		public Task UpdateShellDescriptorAsync(int priorSerialNumber, IEnumerable<ShellFeature> enabledFeatures, IEnumerable<ShellParameter> parameters)
		{
			return Task.CompletedTask;
		}
	}
}