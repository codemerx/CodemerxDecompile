using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Squidex.Areas.Api.Controllers.Rules.Models;
using Squidex.Domain.Apps.Core.HandleRules;
using Squidex.Domain.Apps.Core.HandleRules.Extensions;
using Squidex.Domain.Apps.Entities.Assets;
using Squidex.Domain.Apps.Entities.Comments;
using Squidex.Domain.Apps.Entities.Contents;
using Squidex.Domain.Apps.Entities.Rules;
using Squidex.Domain.Apps.Entities.Rules.Queries;
using Squidex.Domain.Apps.Entities.Rules.Runner;
using Squidex.Domain.Apps.Entities.Rules.UsageTracking;
using Squidex.Domain.Apps.Entities.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	public static class RuleServices
	{
		[NullableContext(1)]
		public static void AddSquidexRules(IServiceCollection services, IConfiguration config)
		{
			ConfigurationServiceExtensions.Configure<RuleOptions>(services, config, "rules");
			DependencyInjectionExtensions.AddSingletonAs<EventEnricher>(services).As<IEventEnricher>();
			DependencyInjectionExtensions.AddSingletonAs<AssetChangedTriggerHandler>(services).As<IRuleTriggerHandler>().As<ISubscriptionEventCreator>();
			DependencyInjectionExtensions.AddSingletonAs<CommentTriggerHandler>(services).As<IRuleTriggerHandler>();
			DependencyInjectionExtensions.AddSingletonAs<ContentChangedTriggerHandler>(services).As<IRuleTriggerHandler>().As<ISubscriptionEventCreator>();
			DependencyInjectionExtensions.AddSingletonAs<AssetsFluidExtension>(services).As<IFluidExtension>();
			DependencyInjectionExtensions.AddSingletonAs<AssetsJintExtension>(services).As<IJintExtension>().As<IScriptDescriptor>();
			DependencyInjectionExtensions.AddSingletonAs<ReferencesFluidExtension>(services).As<IFluidExtension>();
			DependencyInjectionExtensions.AddSingletonAs<ReferencesJintExtension>(services).As<IJintExtension>().As<IScriptDescriptor>();
			DependencyInjectionExtensions.AddSingletonAs<ManualTriggerHandler>(services).As<IRuleTriggerHandler>();
			DependencyInjectionExtensions.AddSingletonAs<SchemaChangedTriggerHandler>(services).As<IRuleTriggerHandler>();
			DependencyInjectionExtensions.AddSingletonAs<UsageTriggerHandler>(services).As<IRuleTriggerHandler>();
			DependencyInjectionExtensions.AddSingletonAs<RuleQueryService>(services).As<IRuleQueryService>();
			DependencyInjectionExtensions.AddSingletonAs<DefaultRuleRunnerService>(services).As<IRuleRunnerService>();
			DependencyInjectionExtensions.AddSingletonAs<RuleEnricher>(services).As<IRuleEnricher>();
			DependencyInjectionExtensions.AddSingletonAs<RuleEnqueuer>(services).As<IRuleEnqueuer>().As<IEventConsumer>();
			DependencyInjectionExtensions.AddSingletonAs<EventJsonSchemaGenerator>(services).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<RuleTypeProvider>(services).As<ITypeProvider>().AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<EventJintExtension>(services).As<IJintExtension>().As<IScriptDescriptor>();
			DependencyInjectionExtensions.AddSingletonAs<EventFluidExtensions>(services).As<IFluidExtension>();
			DependencyInjectionExtensions.AddSingletonAs<PredefinedPatternsFormatter>(services).As<IRuleEventFormatter>();
			DependencyInjectionExtensions.AddSingletonAs<RuleService>(services).As<IRuleService>();
			DependencyInjectionExtensions.AddSingletonAs<RuleEventFormatter>(services).AsSelf();
			HostingServiceExtensions.AddInitializer<RuleTypeProvider>(services, "Serializer (Rules)", (RuleTypeProvider registry) => RuleActionConverter.Mapping = registry.get_Actions().ToDictionary<KeyValuePair<string, RuleActionDefinition>, string, Type>((KeyValuePair<string, RuleActionDefinition> x) => x.Key, (KeyValuePair<string, RuleActionDefinition> x) => x.Value.get_Type()), -1);
		}
	}
}