using OrchardCore.Environment.Shell.Descriptor.Models;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class OrchardCoreBuilderExtensions
	{
		public static OrchardCoreBuilder AddBackgroundService(this OrchardCoreBuilder builder)
		{
			dummyVar0 = ServiceCollectionServiceExtensions.AddSingleton<IHostedService, ModularBackgroundService>(builder.get_ApplicationServices());
			return builder;
		}

		public static OrchardCoreBuilder AddGlobalFeatures(this OrchardCoreBuilder builder, params string[] featureIds)
		{
			V_0 = featureIds;
			V_1 = 0;
			while (V_1 < (int)V_0.Length)
			{
				V_2 = new OrchardCoreBuilderExtensions.u003cu003ec__DisplayClass0_0();
				V_2.featureId = V_0[V_1];
				dummyVar0 = ServiceCollectionServiceExtensions.AddTransient<ShellFeature>(builder.get_ApplicationServices(), new Func<IServiceProvider, ShellFeature>(V_2.u003cAddGlobalFeaturesu003eb__0));
				V_1 = V_1 + 1;
			}
			return builder;
		}

		public static OrchardCoreBuilder AddSetupFeatures(this OrchardCoreBuilder builder, params string[] featureIds)
		{
			V_0 = featureIds;
			V_1 = 0;
			while (V_1 < (int)V_0.Length)
			{
				V_2 = new OrchardCoreBuilderExtensions.u003cu003ec__DisplayClass2_0();
				V_2.featureId = V_0[V_1];
				dummyVar0 = ServiceCollectionServiceExtensions.AddTransient<ShellFeature>(builder.get_ApplicationServices(), new Func<IServiceProvider, ShellFeature>(V_2.u003cAddSetupFeaturesu003eb__0));
				V_1 = V_1 + 1;
			}
			return builder;
		}

		public static OrchardCoreBuilder AddTenantFeatures(this OrchardCoreBuilder builder, params string[] featureIds)
		{
			V_0 = new OrchardCoreBuilderExtensions.u003cu003ec__DisplayClass1_0();
			V_0.featureIds = featureIds;
			dummyVar0 = builder.ConfigureServices(new Action<IServiceCollection>(V_0.u003cAddTenantFeaturesu003eb__0), 0);
			return builder;
		}

		public static OrchardCoreBuilder WithFeatures(this OrchardCoreBuilder builder, params string[] featureIds)
		{
			V_0 = featureIds;
			V_1 = 0;
			while (V_1 < (int)V_0.Length)
			{
				V_2 = new OrchardCoreBuilderExtensions.u003cu003ec__DisplayClass4_0();
				V_2.featureId = V_0[V_1];
				dummyVar0 = ServiceCollectionServiceExtensions.AddTransient<ShellFeature>(builder.get_ApplicationServices(), new Func<IServiceProvider, ShellFeature>(V_2.u003cWithFeaturesu003eb__0));
				V_1 = V_1 + 1;
			}
			dummyVar1 = builder.get_ApplicationServices().AddSetFeaturesDescriptor();
			return builder;
		}

		public static OrchardCoreBuilder WithTenants(this OrchardCoreBuilder builder)
		{
			stackVariable1 = builder.get_ApplicationServices();
			dummyVar0 = ServiceCollectionServiceExtensions.AddSingleton<IShellsSettingsSources, ShellsSettingsSources>(stackVariable1);
			dummyVar1 = ServiceCollectionServiceExtensions.AddSingleton<IShellsConfigurationSources, ShellsConfigurationSources>(stackVariable1);
			dummyVar2 = ServiceCollectionServiceExtensions.AddSingleton<IShellConfigurationSources, ShellConfigurationSources>(stackVariable1);
			dummyVar3 = ServiceCollectionServiceExtensions.AddTransient<IConfigureOptions<ShellOptions>, ShellOptionsSetup>(stackVariable1);
			dummyVar4 = ServiceCollectionServiceExtensions.AddSingleton<IShellSettingsManager, ShellSettingsManager>(stackVariable1);
			stackVariable7 = builder;
			stackVariable8 = OrchardCoreBuilderExtensions.u003cu003ec.u003cu003e9__3_0;
			if (stackVariable8 == null)
			{
				dummyVar5 = stackVariable8;
				stackVariable8 = new Action<IServiceCollection>(OrchardCoreBuilderExtensions.u003cu003ec.u003cu003e9.u003cWithTenantsu003eb__3_0);
				OrchardCoreBuilderExtensions.u003cu003ec.u003cu003e9__3_0 = stackVariable8;
			}
			return stackVariable7.ConfigureServices(stackVariable8, 0);
		}
	}
}