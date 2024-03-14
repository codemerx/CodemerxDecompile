using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Squidex.Domain.Apps.Core.ValidateContent;
using Squidex.Domain.Apps.Entities.Apps.Templates;
using Squidex.Domain.Apps.Entities.Contents;
using Squidex.Domain.Apps.Entities.Contents.Counter;
using Squidex.Domain.Apps.Entities.Contents.Queries;
using Squidex.Domain.Apps.Entities.Contents.Queries.Steps;
using Squidex.Domain.Apps.Entities.Contents.Text;
using Squidex.Domain.Apps.Entities.Contents.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	public static class ContentsServices
	{
		[NullableContext(1)]
		public static void AddSquidexContents(IServiceCollection services, IConfiguration config)
		{
			ConfigurationServiceExtensions.Configure<ContentOptions>(services, config, "contents");
			ConfigurationServiceExtensions.Configure<TemplatesOptions>(services, config, "templates");
			DependencyInjectionExtensions.AddSingletonAs<Lazy<IContentQueryService>>(services, (IServiceProvider c) => new Lazy<IContentQueryService>(new Func<IContentQueryService>(c.GetRequiredService<IContentQueryService>))).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<ContentQueryParser>(services).AsSelf();
			DependencyInjectionExtensions.AddTransientAs<CounterService>(services).As<ICounterService>().As<IDeleter>();
			DependencyInjectionExtensions.AddTransientAs<ContentCache>(services).As<IContentCache>();
			DependencyInjectionExtensions.AddSingletonAs<DefaultValidatorsFactory>(services).As<IValidatorsFactory>();
			DependencyInjectionExtensions.AddSingletonAs<DependencyValidatorsFactory>(services).As<IValidatorsFactory>();
			DependencyInjectionExtensions.AddSingletonAs<ContentHistoryEventsCreator>(services).As<IHistoryEventsCreator>();
			DependencyInjectionExtensions.AddSingletonAs<ContentQueryService>(services).As<IContentQueryService>();
			DependencyInjectionExtensions.AddSingletonAs<ConvertData>(services).As<IContentEnricherStep>();
			DependencyInjectionExtensions.AddSingletonAs<CalculateTokens>(services).As<IContentEnricherStep>();
			DependencyInjectionExtensions.AddSingletonAs<EnrichForCaching>(services).As<IContentEnricherStep>();
			DependencyInjectionExtensions.AddSingletonAs<EnrichWithSchema>(services).As<IContentEnricherStep>();
			DependencyInjectionExtensions.AddSingletonAs<EnrichWithWorkflows>(services).As<IContentEnricherStep>();
			DependencyInjectionExtensions.AddSingletonAs<ResolveAssets>(services).As<IContentEnricherStep>();
			DependencyInjectionExtensions.AddSingletonAs<ResolveReferences>(services).As<IContentEnricherStep>();
			DependencyInjectionExtensions.AddSingletonAs<ScriptContent>(services).As<IContentEnricherStep>();
			DependencyInjectionExtensions.AddSingletonAs<ContentEnricher>(services).As<IContentEnricher>();
			DependencyInjectionExtensions.AddSingletonAs<ContentLoader>(services).As<IContentLoader>();
			DependencyInjectionExtensions.AddSingletonAs<DynamicContentWorkflow>(services).AsOptional<IContentWorkflow>();
			DependencyInjectionExtensions.AddSingletonAs<DefaultWorkflowsValidator>(services).AsOptional<IWorkflowsValidator>();
			DependencyInjectionExtensions.AddSingletonAs<TextIndexingProcess>(services).As<IEventConsumer>();
			DependencyInjectionExtensions.AddSingletonAs<ContentsSearchSource>(services).As<ISearchSource>();
			DependencyInjectionExtensions.AddSingletonAs<TemplatesClient>(services).AsSelf();
		}
	}
}