using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Net.Http.Headers;
using Squidex.Infrastructure.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.Config.Web
{
	[Nullable(0)]
	[NullableContext(1)]
	public static class WebExtensions
	{
		public static IApplicationBuilder UseSquidexCacheKeys(this IApplicationBuilder app)
		{
			UseMiddlewareExtensions.UseMiddleware<CachingKeysMiddleware>(app, Array.Empty<object>());
			return app;
		}

		public static void UseSquidexCors(this IApplicationBuilder app)
		{
			CorsMiddlewareExtensions.UseCors(app, (CorsPolicyBuilder builder) => builder.SetIsOriginAllowed((string x) => true).AllowCredentials().AllowAnyMethod().AllowAnyHeader());
		}

		public static IApplicationBuilder UseSquidexExceptionHandling(this IApplicationBuilder app)
		{
			UseMiddlewareExtensions.UseMiddleware<RequestExceptionMiddleware>(app, Array.Empty<object>());
			return app;
		}

		public static IApplicationBuilder UseSquidexHealthCheck(this IApplicationBuilder app)
		{
			IJsonSerializer requiredService = ServiceProviderServiceExtensions.GetRequiredService<IJsonSerializer>(app.get_ApplicationServices());
			Func<HttpContext, HealthReport, Task> func = (HttpContext httpContext, HealthReport report) => {
				IReadOnlyDictionary<string, HealthReportEntry> entries = report.get_Entries();
				Func<KeyValuePair<string, HealthReportEntry>, string> u003cu003e9_61 = Squidex.Config.Web.WebExtensions.u003cu003ec.u003cu003e9__6_1;
				if (u003cu003e9_61 == null)
				{
					u003cu003e9_61 = (KeyValuePair<string, HealthReportEntry> x) => x.Key;
					Squidex.Config.Web.WebExtensions.u003cu003ec.u003cu003e9__6_1 = u003cu003e9_61;
				}
				var u003cu003e9_62 = Squidex.Config.Web.WebExtensions.u003cu003ec.u003cu003e9__6_2;
				if (u003cu003e9_62 == null)
				{
					u003cu003e9_62 = (KeyValuePair<string, HealthReportEntry> x) => {
						HealthReportEntry value = x.Value;
						return new { Data = (value.get_Data().Count > 0 ? new Dictionary<string, object>(value.get_Data()) : null), Description = value.get_Description(), Duration = value.get_Duration(), Status = value.get_Status() };
					};
					Squidex.Config.Web.WebExtensions.u003cu003ec.u003cu003e9__6_2 = u003cu003e9_62;
				}
				var variable = new { Entries = entries.ToDictionary(u003cu003e9_61, u003cu003e9_62), Status = report.get_Status(), TotalDuration = report.get_TotalDuration() };
				string str = requiredService.Serialize(variable, false);
				httpContext.get_Response().get_Headers().set_Item(HeaderNames.ContentType, "text/json");
				return HttpResponseWritingExtensions.WriteAsync(httpContext.get_Response(), str, new CancellationToken());
			};
			IApplicationBuilder applicationBuilder = app;
			PathString pathString = "/readiness";
			HealthCheckOptions healthCheckOption = new HealthCheckOptions();
			healthCheckOption.set_Predicate((HealthCheckRegistration check) => !check.get_Tags().Contains("background"));
			healthCheckOption.set_ResponseWriter(func);
			HealthCheckApplicationBuilderExtensions.UseHealthChecks(applicationBuilder, pathString, healthCheckOption);
			IApplicationBuilder applicationBuilder1 = app;
			PathString pathString1 = "/healthz";
			HealthCheckOptions healthCheckOption1 = new HealthCheckOptions();
			healthCheckOption1.set_Predicate((HealthCheckRegistration check) => check.get_Tags().Contains("node"));
			healthCheckOption1.set_ResponseWriter(func);
			HealthCheckApplicationBuilderExtensions.UseHealthChecks(applicationBuilder1, pathString1, healthCheckOption1);
			IApplicationBuilder applicationBuilder2 = app;
			PathString pathString2 = "/cluster-healthz";
			HealthCheckOptions healthCheckOption2 = new HealthCheckOptions();
			healthCheckOption2.set_Predicate((HealthCheckRegistration check) => check.get_Tags().Contains("cluster"));
			healthCheckOption2.set_ResponseWriter(func);
			HealthCheckApplicationBuilderExtensions.UseHealthChecks(applicationBuilder2, pathString2, healthCheckOption2);
			IApplicationBuilder applicationBuilder3 = app;
			PathString pathString3 = "/background-healthz";
			HealthCheckOptions healthCheckOption3 = new HealthCheckOptions();
			healthCheckOption3.set_Predicate((HealthCheckRegistration check) => check.get_Tags().Contains("background"));
			healthCheckOption3.set_ResponseWriter(func);
			HealthCheckApplicationBuilderExtensions.UseHealthChecks(applicationBuilder3, pathString3, healthCheckOption3);
			return app;
		}

		public static IApplicationBuilder UseSquidexLocalCache(this IApplicationBuilder app)
		{
			UseMiddlewareExtensions.UseMiddleware<LocalCacheMiddleware>(app, Array.Empty<object>());
			return app;
		}

		public static IApplicationBuilder UseSquidexLocalization(this IApplicationBuilder app)
		{
			string[] strArrays = new string[] { "en", "nl", "it", "zh" };
			RequestLocalizationOptions requestLocalizationOption = (new RequestLocalizationOptions()).SetDefaultCulture(strArrays[0]).AddSupportedCultures(strArrays).AddSupportedUICultures(strArrays);
			ApplicationBuilderExtensions.UseRequestLocalization(app, requestLocalizationOption);
			return app;
		}

		public static IApplicationBuilder UseSquidexLogging(this IApplicationBuilder app)
		{
			UseMiddlewareExtensions.UseMiddleware<RequestLogPerformanceMiddleware>(app, Array.Empty<object>());
			return app;
		}

		public static IApplicationBuilder UseSquidexRobotsTxt(this IApplicationBuilder app)
		{
			MapExtensions.Map(app, "/robots.txt", (IApplicationBuilder builder) => UseMiddlewareExtensions.UseMiddleware<RobotsTxtMiddleware>(builder, Array.Empty<object>()));
			return app;
		}

		public static IApplicationBuilder UseSquidexUsage(this IApplicationBuilder app)
		{
			UseMiddlewareExtensions.UseMiddleware<UsageMiddleware>(app, Array.Empty<object>());
			return app;
		}
	}
}