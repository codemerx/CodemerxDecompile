using GraphQL;
using GraphQL.DI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Squidex.Config.Domain;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Entities.Contents.GraphQL;
using Squidex.Infrastructure.Json.Objects;
using Squidex.Pipeline.Plugins;
using Squidex.Web;
using Squidex.Web.GraphQL;
using Squidex.Web.Pipeline;
using Squidex.Web.Services;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Web
{
	[Nullable(0)]
	[NullableContext(1)]
	public static class WebServices
	{
		public static void AddSquidexGraphQL(this IServiceCollection services)
		{
			MicrosoftDIGraphQLBuilderExtensions.AddGraphQL(services, (IGraphQLBuilder builder) => {
				GraphQLBuilderExtensions.UseApolloTracing(builder, true);
				GraphQLBuilderExtensions.AddSchema<DummySchema>(builder, 0);
				SystemTextJsonGraphQLBuilderExtensions.AddSystemTextJson(builder, null);
				DataLoaderGraphQLBuilderExtensions.AddDataLoader(builder);
			});
			DependencyInjectionExtensions.AddSingletonAs<DummySchema>(services).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<DynamicUserContextBuilder>(services).As<IUserContextBuilder>();
			DependencyInjectionExtensions.AddSingletonAs<CachingGraphQLResolver>(services).As<IConfigureExecution>();
			DependencyInjectionExtensions.AddSingletonAs<GraphQLRunner>(services).AsSelf();
		}

		public static void AddSquidexMvcWithPlugins(this IServiceCollection services, IConfiguration config)
		{
			DependencyInjectionExtensions.AddSingletonAs<ExposedValues>(services, (IServiceProvider c) => new ExposedValues(ServiceProviderServiceExtensions.GetRequiredService<IOptions<ExposedConfiguration>>(c).get_Value(), config, typeof(WebServices).Assembly)).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<FileCallbackResultExecutor>(services).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<ApiCostsFilter>(services).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<AppResolver>(services).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<SchemaResolver>(services).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<TeamResolver>(services).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<UsageMiddleware>(services).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<StringLocalizer>(services).As<IStringLocalizer>().As<IStringLocalizerFactory>();
			DependencyInjectionExtensions.AddSingletonAs<CachingManager>(services).As<IRequestCache>();
			DependencyInjectionExtensions.AddSingletonAs<ContextProvider>(services).As<IContextProvider>();
			DependencyInjectionExtensions.AddSingletonAs<HttpContextAccessor>(services).As<IHttpContextAccessor>();
			DependencyInjectionExtensions.AddSingletonAs<ActionContextAccessor>(services).As<IActionContextAccessor>();
			OptionsServiceCollectionExtensions.Configure<ApiBehaviorOptions>(services, (ApiBehaviorOptions options) => {
				options.set_SuppressInferBindingSourcesForParameters(true);
				options.set_SuppressModelStateInvalidFilter(true);
			});
			LocalizationServiceCollectionExtensions.AddLocalization(services);
			RazorRuntimeCompilationMvcBuilderExtensions.AddRazorRuntimeCompilation(MvcDataAnnotationsMvcBuilderExtensions.AddDataAnnotationsLocalization(MvcServiceCollectionExtensions.AddMvc(services, (MvcOptions options) => {
				options.get_Filters().Add<CachingFilter>();
				options.get_Filters().Add<DeferredActionFilter>();
				options.get_Filters().Add<ContextFilter>();
				options.get_Filters().Add<AppResolver>();
				options.get_Filters().Add<TeamResolver>();
				options.get_Filters().Add<SchemaResolver>();
				options.get_Filters().Add<MeasureResultFilter>();
				options.get_ModelMetadataDetailsProviders().Add(new SuppressChildValidationMetadataProvider(typeof(ContentData)));
				options.get_ModelMetadataDetailsProviders().Add(new SuppressChildValidationMetadataProvider(typeof(ContentFieldData)));
				options.get_ModelMetadataDetailsProviders().Add(new SuppressChildValidationMetadataProvider(typeof(JsonArray)));
				options.get_ModelMetadataDetailsProviders().Add(new SuppressChildValidationMetadataProvider(typeof(JsonObject)));
				options.get_ModelMetadataDetailsProviders().Add(new SuppressChildValidationMetadataProvider(typeof(JsonValue)));
			}))).AddSquidexPlugins(config).AddSquidexSerializers();
		}
	}
}