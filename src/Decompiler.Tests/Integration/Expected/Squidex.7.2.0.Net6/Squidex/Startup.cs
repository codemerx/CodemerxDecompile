using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag.AspNetCore;
using Squidex.Areas.Api.Config.OpenApi;
using Squidex.Areas.Frontend;
using Squidex.Areas.IdentityServer.Config;
using Squidex.Config.Authentication;
using Squidex.Config.Domain;
using Squidex.Config.Messaging;
using Squidex.Config.Web;
using Squidex.Pipeline.Plugins;
using Squidex.Web.Pipeline;
using System;
using System.Runtime.CompilerServices;

namespace Squidex
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class Startup
	{
		private readonly IConfiguration config;

		public Startup(IConfiguration config)
		{
			this.config = config;
		}

		public void Configure(IApplicationBuilder app)
		{
			WebSocketMiddlewareExtensions.UseWebSockets(app);
			CookiePolicyAppBuilderExtensions.UseCookiePolicy(app);
			Microsoft.AspNetCore.Builder.WebExtensions.UseDefaultPathBase(app);
			Microsoft.AspNetCore.Builder.WebExtensions.UseDefaultForwardRules(app);
			app.UseSquidexHealthCheck();
			app.UseSquidexRobotsTxt();
			app.UseSquidexLogging();
			app.UseSquidexLocalization();
			app.UseSquidexLocalCache();
			app.UseSquidexCors();
			NSwagApplicationBuilderExtensions.UseOpenApi(app, (OpenApiDocumentMiddlewareSettings options) => options.set_Path("/api/swagger/v1/swagger.json"));
			UseWhenExtensions.UseWhen(app, (HttpContext c) => c.get_Request().get_Path().StartsWithSegments("/identity-server", StringComparison.OrdinalIgnoreCase), (IApplicationBuilder builder) => ExceptionHandlerExtensions.UseExceptionHandler(builder, "/identity-server/error"));
			UseWhenExtensions.UseWhen(app, (HttpContext c) => c.get_Request().get_Path().StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase), (IApplicationBuilder builder) => {
				builder.UseSquidexCacheKeys();
				builder.UseSquidexExceptionHandling();
				builder.UseSquidexUsage();
				AccessTokenQueryExtensions.UseAccessTokenQueryString(builder);
			});
			EndpointRoutingApplicationBuilderExtensions.UseRouting(app);
			AuthAppBuilderExtensions.UseAuthentication(app);
			AuthorizationAppBuilderExtensions.UseAuthorization(app);
			EndpointRoutingApplicationBuilderExtensions.UseEndpoints(app, (IEndpointRouteBuilder endpoints) => ControllerEndpointRouteBuilderExtensions.MapControllers(endpoints));
			MapExtensions.Map(app, "/api", (IApplicationBuilder builder) => Microsoft.AspNetCore.Builder.WebExtensions.Use404(builder));
			app.UseFrontend();
			app.UsePlugins();
		}

		public void ConfigureServices(IServiceCollection services)
		{
			HttpClientFactoryServiceCollectionExtensions.AddHttpClient(services);
			MemoryCacheServiceCollectionExtensions.AddMemoryCache(services);
			HealthCheckServiceCollectionExtensions.AddHealthChecks(services);
			WebServiceExtensions.AddDefaultWebServices(services, this.config);
			WebServiceExtensions.AddDefaultForwardRules(services);
			services.AddSquidexMvcWithPlugins(this.config);
			IdentityServices.AddSquidexIdentity(services, this.config);
			services.AddSquidexIdentityServer();
			services.AddSquidexAuthentication(this.config);
			AppsServices.AddSquidexApps(services, this.config);
			services.AddSquidexAssetInfrastructure(this.config);
			services.AddSquidexAssets(this.config);
			BackupsServices.AddSquidexBackups(services);
			CommandsServices.AddSquidexCommands(services, this.config);
			CommentsServices.AddSquidexComments(services);
			ContentsServices.AddSquidexContents(services, this.config);
			services.AddSquidexControllerServices(this.config);
			EventPublishersServices.AddSquidexEventPublisher(services, this.config);
			EventSourcingServices.AddSquidexEventSourcing(services, this.config);
			services.AddSquidexGraphQL();
			HealthCheckServices.AddSquidexHealthChecks(services, this.config);
			HistoryServices.AddSquidexHistory(services, this.config);
			ResizeServices.AddSquidexImageResizing(services, this.config);
			services.AddSquidexInfrastructure(this.config);
			services.AddSquidexLocalization();
			MessagingServices.AddSquidexMessaging(services, this.config);
			MigrationServices.AddSquidexMigration(services, this.config);
			NotificationsServices.AddSquidexNotifications(services, this.config);
			services.AddSquidexOpenApiSettings();
			QueryServices.AddSquidexQueries(services, this.config);
			RuleServices.AddSquidexRules(services, this.config);
			SchemasServices.AddSquidexSchemas(services);
			SearchServices.AddSquidexSearch(services);
			services.AddSquidexSerializers();
			services.AddSquidexStoreServices(this.config);
			SubscriptionServices.AddSquidexSubscriptions(services, this.config);
			TeamServices.AddSquidexTeams(services);
			TelemetryServices.AddSquidexTelemetry(services, this.config);
			services.AddSquidexTranslation(this.config);
			services.AddSquidexUsageTracking(this.config);
		}
	}
}