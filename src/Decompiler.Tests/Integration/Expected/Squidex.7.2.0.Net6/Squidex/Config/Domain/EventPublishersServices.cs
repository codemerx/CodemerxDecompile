using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Squidex.Hosting.Configuration;
using Squidex.Infrastructure.CQRS.Events;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	public static class EventPublishersServices
	{
		[NullableContext(1)]
		public static void AddSquidexEventPublisher(IServiceCollection services, IConfiguration config)
		{
			foreach (IConfigurationSection child in config.GetSection("eventPublishers").GetChildren())
			{
				string value = ConfigurationBinder.GetValue<string>(child, "type");
				if (string.IsNullOrWhiteSpace(value))
				{
					throw new ConfigurationException(new ConfigurationError("Value is required.", "eventPublishers:{child.Key}:type"), null);
				}
				string str = ConfigurationBinder.GetValue<string>(child, "eventsFilter");
				bool flag = ConfigurationBinder.GetValue<bool>(child, "enabled");
				if (!string.Equals(value, "RabbitMq", StringComparison.OrdinalIgnoreCase))
				{
					ConfigurationError configurationError = new ConfigurationError(string.Concat("Unsupported value '", child.get_Key()), "eventPublishers:{child.Key}:type.");
					throw new ConfigurationException(configurationError, null);
				}
				string value1 = ConfigurationBinder.GetValue<string>(child, "configuration");
				if (string.IsNullOrWhiteSpace(value1))
				{
					throw new ConfigurationException(new ConfigurationError("Value is required.", "eventPublishers:{child.Key}:configuration"), null);
				}
				string str1 = ConfigurationBinder.GetValue<string>(child, "exchange");
				if (string.IsNullOrWhiteSpace(str1))
				{
					throw new ConfigurationException(new ConfigurationError("Value is required.", "eventPublishers:{child.Key}:exchange"), null);
				}
				string str2 = string.Concat("EventPublishers_", child.get_Key());
				if (!flag)
				{
					continue;
				}
				DependencyInjectionExtensions.AddSingletonAs<RabbitMqEventConsumer>(services, (IServiceProvider c) => new RabbitMqEventConsumer(ServiceProviderServiceExtensions.GetRequiredService<IJsonSerializer>(c), str2, value1, str1, str)).As<IEventConsumer>();
			}
		}
	}
}