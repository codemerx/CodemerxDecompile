using OrchardCore.Environment.Shell.Descriptor;
using OrchardCore.Environment.Shell.Descriptor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrchardCore.Environment.Shell.Descriptor.Settings
{
	public class SetFeaturesShellDescriptorManager : IShellDescriptorManager
	{
		private readonly IEnumerable<ShellFeature> _shellFeatures;

		private ShellDescriptor _shellDescriptor;

		public SetFeaturesShellDescriptorManager(IEnumerable<ShellFeature> shellFeatures)
		{
			this._shellFeatures = shellFeatures;
		}

		public Task<ShellDescriptor> GetShellDescriptorAsync()
		{
			if (this._shellDescriptor == null)
			{
				ShellDescriptor shellDescriptor = new ShellDescriptor();
				shellDescriptor.set_Features(this._shellFeatures.Distinct<ShellFeature>().ToList<ShellFeature>());
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