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
			V_0 = ServiceProviderServiceExtensions.GetRequiredService<IHostEnvironment>(app.get_ApplicationServices());
			V_1 = ServiceProviderServiceExtensions.GetRequiredService<IApplicationContext>(app.get_ApplicationServices());
			stackVariable8 = new IFileProvider[2];
			stackVariable8[0] = new ModuleEmbeddedFileProvider(V_1);
			stackVariable8[1] = V_0.get_ContentRootFileProvider();
			V_0.set_ContentRootFileProvider(new CompositeFileProvider(stackVariable8));
			ServiceProviderServiceExtensions.GetRequiredService<IWebHostEnvironment>(app.get_ApplicationServices()).set_ContentRootFileProvider(V_0.get_ContentRootFileProvider());
			dummyVar0 = UseMiddlewareExtensions.UseMiddleware<PoweredByMiddleware>(app, Array.Empty<object>());
			dummyVar1 = UseMiddlewareExtensions.UseMiddleware<ModularTenantContainerMiddleware>(app, Array.Empty<object>());
			if (configure != null)
			{
				configure.Invoke(app);
			}
			stackVariable30 = new object[1];
			stackVariable30[0] = app.get_ServerFeatures();
			dummyVar2 = UseMiddlewareExtensions.UseMiddleware<ModularTenantRouterMiddleware>(app, stackVariable30);
			return app;
		}
	}
}