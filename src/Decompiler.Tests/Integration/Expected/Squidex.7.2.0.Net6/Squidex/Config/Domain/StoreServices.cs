using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Migrations.Migrations.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using Squidex.Assets;
using Squidex.Domain.Apps.Entities.MongoDb.Apps;
using Squidex.Domain.Apps.Entities.MongoDb.Assets;
using Squidex.Domain.Apps.Entities.MongoDb.Contents;
using Squidex.Domain.Apps.Entities.MongoDb.History;
using Squidex.Domain.Apps.Entities.MongoDb.Rules;
using Squidex.Domain.Apps.Entities.MongoDb.Schemas;
using Squidex.Domain.Apps.Entities.MongoDb.Teams;
using Squidex.Domain.Apps.Entities.MongoDb.Text;
using Squidex.Domain.Users.MongoDb;
using Squidex.Hosting;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Caching;
using Squidex.Infrastructure.Log;
using Squidex.Infrastructure.Migrations;
using Squidex.Infrastructure.MongoDb;
using Squidex.Infrastructure.States;
using Squidex.Infrastructure.UsageTracking;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.Config.Domain
{
	[Nullable(0)]
	[NullableContext(1)]
	public static class StoreServices
	{
		public static void AddSquidexStoreServices(this IServiceCollection services, IConfiguration config)
		{
			Action<JsonSerializerOptions> action2 = null;
			IConfiguration configuration = config;
			Alternatives alternative = new Alternatives();
			alternative["MongoDB"] = () => {
				string requiredValue = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetRequiredValue(config, "store:mongoDb:configuration");
				string str = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetRequiredValue(config, "store:mongoDb:database");
				string optionalValue = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetOptionalValue<string>(config, "store:mongoDb:contentDatabase", str);
				ServiceCollectionServiceExtensions.AddSingleton(services, typeof(ISnapshotStore), typeof(MongoSnapshotStore));
				DependencyInjectionExtensions.AddSingletonAs<IMongoClient>(services, (IServiceProvider c) => StoreServices.GetMongoClient(requiredValue)).As<IMongoClient>();
				DependencyInjectionExtensions.AddSingletonAs<IMongoDatabase>(services, (IServiceProvider c) => StoreServices.GetDatabase(c, str)).As<IMongoDatabase>();
				DependencyInjectionExtensions.AddSingletonAs<MongoMigrationStatus>(services).As<IMigrationStatus>();
				DependencyInjectionExtensions.AddTransientAs<ConvertOldSnapshotStores>(services).As<IMigration>();
				DependencyInjectionExtensions.AddTransientAs<DeleteContentCollections>(services, (IServiceProvider c) => new DeleteContentCollections(StoreServices.GetDatabase(c, optionalValue))).As<IMigration>();
				DependencyInjectionExtensions.AddTransientAs<RestructureContentCollection>(services, (IServiceProvider c) => new RestructureContentCollection(StoreServices.GetDatabase(c, optionalValue))).As<IMigration>();
				DependencyInjectionExtensions.AddTransientAs<ConvertDocumentIds>(services, (IServiceProvider c) => new ConvertDocumentIds(StoreServices.GetDatabase(c, str), StoreServices.GetDatabase(c, optionalValue))).As<IMigration>();
				DependencyInjectionExtensions.AddSingletonAs<MongoContentRepository>(services, (IServiceProvider c) => ActivatorUtilities.CreateInstance<MongoContentRepository>(c, new object[] { StoreServices.GetDatabase(c, optionalValue) })).As<IContentRepository>().As<ISnapshotStore<ContentDomainObject.State>>().As<IDeleter>();
				DependencyInjectionExtensions.AddTransientAs<ConvertRuleEventsJson>(services).As<IMigration>();
				DependencyInjectionExtensions.AddTransientAs<RenameAssetSlugField>(services).As<IMigration>();
				DependencyInjectionExtensions.AddTransientAs<RenameAssetMetadata>(services).As<IMigration>();
				DependencyInjectionExtensions.AddTransientAs<AddAppIdToEventStream>(services).As<IMigration>();
				DependencyInjectionExtensions.AddSingletonAs<MongoDistributedCache>(services).As<IDistributedCache>();
				HealthChecksBuilderAddCheckExtensions.AddCheck<MongoHealthCheck>(HealthCheckServiceCollectionExtensions.AddHealthChecks(services), "MongoDB", null, new string[] { "node" }, null);
				DependencyInjectionExtensions.AddSingletonAs<MongoAssetKeyValueStore<TusMetadata>>(services).As<IAssetKeyValueStore<TusMetadata>>();
				DependencyInjectionExtensions.AddSingletonAs<MongoRequestLogRepository>(services).As<IRequestLogRepository>();
				DependencyInjectionExtensions.AddSingletonAs<MongoUsageRepository>(services).As<IUsageRepository>();
				DependencyInjectionExtensions.AddSingletonAs<MongoRuleEventRepository>(services).As<IRuleEventRepository>().As<IDeleter>();
				DependencyInjectionExtensions.AddSingletonAs<MongoHistoryEventRepository>(services).As<IHistoryEventRepository>().As<IDeleter>();
				DependencyInjectionExtensions.AddSingletonAs<MongoRoleStore>(services).As<IRoleStore<IdentityRole>>();
				DependencyInjectionExtensions.AddSingletonAs<MongoUserStore>(services).As<IUserStore<IdentityUser>>().As<IUserFactory>();
				DependencyInjectionExtensions.AddSingletonAs<MongoAssetRepository>(services).As<IAssetRepository>().As<ISnapshotStore<AssetDomainObject.State>>().As<IDeleter>();
				DependencyInjectionExtensions.AddSingletonAs<MongoAssetFolderRepository>(services).As<IAssetFolderRepository>().As<ISnapshotStore<AssetFolderDomainObject.State>>().As<IDeleter>();
				DependencyInjectionExtensions.AddSingletonAs<MongoAppRepository>(services).As<IAppRepository>().As<ISnapshotStore<AppDomainObject.State>>().As<IDeleter>();
				DependencyInjectionExtensions.AddSingletonAs<MongoTeamRepository>(services).As<ITeamRepository>().As<ISnapshotStore<TeamDomainObject.State>>();
				DependencyInjectionExtensions.AddSingletonAs<MongoRuleRepository>(services).As<IRuleRepository>().As<ISnapshotStore<RuleDomainObject.State>>().As<IDeleter>();
				DependencyInjectionExtensions.AddSingletonAs<MongoSchemaRepository>(services).As<ISchemaRepository>().As<ISnapshotStore<SchemaDomainObject.State>>().As<IDeleter>();
				DependencyInjectionExtensions.AddSingletonAs<MongoSchemasHash>(services).AsOptional<ISchemasHash>().As<IEventConsumer>().As<IDeleter>();
				DependencyInjectionExtensions.AddSingletonAs<MongoTextIndexerState>(services).As<ITextIndexerState>().As<IDeleter>();
				OpenIddictBuilder openIddictBuilder = OpenIddictExtensions.AddOpenIddict(services);
				Action<OpenIddictCoreBuilder> u003cu003e9_08 = StoreServices.u003cu003ec.u003cu003e9__0_8;
				if (u003cu003e9_08 == null)
				{
					u003cu003e9_08 = (OpenIddictCoreBuilder builder) => {
						OpenIddictMongoDbExtensions.UseMongoDb<string>(builder).SetScopesCollectionName("Identity_Scopes").SetTokensCollectionName("Identity_Tokens");
						builder.SetDefaultScopeEntity<ImmutableScope>();
						builder.SetDefaultApplicationEntity<ImmutableApplication>();
					};
					StoreServices.u003cu003ec.u003cu003e9__0_8 = u003cu003e9_08;
				}
				OpenIddictCoreExtensions.AddCore(openIddictBuilder, u003cu003e9_08);
				AtlasOptions atlasOption = ConfigurationBinder.Get<AtlasOptions>(config.GetSection("store:mongoDb:atlas")) ?? new AtlasOptions();
				if (!atlasOption.IsConfigured() || !atlasOption.get_FullTextEnabled())
				{
					DependencyInjectionExtensions.AddSingletonAs<MongoTextIndex>(services).AsOptional<ITextIndex>().As<IDeleter>();
				}
				else
				{
					OptionsConfigurationServiceCollectionExtensions.Configure<AtlasOptions>(services, config.GetSection("store:mongoDb:atlas"));
					DependencyInjectionExtensions.AddSingletonAs<AtlasTextIndex>(services).AsOptional<ITextIndex>().As<IDeleter>();
				}
				IServiceCollection serviceCollection = services;
				Action<JsonSerializerOptions> u003cu003e9_9 = action2;
				if (u003cu003e9_9 == null)
				{
					Action<JsonSerializerOptions> action = (JsonSerializerOptions jsonSerializerOptions) => BsonJsonConvention.Register(jsonSerializerOptions, new BsonType?(ConfigurationBinder.GetValue<BsonType>(config, "store:mongoDB:valueRepresentation")));
					Action<JsonSerializerOptions> action1 = action;
					action2 = action;
					u003cu003e9_9 = action1;
				}
				HostingServiceExtensions.AddInitializer<JsonSerializerOptions>(serviceCollection, "Serializer (BSON)", u003cu003e9_9, -2147483648);
			};
			Microsoft.Extensions.Configuration.ConfigurationExtensions.ConfigureByOption(configuration, "store:type", alternative);
			ServiceCollectionServiceExtensions.AddSingleton(services, typeof(IStore), typeof(Store));
			ServiceCollectionServiceExtensions.AddSingleton(services, typeof(IPersistenceFactory), typeof(Store));
			DependencyInjectionExtensions.AddSingletonAs<IInitializable>(services, (IServiceProvider c) => {
				IAssetKeyValueStore<TusMetadata> requiredService = ServiceProviderServiceExtensions.GetRequiredService<IAssetKeyValueStore<TusMetadata>>(c);
				IAssetKeyValueStore<TusMetadata> assetKeyValueStore = requiredService;
				return new DelegateInitializer(requiredService.GetType().Name, new Func<CancellationToken, Task>(assetKeyValueStore.InitializeAsync));
			});
		}

		private static IMongoDatabase GetDatabase(IServiceProvider serviceProvider, string name)
		{
			return ServiceProviderServiceExtensions.GetRequiredService<IMongoClient>(serviceProvider).GetDatabase(name, null);
		}

		public static IMongoClient GetMongoClient(string configuration)
		{
			return Singletons<IMongoClient>.GetOrAdd(configuration, (string connectionString) => {
				MongoClientSettings mongoClientSetting = MongoClientSettings.FromConnectionString(connectionString);
				mongoClientSetting.set_ClusterConfigurator((ClusterBuilder builder) => builder.Subscribe(new DiagnosticsActivityEventSubscriber()));
				return new MongoClient(mongoClientSetting);
			});
		}
	}
}