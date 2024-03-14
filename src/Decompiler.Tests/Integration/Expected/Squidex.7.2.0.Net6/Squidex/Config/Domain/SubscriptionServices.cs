using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Squidex.Domain.Apps.Entities.Billing;
using Squidex.Infrastructure;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	public static class SubscriptionServices
	{
		[NullableContext(1)]
		public static void AddSquidexSubscriptions(IServiceCollection services, IConfiguration config)
		{
			DependencyInjectionExtensions.AddSingletonAs<IEnumerable<Plan>>(services, (IServiceProvider c) => {
				IOptions<UsageOptions> requiredService = ServiceProviderServiceExtensions.GetRequiredService<IOptions<UsageOptions>>(c);
				if (requiredService == null)
				{
					return null;
				}
				UsageOptions value = requiredService.get_Value();
				if (value == null)
				{
					return null;
				}
				return Squidex.Infrastructure.CollectionExtensions.OrEmpty<Plan>(value.get_Plans());
			});
			DependencyInjectionExtensions.AddSingletonAs<ConfigPlansProvider>(services).AsOptional<IBillingPlans>();
			DependencyInjectionExtensions.AddSingletonAs<NoopBillingManager>(services).AsOptional<IBillingManager>();
			DependencyInjectionExtensions.AddSingletonAs<UsageGate>(services).AsOptional<IUsageGate>().As<IAssetUsageTracker>();
		}
	}
}