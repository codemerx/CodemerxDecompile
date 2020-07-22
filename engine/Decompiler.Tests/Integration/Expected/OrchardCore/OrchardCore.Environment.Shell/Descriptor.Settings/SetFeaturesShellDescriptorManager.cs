using OrchardCore.Environment.Shell.Descriptor;
using OrchardCore.Environment.Shell.Descriptor.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrchardCore.Environment.Shell.Descriptor.Settings
{
	public class SetFeaturesShellDescriptorManager : IShellDescriptorManager
	{
		private readonly IEnumerable<ShellFeature> _shellFeatures;

		private ShellDescriptor _shellDescriptor;

		public SetFeaturesShellDescriptorManager(IEnumerable<ShellFeature> shellFeatures)
		{
			base();
			this._shellFeatures = shellFeatures;
			return;
		}

		public Task<ShellDescriptor> GetShellDescriptorAsync()
		{
			if (this._shellDescriptor == null)
			{
				stackVariable6 = new ShellDescriptor();
				stackVariable6.set_Features(Enumerable.ToList<ShellFeature>(Enumerable.Distinct<ShellFeature>(this._shellFeatures)));
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