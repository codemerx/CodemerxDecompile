using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Shell.Builders;
using OrchardCore.Environment.Shell.Descriptor.Models;
using OrchardCore.Environment.Shell.Scope;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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

		private ConcurrentDictionary<string, ShellContext> _shellContexts;

		private readonly ConcurrentDictionary<string, SemaphoreSlim> _shellSemaphores;

		private SemaphoreSlim _initializingSemaphore;

		public ShellHost(IShellSettingsManager shellSettingsManager, IShellContextFactory shellContextFactory, IRunningShellTable runningShellTable, IExtensionManager extensionManager, ILogger<ShellHost> logger)
		{
			this._shellContexts = new ConcurrentDictionary<string, ShellContext>();
			this._shellSemaphores = new ConcurrentDictionary<string, SemaphoreSlim>();
			this._initializingSemaphore = new SemaphoreSlim(1);
			base();
			this._shellSettingsManager = shellSettingsManager;
			this._shellContextFactory = shellContextFactory;
			this._runningShellTable = runningShellTable;
			this._extensionManager = extensionManager;
			this._logger = logger;
			return;
		}

		private void AddAndRegisterShell(ShellContext context)
		{
			if (this._shellContexts.TryAdd(context.get_Settings().get_Name(), context) && this.CanRegisterShell(context))
			{
				this.RegisterShellSettings(context.get_Settings());
			}
			return;
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
				stackVariable11 = this._logger;
				stackVariable14 = new object[1];
				stackVariable14[0] = context.get_Settings().get_Name();
				LoggerExtensions.LogDebug(stackVariable11, "Skipping shell context registration for tenant '{TenantName}'", stackVariable14);
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
			if (settings.get_State() != 3)
			{
				return true;
			}
			if (!this._shellContexts.TryGetValue(settings.get_Name(), out V_0))
			{
				return false;
			}
			return V_0.get_ActiveScopes() == 0;
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
				stackVariable11 = this._shellSettingsManager.CreateDefaultSettings();
				stackVariable11.set_Name("Default");
				stackVariable11.set_State(0);
				defaultSettings = stackVariable11;
			}
			return this._shellContextFactory.CreateSetupContextAsync(defaultSettings);
		}

		private Task<ShellContext> CreateShellContextAsync(ShellSettings settings)
		{
			if (settings.get_State() == null)
			{
				if (this._logger.IsEnabled(1))
				{
					stackVariable56 = this._logger;
					stackVariable59 = new object[1];
					stackVariable59[0] = settings.get_Name();
					LoggerExtensions.LogDebug(stackVariable56, "Creating shell context for tenant '{TenantName}' setup", stackVariable59);
				}
				return this._shellContextFactory.CreateSetupContextAsync(settings);
			}
			if (settings.get_State() == 3)
			{
				if (this._logger.IsEnabled(1))
				{
					stackVariable40 = this._logger;
					stackVariable43 = new object[1];
					stackVariable43[0] = settings.get_Name();
					LoggerExtensions.LogDebug(stackVariable40, "Creating disabled shell context for tenant '{TenantName}'", stackVariable43);
				}
				stackVariable36 = new ShellContext();
				stackVariable36.set_Settings(settings);
				return Task.FromResult<ShellContext>(stackVariable36);
			}
			if (settings.get_State() != 2 && settings.get_State() != 1)
			{
				throw new InvalidOperationException(string.Concat("Unexpected shell state for ", settings.get_Name()));
			}
			if (this._logger.IsEnabled(1))
			{
				stackVariable17 = this._logger;
				stackVariable20 = new object[1];
				stackVariable20[0] = settings.get_Name();
				LoggerExtensions.LogDebug(stackVariable17, "Creating shell context for tenant '{TenantName}'", stackVariable20);
			}
			return this._shellContextFactory.CreateShellContextAsync(settings);
		}

		public void Dispose()
		{
			V_0 = this.ListShellContexts().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_0.get_Current().Dispose();
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			return;
		}

		public IEnumerable<ShellSettings> GetAllSettings()
		{
			stackVariable1 = this.ListShellContexts();
			stackVariable2 = ShellHost.u003cu003ec.u003cu003e9__20_0;
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable2 = new Func<ShellContext, ShellSettings>(ShellHost.u003cu003ec.u003cu003e9.u003cGetAllSettingsu003eb__20_0);
				ShellHost.u003cu003ec.u003cu003e9__20_0 = stackVariable2;
			}
			return stackVariable1.Select<ShellContext, ShellSettings>(stackVariable2);
		}

		public async Task<ShellContext> GetOrCreateShellContextAsync(ShellSettings settings)
		{
			V_0.u003cu003e4__this = this;
			V_0.settings = settings;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<ShellContext>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ShellHost.u003cGetOrCreateShellContextAsyncu003ed__12>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<ShellScope> GetScopeAsync(ShellSettings settings)
		{
			V_0.u003cu003e4__this = this;
			V_0.settings = settings;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<ShellScope>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ShellHost.u003cGetScopeAsyncu003ed__13>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task InitializeAsync()
		{
			V_0.u003cu003e4__this = this;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ShellHost.u003cInitializeAsyncu003ed__11>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public IEnumerable<ShellContext> ListShellContexts()
		{
			return this._shellContexts.get_Values().ToArray<ShellContext>();
		}

		private async Task PreCreateAndRegisterShellsAsync()
		{
			V_0.u003cu003e4__this = this;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ShellHost.u003cPreCreateAndRegisterShellsAsyncu003ed__21>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private void RegisterShellSettings(ShellSettings settings)
		{
			if (this._logger.IsEnabled(1))
			{
				stackVariable8 = this._logger;
				stackVariable11 = new object[1];
				stackVariable11[0] = settings.get_Name();
				LoggerExtensions.LogDebug(stackVariable8, "Registering shell context for tenant '{TenantName}'", stackVariable11);
			}
			this._runningShellTable.Add(settings);
			return;
		}

		public Task ReleaseShellContextAsync(ShellSettings settings)
		{
			if (!this.CanReleaseShell(settings))
			{
				return Task.get_CompletedTask();
			}
			if (this._shellContexts.TryRemove(settings.get_Name(), out V_0))
			{
				V_0.Release();
			}
			stackVariable10 = this._shellContexts;
			stackVariable13 = V_0.get_Settings().get_Name();
			stackVariable14 = new ShellContext.PlaceHolder();
			stackVariable14.set_Settings(settings);
			dummyVar0 = stackVariable10.TryAdd(stackVariable13, stackVariable14);
			return Task.get_CompletedTask();
		}

		public async Task ReloadShellContextAsync(ShellSettings settings)
		{
			V_0.u003cu003e4__this = this;
			V_0.settings = settings;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ShellHost.u003cReloadShellContextAsyncu003ed__16>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public bool TryGetSettings(string name, out ShellSettings settings)
		{
			if (!this._shellContexts.TryGetValue(name, out V_0))
			{
				settings = null;
				return false;
			}
			settings = V_0.get_Settings();
			return true;
		}

		public async Task UpdateShellSettingsAsync(ShellSettings settings)
		{
			V_0.u003cu003e4__this = this;
			V_0.settings = settings;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ShellHost.u003cUpdateShellSettingsAsyncu003ed__14>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}