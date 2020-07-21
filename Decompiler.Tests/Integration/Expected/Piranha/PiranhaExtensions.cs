using Microsoft.Extensions.DependencyInjection;
using Piranha;
using System;
using System.Runtime.CompilerServices;

public static class PiranhaExtensions
{
	public static IServiceCollection AddPiranha(this IServiceCollection services, ServiceLifetime scope = 1)
	{
		services.Add(new ServiceDescriptor(Type.GetTypeFromHandle(// 
		// Current member / type: Microsoft.Extensions.DependencyInjection.IServiceCollection PiranhaExtensions::AddPiranha(Microsoft.Extensions.DependencyInjection.IServiceCollection,Microsoft.Extensions.DependencyInjection.ServiceLifetime)
		// Exception in: Microsoft.Extensions.DependencyInjection.IServiceCollection AddPiranha(Microsoft.Extensions.DependencyInjection.IServiceCollection,Microsoft.Extensions.DependencyInjection.ServiceLifetime)
		// Specified method is not supported.
		// 
		// mailto: JustDecompilePublicFeedback@telerik.com


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
		stackVariable1 = serviceBuilder.Services;
		stackVariable2 = PiranhaExtensions.u003cu003ec.u003cu003e9__2_0;
		if (stackVariable2 == null)
		{
			dummyVar0 = stackVariable2;
			stackVariable2 = new Func<ServiceDescriptor, bool>(PiranhaExtensions.u003cu003ec.u003cu003e9.u003cUseMemoryCacheu003eb__2_0);
			PiranhaExtensions.u003cu003ec.u003cu003e9__2_0 = stackVariable2;
		}
		if (!stackVariable1.Any<ServiceDescriptor>(stackVariable2))
		{
			throw new NotSupportedException("You need to register a IMemoryCache service in order to use Memory Cache in Piranha");
		}
		dummyVar1 = serviceBuilder.Services.AddPiranhaMemoryCache(false);
		return serviceBuilder;
	}
}