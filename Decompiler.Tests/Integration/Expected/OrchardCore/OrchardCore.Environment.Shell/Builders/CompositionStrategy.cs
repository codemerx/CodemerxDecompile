using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Builders.Models;
using OrchardCore.Environment.Shell.Descriptor.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OrchardCore.Environment.Shell.Builders
{
	public class CompositionStrategy : ICompositionStrategy
	{
		private readonly IExtensionManager _extensionManager;

		private readonly ILogger _logger;

		public CompositionStrategy(IExtensionManager extensionManager, ILogger<CompositionStrategy> logger)
		{
			base();
			this._extensionManager = extensionManager;
			this._logger = logger;
			return;
		}

		public async Task<ShellBlueprint> ComposeAsync(ShellSettings settings, ShellDescriptor descriptor)
		{
			V_0.u003cu003e4__this = this;
			V_0.settings = settings;
			V_0.descriptor = descriptor;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<ShellBlueprint>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<CompositionStrategy.u003cComposeAsyncu003ed__3>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}