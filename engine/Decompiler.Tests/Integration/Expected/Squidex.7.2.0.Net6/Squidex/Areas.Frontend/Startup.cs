using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.StaticFiles.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Squidex.Areas.Frontend.Middlewares;
using Squidex.Hosting.Web;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Frontend
{
	[Nullable(0)]
	[NullableContext(1)]
	public static class Startup
	{
		private static bool IsDevServer(this HttpContext context)
		{
			PathString path = context.get_Request().get_Path();
			return path.StartsWithSegments("/ws", StringComparison.OrdinalIgnoreCase);
		}

		private static bool IsSpaFile(this HttpContext context)
		{
			if (!WebExtensions.IsIndex(context) && Path.HasExtension(context.get_Request().get_Path()))
			{
				return false;
			}
			return !context.IsDevServer();
		}

		public static void UseFrontend(this IApplicationBuilder app)
		{
			IWebHostEnvironment requiredService = ServiceProviderServiceExtensions.GetRequiredService<IWebHostEnvironment>(app.get_ApplicationServices());
			IFileProvider webRootFileProvider = requiredService.get_WebRootFileProvider();
			UseMiddlewareExtensions.UseMiddleware<EmbedMiddleware>(app, Array.Empty<object>());
			if (!HostEnvironmentEnvExtensions.IsDevelopment(requiredService))
			{
				webRootFileProvider = new CompositeFileProvider(new IFileProvider[] { webRootFileProvider, new PhysicalFileProvider(Path.Combine(requiredService.get_WebRootPath(), "build")) });
			}
			MapExtensions.Map(app, "/squid.svg", (IApplicationBuilder builder) => UseMiddlewareExtensions.UseMiddleware<SquidMiddleware>(builder, Array.Empty<object>()));
			UseMiddlewareExtensions.UseMiddleware<NotifoMiddleware>(app, Array.Empty<object>());
			UseWhenExtensions.UseWhen(app, (HttpContext c) => c.IsSpaFile(), (IApplicationBuilder builder) => UseMiddlewareExtensions.UseMiddleware<SetupMiddleware>(builder, Array.Empty<object>()));
			UseWhenExtensions.UseWhen(app, (HttpContext c) => {
				if (c.IsSpaFile())
				{
					return true;
				}
				return WebExtensions.IsHtmlPath(c);
			}, (IApplicationBuilder builder) => {
				IApplicationBuilder applicationBuilder = builder;
				HtmlTransformOptions htmlTransformOption = new HtmlTransformOptions();
				htmlTransformOption.set_Transform((string html, HttpContext context) => new ValueTask<string>(html.AddOptions(context)));
				WebExtensions.UseHtmlTransform(applicationBuilder, htmlTransformOption);
			});
			UseExtensions.Use(app, (HttpContext context, Func<Task> next) => next());
			app.UseSquidexStaticFiles(webRootFileProvider);
			if (!HostEnvironmentEnvExtensions.IsDevelopment(requiredService))
			{
				WebExtensions.UsePathOverride(app, "/index.html");
				app.UseSquidexStaticFiles(webRootFileProvider);
				return;
			}
			SpaApplicationBuilderExtensions.UseSpa(app, (ISpaBuilder builder) => SpaProxyingExtensions.UseProxyToSpaDevelopmentServer(builder, "https://localhost:3000"));
		}

		private static void UseSquidexStaticFiles(this IApplicationBuilder app, IFileProvider fileProvider)
		{
			IApplicationBuilder applicationBuilder = app;
			StaticFileOptions staticFileOption = new StaticFileOptions();
			staticFileOption.set_OnPrepareResponse((StaticFileResponseContext context) => {
				HttpResponse response = context.get_Context().get_Response();
				if (!string.IsNullOrWhiteSpace(context.get_Context().get_Request().get_QueryString().ToString()))
				{
					response.get_Headers().set_Item(HeaderNames.CacheControl, "max-age=5184000");
					return;
				}
				if (string.Equals(response.get_ContentType(), "text/html", StringComparison.OrdinalIgnoreCase))
				{
					response.get_Headers().set_Item(HeaderNames.CacheControl, "no-cache");
				}
			});
			staticFileOption.set_FileProvider(fileProvider);
			StaticFileExtensions.UseStaticFiles(applicationBuilder, staticFileOption);
		}
	}
}