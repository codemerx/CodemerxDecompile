using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Squidex.Domain.Apps.Core.Subscriptions;
using Squidex.Domain.Apps.Entities.Assets;
using Squidex.Domain.Apps.Entities.Backup;
using Squidex.Domain.Apps.Entities.Billing;
using Squidex.Domain.Apps.Entities.Contents;
using Squidex.Domain.Apps.Entities.Rules;
using Squidex.Domain.Apps.Entities.Rules.Runner;
using Squidex.Domain.Apps.Entities.Rules.UsageTracking;
using Squidex.Infrastructure.EventSourcing.Consume;
using Squidex.Messaging;
using Squidex.Messaging.Implementation;
using Squidex.Messaging.Implementation.Null;
using Squidex.Messaging.Implementation.Scheduler;
using Squidex.Messaging.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Messaging
{
	public static class MessagingServices
	{
		[NullableContext(1)]
		public static void AddSquidexMessaging(IServiceCollection services, IConfiguration config)
		{
			Func<IMessagingTransport, bool> func2 = null;
			TransportSelector transportSelector2 = null;
			ChannelName channelName = new ChannelName("backup.restore", 0);
			ChannelName channelName1 = new ChannelName("backup.start", 0);
			ChannelName channelName2 = new ChannelName("default", 0);
			ChannelName channelName3 = new ChannelName("rules.run", 0);
			bool value = ConfigurationBinder.GetValue<bool>(config, "caching:replicated:enable");
			bool flag = ConfigurationBinder.GetValue<bool>(config, "clustering:worker");
			if (flag)
			{
				DependencyInjectionExtensions.AddSingletonAs<AssetCleanupProcess>(services).AsSelf();
				DependencyInjectionExtensions.AddSingletonAs<ContentSchedulerProcess>(services).AsSelf();
				DependencyInjectionExtensions.AddSingletonAs<RuleDequeuerWorker>(services).AsSelf();
				DependencyInjectionExtensions.AddSingletonAs<EventConsumerWorker>(services).AsSelf().As<IMessageHandler>();
				DependencyInjectionExtensions.AddSingletonAs<RuleRunnerWorker>(services).AsSelf().As<IMessageHandler>();
				DependencyInjectionExtensions.AddSingletonAs<BackupWorker>(services).AsSelf().As<IMessageHandler>();
				DependencyInjectionExtensions.AddSingletonAs<UsageNotifierWorker>(services).AsSelf().As<IMessageHandler>();
				DependencyInjectionExtensions.AddSingletonAs<UsageTrackerWorker>(services).AsSelf().As<IMessageHandler>();
			}
			ServiceCollectionServiceExtensions.AddSingleton<IMessagingSerializer>(services, (IServiceProvider c) => new SystemTextJsonMessagingSerializer(ServiceProviderServiceExtensions.GetRequiredService<JsonSerializerOptions>(c)));
			DependencyInjectionExtensions.AddSingletonAs<SubscriptionPublisher>(services).As<IEventConsumer>();
			DependencyInjectionExtensions.AddSingletonAs<EventMessageEvaluator>(services).As<IMessageEvaluator>();
			CachingServiceExtensions.AddReplicatedCacheMessaging(services, value, (ChannelOptions options) => {
				ChannelOptions channelOption = options;
				TransportSelector u003cu003e9_8 = transportSelector2;
				if (u003cu003e9_8 == null)
				{
					TransportSelector transportSelector = new TransportSelector(this, (IEnumerable<IMessagingTransport> transport, ChannelName _) => {
						IEnumerable<IMessagingTransport> messagingTransports = transport;
						Func<IMessagingTransport, bool> u003cu003e9_9 = func2;
						if (u003cu003e9_9 == null)
						{
							Func<IMessagingTransport, bool> func = (IMessagingTransport x) => x is NullTransport != value;
							Func<IMessagingTransport, bool> func1 = func;
							func2 = func;
							u003cu003e9_9 = func1;
						}
						return messagingTransports.First<IMessagingTransport>(u003cu003e9_9);
					});
					TransportSelector transportSelector1 = transportSelector;
					transportSelector2 = transportSelector;
					u003cu003e9_8 = transportSelector1;
				}
				channelOption.set_TransportSelector(u003cu003e9_8);
			}, "caching");
			OptionsServiceCollectionExtensions.Configure<SubscriptionOptions>(services, (SubscriptionOptions options) => options.set_SendMessagesToSelf(false));
			SubscriptionsServiceExtensions.AddMessagingSubscriptions(services, true, null, "subscriptions");
			MessagingServiceExtensions.AddMessagingTransport(services, config);
			MessagingServiceExtensions.AddMessaging(services, (MessagingOptions options) => {
				RoutingCollection routing = options.get_Routing();
				Func<object, bool> u003cu003e9_010 = MessagingServices.u003cu003ec.u003cu003e9__0_10;
				if (u003cu003e9_010 == null)
				{
					u003cu003e9_010 = (object m) => m is RuleRunnerRun;
					MessagingServices.u003cu003ec.u003cu003e9__0_10 = u003cu003e9_010;
				}
				routing.Add(u003cu003e9_010, channelName3);
				RoutingCollection routingCollection = options.get_Routing();
				Func<object, bool> u003cu003e9_011 = MessagingServices.u003cu003ec.u003cu003e9__0_11;
				if (u003cu003e9_011 == null)
				{
					u003cu003e9_011 = (object m) => m is BackupStart;
					MessagingServices.u003cu003ec.u003cu003e9__0_11 = u003cu003e9_011;
				}
				routingCollection.Add(u003cu003e9_011, channelName1);
				RoutingCollection routing1 = options.get_Routing();
				Func<object, bool> u003cu003e9_012 = MessagingServices.u003cu003ec.u003cu003e9__0_12;
				if (u003cu003e9_012 == null)
				{
					u003cu003e9_012 = (object m) => m is BackupRestore;
					MessagingServices.u003cu003ec.u003cu003e9__0_12 = u003cu003e9_012;
				}
				routing1.Add(u003cu003e9_012, channelName);
				options.get_Routing().AddFallback(channelName2);
			});
			MessagingServiceExtensions.AddMessaging(services, channelName1, flag, (ChannelOptions options) => {
				options.set_Timeout(TimeSpan.FromHours(4));
				options.set_Scheduler(new ParallelScheduler(4));
				options.set_LogMessage((object x) => true);
			});
			MessagingServiceExtensions.AddMessaging(services, channelName, flag, (ChannelOptions options) => {
				options.set_Timeout(TimeSpan.FromHours(24));
				options.set_Scheduler(InlineScheduler.Instance);
				options.set_LogMessage((object x) => true);
			});
			MessagingServiceExtensions.AddMessaging(services, channelName3, flag, (ChannelOptions options) => {
				options.set_Scheduler(new ParallelScheduler(4));
				options.set_LogMessage((object x) => true);
			});
			MessagingServiceExtensions.AddMessaging(services, channelName2, flag, (ChannelOptions options) => options.set_Scheduler(InlineScheduler.Instance));
		}
	}
}