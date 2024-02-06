using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Generation.TypeMappers;
using NSwag.Generation;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using Squidex.Areas.Api.Controllers.Rules.Models;
using Squidex.Domain.Apps.Core.Schemas;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Config.OpenApi
{
	[Nullable(0)]
	[NullableContext(1)]
	public static class OpenApiServices
	{
		public static void AddSquidexOpenApiSettings(this IServiceCollection services)
		{
			DependencyInjectionExtensions.AddSingletonAs<ErrorDtoProcessor>(services).As<IDocumentProcessor>();
			DependencyInjectionExtensions.AddSingletonAs<RuleActionProcessor>(services).As<IDocumentProcessor>();
			DependencyInjectionExtensions.AddSingletonAs<CommonProcessor>(services).As<IDocumentProcessor>();
			DependencyInjectionExtensions.AddSingletonAs<XmlTagProcessor>(services).As<IDocumentProcessor>();
			DependencyInjectionExtensions.AddSingletonAs<SecurityProcessor>(services).As<IDocumentProcessor>();
			DependencyInjectionExtensions.AddSingletonAs<ScopesProcessor>(services).As<IOperationProcessor>();
			DependencyInjectionExtensions.AddSingletonAs<FixProcessor>(services).As<IOperationProcessor>();
			DependencyInjectionExtensions.AddSingletonAs<TagByGroupNameProcessor>(services).As<IOperationProcessor>();
			DependencyInjectionExtensions.AddSingletonAs<XmlResponseTypesProcessor>(services).As<IOperationProcessor>();
			DependencyInjectionExtensions.AddSingletonAs<JsonSchemaGenerator>(services).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<OpenApiSchemaGenerator>(services).AsSelf();
			ServiceCollectionServiceExtensions.AddSingleton<JsonSchemaGeneratorSettings>(services, (IServiceProvider c) => {
				JsonSchemaGeneratorSettings jsonSchemaGeneratorSetting = new JsonSchemaGeneratorSettings();
				OpenApiServices.ConfigureSchemaSettings(jsonSchemaGeneratorSetting, true);
				return jsonSchemaGeneratorSetting;
			});
			ServiceCollectionServiceExtensions.AddSingleton<OpenApiDocumentGeneratorSettings>(services, (IServiceProvider c) => {
				OpenApiDocumentGeneratorSettings openApiDocumentGeneratorSetting = new OpenApiDocumentGeneratorSettings();
				OpenApiServices.ConfigureSchemaSettings(openApiDocumentGeneratorSetting, true);
				foreach (IDocumentProcessor requiredService in ServiceProviderServiceExtensions.GetRequiredService<IEnumerable<IDocumentProcessor>>(c))
				{
					openApiDocumentGeneratorSetting.get_DocumentProcessors().Add(requiredService);
				}
				return openApiDocumentGeneratorSetting;
			});
			NSwagServiceCollectionExtensions.AddOpenApiDocument(services, (AspNetCoreOpenApiDocumentGeneratorSettings settings) => {
				settings.set_Title("Squidex API");
				OpenApiServices.ConfigureSchemaSettings(settings, false);
				settings.get_OperationProcessors().Add(new QueryParamsProcessor("/api/apps/{app}/assets"));
			});
		}

		private static void ConfigureSchemaSettings(JsonSchemaGeneratorSettings settings, bool flatten = false)
		{
			settings.set_AllowReferencesWithProperties(true);
			settings.set_ReflectionService(new ReflectionServices());
			JsonSchemaGeneratorSettings jsonSchemaGeneratorSetting = settings;
			List<ITypeMapper> typeMappers = new List<ITypeMapper>()
			{
				OpenApiServices.CreateStringMap<DomainId>(null),
				OpenApiServices.CreateStringMap<Instant>("date-time"),
				OpenApiServices.CreateStringMap<LocalDate>("date"),
				OpenApiServices.CreateStringMap<LocalDateTime>("date-time"),
				OpenApiServices.CreateStringMap<Language>(null),
				OpenApiServices.CreateStringMap<NamedId<DomainId>>(null),
				OpenApiServices.CreateStringMap<NamedId<Guid>>(null),
				OpenApiServices.CreateStringMap<NamedId<string>>(null),
				OpenApiServices.CreateStringMap<RefToken>(null),
				OpenApiServices.CreateStringMap<Status>(null),
				OpenApiServices.CreateObjectMap<JsonObject>(),
				OpenApiServices.CreateObjectMap<AssetMetadata>(),
				OpenApiServices.CreateAnyMap<JsonDocument>(),
				OpenApiServices.CreateAnyMap<JsonValue>(),
				OpenApiServices.CreateAnyMap<FilterNode<JsonValue>>(),
				new PrimitiveTypeMapper(typeof(FieldNames), (JsonSchema schema) => {
					schema.set_Type(1);
					JsonSchema jsonSchema = new JsonSchema();
					jsonSchema.set_Type(64);
					schema.set_Item(jsonSchema);
				})
			};
			jsonSchemaGeneratorSetting.set_TypeMappers(typeMappers);
			settings.set_SchemaType(2);
			settings.set_FlattenInheritanceHierarchy(flatten);
		}

		private static ITypeMapper CreateAnyMap<T>()
		{
			return new PrimitiveTypeMapper(typeof(T), (JsonSchema schema) => schema.set_Type(0));
		}

		private static ITypeMapper CreateObjectMap<T>()
		{
			return new PrimitiveTypeMapper(typeof(T), (JsonSchema schema) => {
				schema.set_Type(32);
				JsonSchema jsonSchema = new JsonSchema();
				jsonSchema.set_Description("Any");
				schema.set_AdditionalPropertiesSchema(jsonSchema);
			});
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		private static ITypeMapper CreateStringMap<T>(string format = null)
		{
			return new PrimitiveTypeMapper(typeof(T), (JsonSchema schema) => {
				schema.set_Type(64);
				schema.set_Format(format);
			});
		}
	}
}