using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
	public class ShellContextFactory : IShellContextFactory
	{
		private readonly ICompositionStrategy _compositionStrategy;

		private readonly IShellContainerFactory _shellContainerFactory;

		private readonly IEnumerable<ShellFeature> _shellFeatures;

		private readonly ILogger _logger;

		public ShellContextFactory(ICompositionStrategy compositionStrategy, IShellContainerFactory shellContainerFactory, IEnumerable<ShellFeature> shellFeatures, ILogger<ShellContextFactory> logger)
		{
			base();
			this._compositionStrategy = compositionStrategy;
			this._shellContainerFactory = shellContainerFactory;
			this._shellFeatures = shellFeatures;
			this._logger = logger;
			return;
		}

		public async Task<ShellContext> CreateDescribedContextAsync(ShellSettings settings, ShellDescriptor shellDescriptor)
		{
			V_0.u003cu003e4__this = this;
			V_0.settings = settings;
			V_0.shellDescriptor = shellDescriptor;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<ShellContext>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ShellContextFactory.u003cCreateDescribedContextAsyncu003ed__7>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private ShellDescriptor MinimumShellDescriptor()
		{
			stackVariable0 = new ShellDescriptor();
			stackVariable0.set_SerialNumber(-1);
			stackVariable0.set_Features(new List<ShellFeature>(this._shellFeatures));
			stackVariable0.set_Parameters(new List<ShellParameter>());
			return stackVariable0;
		}

		async Task<ShellContext> OrchardCore.Environment.Shell.Builders.IShellContextFactory.CreateSetupContextAsync(ShellSettings settings)
		{
			V_0.u003cu003e4__this = this;
			V_0.settings = settings;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<ShellContext>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ShellContextFactory.u003cOrchardCoreu002dEnvironmentu002dShellu002dBuildersu002dIShellContextFactoryu002dCreateSetupContextAsyncu003ed__6>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		async Task<ShellContext> OrchardCore.Environment.Shell.Builders.IShellContextFactory.CreateShellContextAsync(ShellSettings settings)
		{
			V_0.u003cu003e4__this = this;
			V_0.settings = settings;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<ShellContext>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ShellContextFactory.u003cOrchardCoreu002dEnvironmentu002dShellu002dBuildersu002dIShellContextFactoryu002dCreateShellContextAsyncu003ed__5>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}