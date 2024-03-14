using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	public static class HealthCheckServices
	{
		[NullableContext(1)]
		public static void AddSquidexHealthChecks(IServiceCollection services, IConfiguration config)
		{
			ConfigurationServiceExtensions.Configure<GCHealthCheckOptions>(services, config, "diagnostics:gc");
			IHealthChecksBuilder healthChecksBuilder = HealthCheckServiceCollectionExtensions.AddHealthChecks(services);
			IEnumerable<string> strs = new string[] { "node" };
			HealthStatus? nullable = null;
			TimeSpan? nullable1 = null;
			HealthChecksBuilderAddCheckExtensions.AddCheck<GCHealthCheck>(healthChecksBuilder, "GC", nullable, strs, nullable1);
		}
	}
}