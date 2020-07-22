using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;

namespace OrchardCore.Environment.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddExtensionManager(this IServiceCollection services)
		{
			ServiceCollectionDescriptorExtensions.TryAddTransient<IFeatureHash, FeatureHash>(services);
			return services;
		}

		public static IServiceCollection AddExtensionManagerHost(this IServiceCollection services)
		{
			dummyVar0 = ServiceCollectionServiceExtensions.AddSingleton<IExtensionManager, ExtensionManager>(services);
			dummyVar1 = ServiceCollectionServiceExtensions.AddSingleton<ITypeFeatureProvider, TypeFeatureProvider>(services);
			dummyVar2 = ServiceCollectionServiceExtensions.AddSingleton<IFeaturesProvider, FeaturesProvider>(services);
			dummyVar3 = ServiceCollectionServiceExtensions.AddSingleton<IExtensionDependencyStrategy, ExtensionDependencyStrategy>(services);
			dummyVar4 = ServiceCollectionServiceExtensions.AddSingleton<IExtensionPriorityStrategy, ExtensionPriorityStrategy>(services);
			return services;
		}
	}
}