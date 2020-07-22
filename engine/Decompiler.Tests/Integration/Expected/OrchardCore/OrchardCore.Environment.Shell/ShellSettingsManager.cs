using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Environment.Shell.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OrchardCore.Environment.Shell
{
	public class ShellSettingsManager : IShellSettingsManager
	{
		private readonly IConfiguration _applicationConfiguration;

		private readonly IShellsConfigurationSources _tenantsConfigSources;

		private readonly IShellConfigurationSources _tenantConfigSources;

		private readonly IShellsSettingsSources _settingsSources;

		private IConfiguration _configuration;

		private IEnumerable<string> _configuredTenants;

		private Func<string, Task<IConfigurationBuilder>> _configBuilderFactory;

		private readonly SemaphoreSlim _semaphore;

		public ShellSettingsManager(IConfiguration applicationConfiguration, IShellsConfigurationSources tenantsConfigSources, IShellConfigurationSources tenantConfigSources, IShellsSettingsSources settingsSources)
		{
			this._semaphore = new SemaphoreSlim(1);
			base();
			this._applicationConfiguration = applicationConfiguration;
			this._tenantsConfigSources = tenantsConfigSources;
			this._tenantConfigSources = tenantConfigSources;
			this._settingsSources = settingsSources;
			return;
		}

		public ShellSettings CreateDefaultSettings()
		{
			return new ShellSettings(new ShellConfiguration(this._configuration), new ShellConfiguration(this._configuration));
		}

		private async Task EnsureConfigurationAsync()
		{
			V_0.u003cu003e4__this = this;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ShellSettingsManager.u003cEnsureConfigurationAsyncu003ed__13>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<IEnumerable<ShellSettings>> LoadSettingsAsync()
		{
			V_0.u003cu003e4__this = this;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<IEnumerable<ShellSettings>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ShellSettingsManager.u003cLoadSettingsAsyncu003ed__10>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<ShellSettings> LoadSettingsAsync(string tenant)
		{
			V_0.u003cu003e4__this = this;
			V_0.tenant = tenant;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<ShellSettings>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ShellSettingsManager.u003cLoadSettingsAsyncu003ed__11>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task SaveSettingsAsync(ShellSettings settings)
		{
			V_0.u003cu003e4__this = this;
			V_0.settings = settings;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ShellSettingsManager.u003cSaveSettingsAsyncu003ed__12>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}