using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.References;
using NSwag;
using NSwag.Generation;
using Squidex.Domain.Apps.Core;
using Squidex.Domain.Apps.Core.GenerateJsonSchema;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Infrastructure;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Contents.Generator
{
	[Nullable(0)]
	[NullableContext(1)]
	internal sealed class Builder
	{
		public string AppName
		{
			get;
		}

		public JsonSchema BulkRequestSchema
		{
			get;
		}

		public JsonSchema BulkResponseSchema
		{
			get;
		}

		public JsonSchema ChangeStatusSchema
		{
			get;
		}

		public NSwag.OpenApiDocument OpenApiDocument
		{
			get;
		}

		public NSwag.OpenApiSchemaResolver OpenApiSchemaResolver
		{
			get;
		}

		public JsonSchema QuerySchema
		{
			get;
		}

		internal Builder(IAppEntity app, NSwag.OpenApiDocument document, NSwag.OpenApiSchemaResolver schemaResolver, OpenApiSchemaGenerator schemaGenerator)
		{
			this.AppName = app.get_Name();
			this.OpenApiDocument = document;
			this.OpenApiSchemaResolver = schemaResolver;
			this.ChangeStatusSchema = Builder.CreateSchema<ChangeStatusDto>(schemaResolver, schemaGenerator);
			this.BulkRequestSchema = Builder.CreateSchema<BulkUpdateContentsDto>(schemaResolver, schemaGenerator);
			this.BulkResponseSchema = Builder.CreateSchema<BulkResultDto>(schemaResolver, schemaGenerator);
			this.QuerySchema = Builder.CreateSchema<QueryDto>(schemaResolver, schemaGenerator);
		}

		private static JsonSchema BuildResult(JsonSchema contentSchema)
		{
			JsonSchema jsonSchema = new JsonSchema();
			jsonSchema.set_AllowAdditionalProperties(false);
			jsonSchema.get_Properties()["total"] = JsonTypeBuilder.NumberProperty(FieldDescriptions.get_ContentsTotal(), true);
			jsonSchema.get_Properties()["items"] = JsonTypeBuilder.ArrayProperty(contentSchema, FieldDescriptions.get_ContentsItems(), true);
			jsonSchema.set_Type(32);
			return jsonSchema;
		}

		[return: Nullable(new byte[] { 0, 1, 2 })]
		private ValueTuple<JsonSchema, JsonSchema> CreateReference(string name)
		{
			JsonSchema jsonSchema;
			char upperInvariant = char.ToUpperInvariant(name[0]);
			string str = name;
			name = string.Concat(upperInvariant.ToString(), str.Substring(1, str.Length - 1));
			if (this.OpenApiDocument.get_Definitions().TryGetValue(name, out jsonSchema))
			{
				JsonSchema jsonSchema1 = new JsonSchema();
				jsonSchema1.set_Reference(jsonSchema);
				return new ValueTuple<JsonSchema, JsonSchema>(jsonSchema1, null);
			}
			jsonSchema = JsonTypeBuilder.Object();
			this.OpenApiDocument.get_Definitions().Add(name, jsonSchema);
			JsonSchema jsonSchema2 = new JsonSchema();
			jsonSchema2.set_Reference(jsonSchema);
			return new ValueTuple<JsonSchema, JsonSchema>(jsonSchema2, jsonSchema);
		}

		private static JsonSchema CreateSchema<T>(NSwag.OpenApiSchemaResolver schemaResolver, OpenApiSchemaGenerator schemaGenerator)
		{
			ContextualType contextualType = ContextualTypeExtensions.ToContextualType(typeof(T));
			return schemaGenerator.GenerateWithReference<JsonSchema>(contextualType, schemaResolver, null);
		}

		private JsonSchema RegisterReference(string name, Func<string, JsonSchema> creator)
		{
			char upperInvariant = char.ToUpperInvariant(name[0]);
			string str = name;
			name = string.Concat(upperInvariant.ToString(), str.Substring(1, str.Length - 1));
			JsonSchema orAdd = Squidex.Infrastructure.CollectionExtensions.GetOrAdd<string, JsonSchema>(this.OpenApiDocument.get_Definitions(), name, creator);
			JsonSchema jsonSchema = new JsonSchema();
			jsonSchema.set_Reference(orAdd);
			return jsonSchema;
		}

		public OperationsBuilder Schema(Schema schema, PartitionResolver partitionResolver, ResolvedComponents components, bool flat)
		{
			string str = SchemaExtensions.TypeName(schema);
			JsonSchema jsonSchema = this.RegisterReference(string.Concat(str, "DataDto"), (string _) => JsonSchemaExtensions.BuildJsonSchemaDynamic(schema, partitionResolver, components, new JsonTypeFactory(this, Builder.CreateReference), false, true));
			JsonSchema jsonSchema1 = jsonSchema;
			if (flat)
			{
				jsonSchema1 = this.RegisterReference(string.Concat(str, "FlatDataDto"), (string _) => JsonSchemaExtensions.BuildJsonSchemaFlat(schema, partitionResolver, components, new JsonTypeFactory(this, Builder.CreateReference), false, true));
			}
			JsonSchema jsonSchema2 = this.RegisterReference(string.Concat(str, "ContentDto"), (string _) => ContentJsonSchema.Build(jsonSchema1, true, false));
			JsonSchema jsonSchema3 = this.RegisterReference(string.Concat(str, "ContentResultDto"), (string _) => Builder.BuildResult(jsonSchema2));
			string str1 = string.Concat("/api/content/", this.AppName, "/", schema.get_Name());
			OperationsBuilder operationsBuilder = new OperationsBuilder()
			{
				ContentSchema = jsonSchema2,
				ContentsSchema = jsonSchema3,
				DataSchema = jsonSchema,
				Path = str1,
				Parent = this,
				SchemaDisplayName = SchemaExtensions.DisplayName(schema),
				SchemaName = schema.get_Name(),
				SchemaTypeName = str
			};
			operationsBuilder.AddTag("API endpoints for [schema] content items.");
			return operationsBuilder;
		}

		public OperationsBuilder Shared()
		{
			JsonSchema jsonSchema = this.RegisterReference("DataDto", (string _) => JsonSchema.CreateAnySchema());
			JsonSchema jsonSchema1 = this.RegisterReference("ContentDto", (string _) => ContentJsonSchema.Build(jsonSchema, true, false));
			JsonSchema jsonSchema2 = this.RegisterReference("ContentResultDto", (string _) => Builder.BuildResult(jsonSchema1));
			string str = string.Concat("/api/content/", this.AppName);
			OperationsBuilder operationsBuilder = new OperationsBuilder()
			{
				ContentSchema = jsonSchema1,
				ContentsSchema = jsonSchema2,
				DataSchema = jsonSchema,
				Path = str,
				Parent = this,
				SchemaDisplayName = "__Shared",
				SchemaName = "__Shared",
				SchemaTypeName = "__Shared"
			};
			operationsBuilder.AddTag("API endpoints for operations across all schemas.");
			return operationsBuilder;
		}
	}
}