using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrchardCore.Environment.Shell.Descriptor;
using OrchardCore.Environment.Shell.Descriptor.Settings;
using System;
using System.Runtime.CompilerServices;

namespace OrchardCore.Environment.Shell
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddAllFeaturesDescriptor(this IServiceCollection services)
		{
			ServiceCollectionServiceExtensions.AddScoped<IShellDescriptorManager, AllFeaturesShellDescriptorManager>(services);
			return services;
		}

		public static IServiceCollection AddHostingShellServices(this IServiceCollection services)
		{
			ServiceCollectionServiceExtensions.AddSingleton<IShellHost, ShellHost>(services);
			ServiceCollectionServiceExtensions.AddSingleton<IShellDescriptorManagerEventHandler>(services, (IServiceProvider sp) => ServiceProviderServiceExtensions.GetRequiredService<IShellHost>(sp));
			ServiceCollectionDescriptorExtensions.TryAddSingleton<IShellSettingsManager, SingleShellSettingsManager>(services);
			ServiceCollectionServiceExtensions.AddTransient<IConfigureOptions<ShellOptions>, ShellOptionsSetup>(services);
			ServiceCollectionServiceExtensions.AddSingleton<IShellContextFactory, ShellContextFactory>(services);
			ServiceCollectionServiceExtensions.AddSingleton<ICompositionStrategy, CompositionStrategy>(services);
			ServiceCollectionServiceExtensions.AddSingleton<IShellContainerFactory, ShellContainerFactory>(services);
			ServiceCollectionServiceExtensions.AddSingleton<IRunningShellTable, RunningShellTable>(services);
			return services;
		}

		public static IServiceCollection AddSetFeaturesDescriptor(this IServiceCollection services)
		{
			ServiceCollectionServiceExtensions.AddSingleton<IShellDescriptorManager>(services, (IServiceProvider sp) => new SetFeaturesShellDescriptorManager(ServiceProviderServiceExtensions.GetServices<ShellFeature>(sp)));
			return services;
		}
	}
}