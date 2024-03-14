using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Domain.Apps.Entities.Apps.DomainObject;
using Squidex.Domain.Apps.Entities.Apps.Indexes;
using Squidex.Domain.Apps.Entities.Apps.Plans;
using Squidex.Domain.Apps.Entities.Apps.Templates;
using Squidex.Domain.Apps.Entities.Assets.Commands;
using Squidex.Domain.Apps.Entities.Assets.DomainObject;
using Squidex.Domain.Apps.Entities.Comments.DomainObject;
using Squidex.Domain.Apps.Entities.Contents;
using Squidex.Domain.Apps.Entities.Contents.DomainObject;
using Squidex.Domain.Apps.Entities.Invitation;
using Squidex.Domain.Apps.Entities.Rules;
using Squidex.Domain.Apps.Entities.Rules.Indexes;
using Squidex.Domain.Apps.Entities.Rules.UsageTracking;
using Squidex.Domain.Apps.Entities.Schemas.Commands;
using Squidex.Domain.Apps.Entities.Schemas.DomainObject;
using Squidex.Domain.Apps.Entities.Schemas.Indexes;
using Squidex.Domain.Apps.Entities.Teams.Commands;
using Squidex.Domain.Apps.Entities.Teams.DomainObject;
using Squidex.Domain.Apps.Entities.Teams.Indexes;
using Squidex.Infrastructure.Commands;
using Squidex.Web.CommandMiddlewares;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	public static class CommandsServices
	{
		[NullableContext(1)]
		public static void AddSquidexCommands(IServiceCollection services, IConfiguration config)
		{
			ConfigurationServiceExtensions.Configure<ReadonlyOptions>(services, config, "mode");
			ConfigurationServiceExtensions.Configure<RestrictAppsOptions>(services, config, "usage");
			ConfigurationServiceExtensions.Configure<DomainObjectCacheOptions>(services, config, "caching:domainObjects");
			DependencyInjectionExtensions.AddSingletonAs<InMemoryCommandBus>(services).As<ICommandBus>();
			DependencyInjectionExtensions.AddSingletonAs<ReadonlyCommandMiddleware>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<ETagCommandMiddleware>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<EnrichWithTimestampCommandMiddleware>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<EnrichWithActorCommandMiddleware>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<EnrichWithAppIdCommandMiddleware>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<EnrichWithTeamIdCommandMiddleware>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<EnrichWithSchemaIdCommandMiddleware>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<EnrichWithContentIdCommandMiddleware>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<CustomCommandMiddlewareRunner>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<TemplateCommandMiddleware>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<AlwaysCreateClientCommandMiddleware>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<RestrictAppsCommandMiddleware>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<InviteUserCommandMiddleware>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<AppsIndex>(services).As<ICommandMiddleware>().As<IAppsIndex>();
			DependencyInjectionExtensions.AddSingletonAs<RulesIndex>(services).As<IRulesIndex>();
			DependencyInjectionExtensions.AddSingletonAs<SchemasIndex>(services).As<ICommandMiddleware>().As<ISchemasIndex>();
			DependencyInjectionExtensions.AddSingletonAs<TeamsIndex>(services).As<ITeamsIndex>();
			DependencyInjectionExtensions.AddSingletonAs<AppCommandMiddleware>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<AssetsBulkUpdateCommandMiddleware>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<AssetCommandMiddleware>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<CommentsCommandMiddleware>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<ContentsBulkUpdateCommandMiddleware>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<ContentCommandMiddleware>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<RuleCommandMiddleware>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<AggregateCommandMiddleware<AssetFolderCommandBase, AssetFolderDomainObject>>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<AggregateCommandMiddleware<SchemaCommandBase, SchemaDomainObject>>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<AggregateCommandMiddleware<TeamCommandBase, TeamDomainObject>>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<SingletonCommandMiddleware>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<UsageTrackerCommandMiddleware>(services).As<ICommandMiddleware>();
			DependencyInjectionExtensions.AddSingletonAs<DefaultDomainObjectFactory>(services).As<IDomainObjectFactory>();
			DependencyInjectionExtensions.AddSingletonAs<DefaultDomainObjectCache>(services).As<IDomainObjectCache>();
		}
	}
}