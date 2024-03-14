using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Shell.State;
using OrchardCore.Modules;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OrchardCore.Environment.Shell
{
	public class ShellStateUpdater : IShellStateUpdater
	{
		private readonly ShellSettings _settings;

		private readonly IShellStateManager _stateManager;

		private readonly IExtensionManager _extensionManager;

		private readonly IEnumerable<IFeatureEventHandler> _featureEventHandlers;

		private readonly ILogger _logger;

		public ShellStateUpdater(ShellSettings settings, IShellStateManager stateManager, IExtensionManager extensionManager, IEnumerable<IFeatureEventHandler> featureEventHandlers, ILogger<ShellStateUpdater> logger)
		{
			this._settings = settings;
			this._stateManager = stateManager;
			this._extensionManager = extensionManager;
			this._featureEventHandlers = featureEventHandlers;
			this._logger = logger;
		}

		public async Task ApplyChanges()
		{
			var variable = null;
			if (this._logger.IsEnabled(2))
			{
				ILogger logger = this._logger;
				object[] name = new object[] { this._settings.get_Name() };
				LoggerExtensions.LogInformation(logger, "Applying changes for for tenant '{TenantName}'", name);
			}
			IEnumerable<FeatureEntry> featureEntries = await this._extensionManager.LoadFeaturesAsync();
			ShellState shellStateAsync = await this._stateManager.GetShellStateAsync();
			var collection = 
				from fe in featureEntries
				select new { Feature = fe, FeatureDescriptor = fe.get_FeatureInfo(), FeatureState = shellStateAsync.get_Features().FirstOrDefault<ShellFeatureState>((ShellFeatureState s) => s.get_Id() == fe.get_FeatureInfo().get_Id()) };
			var array = (
				from entry in collection
				where entry.FeatureState != null
				select entry).ToArray();
			List<ShellFeatureState> features = shellStateAsync.get_Features();
			var variable1 = array;
			IEnumerable<ShellFeatureState> shellFeatureStates = features.Except<ShellFeatureState>(
				from entry in (IEnumerable<u003cu003ef__AnonymousType4<FeatureEntry, IFeatureInfo, ShellFeatureState>>)variable1
				select entry.FeatureState);
			var variable2 = array;
			IEnumerable<ShellFeatureState> shellFeatureStates1 = shellFeatureStates;
			var array1 = ((IEnumerable<u003cu003ef__AnonymousType4<FeatureEntry, IFeatureInfo, ShellFeatureState>>)variable2).Concat(shellFeatureStates1.Select((ShellFeatureState featureState) => {
				InternalFeatureInfo internalFeatureInfo = new InternalFeatureInfo(featureState.get_Id(), new InternalExtensionInfo(featureState.get_Id()));
				return new { Feature = new NonCompiledFeatureEntry(internalFeatureInfo), FeatureDescriptor = internalFeatureInfo, FeatureState = featureState };
			})).ToArray();
			var collection1 = array1.Reverse();
			foreach (var variable in 
				from entry in collection1
				where entry.FeatureState.get_EnableState() == 3
				select entry)
			{
				if (this._logger.IsEnabled(2))
				{
					ILogger logger1 = this._logger;
					object[] id = new object[] { variable.Feature.get_FeatureInfo().get_Id() };
					LoggerExtensions.LogInformation(logger1, "Disabling feature '{FeatureName}'", id);
				}
				IEnumerable<IFeatureEventHandler> featureEventHandlers = this._featureEventHandlers;
				await InvokeExtensions.InvokeAsync<IFeatureEventHandler, IFeatureInfo>(featureEventHandlers, (IFeatureEventHandler handler, IFeatureInfo featureInfo) => handler.DisablingAsync(featureInfo), variable.Feature.get_FeatureInfo(), this._logger);
				await this._stateManager.UpdateEnabledStateAsync(variable.FeatureState, 4);
				IEnumerable<IFeatureEventHandler> featureEventHandlers1 = this._featureEventHandlers;
				await InvokeExtensions.InvokeAsync<IFeatureEventHandler, IFeatureInfo>(featureEventHandlers1, (IFeatureEventHandler handler, IFeatureInfo featureInfo) => handler.DisabledAsync(featureInfo), variable.Feature.get_FeatureInfo(), this._logger);
			}
			var collection2 = array1.Reverse();
			foreach (var variable3 in 
				from entry in collection2
				where entry.FeatureState.get_InstallState() == 3
				select entry)
			{
				if (this._logger.IsEnabled(2))
				{
					ILogger logger2 = this._logger;
					object[] objArray = new object[] { variable3.Feature.get_FeatureInfo().get_Id() };
					LoggerExtensions.LogInformation(logger2, "Uninstalling feature '{FeatureName}'", objArray);
				}
				IEnumerable<IFeatureEventHandler> featureEventHandlers2 = this._featureEventHandlers;
				await InvokeExtensions.InvokeAsync<IFeatureEventHandler, IFeatureInfo>(featureEventHandlers2, (IFeatureEventHandler handler, IFeatureInfo featureInfo) => handler.UninstallingAsync(featureInfo), variable3.Feature.get_FeatureInfo(), this._logger);
				await this._stateManager.UpdateInstalledStateAsync(variable3.FeatureState, 4);
				IEnumerable<IFeatureEventHandler> featureEventHandlers3 = this._featureEventHandlers;
				await InvokeExtensions.InvokeAsync<IFeatureEventHandler, IFeatureInfo>(featureEventHandlers3, (IFeatureEventHandler handler, IFeatureInfo featureInfo) => handler.UninstalledAsync(featureInfo), variable3.Feature.get_FeatureInfo(), this._logger);
			}
			var variable4 = array1;
			foreach (var variable5 in 
				from entry in (IEnumerable<u003cu003ef__AnonymousType4<FeatureEntry, IFeatureInfo, ShellFeatureState>>)variable4
				where ShellStateUpdater.IsRising(entry.FeatureState)
				select entry)
			{
				if (variable5.FeatureState.get_InstallState() == 1)
				{
					if (this._logger.IsEnabled(2))
					{
						ILogger logger3 = this._logger;
						object[] id1 = new object[] { variable5.Feature.get_FeatureInfo().get_Id() };
						LoggerExtensions.LogInformation(logger3, "Installing feature '{FeatureName}'", id1);
					}
					IEnumerable<IFeatureEventHandler> featureEventHandlers4 = this._featureEventHandlers;
					await InvokeExtensions.InvokeAsync<IFeatureEventHandler, IFeatureInfo>(featureEventHandlers4, (IFeatureEventHandler handler, IFeatureInfo featureInfo) => handler.InstallingAsync(featureInfo), variable5.Feature.get_FeatureInfo(), this._logger);
					await this._stateManager.UpdateInstalledStateAsync(variable5.FeatureState, 2);
					IEnumerable<IFeatureEventHandler> featureEventHandlers5 = this._featureEventHandlers;
					await InvokeExtensions.InvokeAsync<IFeatureEventHandler, IFeatureInfo>(featureEventHandlers5, (IFeatureEventHandler handler, IFeatureInfo featureInfo) => handler.InstalledAsync(featureInfo), variable5.Feature.get_FeatureInfo(), this._logger);
				}
				if (variable5.FeatureState.get_EnableState() == 1)
				{
					if (this._logger.IsEnabled(2))
					{
						ILogger logger4 = this._logger;
						object[] objArray1 = new object[] { variable5.Feature.get_FeatureInfo().get_Id() };
						LoggerExtensions.LogInformation(logger4, "Enabling feature '{FeatureName}'", objArray1);
					}
					IEnumerable<IFeatureEventHandler> featureEventHandlers6 = this._featureEventHandlers;
					await InvokeExtensions.InvokeAsync<IFeatureEventHandler, IFeatureInfo>(featureEventHandlers6, (IFeatureEventHandler handler, IFeatureInfo featureInfo) => handler.EnablingAsync(featureInfo), variable5.Feature.get_FeatureInfo(), this._logger);
					await this._stateManager.UpdateEnabledStateAsync(variable5.FeatureState, 2);
					IEnumerable<IFeatureEventHandler> featureEventHandlers7 = this._featureEventHandlers;
					await InvokeExtensions.InvokeAsync<IFeatureEventHandler, IFeatureInfo>(featureEventHandlers7, (IFeatureEventHandler handler, IFeatureInfo featureInfo) => handler.EnabledAsync(featureInfo), variable5.Feature.get_FeatureInfo(), this._logger);
				}
			}
		}

		private static bool IsRising(ShellFeatureState state)
		{
			if (state.get_InstallState() == 1)
			{
				return true;
			}
			return state.get_EnableState() == 1;
		}
	}
}