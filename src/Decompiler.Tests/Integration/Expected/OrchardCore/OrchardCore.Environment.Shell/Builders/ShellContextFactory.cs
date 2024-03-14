using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Builders.Models;
using OrchardCore.Environment.Shell.Descriptor;
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
			this._compositionStrategy = compositionStrategy;
			this._shellContainerFactory = shellContainerFactory;
			this._shellFeatures = shellFeatures;
			this._logger = logger;
		}

		public async Task<ShellContext> CreateDescribedContextAsync(ShellSettings settings, ShellDescriptor shellDescriptor)
		{
			if (this._logger.IsEnabled(1))
			{
				ILogger logger = this._logger;
				object[] name = new object[] { settings.get_Name() };
				LoggerExtensions.LogDebug(logger, "Creating described context for tenant '{TenantName}'", name);
			}
			await settings.EnsureConfigurationAsync();
			ShellBlueprint shellBlueprint = await this._compositionStrategy.ComposeAsync(settings, shellDescriptor);
			IServiceProvider serviceProvider = this._shellContainerFactory.CreateContainer(settings, shellBlueprint);
			ShellContext shellContext = new ShellContext();
			shellContext.set_Settings(settings);
			shellContext.set_Blueprint(shellBlueprint);
			shellContext.set_ServiceProvider(serviceProvider);
			return shellContext;
		}

		private ShellDescriptor MinimumShellDescriptor()
		{
			ShellDescriptor shellDescriptor = new ShellDescriptor();
			shellDescriptor.set_SerialNumber(-1);
			shellDescriptor.set_Features(new List<ShellFeature>(this._shellFeatures));
			shellDescriptor.set_Parameters(new List<ShellParameter>());
			return shellDescriptor;
		}

		async Task<ShellContext> OrchardCore.Environment.Shell.Builders.IShellContextFactory.CreateSetupContextAsync(ShellSettings settings)
		{
			if (this._logger.IsEnabled(1))
			{
				LoggerExtensions.LogDebug(this._logger, "No shell settings available. Creating shell context for setup", Array.Empty<object>());
			}
			return await this.CreateDescribedContextAsync(settings, this.MinimumShellDescriptor());
		}

		async Task<ShellContext> OrchardCore.Environment.Shell.Builders.IShellContextFactory.CreateShellContextAsync(ShellSettings settings)
		{
			ShellContext shellContext;
			ShellDescriptor shellDescriptorAsync;
			if (this._logger.IsEnabled(2))
			{
				ILogger logger = this._logger;
				object[] name = new object[] { settings.get_Name() };
				LoggerExtensions.LogInformation(logger, "Creating shell context for tenant '{TenantName}'", name);
			}
			ShellContext shellContext1 = await this.CreateDescribedContextAsync(settings, this.MinimumShellDescriptor());
			using (IServiceScope serviceScope = ServiceProviderServiceExtensions.CreateScope(shellContext1.get_ServiceProvider()))
			{
				shellDescriptorAsync = await ServiceProviderServiceExtensions.GetService<IShellDescriptorManager>(serviceScope.get_ServiceProvider()).GetShellDescriptorAsync();
			}
			serviceScope = null;
			if (shellDescriptorAsync == null)
			{
				shellContext = shellContext1;
			}
			else
			{
				shellContext1.Release();
				shellContext = await this.CreateDescribedContextAsync(settings, shellDescriptorAsync);
			}
			return shellContext;
		}
	}
}