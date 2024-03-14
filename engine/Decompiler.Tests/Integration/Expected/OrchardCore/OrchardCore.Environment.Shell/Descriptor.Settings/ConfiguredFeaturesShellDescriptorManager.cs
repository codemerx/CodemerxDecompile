using Microsoft.Extensions.Configuration;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Environment.Shell.Descriptor;
using OrchardCore.Environment.Shell.Descriptor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OrchardCore.Environment.Shell.Descriptor.Settings
{
	public class ConfiguredFeaturesShellDescriptorManager : IShellDescriptorManager
	{
		private readonly IShellConfiguration _shellConfiguration;

		private readonly IEnumerable<ShellFeature> _alwaysEnabledFeatures;

		private ShellDescriptor _shellDescriptor;

		public ConfiguredFeaturesShellDescriptorManager(IShellConfiguration shellConfiguration, IEnumerable<ShellFeature> shellFeatures)
		{
			this._shellConfiguration = shellConfiguration;
			this._alwaysEnabledFeatures = (
				from f in shellFeatures
				where f.get_AlwaysEnabled()
				select f).ToArray<ShellFeature>();
		}

		public Task<ShellDescriptor> GetShellDescriptorAsync()
		{
			if (this._shellDescriptor == null)
			{
				ConfiguredFeaturesShellDescriptorManager.ConfiguredFeatures configuredFeature = new ConfiguredFeaturesShellDescriptorManager.ConfiguredFeatures();
				ConfigurationBinder.Bind(this._shellConfiguration, configuredFeature);
				IEnumerable<ShellFeature> shellFeatures = this._alwaysEnabledFeatures.Concat<ShellFeature>(configuredFeature.Features.Select<string, ShellFeature>((string id) => {
					ShellFeature shellFeature = new ShellFeature(id, false);
					shellFeature.set_AlwaysEnabled(true);
					return shellFeature;
				})).Distinct<ShellFeature>();
				ShellDescriptor shellDescriptor = new ShellDescriptor();
				shellDescriptor.set_Features(shellFeatures.ToList<ShellFeature>());
				this._shellDescriptor = shellDescriptor;
			}
			return Task.FromResult<ShellDescriptor>(this._shellDescriptor);
		}

		public Task UpdateShellDescriptorAsync(int priorSerialNumber, IEnumerable<ShellFeature> enabledFeatures, IEnumerable<ShellParameter> parameters)
		{
			return Task.CompletedTask;
		}

		private class ConfiguredFeatures
		{
			public string[] Features
			{
				get;
				set;
			}

			public ConfiguredFeatures()
			{
			}
		}
	}
}