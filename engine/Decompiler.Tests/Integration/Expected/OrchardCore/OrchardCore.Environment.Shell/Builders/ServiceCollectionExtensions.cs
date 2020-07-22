using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;

namespace OrchardCore.Environment.Shell.Builders
{
	internal static class ServiceCollectionExtensions
	{
		public static IServiceCollection CloneSingleton(this IServiceCollection services, ServiceDescriptor parent, object implementationInstance)
		{
			services.Add(new ClonedSingletonDescriptor(parent, implementationInstance));
			return services;
		}

		public static IServiceCollection CloneSingleton(this IServiceCollection collection, ServiceDescriptor parent, Func<IServiceProvider, object> implementationFactory)
		{
			collection.Add(new ClonedSingletonDescriptor(parent, implementationFactory));
			return collection;
		}
	}
}