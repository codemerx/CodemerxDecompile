using System;
using System.Runtime.CompilerServices;

namespace Microsoft.AspNetCore.Builder
{
	public static class PoweredByOrchardCoreExtensions
	{
		public static IApplicationBuilder UsePoweredBy(this IApplicationBuilder app, bool enabled, string headerValue)
		{
			stackVariable2 = ServiceProviderServiceExtensions.GetRequiredService<IPoweredByMiddlewareOptions>(app.get_ApplicationServices());
			stackVariable2.set_Enabled(enabled);
			stackVariable2.set_HeaderValue(headerValue);
			return app;
		}

		public static IApplicationBuilder UsePoweredByOrchardCore(this IApplicationBuilder app, bool enabled)
		{
			ServiceProviderServiceExtensions.GetRequiredService<IPoweredByMiddlewareOptions>(app.get_ApplicationServices()).set_Enabled(enabled);
			return app;
		}
	}
}