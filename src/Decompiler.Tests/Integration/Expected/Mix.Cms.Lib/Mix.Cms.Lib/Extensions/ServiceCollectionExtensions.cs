using Microsoft.Extensions.DependencyInjection;
using Mix.Cms.Lib.Controllers;
using Mix.Heart.NetCore;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddGenerateApis(this IServiceCollection services)
		{
			Mix.Heart.NetCore.ServiceCollectionExtensions.AddGeneratedRestApi(services, Assembly.GetExecutingAssembly(), typeof(BaseRestApiController<,,>));
			SignalRDependencyInjectionExtensions.AddSignalR(services);
			return services;
		}

		public static IServiceCollection AddMyGraphQL(this IServiceCollection services)
		{
			SignalRDependencyInjectionExtensions.AddSignalR(services);
			return services;
		}
	}
}