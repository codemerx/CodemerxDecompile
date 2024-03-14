using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public static class PiranhaExtensions
{
	public static IServiceCollection AddPiranha(this IServiceCollection services, ServiceLifetime scope = 1)
	{
		services.Add(new ServiceDescriptor(typeof(IContentFactory), typeof(ContentFactory), 0));
		services.Add(new ServiceDescriptor(typeof(IApi), typeof(Api), scope));
		services.Add(new ServiceDescriptor(typeof(Config), typeof(Config), scope));
		return services;
	}

	public static IServiceCollection AddPiranhaDistributedCache(this IServiceCollection services)
	{
		return ServiceCollectionServiceExtensions.AddSingleton<ICache, DistributedCache>(services);
	}

	public static IServiceCollection AddPiranhaMemoryCache(this IServiceCollection services, bool clone = false)
	{
		if (clone)
		{
			return ServiceCollectionServiceExtensions.AddSingleton<ICache, MemoryCacheWithClone>(services);
		}
		return ServiceCollectionServiceExtensions.AddSingleton<ICache, MemoryCache>(services);
	}

	public static IServiceCollection AddPiranhaSimpleCache(this IServiceCollection services, bool clone = false)
	{
		if (clone)
		{
			return ServiceCollectionServiceExtensions.AddSingleton<ICache, SimpleCacheWithClone>(services);
		}
		return ServiceCollectionServiceExtensions.AddSingleton<ICache, SimpleCache>(services);
	}

	public static PiranhaServiceBuilder UseMemoryCache(this PiranhaServiceBuilder serviceBuilder, bool clone = false)
	{
		if (!serviceBuilder.Services.Any<ServiceDescriptor>((ServiceDescriptor s) => s.get_ServiceType() == typeof(IMemoryCache)))
		{
			throw new NotSupportedException("You need to register a IMemoryCache service in order to use Memory Cache in Piranha");
		}
		serviceBuilder.Services.AddPiranhaMemoryCache(false);
		return serviceBuilder;
	}
}