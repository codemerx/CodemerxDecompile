using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NodaTime;
using Squidex.Areas.Api.Controllers.Contents.Generator;
using Squidex.Areas.Api.Controllers.News.Service;
using Squidex.Domain.Apps.Core.Scripting;
using Squidex.Domain.Apps.Core.Scripting.Extensions;
using Squidex.Domain.Apps.Core.Templates;
using Squidex.Domain.Apps.Core.Templates.Extensions;
using Squidex.Domain.Apps.Entities.Contents.Counter;
using Squidex.Domain.Apps.Entities.Tags;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Diagnostics;
using Squidex.Infrastructure.Log;
using Squidex.Infrastructure.Translations;
using Squidex.Infrastructure.UsageTracking;
using Squidex.Shared;
using Squidex.Text.Translations;
using Squidex.Text.Translations.GoogleCloud;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	[Nullable(0)]
	[NullableContext(1)]
	public static class InfrastructureServices
	{
		public static void AddSquidexControllerServices(this IServiceCollection services, IConfiguration config)
		{
			ConfigurationServiceExtensions.Configure<RobotsTxtOptions>(services, config, "robots");
			ConfigurationServiceExtensions.Configure<CachingOptions>(services, config, "caching");
			ConfigurationServiceExtensions.Configure<MyUIOptions>(services, config, "ui");
			ConfigurationServiceExtensions.Configure<MyNewsOptions>(services, config, "news");
			DependencyInjectionExtensions.AddSingletonAs<FeaturesService>(services).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<SchemasOpenApiGenerator>(services).AsSelf();
		}

		public static void AddSquidexInfrastructure(this IServiceCollection services, IConfiguration config)
		{
			ConfigurationServiceExtensions.Configure<ExposedConfiguration>(services, config, "exposedConfiguration");
			ConfigurationServiceExtensions.Configure<ReplicatedCacheOptions>(services, config, "caching:replicated");
			ConfigurationServiceExtensions.Configure<JintScriptOptions>(services, config, "scripting");
			ConfigurationServiceExtensions.Configure<DiagnoserOptions>(services, config, "diagnostics");
			CachingServiceExtensions.AddReplicatedCache(services, null);
			CachingServiceExtensions.AddAsyncLocalCache(services);
			CachingServiceExtensions.AddBackgroundCache(services);
			DependencyInjectionExtensions.AddSingletonAs<SystemClock>(services, (IServiceProvider _) => SystemClock.get_Instance()).As<IClock>();
			DependencyInjectionExtensions.AddSingletonAs<BackgroundRequestLogStore>(services).AsOptional<IRequestLogStore>();
			DependencyInjectionExtensions.AddSingletonAs<Diagnoser>(services).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<ScriptingCompleter>(services).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<JintScriptEngine>(services).As<IScriptEngine>().As<IScriptDescriptor>();
			DependencyInjectionExtensions.AddSingletonAs<TagService>(services).As<ITagService>();
			DependencyInjectionExtensions.AddSingletonAs<CounterJintExtension>(services).As<IJintExtension>().As<IScriptDescriptor>();
			DependencyInjectionExtensions.AddSingletonAs<DateTimeJintExtension>(services).As<IJintExtension>().As<IScriptDescriptor>();
			DependencyInjectionExtensions.AddSingletonAs<StringJintExtension>(services).As<IJintExtension>().As<IScriptDescriptor>();
			DependencyInjectionExtensions.AddSingletonAs<StringWordsJintExtension>(services).As<IJintExtension>().As<IScriptDescriptor>();
			DependencyInjectionExtensions.AddSingletonAs<HttpJintExtension>(services).As<IJintExtension>().As<IScriptDescriptor>();
			DependencyInjectionExtensions.AddSingletonAs<FluidTemplateEngine>(services).AsOptional<ITemplateEngine>();
			DependencyInjectionExtensions.AddSingletonAs<ContentFluidExtension>(services).As<IFluidExtension>();
			DependencyInjectionExtensions.AddSingletonAs<DateTimeFluidExtension>(services).As<IFluidExtension>();
			DependencyInjectionExtensions.AddSingletonAs<StringFluidExtension>(services).As<IFluidExtension>();
			DependencyInjectionExtensions.AddSingletonAs<StringWordsFluidExtension>(services).As<IFluidExtension>();
			DependencyInjectionExtensions.AddSingletonAs<UserFluidExtension>(services).As<IFluidExtension>();
		}

		public static void AddSquidexLocalization(this IServiceCollection services)
		{
			ResourcesLocalizer resourcesLocalizer = new ResourcesLocalizer(Texts.get_ResourceManager());
			T.Setup(resourcesLocalizer);
			DependencyInjectionExtensions.AddSingletonAs<ResourcesLocalizer>(services, (IServiceProvider c) => resourcesLocalizer).As<ILocalizer>();
		}

		public static void AddSquidexTranslation(this IServiceCollection services, IConfiguration config)
		{
			ConfigurationServiceExtensions.Configure<GoogleCloudTranslationOptions>(services, config, "translations:googleCloud");
			ConfigurationServiceExtensions.Configure<DeepLOptions>(services, config, "translations:deepL");
			ConfigurationServiceExtensions.Configure<LanguagesOptions>(services, config, "languages");
			DependencyInjectionExtensions.AddSingletonAs<LanguagesInitializer>(services).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<DeepLTranslationService>(services, (IServiceProvider c) => new DeepLTranslationService(ServiceProviderServiceExtensions.GetRequiredService<IOptions<DeepLOptions>>(c).get_Value())).As<ITranslationService>();
			DependencyInjectionExtensions.AddSingletonAs<GoogleCloudTranslationService>(services, (IServiceProvider c) => new GoogleCloudTranslationService(ServiceProviderServiceExtensions.GetRequiredService<IOptions<GoogleCloudTranslationOptions>>(c).get_Value())).As<ITranslationService>();
			DependencyInjectionExtensions.AddSingletonAs<Translator>(services).As<ITranslator>();
		}

		public static void AddSquidexUsageTracking(this IServiceCollection services, IConfiguration config)
		{
			ConfigurationServiceExtensions.Configure<UsageOptions>(services, config, "usage");
			DependencyInjectionExtensions.AddSingletonAs<CachingUsageTracker>(services, (IServiceProvider c) => new CachingUsageTracker(ServiceProviderServiceExtensions.GetRequiredService<BackgroundUsageTracker>(c), ServiceProviderServiceExtensions.GetRequiredService<IMemoryCache>(c))).As<IUsageTracker>();
			DependencyInjectionExtensions.AddSingletonAs<ApiUsageTracker>(services).As<IApiUsageTracker>();
			DependencyInjectionExtensions.AddSingletonAs<BackgroundUsageTracker>(services).AsSelf();
		}
	}
}