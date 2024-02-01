using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Shell.Builders;
using OrchardCore.Environment.Shell.Descriptor.Models;
using OrchardCore.Environment.Shell.Scope;
using OrchardCore.Environment.Shell.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OrchardCore.Environment.Shell
{
	public class ShellStateCoordinator : IShellDescriptorManagerEventHandler
	{
		private readonly IShellStateManager _stateManager;

		private readonly ILogger _logger;

		public ShellStateCoordinator(IShellStateManager stateManager, ILogger<ShellStateCoordinator> logger)
		{
			this._stateManager = stateManager;
			this._logger = logger;
		}

		private static bool FeatureIsChanging(ShellFeatureState shellFeatureState)
		{
			if (shellFeatureState.get_EnableState() == 1 || shellFeatureState.get_EnableState() == 3)
			{
				return true;
			}
			if (shellFeatureState.get_InstallState() != 1 && shellFeatureState.get_InstallState() != 3)
			{
				return false;
			}
			return true;
		}

		private void FireApplyChangesIfNeeded()
		{
			ShellScope.AddDeferredTask(async (ShellScope scope) => {
				IShellStateManager requiredService = ServiceProviderServiceExtensions.GetRequiredService<IShellStateManager>(scope.get_ServiceProvider());
				IShellStateUpdater shellStateUpdater = ServiceProviderServiceExtensions.GetRequiredService<IShellStateUpdater>(scope.get_ServiceProvider());
				ShellState shellStateAsync = await requiredService.GetShellStateAsync();
				while (shellStateAsync.get_Features().Any<ShellFeatureState>(new Func<ShellFeatureState, bool>(ShellStateCoordinator.FeatureIsChanging)))
				{
					if (this._logger.IsEnabled(2))
					{
						LoggerExtensions.LogInformation(this._logger, "Adding pending task 'ApplyChanges' for tenant '{TenantName}'", new object[] { scope.get_ShellContext().get_Settings().get_Name() });
					}
					await shellStateUpdater.ApplyChanges();
				}
			});
		}

		async Task OrchardCore.Environment.Shell.IShellDescriptorManagerEventHandler.ChangedAsync(ShellDescriptor descriptor, ShellSettings settings)
		{
			ShellState shellStateAsync = await this._stateManager.GetShellStateAsync();
			foreach (ShellFeature feature in descriptor.get_Features())
			{
				string id = feature.get_Id();
				ShellFeatureState shellFeatureState = shellStateAsync.get_Features().SingleOrDefault<ShellFeatureState>((ShellFeatureState f) => f.get_Id() == id);
				if (shellFeatureState == null)
				{
					ShellFeatureState shellFeatureState1 = new ShellFeatureState();
					shellFeatureState1.set_Id(id);
					shellFeatureState = shellFeatureState1;
				}
				if (!shellFeatureState.get_IsInstalled())
				{
					await this._stateManager.UpdateInstalledStateAsync(shellFeatureState, 1);
				}
				if (!shellFeatureState.get_IsEnabled())
				{
					await this._stateManager.UpdateEnabledStateAsync(shellFeatureState, 1);
				}
				shellFeatureState = null;
			}
			foreach (ShellFeatureState feature1 in shellStateAsync.get_Features())
			{
				string str = feature1.get_Id();
				if (descriptor.get_Features().Any<ShellFeature>((ShellFeature f) => f.get_Id() == str) || feature1.get_IsDisabled())
				{
					continue;
				}
				await this._stateManager.UpdateEnabledStateAsync(feature1, 3);
			}
			this.FireApplyChangesIfNeeded();
		}
	}
}