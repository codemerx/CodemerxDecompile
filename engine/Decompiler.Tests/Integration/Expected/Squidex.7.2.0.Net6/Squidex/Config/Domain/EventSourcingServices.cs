using EventStore.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Squidex.Infrastructure.Commands;
using Squidex.Infrastructure.EventSourcing;
using Squidex.Infrastructure.EventSourcing.Consume;
using Squidex.Infrastructure.States;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	public static class EventSourcingServices
	{
		[NullableContext(1)]
		public static void AddSquidexEventSourcing(IServiceCollection services, IConfiguration config)
		{
			IConfiguration configuration = config;
			Alternatives alternative = new Alternatives();
			alternative["MongoDb"] = () => {
				string requiredValue = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetRequiredValue(config, "eventStore:mongoDb:configuration");
				string str = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetRequiredValue(config, "eventStore:mongoDb:database");
				DependencyInjectionExtensions.AddSingletonAs<MongoEventStore>(services, (IServiceProvider c) => new MongoEventStore(StoreServices.GetMongoClient(requiredValue).GetDatabase(str, null), ServiceProviderServiceExtensions.GetRequiredService<IEventNotifier>(c))).As<IEventStore>();
			};
			alternative["GetEventStore"] = () => {
				string requiredValue = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetRequiredValue(config, "eventStore:getEventStore:configuration");
				DependencyInjectionExtensions.AddSingletonAs<EventStoreClientSettings>(services, (IServiceProvider _) => EventStoreClientSettings.Create(requiredValue)).AsSelf();
				DependencyInjectionExtensions.AddSingletonAs<GetEventStore>(services).As<IEventStore>();
				HealthChecksBuilderAddCheckExtensions.AddCheck<GetEventStoreHealthCheck>(HealthCheckServiceCollectionExtensions.AddHealthChecks(services), "EventStore", null, new string[] { "node" }, null);
			};
			Microsoft.Extensions.Configuration.ConfigurationExtensions.ConfigureByOption(configuration, "eventStore:type", alternative);
			DependencyInjectionExtensions.AddTransientAs<Rebuilder>(services).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<EventConsumerManager>(services).As<IEventConsumerManager>();
			DependencyInjectionExtensions.AddSingletonAs<DefaultEventStreamNames>(services).As<IEventStreamNames>();
			DependencyInjectionExtensions.AddSingletonAs<DefaultEventFormatter>(services).As<IEventFormatter>();
			DependencyInjectionExtensions.AddSingletonAs<NoopEventNotifier>(services).As<IEventNotifier>();
			ServiceCollectionServiceExtensions.AddSingleton<Func<IEventConsumer, EventConsumerProcessor>>(services, (IServiceProvider sb) => (IEventConsumer c) => ActivatorUtilities.CreateInstance<EventConsumerProcessor>(sb, new object[] { c }));
		}
	}
}