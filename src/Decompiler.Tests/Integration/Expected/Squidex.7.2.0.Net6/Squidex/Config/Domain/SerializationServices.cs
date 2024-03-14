using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Migrations;
using NetTopologySuite.IO.Converters;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Squidex.Domain.Apps.Core;
using Squidex.Domain.Apps.Core.Apps;
using Squidex.Domain.Apps.Core.Apps.Json;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Core.Contents.Json;
using Squidex.Domain.Apps.Core.Rules;
using Squidex.Domain.Apps.Core.Rules.Json;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Domain.Apps.Core.Schemas.Json;
using Squidex.Domain.Apps.Events;
using Squidex.Infrastructure;
using Squidex.Infrastructure.EventSourcing;
using Squidex.Infrastructure.Json;
using Squidex.Infrastructure.Json.Objects;
using Squidex.Infrastructure.Json.System;
using Squidex.Infrastructure.Queries;
using Squidex.Infrastructure.Queries.Json;
using Squidex.Infrastructure.Reflection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Squidex.Config.Domain
{
	[Nullable(0)]
	[NullableContext(1)]
	public static class SerializationServices
	{
		public static IServiceCollection AddSquidexSerializers(this IServiceCollection services)
		{
			DependencyInjectionExtensions.AddSingletonAs<AutoAssembyTypeProvider<SquidexCoreModel>>(services).As<ITypeProvider>();
			DependencyInjectionExtensions.AddSingletonAs<AutoAssembyTypeProvider<SquidexEvents>>(services).As<ITypeProvider>();
			DependencyInjectionExtensions.AddSingletonAs<AutoAssembyTypeProvider<SquidexInfrastructure>>(services).As<ITypeProvider>();
			DependencyInjectionExtensions.AddSingletonAs<AutoAssembyTypeProvider<SquidexMigrations>>(services).As<ITypeProvider>();
			DependencyInjectionExtensions.AddSingletonAs<FieldTypeProvider>(services).As<ITypeProvider>();
			DependencyInjectionExtensions.AddSingletonAs<SystemJsonSerializer>(services).As<IJsonSerializer>();
			DependencyInjectionExtensions.AddSingletonAs<TypeNameRegistry>(services).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<JsonSerializerOptions>(services, (IServiceProvider c) => SerializationServices.ConfigureJson(ServiceProviderServiceExtensions.GetRequiredService<TypeNameRegistry>(c), null)).As<JsonSerializerOptions>();
			ConfigurationServiceExtensions.Configure<JsonSerializerOptions>(services, (IServiceProvider c, JsonSerializerOptions options) => SerializationServices.ConfigureJson(ServiceProviderServiceExtensions.GetRequiredService<TypeNameRegistry>(c), options));
			return services;
		}

		public static IMvcBuilder AddSquidexSerializers(this IMvcBuilder builder)
		{
			ConfigurationServiceExtensions.Configure<JsonOptions>(builder.get_Services(), (IServiceProvider c, JsonOptions options) => {
				SerializationServices.ConfigureJson(ServiceProviderServiceExtensions.GetRequiredService<TypeNameRegistry>(c), options.get_JsonSerializerOptions());
				options.get_JsonSerializerOptions().DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
				options.set_AllowInputFormatterExceptionMessages(false);
			});
			return builder;
		}

		private static JsonSerializerOptions ConfigureJson(TypeNameRegistry typeNameRegistry, [Nullable(2)] JsonSerializerOptions options = null)
		{
			if (options == null)
			{
				options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
			}
			Extensions.ConfigureForNodaTime(options, DateTimeZoneProviders.get_Tzdb());
			options.Converters.Add(new StringConverter<PropertyPath>((string x) => x, null));
			options.Converters.Add(new GeoJsonConverterFactory());
			options.Converters.Add(new InheritanceConverter<IEvent>(typeNameRegistry));
			options.Converters.Add(new InheritanceConverter<FieldProperties>(typeNameRegistry));
			options.Converters.Add(new InheritanceConverter<RuleAction>(typeNameRegistry));
			options.Converters.Add(new InheritanceConverter<RuleTrigger>(typeNameRegistry));
			options.Converters.Add(new JsonValueConverter());
			options.Converters.Add(new ReadonlyDictionaryConverterFactory());
			options.Converters.Add(new ReadonlyListConverterFactory());
			options.Converters.Add(new SurrogateJsonConverter<ClaimsPrincipal, ClaimsPrincipalSurrogate>());
			options.Converters.Add(new SurrogateJsonConverter<FilterNode<JsonValue>, JsonFilterSurrogate>());
			options.Converters.Add(new SurrogateJsonConverter<LanguageConfig, LanguageConfigSurrogate>());
			options.Converters.Add(new SurrogateJsonConverter<LanguagesConfig, LanguagesConfigSurrogate>());
			options.Converters.Add(new SurrogateJsonConverter<Roles, RolesSurrogate>());
			options.Converters.Add(new SurrogateJsonConverter<Rule, RuleSorrgate>());
			options.Converters.Add(new SurrogateJsonConverter<Schema, SchemaSurrogate>());
			options.Converters.Add(new SurrogateJsonConverter<WorkflowStep, WorkflowStepSurrogate>());
			options.Converters.Add(new SurrogateJsonConverter<WorkflowTransition, WorkflowTransitionSurrogate>());
			options.Converters.Add(new StringConverter<CompareOperator>());
			options.Converters.Add(new StringConverter<DomainId>());
			options.Converters.Add(new StringConverter<NamedId<DomainId>>());
			options.Converters.Add(new StringConverter<NamedId<Guid>>());
			options.Converters.Add(new StringConverter<NamedId<long>>());
			options.Converters.Add(new StringConverter<NamedId<string>>());
			options.Converters.Add(new StringConverter<Language>());
			options.Converters.Add(new StringConverter<PropertyPath>((string x) => x, null));
			options.Converters.Add(new StringConverter<RefToken>());
			options.Converters.Add(new StringConverter<Status>());
			options.Converters.Add(new JsonStringEnumConverter());
			options.IncludeFields = true;
			return options;
		}
	}
}