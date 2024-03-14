using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Squidex.Log;
using Squidex.Web.Pipeline;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	[Nullable(0)]
	[NullableContext(1)]
	public static class LoggingServices
	{
		private static void AddFilters(this ILoggingBuilder builder)
		{
			FilterLoggingBuilderExtensions.AddFilter(builder, (string category, LogLevel level) => {
				if (level < 2)
				{
					return false;
				}
				if (category.StartsWith("OpenIddict", StringComparison.OrdinalIgnoreCase))
				{
					return level >= 3;
				}
				if (category.StartsWith("Runtime.", StringComparison.OrdinalIgnoreCase))
				{
					return level >= 3;
				}
				if (!category.StartsWith("Microsoft.AspNetCore.", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				return level >= 3;
			});
		}

		private static void AddServices(this IServiceCollection services, IConfiguration config)
		{
			ConfigurationServiceExtensions.Configure<RequestLogOptions>(services, config, "logging");
			ConfigurationServiceExtensions.Configure<RequestLogStoreOptions>(services, config, "logging");
			DependencyInjectionExtensions.AddSingletonAs<ApplicationInfoLogAppender>(services, (IServiceProvider _) => new ApplicationInfoLogAppender(typeof(LoggingServices).Assembly, Guid.NewGuid())).As<ILogAppender>();
			DependencyInjectionExtensions.AddSingletonAs<ActionContextLogAppender>(services).As<ILogAppender>();
		}

		public static void ConfigureForSquidex(this ILoggingBuilder builder, IConfiguration config)
		{
			LoggingBuilderExtensions.ClearProviders(builder);
			LoggingServiceExtensions.ConfigureSemanticLog(builder, config);
			LoggingBuilderExtensions.AddConfiguration(builder, config.GetSection("logging"));
			builder.AddFilters();
			builder.get_Services().AddServices(config);
		}
	}
}