using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.AspNetCore.Builder
{
	public static class PoweredByOrchardCoreExtensions
	{
		public static IApplicationBuilder UsePoweredBy(this IApplicationBuilder app, bool enabled, string headerValue)
		{
			IPoweredByMiddlewareOptions requiredService = ServiceProviderServiceExtensions.GetRequiredService<IPoweredByMiddlewareOptions>(app.get_ApplicationServices());
			requiredService.Enabled = enabled;
			requiredService.HeaderValue = headerValue;
			return app;
		}

		public static IApplicationBuilder UsePoweredByOrchardCore(this IApplicationBuilder app, bool enabled)
		{
			ServiceProviderServiceExtensions.GetRequiredService<IPoweredByMiddlewareOptions>(app.get_ApplicationServices()).Enabled = enabled;
			return app;
		}
	}
}