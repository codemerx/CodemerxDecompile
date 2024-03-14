using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Newtonsoft.Json.Linq;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Environment.Shell.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

		public ShellSettingsManager(IConfiguration applicationConfiguration, IShellsConfigurationSources tenantsConfigSources, IShellConfigurationSources tenantConfigSources, IShellsSettingsSources settingsSources)
		{
			this._applicationConfiguration = applicationConfiguration;
			this._tenantsConfigSources = tenantsConfigSources;
			this._tenantConfigSources = tenantConfigSources;
			this._settingsSources = settingsSources;
		}

		public ShellSettings CreateDefaultSettings()
		{
			return new ShellSettings(new ShellConfiguration(this._configuration), new ShellConfiguration(this._configuration));
		}

		private async Task EnsureConfigurationAsync()
		{
			IConfigurationProvider[] array;
			if (this._configuration == null)
			{
				await this._semaphore.WaitAsync();
				try
				{
					if (this._configuration == null)
					{
						IConfigurationRoot configurationRoot = this._applicationConfiguration as IConfigurationRoot;
						if (configurationRoot != null)
						{
							IEnumerable<IConfigurationProvider> providers = configurationRoot.get_Providers();
							array = providers.Where<IConfigurationProvider>((IConfigurationProvider p) => {
								if (p is EnvironmentVariablesConfigurationProvider)
								{
									return true;
								}
								return p is CommandLineConfigurationProvider;
							}).ToArray<IConfigurationProvider>();
						}
						else
						{
							array = null;
						}
						IConfigurationProvider[] configurationProviderArray = array;
						IConfigurationBuilder configurationBuilder2 = await ShellsConfigurationSourcesExtensions.AddSourcesAsync(ChainedBuilderExtensions.AddConfiguration(new ConfigurationBuilder(), this._applicationConfiguration), this._tenantsConfigSources);
						if (configurationProviderArray.Count<IConfigurationProvider>() > 0)
						{
							ChainedBuilderExtensions.AddConfiguration(configurationBuilder2, new ConfigurationRoot(configurationProviderArray));
						}
						IConfigurationSection configurationSection = configurationBuilder2.Build().GetSection("OrchardCore");
						ShellSettingsManager shellSettingsManager = this;
						IEnumerable<IConfigurationSection> children = configurationSection.GetChildren();
						IEnumerable<IConfigurationSection> configurationSections = children.Where<IConfigurationSection>((IConfigurationSection section) => {
							TenantState tenantState;
							return Enum.TryParse<TenantState>(section.get_Item("State"), true, out tenantState);
						});
						shellSettingsManager._configuredTenants = (
							from section in configurationSections
							select section.get_Key()).Distinct<string>().ToArray<string>();
						this._configBuilderFactory = async (string tenant) => {
							IConfigurationBuilder configurationBuilder;
							await this._semaphore.WaitAsync();
							try
							{
								IConfigurationBuilder configurationBuilder1 = ChainedBuilderExtensions.AddConfiguration(new ConfigurationBuilder(), this._configuration);
								ChainedBuilderExtensions.AddConfiguration(configurationBuilder1, configurationSection.GetSection(tenant));
								configurationBuilder = await ShellConfigurationSourcesExtensions.AddSourcesAsync(configurationBuilder1, tenant, this._tenantConfigSources);
							}
							finally
							{
								this._semaphore.Release();
							}
							return configurationBuilder;
						};
						this._configuration = configurationSection;
						configurationProviderArray = null;
					}
					else
					{
						return;
					}
				}
				finally
				{
					this._semaphore.Release();
				}
			}
		}

		public async Task<IEnumerable<ShellSettings>> LoadSettingsAsync()
		{
			IEnumerable<ShellSettings> shellSettings;
			await this.EnsureConfigurationAsync();
			await this._semaphore.WaitAsync();
			try
			{
				IConfigurationRoot configurationRoot = await ShellsSettingsSourcesExtensions.AddSourcesAsync(new ConfigurationBuilder(), this._settingsSources).Build();
				IEnumerable<IConfigurationSection> children = configurationRoot.GetChildren();
				IEnumerable<string> key = 
					from section in children
					select section.get_Key();
				string[] array = this._configuredTenants.Concat<string>(key).Distinct<string>().ToArray<string>();
				List<ShellSettings> shellSettings1 = new List<ShellSettings>();
				string[] strArrays = array;
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					string str = strArrays[i];
					ShellConfiguration shellConfiguration = new ShellConfiguration(ChainedBuilderExtensions.AddConfiguration(ChainedBuilderExtensions.AddConfiguration(ChainedBuilderExtensions.AddConfiguration(new ConfigurationBuilder(), this._configuration), this._configuration.GetSection(str)), configurationRoot.GetSection(str)).Build());
					ShellSettings shellSetting = new ShellSettings(shellConfiguration, new ShellConfiguration(str, this._configBuilderFactory));
					shellSetting.set_Name(str);
					shellSettings1.Add(shellSetting);
				}
				shellSettings = shellSettings1;
			}
			finally
			{
				this._semaphore.Release();
			}
			return shellSettings;
		}

		public async Task<ShellSettings> LoadSettingsAsync(string tenant)
		{
			ShellSettings shellSetting;
			await this.EnsureConfigurationAsync();
			await this._semaphore.WaitAsync();
			try
			{
				IConfigurationRoot configurationRoot = await ShellsSettingsSourcesExtensions.AddSourcesAsync(new ConfigurationBuilder(), this._settingsSources).Build();
				ShellConfiguration shellConfiguration = new ShellConfiguration(ChainedBuilderExtensions.AddConfiguration(ChainedBuilderExtensions.AddConfiguration(ChainedBuilderExtensions.AddConfiguration(new ConfigurationBuilder(), this._configuration), this._configuration.GetSection(tenant)), configurationRoot.GetSection(tenant)).Build());
				ShellConfiguration shellConfiguration1 = new ShellConfiguration(tenant, this._configBuilderFactory);
				ShellSettings shellSetting1 = new ShellSettings(shellConfiguration, shellConfiguration1);
				shellSetting1.set_Name(tenant);
				shellSetting = shellSetting1;
			}
			finally
			{
				this._semaphore.Release();
			}
			return shellSetting;
		}

		public async Task SaveSettingsAsync(ShellSettings settings)
		{
			await this.EnsureConfigurationAsync();
			await this._semaphore.WaitAsync();
			try
			{
				if (settings == null)
				{
					throw new ArgumentNullException("settings");
				}
				IConfigurationRoot configurationRoot = ChainedBuilderExtensions.AddConfiguration(ChainedBuilderExtensions.AddConfiguration(new ConfigurationBuilder(), this._configuration), this._configuration.GetSection(settings.get_Name())).Build();
				ShellSettings shellSetting = new ShellSettings();
				shellSetting.set_Name(settings.get_Name());
				ShellSettings shellSetting1 = shellSetting;
				ConfigurationBinder.Bind(configurationRoot, shellSetting1);
				JObject jObject = JObject.FromObject(shellSetting1);
				JObject jObject1 = JObject.FromObject(settings);
				foreach (KeyValuePair<string, JToken> keyValuePair in jObject)
				{
					string str = jObject1.Value<string>(keyValuePair.Key);
					if (str == jObject.Value<string>(keyValuePair.Key))
					{
						jObject1.set_Item(keyValuePair.Key, null);
					}
					else
					{
						jObject1.set_Item(keyValuePair.Key, str);
					}
				}
				jObject1.Remove("Name");
				await this._settingsSources.SaveAsync(settings.get_Name(), jObject1.ToObject<Dictionary<string, string>>());
				JObject jObject2 = new JObject();
				IEnumerable<IConfigurationSection> children = settings.get_ShellConfiguration().GetChildren();
				IConfigurationSection[] array = (
					from s in children
					where !s.GetChildren().Any<IConfigurationSection>()
					select s).ToArray<IConfigurationSection>();
				for (int i = 0; i < (int)array.Length; i++)
				{
					IConfigurationSection configurationSection = array[i];
					if (settings.get_Item(configurationSection.get_Key()) == configurationRoot.get_Item(configurationSection.get_Key()))
					{
						jObject2.set_Item(configurationSection.get_Key(), null);
					}
					else
					{
						jObject2.set_Item(configurationSection.get_Key(), settings.get_Item(configurationSection.get_Key()));
					}
				}
				jObject2.Remove("Name");
				await this._tenantConfigSources.SaveAsync(settings.get_Name(), jObject2.ToObject<Dictionary<string, string>>());
				configurationRoot = null;
			}
			finally
			{
				this._semaphore.Release();
			}
		}
	}
}