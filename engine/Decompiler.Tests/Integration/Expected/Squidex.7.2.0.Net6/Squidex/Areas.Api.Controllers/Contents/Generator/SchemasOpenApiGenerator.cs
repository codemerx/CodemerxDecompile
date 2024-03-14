using Microsoft.AspNetCore.Http;
using NSwag;
using NSwag.Generation;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using Squidex.Domain.Apps.Core;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Domain.Apps.Entities;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Domain.Apps.Entities.Schemas;
using Squidex.Hosting;
using Squidex.Infrastructure.Caching;
using Squidex.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Contents.Generator
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class SchemasOpenApiGenerator
	{
		private readonly IAppProvider appProvider;

		private readonly OpenApiDocumentGeneratorSettings schemaSettings;

		private readonly OpenApiSchemaGenerator schemaGenerator;

		private readonly Squidex.Hosting.IUrlGenerator urlGenerator;

		private readonly IRequestCache requestCache;

		public SchemasOpenApiGenerator(IAppProvider appProvider, OpenApiDocumentGeneratorSettings schemaSettings, OpenApiSchemaGenerator schemaGenerator, Squidex.Hosting.IUrlGenerator urlGenerator, IRequestCache requestCache)
		{
			this.appProvider = appProvider;
			this.urlGenerator = urlGenerator;
			this.schemaSettings = schemaSettings;
			this.schemaGenerator = schemaGenerator;
			this.requestCache = requestCache;
		}

		private OpenApiDocument CreateApiDocument(HttpContext context, IAppEntity app)
		{
			string name = app.get_Name();
			OpenApiSchema openApiSchema = (string.Equals(context.get_Request().get_Scheme(), "http", StringComparison.OrdinalIgnoreCase) ? 1 : 2);
			OpenApiDocument openApiDocument = new OpenApiDocument();
			openApiDocument.set_Schemes(new List<OpenApiSchema>()
			{
				openApiSchema
			});
			openApiDocument.set_Consumes(new List<string>()
			{
				"application/json"
			});
			openApiDocument.set_Produces(new List<string>()
			{
				"application/json"
			});
			OpenApiInfo openApiInfo = new OpenApiInfo();
			openApiInfo.set_Title(string.Concat("Squidex Content API for '", name, "' App"));
			openApiInfo.set_Description(Resources.OpenApiContentDescription.Replace("[REDOC_LINK_NORMAL]", this.urlGenerator.BuildUrl(string.Concat("api/content/", app.get_Name(), "/docs"), true), StringComparison.Ordinal).Replace("[REDOC_LINK_SIMPLE]", this.urlGenerator.BuildUrl(string.Concat("api/content/", app.get_Name(), "/docs/flat"), true), StringComparison.Ordinal));
			openApiDocument.set_Info(openApiInfo);
			openApiDocument.set_SchemaType(2);
			OpenApiDocument openApiDocument1 = openApiDocument;
			if (!string.IsNullOrWhiteSpace(context.get_Request().get_Host().get_Value()))
			{
				HostString host = context.get_Request().get_Host();
				openApiDocument1.set_Host(host.get_Value());
			}
			return openApiDocument1;
		}

		public async Task<OpenApiDocument> GenerateAsync(HttpContext httpContext, IAppEntity app, IEnumerable<ISchemaEntity> schemas, bool flat)
		{
			SchemasOpenApiGenerator.u003cGenerateAsyncu003ed__6 variable = new SchemasOpenApiGenerator.u003cGenerateAsyncu003ed__6();
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<OpenApiDocument>.Create();
			variable.u003cu003e4__this = this;
			variable.httpContext = httpContext;
			variable.app = app;
			variable.schemas = schemas;
			variable.flat = flat;
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<SchemasOpenApiGenerator.u003cGenerateAsyncu003ed__6>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		private static void GenerateSchemaOperations(OperationsBuilder builder)
		{
			builder.AddOperation("get", "/").RequirePermission("squidex.apps.{app}.contents.{schema}.read.own").Operation("Query").OperationSummary("Query [schema] contents items.").Describe(Resources.OpenApiSchemaQuery).HasQueryOptions(true).Responds(200, "Content items retrieved.", builder.ContentsSchema).Responds(0x190, "Content query not valid.", null);
			builder.AddOperation("post", "/query").RequirePermission("squidex.apps.{app}.contents.{schema}.read.own").Operation("QueryPost").OperationSummary("Query [schema] contents items using Post.").HasBody("query", builder.Parent.QuerySchema, null).Responds(200, "Content items retrieved.", builder.ContentsSchema).Responds(0x190, "Content query not valid.", null);
			builder.AddOperation("get", "/{id}").RequirePermission("squidex.apps.{app}.contents.{schema}.read.own").Operation("Get").OperationSummary("Get a [schema] content item.").HasQuery("version", 16, FieldDescriptions.get_EntityVersion()).HasId().Responds(200, "Content item returned.", builder.ContentSchema);
			builder.AddOperation("get", "/{id}/{version}").RequirePermission("squidex.apps.{app}.contents.{schema}.read.own").Deprecated().Operation("GetVersioned").OperationSummary("Get a [schema] content item by id and version.").HasPath("version", 16, FieldDescriptions.get_EntityVersion(), null).HasId().Responds(200, "Content item returned.", builder.DataSchema);
			builder.AddOperation("get", "/{id}/validity").RequirePermission("squidex.apps.{app}.contents.{schema}.read.own").Operation("Validate").OperationSummary("Validates a [schema] content item.").HasId().Responds(200, "Content item is valid.", null).Responds(0x190, "Content item is not valid.", null);
			builder.AddOperation("post", "/").RequirePermission("squidex.apps.{app}.contents.{schema}.create").Operation("Create").OperationSummary("Create a [schema] content item.").HasQuery("publish", 2, FieldDescriptions.get_ContentRequestPublish()).HasQuery("id", 64, FieldDescriptions.get_ContentRequestOptionalId()).HasBody("data", builder.DataSchema, Resources.OpenApiSchemaBody).Responds(201, "Content item created", builder.ContentSchema).Responds(0x190, "Content data not valid.", null);
			builder.AddOperation("post", "/bulk").RequirePermission("squidex.apps.{app}.contents.{schema}.read.own").Operation("Bulk").OperationSummary("Bulk update content items.").HasBody("request", builder.Parent.BulkRequestSchema, null).Responds(200, "Contents created, update or delete.", builder.Parent.BulkResponseSchema).Responds(0x190, "Contents request not valid.", null);
			builder.AddOperation("post", "/{id}").RequirePermission("squidex.apps.{app}.contents.{schema}.upsert").Operation("Upsert").OperationSummary("Upsert a [schema] content item.").HasQuery("patch", 2, FieldDescriptions.get_ContentRequestPatch()).HasQuery("publish", 2, FieldDescriptions.get_ContentRequestPublish()).HasId().HasBody("data", builder.DataSchema, Resources.OpenApiSchemaBody).Responds(200, "Content item created or updated.", builder.ContentSchema).Responds(0x190, "Content data not valid.", null);
			builder.AddOperation("put", "/{id}").RequirePermission("squidex.apps.{app}.contents.{schema}.update.own").Operation("Update").OperationSummary("Update a [schema] content item.").HasId().HasBody("data", builder.DataSchema, Resources.OpenApiSchemaBody).Responds(200, "Content item updated.", builder.ContentSchema).Responds(0x190, "Content data not valid.", null);
			builder.AddOperation("patch", "/{id}").RequirePermission("squidex.apps.{app}.contents.{schema}.update.own").Operation("Patch").OperationSummary("Patch a [schema] content item.").HasId().HasBody("data", builder.DataSchema, Resources.OpenApiSchemaBody).Responds(200, "Content item updated.", builder.ContentSchema).Responds(0x190, "Content data not valid.", null);
			builder.AddOperation("put", "/{id}/status").RequirePermission("squidex.apps.{app}.contents.{schema}.changestatus.own").Operation("Change").OperationSummary("Change the status of a [schema] content item.").HasId().HasBody("request", builder.Parent.ChangeStatusSchema, "The request to change content status.").Responds(200, "Content status updated.", builder.ContentSchema).Responds(0x190, "Content status not valid.", null);
			builder.AddOperation("delete", "/{id}").RequirePermission("squidex.apps.{app}.contents.{schema}.delete.own").Operation("Delete").OperationSummary("Delete a [schema] content item.").HasQuery("permanent", 2, FieldDescriptions.get_EntityRequestDeletePermanent()).HasId().Responds(204, "Content item deleted", null);
		}

		private static void GenerateSharedOperations(OperationsBuilder builder)
		{
			builder.AddOperation("get", "/").RequirePermission("squidex.apps.{app}.contents.{schema}.read.own").Operation("Query").OperationSummary("Query contents across all schemas.").HasQuery("ids", 64, "Comma-separated list of content IDs.").Responds(200, "Content items retrieved.", builder.ContentsSchema).Responds(0x190, "Query not valid.", null);
			builder.AddOperation("post", "/bulk").RequirePermission("squidex.apps.{app}.contents.{schema}.read.own").Operation("Bulk").OperationSummary("Bulk update content items across all schemas.").HasBody("request", builder.Parent.BulkRequestSchema, null).Responds(200, "Contents created, update or delete.", builder.Parent.BulkResponseSchema).Responds(0x190, "Contents request not valid.", null);
		}
	}
}