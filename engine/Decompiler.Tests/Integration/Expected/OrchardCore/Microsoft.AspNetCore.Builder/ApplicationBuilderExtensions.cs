using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using OrchardCore.Modules;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.AspNetCore.Builder
{
	public static class ApplicationBuilderExtensions
	{
		public static IApplicationBuilder UseOrchardCore(this IApplicationBuilder app, Action<IApplicationBuilder> configure = null)
		{
			IHostEnvironment requiredService = ServiceProviderServiceExtensions.GetRequiredService<IHostEnvironment>(app.get_ApplicationServices());
			IApplicationContext applicationContext = ServiceProviderServiceExtensions.GetRequiredService<IApplicationContext>(app.get_ApplicationServices());
			requiredService.set_ContentRootFileProvider(new CompositeFileProvider(new IFileProvider[] { new ModuleEmbeddedFileProvider(applicationContext), requiredService.get_ContentRootFileProvider() }));
			ServiceProviderServiceExtensions.GetRequiredService<IWebHostEnvironment>(app.get_ApplicationServices()).set_ContentRootFileProvider(requiredService.get_ContentRootFileProvider());
			UseMiddlewareExtensions.UseMiddleware<PoweredByMiddleware>(app, Array.Empty<object>());
			UseMiddlewareExtensions.UseMiddleware<ModularTenantContainerMiddleware>(app, Array.Empty<object>());
			if (configure != null)
			{
				configure(app);
			}
			UseMiddlewareExtensions.UseMiddleware<ModularTenantRouterMiddleware>(app, new object[] { app.get_ServerFeatures() });
			return app;
		}
	}
}