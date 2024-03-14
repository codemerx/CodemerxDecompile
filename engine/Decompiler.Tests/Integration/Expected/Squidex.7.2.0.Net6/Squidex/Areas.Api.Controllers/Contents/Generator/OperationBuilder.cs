using NJsonSchema;
using NSwag;
using Squidex.Areas.Api.Config.OpenApi;
using Squidex.Domain.Apps.Core;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Security;
using Squidex.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Contents.Generator
{
	[Nullable(0)]
	[NullableContext(1)]
	internal sealed class OperationBuilder
	{
		private readonly OpenApiOperation operation = new OpenApiOperation();

		private readonly OperationsBuilder operations;

		public OperationBuilder(OperationsBuilder operations, OpenApiOperation operation)
		{
			this.operations = operations;
			this.operation = operation;
		}

		private OperationBuilder AddParameter(string name, JsonSchema schema, OpenApiParameterKind kind, [Nullable(2)] string description)
		{
			OpenApiParameter openApiParameter = new OpenApiParameter();
			openApiParameter.set_Kind(kind);
			openApiParameter.set_Schema(schema);
			openApiParameter.set_Name(name);
			OpenApiParameter openApiParameter1 = openApiParameter;
			if (!string.IsNullOrWhiteSpace(description))
			{
				openApiParameter1.set_Description(this.operations.FormatText(description));
			}
			if (kind != 2)
			{
				openApiParameter1.set_IsRequired(true);
				openApiParameter1.set_IsNullableRaw(new bool?(false));
			}
			this.operation.get_Parameters().Add(openApiParameter1);
			return this;
		}

		public OperationBuilder Deprecated()
		{
			this.operation.set_IsDeprecated(true);
			return this;
		}

		public OperationBuilder Describe(string description)
		{
			if (!string.IsNullOrWhiteSpace(description))
			{
				this.operation.set_Description(description);
			}
			return this;
		}

		public OperationBuilder HasBody(string name, JsonSchema schema, [Nullable(2)] string description = null)
		{
			return this.AddParameter(name, schema, 1, description);
		}

		public OperationBuilder HasId()
		{
			this.HasPath("id", 64, FieldDescriptions.get_EntityId(), null);
			this.Responds(0x194, "Content item not found.", null);
			return this;
		}

		public OperationBuilder HasPath(string name, JsonObjectType type, string description, [Nullable(2)] string format = null)
		{
			JsonSchema jsonSchema = new JsonSchema();
			jsonSchema.set_Type(type);
			jsonSchema.set_Format(format);
			return this.AddParameter(name, jsonSchema, 3, description);
		}

		public OperationBuilder HasQuery(string name, JsonObjectType type, string description)
		{
			JsonSchema jsonSchema = new JsonSchema();
			jsonSchema.set_Type(type);
			return this.AddParameter(name, jsonSchema, 2, description);
		}

		public OperationBuilder HasQueryOptions(bool supportSearch)
		{
			QueryExtensions.AddQuery(this.operation, true);
			return this;
		}

		public OperationBuilder Operation(string name)
		{
			this.operation.set_OperationId(string.Concat(name, this.operations.SchemaTypeName, "Content"));
			return this;
		}

		public OperationBuilder OperationSummary(string summary)
		{
			if (!string.IsNullOrWhiteSpace(summary))
			{
				this.operation.set_Summary(this.operations.FormatText(summary));
			}
			return this;
		}

		public OperationBuilder RequirePermission(string permissionId)
		{
			string id = PermissionIds.ForApp(permissionId, this.operations.Parent.AppName, this.operations.SchemaName, "*").get_Id();
			OpenApiOperation openApiOperation = this.operation;
			List<OpenApiSecurityRequirement> openApiSecurityRequirements = new List<OpenApiSecurityRequirement>();
			OpenApiSecurityRequirement openApiSecurityRequirement = new OpenApiSecurityRequirement();
			openApiSecurityRequirement["squidex-oauth-auth"] = new string[] { id };
			openApiSecurityRequirements.Add(openApiSecurityRequirement);
			openApiOperation.set_Security(openApiSecurityRequirements);
			return this;
		}

		public OperationBuilder Responds(int statusCode, string description, [Nullable(2)] JsonSchema schema = null)
		{
			OpenApiResponse openApiResponse = new OpenApiResponse();
			openApiResponse.set_Description(description);
			OpenApiResponse openApiResponse1 = openApiResponse;
			if (schema != null && statusCode == 204)
			{
				Squidex.Infrastructure.ThrowHelper.ArgumentException("Invalid status code.", "statusCode");
			}
			openApiResponse1.set_Schema(schema);
			this.operation.get_Responses().Add(statusCode.ToString(CultureInfo.InvariantCulture), openApiResponse1);
			return this;
		}
	}
}