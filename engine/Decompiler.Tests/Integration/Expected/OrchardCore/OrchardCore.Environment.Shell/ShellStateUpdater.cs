using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Shell.State;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			base();
			this._settings = settings;
			this._stateManager = stateManager;
			this._extensionManager = extensionManager;
			this._featureEventHandlers = featureEventHandlers;
			this._logger = logger;
			return;
		}

		public async Task ApplyChanges()
		{
			V_0.u003cu003e4__this = this;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ShellStateUpdater.u003cApplyChangesu003ed__6>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
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