using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Shell.Descriptor.Models;
using OrchardCore.Environment.Shell.Scope;
using OrchardCore.Environment.Shell.State;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			base();
			this._stateManager = stateManager;
			this._logger = logger;
			return;
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
			ShellScope.AddDeferredTask(new Func<ShellScope, Task>(this.u003cFireApplyChangesIfNeededu003eb__4_0));
			return;
		}

		async Task OrchardCore.Environment.Shell.IShellDescriptorManagerEventHandler.ChangedAsync(ShellDescriptor descriptor, ShellSettings settings)
		{
			V_0.u003cu003e4__this = this;
			V_0.descriptor = descriptor;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ShellStateCoordinator.u003cOrchardCoreu002dEnvironmentu002dShellu002dIShellDescriptorManagerEventHandleru002dChangedAsyncu003ed__3>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}