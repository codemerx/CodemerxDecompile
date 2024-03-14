using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Descriptor.Models;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class OrchardCoreBuilderExtensions
	{
		public static OrchardCoreBuilder AddBackgroundService(this OrchardCoreBuilder builder)
		{
			ServiceCollectionServiceExtensions.AddSingleton<IHostedService, ModularBackgroundService>(builder.get_ApplicationServices());
			return builder;
		}

		public static OrchardCoreBuilder AddGlobalFeatures(this OrchardCoreBuilder builder, params string[] featureIds)
		{
			string[] strArrays = featureIds;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i];
				ServiceCollectionServiceExtensions.AddTransient<ShellFeature>(builder.get_ApplicationServices(), (IServiceProvider sp) => new ShellFeature(str, true));
			}
			return builder;
		}

		public static OrchardCoreBuilder AddSetupFeatures(this OrchardCoreBuilder builder, params string[] featureIds)
		{
			string[] strArrays = featureIds;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i];
				ServiceCollectionServiceExtensions.AddTransient<ShellFeature>(builder.get_ApplicationServices(), (IServiceProvider sp) => new ShellFeature(str, false));
			}
			return builder;
		}

		public static OrchardCoreBuilder AddTenantFeatures(this OrchardCoreBuilder builder, params string[] featureIds)
		{
			builder.ConfigureServices((IServiceCollection services) => {
				string[] strArrays = featureIds;
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					string str = strArrays[i];
					ServiceCollectionServiceExtensions.AddTransient<ShellFeature>(services, (IServiceProvider sp) => new ShellFeature(str, true));
				}
			}, 0);
			return builder;
		}

		public static OrchardCoreBuilder WithFeatures(this OrchardCoreBuilder builder, params string[] featureIds)
		{
			string[] strArrays = featureIds;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i];
				ServiceCollectionServiceExtensions.AddTransient<ShellFeature>(builder.get_ApplicationServices(), (IServiceProvider sp) => new ShellFeature(str, false));
			}
			builder.get_ApplicationServices().AddSetFeaturesDescriptor();
			return builder;
		}

		public static OrchardCoreBuilder WithTenants(this OrchardCoreBuilder builder)
		{
			IServiceCollection applicationServices = builder.get_ApplicationServices();
			ServiceCollectionServiceExtensions.AddSingleton<IShellsSettingsSources, ShellsSettingsSources>(applicationServices);
			ServiceCollectionServiceExtensions.AddSingleton<IShellsConfigurationSources, ShellsConfigurationSources>(applicationServices);
			ServiceCollectionServiceExtensions.AddSingleton<IShellConfigurationSources, ShellConfigurationSources>(applicationServices);
			ServiceCollectionServiceExtensions.AddTransient<IConfigureOptions<ShellOptions>, ShellOptionsSetup>(applicationServices);
			ServiceCollectionServiceExtensions.AddSingleton<IShellSettingsManager, ShellSettingsManager>(applicationServices);
			return builder.ConfigureServices((IServiceCollection s) => ServiceCollectionServiceExtensions.AddScoped<IShellDescriptorManager, ConfiguredFeaturesShellDescriptorManager>(s), 0);
		}
	}
}