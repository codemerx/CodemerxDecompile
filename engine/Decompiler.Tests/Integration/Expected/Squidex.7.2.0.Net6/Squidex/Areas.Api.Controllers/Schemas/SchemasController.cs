using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using NSwag.Annotations;
using Squidex.Areas.Api.Controllers.Schemas.Models;
using Squidex.Domain.Apps.Core.GenerateFilters;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Domain.Apps.Core.Scripting;
using Squidex.Domain.Apps.Entities;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Domain.Apps.Entities.Schemas;
using Squidex.Domain.Apps.Entities.Schemas.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Infrastructure.Queries;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Schemas
{
	[ApiExplorerSettings(GroupName="Schemas")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class SchemasController : ApiController
	{
		private readonly IAppProvider appProvider;

		public SchemasController(ICommandBus commandBus, IAppProvider appProvider) : base(commandBus)
		{
			this.appProvider = appProvider;
		}

		private async Task<FilterSchema> BuildModel()
		{
			ResolvedComponents componentsAsync = await AppProviderExtensions.GetComponentsAsync(this.appProvider, base.get_Schema(), base.get_HttpContext().get_RequestAborted());
			FilterSchema filterSchema = FilterExtensions.BuildDataSchema(base.get_Schema().get_SchemaDef(), AppEntityExtensions.PartitionResolver(base.get_App()), componentsAsync);
			return filterSchema;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.delete" })]
		[HttpDelete]
		[Route("apps/{app}/schemas/{schema}/")]
		public async Task<IActionResult> DeleteSchema(string app, string schema)
		{
			DeleteSchema deleteSchema = new DeleteSchema();
			await base.get_CommandBus().PublishAsync(deleteSchema, base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] {  })]
		[HttpGet]
		[OpenApiIgnore]
		[Route("apps/{app}/schemas/{schema}/filters")]
		public async Task<IActionResult> GetFilters(string app, string schema)
		{
			ResolvedComponents componentsAsync = await AppProviderExtensions.GetComponentsAsync(this.appProvider, base.get_Schema(), base.get_HttpContext().get_RequestAborted());
			QueryModel queryModel = ContentQueryModel.Build(base.get_Schema().get_SchemaDef(), AppEntityExtensions.PartitionResolver(base.get_App()), componentsAsync).Flatten(7, true);
			return this.Ok(queryModel);
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/")]
		public IActionResult GetSchema(string app, string schema)
		{
			Deferred deferred = Deferred.Response(() => SchemaDto.FromDomain(base.get_Schema(), base.get_Resources()));
			base.get_Response().get_Headers().set_Item(HeaderNames.ETag, ETagExtensions.ToEtag<ISchemaEntity>(base.get_Schema()));
			return this.Ok(deferred);
		}

		[return: Nullable(new byte[] { 1, 2 })]
		private Task<ISchemaEntity> GetSchemaAsync(string schema)
		{
			Guid guid;
			if (!Guid.TryParse(schema, out guid))
			{
				return this.appProvider.GetSchemaAsync(base.get_AppId(), schema, false, base.get_HttpContext().get_RequestAborted());
			}
			DomainId domainId = DomainId.Create(guid);
			return this.appProvider.GetSchemaAsync(base.get_AppId(), domainId, false, base.get_HttpContext().get_RequestAborted());
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(SchemasDto), 200)]
		[Route("apps/{app}/schemas/")]
		public async Task<IActionResult> GetSchemas(string app)
		{
			List<ISchemaEntity> schemasAsync = await this.appProvider.GetSchemasAsync(base.get_AppId(), base.get_HttpContext().get_RequestAborted());
			List<ISchemaEntity> schemaEntities = schemasAsync;
			Deferred deferred = Deferred.Response(() => SchemasDto.FromDomain(schemaEntities, base.get_Resources()));
			base.get_Response().get_Headers().set_Item(HeaderNames.ETag, ETagExtensions.ToEtag<ISchemaEntity>(schemaEntities));
			IActionResult actionResult = this.Ok(deferred);
			return actionResult;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] {  })]
		[HttpGet]
		[OpenApiIgnore]
		[Route("apps/{app}/schemas/{schema}/completion")]
		public async Task<IActionResult> GetScriptCompletion(string app, string schema, [FromServices] ScriptingCompleter completer)
		{
			ScriptingCompleter scriptingCompleter = completer;
			IReadOnlyList<ScriptingValue> scriptingValues = scriptingCompleter.ContentScript(await this.BuildModel());
			scriptingCompleter = null;
			return this.Ok(scriptingValues);
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] {  })]
		[HttpGet]
		[OpenApiIgnore]
		[Route("apps/{app}/schemas/{schema}/completion/triggers")]
		public async Task<IActionResult> GetScriptTriggerCompletion(string app, string schema, [FromServices] ScriptingCompleter completer)
		{
			ScriptingCompleter scriptingCompleter = completer;
			IReadOnlyList<ScriptingValue> scriptingValues = scriptingCompleter.ContentTrigger(await this.BuildModel());
			scriptingCompleter = null;
			return this.Ok(scriptingValues);
		}

		private async Task<SchemaDto> InvokeCommandAsync(ICommand command)
		{
			ISchemaEntity schemaEntity = await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted()).Result<ISchemaEntity>();
			return SchemaDto.FromDomain(schemaEntity, base.get_Resources());
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.create" })]
		[HttpPost]
		[ProducesResponseType(typeof(SchemaDto), 201)]
		[Route("apps/{app}/schemas/")]
		public async Task<IActionResult> PostSchema(string app, [FromBody] CreateSchemaDto request)
		{
			SchemaDto schemaDto = await this.InvokeCommandAsync(request.ToCommand());
			IActionResult actionResult = this.CreatedAtAction("GetSchema", new { app = app, schema = request.Name }, schemaDto);
			return actionResult;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.publish" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/publish/")]
		public async Task<IActionResult> PublishSchema(string app, string schema)
		{
			IActionResult actionResult = this.Ok(await this.InvokeCommandAsync(new PublishSchema()));
			return actionResult;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/category")]
		public async Task<IActionResult> PutCategory(string app, string schema, [FromBody] ChangeCategoryDto request)
		{
			ChangeCategory command = request.ToCommand();
			return this.Ok(await this.InvokeCommandAsync(command));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/preview-urls")]
		public async Task<IActionResult> PutPreviewUrls(string app, string schema, [FromBody] ConfigurePreviewUrlsDto request)
		{
			ConfigurePreviewUrls command = request.ToCommand();
			return this.Ok(await this.InvokeCommandAsync(command));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/rules/")]
		public async Task<IActionResult> PutRules(string app, string schema, [FromBody] ConfigureFieldRulesDto request)
		{
			ConfigureFieldRules command = request.ToCommand();
			return this.Ok(await this.InvokeCommandAsync(command));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/")]
		public async Task<IActionResult> PutSchema(string app, string schema, [FromBody] UpdateSchemaDto request)
		{
			UpdateSchema command = request.ToCommand();
			return this.Ok(await this.InvokeCommandAsync(command));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/sync")]
		public async Task<IActionResult> PutSchemaSync(string app, string schema, [FromBody] SynchronizeSchemaDto request)
		{
			SynchronizeSchema command = request.ToCommand();
			return this.Ok(await this.InvokeCommandAsync(command));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.scripts" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/scripts/")]
		public async Task<IActionResult> PutScripts(string app, string schema, [FromBody] SchemaScriptsDto request)
		{
			ConfigureScripts command = request.ToCommand();
			return this.Ok(await this.InvokeCommandAsync(command));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.publish" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/unpublish/")]
		public async Task<IActionResult> UnpublishSchema(string app, string schema)
		{
			IActionResult actionResult = this.Ok(await this.InvokeCommandAsync(new UnpublishSchema()));
			return actionResult;
		}
	}
}