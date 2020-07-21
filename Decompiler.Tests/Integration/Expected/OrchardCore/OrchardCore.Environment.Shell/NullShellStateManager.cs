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
			base();
			this._logger = logger;
			return;
		}

		public Task<ShellState> GetShellStateAsync()
		{
			return Task.FromResult<ShellState>(new ShellState());
		}

		public Task UpdateEnabledStateAsync(ShellFeatureState featureState, ShellFeatureState.State value)
		{
			if (this._logger.IsEnabled(1))
			{
				stackVariable6 = this._logger;
				stackVariable9 = new object[3];
				stackVariable9[0] = featureState.get_Id();
				stackVariable9[1] = featureState.get_EnableState();
				stackVariable9[2] = value;
				LoggerExtensions.LogDebug(stackVariable6, "Feature '{FeatureName}' EnableState changed from '{FeatureState}' to '{FeatureState}'", stackVariable9);
			}
			return Task.get_CompletedTask();
		}

		public Task UpdateInstalledStateAsync(ShellFeatureState featureState, ShellFeatureState.State value)
		{
			if (this._logger.IsEnabled(1))
			{
				stackVariable6 = this._logger;
				stackVariable9 = new object[3];
				stackVariable9[0] = featureState.get_Id();
				stackVariable9[1] = featureState.get_InstallState();
				stackVariable9[2] = value;
				LoggerExtensions.LogDebug(stackVariable6, "Feature '{FeatureName}' InstallState changed from '{FeatureState}' to '{FeatureState}'", stackVariable9);
			}
			return Task.get_CompletedTask();
		}
	}
}