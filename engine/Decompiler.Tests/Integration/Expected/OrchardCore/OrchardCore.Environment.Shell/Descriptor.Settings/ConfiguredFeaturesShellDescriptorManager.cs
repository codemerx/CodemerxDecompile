using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Environment.Shell.Descriptor;
using OrchardCore.Environment.Shell.Descriptor.Models;
using System;
using System.Collections.Generic;
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
			base();
			this._shellConfiguration = shellConfiguration;
			stackVariable4 = shellFeatures;
			stackVariable5 = ConfiguredFeaturesShellDescriptorManager.u003cu003ec.u003cu003e9__3_0;
			if (stackVariable5 == null)
			{
				dummyVar0 = stackVariable5;
				stackVariable5 = new Func<ShellFeature, bool>(ConfiguredFeaturesShellDescriptorManager.u003cu003ec.u003cu003e9, ConfiguredFeaturesShellDescriptorManager.u003cu003ec.u003cu002ectoru003eb__3_0);
				ConfiguredFeaturesShellDescriptorManager.u003cu003ec.u003cu003e9__3_0 = stackVariable5;
			}
			this._alwaysEnabledFeatures = Enumerable.ToArray<ShellFeature>(Enumerable.Where<ShellFeature>(stackVariable4, stackVariable5));
			return;
		}

		public Task<ShellDescriptor> GetShellDescriptorAsync()
		{
			if (this._shellDescriptor == null)
			{
				V_0 = new ConfiguredFeaturesShellDescriptorManager.ConfiguredFeatures();
				ConfigurationBinder.Bind(this._shellConfiguration, V_0);
				stackVariable10 = this._alwaysEnabledFeatures;
				stackVariable12 = V_0.get_Features();
				stackVariable13 = ConfiguredFeaturesShellDescriptorManager.u003cu003ec.u003cu003e9__4_0;
				if (stackVariable13 == null)
				{
					dummyVar0 = stackVariable13;
					stackVariable13 = new Func<string, ShellFeature>(ConfiguredFeaturesShellDescriptorManager.u003cu003ec.u003cu003e9, ConfiguredFeaturesShellDescriptorManager.u003cu003ec.u003cGetShellDescriptorAsyncu003eb__4_0);
					ConfiguredFeaturesShellDescriptorManager.u003cu003ec.u003cu003e9__4_0 = stackVariable13;
				}
				V_1 = Enumerable.Distinct<ShellFeature>(Enumerable.Concat<ShellFeature>(stackVariable10, Enumerable.Select<string, ShellFeature>(stackVariable12, stackVariable13)));
				stackVariable18 = new ShellDescriptor();
				stackVariable18.set_Features(Enumerable.ToList<ShellFeature>(V_1));
				this._shellDescriptor = stackVariable18;
			}
			return Task.FromResult<ShellDescriptor>(this._shellDescriptor);
		}

		public Task UpdateShellDescriptorAsync(int priorSerialNumber, IEnumerable<ShellFeature> enabledFeatures, IEnumerable<ShellParameter> parameters)
		{
			return Task.get_CompletedTask();
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
				this.u003cFeaturesu003ek__BackingField = new string[0];
				base();
				return;
			}
		}
	}
}