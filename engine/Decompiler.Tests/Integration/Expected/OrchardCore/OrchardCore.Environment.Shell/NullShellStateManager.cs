using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Shell.State;
using System;
using System.Threading.Tasks;

namespace OrchardCore.Environment.Shell
{
	public class NullShellStateManager : IShellStateManager
	{
		private readonly ILogger _logger;

		public NullShellStateManager(ILogger<NullShellStateManager> logger)
		{
			this._logger = logger;
		}

		public Task<ShellState> GetShellStateAsync()
		{
			return Task.FromResult<ShellState>(new ShellState());
		}

		public Task UpdateEnabledStateAsync(ShellFeatureState featureState, ShellFeatureState.State value)
		{
			if (this._logger.IsEnabled(1))
			{
				LoggerExtensions.LogDebug(this._logger, "Feature '{FeatureName}' EnableState changed from '{FeatureState}' to '{FeatureState}'", new object[] { featureState.get_Id(), featureState.get_EnableState(), value });
			}
			return Task.CompletedTask;
		}

		public Task UpdateInstalledStateAsync(ShellFeatureState featureState, ShellFeatureState.State value)
		{
			if (this._logger.IsEnabled(1))
			{
				LoggerExtensions.LogDebug(this._logger, "Feature '{FeatureName}' InstallState changed from '{FeatureState}' to '{FeatureState}'", new object[] { featureState.get_Id(), featureState.get_InstallState(), value });
			}
			return Task.CompletedTask;
		}
	}
}