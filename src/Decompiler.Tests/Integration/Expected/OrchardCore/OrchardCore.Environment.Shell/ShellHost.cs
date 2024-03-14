using Microsoft.Extensions.Logging;
using OrchardCore;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Shell.Builders;
using OrchardCore.Environment.Shell.Descriptor.Models;
using OrchardCore.Environment.Shell.Scope;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OrchardCore.Environment.Shell
{
	public class ShellHost : IShellHost, IShellDescriptorManagerEventHandler, IDisposable
	{
		private const int ReloadShellMaxRetriesCount = 9;

		private readonly IShellSettingsManager _shellSettingsManager;

		private readonly IShellContextFactory _shellContextFactory;

		private readonly IRunningShellTable _runningShellTable;

		private readonly IExtensionManager _extensionManager;

		private readonly ILogger _logger;

		private bool _initialized;

		private ConcurrentDictionary<string, ShellContext> _shellContexts = new ConcurrentDictionary<string, ShellContext>();

		private readonly ConcurrentDictionary<string, SemaphoreSlim> _shellSemaphores = new ConcurrentDictionary<string, SemaphoreSlim>();

		private SemaphoreSlim _initializingSemaphore = new SemaphoreSlim(1);

		public ShellHost(IShellSettingsManager shellSettingsManager, IShellContextFactory shellContextFactory, IRunningShellTable runningShellTable, IExtensionManager extensionManager, ILogger<ShellHost> logger)
		{
			this._shellSettingsManager = shellSettingsManager;
			this._shellContextFactory = shellContextFactory;
			this._runningShellTable = runningShellTable;
			this._extensionManager = extensionManager;
			this._logger = logger;
		}

		private void AddAndRegisterShell(ShellContext context)
		{
			if (this._shellContexts.TryAdd(context.get_Settings().get_Name(), context) && this.CanRegisterShell(context))
			{
				this.RegisterShellSettings(context.get_Settings());
			}
		}

		private bool CanCreateShell(ShellSettings shellSettings)
		{
			if (shellSettings.get_State() == 2 || shellSettings.get_State() == null || shellSettings.get_State() == 1)
			{
				return true;
			}
			return shellSettings.get_State() == 3;
		}

		private bool CanRegisterShell(ShellContext context)
		{
			if (this.CanRegisterShell(context.get_Settings()))
			{
				return true;
			}
			if (this._logger.IsEnabled(1))
			{
				LoggerExtensions.LogDebug(this._logger, "Skipping shell context registration for tenant '{TenantName}'", new object[] { context.get_Settings().get_Name() });
			}
			return false;
		}

		private bool CanRegisterShell(ShellSettings shellSettings)
		{
			if (shellSettings.get_State() == 2 || shellSettings.get_State() == null)
			{
				return true;
			}
			return shellSettings.get_State() == 1;
		}

		private bool CanReleaseShell(ShellSettings settings)
		{
			ShellContext shellContext;
			if (settings.get_State() != 3)
			{
				return true;
			}
			if (!this._shellContexts.TryGetValue(settings.get_Name(), out shellContext))
			{
				return false;
			}
			return shellContext.get_ActiveScopes() == 0;
		}

		public Task ChangedAsync(ShellDescriptor descriptor, ShellSettings settings)
		{
			return this.ReleaseShellContextAsync(settings);
		}

		private Task<ShellContext> CreateSetupContextAsync(ShellSettings defaultSettings)
		{
			if (this._logger.IsEnabled(1))
			{
				LoggerExtensions.LogDebug(this._logger, "Creating shell context for root setup.", Array.Empty<object>());
			}
			if (defaultSettings == null)
			{
				ShellSettings shellSetting = this._shellSettingsManager.CreateDefaultSettings();
				shellSetting.set_Name("Default");
				shellSetting.set_State(0);
				defaultSettings = shellSetting;
			}
			return this._shellContextFactory.CreateSetupContextAsync(defaultSettings);
		}

		private Task<ShellContext> CreateShellContextAsync(ShellSettings settings)
		{
			if (settings.get_State() == null)
			{
				if (this._logger.IsEnabled(1))
				{
					LoggerExtensions.LogDebug(this._logger, "Creating shell context for tenant '{TenantName}' setup", new object[] { settings.get_Name() });
				}
				return this._shellContextFactory.CreateSetupContextAsync(settings);
			}
			if (settings.get_State() == 3)
			{
				if (this._logger.IsEnabled(1))
				{
					LoggerExtensions.LogDebug(this._logger, "Creating disabled shell context for tenant '{TenantName}'", new object[] { settings.get_Name() });
				}
				ShellContext shellContext = new ShellContext();
				shellContext.set_Settings(settings);
				return Task.FromResult<ShellContext>(shellContext);
			}
			if (settings.get_State() != 2 && settings.get_State() != 1)
			{
				throw new InvalidOperationException(string.Concat("Unexpected shell state for ", settings.get_Name()));
			}
			if (this._logger.IsEnabled(1))
			{
				LoggerExtensions.LogDebug(this._logger, "Creating shell context for tenant '{TenantName}'", new object[] { settings.get_Name() });
			}
			return this._shellContextFactory.CreateShellContextAsync(settings);
		}

		public void Dispose()
		{
			foreach (ShellContext shellContext in this.ListShellContexts())
			{
				shellContext.Dispose();
			}
		}

		public IEnumerable<ShellSettings> GetAllSettings()
		{
			return 
				from s in this.ListShellContexts()
				select s.get_Settings();
		}

		public async Task<ShellContext> GetOrCreateShellContextAsync(ShellSettings settings)
		{
			ShellContext shellContext;
			ShellContext shellContext1 = null;
			while (shellContext1 == null)
			{
				if (!this._shellContexts.TryGetValue(settings.get_Name(), out shellContext1))
				{
					ConcurrentDictionary<string, SemaphoreSlim> strs = this._shellSemaphores;
					string str = settings.get_Name();
					SemaphoreSlim orAdd = strs.GetOrAdd(str, (string name) => new SemaphoreSlim(1));
					await orAdd.WaitAsync();
					try
					{
						if (!this._shellContexts.TryGetValue(settings.get_Name(), out shellContext1))
						{
							shellContext1 = await this.CreateShellContextAsync(settings);
							this.AddAndRegisterShell(shellContext1);
						}
					}
					finally
					{
						orAdd.Release();
						this._shellSemaphores.TryRemove(settings.get_Name(), out orAdd);
					}
					orAdd = null;
				}
				if (!shellContext1.get_Released())
				{
					continue;
				}
				this._shellContexts.TryRemove(settings.get_Name(), out shellContext);
				shellContext1 = null;
			}
			return shellContext1;
		}

		public async Task<ShellScope> GetScopeAsync(ShellSettings settings)
		{
			ShellContext orCreateShellContextAsync;
			ShellContext shellContext;
			ShellScope shellScope = null;
			while (shellScope == null)
			{
				if (!this._shellContexts.TryGetValue(settings.get_Name(), out orCreateShellContextAsync))
				{
					orCreateShellContextAsync = await this.GetOrCreateShellContextAsync(settings);
				}
				shellScope = orCreateShellContextAsync.CreateScope();
				if (shellScope != null)
				{
					continue;
				}
				this._shellContexts.TryRemove(settings.get_Name(), out shellContext);
			}
			return shellScope;
		}

		public async Task InitializeAsync()
		{
			if (!this._initialized)
			{
				await this._initializingSemaphore.WaitAsync();
				try
				{
					if (!this._initialized)
					{
						await this.PreCreateAndRegisterShellsAsync();
					}
				}
				finally
				{
					this._initialized = true;
					this._initializingSemaphore.Release();
				}
			}
		}

		public IEnumerable<ShellContext> ListShellContexts()
		{
			return this._shellContexts.Values.ToArray<ShellContext>();
		}

		private async Task PreCreateAndRegisterShellsAsync()
		{
			bool state;
			if (this._logger.IsEnabled(2))
			{
				LoggerExtensions.LogInformation(this._logger, "Start creation of shells", Array.Empty<object>());
			}
			Task<IEnumerable<FeatureEntry>> task = this._extensionManager.LoadFeaturesAsync();
			IEnumerable<ShellSettings> shellSettings = await this._shellSettingsManager.LoadSettingsAsync();
			ShellSettings[] array = shellSettings.Where<ShellSettings>(new Func<ShellSettings, bool>(this.CanCreateShell)).ToArray<ShellSettings>();
			ShellSettings[] shellSettingsArray = array;
			ShellSettings shellSetting = shellSettingsArray.FirstOrDefault<ShellSettings>((ShellSettings s) => s.get_Name() == "Default");
			ShellSettings[] shellSettingsArray1 = array;
			ShellSettings[] shellSettingsArray2 = new ShellSettings[] { shellSetting };
			ShellSettings[] array1 = shellSettingsArray1.Except<ShellSettings>(shellSettingsArray2).ToArray<ShellSettings>();
			await task;
			ShellSettings shellSetting1 = shellSetting;
			if (shellSetting1 != null)
			{
				state = shellSetting1.get_State() != 2;
			}
			else
			{
				state = true;
			}
			if (state)
			{
				this.AddAndRegisterShell(await this.CreateSetupContextAsync(shellSetting));
				array = array1;
			}
			if (array.Length != 0)
			{
				ShellSettings[] shellSettingsArray3 = array;
				for (int i = 0; i < (int)shellSettingsArray3.Length; i++)
				{
					ShellSettings shellSetting2 = shellSettingsArray3[i];
					ShellContext.PlaceHolder placeHolder = new ShellContext.PlaceHolder();
					placeHolder.set_Settings(shellSetting2);
					this.AddAndRegisterShell(placeHolder);
				}
			}
			if (this._logger.IsEnabled(2))
			{
				LoggerExtensions.LogInformation(this._logger, "Done pre-creating and registering shells", Array.Empty<object>());
			}
		}

		private void RegisterShellSettings(ShellSettings settings)
		{
			if (this._logger.IsEnabled(1))
			{
				LoggerExtensions.LogDebug(this._logger, "Registering shell context for tenant '{TenantName}'", new object[] { settings.get_Name() });
			}
			this._runningShellTable.Add(settings);
		}

		public Task ReleaseShellContextAsync(ShellSettings settings)
		{
			ShellContext shellContext;
			if (!this.CanReleaseShell(settings))
			{
				return Task.CompletedTask;
			}
			if (this._shellContexts.TryRemove(settings.get_Name(), out shellContext))
			{
				shellContext.Release();
			}
			ConcurrentDictionary<string, ShellContext> strs = this._shellContexts;
			string name = shellContext.get_Settings().get_Name();
			ShellContext.PlaceHolder placeHolder = new ShellContext.PlaceHolder();
			placeHolder.set_Settings(settings);
			strs.TryAdd(name, placeHolder);
			return Task.CompletedTask;
		}

		public async Task ReloadShellContextAsync(ShellSettings settings)
		{
			ShellContext shellContext;
			if (this.CanReleaseShell(settings))
			{
				if (settings.get_State() != 1)
				{
					settings = await this._shellSettingsManager.LoadSettingsAsync(settings.get_Name());
				}
				int num = 0;
				while (num < 9)
				{
					num++;
					if (this._shellContexts.TryRemove(settings.get_Name(), out shellContext))
					{
						this._runningShellTable.Remove(settings);
						shellContext.Release();
					}
					ConcurrentDictionary<string, ShellContext> strs = this._shellContexts;
					string name = settings.get_Name();
					ShellContext.PlaceHolder placeHolder = new ShellContext.PlaceHolder();
					placeHolder.set_Settings(settings);
					if (!strs.TryAdd(name, placeHolder))
					{
						continue;
					}
					if (this.CanRegisterShell(settings))
					{
						this._runningShellTable.Add(settings);
					}
					if (settings.get_State() != 1)
					{
						string identifier = settings.get_Identifier();
						settings = await this._shellSettingsManager.LoadSettingsAsync(settings.get_Name());
						if (settings.get_Identifier() != identifier)
						{
							identifier = null;
						}
						else
						{
							return;
						}
					}
					else
					{
						return;
					}
				}
				throw new ShellHostReloadException(string.Concat("Unable to reload the tenant '", settings.get_Name(), "' as too many concurrent processes are trying to do so."));
			}
			else
			{
				this._runningShellTable.Remove(settings);
			}
		}

		public bool TryGetSettings(string name, out ShellSettings settings)
		{
			ShellContext shellContext;
			if (!this._shellContexts.TryGetValue(name, out shellContext))
			{
				settings = null;
				return false;
			}
			settings = shellContext.get_Settings();
			return true;
		}

		public async Task UpdateShellSettingsAsync(ShellSettings settings)
		{
			settings.set_Identifier(IdGenerator.GenerateId());
			await this._shellSettingsManager.SaveSettingsAsync(settings);
			await this.ReloadShellContextAsync(settings);
		}
	}
}