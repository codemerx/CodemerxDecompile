using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Squidex.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	public static class TelemetryServices
	{
		[NullableContext(1)]
		public static void AddSquidexTelemetry(IServiceCollection services, IConfiguration config)
		{
			OpenTelemetryServicesExtensions.AddOpenTelemetryTracing(services);
			ServiceCollectionServiceExtensions.AddSingleton<TracerProvider>(services, (IServiceProvider serviceProvider) => {
				TracerProviderBuilder tracerProviderBuilder = Sdk.CreateTracerProviderBuilder();
				TracerProviderBuilderExtensions.SetResourceBuilder(tracerProviderBuilder, ResourceBuilderExtensions.AddService(ResourceBuilder.CreateDefault(), ConfigurationBinder.GetValue<string>(config, "logging:name") ?? "Squidex", "Squidex", typeof(TelemetryServices).Assembly.GetName().Version.ToString(), true, null));
				tracerProviderBuilder.AddSource(new string[] { "Squidex" });
				TracerProviderBuilderExtensions.AddAspNetCoreInstrumentation(tracerProviderBuilder, null);
				TracerProviderBuilderExtensions.AddHttpClientInstrumentation(tracerProviderBuilder, null);
				TracerProviderBuilderExtensions.AddMongoDBInstrumentation(tracerProviderBuilder);
				double value = ConfigurationBinder.GetValue<double>(config, "logging:otlp:sampling");
				if (value > 0 && value < 1)
				{
					TracerProviderBuilderExtensions.SetSampler(tracerProviderBuilder, new ParentBasedSampler(new TraceIdRatioBasedSampler(value)));
				}
				foreach (ITelemetryConfigurator requiredService in ServiceProviderServiceExtensions.GetRequiredService<IEnumerable<ITelemetryConfigurator>>(serviceProvider))
				{
					requiredService.Configure(tracerProviderBuilder);
				}
				return TracerProviderBuilderExtensions.Build(tracerProviderBuilder);
			});
		}
	}
}