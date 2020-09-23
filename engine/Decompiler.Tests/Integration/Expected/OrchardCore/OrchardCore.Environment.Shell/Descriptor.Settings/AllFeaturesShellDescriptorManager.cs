using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Shell.Descriptor;
using OrchardCore.Environment.Shell.Descriptor.Models;
using System;
using System.Collections.Generic;
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
			base();
			this._extensionManager = extensionManager;
			return;
		}

		public Task<ShellDescriptor> GetShellDescriptorAsync()
		{
			if (this._shellDescriptor == null)
			{
				stackVariable6 = new ShellDescriptor();
				stackVariable9 = this._extensionManager.GetFeatures();
				stackVariable10 = AllFeaturesShellDescriptorManager.u003cu003ec.u003cu003e9__3_0;
				if (stackVariable10 == null)
				{
					dummyVar0 = stackVariable10;
					stackVariable10 = new Func<IFeatureInfo, ShellFeature>(AllFeaturesShellDescriptorManager.u003cu003ec.u003cu003e9.u003cGetShellDescriptorAsyncu003eb__3_0);
					AllFeaturesShellDescriptorManager.u003cu003ec.u003cu003e9__3_0 = stackVariable10;
				}
				stackVariable6.set_Features(stackVariable9.Select<IFeatureInfo, ShellFeature>(stackVariable10).ToList<ShellFeature>());
				this._shellDescriptor = stackVariable6;
			}
			return Task.FromResult<ShellDescriptor>(this._shellDescriptor);
		}

		public Task UpdateShellDescriptorAsync(int priorSerialNumber, IEnumerable<ShellFeature> enabledFeatures, IEnumerable<ShellParameter> parameters)
		{
			return Task.get_CompletedTask();
		}
	}
}