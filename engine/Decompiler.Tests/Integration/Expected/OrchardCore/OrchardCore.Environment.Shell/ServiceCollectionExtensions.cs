using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell.Descriptor;
using System;
using System.Runtime.CompilerServices;

namespace OrchardCore.Environment.Shell
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddAllFeaturesDescriptor(this IServiceCollection services)
		{
			dummyVar0 = ServiceCollectionServiceExtensions.AddScoped<IShellDescriptorManager, AllFeaturesShellDescriptorManager>(services);
			return services;
		}

		public static IServiceCollection AddHostingShellServices(this IServiceCollection services)
		{
			dummyVar0 = ServiceCollectionServiceExtensions.AddSingleton<IShellHost, ShellHost>(services);
			stackVariable2 = services;
			stackVariable3 = OrchardCore.Environment.Shell.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__0_0;
			if (stackVariable3 == null)
			{
				dummyVar1 = stackVariable3;
				stackVariable3 = new Func<IServiceProvider, IShellDescriptorManagerEventHandler>(OrchardCore.Environment.Shell.ServiceCollectionExtensions.u003cu003ec.u003cu003e9, OrchardCore.Environment.Shell.ServiceCollectionExtensions.u003cu003ec.u003cAddHostingShellServicesu003eb__0_0);
				OrchardCore.Environment.Shell.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__0_0 = stackVariable3;
			}
			dummyVar2 = ServiceCollectionServiceExtensions.AddSingleton<IShellDescriptorManagerEventHandler>(stackVariable2, stackVariable3);
			ServiceCollectionDescriptorExtensions.TryAddSingleton<IShellSettingsManager, SingleShellSettingsManager>(services);
			dummyVar3 = ServiceCollectionServiceExtensions.AddTransient<IConfigureOptions<ShellOptions>, ShellOptionsSetup>(services);
			dummyVar4 = ServiceCollectionServiceExtensions.AddSingleton<IShellContextFactory, ShellContextFactory>(services);
			dummyVar5 = ServiceCollectionServiceExtensions.AddSingleton<ICompositionStrategy, CompositionStrategy>(services);
			dummyVar6 = ServiceCollectionServiceExtensions.AddSingleton<IShellContainerFactory, ShellContainerFactory>(services);
			dummyVar7 = ServiceCollectionServiceExtensions.AddSingleton<IRunningShellTable, RunningShellTable>(services);
			return services;
		}

		public static IServiceCollection AddSetFeaturesDescriptor(this IServiceCollection services)
		{
			stackVariable0 = services;
			stackVariable1 = OrchardCore.Environment.Shell.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__2_0;
			if (stackVariable1 == null)
			{
				dummyVar0 = stackVariable1;
				stackVariable1 = new Func<IServiceProvider, IShellDescriptorManager>(OrchardCore.Environment.Shell.ServiceCollectionExtensions.u003cu003ec.u003cu003e9, OrchardCore.Environment.Shell.ServiceCollectionExtensions.u003cu003ec.u003cAddSetFeaturesDescriptoru003eb__2_0);
				OrchardCore.Environment.Shell.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__2_0 = stackVariable1;
			}
			dummyVar1 = ServiceCollectionServiceExtensions.AddSingleton<IShellDescriptorManager>(stackVariable0, stackVariable1);
			return services;
		}
	}
}