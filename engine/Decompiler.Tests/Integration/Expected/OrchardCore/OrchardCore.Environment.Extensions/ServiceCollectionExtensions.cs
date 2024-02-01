using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
			ServiceCollectionServiceExtensions.AddSingleton<IExtensionManager, ExtensionManager>(services);
			ServiceCollectionServiceExtensions.AddSingleton<ITypeFeatureProvider, TypeFeatureProvider>(services);
			ServiceCollectionServiceExtensions.AddSingleton<IFeaturesProvider, FeaturesProvider>(services);
			ServiceCollectionServiceExtensions.AddSingleton<IExtensionDependencyStrategy, ExtensionDependencyStrategy>(services);
			ServiceCollectionServiceExtensions.AddSingleton<IExtensionPriorityStrategy, ExtensionPriorityStrategy>(services);
			return services;
		}
	}
}